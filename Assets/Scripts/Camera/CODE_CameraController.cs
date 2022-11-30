using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CODE_CameraController : MonoBehaviour
{
    public GameObject[] cameras;
    public float freezeTime = 1f;

    private void Awake()
    {
        Debug.Log(transform.childCount);
        cameras = new GameObject[transform.childCount];
        for(int i = 0; i < transform.childCount; i++)
        {
            cameras[i] = this.transform.GetChild(i).gameObject;
        }
    }

    public void EnableCamera(GameObject camera)
    {
        if (camera.activeInHierarchy)
            return;

        for(int i = 0; i < cameras.Length; i++)
        {
            cameras[i].SetActive(false);
        }

        camera.SetActive(true);

        StartCoroutine(FreezeTime());
    }

    IEnumerator FreezeTime()
    {
        Time.timeScale = 0;

        yield return new WaitForSecondsRealtime(freezeTime);

        Time.timeScale = 1;
    }
}
