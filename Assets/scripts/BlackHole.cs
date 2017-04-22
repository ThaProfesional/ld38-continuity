using UnityEngine;

public class BlackHole : MonoBehaviour {

	void Start () {
		
	}
	
	void Update () {
		
	}

    void FixedUpdate() {
        // TODO: when a planet's completely contained - remove it

        // also grow bigger
    }

    void OnTriggerEnter2D(Collision2D coll)
    {
        Suck(coll.gameObject);
        /*
        if (coll.gameObject.tag == "Enemy")
            coll.gameObject.SendMessage("ApplyDamage", 10);
       */

    }

    private void Suck(GameObject planet) {
        var t = 1;
        // disable gravity

        // pull straight in
    }
}
