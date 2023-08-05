using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowthMeterController : MonoBehaviour
{
    string GrowSimualtorTag = "GrowSimulator";
    Grower GrowSimulator;

    Transform GrowthMeterFillTransform;
    void Start()
    {
        GrowSimulator = GameObject.FindGameObjectWithTag(GrowSimualtorTag).GetComponent<Grower>();
        if (GrowSimulator == null)
        {
            Debug.Log("Missing grow simualtor!");
            return;
        }

        GrowthMeterFillTransform = GetComponentInChildren<GrowthMeterFillComponentTag>()?.gameObject.transform;
        if (GrowthMeterFillTransform == null)
        {
            Debug.Log("Missing growth meter fill for UI display!");
            return;
        }

        GrowSimulator.OnGrowthHandler += UpdateGrowthMeterLevel;
    }

    void UpdateGrowthMeterLevel(float current, float max)
    {
        var newScale = new Vector3(current / max, 1, 1);
        GrowthMeterFillTransform.localScale = newScale;
    }
}
