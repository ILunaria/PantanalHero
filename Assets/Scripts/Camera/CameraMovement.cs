using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform player;
    public float dampTime = 0.4f;

    private Vector3 cameraPos;
    private Vector3 velocity = Vector3.zero;

    public float cameraOffsetX;
    public float cameraOffsetY;


    // Update is called once per frame
    void Update()
    {
        cameraPos = new Vector3(player.position.x, player.position.y + cameraOffsetY, -10f);

        transform.position = Vector3.SmoothDamp(this.transform.position, cameraPos, ref velocity, dampTime);
    }
}
