using UnityEngine;

public class Growth : MonoBehaviour
{
    public float growth = .0f;
    public float GrowthStage = 0;
    Material mat;
    string shaderProp = "_TintLevel";
    public Sprite SeedStage01;
    public Sprite SeedStage02;


    void Start()
    {
        mat = gameObject.GetComponent<Renderer>().material;
        mat.SetFloat(shaderProp, growth);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetGrouwth(float value)
    {
        if(value >= .95)
        {
            GrowthStage++;
            growth = 0f;
            gameObject.GetComponent<SpriteRenderer>().sprite = GetSpriteGrowthStage();
        }
        else
            growth = value;

        mat.SetFloat(shaderProp, growth);

    }

    Sprite GetSpriteGrowthStage()
    {
        if (GrowthStage == 1)
            return SeedStage01;
        else if (GrowthStage == 2)
            return SeedStage02;

        return SeedStage01;
    }
}
