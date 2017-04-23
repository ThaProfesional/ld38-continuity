using UnityEngine;

public class Fullness : MonoBehaviour {
    private float _hunger;
    private float _startMass;

	void Start () {
        // TODO: make "HoleSystem" to spawn planets - with planet mass limit
        // hunger should be 3/4 of that
        // TODO: take into account black hole multiplier
        _hunger = 50;

        var blackHoleGravityComponent = GetBlackHoleGravity();
        _startMass = blackHoleGravityComponent.Mass;
    }
	
	void Update () {
        var blackHoleGravityComponent = GetBlackHoleGravity();

        var currentMass = blackHoleGravityComponent.Mass - _startMass;

        var percentage = currentMass / _hunger;

        SetPercentage(percentage);

        if (percentage >= 1)
            Win(blackHoleGravityComponent);
    }

    private Gravity GetBlackHoleGravity() {
        var blackHoleObject = GameObject.Find("Black Hole");
        return blackHoleObject.GetComponent<Gravity>();
    }

    private void SetPercentage(float percentage) {
        var mask = GetMask();

        var maskPercentage = (percentage == 0)
            ? 1
            : 1 - percentage;

        mask.transform.localScale = new Vector2(
            mask.transform.localScale.x,
            maskPercentage
        );
    }

    private GameObject GetMask() {
        return transform.Find("Bar/Mask").gameObject;
    }

    private void Win(Gravity blackHoleGravityComponent) {
        blackHoleGravityComponent.DisablePull = true;

        var loseObject = GameObject.Find("Camera/Lose");
        var loseSpriteRenderComponent = loseObject.GetComponent<SpriteRenderer>();
        loseSpriteRenderComponent.enabled = false;

        var winObject = GameObject.Find("Camera/Win");
        var winSpriteRenderComponent = winObject.GetComponent<SpriteRenderer>();
        winSpriteRenderComponent.enabled = true;
    }
}
