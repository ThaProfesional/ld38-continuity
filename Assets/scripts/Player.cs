using UnityEngine;

public class Player : MonoBehaviour {
    const string HORIZONTAL_AXIS = "Horizontal";
    const string VERTICAL_AXIS = "Vertical";

    const float HORITONZAL_SPEED = 2.5F;
    const float VERTICAL_SPEED = 2.5F;

    const float BOOST_THRESHOLD = 20F;
    const float LARGER_BOOST_THRESHOLD = 10F;

    const float BOOST = 1.5F;
    const float LARGER_BOOST = 2F;

    private Gravity _gravity;

    private Vector2 _velocity;
    public Vector2 Velocity {
        get {
            return _velocity;
        }
    }

    void Start () {
        _gravity = GetComponent<Gravity>();
    }
	
	void Update () {
        float horizontalMove = Input.GetAxis(HORIZONTAL_AXIS);
        float verticalMove = Input.GetAxis(VERTICAL_AXIS);

        var rigidBodyComponent = GetComponent<Rigidbody2D>();

        var v = new Vector2(
            horizontalMove * HORITONZAL_SPEED,
            verticalMove * VERTICAL_SPEED
        );

        _velocity = v * GetBoost(v);
    }

    private float GetBoost(Vector2 v) {
        var a = Vector2.Angle(v, _gravity.Velocity);

        return a < LARGER_BOOST_THRESHOLD
            ? LARGER_BOOST
            : a < BOOST_THRESHOLD
                ? BOOST
                : 1;
    }

    private void SetVelocity() {
        var velocity = _velocity;

        velocity += _gravity.Velocity;

        var planetComponent = GetComponent<Planet>();

        if (planetComponent != null)
            velocity += planetComponent.Velocity;

        var rigidComponent = GetComponent<Rigidbody2D>();

        rigidComponent.velocity = velocity;
    }
}
