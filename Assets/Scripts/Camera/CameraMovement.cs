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


    //[SerializeField] Vector3 newPosition;

    // Update is called once per frame
    //private void Awake()
    //{
    //    cam = GameObject.Find("PlayerFollowCam").transform;
    //}
    //private void CamMove()
    //{
    //    Vector2 actualPosition = new Vector2(cam.position.x, cam.position.y);
    //    cam.position = new Vector3(newPosition.x , newPosition.y, newPosition.z);
    //}

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if(collision.gameObject.tag == "Player")
    //    {
    //        CamMove();
    //    }
    //}
}
