using UnityEngine;

public class Planet : MonoBehaviour {

    public int GravitationalRadius;
    public int Mass;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        // TODO: loop through all the other planets

        var player = GameObject.Find("Player");

    }

    private bool AffectedByGravity(GameObject planet)
    {
        var distance = planet.transform.position - transform.position;

        return distance.magnitude <= GravitationalRadius;
    }

    private void ApplyGravity(GameObject planet) {
        var displacement = planet.transform.position - transform.position;

        // accelerate in proportionally to mass and displacement
    }
}
