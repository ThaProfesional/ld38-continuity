using UnityEngine;

public class BlackHole : MonoBehaviour {

	void Start () {

    }
	
	void Update () {
		
	}

    void FixedUpdate() {
        // TODO: when a planet's completely contained - remove it

        // also grow bigger
    }

    void OnTriggerEnter2D(Collider2D other) {
        var planet = other.gameObject;

        var gravityComponent = planet.GetComponent<Gravity>();

        gravityComponent.IsDoomed = true;
    }

    void OnTriggerStay2D(Collider2D other) {
        var blackHoleColliderComponent = GetComponent<Collider2D>();

        if (blackHoleColliderComponent.bounds.Contains(other.bounds.min)
            && blackHoleColliderComponent.bounds.Contains(other.bounds.max)) {
            Destroy(other.gameObject);
        }

    }
}
