using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tag대신 사용하기 위한 Enum
/// </summary>
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

/// <summary>
/// Tag대신 사용하기 위한 클래스
/// </summary>
public class ObjectType : MonoBehaviour
{
    public int mainType;
    public int subType;
}
