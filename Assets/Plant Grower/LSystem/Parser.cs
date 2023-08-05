using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ParametrizedTreeNode
{
    public char Symbol;
    public float? TwigAngleVariation;
    public float? TwigLenghtVariation;
    public float? BranchAngleVariation;

    public static ParametrizedTreeNode Create(char symbol, PlantDNA dna)
    {
        float? twigAngleVariation = null;
        float? twigLenghtVariation = null;
        float? branchAngleVariation = null;

        if(symbol == 'F')
        {
            twigLenghtVariation = Random.Range(dna.AvarageTwigLenght * dna.TwigLenghtMinMax.x, dna.AvarageTwigLenght * dna.TwigLenghtMinMax.y);
            twigAngleVariation = Random.Range(-dna.TwigAngleMinMax.x, dna.TwigAngleMinMax.y);
        }
        else if ("+".Contains(symbol))
        {
            branchAngleVariation = dna.AvarageBranchAngle + Random.Range(dna.BranchAngleMinMax.x, dna.BranchAngleMinMax.y);
        }
        else if ("-".Contains(symbol))
        {
            branchAngleVariation = -(dna.AvarageBranchAngle + Random.Range(dna.BranchAngleMinMax.x, dna.BranchAngleMinMax.y));
        }

        return new ParametrizedTreeNode()
        {
            Symbol = symbol,
            TwigAngleVariation = twigAngleVariation,
            TwigLenghtVariation = twigLenghtVariation,
            BranchAngleVariation = branchAngleVariation,
        };
    }
}

public class Parser
{
    PlantDNA PlantDNA;
    public string CurrentWord;

    public Parser(PlantDNA plantDNA)
    {
        PlantDNA = plantDNA;
        CurrentWord = plantDNA.Axiom;
    }

    public string Iterate()
    {
        var newWord = new StringBuilder();
        var sentence = CurrentWord.ToCharArray();

        for (int i = 0; i < sentence.Length; i++)
        {
            var matchingRules = PlantDNA.Rules.Where(rule => sentence[i] == rule.Symbol);
            if (matchingRules.Any())
            {
                var selectedRule = RoulleteRuleSelector(matchingRules.ToList());
                newWord.Append(selectedRule.Next);
            }
            else
                newWord.Append(sentence[i]);
        }

        CurrentWord = newWord.ToString();
        return CurrentWord;
    }

    public List<ParametrizedTreeNode> Parse()
    {
        return CurrentWord.ToCharArray().Select(c => ParametrizedTreeNode.Create(c, PlantDNA)).ToList();
    }

    private Rule RoulleteRuleSelector(List<Rule> matchingRules)
    {
        var selectedRule = PlantDNA.Rules.First();
        var roll = Random.Range(0f, 1f);
        var sum = 0f;
        foreach (var rule in matchingRules)
        {
            sum += rule.Odds;
            if (roll < sum)
            {
                selectedRule = rule;
                break;
            }
        }

        return selectedRule;
    }
}
