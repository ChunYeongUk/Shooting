using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// 오브젝트풀 클래스
/// </summary>
public class ObjectManager : MonoBehaviour
{
    IObjectPool<ObjectType>[] objectPoolArray;

    int[] defaultCapacity;
    int[] maxSize;
    string[] objectNames;
    Transform[] objectParents;

    ObjectType originalType;

    /// <summary>
    /// 오브젝트풀에 사용될 Object, Parent, Name등을 캐싱하는 함수
    /// </summary>
    public void Init()
    {
        objectPoolArray = new IObjectPool<ObjectType>[(int)ObjectTypeEnum.TypeCount];
        defaultCapacity = new int[(int)ObjectTypeEnum.TypeCount] { 1, 20, 100, 20, 20, 4, };
        maxSize = new int[(int)ObjectTypeEnum.TypeCount] { 1, 100, 500, 100, 100, 4, };
        objectNames = new string[(int)ObjectTypeEnum.TypeCount];
        objectParents = new Transform[(int)ObjectTypeEnum.TypeCount];

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
    /// 오브젝트가 생성될때 호출되는 기본 함수
    /// </summary>
    /// <returns></returns>
    ObjectType CreatePool()
    {
        return Instantiate<ObjectType>(originalType);
    }

    /// <summary>
    /// 오브젝트를 꺼낼 때 호출되는 기본 함수
    /// </summary>
    /// <param name="p_ObjectType"></param>
    void OnTakeFromPool(ObjectType p_ObjectType)
    {
        p_ObjectType.gameObject.SetActive(true);
    }

    /// <summary>
    /// 오브젝트를 넣을 때 호출되는 기본 함수
    /// </summary>
    /// <param name="p_ObjectType"></param>
    void OnReturnedToPool(ObjectType p_ObjectType)
    {
        p_ObjectType.gameObject.SetActive(false);
    }

    /// <summary>
    /// 오브젝트를 파괴할 때 호출되는 기본 함수
    /// </summary>
    /// <param name="p_ObjectType"></param>
    void OnDestroyPool(ObjectType p_ObjectType)
    {
        Destroy(p_ObjectType);
    }
    #endregion

    #region PublicFunction
    /// <summary>
    /// GameManager스크립트에서 오브젝트 요청하는 함수
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
    /// GameManager스크립트에서 오브젝트 반환하는 함수
    /// </summary>
    /// <param name="p_ObjectType"></param>
    public void SetObjectType(ObjectType p_ObjectType)
    {
        int mainType = p_ObjectType.mainType;
        objectPoolArray[mainType].Release(p_ObjectType);
    }

    /// <summary>
    /// GameManager스크립트에서 GameOver을 알려주면 모든 오브젝트를 비활성화하는 함수
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
