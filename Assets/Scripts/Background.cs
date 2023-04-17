using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����� �Ʒ��������� ��ũ�Ѹ��ϴ� Ŭ����
/// </summary>
public class Background : MonoBehaviour
{
    WaitForSeconds waitForScroll;

    int maxPosY;
    float moveSpeed;

    void Awake()
    {
        Init();
        StartCoroutine(Scroll());
    }

    /// <summary>
    /// ���������� �ʱ�ȭ�ϴ� �Լ�
    /// </summary>
    void Init()
    {
        waitForScroll = new WaitForSeconds(0.016f);
        maxPosY = 8;
        moveSpeed = 0.05f;
    }

    /// <summary>
    /// ������ �ð����� �̵��ϴ� �Լ�
    /// </summary>
    /// <returns></returns>
    IEnumerator Scroll()
    {
        while(true)
        {
            yield return waitForScroll;

            transform.position += Vector3.down * moveSpeed;

            if(transform.position.y < -maxPosY)
            {
                transform.position += Vector3.up * maxPosY * 2;
            }
        }
    }
}
