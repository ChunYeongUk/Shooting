using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 배경을 아래방향으로 스크롤링하는 클래스
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
    /// 전역변수를 초기화하는 함수
    /// </summary>
    void Init()
    {
        waitForScroll = new WaitForSeconds(0.016f);
        maxPosY = 8;
        moveSpeed = 0.05f;
    }

    /// <summary>
    /// 정해진 시간마다 이동하는 함수
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
