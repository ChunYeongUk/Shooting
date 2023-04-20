using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[DisallowMultipleComponent]
public class ObjectPoolManager : MonoBehaviour
{
    /// <summary>
    /// ������ƮǮ�����͸� ���߹迭�� �ֱ����� ����ü
    /// </summary>
    [Serializable]
    struct ObjectPool
    {
        [SerializeField] private ObjectPoolData[] datas;
        public ObjectPoolData[] _data
        {
            get
            {
                return datas;
            }
        }

        private Stack<ObjectType>[] stack;
        public Stack<ObjectType>[] _stack
        {
            get
            {
                return stack;
            }
            set
            {
                stack = value;
            }
        }
    }

    [SerializeField] private ObjectPool[] objectPools;

    /// <summary>
    /// ������ƮǮ�� �����ϴ� �Լ�
    /// </summary>
    public void Init()
    {
        int firstLength = objectPools.Length;

        for (int i = 0; i < firstLength; i++)
        {
            int secondLength = objectPools[i]._data.Length;
            objectPools[i]._stack = new Stack<ObjectType>[secondLength];
            for (int j = 0; j < secondLength; j++)
            {
                objectPools[i]._stack[j] = new Stack<ObjectType>(objectPools[i]._data[j].maxCount);
                Register(objectPools[i]._data[j], objectPools[i]._stack[j]);
            }
        }
    }

    /// <summary>
    /// ������Ʈ�� �����ϰ� ������ƮǮ�� �ִ� �Լ�
    /// </summary>
    /// <param name="p_Data"></param>
    /// <param name="p_Stack"></param>
    private void Register(ObjectPoolData p_Data, Stack<ObjectType> p_Stack)
    {
        if (p_Data.prefab.TryGetComponent(out ObjectType objectType))
        {
            for (int i = 0; i < p_Data.initialCount; i++)
            {
                ObjectType clone = objectType.Clone();
                clone.gameObject.SetActive(false);
                p_Stack.Push(clone);
            }
        }
    }

    /// <summary>
    /// ������ƮǮ���� ������Ʈ�� ������ �Լ�
    /// </summary>
    /// <param name="p_MainType"></param>
    /// <param name="p_SubType"></param>
    /// <returns></returns>
    public ObjectType Spawn(int p_MainType, int p_SubType)
    {
        Stack<ObjectType> tempPool = objectPools[p_MainType]._stack[p_SubType];
        ObjectType newObjectType = tempPool.Pop(); ;

        if(tempPool.Count.Equals(0))
        {
            ObjectType clone = newObjectType.Clone();
            tempPool.Push(clone);
        }

        newObjectType.gameObject.SetActive(true);

        return newObjectType;
    }

    /// <summary>
    /// ������Ʈ�� ������ƮǮ�� �ִ� �Լ�
    /// </summary>
    /// <param name="p_ObjectType"></param>
    public void Despawn(ObjectType p_ObjectType)
    {
        int mainType = p_ObjectType.mainType;
        int subType = p_ObjectType.subType;
        Stack<ObjectType> tempPool = objectPools[mainType]._stack[subType];

        if(tempPool.Count < objectPools[mainType]._data[subType].maxCount)
        {
            p_ObjectType.gameObject.SetActive(false);
            tempPool.Push(p_ObjectType);
        }
        else
        {
            Destroy(p_ObjectType.gameObject);
        }
    }
}
