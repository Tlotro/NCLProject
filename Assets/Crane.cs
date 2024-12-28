using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Crane : MonoBehaviour
{
    public bool EnableExpertSystem;

    public Ship ship;
    public GameObject ContainerPrefab;
    public Rigidbody ContainerRb;
    public SpringJoint rope;
    public Rigidbody CraneRb;
    public Rigidbody RailRb;

    public InfluenceMode WindModeDirectionX;
    public float WindPeriodX;
    public InfluenceMode WindModeDirectionY;
    public float WindPeriodY;
    public InfluenceMode WindModeStrength;
    public float WindPeriodStrength;
    public Vector2 WindDirection;
    public Vector2 WindStrengthClamp;
    public float WindStrength;

    public float DescentSpeed;
    public Vector2 PlatformSpeed;

    private float WindTimerX;
    private float WindTimerY;
    private float WindTimerStrength;

    public Vector3Int ContainerGoal;
    public float LandingPrecision;
    public int[,] towers = new int[7,8];

    public void SetCurrentMode(int mode) { WindModeStrength = (InfluenceMode)mode; }
    public void SetCurrentStrength(float mode) { if (WindModeStrength == InfluenceMode.manual) WindStrength = mode; }
    public void SetCurrentModeX(int mode) { WindModeDirectionX = (InfluenceMode)mode; }
    public void SetCurrentX(float mode) { if (WindModeDirectionX == InfluenceMode.manual) WindDirection.x = mode; }
    public void SetCurrentModeY(int mode) { WindModeDirectionY = (InfluenceMode)mode; }
    public void SetCurrentY(float mode) { if (WindModeDirectionY == InfluenceMode.manual) WindDirection.y = mode; }

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
        if (rope.connectedBody != null)
        {
            WindTimerX += (Time.fixedDeltaTime / WindPeriodX) % (Mathf.PI * 2);
            WindTimerY += (Time.fixedDeltaTime / WindPeriodY) % (Mathf.PI * 2);
            WindTimerStrength += (Time.fixedDeltaTime / WindPeriodStrength) % (Mathf.PI * 2);
            switch (WindModeDirectionX)
            {
                case InfluenceMode.random:
                    WindDirection.x = Random.Range(-1f, 1f);
                    break;
                case InfluenceMode.sine:
                    WindDirection.x = Mathf.Sin(WindTimerX);
                    break;
            }
            switch (WindModeDirectionY)
            {
                case InfluenceMode.random:
                    WindDirection.y = Random.Range(-1f, 1f);
                    break;
                case InfluenceMode.sine:
                    WindDirection.y = Mathf.Sin(WindTimerY);
                    break;
            }
            switch (WindModeStrength)
            {
                case InfluenceMode.random:
                    WindStrength = Random.Range(WindStrengthClamp.x, WindStrengthClamp.y);
                    break;
                case InfluenceMode.sine:
                    WindStrength = (Mathf.Sin(WindTimerStrength) + 1) / 2 * (WindStrengthClamp.y - WindStrengthClamp.x) + WindStrengthClamp.x;
                    break;
            }

            ContainerRb.AddForce(WindDirection.normalized.AsXZ() * WindStrength, ForceMode.Acceleration);
            Debug.Log((ContainerRb.position.x - ship.transform.position.x) / 6 + 3.5f + " " + ((ContainerRb.position.z - ship.transform.position.z) / 3 + 4));
            if (EnableExpertSystem) {
            Dictionary<string, float> parameters = new Dictionary<string, float>
            {
                { "CargoDistanceX", (ContainerRb.position.x-ship.transform.position.x)+49f-ContainerGoal.x*2 },
                { "CargoDistanceY", ContainerRb.position.y - ship.transform.position.y-5.5f-ContainerGoal.y },
                { "CargoDistanceZ", ContainerRb.position.z - ship.transform.position.z+9.5f-ContainerGoal.z }
            };
            var res = ExpertSystem.Run(parameters);
            DescentSpeed = res["DescentSpeed"];
            CraneRb.velocity = new Vector2(res["CraneSpeedX"], res["CraneSpeedZ"]).AsXZ();
            }
            rope.maxDistance += DescentSpeed * Time.fixedDeltaTime;
        }
        else
        {
            RailRb.velocity = new Vector3(-Mathf.Clamp(RailRb.position.x, -5, 5),0,0);
            CraneRb.velocity = new Vector3(0,0,-Mathf.Clamp(transform.position.z, -5, 5));
            if (CraneRb.velocity.magnitude < 0.1f)
            {
                transform.localPosition = new Vector3(0, 1.5f, 0);
                RailRb.position = new Vector3(0, 45.5f, 0);
                CraneRb.velocity = Vector3.zero;
                RailRb.velocity = Vector3.zero;
                CreateContainer();
            }
        }
    }

    public void CreateContainer()
    {
        var container = Object.Instantiate(ContainerPrefab, transform);
        container.transform.localPosition = new Vector3(0, -1, 0);
        ContainerRb = container.GetComponent<Rigidbody>();
        rope.connectedBody = ContainerRb;
        rope.maxDistance = 7;
    }

    public void DetachContainer()
    {
        rope.connectedBody = null;
        rope.maxDistance = 0;
        ContainerRb.transform.parent = ship.transform;
        Destroy(ContainerRb);
        ContainerRb = null;
    }

    public void Land()
    {
        Vector3 landPos = new Vector3((ContainerRb.position.x-ship.transform.position.x)/6+3.5f,(ContainerRb.position.y - ship.transform.position.y)/3-0.67f, (ContainerRb.position.z - ship.transform.position.z)/3+4);
        Debug.Log(landPos + " " + (landPos - ContainerGoal));
        if ((landPos - ContainerGoal).magnitude > LandingPrecision)
            Debug.LogWarning("Container collision or improper location!");
        DetachContainer();
        RerollGoal();
        Debug.Log(Mathf.RoundToInt(landPos.x) + " " + Mathf.RoundToInt(landPos.z));
        towers[Mathf.RoundToInt(landPos.x), Mathf.RoundToInt(landPos.z)]++;
    }   
    
    public void RerollGoal()
    {
        int x = Random.Range(0, 7);
        int z = Random.Range(0, 8);
        Debug.Log(x + " " + z);
        ContainerGoal = new Vector3Int(x, towers[x,z] ,z);
    }
}

public static class VectorExtensions
{
    public static Vector2 AsXZ(this Vector3 v3) => new Vector2(v3.x, v3.z);
    public static Vector3 AsXZ(this Vector2 v2) => new Vector3(v2.x, 0f, v2.y);
}
