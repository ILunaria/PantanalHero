using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimAttack : MonoBehaviour
{
    // Start is called before the first frame update
    private void DestroyThis()
    {
        Destroy(transform);
    }
}
