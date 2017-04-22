using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Gravity : MonoBehaviour {

    const float G = 1F;
    const float THRESHOLD = 0.1F;

    const float VELOCITY_CAP = 2F;

    const float GRAVITY_MODIFIER = 0.05F;
    const float ROTATIONAL_MODIFIER_SPARSE = 0.2F;
    const float ROTATIONAL_MODIFIER_DENSE = 1F;

    const float MASS_LOWER_BOUNDS = 10;
    const float MASS_UPPER_BOUNDS = 100;

    private Vector2 _gravitationalVelocity;

    private float _mass;
    public float Mass;

    void Start () {
        _mass = Mass;

        if (_mass == 0) {
            _mass = Random.Range(MASS_LOWER_BOUNDS, MASS_UPPER_BOUNDS);
        }
    }
	
	void Update () {
		
	}

    void FixedUpdate() {
        var bodies = GetPlanetaryBodies();

        foreach (var body in bodies) {
            var d = (Vector2)(transform.position - body.transform.position);
            var dm = d.magnitude;
            var dn = d.normalized;

            var f = GetGravtiationalForce(body.Mass, dm);

            if (f > THRESHOLD) {
                var rigidComponent = GetRigidbody2dComponent(body);

                if (rigidComponent != null) {
                    CalculateGravitationalVelocity(dn, f, Mass, body.Mass);

                    rigidComponent.velocity = _gravitationalVelocity;
                }
            }
        }
    }

    private IList<Gravity> GetPlanetaryBodies() {
        return FindObjectsOfType<Gravity>()
            .Where(x => x != this)
            .ToList();
    }

    private float GetGravtiationalForce(float bodyMass, float dm) {
        return G * ((Mass / bodyMass) / (dm * dm));
    }

    private Rigidbody2D GetRigidbody2dComponent(Gravity body) {
        var bodyObject = body.gameObject;
        return bodyObject.GetComponent<Rigidbody2D>();
    }

    private void CalculateGravitationalVelocity(Vector2 dn, float f, float mass1, float mass2) {
        var gvm = _gravitationalVelocity.magnitude;

        if (gvm > VELOCITY_CAP)
            gvm = VELOCITY_CAP;

        var gravity = dn * (f + gvm);

        var rotation = gravity.Rotate(-90F);

        if (mass1 > mass2) {
            _gravitationalVelocity = gravity * GRAVITY_MODIFIER;
            _gravitationalVelocity += rotation * ROTATIONAL_MODIFIER_DENSE;
        } else {
            _gravitationalVelocity = rotation * ROTATIONAL_MODIFIER_SPARSE;
        }
    }
}