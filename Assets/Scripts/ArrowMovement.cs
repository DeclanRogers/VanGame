﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowMovement : MonoBehaviour
{
    public GameObject pointAt;
    private GameObject player;
    public GameObject nodeManager;
    public NodeManager node;
  
    void Start()
    {
        node = nodeManager.GetComponent<NodeManager>();
        //player = GameObject.FindGameObjectWithTag("Player");
        pointAt.GetComponent<Transform>();
        //transform.position = new Vector3 (player.transform.position.x,player.transform.position.y + arrowHeight,player.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = player.transform.localPosition;
        transform.LookAt(node.currentNode.transform);
        //this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, pointAt.transform.rotation, 1* Time.deltaTime);
        /*        currentPos = new Vector3(player.transform.position.x, player.transform.position.y + 2.5f, player.transform.position.z);
                transform.position = currentPos;*/
    }
}