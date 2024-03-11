using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCamera : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float rotateSpeed = 5;
    private Vector3 offset;
    private void Start()
    {
        offset = player.transform.position - transform.position;
    }

    void Update()
    {
        float h = Input.GetAxis("Mouse X") * rotateSpeed; 
        player.transform.Rotate(0,h,0);

        float angle = player.transform.eulerAngles.y;
        Quaternion rotation = Quaternion.Euler(0,angle,0);
        transform.position = player.transform.position - (rotation * offset);
        
        transform.LookAt(player.transform.position + new Vector3(0,1,0));
    }

}
