using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    //Public variables
    public GameObject player;
    public string mode;
    public float mass;
    public float maxSpeed;
    public float maxForce;
    public float radius;
    public float sepWeight;
    public float aliWeight;
    public float cohWeight;
    [HideInInspector]
    public Vector3 velocity;


    //Local variables
    Vector3 heading;
    Vector3 side;
    Vector3 force;
    Vector3 accel;

    bool tagged;

    //Public wander variables (NOT FUNCTIONAL)
    public float wanderDist;
    public float wanderRadius;
    public float wanderJitter;
    public float bRadius;
    //Wander variables (NOT FUNCTIONING)
    Vector3 wanderTarget;
    

	void Start () {
        //General allocations
        if (GameObject.FindGameObjectWithTag("Player")) player = GameObject.FindGameObjectWithTag("Player");
        System.Random rnd = new System.Random();

        //Set starting values of the enemy
        float dir = Mathf.Deg2Rad * (float)(rnd.NextDouble() * 360);
        velocity = new Vector3();
        heading = new Vector3(Mathf.Sin(dir), Mathf.Cos(dir));
        side = new Vector3(-heading.y, heading.x);
        force = new Vector3();
        accel = new Vector3();
         
        //Set the wander values for the enemy (NOT FUNCTIONING)
        wanderTarget = new Vector3(1, 0);

        //Set group behaviour items
        tagged = false;
	}
	
	void FixedUpdate () {
        if (!player) return;

        //Calculate for to be applied, and clamp to max force
        force = Calculate(Time.deltaTime);
        Vector3.ClampMagnitude(force, maxForce);

        //Calculate new acceleration
        accel = force / mass;

        //Calculate new velocity and clamp to max
        velocity += accel * Time.deltaTime;
        Vector3.ClampMagnitude(velocity, maxSpeed);

        //Move enemy by velocity
        gameObject.transform.position += velocity * Time.deltaTime;

        //Update heading and side
        if (velocity.sqrMagnitude > 0.000000001)
        {
            heading = velocity.normalized;
            side = new Vector3(-heading.y, heading.x);
        }
    }

    private Vector3 Calculate(float delta)
    {
        Vector3 resultantForce;
        switch(mode)
        {
            case "pursuit":
                TagNeighbours();
                var pursuit = Seek(player.transform.position);
                var sep = Separation() * sepWeight;
                var ali = Alignment() * aliWeight;
                var coh = Cohesion() * cohWeight;
                resultantForce = sep + ali + coh + pursuit; 
                break;
            // NOT FUNCTIONAL
            //case "wander": 
            //    resultantForce = Wander(delta);
            //    break;
            default:
                resultantForce = new Vector3();
                break;
        }

        return resultantForce;
    }

    private Vector3 Seek(Vector3 targetPosition)
    {
        Vector3 desiredVel = (targetPosition - gameObject.transform.position).normalized * maxSpeed;

        return desiredVel - velocity;
    }

    private void TagNeighbours()
    {
        foreach(var enemy in GameInfo.enemies)
        {
            Enemy e = enemy.GetComponent<Enemy>();

            e.tagged = false;
            if (e != this)
            {
                Vector3 distBetween = gameObject.transform.position - enemy.transform.position;

                if (distBetween.sqrMagnitude < Mathf.Pow(radius, 2))
                    e.tagged = true;
            }
        }
    }

    private Vector3 Separation()
    {
        Vector3 steerForce = new Vector3();
        Vector3 sumPos = new Vector3();
        int count = 0;

        foreach(var enemy in GameInfo.enemies)
        {
            Enemy e = enemy.GetComponent<Enemy>();

            float d = Vector3.Distance(gameObject.transform.position, enemy.transform.position);

            if (e != this && e.tagged)
            {
                Vector3 toTarget = gameObject.transform.position - enemy.transform.position;
                sumPos += toTarget.normalized / d;
                count += 1;
            }
        }

        if (count > 0)
        {
            sumPos = (sumPos / count).normalized * maxSpeed;
            steerForce = sumPos - velocity;
        }

        return steerForce;
    }

    private Vector3 Alignment()
    {
        Vector3 steerForce = new Vector3();
        Vector3 sumHeading = new Vector3();
        int count = 0;

        foreach (var enemy in GameInfo.enemies)
        {
            Enemy e = enemy.GetComponent<Enemy>();

            if (e != this && e.tagged)
            {
                sumHeading += e.heading;
                count += 1;
            }
        }

        if (count > 0)
        {
            sumHeading = (sumHeading / count).normalized * maxSpeed;
            steerForce = sumHeading - velocity;
        }

        return steerForce;
    }

    private Vector3 Cohesion()
    {
        Vector3 steerForce = new Vector3();
        Vector3 sumPos = new Vector3();
        int count = 0;

        foreach (var enemy in GameInfo.enemies)
        {
            Enemy e = enemy.GetComponent<Enemy>();

            if (e != this && e.tagged)
            {
                sumPos += enemy.transform.position;
                count += 1;
            }
        }

        if (count > 0)
        {
            sumPos = sumPos / count;
            steerForce = Seek(sumPos);
        }

        return steerForce;
    }


    //NOT FUNCTIONAL
    private Vector3 Wander(float delta)
    {
        //Get a small random value to add to the target
        float jitterTts = wanderJitter * delta;

        //Add value to target and normalise
        wanderTarget += new Vector3(Random.Range(-1, 1) * jitterTts, Random.Range(-1, 1) * jitterTts);
        wanderTarget.Normalize();

        wanderTarget *= wanderRadius;
        Vector3 target = wanderTarget + new Vector3(wanderDist, 0);


        Vector3 worldTarget = transform.TransformPoint(target);

        return Seek(worldTarget);
    }
}
