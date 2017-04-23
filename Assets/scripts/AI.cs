using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AI : MonoBehaviour {

	void Start () {
		
	}
	
	void Update () {
        //var blackHoleObject = GameObject.Find("Black Hole");

        // loop through all planets not this one
        var planets = GetOtherPlanets();

        // get all closer in to attack


        // get all further out to defend

        // find next closest planet in
        // wait until close in rotation

        // move towards

        // no close ones
        // pull out as fast as possible
    }

    private IList<Planet> GetOtherPlanets() {
        return FindObjectsOfType<Planet>()
            .Where(x => x != this)
            .ToList();
    }



    private void Attack() {

    }

    private void Bide() {

    }

    private void Defend() {

    }

    private void Survive() {

    }
}
