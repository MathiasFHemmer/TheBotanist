using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlantDNA", menuName = "LSystem/PlantDNA", order = 1)]
public class PlantDNA : ScriptableObject
{
    /* Terminology:
     * Twig: Single section of wood
     * Brach: Section of a plant that has more then one path, where the three branches into 2 or more twigs
     */


    public string Axiom;
    public List<Rule> Rules;

    public Color TwigColor;

    public float RootTwigWidth;
    public int MaxIterations;
    public float WaterGrowthUsagePerSecond;

    // Base values for generating the plant
    public float AvarageLifetime;
    public float AvarageTwigLenght;
    public float AvarageBranchAngle;
 
    public float TwigBranchDepthLengthFalloff;
    public float TwigBranchDepthWidthFalloff;

    // Random range values for variation
    public Vector2 TwigLenghtMinMax;
    public Vector2 TwigAngleMinMax;
    public Vector2 BranchAngleMinMax;
}

[Serializable]
public class Rule
{
    public char Symbol;
    public string Next;
    public float Odds;
}
