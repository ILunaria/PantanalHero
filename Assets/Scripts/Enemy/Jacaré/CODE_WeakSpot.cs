using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CODE_WeakSpot : MonoBehaviour
{
    public int hitCount = 0;
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Attack"))
        {
            if (hitCount > 2)
            {
                Debug.Log("Winner");
            }
            else
            {
                hitCount++;
            }

            gameObject.SetActive(false);
        }
      
    }
}
