using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class SaveFile
{
    public List<AISave> parents = new List<AISave>();
}


[System.Serializable]

public struct AISave
{
    public float[][,] weights { get; set; }
    public float[][] constants { get; set; }
}

