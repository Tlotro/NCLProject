using UnityEngine;
using UnityEngine.UI;

public class Crane : MonoBehaviour
{
    [SerializeField] private Toggle randomWindToggle;

    [SerializeField] private Slider sliderStrength;

    [SerializeField] private Slider sliderDirectionX;
    [SerializeField] private Slider sliderDirectionY;

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
        RandomWind = randomWindToggle.isOn;
        WindStrength = sliderStrength.value;

        if (!RandomWind)
        {
            WindDirection.x = sliderDirectionX.value;
            WindDirection.y = sliderDirectionY.value;
        }
        else
        {
            sliderDirectionX.interactable = false;
            sliderDirectionY.interactable = false;
        }

        randomWindToggle.onValueChanged.AddListener(OnToggleValueChanged);

        sliderStrength.onValueChanged.AddListener(OnSliderStrengthValueChanged);

        sliderDirectionX.onValueChanged.AddListener(OnSliderDirectionXValueChanged);
        sliderDirectionY.onValueChanged.AddListener(OnSliderDirectionYValueChanged);
    }

    private void OnToggleValueChanged(bool value)
    {
        RandomWind = value;

        if (!RandomWind)
        {
            sliderDirectionX.interactable = true;
            sliderDirectionY.interactable = true;
        }
        else
        {
            sliderDirectionX.interactable = false;
            sliderDirectionY.interactable = false;
        }

    }

    private void OnSliderStrengthValueChanged(float value)
    {
        WindStrength = value;
    }

    private void OnSliderDirectionXValueChanged(float value)
    {
        WindDirection.x = value;
    }

    private void OnSliderDirectionYValueChanged(float value)
    {
        WindDirection.y = value;
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
