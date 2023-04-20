using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ObjectType : MonoBehaviour
{
    public int mainType;
    public int subType;

    /// <summary>
    /// 오브젝트 생성에 사용되는 함수
    /// </summary>
    /// <returns></returns>
    public ObjectType Clone()
    {
        GameObject newGameObject = Instantiate(gameObject);
        newGameObject.SetActive(false);

        if (!newGameObject.TryGetComponent(out ObjectType objectType))
        {
            objectType = newGameObject.AddComponent<ObjectType>();
        }

        return objectType;
    }
}
