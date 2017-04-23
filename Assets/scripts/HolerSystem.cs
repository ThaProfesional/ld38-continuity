using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolerSystem : MonoBehaviour {
    const float BLACK_HOLE_SCALE = 0.5F;
    const float BLACK_HOLE_MASS = 500;

    const float PLANET_MIN_SCALE = 0.5F;
    const float PLANET_MAX_SCALE = 2.5F;

    const int PLANET_MIN_MASS = 10;
    const int PLANET_MAX_MASS = 50;

    const int MAX_TOTAL_PLANET_MASS = 250;
    const int MAX_PLANETS = 10;

    const int DANGER_ZONE = 5;
    const int EDGE = 20;

    const float START_VELOCITY = 0.1F;

    const float ROTATIONAL_ANGLE = 90F;

    const float START_TIME = 5F;

    public bool EnableReset;

    public readonly IList<string> PLANET_SPRITES = new List<string> {
        "planet",
        "planet-2"
    }.AsReadOnly();

    public int PlanetaryMass;

    void Awake() {
        Generate();
    }

    private void Update() {
        if (EnableReset && Input.GetKeyDown("r"))
            Regenerate();
    }

    public void Regenerate() {
        EnableReset = false;

        Clear();

        Generate();

        var fullnessObject = GameObject.Find("Fullness");
        var fullnessComponent = fullnessObject.GetComponent<Fullness>();
        fullnessComponent.Initialise();
    }

    private void Clear() {
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
    }

    private void Generate() {
        CreateBlackHole();

        CreatePlanets();

        CreatePlayer();

        // start countdown timer to DOOM!
        StartCoroutine("EnableHole");
    }

    private void CreateBlackHole() {
        var blackHoleInstance = (GameObject)Instantiate(Resources.Load("Black Hole"));
        blackHoleInstance.transform.SetParent(transform);

        blackHoleInstance.name = "Black Hole";

        blackHoleInstance.transform.localScale = new Vector2(BLACK_HOLE_SCALE, BLACK_HOLE_SCALE);

        var blackHoleGravityComponent = blackHoleInstance.GetComponent<Gravity>();
        blackHoleGravityComponent.Mass = BLACK_HOLE_MASS;
        blackHoleGravityComponent.DisablePull = true;
    }

    private void CreatePlanets() {
        var remainingMass = MAX_TOTAL_PLANET_MASS;

        for(var i = 0; i <= MAX_PLANETS; i++) {
            var mass = Random.Range(PLANET_MIN_MASS, PLANET_MAX_MASS);

            if (mass > remainingMass)
                mass = remainingMass;

            var position = GetRandomPosition();

            // make sure the planet's a bit away from the others

            CreatePlanet(mass, position);

            remainingMass -= mass;

            if (remainingMass <= 0)
                break;
        }

        PlanetaryMass = MAX_TOTAL_PLANET_MASS - remainingMass;
    }

    private void CreatePlanet(int Mass, Vector2 position) {
        var planetInstance = (GameObject)Instantiate(Resources.Load("Planet"));
        planetInstance.transform.SetParent(transform);

        planetInstance.transform.transform.position = position;

        var randomScale = Random.Range(PLANET_MIN_SCALE, PLANET_MAX_SCALE);
        planetInstance.transform.localScale = new Vector2(randomScale, randomScale);

        var planetGravityComponent = planetInstance.GetComponent<Gravity>();
        planetGravityComponent.Mass = Mass;

        var planetSpriteRendererComponent = planetInstance.GetComponent<SpriteRenderer>();
        planetSpriteRendererComponent.sprite = GetRandomPlanetSprite();

        var planetColliderComponent = planetInstance.GetComponent<CircleCollider2D>();
        planetColliderComponent.radius = planetSpriteRendererComponent.sprite.bounds.size.x / 2;

        SetInitialVelocity(planetInstance);
    }

    private void CreatePlayer() {
        var playerInstance = (GameObject)Instantiate(Resources.Load("Player"));
        playerInstance.transform.SetParent(transform);

        playerInstance.transform.transform.position = GetRandomPosition();

        SetInitialVelocity(playerInstance);
    }

    private Vector2 GetRandomPosition() {
        var x = Random.Range(DANGER_ZONE, EDGE);

        if (Random.value < 0.5F)
            x *= -1;

        var y = Random.Range(DANGER_ZONE, EDGE);

        if (Random.value < 0.5F)
            y *= -1;

        return new Vector2(
            x,
            y
        );
    }

    private Sprite GetRandomPlanetSprite() {
        var spriteIndex = Random.Range(0, PLANET_SPRITES.Count);
        var spriteName = PLANET_SPRITES[spriteIndex];
        var fullName = string.Format("textures/{0}", spriteName);

        return (Sprite)Resources.Load(fullName, typeof(Sprite));
    }

    // hack to stop things instantle being sucked into the black hole
    // don't judge me (or do - I'm not your boss)
    private void SetInitialVelocity(GameObject planet) {
        var blackHoleObject = GameObject.Find("Black Hole");
        var d = (Vector2)(blackHoleObject.transform.position - transform.position);
        var dn = d.normalized;

        var a = Random.value < 0.5F
            ? ROTATIONAL_ANGLE
            : ROTATIONAL_ANGLE * -1;

        dn = dn.Rotate(a);

        var planetComponent = planet.GetComponent<Planet>();
        planetComponent.PushVelocity(dn);
    }

    private IEnumerator EnableHole() {
        yield return new WaitForSeconds(START_TIME);

        var blackHoleObject = GameObject.Find("Black Hole");
        var blackHoleGravityComponent = blackHoleObject.GetComponent<Gravity>();
        blackHoleGravityComponent.DisablePull = false;
    }
}
