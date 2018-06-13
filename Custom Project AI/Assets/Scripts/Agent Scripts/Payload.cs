using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Payload : MonoBehaviour {

    //Public variables
    public Animator animator;
    [HideInInspector]
    public List<GameObject> bullets = new List<GameObject>();
    [HideInInspector]
    public GameObject target = null;

    //Local variables
    bool start = false;
    int count = 1;
    float totalTripLength;
    float turnsRemaining = 0f;
    Vector3 newPosition;
    GameObject targetCollectable;

    //Bullet variables
    public int targetRadius;
    string mode = "search";
    string attackMode = "shooting";
    int attackCount = 0;
    int bulletCount = 1;
    
    void Update () {
        //When spacebar is pressed set default values and plan search path
		if (Input.GetKeyDown(KeyCode.Space) && !start)
        {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<LevelBuilder>().SpawnEnemies();
            GameInfo.PlanPath();
            count = 1;
            start = true;
            newPosition = gameObject.transform.position;
        }

        //Start when spacebar is pressed
        if (start)
        {
            //Check if enemies are close
            if (GameInfo.enemies.Count > 0)
            {
                foreach (var enemy in GameInfo.enemies)
                {
                    if (Vector3.Distance(gameObject.transform.position, enemy.transform.position) < targetRadius)
                    {
                        target = enemy;
                        mode = "attack";
                    }
                    else mode = "search";
                }
            }
            else mode = "search";

            //Check if the path was complete
            if (GameInfo.path == null) return;
            if (GameInfo.path.result != "Success!") return;

            if (count < GameInfo.path.pathPoints.Count)
            {
                UpdatePayload();
                //Increase node tracker
                if (turnsRemaining <= 0)
                    count += 1;
            }

            if (mode == "attack")
            {
                Attack();
            }
        }
	}

    private void UpdatePayload()
    {
        //Function variables
        List<int> path = GameInfo.path.pathPoints;
        Vector3 src = GameInfo.tiles[path[count - 1]].transform.position;
        Vector3 dest = GameInfo.tiles[path[count]].transform.position;

        RotatePayload(path[count - 1], path[count]);

        //Calculate total length of the trip from node to node
        totalTripLength = Vector3.Distance(src, dest);

        if (turnsRemaining <= 0)
            turnsRemaining = totalTripLength;
        
        //Check if player has reached next node or not
        if (Vector3.Distance(gameObject.transform.position, newPosition) == 0)
        {
            turnsRemaining -= 1;
            newPosition = new Vector3(src.x + (dest.x - src.x), src.y + (dest.y - src.y), 0);
            
            CollectableCheck(GameInfo.path.midpoindIdx);
        }
        //Smoothly move payload along nodes
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, newPosition, 0.05f);
    }

    private void RotatePayload(int startIdx, int nextTile)
    {
        Quaternion rotation;
        
        if (nextTile == startIdx - 1) //Left
        {
            rotation = Quaternion.Euler(0, 0, 90);
        }
        else if (nextTile == startIdx + 1) //Right
        {
            rotation = Quaternion.Euler(0, 0, 270);
        }
        else if (nextTile == startIdx + GameInfo.width) //Up
        {
            rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (nextTile == startIdx - GameInfo.width) //Down
        {
            rotation = Quaternion.Euler(0, 0, 180);
        }
        else if (nextTile == startIdx + (GameInfo.width - 1)) //Up-Left
        {
            rotation = Quaternion.Euler(0, 0, 45);
        }
        else if (nextTile == startIdx + (GameInfo.width + 1)) //Up-Right
        {
            rotation = Quaternion.Euler(0, 0, 315);
        }
        else if (nextTile == startIdx - (GameInfo.width + 1)) //Down-Left
        {
            rotation = Quaternion.Euler(0, 0, 135);
        }
        else if (nextTile == startIdx - (GameInfo.width - 1)) //Down-Right
        {
            rotation = Quaternion.Euler(0, 0, 225);
        }
        else
        {
            rotation = Quaternion.Euler(0, 0, 90);
        }

        gameObject.transform.rotation = rotation;
    }

    public void CollectableCheck(int collectableIdx)
    {
        //Get the collectable targeted
        if (!targetCollectable)
        {
            foreach (var collectable in GameInfo.coinTiles)
            {
                Tile t = collectable.GetComponent<Tile>();
                if (t.id == collectableIdx)
                {
                    targetCollectable = collectable;
                }
            }
        }
        else
        {
            //Check if the payload has reached the collectable
            if (Vector3.Distance(gameObject.transform.position, targetCollectable.transform.position) == 0)
            {
                //If so change sprite and remove coin
                animator.SetBool("Collected", true);
                GameInfo.coinTiles.Remove(targetCollectable);

                Tile t = targetCollectable.GetComponent<Tile>();
                DestroyImmediate(t.contentObject);
                targetCollectable = null;
            }
        }

        //Check if payload is back at home
        if (animator.GetBool("Collected") && gameObject.transform.position == GameInfo.startPosition)
        {
            //If so change sprite and add to score, destroy all remaining enemies
            animator.SetBool("Collected", false);
            GameInfo.collectedCount += 1;
            start = false;

            foreach (var enemy in GameInfo.enemies)
            {
                Destroy(enemy);
            }
            GameInfo.enemies.Clear();
        }
    }

    private void Attack()
    {
        //See what attack mode the payload is in
        switch (attackMode)
        {
            case "shooting":
                AttackShoot();
                break;
            case "reloading":
                AttackReload();
                break;
        }
    }

    private void AttackShoot()
    {
        if (!target) return;

        //Delay between bullets
        attackCount += 1;
        if (attackCount >= 5)
        {
            attackCount = 0;
            bulletCount -= 1;

            //Shoot new bullet
            bullets.Add(Instantiate((GameObject)Resources.Load("Prefabs/Bullet", typeof(GameObject)),
                gameObject.transform.position,
                new Quaternion(),
                gameObject.transform));

            //If out of bullets, reload
            if (bulletCount == 0)
                attackMode = "reloading";
        }
    }

    private void AttackReload()
    {
        attackCount += 1;

        if (attackCount == 5)
        {
            bulletCount += 1;
            attackCount = 0;
        }

        if (bulletCount == 1)
            attackMode = "shooting";
    }
}
