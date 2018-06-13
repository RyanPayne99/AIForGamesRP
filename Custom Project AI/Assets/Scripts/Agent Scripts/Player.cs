using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    //Constansts for direction values
    private const int RIGHT = 1;
    private const int DOWN = 2;
    private const int LEFT = 3;
    private const int UP = 0;

    //Public variables
    public Animator animator;
    public double speed;
    public float rotateSmoother;

    //Local variables
    //Variables for sprite speed calculation
    double moveHor;
    double moveVert;
    double direction;
    double oldDirection;
    double length;
    float xSpeed;
    float ySpeed;
	
	void FixedUpdate() {
        MovePlayer();
        RotatePlayer();
    }

    private void MovePlayer()
    {
        Vector3 movement = new Vector3();

        moveHor = Convert.ToInt32(Input.GetKey(KeyCode.RightArrow)) - Convert.ToInt32(Input.GetKey(KeyCode.LeftArrow));
        moveVert = Convert.ToInt32(Input.GetKey(KeyCode.UpArrow)) - Convert.ToInt32(Input.GetKey(KeyCode.DownArrow));
        
        //Determine the direction in degrees
        if (moveHor == 0 && moveVert == 0)
        {
            direction = oldDirection;
        }
        else
        {
            direction = Math.Atan2(moveHor, moveVert) * (180 / Math.PI);
            if (direction < 0) direction += 360;
            oldDirection = direction;
        }
        
        //Set speed if moving
        if (moveHor == 0 && moveVert == 0)
        {
            length = 0;
        }
        else
        {
            length = speed;
        }

        //Set movement speed vector
        xSpeed = (float)(Math.Sin(direction * Math.PI / 180) * length);
        ySpeed = (float)(Math.Cos(direction * Math.PI / 180) * length);
        movement = new Vector3(xSpeed, ySpeed, 0);
        
        //Move player
        gameObject.transform.position += movement;
    }

    private void RotatePlayer()
    {
        gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, Quaternion.Euler(0, 0, 360 - (float)direction), Time.deltaTime * rotateSmoother);

        //Stop animation when still
        if (length == 0)
        {
            animator.SetBool("Moving", false);
        }
        else
        {
            animator.SetBool("Moving", true);
        }
    }
}
