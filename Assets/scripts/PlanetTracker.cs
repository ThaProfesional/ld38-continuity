using UnityEngine;

public class PlanetTracker : MonoBehaviour {
    private Camera _camera;

	void Start() {
        _camera = Camera.main;
	}
	
	void Update() {

        //var player = GameObject.Find("Player");

        //UpdatePlanets(player);

        // UpdateBlackHole(player);
    }

    private void UpdatePlanets(GameObject player) {
        var planets = FindObjectsOfType<Planet>();

        foreach(var planet in planets) {
            if (!OnScreen(planet.gameObject))
                DisplayIndicator(player, planet.gameObject, false);
        }
    }

    private bool OnScreen(GameObject gameObject) {
        var renderer = gameObject.GetComponent<SpriteRenderer>();
        return renderer.isVisible;
    }

    private void DisplayIndicator(GameObject player, GameObject gameObject, bool isBlackHole) {
        var displacement = player.transform.position - gameObject.transform.position;

        // check if it already exists (by name)

        // if not - make it
    }
}
