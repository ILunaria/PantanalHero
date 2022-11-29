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
    }

}
