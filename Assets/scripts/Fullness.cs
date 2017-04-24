using UnityEngine;

public class Fullness : MonoBehaviour {
    private GameObject _blackHole;
    private GameObject _holerSystem;
    private GameObject _mask;
    private GameObject _sprite;

    private float _hunger;
    private float _previousPercentage;
    private float _startMass;

	void Start() {
        _holerSystem = GameObject.Find("Holer System");
        _mask = GameObject.Find("Bar/Mask");
        _sprite = GameObject.Find("Bar/Sprite");

        Initialise();
    }

    void Update () {
        if (_blackHole != null) {
            var blackHoleGravityComponent = _blackHole.GetComponent<Gravity>();

            if (blackHoleGravityComponent != null) {
                var currentMass = blackHoleGravityComponent.Mass - _startMass;

                var percentage = currentMass / _hunger;

                if (percentage >= _previousPercentage) {
                    SetPercentage(percentage);

                    if (percentage >= 1)
                        Win(blackHoleGravityComponent);
                }
            }
        } else {
            _blackHole = GameObject.Find("Black Hole");
        }
    }

    public void Initialise() {
        _blackHole = GameObject.Find("Black Hole");

        _previousPercentage = 0;

        _startMass = _blackHole.GetComponent<Gravity>().Mass;
        _hunger = _holerSystem.GetComponent<HolerSystem>().PlanetaryMass * 0.75F * BlackHole.MASS_MODIFIER;

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

    private void SetPercentage(float percentage) {
        var spriteRenderComponent = _sprite.GetComponent<SpriteRenderer>();

        var barWidth = spriteRenderComponent.sprite.bounds.size.x;

        var xOffset = spriteRenderComponent.transform.position.x + (barWidth * percentage);

        _mask.transform.position = new Vector3(
            xOffset,
            _mask.transform.position.y,
            _mask.transform.position.z
        );
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
