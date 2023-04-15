using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Text;
using System.IO;

/// <summary>
/// 스폰 정보를 저장하는 구조체
/// </summary>
public struct SpawnInfo
{
    public int spawnType { get; private set; }
    public int spawnPos { get; private set; }
    public int spawnDelay { get; private set; }

    public void SetData(string p_SpawnData)
    {
        string[] spawnData = p_SpawnData.Split(',');

        spawnType = int.Parse(spawnData[0]);
        spawnPos = int.Parse(spawnData[1]);
        spawnDelay = int.Parse(spawnData[2]);
    }
}

/// <summary>
/// Battle씬의 이벤트를 관리하는 클래스
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField] TMP_Text mainText;
    [SerializeField] TMP_Text scoreText;

    [SerializeField] GameObject[] HpObj;
    [SerializeField] GameObject[] MpObj;
    [SerializeField] GameObject gameOverObj;
    [SerializeField] Transform joystickTran;

    int gameScore;
    int mainTextAlpha;
    float ratioX;
    float ratioY;

    Vector2 screenHalfSize;

    StringBuilder mainTextString = new StringBuilder(3);
    WaitForSeconds waitForMainText = new WaitForSeconds(0.1f);

    [SerializeField] TextAsset[] stageArray;
    List<SpawnInfo> spawnInfoList = new List<SpawnInfo>();

    int stageLevel = 1;
    static int playerType = 0;

    float nextSpawnDelay = 100;
    float curSpawnDelay = 0;

    int spawnIndex = 0;
    bool isStageEnd;

    ObjectManager objectManager;
    Player player;

    [SerializeField] Sprite[] playerSprites;
    [SerializeField] Sprite[] enemySprites;
    [SerializeField] Sprite[] bulletSprites;
    [SerializeField] Sprite[] itemSprites;
    [SerializeField] Sprite[] effectSprites;

    IEnumerator MainTextCoroutine;
    IEnumerator StageProgressCoroutine;
    WaitForSeconds waitForEffect = new WaitForSeconds(0.05f);
    WaitForSeconds waitForStage = new WaitForSeconds(0.016f);

    void Awake()
    {
        Init();
        StartCoroutine(MainTextCoroutine);
    }

    /// <summary>
    /// 전역변수를 초기화하는 함수
    /// </summary>
    void Init()
    {
        objectManager = GetComponent<ObjectManager>();
        objectManager.Init();
        player = GetPlayer(playerType).GetComponent<Player>();

        screenHalfSize.x = Screen.width * 0.5f;
        screenHalfSize.y = Screen.height * 0.5f;

        ratioX = joystickTran.localPosition.x / screenHalfSize.x;
        ratioY = joystickTran.localPosition.y / screenHalfSize.y;

        MainTextCoroutine = MainText();
        StageProgressCoroutine = StageProgress();
    }

    #region StageManagement
    /// <summary>
    /// 스테이지를 준비하는 함수
    /// </summary>
    void StageStart()
    {
        StopCoroutine(MainTextCoroutine);
        spawnInfoList.Clear();
        spawnIndex = 0;
        isStageEnd = false;

        StringReader stringReader = new StringReader(stageArray[stageLevel - 1].text);
        string stringLine = stringReader.ReadToEnd();
        stringReader.Close();
        stringReader.Dispose();

        string[] lines = stringLine.Split('\n');

        for (int i = 0; i < lines.Length; i++)
        {
            SpawnInfo stageData = new SpawnInfo();
            stageData.SetData(lines[i]);
            spawnInfoList.Add(stageData);
        }

        curSpawnDelay = 0;
        nextSpawnDelay = spawnInfoList[0].spawnDelay;

        StartCoroutine(StageProgressCoroutine);
    }

    /// <summary>
    /// 스테이지 정보에 맞추어 적 생성을 요청하는 함수
    /// </summary>
    /// <returns></returns>
    IEnumerator StageProgress()
    {
        while (!isStageEnd)
        {
            yield return waitForStage;
            curSpawnDelay++;

            if (curSpawnDelay.Equals(nextSpawnDelay))
            {
                SpawnEnemy();
                curSpawnDelay = 0;
            }
        }
    }
    
    /// <summary>
    /// 스테이지 클리어시 호출되는 함수
    /// </summary>
    public void StageEnd()
    {
        isStageEnd = true;

        if (stageLevel.Equals(stageArray.Length))
        {
            stageLevel = 0;
        }
        else
        {
            stageLevel++;
        }
        Debug.Log("1");
        StartCoroutine(MainTextCoroutine);
    }

    /// <summary>
    /// 스테이지 시작전 Text를 켰다가 끄는 함수
    /// </summary>
    /// <returns></returns>
    IEnumerator MainText()
    {
        Debug.Log("2");
        mainTextString.Clear();
        mainTextString.Append("STAGE ");
        mainTextString.Append(stageLevel);
        mainTextString.Append("\nSTART");
        mainText.text = mainTextString.ToString();

        mainTextAlpha = 0;
        while (!mainTextAlpha.Equals(10))
        {
            yield return waitForMainText;

            mainTextAlpha += 1;
            mainText.color = new Color(1, 1, 1, mainTextAlpha * 0.1f);
        }
        while (!mainTextAlpha.Equals(0))
        {
            yield return waitForMainText;

            mainTextAlpha -= 1;
            mainText.color = new Color(1, 1, 1, mainTextAlpha * 0.1f);
        }

        StageStart();
    }

    /// <summary>
    /// 적을 오브젝트풀에서 꺼내오는 함수
    /// </summary>
    void SpawnEnemy()
    {
        ObjectType newObjectType = GetEnemy(spawnInfoList[spawnIndex].spawnType);
        newObjectType.transform.position = new Vector3(spawnInfoList[spawnIndex].spawnPos - 4, 8, 0);

        spawnIndex++;

        if (spawnIndex.Equals(spawnInfoList.Count))
        {
            spawnIndex = 1;
        }
        else
        {
            nextSpawnDelay = spawnInfoList[spawnIndex].spawnDelay;
        }
    }
    #endregion

    #region Object
    /// <summary>
    /// 오브젝트 풀에서 플레이어를 꺼내온 후 오브젝트 세팅하는 함수
    /// </summary>
    /// <param name="p_SubType"></param>
    /// <returns></returns>
    ObjectType GetPlayer(int p_SubType)
    {
        ObjectType newObjectType = objectManager.GetObjectType((int)ObjectTypeEnum.Player);
        newObjectType.mainType = (int)ObjectTypeEnum.Player;
        newObjectType.subType = p_SubType;

        SpriteRenderer newSprite = newObjectType.GetComponent<SpriteRenderer>();
        if (p_SubType > playerSprites.Length)
        {
            newSprite.sprite = playerSprites[playerSprites.Length - 1];
        }
        else
        {
            newSprite.sprite = playerSprites[p_SubType];
        }

        BoxCollider2D newCollider = newObjectType.gameObject.AddComponent<BoxCollider2D>();
        newCollider.size = new Vector2(0.5f, 1);

        Rigidbody2D newRigid = newObjectType.gameObject.AddComponent<Rigidbody2D>();
        newRigid.bodyType = RigidbodyType2D.Kinematic;
        newRigid.gravityScale = 0;

        Player newPlayer = newObjectType.gameObject.AddComponent<Player>();
        newPlayer.Init(this);

        return newObjectType;
    }

    /// <summary>
    /// 오브젝트 풀에서 적을 꺼내온 후 오브젝트 세팅하는 함수
    /// </summary>
    /// <param name="p_SubType"></param>
    /// <returns></returns>
    ObjectType GetEnemy(int p_SubType)
    {
        ObjectType newObjectType = objectManager.GetObjectType((int)ObjectTypeEnum.Enemy);

        newObjectType.subType = p_SubType;

        if (!newObjectType.mainType.Equals((int)ObjectTypeEnum.Enemy))
        {
            CircleCollider2D newCollider = newObjectType.gameObject.AddComponent<CircleCollider2D>();
            newCollider.isTrigger = true;
            newCollider.radius = 0.16f;

            Rigidbody2D newRigid = newObjectType.gameObject.AddComponent<Rigidbody2D>();
            newRigid.gravityScale = 0;

            Enemy newEnemy = newObjectType.gameObject.AddComponent<Enemy>();
            newEnemy.Init(this, player);
            newEnemy.ReInit();

            newObjectType.mainType = (int)ObjectTypeEnum.Enemy;
        }
        else
        {
            Enemy newEnemy = newObjectType.GetComponent<Enemy>();
            newEnemy.ReInit();
        }

        SpriteRenderer newSprite = newObjectType.GetComponent<SpriteRenderer>();
        if (p_SubType > playerSprites.Length)
        {
            newSprite.sprite = enemySprites[enemySprites.Length - 1];
        }
        else
        {
            newSprite.sprite = enemySprites[p_SubType];
        }
        newSprite.flipY = true;

        return newObjectType;
    }

    /// <summary>
    /// 오브젝트 풀에서 총알을 꺼내온 후 오브젝트 세팅하는 함수
    /// </summary>
    /// <param name="p_SubType"></param>
    /// <param name="p_Position"></param>
    /// <param name="p_Rotation"></param>
    /// <param name="p_Force"></param>
    public void GetBullet(int p_SubType, Vector3 p_Position, Quaternion p_Rotation, Vector2 p_Force)
    {
        ObjectType newObjectType = objectManager.GetObjectType((int)ObjectTypeEnum.Bullet);
        Rigidbody2D newRigidbody = null;

        if (!newObjectType.mainType.Equals((int)ObjectTypeEnum.Bullet))
        {
            CircleCollider2D newCollider = newObjectType.gameObject.AddComponent<CircleCollider2D>();
            newCollider.isTrigger = true;
            newCollider.radius = 0.16f;

            newRigidbody = newObjectType.gameObject.AddComponent<Rigidbody2D>();
            newRigidbody.gravityScale = 0;

            newObjectType.mainType = (int)ObjectTypeEnum.Bullet;
        }
        else
        {
            newRigidbody = newObjectType.GetComponent<Rigidbody2D>();
        }

        SpriteRenderer newSprite = newObjectType.GetComponent<SpriteRenderer>();
        if (p_SubType > playerSprites.Length)
        {
            newSprite.sprite = bulletSprites[bulletSprites.Length - 1];
        }
        else
        {
            newSprite.sprite = bulletSprites[p_SubType];
        }

        newObjectType.subType = p_SubType;

        newObjectType.transform.SetPositionAndRotation(p_Position, p_Rotation);
        newRigidbody.AddForce(p_Force, ForceMode2D.Impulse);
    }

    /// <summary>
    /// 오브젝트 풀에서 아이템을 꺼내온 후 오브젝트 세팅하는 함수
    /// </summary>
    /// <param name="p_Position"></param>
    public void GetItem(Vector3 p_Position)
    {
        ObjectType newObjectType = objectManager.GetObjectType((int)ObjectTypeEnum.Item);
        Rigidbody2D newRigidbody = null;
        int subType = Random.Range(0, itemSprites.Length);
        newObjectType.subType = subType;

        if (!newObjectType.mainType.Equals((int)ObjectTypeEnum.Item))
        {
            CircleCollider2D newCollider = newObjectType.gameObject.AddComponent<CircleCollider2D>();
            newCollider.isTrigger = true;
            newCollider.radius = 0.16f;

            newRigidbody = newObjectType.gameObject.AddComponent<Rigidbody2D>();
            newRigidbody.gravityScale = 0;

            newObjectType.mainType = (int)ObjectTypeEnum.Item;
        }
        else
        {
            newRigidbody = newObjectType.GetComponent<Rigidbody2D>();
        }

        SpriteRenderer newSprite = newObjectType.GetComponent<SpriteRenderer>();
        if (subType > playerSprites.Length)
        {
            newSprite.sprite = itemSprites[itemSprites.Length - 1];
        }
        else
        {
            newSprite.sprite = itemSprites[subType];
        }

        newObjectType.transform.position = p_Position;
        newRigidbody.AddForce(Vector2.down, ForceMode2D.Impulse);
    }

    /// <summary>
    /// 오브젝트 풀에서 이펙트를 꺼내온 후 오브젝트 세팅하는 함수
    /// </summary>
    /// <param name="p_SubType"></param>
    /// <param name="p_Position"></param>
    public void GetEffect(int p_SubType, Vector3 p_Position)
    {
        ObjectType newObjectType = objectManager.GetObjectType((int)ObjectTypeEnum.Effect);
        
        if (!newObjectType.mainType.Equals((int)ObjectTypeEnum.Effect))
        {
            newObjectType.mainType = (int)ObjectTypeEnum.Effect;
        }

        newObjectType.subType = p_SubType;
        newObjectType.transform.position = p_Position;

        StartCoroutine(Effect(newObjectType));
    }

    /// <summary>
    /// 꺼내온 이펙트를 실행한 후 오브젝트 풀로 반환하는 함수
    /// </summary>
    /// <param name="p_ObjectType"></param>
    /// <returns></returns>
    IEnumerator Effect(ObjectType p_ObjectType)
    {
        SpriteRenderer spriteRenderer = p_ObjectType.GetComponent<SpriteRenderer>();
        for(int i = 0; i < effectSprites.Length; i++)
        {
            spriteRenderer.sprite = effectSprites[i];
            yield return waitForEffect;
        }
        spriteRenderer.sprite = null;

        SetObject(p_ObjectType);
    }
    #endregion

    #region PrivateFunction
    /// <summary>
    /// HP, MP변동시 UI에 반영하는 함수
    /// </summary>
    /// <param name="p_IsHp"></param>
    /// <param name="p_IsPlus"></param>
    /// <param name="p_Count"></param>
    public void HpMpUpdate(bool p_IsHp, bool p_IsPlus, int p_Count)
    {
        if(p_IsHp)
        {
            HpObj[p_Count].SetActive(p_IsPlus);
        }
        else
        {
            MpObj[p_Count].SetActive(p_IsPlus);
        }
    }

    /// <summary>
    /// 터치값을 받아와서 계산 후 UI에 반영하는 함수
    /// 터치 시작시 호출
    /// </summary>
    /// <param name="p_Position"></param>
    public void JoystickMove(Vector2 p_Position)
    {
        Vector2 joystickPos = p_Position - screenHalfSize;
        joystickPos.x *= ratioX;
        joystickPos.y *= ratioY;
        joystickTran.localPosition = joystickPos;

        joystickTran.gameObject.SetActive(true);
    }

    /// <summary>
    /// 터치값을 받아와서 계산 후 UI에 반영하는 함수
    /// 터치 해제시 호출
    /// </summary>
    /// <param name="p_Position"></param>
    public void JoystickMove()
    {
        joystickTran.gameObject.SetActive(false);
    }

    /// <summary>
    /// 오브젝트풀에 반환하는 함수
    /// </summary>
    /// <param name="p_ObjectType"></param>
    public void SetObject(ObjectType p_ObjectType)
    {
        objectManager.SetObjectType(p_ObjectType);
    }

    /// <summary>
    /// 점수 추가시 값을 저장 후 UI에 반영하는 함수
    /// </summary>
    /// <param name="p_Score"></param>
    public void GetScore(int p_Score)
    {
        gameScore += p_Score;
        scoreText.text = string.Format("{0:n0}", gameScore);
    }

    /// <summary>
    /// GameOver시 UI에 반영하는 함수
    /// </summary>
    public void GameOver()
    {
        Time.timeScale = 0;

        mainTextString.Clear();
        mainTextString.Append("GAMEOVER");
        mainText.text = mainTextString.ToString();
        mainText.color = new Color(1, 1, 1, 1);

        gameOverObj.SetActive(true);
    }

    /// <summary>
    /// 종료 버튼 클릭시 로비씬으로 이동하는 함수
    /// </summary>
    public void SceneMove()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }
    #endregion
}
