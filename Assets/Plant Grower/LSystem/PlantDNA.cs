using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlantDNA", menuName = "LSystem/PlantDNA", order = 1)]
public class PlantDNA : ScriptableObject
{
    // Alphabet:
    // F: Create Twig
    // +: Rotate Right
    // -: Rotate Left
    // [: Enter Subtree
    // ]: Exit subtree

    public string Axiom;
    public List<Rule> Rules;

    public Color TwigColor;

    public float RootTwigWidth;

    public float AvarageLifetime;
    public int MaxIterations;
    public float AvarageTwigLenght;
    public float AvarageBranchAngle;
    public float WaterGrowthUsagePerSecond;

    public float TwigBranchDepthLengthFalloff;
    public float TwigBranchDepthWidthFalloff;

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
