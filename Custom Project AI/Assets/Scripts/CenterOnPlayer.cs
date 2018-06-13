using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterOnPlayer : MonoBehaviour {
    
	void Update () {
        //Check if player exists
		if (GameObject.FindGameObjectWithTag("Player"))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            //Centre camera on player
            gameObject.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -1);
        }
	}
}
