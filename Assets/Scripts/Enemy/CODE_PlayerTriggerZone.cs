using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CHARACTERS
{
    public class CODE_PlayerTriggerZone : MonoBehaviour
    {
        public bool isInsideOfRange;
        public GameObject collisionTarget;

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.name == "Player")
            {
                collisionTarget = collision.gameObject;
                isInsideOfRange = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if(collision.name ==  "Player")
            {
                collisionTarget = null;
                isInsideOfRange = false;
            }
           
        }
    }

}
