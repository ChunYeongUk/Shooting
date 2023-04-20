using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ObjectPoolData")]
public class ObjectPoolData : ScriptableObject
{
    public int mainType;
    public int subType;

    public GameObject prefab;

    public int initialCount;
    public int maxCount;
}
