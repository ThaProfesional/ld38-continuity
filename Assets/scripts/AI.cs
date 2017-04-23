using UnityEngine;

public class AI : MonoBehaviour {
    const float REALLY_PANIC_DISTANCE = 3F;
    const float PANIC_DISTANCE = 5F;

    const float SPEED = 1.5F;
    const float DESPERATE = 2F;

    const float ESCAPE_ANGLE = 55F;

    private Gravity _gravity;
    private Planet _planet;
    private Rigidbody2D _rigid;

    private GameObject _blackHoleObject;

    void Start () {
        _gravity = gameObject.GetComponent<Gravity>();
        _planet = gameObject.GetComponent<Planet>();
        _rigid = gameObject.GetComponent<Rigidbody2D>();

        _blackHoleObject = GameObject.Find("Black Hole");
    }
	
	void FixedUpdate () {
        if (_blackHoleObject != null) {
            var d = transform.position - _blackHoleObject.transform.position;
            var dm = d.magnitude;

            if (dm < PANIC_DISTANCE) {
                Survive(d, dm < REALLY_PANIC_DISTANCE);
            } else {
                Attack();
            }

            SetVelocity();
        }
    }

    private void Survive(Vector2 d, bool reallyPanic) {
        float a = ESCAPE_ANGLE;

        var v = _gravity.Velocity;

        var cross = Vector3.Cross(v, d);

        if (cross.z < 0)
            a *= -1;

        v = v.Rotate(a);

        _planet.Velocity += v.normalized * SPEED;
    }

    private void Attack() {
        // get players in radius

        // get vectors to them - if any line up with movement attack
    }

    private void SetVelocity() {
        var velocity = _gravity.Velocity;

        velocity += _planet.Velocity;

        _rigid.velocity = velocity;
    }
}
