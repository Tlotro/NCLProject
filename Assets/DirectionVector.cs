using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DirectionVector : MonoBehaviour
{
    [SerializeField] private Slider sliderDirectionX;
    [SerializeField] private Slider sliderDirectionY;

    private Vector3 direction = new Vector3(0f, 0f);

    // private const float length = 100f;

    [SerializeField] private Transform parentTransform;

    private void Start()
    {
        sliderDirectionX.onValueChanged.AddListener(OnSliderDirectionXValueChanged);
        sliderDirectionY.onValueChanged.AddListener(OnSliderDirectionYValueChanged);
    }

    private void OnSliderDirectionXValueChanged(float value)
    {
        direction.x = value;
    }

    private void OnSliderDirectionYValueChanged(float value)
    {
        direction.y = value;
    }

    private void Update()
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        parentTransform.localEulerAngles = new Vector3(0f, 0f, angle + 180);
    }

}
