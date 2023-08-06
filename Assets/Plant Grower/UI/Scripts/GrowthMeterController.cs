using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowthMeterController : MonoBehaviour
{
    string GrowSimualtorTag = "GrowSimulator";
    Grower GrowSimulator;

    Material GrowthMeterFillMaterial;
    void Start()
    {
        GrowSimulator = GameObject.FindGameObjectWithTag(GrowSimualtorTag).GetComponent<Grower>();
        if (GrowSimulator == null)
        {
            Debug.Log("Missing grow simualtor!");
            return;
        }

        GrowthMeterFillMaterial = GetComponentInChildren<GrowthMeterFillComponentTag>()?.gameObject.transform.GetComponent<Renderer>().material;
        if (GrowthMeterFillMaterial == null)
        {
            Debug.Log("Missing growth meter fill for UI display!");
            return;
        }

        GrowSimulator.OnGrowthHandler += UpdateGrowthMeterLevel;
    }

    void UpdateGrowthMeterLevel(float current, float max)
    {
        var percent = current / max;
        GrowthMeterFillMaterial.SetFloat("_TintLevel", 1f- percent);
    }
}
