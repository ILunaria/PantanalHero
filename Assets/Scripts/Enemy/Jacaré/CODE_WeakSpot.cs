using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CODE_WeakSpot : MonoBehaviour
{
    public int hitCount = 0;


    [SerializeField] Animator anim;
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Attack"))
        {
            if (hitCount > 2)
            {
                anim.Play("ANIM_Defeat");
            }
            else
            {
                hitCount++;
                anim.Play("Hit");
            }
            
            gameObject.SetActive(false);
        }
      
    }
}
