using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ObjectTypeEnum
{
    Border,
    Item,
    Bullet,
    Enemy,
    Effect,
    Player,
    TypeCount,
}

public class ObjectType : MonoBehaviour
{
    public int mainType;
    public int subType;
}
