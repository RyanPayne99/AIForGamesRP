using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    
    //Public variables
    public Text roundCount;
    public Text killCount;
    public Text collectableCount;

    //Local variables
    int totalCollectable;

    public void Initiate() {
        totalCollectable = GameInfo.coinTiles.Count;
	}
	
	void Update () {
        collectableCount.text = "Collectables:  " + GameInfo.collectedCount + " / " + totalCollectable;
	}
}
