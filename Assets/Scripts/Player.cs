using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 이동, 공격, 피격에 대한 클래스
/// </summary>
public class Player : MonoBehaviour
{
    int maxHp;
    int curHp;
    int maxMp;
    int curMp;
    int maxPower;
    int curPower;

    float moveSpeed;

    bool isHit;
    bool isRepawn;

    Vector2 curPos;
    Vector2 moveDir;

    IEnumerator moveCoroutine;

    SpriteRenderer thisRenderer;

    GameManager gameManager;

    WaitForSeconds waitForFrame;
    WaitForSeconds waitForRespawn;
    WaitForSeconds waitForAttack;

    /// <summary>
    /// 전역변수를 초기화하는 함수
    /// </summary>
    /// <param name="p_GameManager"></param>
    public void Init(GameManager p_GameManager)
    {
        gameManager = p_GameManager;
        thisRenderer = GetComponent<SpriteRenderer>();
        curPos = new Vector2(0, -6f);
        transform.position = curPos;

        maxHp = 3;
        curHp = maxHp;
        maxMp = 3;
        curMp = maxMp;
        maxPower = 10;
        curPower = 1;
        moveSpeed = 5;

        waitForFrame = new WaitForSeconds(0.016f);
        waitForRespawn = new WaitForSeconds(2.0f);
        waitForAttack = new WaitForSeconds(0.1f);

        StartCoroutine(Attack());
    }

    #region Move
    /// <summary>
    /// 터치값에 속도를 더하고, 
    /// 범위를 확인 한 후,
    /// 캐릭터를 이동시키는 함수 
    /// </summary>
    IEnumerator Move()
    {
        while(true)
        {
            yield return waitForFrame;

            if (!isHit)
            {
                curPos += moveDir.normalized * moveSpeed * Time.deltaTime;

                if (curPos.x < -4f)
                {
                    curPos.x = -4f;
                }
                else if (curPos.x > 4f)
                {
                    curPos.x = 4f;
                }

                if (curPos.y < -7.5f)
                {
                    curPos.y = -7.5f;
                }
                else if (curPos.y > 7.5f)
                {
                    curPos.y = 7.5f;
                }

                transform.position = curPos;
            }
        }
    }

    /// <summary>
    /// 이동 방향을 받아오는 함수
    /// </summary>
    /// <param name="p_MoveDir"></param>
    public void ChangeMoveDir(Vector2 p_MoveDir)
    {
        moveDir = p_MoveDir;
    }

    /// <summary>
    /// 터치시 이동을 시작하는 함수
    /// </summary>
    public void TouchStart()
    {
        moveCoroutine = Move();
        StartCoroutine(moveCoroutine);
    }

    /// <summary>
    /// 터치 해제시 이동을 멈추는 함수
    /// </summary>
    public void TouchEnd()
    {
        StopCoroutine(moveCoroutine);
    }
    #endregion

    #region Attack
    /// <summary>
    /// 일정 시간마다 공격을 하는 함수
    /// </summary>
    IEnumerator Attack()
    {
        while(true)
        {
            yield return waitForAttack;
            if(!isHit && !isRepawn)
            {
                StraightShot(curPower);
            }
        }
    }

    /// <summary>
    /// 직선으로 공격하는 함수
    /// </summary>
    /// <param name="p_AmmoNum">curPower</param>
    void StraightShot(int p_AmmoNum)
    {
        float distance = 0;
        if ((p_AmmoNum % 2).Equals(0))
        {
            distance = p_AmmoNum / 2 - 0.5f;
        }
        else
        {
            distance = (p_AmmoNum - 1) / 2;
        }

        for (int i = 0; i < p_AmmoNum; i++)
        {
            Vector3 posVec = transform.position + (distance - i) * 0.3f * Vector3.right;

            gameManager.GetBullet(0, posVec, Quaternion.identity, Vector2.up * 10);
        }
    }
    #endregion

    #region Hit
    /// <summary>
    /// 충돌을 감지하는 함수
    /// </summary>
    /// <param name="collision"></param>
    void OnTriggerEnter2D(Collider2D collision)
    {
        ObjectType newObjectType = collision.GetComponent<ObjectType>();

        switch ((ObjectTypeEnum)newObjectType.mainType)
        {
            case ObjectTypeEnum.Enemy:
                if (!isHit && !isRepawn)
                {
                    gameManager.SetObject(newObjectType);
                    OnHit();
                }
                break;
            case ObjectTypeEnum.Bullet:
                if (!isHit && !isRepawn && newObjectType.subType.Equals(1))
                {
                    gameManager.SetObject(newObjectType);
                    OnHit();
                }
                break;
            case ObjectTypeEnum.Item:
                if (!isHit)
                {
                    gameManager.SetObject(newObjectType);
                    GetItem(newObjectType.subType);
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 피격 이벤트 함수
    /// </summary>
    void OnHit()
    {
        gameManager.GetEffect(0, curPos);
        isHit = true;
        thisRenderer.color = new Color(1, 1, 1, 0);
        transform.position = curPos;
        curHp--;
        gameManager.HpMpUpdate(true, false, curHp);

        if (curHp.Equals(0))
        {
            gameManager.GameOver();
            return;
        }

        StartCoroutine(Respawn());
    }

    /// <summary>
    /// 피격 당한 후 부활하는 함수
    /// </summary>
    /// <returns></returns>
    IEnumerator Respawn()
    {
        yield return waitForRespawn;

        isHit = false;
        isRepawn = true;
        curPos = new Vector2(0, -6f);
        thisRenderer.color = new Color(1, 1, 1, 0.5f);

        yield return waitForRespawn;

        thisRenderer.color = new Color(1, 1, 1, 1);
        isRepawn = false;
    }

    /// <summary>
    /// 아이템 획득 함수
    /// </summary>
    /// <param name="p_ItemType"></param>
    void GetItem(int p_ItemType)
    {
        switch (p_ItemType)
        {
            case 0:
                gameManager.GetScore(1000);
                break;
            case 1:
                if (curHp.Equals(maxHp))
                {
                    gameManager.GetScore(500);
                }
                else
                {
                    gameManager.HpMpUpdate(true, true, curHp);
                    curHp++;
                }
                break;
            case 2:
                if (curMp.Equals(maxMp))
                {
                    gameManager.GetScore(500);
                }
                else
                {
                    gameManager.HpMpUpdate(false, true, curMp);
                    curMp++;
                }
                break;
            case 3:
                if (curPower.Equals(maxPower))
                {
                    gameManager.GetScore(500);
                }
                else
                {
                    curPower++;
                }
                break;
        }
    }
    #endregion
}
