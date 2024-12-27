using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class Ship : MonoBehaviour
{
    [SerializeField] private Toggle randomWavesToggle;

    [SerializeField] private Slider sliderStrength;

    [SerializeField] private Slider sliderDirectionX;
    [SerializeField] private Slider sliderDirectionY;


    public Rigidbody rb;
    public float bouyancyDepth;
    public float bouyancyForce;
    public float waterDrag;

    public bool RandomWaves;
    public Vector2 WaveDirection;
    public float WaveStrength;
    public Rect boundingrect;
    
    // Start is called before the first frame update
    void Start()
    {
        RandomWaves = randomWavesToggle.isOn;

        WaveStrength = sliderStrength.value;

        if (!RandomWaves)
        {
            WaveDirection.x = sliderDirectionX.value;
            WaveDirection.y = sliderDirectionY.value;
        }
        else
        {
            sliderDirectionX.interactable = false;
            sliderDirectionY.interactable = false;
        }

        randomWavesToggle.onValueChanged.AddListener(OnToggleValueChanged);

        sliderStrength.onValueChanged.AddListener(OnSliderStrengthValueChanged);

        sliderDirectionX.onValueChanged.AddListener(OnSliderDirectionXValueChanged);
        sliderDirectionY.onValueChanged.AddListener(OnSliderDirectionYValueChanged);
    }

    private void OnToggleValueChanged(bool value)
    {
        RandomWaves = value;

        if (!RandomWaves)
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
        WaveStrength = value;
    }

    private void OnSliderDirectionXValueChanged(float value)
    {
        WaveDirection.x = value;
    }

    private void OnSliderDirectionYValueChanged(float value)
    {
        WaveDirection.y = value;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.AddForceAtPosition(Physics.gravity, transform.position, ForceMode.Acceleration);
        if (transform.position.y < bouyancyDepth)
        {
            float displacement = Mathf.Clamp((bouyancyDepth - transform.position.y),0,10) * bouyancyForce;
            rb.AddForce(new Vector3(0, Mathf.Abs(Physics.gravity.y)*displacement/rb.mass), ForceMode.Acceleration);

            rb.AddForce(-rb.velocity * Time.fixedDeltaTime * displacement * waterDrag, ForceMode.VelocityChange);
        }
        if (RandomWaves)
            WaveDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        rb.AddForce(WaveDirection.AsXZ() * WaveStrength, ForceMode.Acceleration);
        if (transform.position.x < boundingrect.min.x)
            rb.AddForce(new Vector3((boundingrect.min.x - transform.position.x) * WaveStrength, 0,0), ForceMode.Acceleration);
        if (transform.position.x > boundingrect.max.x)
            rb.AddForce(new Vector3(-(transform.position.x-boundingrect.max.x) * WaveStrength, 0, 0), ForceMode.Acceleration);
        if (transform.position.z < boundingrect.min.y)
            rb.AddForce(new Vector3(0,0,(boundingrect.min.y - transform.position.z) * WaveStrength), ForceMode.Acceleration);
        if (transform.position.z > boundingrect.max.y)
            rb.AddForce(new Vector3(0,0,-(transform.position.z - boundingrect.max.y) * WaveStrength), ForceMode.Acceleration);

    }
}
