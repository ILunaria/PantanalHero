using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CODE_BossDeathCheck : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Death");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
