using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanFlip : MonoBehaviour
{
    bool flipped = false;
    float flipSpeed;

    public float flippedRotation = 45; //the angle at which the van is considdered flipped and needs to be put back on it's wheels
    public float regularFlipSpeed = 1;
    public float boostFlipSpeed = 5;
    [HideInInspector]
    public bool canDrive = true; // allows the Van to stop moving forward when it is sideways enough to loose traction
    void Start()
    {
        
    }

    void Update()
    {
        if((transform.rotation.eulerAngles.z < 360-flippedRotation && transform.rotation.eulerAngles.z > 180) || (transform.rotation.eulerAngles.z < 180 && transform.rotation.eulerAngles.z > flippedRotation))
        {
            canDrive = false;
            flipped = true;
        }


        if (flipped && Input.GetKey(KeyCode.LeftShift))
        {
            flipSpeed = (flipped && transform.rotation.eulerAngles.z < 180 ? -boostFlipSpeed : boostFlipSpeed);
            if (transform.rotation.eulerAngles.z != 0)
            {
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + flipSpeed);
            }
        }
        if (flipped && Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.LeftShift))
        {
            flipSpeed = (flipped && transform.rotation.eulerAngles.z < 180 ? -regularFlipSpeed : regularFlipSpeed);
            if (transform.rotation.eulerAngles.z != 0)
            {
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + flipSpeed);
            }
        }
        if (flipped && transform.rotation.eulerAngles.z <= 3 || transform.rotation.eulerAngles.z >= 357)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
            flipped = false;
            canDrive = true;
        }
    }
}
