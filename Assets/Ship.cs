using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public Rigidbody rb;
    public float bouyancyDepth;
    public float bouyancyForce;
    public float waterDrag;
    public Rect boundingrect;
    public float shiftForce;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.AddForceAtPosition(Physics.gravity, transform.position, ForceMode.Acceleration);
        if (transform.position.y < bouyancyDepth)
        {
            float displacementMult = Mathf.Clamp01((bouyancyDepth - transform.position.y)) * bouyancyForce;
            rb.AddForce(new Vector3(0, Mathf.Abs(Physics.gravity.y) * displacementMult));

            rb.AddForce(-rb.velocity * Time.fixedDeltaTime * displacementMult * waterDrag, ForceMode.VelocityChange);
        }

        rb.AddForce(new Vector3(Random.Range(-1f, 1f)*shiftForce, 0, Random.Range(-1, 1)*shiftForce));
        if (transform.position.x < boundingrect.min.x)
            rb.AddForce(new Vector3(Mathf.Clamp01((boundingrect.min.x - transform.position.x)) * shiftForce, 0,0));
        if (transform.position.x > boundingrect.max.x)
            rb.AddForce(new Vector3(-Mathf.Clamp01((transform.position.x-boundingrect.max.x)) * shiftForce, 0, 0));
        if (transform.position.z < boundingrect.min.y)
            rb.AddForce(new Vector3(0,0,Mathf.Clamp01((boundingrect.min.y - transform.position.z)) * shiftForce));
        if (transform.position.z > boundingrect.max.y)
            rb.AddForce(new Vector3(0,0,-Mathf.Clamp01((transform.position.z - boundingrect.max.y)) * shiftForce));

    }
}
