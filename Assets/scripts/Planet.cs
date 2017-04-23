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

    void Awake() {
        _velocities = new List<Vector2>();
    }

    void FixedUpdate() {
        _velocities = _velocities.Select(x => {
            var entropy = x / 20;

            if (entropy.magnitude < BOUNCE_ENTROPY_MINIMUM)
                entropy = entropy.normalized * BOUNCE_ENTROPY_MINIMUM;

            return x - entropy;
        }).ToList();

        _velocities.RemoveAll(x => x.magnitude < BOUNCE_THRESHOLD);

        SetVelocity();
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

    public void PushVelocity(Vector2 v) {
        _velocities.Add(v);
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
