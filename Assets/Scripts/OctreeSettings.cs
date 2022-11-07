using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class OctreeSettings : ScriptableObject
{
    public uint rootNodeSize;
    public float[] detailLevels;
}
