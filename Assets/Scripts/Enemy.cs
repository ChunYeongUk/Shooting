using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적 오브젝트의 이동, 공격, 피격에 대한 클래스
/// </summary>
public class Enemy : MonoBehaviour
{
    GameManager gameManager;
    Player player;

    Rigidbody2D thisRigidbody;
    ObjectType thisType;

    float moveSpeed;

    int hitPoint;
    int thisScore;
    int curAtkDelay;
    int nextAtkDelay;
    int attackType;

    float[] archShotX = { 0.95f, 0.59f, 0.31f, 0, -0.31f, -0.59f, -0.95f };
    float[] aroundShotX = { 1, 0.92f, 0.71f, 0.38f, 0, -0.38f, -0.71f, -0.92f, -1, -0.92f, -0.71f, -0.38f, 0, 0.38f, 0.71f, 0.92f };
    float[] aroundShotY = { 0, 0.38f, 0.71f, 0.92f, 1, 0.92f, 0.71f, 0.38f, 0, -0.38f, -0.71f, -0.92f, -1, -0.92f, -0.71f, -0.38f };

    bool isHit;

    WaitForSeconds waitForBossMove;
    WaitForSeconds waitForAttack;

    #region Initialization
    /// <summary>
    /// 오브젝트 생성후 1회만 실행되는 초기화 함수
    /// </summary>
    /// <param name="p_GameManager"></param>
    /// <param name="p_Player"></param>
    public void Init(GameManager p_GameManager, Player p_Player)
    {
        gameManager = p_GameManager;
        player = p_Player;
        thisRigidbody = GetComponent<Rigidbody2D>();
        thisType = GetComponent<ObjectType>();

        waitForBossMove = new WaitForSeconds(2.0f);
        waitForAttack = new WaitForSeconds(0.5f);
    }

    /// <summary>
    /// 재활용되었을때 초기화해주는 함수
    /// </summary>
    public void ReInit()
    {
        isHit = true;

        switch (thisType.subType)
        {
            case 0:
                hitPoint = 1;
                thisScore = 10;
                nextAtkDelay = 5;
                attackType = Random.Range(0, 2);
                moveSpeed = -4;
                isHit = false;
                curAtkDelay = 0;
                StartCoroutine(Attack());
                break;
            case 1:
                hitPoint = 10;
                thisScore = 100;
                nextAtkDelay = 3;
                attackType = 0;
                moveSpeed = -2;
                StartCoroutine(BossMove());
                break;
            default:
                break;
        }

        thisRigidbody.velocity = new Vector2(0, moveSpeed);
    }
    #endregion

    #region Move & Attack
    /// <summary>
    /// 보스일때 맵 위쪽에 멈추도록 만드는 함수
    /// </summary>
    /// <returns></returns>
    IEnumerator BossMove()
    {
        yield return waitForBossMove;
        thisRigidbody.velocity = Vector2.zero;
        isHit = false;
        curAtkDelay = nextAtkDelay;
        StartCoroutine(Attack());
    }

    /// <summary>
    /// 자신의 타입에 따라 일정간격으로 공경패턴을 실행한다.
    /// </summary>
    /// <returns></returns>
    IEnumerator Attack()
    {
        while(true)
        {
            yield return waitForAttack;
            curAtkDelay++;

            if (curAtkDelay >= nextAtkDelay)
            {
                if (thisType.subType.Equals(1))
                {
                    attackType = 3;
                }

                switch (attackType)
                {
                    case 0:
                        StraightShot();
                        break;
                    case 1:
                        GuidedShot();
                        break;
                    case 2:
                        ArchShot();
                        break;
                    case 3:
                        AroundShot();
                        break;
                    default:
                        break;
                }
                curAtkDelay = 0;
            }
        }
    }

    /// <summary>
    /// 직선으로 발사하는 함수
    /// </summary>
    void StraightShot()
    {
        for (int i = 0; i < 2; i++)
        {
            Vector3 posVec = transform.position + new Vector3(i - 0.5f, 0, 0);

            gameManager.GetBullet(1, posVec, Quaternion.identity, Vector2.down * 8);
        }
    }

    /// <summary>
    /// 플레이어 방향으로 발사하는 함수
    /// </summary>
    void GuidedShot()
    {
        Vector2 dirVec = player.transform.position - transform.position;

        gameManager.GetBullet(1, transform.position, Quaternion.identity, dirVec.normalized * 5);
    }

    /// <summary>
    /// 아치형으로 발사하는 함수
    ///  - 발사 방향 미리 계산하여 저장
    /// </summary>
    void ArchShot()
    {
        for(int i = 0; i < archShotX.Length; i++)
        {
            Vector2 dirVec = new Vector2(archShotX[i], -1);

            gameManager.GetBullet(1, transform.position, Quaternion.identity, dirVec.normalized * 5);
        }
    }

    /// <summary>
    /// 아치형으로 발사하는 함수
    ///  - 발사 방향 미리 계산하여 저장
    /// </summary>
    void AroundShot()
    {
        for (int i = 0; i < aroundShotX.Length; i++)
        {
            Vector2 dirVec = new Vector2(aroundShotX[i], aroundShotY[i]);

            gameManager.GetBullet(1, transform.position, Quaternion.identity, dirVec.normalized * 2);
        }
    }
    #endregion

    #region Hit
    /// <summary>
    /// 충돌을 감지하는 함수
    /// <param name="collision"></param>
    void OnTriggerEnter2D(Collider2D collision)
    {
        ObjectType newObjectType = collision.GetComponent<ObjectType>();

        if (newObjectType.mainType.Equals((int)ObjectTypeEnum.Bullet) && newObjectType.subType.Equals(0))
        {
            gameManager.SetObject(newObjectType);
            OnHit(1);
        }
    }

    /// <summary>
    /// 피격 이벤트 함수
    /// </summary>
    /// <param name="p_Damage">피격 데미지</param>
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
