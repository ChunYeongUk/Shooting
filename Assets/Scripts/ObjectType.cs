using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tag��� ����ϱ� ���� Enum
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
/// Tag��� ����ϱ� ���� Ŭ����
/// </summary>
public class ObjectType : MonoBehaviour
{
    public int mainType;
    public int subType;
}
