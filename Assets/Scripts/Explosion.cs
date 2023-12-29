using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public void AnimationSize(float _size)
    {
        Vector3 scale = transform.localScale;
        scale *= _size/24.0f;
        transform.localScale = scale;
    }
    public void doEventDestroy()
    {
        Destroy(gameObject);
    }
}
