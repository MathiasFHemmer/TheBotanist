using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterLevelController : MonoBehaviour
{

    string GrowSimualtorTag = "GrowSimulator";
    Grower GrowSimulator;

    Transform WaterMeterFillTransform;

    Wobble WaterMeterWobble;
    void Start()
    {
        GrowSimulator = GameObject.FindGameObjectWithTag(GrowSimualtorTag).GetComponent<Grower>();
        if(GrowSimulator == null)
        {
            Debug.Log("Missing grow simualtor!");
            return;
        }

        WaterMeterFillTransform = GetComponentInChildren<WaterMeterFillComponentTag>()?.gameObject.transform;
        if (WaterMeterFillTransform == null)
        {
            Debug.Log("Missing water meter fill for UI display!");
            return;
        }

        WaterMeterWobble = GetComponent<Wobble>();

        GrowSimulator.OnWaterUsedHandler += UpdateWaterMeterLevel;
        GrowSimulator.OnWaterAddedHandler += UpdateWaterMeterLevel;
        GrowSimulator.OnWaterAddedHandler += DisableWobble;
        GrowSimulator.OnWaterDepletedHandler += EnableWobble;
    }

    void DisableWobble(float ammount, float max)
    {
        GrowSimulator.OnWaterUsedHandler += UpdateWaterMeterLevel;
        WaterMeterWobble.enabled = false;
    }

    void EnableWobble()
    {
        GrowSimulator.OnWaterUsedHandler -= UpdateWaterMeterLevel;
        WaterMeterWobble.enabled = true;
    }
    void UpdateWaterMeterLevel(float current, float max)
    {
        var newScale = new Vector3(1, current / max, 1);
        WaterMeterFillTransform.localScale = newScale;
    }
}
