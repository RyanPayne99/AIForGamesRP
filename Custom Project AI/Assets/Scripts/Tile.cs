using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    //Public variables (to prevent changing values in Unity inspector)
    [HideInInspector]
    public int id;
    [HideInInspector]
    public Node node;
    [HideInInspector]
    public GameObject contentObject;
    public string kind;
    public string content;

    public void Initiate(string of_kind, string of_content)
    {
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        GameObject contentPrefab = null;

        if (GameInfo.tileKinds.Contains(of_kind))
        {
            kind = of_kind.ToLower();
            spriteRenderer.sprite = (Sprite)Resources.Load("Sprites/" + kind, typeof(Sprite));
        }

        content = of_content;
        if (gameObject.transform.position == GameInfo.middlePosition) content = "player";
        else if (gameObject.transform.position == GameInfo.basePosition) content = "base";
        else if (gameObject.transform.position == GameInfo.startPosition) content = "payload";

        switch(content)
        {
            case "player":
                contentPrefab = (GameObject)Resources.Load("Prefabs/Player", typeof(GameObject));
                break;
            case "base":
                contentPrefab = (GameObject)Resources.Load("Prefabs/Base", typeof(GameObject));
                break;
            case "payload":
                contentPrefab = (GameObject)Resources.Load("Prefabs/Payload", typeof(GameObject));
                break;
            case "coin":
                contentPrefab = (GameObject)Resources.Load("Prefabs/Coin", typeof(GameObject));
                break;
            case "wall":
                float rotation = Random.value;

                if (rotation < 0.25) contentPrefab = (GameObject)Resources.Load("Prefabs/Bags", typeof(GameObject));
                else if (rotation < 0.5) contentPrefab = (GameObject)Resources.Load("Prefabs/Barrel Hor", typeof(GameObject));
                else if (rotation < 0.75) contentPrefab = (GameObject)Resources.Load("Prefabs/Barrel Vert", typeof(GameObject));
                else contentPrefab = (GameObject)Resources.Load("Prefabs/Barrel Group", typeof(GameObject));
                
                break;
        }

        if (contentPrefab != null)
        {
            contentObject = Instantiate(contentPrefab, gameObject.transform);
            contentObject.transform.position = gameObject.transform.position;
        }

        if (content == "wall" || content == "base") kind = "wall";
    }
	
	void Update () {
		
	}
}
