using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraControl : MonoBehaviour
{
    public RectTransform UI;
    // Start is called before the first frame update
    void Start()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(UI);
        
    }
    void FixedUpdate()
    {
        
        transform.RotateAround(Vector3.zero,Vector3.up, -Input.GetAxisRaw("Horizontal"));
    }


}
