using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HolerSystem : MonoBehaviour {
    const float BLACK_HOLE_SCALE = 0.5F;
    const float BLACK_HOLE_MASS = 1000;

    const float PLAYER_MASS = 20;

    const float PLANET_MIN_SCALE = 0.5F;
    const float PLANET_MAX_SCALE = 2.5F;

    const int PLANET_MIN_MASS = 10;
    const int PLANET_MAX_MASS = 50;

    const int MAX_TOTAL_PLANET_MASS = 200;
    const int MAX_PLANETS = 8;

    const int DANGER_ZONE = 5;
    const int EDGE = 20;
    public const int BOUNDS = 30;

    const float START_VELOCITY = 0.1F;

    const float ROTATIONAL_ANGLE = 90F;

    const float START_TIME = 5F;

    private GameObject _fullness;

    public readonly IList<string> PLANET_SPRITES = new List<string> {
        "planet",
        "planet-2"
    }.AsReadOnly();

    public int PlanetaryMass;

    void Awake() {
        Generate();
    }

    void Start() {
        _fullness = GameObject.Find("Fullness");
    }

    private void Update() {
        if (Input.GetKeyDown("r"))
            Regenerate();
    }

    public void Regenerate() {
        Clear();

        Generate();

        var fullnessComponent = _fullness.GetComponent<Fullness>();
        fullnessComponent.Initialise();
    }

    private void Clear() {
        // https://forum.unity3d.com/threads/deleting-all-chidlren-of-an-object.92827/
        var children = new List<GameObject>();

        foreach (Transform child in transform) {
            children.Add(child.gameObject);
        }

        children.ForEach(child => Destroy(child));
    }

    private void Generate() {
        CreateBlackHole();

        CreatePlayer();

        CreatePlanets();

        // start countdown timer to DOOM!
        StartCoroutine("EnableHole");
    }

    private void CreateBlackHole() {
        var blackHoleInstance = (GameObject)Instantiate(Resources.Load("Black Hole"));
        blackHoleInstance.transform.SetParent(transform);

        blackHoleInstance.name = "Black Hole";

        blackHoleInstance.transform.transform.position = new Vector3(0, 0, 1);

        blackHoleInstance.transform.localScale = new Vector2(BLACK_HOLE_SCALE, BLACK_HOLE_SCALE);

        var blackHoleGravityComponent = blackHoleInstance.GetComponent<Gravity>();
        blackHoleGravityComponent.Mass = BLACK_HOLE_MASS;
        blackHoleGravityComponent.DisablePull = true;
    }

    private void CreatePlanets() {
        var remainingMass = MAX_TOTAL_PLANET_MASS;

        for(var i = 0; i < MAX_PLANETS; i++) {
            var randomScale = Random.Range(PLANET_MIN_SCALE, PLANET_MAX_SCALE);
            var scale = new Vector2(randomScale, randomScale);

            var mass = Random.Range(PLANET_MIN_MASS, PLANET_MAX_MASS);

            if (mass > remainingMass)
                mass = remainingMass;

            Vector3 position;

            do {
                position = GetRandomPosition();
            } while(PlanetWillOverlap(scale, position));

            CreatePlanet(i, mass, scale, position);

            remainingMass -= mass;

            if (remainingMass <= 0)
                break;
        }

        PlanetaryMass = MAX_TOTAL_PLANET_MASS - remainingMass;
    }

    private bool PlanetWillOverlap(Vector2 scale, Vector2 position) {
        var bounds = new Bounds(position, scale);

        // loop through all existing planets
        return FindObjectsOfType<Planet>().Where(x => {
            var collider = x.gameObject.GetComponent<CircleCollider2D>();

            return collider.bounds.Intersects(bounds);
        }).Any();
    }

    private void CreatePlanet(int num, int Mass, Vector2 scale, Vector3 position) {
        var planetInstance = (GameObject)Instantiate(Resources.Load("Planet"));
        planetInstance.transform.SetParent(transform);

        planetInstance.name = string.Format("Planet{0}", num);

        planetInstance.transform.transform.position = position;

        planetInstance.transform.localScale = scale;

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

        playerInstance.name = "Player";

        playerInstance.transform.transform.position = GetRandomPosition();

        var playerGravityComponent = playerInstance.GetComponent<Gravity>();
        playerGravityComponent.Mass = PLAYER_MASS;

        SetInitialVelocity(playerInstance);
    }

    private Vector3 GetRandomPosition() {
        var x = Random.Range(DANGER_ZONE, EDGE);

        if (Random.value < 0.5F)
            x *= -1;

        var y = Random.Range(DANGER_ZONE, EDGE);

        if (Random.value < 0.5F)
            y *= -1;

        return new Vector3(
            x,
            y,
            1
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

        if (blackHoleObject != null) {
            var d = (Vector2)(blackHoleObject.transform.position - transform.position);
            var dn = d.normalized;

            var a = Random.value < 0.5F
                ? ROTATIONAL_ANGLE
                : ROTATIONAL_ANGLE * -1;

            dn = dn.Rotate(a);

            var planetComponent = planet.GetComponent<Planet>();
            planetComponent.Velocity += dn;
        }
    }

    private IEnumerator EnableHole() {
        yield return new WaitForSeconds(START_TIME);

        var blackHoleObject = GameObject.Find("Black Hole");
        var blackHoleGravityComponent = blackHoleObject.GetComponent<Gravity>();
        blackHoleGravityComponent.DisablePull = false;
    }
}
