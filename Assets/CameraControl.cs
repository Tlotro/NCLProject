using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void FixedUpdate()
    {
        
        transform.RotateAround(Vector3.zero,Vector3.up, -Input.GetAxisRaw("Horizontal"));
    }


}
