using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CHARACTERS
{
    public class ACODE_Characters : MonoBehaviour
    {
        protected bool _IsFacingRight = true;

        public void CheckDirectionToFace(bool isMovingRight)
        {
            if (isMovingRight != _IsFacingRight)
                Turn();
        }

        private void Turn()
        {
            //stores scale and flips the player along the x axis, 
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;

            _IsFacingRight = !_IsFacingRight; // false
        }
    }
}
