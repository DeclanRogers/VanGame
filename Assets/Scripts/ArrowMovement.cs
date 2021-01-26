using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowMovement : MonoBehaviour
{
    public GameObject nodeManager;
    private NodeManager node;
  
    void Start()
    {
        node = nodeManager.GetComponent<NodeManager>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(node.currentNode.transform);
    }
}
