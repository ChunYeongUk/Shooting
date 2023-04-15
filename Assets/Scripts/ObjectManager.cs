using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// ������ƮǮ Ŭ����
/// </summary>
public class ObjectManager : MonoBehaviour
{
    IObjectPool<ObjectType>[] objectPoolArray = new IObjectPool<ObjectType>[(int)ObjectTypeEnum.TypeCount];

    int[] defaultCapacity = new int[(int)ObjectTypeEnum.TypeCount] { 1, 20, 100, 20, 20, 4, };
    int[] maxSize = new int[(int)ObjectTypeEnum.TypeCount] { 1, 100, 500, 100, 100, 4, };
    string[] objectNames = new string[(int)ObjectTypeEnum.TypeCount];
    Transform[] objectParents = new Transform[(int)ObjectTypeEnum.TypeCount];

    ObjectType originalType;

    /// <summary>
    /// ������ƮǮ�� ���� Object, Parent, Name���� ĳ���ϴ� �Լ�
    /// </summary>
    public void Init()
    {
        GameObject newGameObject = new GameObject("TempObj");

        for (int i = 0; i < (int)ObjectTypeEnum.TypeCount; i++)
        {
            objectPoolArray[i] = new ObjectPool<ObjectType>(CreatePool, OnTakeFromPool, OnReturnedToPool, OnDestroyPool, false, defaultCapacity[i], maxSize[i]);
            objectNames[i] = Enum.ToObject(typeof(ObjectTypeEnum), i).ToString();
            objectParents[i] = Instantiate<Transform>(newGameObject.transform, this.transform);
            objectParents[i].name = objectNames[i];
        }

        originalType = newGameObject.AddComponent<ObjectType>();
        originalType.mainType = (int)ObjectTypeEnum.TypeCount;
        originalType.gameObject.AddComponent<SpriteRenderer>();
    }

    #region PrivateFunction
    /// <summary>
    /// ������Ʈ�� �����ɶ� ȣ��Ǵ� �⺻ �Լ�
    /// </summary>
    /// <returns></returns>
    ObjectType CreatePool()
    {
        return Instantiate<ObjectType>(originalType);
    }

    /// <summary>
    /// ������Ʈ�� ���� �� ȣ��Ǵ� �⺻ �Լ�
    /// </summary>
    /// <param name="p_ObjectType"></param>
    void OnTakeFromPool(ObjectType p_ObjectType)
    {
        p_ObjectType.gameObject.SetActive(true);
    }

    /// <summary>
    /// ������Ʈ�� ���� �� ȣ��Ǵ� �⺻ �Լ�
    /// </summary>
    /// <param name="p_ObjectType"></param>
    void OnReturnedToPool(ObjectType p_ObjectType)
    {
        p_ObjectType.gameObject.SetActive(false);
    }

    /// <summary>
    /// ������Ʈ�� �ı��� �� ȣ��Ǵ� �⺻ �Լ�
    /// </summary>
    /// <param name="p_ObjectType"></param>
    void OnDestroyPool(ObjectType p_ObjectType)
    {
        Destroy(p_ObjectType);
    }
    #endregion

    #region PublicFunction
    /// <summary>
    /// GameManager��ũ��Ʈ���� ������Ʈ ��û�ϴ� �Լ�
    /// </summary>
    /// <param name="p_MainType"></param>
    /// <returns></returns>
    public ObjectType GetObjectType(int p_MainType)
    {
        ObjectType newObjectType = objectPoolArray[p_MainType].Get();
        newObjectType.name = objectNames[p_MainType];
        newObjectType.transform.SetParent(objectParents[p_MainType]);
        newObjectType.GetComponent<SpriteRenderer>().sortingOrder = p_MainType;

        return newObjectType;
    }

    /// <summary>
    /// GameManager��ũ��Ʈ���� ������Ʈ ��ȯ�ϴ� �Լ�
    /// </summary>
    /// <param name="p_ObjectType"></param>
    public void SetObjectType(ObjectType p_ObjectType)
    {
        int mainType = p_ObjectType.mainType;
        objectPoolArray[mainType].Release(p_ObjectType);
    }

    /// <summary>
    /// GameManager��ũ��Ʈ���� GameOver�� �˷��ָ� ��� ������Ʈ�� ��Ȱ��ȭ�ϴ� �Լ�
    /// </summary>
    public void GameOver()
    {
        for (int i = 0; i < (int)ObjectTypeEnum.TypeCount; i++)
        {
            objectPoolArray[i].Clear();
        }
    }
    #endregion
}
