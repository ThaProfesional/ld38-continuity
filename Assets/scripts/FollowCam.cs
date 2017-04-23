using UnityEngine;

public class FollowCam : MonoBehaviour {
	void Start () {
        var playerObject = GameObject.Find("Player");

        transform.position = new Vector2(
            playerObject.transform.position.x,
            playerObject.transform.position.y
        );
    }
	
	void Update () {
        var playerObject = GameObject.Find("Player");

        if (playerObject != null) {
            var currentX = transform.position.x;
            var currentY = transform.position.y;

            transform.position = new Vector2(
                playerObject.transform.position.x,
                playerObject.transform.position.y
            );
        }
    }
}
