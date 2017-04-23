using UnityEngine;

public class Fullness : MonoBehaviour {
    private float _hunger;
    private float _startMass;

	void Start() {
        Initialise();
    }

    void Update () {
        var blackHoleGravityComponent = GetBlackHoleGravity();

        var currentMass = blackHoleGravityComponent.Mass - _startMass;

        var percentage = currentMass / _hunger;

        SetPercentage(percentage);

        if (percentage >= 1)
            Win(blackHoleGravityComponent);
    }

    public void Initialise() {
        var blackHoleGravityComponent = GetBlackHoleGravity();
        _startMass = blackHoleGravityComponent.Mass;

        var holerSystemComponent = GetHolerSystem();
        _hunger = holerSystemComponent.PlanetaryMass * 0.75F * BlackHole.MASS_MODIFIER;

        SetGameOutcome("Lose", false);

        SetGameOutcome("Win", false);
    }

    private HolerSystem GetHolerSystem() {
        var holerSystemObject = GameObject.Find("Holer System");
        return holerSystemObject.GetComponent<HolerSystem>();
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

        SetGameOutcome("Lose", false);

        SetGameOutcome("Win", true);
    }

    private void SetGameOutcome(string outcome, bool enabled) {
        var objectName = string.Format("Camera/{0}", outcome);
        var instance = GameObject.Find(objectName);
        var spriteRenderComponent = instance.GetComponent<SpriteRenderer>();
        spriteRenderComponent.enabled = enabled;
    }
}
