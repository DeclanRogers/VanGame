using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{

    public GameObject[] nodes;

    void Start()
    {
        NewNode();
    }


    void Update()
    {
        
    }

    public void NewNode()
    {
        int i = Random.Range(1, nodes.Length);

        nodes[i].gameObject.GetComponent<DeliveryNode>().isActive = true;
    }
}
