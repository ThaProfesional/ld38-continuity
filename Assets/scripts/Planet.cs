using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Planet : MonoBehaviour {
    const float BOUNCE_MODIFIER = 2F;
    const float BOUNCE_ENTROPY_MINIMUM = 0.8F;
    const float BOUNCE_THRESHOLD = 0.5F;

    const float START_VELOCITY = 0.1F;

    const float ROTATIONAL_ANGLE = 90F;

    private List<Vector2> _velocities;
    public Vector2 Velocity {
        get {
            var v = new Vector2();

            foreach (var velocity in _velocities)
                v += velocity;

            return v;
        }
    }

    void Start () {
        _velocities = new List<Vector2>();

        SetInitialVelocity();
    }

    void FixedUpdate() {
        _velocities = _velocities.Select(x => {
            var entropy = x / 20;

            if (entropy.magnitude < BOUNCE_ENTROPY_MINIMUM)
                entropy = entropy.normalized * BOUNCE_ENTROPY_MINIMUM;

            return x - entropy;
        }).ToList();

        SetVelocity();

        _velocities.RemoveAll(x => x.magnitude < BOUNCE_THRESHOLD);
    }

    void OnCollisionEnter2D(Collision2D other) {
        var planet = other.gameObject;

        var blackHoleComponent = planet.GetComponent<BlackHole>();

        if (blackHoleComponent == null) {
            var gravityComponent = GetComponent<Gravity>();
            var planetGravityComponent = planet.GetComponent<Gravity>();

            var mass = gravityComponent.Mass;
            var planetMass = planetGravityComponent.Mass;

            var massModifier = planetMass / (mass + planetMass);

            var v = other.relativeVelocity * massModifier * BOUNCE_MODIFIER;

            _velocities.Add(v);
        }
    }

    private void SetInitialVelocity() {
        var blackHoleObject = GameObject.Find("Black Hole");
        var d = (Vector2)(blackHoleObject.transform.position - transform.position);
        var dn = d.normalized;

        var a = Random.Range(0, 1) == 1
            ? ROTATIONAL_ANGLE
            : ROTATIONAL_ANGLE * -1;

        dn = dn.Rotate(a);

        var rigidComponent = GetComponent<Rigidbody2D>();

        _velocities.Add(dn);
    }

    private void SetVelocity() {
        var velocity = Velocity;

        var playerComponent = GetComponent<Player>();

        if (playerComponent != null)
            velocity += playerComponent.Velocity;

        var gravityComponent = GetComponent<Gravity>();

        if (gravityComponent != null)
            velocity += gravityComponent.Velocity;

        var rigidComponent = GetComponent<Rigidbody2D>();

        rigidComponent.velocity = velocity;
    }
}
