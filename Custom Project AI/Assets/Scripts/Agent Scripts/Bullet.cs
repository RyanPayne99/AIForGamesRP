using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    //Public variables
    public GameObject payload;
    public float mass;
    public float maxSpeed;

    //Local varaibles
    Payload p;
    Vector3 velocity;
    Vector3 desiredVel;
    Vector3 predictedLoc;

    void Start () {
        if (GameObject.FindGameObjectWithTag("Payload"))
        {
            payload = GameObject.FindGameObjectWithTag("Payload");
            p = payload.GetComponent<Payload>();
        }

        //Set default values
        gameObject.transform.position = gameObject.transform.parent.position;
        velocity = new Vector3();
        desiredVel = new Vector3();
        predictedLoc = new Vector3();
	}
	
	void FixedUpdate () {
        //Get bullet velocity and direction and clamp to max
        ShootBullet();
        Vector3.ClampMagnitude(velocity, maxSpeed);

        //Move bullet
        gameObject.transform.position += velocity * Time.deltaTime;

        //If bullet goes off the world, destroy it
        //if (OffWorld(gameObject.transform.position) && p.bullets.Contains(gameObject))
            //DestroyBullet();
    }

    private void ShootBullet()
    {
        if (p.target)
        {
            Enemy e = p.target.GetComponent<Enemy>();

            //Get desired velocity
            if (desiredVel == Vector3.zero)
            {
                Vector3 predict = e.velocity * (Vector3.Distance(gameObject.transform.position, p.target.transform.position) / maxSpeed);

                predictedLoc = p.target.transform.position + predict;
                desiredVel = (predictedLoc - gameObject.transform.position) * maxSpeed;
                velocity = desiredVel;
            }

            foreach (var enemy in GameInfo.enemies)
            {
                //If bullet hits enemy, destroy enemy
                if (Vector3.Distance(gameObject.transform.position, enemy.transform.position) < 1 && GameInfo.enemies.Contains(enemy))
                {
                    DestroyBullet();
                    GameInfo.enemies.Remove(enemy);
                    Destroy(enemy);
                }
            }
        }
        else
            DestroyBullet();
    }

    private void DestroyBullet()
    {
        p.bullets.Remove(gameObject);
        Destroy(gameObject);
    }

    private bool OffWorld(Vector3 pos)
    {
        //If position is off the world return true, else false
        if (pos.x > GameInfo.width * 2)
            return true;
        else if (pos.x < 0)
            return true;
        else if (pos.y > GameInfo.height * 2)
            return true;
        else if (pos.y > 0)
            return true;

        return false;
    }
}
