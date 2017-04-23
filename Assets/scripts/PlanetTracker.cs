using System.Collections.Generic;
using UnityEngine;

public class PlanetTracker : MonoBehaviour {
    const int UPDATE_PERIOD = 1;

    private IDictionary<string, PlanetaryDisplacement> _planets;

    private int updateTracker;

	void Start () {
        _planets = new Dictionary<string, PlanetaryDisplacement>();

        updateTracker = 0;
    }
	
	void Update () {
        var blackHoleObject = GameObject.Find("Black Hole");

        if (updateTracker == UPDATE_PERIOD) {
            foreach (var planet in _planets) {
                var planetObject = GameObject.Find(planet.Key);

                _planets[planet.Key] = GetDisplacement(planetObject, blackHoleObject);
            }

            updateTracker = 0;
        } else {
            updateTracker++;
        }
    }

    public void AddPlanet(GameObject planetObject) {
        var blackHoleObject = GameObject.Find("Black Hole");

        var displacement = GetDisplacement(planetObject, blackHoleObject);

        _planets[planetObject.name] = displacement;
    }

    public void RemovePlanet(GameObject planetObject) {
        _planets.Remove(planetObject.name);
    }

    public float GetDistanceFromHole(GameObject planetObject) {
        var displacement = _planets[planetObject.name];

        return displacement.Magnitude;
    }

    /*public IList<string> GetClosePlanets(GameObject planetObject) {
        var displacement = _planets[planetObject.name];
        var orbit = displacement.Magnitude;

        // get all the planets in a similar orbit

        // filter down to those 

        return new List<string>();
    }*/

    private PlanetaryDisplacement GetDisplacement(GameObject planetObject, GameObject blackHoleObject) {
        var v = blackHoleObject.transform.position - planetObject.transform.position;

        return new PlanetaryDisplacement {
            Displacement = v,
            Magnitude = v.magnitude
        };
    }
}

public struct PlanetaryDisplacement {
    public Vector2 Displacement;
    public float Magnitude;
}