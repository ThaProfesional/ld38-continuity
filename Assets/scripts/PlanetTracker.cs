using System.Collections.Generic;
using UnityEngine;

public class PlanetTracker : MonoBehaviour {
    const int UPDATE_PERIOD = 1;

    private IDictionary<string, PlanetaryPosition> _planets;

    private int updateTracker;

	void Start () {
        _planets = new Dictionary<string, PlanetaryPosition>();

        updateTracker = 0;
    }
	
	void Update () {
        var blackHoleObject = GameObject.Find("Black Hole");

        if (updateTracker == UPDATE_PERIOD) {
            foreach (var planet in _planets) {
                var planetObject = GameObject.Find(planet.Key);

                _planets[planet.Key] = StorePosition(planetObject, blackHoleObject);
            }

            updateTracker = 0;
        } else {
            updateTracker++;
        }
    }

    public void AddPlanet(GameObject planetObject) {
        var blackHoleObject = GameObject.Find("Black Hole");

        var displacement = StorePosition(planetObject, blackHoleObject);

        // _planets[planetObject.name] = displacement;
    }

    public void RemovePlanet(GameObject planetObject) {
        _planets.Remove(planetObject.name);
    }

    public float GetDistanceFromHole(GameObject planetObject) {
        var displacement = _planets[planetObject.name];

        return displacement.Magnitude;
    }

    public bool OutOfBounds(GameObject planetObject) {
        var displacement = _planets[planetObject.name];

        return displacement.Position.x > HolerSystem.EDGE
            || displacement.Position.y > HolerSystem.EDGE;
    }

    /*public IList<string> GetClosePlanets(GameObject planetObject) {
        var displacement = _planets[planetObject.name];
        var orbit = displacement.Magnitude;

        // get all the planets in a similar orbit

        // filter down to those 

        return new List<string>();
    }*/

    private PlanetaryPosition StorePosition(GameObject planetObject, GameObject blackHoleObject) {
        return new PlanetaryPosition {
            Position = planetObject.transform.position,
            Magnitude = planetObject.transform.position.magnitude
        };
    }
}

public struct PlanetaryPosition {
    public Vector2 Position;
    public float Magnitude;
}