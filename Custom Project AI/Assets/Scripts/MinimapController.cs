using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour {
    //Public variables
    public Camera camera;
    public GameObject playerTracker;
    public float playerScaleFactor;

    //Local variables
    GameObject player = null;

    // Update is called once per frame
    void Update () {
        if (GameObject.FindGameObjectWithTag("Player")) player = GameObject.FindGameObjectWithTag("Player");

        float scaleFactor = GameInfo.width * playerScaleFactor;

        //Set player tracker position
        playerTracker.transform.position = player.transform.position;
        playerTracker.transform.rotation = player.transform.rotation;

        //Set camera size
        gameObject.transform.position = new Vector3(GameInfo.width, GameInfo.height, -1);
        camera.orthographicSize = GameInfo.height;
	}
}
