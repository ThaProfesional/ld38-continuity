using UnityEngine;

public class Player : MonoBehaviour {
    const string HORIZONTAL_AXIS = "Horizontal";
    const string VERTICAL_AXIS = "Vertical";

    const float HORITONZAL_SPEED = 2F;
    const float VERTICAL_SPEED = 2F;

    private Vector2 _velocity;
    public Vector2 Velocity {
        get {
            return _velocity;
        }
    }

    void Start () {
		
	}
	
	void Update () {
        float horizontalMove = Input.GetAxis(HORIZONTAL_AXIS);
        float verticalMove = Input.GetAxis(VERTICAL_AXIS);

        var rigidBodyComponent = GetComponent<Rigidbody2D>();

        _velocity = new Vector2(
            horizontalMove * HORITONZAL_SPEED,
            verticalMove * VERTICAL_SPEED
        );
    }

    private void SetVelocity() {
        var velocity = _velocity;

        var gravityComponent = GetComponent<Gravity>();

        if (gravityComponent != null)
            velocity += gravityComponent.Velocity;

        var planetComponent = GetComponent<Planet>();

        if (planetComponent != null)
            velocity += planetComponent.Velocity;

        var rigidComponent = GetComponent<Rigidbody2D>();

        rigidComponent.velocity = velocity;
    }
}
