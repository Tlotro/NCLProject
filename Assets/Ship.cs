using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.EventSystems;

public class Ship : MonoBehaviour
{
    public Crane crane;
    public Rigidbody rb;
    public float bouyancyDepth;
    public float bouyancyForce;
    public float waterDrag;

    public InfluenceMode WaveModeStrength;
    public float WavePeriod;
    public Vector2 WaveStrengthClamp;
    public float WaveStrength;

    public InfluenceMode CurrentModeDirectionX;
    public float CurrentPeriodX;
    public InfluenceMode CurrentModeDirectionY;
    public float CurrentPeriodY;
    public InfluenceMode CurrentModeStrength;
    public float CurrentPeriodStrength;
    public Vector2 CurrentDirection;
    public Vector2 CurrentStrengthClamp;
    public float CurrentStrength;
    public Rect boundingrect;

    private float WaveTimerStrength;
    private float CurrentTimerX;
    private float CurrentTimerY;
    private float CurrentTimerStrength;

    public void SetWaveMode(int mode) { WaveModeStrength = (InfluenceMode)mode; }
    public void SetWaveStrength(float mode) { if (WaveModeStrength == InfluenceMode.manual) WaveStrength = mode; }
    public void SetCurrentMode(int mode) { CurrentModeStrength = (InfluenceMode)mode; }
    public void SetCurrentStrength(float mode) { if (CurrentModeStrength == InfluenceMode.manual) CurrentStrength = mode; }
    public void SetCurrentModeX(int mode) { CurrentModeDirectionX = (InfluenceMode)mode; }
    public void SetCurrentX(float mode) { if (CurrentModeDirectionX == InfluenceMode.manual) CurrentDirection.x = mode; }
    public void SetCurrentModeY(int mode) { CurrentModeDirectionY = (InfluenceMode)mode; }
    public void SetCurrentY(float mode) { if (CurrentModeDirectionY == InfluenceMode.manual) CurrentDirection.y = mode; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        WaveTimerStrength += (Time.fixedDeltaTime / WavePeriod) % (Mathf.PI * 2);
        CurrentTimerX += (Time.fixedDeltaTime / CurrentPeriodX) % (Mathf.PI * 2);
        CurrentTimerY += (Time.fixedDeltaTime / CurrentPeriodY) % (Mathf.PI * 2);
        CurrentTimerStrength += (Time.fixedDeltaTime / CurrentPeriodStrength) % (Mathf.PI * 2);
        switch (WaveModeStrength)
        {
            case InfluenceMode.random:
                WaveStrength = Random.Range(WaveStrengthClamp.x, WaveStrengthClamp.y);
                break;
            case InfluenceMode.sine:
                WaveStrength = (Mathf.Sin(WaveTimerStrength) + 1) / 2 * (WaveStrengthClamp.y - WaveStrengthClamp.x) + WaveStrengthClamp.x;
                break;
        }
        switch (CurrentModeDirectionX)
        {
            case InfluenceMode.random:
                CurrentDirection.x = Random.Range(-1f, 1f);
                break;
            case InfluenceMode.sine:
                CurrentDirection.x = Mathf.Sin(CurrentTimerX);
                break;
        }
        switch (CurrentModeDirectionY)
        {
            case InfluenceMode.random:
                CurrentDirection.y = Random.Range(-1f, 1f);
                break;
            case InfluenceMode.sine:
                CurrentDirection.y = Mathf.Sin(CurrentTimerY);
                break;
        }
        switch (CurrentModeStrength)
        {
            case InfluenceMode.random:
                CurrentStrength = Random.Range(CurrentStrengthClamp.x, CurrentStrengthClamp.y);
                break;
            case InfluenceMode.sine:
                CurrentStrength = (Mathf.Sin(CurrentTimerStrength) + 1) / 2 * (CurrentStrengthClamp.y - CurrentStrengthClamp.x) + CurrentStrengthClamp.x;
                break;
        }

        rb.AddForceAtPosition(Physics.gravity, transform.position, ForceMode.Acceleration);
        if (transform.position.y < bouyancyDepth)
        {
            float displacement = Mathf.Clamp((bouyancyDepth - transform.position.y),0,10) * bouyancyForce;
            rb.AddForce(new Vector3(0, (Mathf.Abs(Physics.gravity.y) + WaveStrength) *displacement/rb.mass), ForceMode.Acceleration);

            rb.AddForce(-rb.velocity * Time.fixedDeltaTime * displacement * waterDrag, ForceMode.VelocityChange);
        }
        rb.AddForce(CurrentDirection.normalized.AsXZ() * CurrentStrength, ForceMode.Acceleration);
        if (transform.position.x < boundingrect.min.x)
            rb.AddForce(new Vector3((boundingrect.min.x - transform.position.x) * WaveStrength, 0,0), ForceMode.Acceleration);
        if (transform.position.x > boundingrect.max.x)
            rb.AddForce(new Vector3(-(transform.position.x-boundingrect.max.x) * WaveStrength, 0, 0), ForceMode.Acceleration);
        if (transform.position.z < boundingrect.min.y)
            rb.AddForce(new Vector3(0,0,(boundingrect.min.y - transform.position.z) * WaveStrength), ForceMode.Acceleration);
        if (transform.position.z > boundingrect.max.y)
            rb.AddForce(new Vector3(0,0,-(transform.position.z - boundingrect.max.y) * WaveStrength), ForceMode.Acceleration);

    }

    private void OnCollisionEnter(Collision collision)
    {
        crane.Land();
    }
}
