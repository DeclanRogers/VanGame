using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryNode : MonoBehaviour
{

    
    public bool isActive;

    public GameObject TriggerMesh;

    void Start()
    {

    }

    void Update()
    {
        if (isActive == true)
        {
            TriggerMesh.SetActive(true);
            gameObject.GetComponent<CapsuleCollider>().enabled = true;
        } else if (isActive == false)
        {
            TriggerMesh.SetActive(false);
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            NodeTriggered();
        }
    }

    public void NodeTriggered()
    {
        isActive = false;
        GameObject.Find("NodeManager").GetComponent<NodeManager>().NewNode();
    }
}
