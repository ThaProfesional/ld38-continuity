﻿using UnityEngine;

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

        var winObject = GameObject.Find("Camera/Win");
        var winSpriteRenderComponent = winObject.GetComponent<SpriteRenderer>();
        winSpriteRenderComponent.enabled = false;

        var loseObject = GameObject.Find("Camera/Lose");
        var loseSpriteRenderComponent = loseObject.GetComponent<SpriteRenderer>();
        loseSpriteRenderComponent.enabled = true;
    }
}