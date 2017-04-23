using UnityEngine;

public class Planet : MonoBehaviour {
    const float BOUNCE_MODIFIER = 15F;
    const float BOUNCE_ENTROPY_MINIMUM = 0.4F;
    const float BOUNCE_THRESHOLD = 0.8F;

    const float MINIMUM_BOUNCE = 1F;

    const float START_VELOCITY = 0.1F;

    const float ROTATIONAL_ANGLE = 90F;

    const float ENTROPY_RATE = 0.35F;

    public Vector2 Velocity;

    private Rigidbody2D _rigid;

    private void Awake() {
        _rigid = GetComponent<Rigidbody2D>();

        Velocity = new Vector2();
    }

    void FixedUpdate() {
        var entropy = Velocity * ENTROPY_RATE;

        if (entropy.magnitude < BOUNCE_ENTROPY_MINIMUM)
            entropy = entropy.normalized * BOUNCE_ENTROPY_MINIMUM;

        Velocity -= entropy;

        if (Velocity.magnitude <= BOUNCE_THRESHOLD)
            Velocity = new Vector2(0, 0);

        if (OutOfBounds())
            GetBackInBounds();

        SetVelocity();
    }

    void OnCollisionEnter2D(Collision2D other) {
        var planet = other.gameObject;

        var blackHoleComponent = planet.GetComponent<BlackHole>();

        if (blackHoleComponent == null) {
            var v = new Vector2();

            if (other.relativeVelocity.x != 0 || other.relativeVelocity.y != 0) {
                var gravityComponent = GetComponent<Gravity>();
                var planetGravityComponent = planet.GetComponent<Gravity>();

                var mass = gravityComponent.Mass;
                var planetMass = planetGravityComponent.Mass;

                var massModifier = planetMass / (mass + planetMass);

                v = other.relativeVelocity * massModifier * BOUNCE_MODIFIER;
            }

            if (v.magnitude < MINIMUM_BOUNCE)
                v = v.normalized * MINIMUM_BOUNCE;

            Velocity += v;
        }
    }

    private void SetVelocity() {
        var velocity = Velocity;

        var playerComponent = GetComponent<Player>();

        if (playerComponent != null)
            velocity += playerComponent.Velocity;

        var gravityComponent = GetComponent<Gravity>();

        if (gravityComponent != null)
            velocity += gravityComponent.Velocity;

        _rigid.velocity = velocity;
    }

    private bool OutOfBounds() {
        return gameObject.transform.position.x > HolerSystem.BOUNDS
            || gameObject.transform.position.y > HolerSystem.BOUNDS;
    }

    private void GetBackInBounds() {
        var newPosition = gameObject.transform.position.normalized * (HolerSystem.BOUNDS - 1);
        gameObject.transform.position = newPosition;

        Velocity = _rigid.velocity * -1;
    }
}
