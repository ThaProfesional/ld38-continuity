using UnityEngine;

public class BlackHole : MonoBehaviour {
    public const float MASS_MODIFIER = 10F;
    const float SIZE_MODIFIER = 8F;

    void Start() {

    }

    void Update() {

    }

    void OnTriggerEnter2D(Collider2D other) {
        var planet = other.gameObject;

        var gravityComponent = planet.GetComponent<Gravity>();

        gravityComponent.IsDoomed = true;
    }

    void OnTriggerStay2D(Collider2D other) {
        var blackHoleColliderComponent = GetComponent<Collider2D>();

        other.transform.localScale = new Vector2(
            other.transform.localScale.x * 0.5F,
            other.transform.localScale.y * 0.5F
        );

        if (blackHoleColliderComponent.bounds.Contains(other.bounds.min)
            && blackHoleColliderComponent.bounds.Contains(other.bounds.max)) {
            Grow(other.gameObject);

            if (other.gameObject.GetComponent<Player>() != null)
                Lose();

            Destroy(other.gameObject);
        }
    }

    private void Grow(GameObject planet) {
        var gravityComponent = GetComponent<Gravity>();
        var planetGravityComponent = planet.GetComponent<Gravity>();

        var increase = planetGravityComponent.Mass / (gravityComponent.Mass + planetGravityComponent.Mass);

        // TODO: deal with rounding errors
        gravityComponent.Mass += (int)(gravityComponent.Mass * increase * MASS_MODIFIER);

        transform.localScale += transform.localScale * increase * SIZE_MODIFIER;
    }

    private void Lose() {
        var gravityComponent = GetComponent<Gravity>();
        gravityComponent.DisablePull = true;

        var fullnessObject = GameObject.Find("Fullness");
        var fullnessComponent = fullnessObject.GetComponent<Fullness>();
        fullnessComponent.Lose();
    }
}