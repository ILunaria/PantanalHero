using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CODE_CameraTrigger : MonoBehaviour
{
    public GameObject myCamera;
    public CODE_CameraController controller;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            controller.EnableCamera(myCamera);
        }
    }
}
