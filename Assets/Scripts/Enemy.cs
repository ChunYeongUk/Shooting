using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    GameManager gameManager;
    Player player;

    SpriteRenderer thisRenderer;
    Rigidbody2D thisRigidbody;
    ObjectType thisType;

    float moveSpeed;

    int hitPoint;
    int thisScore;
    int curAtkDelay;
    int nextAtkDelay;
    int attackType;

    bool isHit;

    WaitForSeconds waitForBossMove = new WaitForSeconds(2.0f);
    WaitForSeconds waitForAttack = new WaitForSeconds(0.5f);

    #region Initialization
    public void Init(GameManager p_GameManager, Player p_Player)
    {
        gameManager = p_GameManager;
        player = p_Player;
        thisRenderer = GetComponent<SpriteRenderer>();
        thisRigidbody = GetComponent<Rigidbody2D>();
        thisType = GetComponent<ObjectType>();
    }

    public void ReInit()
    {
        switch (thisType.subType)
        {
            case 0:
                hitPoint = 1;
                thisScore = 10;
                nextAtkDelay = 5;
                attackType = Random.Range(0, 2);
                moveSpeed = -3;
                break;
            case 1:
                hitPoint = 10;
                thisScore = 100;
                nextAtkDelay = 3;
                attackType = 0;
                moveSpeed = -3;
                StartCoroutine(BossMove());
                break;
            default:
                break;
        }

        curAtkDelay = 0;
        isHit = false;

        thisRigidbody.velocity = new Vector2(0, moveSpeed);
        StartCoroutine(Attack());
    }
    #endregion

    #region Move & Attack
    IEnumerator BossMove()
    {
        yield return waitForBossMove;
        thisRigidbody.velocity = Vector2.zero;
    }

    IEnumerator Attack()
    {
        while(true)
        {
            yield return waitForAttack;
            curAtkDelay++;

            if (curAtkDelay.Equals(nextAtkDelay))
            {
                if (thisType.subType.Equals(1))
                {
                    attackType = Random.Range(0, 5);
                }

                switch (attackType)
                {
                    case 0:
                        StraightShot(1);
                        break;
                    case 1:
                        GuidedShot(2);
                        break;
                    case 2:
                        ArchShot(20);
                        break;
                    case 3:
                        AroundShot(10);
                        break;
                    default:
                        break;
                }
                curAtkDelay = 0;
            }
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

            gameManager.GetBullet(1, posVec, Quaternion.identity, Vector2.down * 8);
        }
    }

    void GuidedShot(int p_AmmoNum)
    {
        for (int i = 0; i < p_AmmoNum; i++)
        {
            Vector2 dirVec = player.transform.position - transform.position;
            Vector2 ranVec = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(0f, 2f)) * p_AmmoNum;
            dirVec += ranVec;

            gameManager.GetBullet(1, transform.position, Quaternion.identity, dirVec.normalized * 5);
        }
    }

    void ArchShot(int p_AmmoNum)
    {
        for(int i = 0; i < p_AmmoNum; i++)
        {
            Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 10 * i * 0.01f), -1);

            gameManager.GetBullet(1, transform.position, Quaternion.identity, dirVec.normalized * 5);
        }
    }

    void AroundShot(int p_AmmoNum)
    {
        for (int i = 0; i < p_AmmoNum; i++)
        {
            Vector3 rotVec = Vector3.forward * 360 * i * 0.02f + Vector3.forward * 90;
            Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 2 * i * 0.02f), Mathf.Sin(Mathf.PI * 2 * i * 0.02f));

            gameManager.GetBullet(1, transform.position, Quaternion.Euler(rotVec), dirVec.normalized * 2);
        }
    }
    #endregion

    #region Hit
    void OnTriggerEnter2D(Collider2D collision)
    {
        ObjectType newObjectType = collision.GetComponent<ObjectType>();

        if (newObjectType.mainType.Equals((int)ObjectTypeEnum.Bullet) && newObjectType.subType.Equals(0))
        {
            gameManager.SetObject(newObjectType);
            OnHit(1);
        }
    }

    void OnHit(int p_Damage)
    {
        if (!isHit)
        {
            isHit = true;

            gameManager.GetEffect(0, transform.position);

            hitPoint -= p_Damage;

            if (hitPoint <= 0)
            {
                gameManager.GetScore(thisScore);
                gameManager.SetObject(thisType);

                if (thisType.subType.Equals(0))
                {
                    gameManager.GetItem(transform.position);
                }
                else
                {
                    gameManager.StageEnd();
                }
            }
            isHit = false;
        }
    }
    #endregion
}
