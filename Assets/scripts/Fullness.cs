using UnityEngine;

public class Fullness : MonoBehaviour {
    private float _hunger;
    private float _previousPercentage;
    private float _startMass;

	void Start() {
        Initialise();
    }

    void Update () {
        var blackHoleGravityComponent = GetBlackHoleGravity();

        var currentMass = blackHoleGravityComponent.Mass - _startMass;

        var percentage = currentMass / _hunger;

        if (percentage > _previousPercentage) {
            SetPercentage(percentage);

            if (percentage >= 1)
                Win(blackHoleGravityComponent);
        }
    }

    public void Initialise() {
        _previousPercentage = 0;

        var blackHoleGravityComponent = GetBlackHoleGravity();
        _startMass = blackHoleGravityComponent.Mass;

        var holerSystemComponent = GetHolerSystem();
        _hunger = holerSystemComponent.PlanetaryMass * 0.75F * BlackHole.MASS_MODIFIER;

        ShowText("Lose", false);
        ShowText("Lose Retry Text", false);

        ShowText("Win", false);
        ShowText("Win Retry Text", false);
    }

    public void Lose() {
        ShowText("Lose", true);
        ShowText("Lose Retry Text", true);

        ShowText("Win", false);
        ShowText("Win Retry Text", false);
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
        var maskObject = GetMask();
        var spriteRenderComponent = GetSpriteRenderComponent();

        var barWidth = spriteRenderComponent.sprite.bounds.size.x;

        var xOffset = spriteRenderComponent.transform.position.x + (barWidth * percentage);

        maskObject.transform.position = new Vector3(
            xOffset,
            maskObject.transform.position.y,
            maskObject.transform.position.z
        );
    }

    private GameObject GetMask() {
        return transform.Find("Bar/Mask").gameObject;
    }

    private SpriteRenderer GetSpriteRenderComponent() {
        var spriteObject = transform.Find("Bar/Sprite").gameObject;
        return spriteObject.GetComponent<SpriteRenderer>();
    }

    private void Win(Gravity blackHoleGravityComponent) {
        blackHoleGravityComponent.DisablePull = true;

        ShowText("Lose", false);
        ShowText("Lose Retry Text", false);

        ShowText("Win", true);
        ShowText("Win Retry Text", true);
    }

    private void ShowText(string outcome, bool enabled) {
        var objectName = string.Format("Camera/{0}", outcome);
        var instance = GameObject.Find(objectName);
        var spriteRenderComponent = instance.GetComponent<SpriteRenderer>();
        spriteRenderComponent.enabled = enabled;
    }
}
