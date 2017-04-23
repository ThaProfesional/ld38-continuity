using UnityEngine;

public class Planet : MonoBehaviour {
    const float BOUNCE_MODIFIER = 15F;
    const float BOUNCE_ENTROPY_MINIMUM = 0.4F;
    const float BOUNCE_THRESHOLD = 0.8F;

    const float MINIMUM_BOUNCE = 1F;
    const float MAXIMUM_BOUNCE = 5F;

    const float START_VELOCITY = 0.1F;

    const float ROTATIONAL_ANGLE = 90F;

    const float ENTROPY_RATE = 0.35F;

    public Vector2 Velocity;

    private AudioSource _audio;
    private Gravity _gravity;
    private Rigidbody2D _rigid;

    private void Awake() {
        _audio = GetComponent<AudioSource>();
        _gravity = GetComponent<Gravity>();
        _rigid = GetComponent<Rigidbody2D>();

        Velocity = new Vector2();
    }

    void FixedUpdate() {
        if (OutOfBounds())
            GetBackInBounds();

        var entropy = Velocity * ENTROPY_RATE;

        if (entropy.magnitude < BOUNCE_ENTROPY_MINIMUM)
            entropy = entropy.normalized * BOUNCE_ENTROPY_MINIMUM;

        Velocity -= entropy;

        if (Velocity.magnitude <= BOUNCE_THRESHOLD)
            Velocity = new Vector2(0, 0);

        SetVelocity();
    }

    void OnCollisionEnter2D(Collision2D other) {
        var planet = other.gameObject;

        var blackHoleComponent = planet.GetComponent<BlackHole>();

        if (blackHoleComponent == null) {
            var v = new Vector2();

            var planetGravityComponent = planet.GetComponent<Gravity>();

            var mass = _gravity.Mass;
            var planetMass = planetGravityComponent.Mass;

            var isBigger = (mass > planetMass);
            PlayAudio(other, isBigger);

            if (other.relativeVelocity.x != 0 || other.relativeVelocity.y != 0) {
                var massModifier = planetMass / (mass + planetMass);

                v = other.relativeVelocity * massModifier * BOUNCE_MODIFIER;
            }

            if (v.magnitude < MINIMUM_BOUNCE)
                v = v.normalized * MINIMUM_BOUNCE;
            else if (v.magnitude < MAXIMUM_BOUNCE)
                v = v.normalized * MAXIMUM_BOUNCE;

            Velocity += v;
        }
    }

    private void PlayAudio(Collision2D other, bool isBigger) {
        if (gameObject.name == "Player")
            _audio.Play();

        if (other.gameObject.name != "Player" && isBigger)
            _audio.Play();
    }

    private void SetVelocity() {
        var velocity = Velocity;

        velocity += _gravity.Velocity;

        var playerComponent = GetComponent<Player>();

        if (playerComponent != null)
            velocity += playerComponent.Velocity;

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
