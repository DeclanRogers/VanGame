using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{

    public GameObject[] nodes;
    public int nodeNumber;
    public GameObject currentNode;

    void Start()
    {
        NewNode();
    }


    void Update()
    {
        
    }

    public void NewNode()
    {
        nodeNumber = Random.Range(1, nodes.Length);
        currentNode = nodes[nodeNumber];

        currentNode.gameObject.GetComponent<DeliveryNode>().isActive = true;
    }
}
