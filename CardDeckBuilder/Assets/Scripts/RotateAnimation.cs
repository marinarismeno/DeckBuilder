using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAnimation : MonoBehaviour
{
    public float speed = -300;
    void Update()
    {
        transform.Rotate(0, 0, speed * Time.deltaTime);
    }
}
