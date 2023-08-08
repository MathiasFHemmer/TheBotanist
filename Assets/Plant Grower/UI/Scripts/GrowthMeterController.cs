using DG.Tweening;
using UnityEngine;

public class GrowthMeterController : MonoBehaviour
{
    string GrowSimualtorTag = "GrowSimulator";
    Grower GrowSimulator;

    Material GrowthMeterFillMaterial;
    void Start()
    {
        var temp = transform.DOShakePosition(.2f, randomnessMode: ShakeRandomnessMode.Harmonic).SetLoops(-1);

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
        GrowthMeterFillMaterial.SetFloat("_TintLevel", percent);
    }
}
