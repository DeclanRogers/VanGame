using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform m_Player;
    public float m_FollowSmoothing = 0.5f;
    public float m_RotationSmoothing = 0.2f;

    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, m_Player.position, m_FollowSmoothing);
        transform.rotation = Quaternion.Slerp(transform.rotation, m_Player.rotation, m_RotationSmoothing);
    }
}
