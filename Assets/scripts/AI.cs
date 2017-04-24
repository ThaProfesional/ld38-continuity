using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AI : MonoBehaviour {
    const float REALLY_PANIC_DISTANCE = 3F;
    const float PANIC_DISTANCE = 5F;

    const float SPEED = 1.5F;
    const float DESPERATE = 2F;

    const float ESCAPE_ANGLE = 55F;

    const float ATTACK_DISTANCE = 5F;
    const float ATTACK_CHECK_ANGLE = 45F;
    const float ATTACK_ANGLE = 10F;
    const float ATTACK_SPEED = 3F;

    const float DEFEND_DISTANCE = 0.5F;
    const float DEFEND_SPEED = 3F;

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
                if (!Attack(d)){
                    Defend();
                }
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

    private bool Attack(Vector2 d) {
        var planets = GetPlanets();

        var player = planets.Where(x => x.gameObject.name == "Player").FirstOrDefault();

        var attacked= AttackPlanet(player, d);

        if (player == null || !attacked) {
            var nonPlayers = planets.Except(new List<Planet> { player }).ToList();

            foreach(var planet in nonPlayers) {
                attacked = AttackPlanet(planet, d);

                if (attacked)
                    return attacked;
            }
        }

        return attacked;
    }

    private void Defend() {
        var planets = GetPlanets();

        foreach (var planet in planets) {
            var dp = (Vector2)(planet.transform.position - transform.position);

            if (dp.magnitude < DEFEND_DISTANCE) {
                _planet.Velocity += dp.normalized * DEFEND_SPEED;
            }
        }
    }

    private IList<Planet> GetPlanets() {
        return FindObjectsOfType<Planet>()
            .Where(x => x.gameObject != gameObject)
            .ToList();
    }

    private bool AttackPlanet(Planet planet, Vector2 d) {
        if (planet != null) {
            var dp = (Vector2)(planet.transform.position - transform.position);

            if (dp.magnitude < ATTACK_DISTANCE) {
                var velocity = GetVelocity();
                var planetVelocity = planet.GetVelocity();

                var a = Vector2.Angle(velocity, planetVelocity);

                if (a < ATTACK_CHECK_ANGLE) {
                    var ra = GetAttackAngle(d, dp);

                    dp = dp.Rotate(ra);

                    _planet.Velocity += dp.normalized * ATTACK_SPEED;
                    return true;
                }
            }
        }

        return false;
    }

    public Vector2 GetVelocity() {
        return _gravity.Velocity + _planet.Velocity;
    }

    private void SetVelocity() {
        _rigid.velocity = GetVelocity();
    }

    private float GetAttackAngle(Vector2 d, Vector2 dp) {
        var cross = Vector3.Cross(dp, d);

        return (cross.z < 0)
            ? ATTACK_ANGLE * -1
            : ATTACK_ANGLE;
    }
}
