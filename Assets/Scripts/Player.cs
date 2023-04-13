using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    void TouchCheck()
    {
#if UNITY_EDITOR
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
#elif UNITY_ANDROID
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
#endif
    }

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
    void Attack()
    {
        curAttackDelay += Time.deltaTime;

        if(curAttackDelay > maxAttackDelay && !isHit && !isRepawn)
        {
            switch(thisType.subType)
            {
                case 0:
                    StraightShot(curPower);
                    break;
                case 1:
                    ArchShot(curPower);
                    break;
            }
            curAttackDelay = 0;
        }
    }

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

    void ArchShot(int p_AmmoNum)
    {
        for (int i = 0; i < p_AmmoNum; i++)
        {
            Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 10 * i * 0.01f), -1);

            gameManager.GetBullet(0, transform.position, Quaternion.identity, dirVec.normalized * 5);
        }
    }
    #endregion

    #region Hit
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
