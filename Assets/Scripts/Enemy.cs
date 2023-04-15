using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �� ������Ʈ�� �̵�, ����, �ǰݿ� ���� Ŭ����
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

    IEnumerator bossMoveCoroutine;
    IEnumerator attackCoroutine;
    WaitForSeconds waitForBossMove;
    WaitForSeconds waitForAttack;

    #region Initialization
    /// <summary>
    /// ������Ʈ ������ 1ȸ�� ����Ǵ� �ʱ�ȭ �Լ�
    /// </summary>
    /// <param name="p_GameManager"></param>
    /// <param name="p_Player"></param>
    public void Init(GameManager p_GameManager, Player p_Player)
    {
        gameManager = p_GameManager;
        player = p_Player;
        thisRigidbody = GetComponent<Rigidbody2D>();
        thisType = GetComponent<ObjectType>();

        bossMoveCoroutine = BossMove();
        attackCoroutine = Attack();
        waitForBossMove = new WaitForSeconds(2.0f);
        waitForAttack = new WaitForSeconds(0.5f);
    }

    /// <summary>
    /// ��Ȱ��Ǿ����� �ʱ�ȭ���ִ� �Լ�
    /// </summary>
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
                StartCoroutine(bossMoveCoroutine);
                break;
            default:
                break;
        }

        curAtkDelay = 0;
        isHit = false;

        thisRigidbody.velocity = new Vector2(0, moveSpeed);
        StartCoroutine(attackCoroutine);
    }
    #endregion

    #region Move & Attack
    /// <summary>
    /// �����϶� �� ���ʿ� ���ߵ��� ����� �Լ�
    /// </summary>
    /// <returns></returns>
    IEnumerator BossMove()
    {
        yield return waitForBossMove;
        thisRigidbody.velocity = Vector2.zero;
    }

    /// <summary>
    /// �ڽ��� Ÿ�Կ� ���� ������������ ���������� �����Ѵ�.
    /// </summary>
    /// <returns></returns>
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
    /// �������� �߻��ϴ� �Լ�
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
    /// �÷��̾� �������� �߻��ϴ� �Լ�
    /// </summary>
    void GuidedShot()
    {
        Vector2 dirVec = player.transform.position - transform.position;

        gameManager.GetBullet(1, transform.position, Quaternion.identity, dirVec.normalized * 5);
    }

    /// <summary>
    /// ��ġ������ �߻��ϴ� �Լ�
    ///  - �߻� ���� �̸� ����Ͽ� ����
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
    /// ��ġ������ �߻��ϴ� �Լ�
    ///  - �߻� ���� �̸� ����Ͽ� ����
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
    /// �浹�� �����ϴ� �Լ�
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
    /// �ǰ� �̺�Ʈ �Լ�
    /// </summary>
    /// <param name="p_Damage">�ǰ� ������</param>
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
                StopCoroutine(attackCoroutine);

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
