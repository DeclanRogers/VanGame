using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanFlip : MonoBehaviour
{
    public VanController controller;

    bool flipped = false;
    bool onButt = false;
    float flipSpeed;

    public float flippedRotation = 45; //the angle at which the van is considdered flipped and needs to be put back on it's wheels
    public float regularFlipSpeed = 1;
    public float boostFlipSpeed = 5;

    public float lineCastLength = 2;

    void Start()
    {
        controller = GetComponent<VanController>();
    }

    void Update()
    {
        int layerMask = 1 << 8;
        layerMask = ~layerMask;

        ///drawing Line Casts for roll over checking
        Debug.DrawLine(transform.position, transform.position - transform.right * lineCastLength, Color.blue);
        Debug.DrawLine(transform.position, transform.position + transform.right * lineCastLength, Color.blue);
        Debug.DrawLine(transform.position, transform.position + transform.up * lineCastLength, Color.blue);
        Debug.DrawLine(transform.position, transform.position - transform.forward * lineCastLength * 2, Color.blue);

        //checking if the vehicle has rolled over past the public variable flippedRotation
        if ((transform.rotation.eulerAngles.z < 360 - flippedRotation && transform.rotation.eulerAngles.z > 180) || (transform.rotation.eulerAngles.z < 180 && transform.rotation.eulerAngles.z > flippedRotation))
        {
            //checks if vehicle is also on the ground
            if (Physics.Linecast(transform.position, transform.position - transform.right * lineCastLength, layerMask) || Physics.Linecast(transform.position, transform.position + transform.right * lineCastLength, layerMask) || Physics.Linecast(transform.position, transform.position + transform.up * lineCastLength, layerMask))
                flipped = true;
        }
        //checking if vehicle's butt is colliding with an object
        else if (Physics.Linecast(transform.position, transform.position - transform.forward * lineCastLength * 2, layerMask))
        {
            //check if this is due to the vehicle being flipped onto it's butt
            if (transform.rotation.eulerAngles.x == 270)
            {
                onButt = true;
                flipped = true;
            }
        }

        //check if vehicle is currently flipped and user is trying to boost
        if (flipped && Input.GetKey(KeyCode.LeftShift))
        {
            //calculate flip rotation depending on which side the vehicle has flipped over
            flipSpeed = (flipped && transform.rotation.eulerAngles.z < 180 ? -boostFlipSpeed : boostFlipSpeed);
            //check if the vehicle has flipped on it's butt rather than it's side/back
            if (onButt)
            {
                //flip around x axis
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x - flipSpeed, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
            }
            else 
            {
                //flip around z axis
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + flipSpeed);
            }

        }
        //check if vehicle is currently flipped and user is trying to move forward but NOT boost
        if (flipped && Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.LeftShift))
        {
            //calculate flip rotation depending on which side the vehicle has flipped over
            flipSpeed = (flipped && transform.rotation.eulerAngles.z < 180 ? -regularFlipSpeed : regularFlipSpeed);
            //check if the vehicle has flipped on it's butt rather than it's side/back
            if (onButt)
            {
                //flip around x axis
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x - flipSpeed, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
            }
            else
            {
                //flip around z axis
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + flipSpeed);
            }

        }
        //if vehicle is on it's butt, check that it is back on it's wheels in terms of x rotation
        if (onButt)
        {
            if(flipped && transform.rotation.eulerAngles.x <= 3 || transform.rotation.eulerAngles.x >= 357)
            {
                //if vehicle is very close to upright, allow it to be upright
                transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
                flipped = false;
                onButt = false;
            }
        }
        //if vehicle is on it's side/bacl, check that it is back on it's wheels in terms of z rotation
        else if (flipped && transform.rotation.eulerAngles.z <= 3 || transform.rotation.eulerAngles.z >= 357)
        {
            //if vehicle is very close to upright, allow it to be upright
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
            flipped = false;
        }

    }
}
