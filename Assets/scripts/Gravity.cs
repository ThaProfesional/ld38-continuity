using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Gravity : MonoBehaviour {

    const float G = 1F;
    const float THRESHOLD = 0.1F;

    const float VELOCITY_CAP = 1F;

    const float GRAVITY_MODIFIER = 0.05F;
    const float GRAVITY_MODIFIER_MODIFIER = 0.00001F;

    const float ROTATIONAL_MODIFIER_SPARSE = 0.2F;
    const float ROTATIONAL_MODIFIER_DENSE = 1F;

    const float MASS_LOWER_BOUNDS = 10;
    const float MASS_UPPER_BOUNDS = 100;

    public float Mass;

    void Start () {
        if(Mass == 0) {
            var rigidComponent = GetComponent<Rigidbody2D>();

            Mass = rigidComponent.mass;
        } 
    }
	
	void Update () {
		
	}

    void FixedUpdate() {
        var rigidComponent = GetComponent<Rigidbody2D>();

        var _gravitationalVelocity = new Vector2();

        if (rigidComponent != null) {
            var bodies = GetPlanetaryBodies();

            foreach (var body in bodies) {
                if (rigidComponent != null)
                {
                    var d = (Vector2)(body.transform.position - transform.position);
                    var dm = d.magnitude;
                    var dn = d.normalized;

                    var f = GetGravtiationalForce(body.Mass, dm);

                    f = PerpetuateVelocity(f, rigidComponent);

                    if (f > THRESHOLD) {
                        _gravitationalVelocity += CalculateGravitationalVelocity(dn, f, body.Mass);
                    }
                }
            }
        }

        rigidComponent.velocity = _gravitationalVelocity;
    }

    private IList<Gravity> GetPlanetaryBodies() {
        return FindObjectsOfType<Gravity>()
            .Where(x => x != this)
            .ToList();
    }

    private float GetGravtiationalForce(float bodyMass, float dm) {
        return G * ((bodyMass / Mass) / (dm * dm));
    }

    private float PerpetuateVelocity(float f, Rigidbody2D rigidComponent) {
        var gvm = rigidComponent.velocity.magnitude;

        if (gvm > VELOCITY_CAP)
            gvm = VELOCITY_CAP;

        return f + gvm;
    }

    private Vector2 CalculateGravitationalVelocity(Vector2 dn, float f, float bodyMass) {
        var _gravitationalVelocity = new Vector2();

        var gvm = _gravitationalVelocity.magnitude;

        if (gvm > VELOCITY_CAP)
            gvm = VELOCITY_CAP;

        var gravity = dn * (f + gvm);

        // TODO: Change this to be affected by angle of approach
        var rotation = gravity.Rotate(-90F);

        if (Mass < bodyMass) {
            _gravitationalVelocity += gravity * CalculateGravityMultiplier(bodyMass);
            _gravitationalVelocity += rotation * ROTATIONAL_MODIFIER_DENSE;
        } else {
            _gravitationalVelocity += rotation * ROTATIONAL_MODIFIER_SPARSE;
        }

        return _gravitationalVelocity;
    }

    private float CalculateGravityMultiplier(float bodyMass) {
        return GRAVITY_MODIFIER + (bodyMass * GRAVITY_MODIFIER_MODIFIER);
    }
}