using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Gravity : MonoBehaviour {
    const float G = 1F;
    const float THRESHOLD = 0.1F;

    const float VELOCITY_CAP = 1F;

    const float EVENT_HORIZON_VELOCITY = 3F;

    const float GRAVITY_MODIFIER_GOOD_ANGLE = 0.05F;
    const float GRAVITY_MODIFIER_BAD_ANGLE = 1F;
    const float GRAVITY_MODIFIER_MASS_MODIFIER = 0.00001F;

    const float ROTATIONAL_MODIFIER_SPARSE = 0.2F;
    const float ROTATIONAL_MODIFIER_DENSE = 1F;

    const float ROTATIONAL_CUTOFF_ANGLE = 35f;

    const float ROTATIONAL_ANGLE = 90f;

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

                var v = rigidComponent.velocity;

                gravitationalVelocity += CalculateGravitationalVelocity(v, dn, f, body.Mass);
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

    private Vector2 CalculateGravitationalVelocity(Vector2 v, Vector2 dn, float f, float bodyMass) {
        var gravity = GetGravity(dn, f);

        var a = Vector2.Angle(v, dn);

        var cross = Vector3.Cross(v, dn);

        if (cross.z > 0)
            a *= -1;

        return ShouldCrash(v, a)
            ? GetGravitationalVelocity(bodyMass, gravity)
            : GetRotationalVelocity(bodyMass, gravity, a);
    }

    private Vector2 GetGravity(Vector2 dn, float f) {
        var gvm = _velocity.magnitude;

        if (gvm > VELOCITY_CAP)
            gvm = VELOCITY_CAP;

        return dn * (f + gvm);
    }

    private bool ShouldCrash(Vector2 v, float a) {
        if (v.x == 0 && v.y == 0)
            return true;

        return (a < ROTATIONAL_CUTOFF_ANGLE && a > (ROTATIONAL_CUTOFF_ANGLE * -1));
    }

    private Vector2 GetGravitationalVelocity(float bodyMass, Vector2 gravity) {
        return gravity * GRAVITY_MODIFIER_BAD_ANGLE;
    }

    private Vector2 GetRotationalVelocity(float bodyMass, Vector2 gravity, float a) {
        var rotationalVelocity = new Vector2();

        var rotation = (a < 0)
            ? gravity.Rotate(ROTATIONAL_ANGLE * -1)
            : gravity.Rotate(ROTATIONAL_ANGLE);

        if (Mass < bodyMass) {
            rotationalVelocity += gravity * CalculateGoodGravityMultiplier(bodyMass);
            rotationalVelocity += rotation * ROTATIONAL_MODIFIER_DENSE;
        }
        else {
            rotationalVelocity += rotation * ROTATIONAL_MODIFIER_SPARSE;
        }

        return rotationalVelocity;
    }

    private float CalculateGoodGravityMultiplier(float bodyMass) {
        return GRAVITY_MODIFIER_GOOD_ANGLE + (bodyMass * GRAVITY_MODIFIER_MASS_MODIFIER);
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