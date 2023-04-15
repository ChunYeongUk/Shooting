using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 이동, 공격, 피격에 대한 클래스
/// </summary>
public class Player : MonoBehaviour
{
    int maxHp = 3;
    int curHp = 3;
    int maxMp = 3;
    int curMp = 3;
    int maxPower = 10;
    int curPower = 1;

    float moveSpeed = 5;
    float maxAttackDelay = 0.5f;
    float curAttackDelay = 0;

    bool isHit;
    bool isRepawn;

    Vector2 touchPos;
    Vector2 moveDir;
    Vector2 curPos = new Vector2(0, -6f);

    ObjectType thisType;
    SpriteRenderer thisRenderer;

    GameManager gameManager;

    WaitForSeconds waitForRespawn = new WaitForSeconds(2.0f);

    /// <summary>
    /// 전역변수를 초기화하는 함수
    /// </summary>
    /// <param name="p_GameManager"></param>
    public void Init(GameManager p_GameManager)
    {
        gameManager = p_GameManager;
        thisType = GetComponent<ObjectType>();
        thisRenderer = GetComponent<SpriteRenderer>();
        transform.position = curPos;
    }

    void Update()
    {
        TouchCheck();
        Attack();
    }

    #region Move
    /// <summary>
    /// 터치 값을 받아서 저장하는 함수
    /// </summary>
    void TouchCheck()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch(touch.phase)
            {
                case TouchPhase.Began:
                    touchPos = touch.position;
                    gameManager.JoystickMove(touchPos);
                    break;
                case TouchPhase.Moved:
                    moveDir = touch.position - touchPos;
                    Move();
                    break;
                case TouchPhase.Stationary:
                    Move();
                    break;
                case TouchPhase.Ended:
                    gameManager.JoystickMove();
                    break;
                case TouchPhase.Canceled:
                    gameManager.JoystickMove();
                    break;
            }
        }
#else
        if (Input.GetMouseButtonDown(0))
        {
            touchPos = Input.mousePosition;
            gameManager.JoystickMove(touchPos);
        }

        if (Input.GetMouseButton(0))
        {
            moveDir = (Vector2)Input.mousePosition - touchPos;
            Move();
        }

        if (Input.GetMouseButtonUp(0))
        {
            gameManager.JoystickMove();
        }
#endif
    }

    /// <summary>
    /// 터치값에 속도를 더하고, 
    /// 범위를 확인 한 후,
    /// 캐릭터를 이동시키는 함수 
    /// </summary>
    void Move()
    {
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
    #endregion

    #region Attack
    /// <summary>
    /// 일정 시간마다 공격을 하는 함수
    /// </summary>
    void Attack()
    {
        curAttackDelay += Time.deltaTime;

        if(curAttackDelay > maxAttackDelay && !isHit && !isRepawn)
        {
            StraightShot(curPower);
            curAttackDelay = 0;
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
