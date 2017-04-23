using UnityEngine;

public class FollowCam : MonoBehaviour {
    const float FOLLOW_LAG = 200F;

	void Start () {
        var playerObject = GameObject.Find("Player");

        transform.position = new Vector2(
            playerObject.transform.position.x,
            playerObject.transform.position.y
        );
    }
	
	void Update () {
        var currentX = transform.position.x;
        var currentY = transform.position.y;

        var playerObject = GameObject.Find("Player");

        var goalX = playerObject.transform.position.x;
        var goalY = playerObject.transform.position.y;

        transform.position = new Vector2(
            Mathf.Lerp(currentX, goalX, Time.time * FOLLOW_LAG),
            Mathf.Lerp(currentY, goalY, Time.time * FOLLOW_LAG)
        );
    }
}
