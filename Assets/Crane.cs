using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crane : MonoBehaviour
{
    public Rigidbody ContainerRb;
    public SpringJoint rope;

    public bool RandomWind;
    public Vector2 WindDirection;
    public float WindStrength;

    public Rigidbody CraneRb;
    public float DescentSpeed;
    public Vector2 platformSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        CraneRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (RandomWind)
            WindDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        ContainerRb.AddForce(WindDirection.AsXZ() * WindStrength,ForceMode.Acceleration);

        rope.maxDistance += DescentSpeed*Time.fixedDeltaTime;
        CraneRb.velocity = platformSpeed.AsXZ();
    }
}

public static class VectorExtensions
{
    public static Vector2 AsXZ(this Vector3 v3) => new Vector2(v3.x, v3.z);
    public static Vector3 AsXZ(this Vector2 v2) => new Vector3(v2.x, 0f, v2.y);
}
