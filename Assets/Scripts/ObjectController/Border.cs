using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 화면밖으로 나간 오브젝트를 회수하는 클래스
/// </summary>
public class Border : MonoBehaviour
{
    [SerializeField] GameManager gameManager;

    /// <summary>
    /// 충돌하는 모든 오브젝트를 회수하는 함수
    /// </summary>
    /// <param name="collision"></param>
    void OnTriggerEnter2D(Collider2D collision)
    {
        ObjectType newObjectType = collision.GetComponent<ObjectType>();
        gameManager.SetObject(newObjectType);
    }
}
