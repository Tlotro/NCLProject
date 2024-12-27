using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;


public enum InfluenceMode { manual, random, sine };
public static class ExpertSystem
{
    public static Dictionary<string,ParameterRange> ParamRanges = new Dictionary<string, ParameterRange>
    {
        //{ "CargoDistanceX", new ParameterRange(new Subset("Lneg",(-15,1))},
        //{ "CargoDistanceY", new ParameterRange()},
        //{ "CargoDistanceZ", new ParameterRange()},
        //{ "DescentSpeed", new ParameterRange()},
        //{ "CraneSpeedX", new ParameterRange()},
        //{ "CraneSpeedZ", new ParameterRange()}
    };
    public static Production[] productions = new Production[] 
    { 
    
    };
    public static Dictionary<string,float> Run(Dictionary<string,float> parameters)
    {
        Debug.Log("Ran");
        Dictionary<string, Dictionary<string, float>> paramTable = new Dictionary<string, Dictionary<string, float>>(); 
        foreach (KeyValuePair<string,float> kvp in parameters)
        {
            if (ParamRanges.ContainsKey(kvp.Key))
                paramTable[kvp.Key] = ParamRanges[kvp.Key].Fixation(kvp.Value);
            else
                Debug.LogError("No range for parameter" + kvp.Key);
        }

        Dictionary<string, Dictionary<string, float>> resTable = new Dictionary<string, Dictionary<string, float>>();
        foreach (Production production in productions) 
        {
            if (!resTable.ContainsKey(production.result.Item1))
                resTable[production.result.Item1] = new Dictionary<string, float>();
            float prodresnumeric = 1;
            foreach ((string,string) pair in production.messages)
            {
                prodresnumeric = math.min(paramTable[pair.Item1][pair.Item2], prodresnumeric);
            }
            if (!resTable[production.result.Item1].ContainsKey(production.result.Item2))
                resTable[production.result.Item1][production.result.Item2] = prodresnumeric;
            else
                resTable[production.result.Item1][production.result.Item2] = math.max(prodresnumeric, resTable[production.result.Item1][production.result.Item2]);
        }

        Dictionary<string,float> res = new Dictionary<string, float>();
        foreach (var result in resTable)
        {
            float num = 0, denum = 0;
            foreach (var ss in ParamRanges[result.Key].Subsets)
            {
                if (result.Value.ContainsKey(ss.Name))
                {
                    float a, b;
                    (a,b) = Subset.Defuzzy(Subset.Activate(ss.LinePoints, result.Value[ss.Name]));
                    num += a;
                    denum += b;
                }
            }
            res.Add(result.Key, num/denum);
        }
        return res;
    }
}

public struct Production
{
    /// <summary>
    /// комбинаци€ параметр+подмножество
    /// </summary>
    public (string, string)[] messages;
    public (string, string) result;
}

public struct ParameterRange
{
    /// <summary>
    /// ѕодмножества
    /// </summary>
    public Subset[] Subsets;
    /// <summary>
    /// »мена подмножеств
    /// </summary>
    public string[] Names => Subsets.Select(x=>x.Name).ToArray();
    public ParameterRange(params Subset[] subsets)
    {
        this.Subsets = subsets;
    }
    /// <summary>
    /// ‘ункци€, определ€юща€ степени принадлежности параметра к каждому из подмножеств
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public Dictionary<string,float> Fixation(float param)
    {
        return Subsets.ToDictionary(x=>x.Name,x=>x.MemDegree(param));
    }
}

/// <summary>
/// ѕодмножество, к которому может принадлежать некоторый параметр.
/// ” параметра может быть несколько подмножеств.
/// </summary>
public struct Subset
{
    public string Name;
    /// <summary>
    /// точки ломаной линии в в диапазоне значени€ параметра [0,inf) и степени принадлежности к этому подмножеству [0,1]
    /// точка определ€ет начало следующего и конец прошлого промежутков, если следующа€ точка не указана, считаетс€ что промежуток уходит в бесконечность.
    /// все промежутки - части пр€мой, обозначающей это конкретное подмножество.
    /// </summary>
    public SortedList<float, float> LinePoints;

    public Subset(string Name = "Subset", params (float,float)[] points)
    {
        this.Name = Name;
        LinePoints = new SortedList<float, float>();
        foreach (var p in points) 
        {
            LinePoints.Add(p.Item1,p.Item2);
        }
    }

    public float MemDegree(float param)
    {
        if (LinePoints.Count == 0)
        {
            Debug.LogWarning("ѕопытка получени€ степени принадлежности пустому подмножеству");
            return 0;
        }
        if (LinePoints.Keys.Last() <= param)
            return LinePoints.Values.Last();
        if (LinePoints.Keys.First() >= param)
            return LinePoints.Values.First();

        int min = 0;
        int max = LinePoints.Count - 2;
        while (LinePoints.Keys[(min+max)/2] > param || LinePoints.Keys[(min + max)/2+1] < param)
        {
            if (LinePoints.Keys[(min + max) / 2] > param)
            {
                max = (min + max) / 2-1;
            }
            if (LinePoints.Keys[(min + max) / 2 + 1] < param)
            {
                min = (min + max) / 2+1;
            }
        }
        int ind = (min + max) / 2;
        return (LinePoints.Values[ind + 1] - LinePoints.Values[ind]) / (LinePoints.Keys[ind + 1] - LinePoints.Keys[ind])*(param-LinePoints.Keys[ind]) + LinePoints.Values[ind];
    }

    public static (float,float) Defuzzy(SortedList<float, float> LinePoints)
    {
        float denumerator = 0;
        float numerator = 0;
        for (int i = 0; i < LinePoints.Count-1;i++)
        {
            //This is retarded, but it works.
            //Occam would be laughing at me for taking literal hours to come up with just breaking the line into segments, on which they are defined by linear functions.
            //And then easily finding their integrals, because they would be quadratic equasions after being multiplied by x.
            float A = (LinePoints.Values[i + 1] - LinePoints.Values[i]) / (LinePoints.Keys[i+1] - LinePoints.Keys[i]);
            float B = (LinePoints.Values[i] - LinePoints.Keys[i] * A);
            float C = LinePoints.Keys[i];
            float D = LinePoints.Keys[i + 1];
            numerator += A * D * D * D / 3 - A * C * C * C / 3 + B * D * D / 2 - B * C * C / 2;
            denumerator += A * D * D / 2 - A * C * C / 2 + B * D - B * C;
        }
        return (numerator, denumerator);
    }

    public static SortedList<float, float> Activate(SortedList<float, float> LinePoints, float lim) 
    {
        SortedList<float, float> newPoints = new SortedList<float, float>();
        for (int i = 0; i < LinePoints.Count - 1; i++)
        {
            float minval = math.min(lim, math.min(LinePoints.Values[i + 1], LinePoints.Values[i]));
            float maxval = math.max(lim, math.max(LinePoints.Values[i + 1], LinePoints.Values[i]));
            newPoints.Add(LinePoints.Keys[i], math.min(LinePoints.Values[i], lim));
            if (math.min(LinePoints.Values[i + 1], LinePoints.Values[i]) < lim && math.max(LinePoints.Values[i + 1], LinePoints.Values[i]) > lim)
            {
                newPoints.Add((LinePoints.Keys[i + 1] - LinePoints.Keys[i]) / (LinePoints.Values[i + 1] - LinePoints.Values[i]) * (lim - LinePoints.Values[i]), lim);
            }
        }
        //Clean up
        int ind = 1;
        while (ind < newPoints.Count - 1)
        {
            if (newPoints.Values[ind - 1] == newPoints.Values[ind] && newPoints.Values[ind] == newPoints.Values[ind + 1])
                newPoints.Remove(newPoints.Keys[ind]);
            else
                ind++;
        }
        newPoints.Add(LinePoints.Keys.Last(), math.min(LinePoints.Values.Last(), lim));
        return newPoints;
    }
}
