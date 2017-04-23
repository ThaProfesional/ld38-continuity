using System.Linq;
using UnityEngine;

public class AI : MonoBehaviour {
    const float PANIC_DISTANCE = 5F;

    const float SPEED = 3F;

    private Gravity _gravity;

    private GameObject _blackHoleObject;

    private Vector2 _velocity;
    public Vector2 Velocity {
        get {
            return _velocity;
        }
    }

    void Start () {
        _gravity = gameObject.GetComponent<Gravity>();

        _blackHoleObject = GameObject.Find("Black Hole");
    }
	
	void Update () {
        var d = transform.position - _blackHoleObject.transform.position;
        var dm = d.magnitude;

        if (dm < PANIC_DISTANCE) {
            Survive(d);
        } else {
            Attack();
        }
    }

    // TODO: test the shit out of this
    private void Survive(Vector2 d) {
        float a = 45;

        var v = _gravity.Velocity;

        var cross = Vector3.Cross(v, d);

        if (cross.z > 0)
            a *= -1;

        v = v.Rotate(a);

        _velocity += v.normalized * SPEED;
    }

    private void Attack() {
        // get players in radius

        // get vectors to them - if any line up with movement attack
    }

    /*private float DistanceToBlackHole() {
        return FindObjectsOfType<Planet>()
            .Where(x => x != this)
            .ToList();
    }*/
}
