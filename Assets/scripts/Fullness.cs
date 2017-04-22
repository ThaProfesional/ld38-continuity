using UnityEngine;

public class Fullness : MonoBehaviour {
    private float _hunger;
    private float _startMass;

	void Start () {
		// TODO: make "HoleSystem" to spawn planets - with planet mass limit
        // hunger should be 3/4 of that

        var blackHoleObject = GameObject.Find("Black Hole");
        var blackHoleGravityComponent = blackHoleObject.GetComponent<Gravity>();

        _startMass = blackHoleGravityComponent.Mass;
    }
	
	void Update () {
		
	}
}
