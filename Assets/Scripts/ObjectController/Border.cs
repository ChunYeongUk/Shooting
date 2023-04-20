using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ȭ������� ���� ������Ʈ�� ȸ���ϴ� Ŭ����
/// </summary>
public class Border : MonoBehaviour
{
    [SerializeField] GameManager gameManager;

    /// <summary>
    /// �浹�ϴ� ��� ������Ʈ�� ȸ���ϴ� �Լ�
    /// </summary>
    /// <param name="collision"></param>
    void OnTriggerEnter2D(Collider2D collision)
    {
        ObjectType newObjectType = collision.GetComponent<ObjectType>();
        gameManager.SetObject(newObjectType);
    }
}
