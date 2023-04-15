using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����� �Ʒ��������� ��ũ�Ѹ��ϴ� Ŭ����
/// </summary>
public class Background : MonoBehaviour
{
    List<Transform> backgroundList;

    IEnumerator scrollCoroutine;
    WaitForSeconds waitForScroll;

    int maxPosY;
    float moveSpeed;

    void Awake()
    {
        Init();
        StartCoroutine(scrollCoroutine);
    }

    /// <summary>
    /// ���������� �ʱ�ȭ�ϴ� �Լ�
    /// </summary>
    void Init()
    {
        backgroundList = new List<Transform>();
        scrollCoroutine = Scroll();
        waitForScroll = new WaitForSeconds(0.016f);
        maxPosY = 16;
        moveSpeed = 0.05f;

        for (int i = 0; i < transform.childCount; i++)
        {
            backgroundList.Add(transform.GetChild(i));
        }
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

            for (int i = 0; i < backgroundList.Count; i++)
            {
                backgroundList[i].position += Vector3.down * moveSpeed;
                if (backgroundList[i].position.y < -maxPosY)
                {
                    backgroundList[i].position += Vector3.up * maxPosY * 2;
                }
            }
        }
    }
}
