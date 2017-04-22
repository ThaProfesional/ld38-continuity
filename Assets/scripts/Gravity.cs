using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Gravity : MonoBehaviour {
    const float G = 1F;
    const float THRESHOLD = 0.1F;

    const float VELOCITY_CAP = 1F;

    const float EVENT_HORIZON_VELOCITY = 3F;

    const float GRAVITY_MODIFIER = 0.05F;
    const float GRAVITY_MODIFIER_MODIFIER = 0.00001F;

    const float ROTATIONAL_MODIFIER_SPARSE = 0.2F;
    const float ROTATIONAL_MODIFIER_DENSE = 1F;

    const float MASS_LOWER_BOUNDS = 10;
    const float MASS_UPPER_BOUNDS = 100;

    public bool IsDoomed;
    public float Mass;

    private Vector2 _velocity;
    public Vector2 Velocity {
        get {
            return _velocity;
        }
    }

    void Start () {
        if(Mass == 0) {
            var rigidComponent = GetComponent<Rigidbody2D>();

            Mass = rigidComponent.mass;
        } 
    }

    void FixedUpdate() {
        var rigidComponent = GetComponent<Rigidbody2D>();

        if (rigidComponent != null) {
            _velocity = IsDoomed
                ? GetDoomedVelocity(rigidComponent)
                : GetVelocity(rigidComponent);

            SetVelocity();
        }
    }

    private Vector2 GetDoomedVelocity(Rigidbody2D rigidComponent) {
        var gravitationalVelocity = new Vector2();

        var blackHole = GetBlackHoleObject();

        if (blackHole != null) {
            var d = GetDisplacement(blackHole.transform.position);
            var dm = d.magnitude;
            var dn = d.normalized;

            var blackHoleGravityComponent = blackHole.GetComponent<Gravity>();

            var f = GetGravtiationalForce(blackHoleGravityComponent.Mass, dm);

            gravitationalVelocity = dn * (f + EVENT_HORIZON_VELOCITY);
        }

        return gravitationalVelocity;
    }

    private Vector2 GetVelocity(Rigidbody2D rigidComponent) {
        var gravitationalVelocity = new Vector2();

        var gravities = GetGravities();

        foreach (var body in gravities) {
            var d = GetDisplacement(body.transform.position);
            var dm = d.magnitude;
            var dn = d.normalized;

            var f = GetGravtiationalForce(body.Mass, dm);

            if (f > THRESHOLD) {
                f = PerpetuateVelocity(f, rigidComponent);

                gravitationalVelocity += CalculateGravitationalVelocity(dn, f, body.Mass);
            }
        }

        return gravitationalVelocity;
    }

    private GameObject GetBlackHoleObject() {
        var blackHoleComponent = FindObjectOfType<BlackHole>();

        return blackHoleComponent != null
            ? blackHoleComponent.gameObject
            : null;
    }

    private IList<Gravity> GetGravities() {
        return FindObjectsOfType<Gravity>()
            .Where(x => x != this)
            .ToList();
    }

    private Vector2 GetDisplacement(Vector2 bodyPosition) {
        return bodyPosition - (Vector2)transform.position;
    }

    private float GetGravtiationalForce(float bodyMass, float dm) {
        return G * ((bodyMass / Mass) / (dm * dm));
    }

    private float PerpetuateVelocity(float f, Rigidbody2D rigidComponent) {
        var gvm = _velocity.magnitude;

        if (gvm > VELOCITY_CAP)
            gvm = VELOCITY_CAP;

        return f + gvm;
    }

    private Vector2 CalculateGravitationalVelocity(Vector2 dn, float f, float bodyMass) {
        var gravitationalVelocity = new Vector2();

        var gvm = _velocity.magnitude;

        if (gvm > VELOCITY_CAP)
            gvm = VELOCITY_CAP;

        var gravity = dn * (f + gvm);

        // TODO: Change this to be affected by angle of approach
        var rotation = gravity.Rotate(-90F);

        if (Mass < bodyMass) {
            gravitationalVelocity += gravity * CalculateGravityMultiplier(bodyMass);
            gravitationalVelocity += rotation * ROTATIONAL_MODIFIER_DENSE;
        } else {
            gravitationalVelocity += rotation * ROTATIONAL_MODIFIER_SPARSE;
        }

        return gravitationalVelocity;
    }

    private float CalculateGravityMultiplier(float bodyMass) {
        return GRAVITY_MODIFIER + (bodyMass * GRAVITY_MODIFIER_MODIFIER);
    }

    private void SetVelocity() {
        var velocity = _velocity;

        var playerComponent = GetComponent<Player>();

        if (playerComponent != null)
            velocity += playerComponent.Velocity;

        var rigidComponent = GetComponent<Rigidbody2D>();

        rigidComponent.velocity = velocity;
    }
}