using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CHARACTERS
{
    public class CODE_Jacare : ACODE_Characters
    {
        public void ScreenShake()
        {
            CODE_EventManager.current.ScreenShakeCallback();
        }
        public void Defeat()
        {
            CODE_EventManager.current.OnDefeat();
            Destroy(gameObject);
        }
    }

}
