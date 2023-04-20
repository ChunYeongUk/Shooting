using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Text;
using System.IO;

/// <summary>
/// Tag대신 사용하기 위한 Enum
/// </summary>
enum MainType
{
    Player,
    Enemy,
    PlayerBullet,
    EnemyBullet,
    Item,
    Effect,
    TypeCount,
}

enum ItemType
{
    GoldUp,
    HpUp,
    MpUp,
    PowerUp,
    TypeCount,
}

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
/// Game씬의 이벤트를 관리하는 클래스
/// </summary>
public class GameManager : Singleton<GameManager>
{
    [SerializeField] TMP_Text mainText;
    [SerializeField] TMP_Text scoreText;

    [SerializeField] GameObject[] HpObj;
    [SerializeField] GameObject[] MpObj;
    [SerializeField] GameObject gameOverObj;

    int gameScore;
    int mainTextAlpha;

    StringBuilder mainTextString;
    WaitForSeconds waitForMainText;

    [SerializeField] TextAsset[] stageArray;
    List<SpawnInfo> spawnInfoList;

    int stageLevel;
    int stageCount;

    float nextSpawnDelay;
    float curSpawnDelay;

    int spawnIndex;
    bool isStageEnd;

    ObjectPoolManager objectPool;
    [HideInInspector] public Player player;
    [SerializeField] Joystick joystick;
            
    [SerializeField] Sprite[] characterSprites;
    [SerializeField] Sprite[] bulletSprites;
    [SerializeField] Sprite[] itemSprites;
    [SerializeField] Sprite[] effectSprites;

    WaitForSeconds waitForStage;

    void Awake()
    {
        Init();
        StartCoroutine(MainText());
    }

    /// <summary>
    /// 전역변수를 초기화하는 함수
    /// </summary>
    void Init()
    {
        objectPool = GetComponent<ObjectPoolManager>();
        objectPool.Init();
        if(objectPool.Spawn((int)MainType.Player, PlayerPrefs.GetInt("lastCharacter", 0)).TryGetComponent(out player))
        {
            player.Init(this);
        }
        else
        {
            SceneMove();
        }

        joystick.Init(player);

        mainTextString = new StringBuilder(3);
        waitForMainText = new WaitForSeconds(0.1f);
        waitForStage = new WaitForSeconds(0.016f);

        spawnInfoList = new List<SpawnInfo>();
    }

    #region StageManagement
    /// <summary>
    /// 스테이지를 준비하는 함수
    /// </summary>
    void StageStart()
    {
        spawnInfoList.Clear();
        spawnIndex = 0;
        isStageEnd = false;

        StringReader stringReader = new StringReader(stageArray[stageLevel].text);
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

        StartCoroutine(StageProgress());
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

        if (stageLevel.Equals(stageArray.Length - 1))
        {
            stageLevel = 0;
        }
        else
        {
            stageLevel++;
        }
        StartCoroutine(MainText());
    }

    /// <summary>
    /// 스테이지 시작전 Text를 켰다가 끄는 함수
    /// </summary>
    /// <returns></returns>
    IEnumerator MainText()
    {
        stageCount++;
        mainTextString.Clear();
        mainTextString.Append("STAGE ");
        mainTextString.Append(stageCount);
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
        ObjectType newObjectType = objectPool.Spawn((int)MainType.Enemy, spawnInfoList[spawnIndex].spawnType);
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
    /// 오브젝트 풀에서 총알을 꺼내온 후 오브젝트 세팅하는 함수
    /// </summary>
    /// <param name="p_SubType"></param>
    /// <param name="p_Position"></param>
    /// <param name="p_Rotation"></param>
    /// <param name="p_Force"></param>
    public void GetBullet(int p_MainType, int p_SubType, Vector3 p_Position, Quaternion p_Rotation, Vector2 p_Force)
    {
        ObjectType newObjectType = objectPool.Spawn(p_MainType, p_SubType);
        newObjectType.transform.SetPositionAndRotation(p_Position, p_Rotation);

        Rigidbody2D newRigidbody = newObjectType.GetComponent<Rigidbody2D>();
        newRigidbody.velocity = Vector2.zero;
        newRigidbody.AddForce(p_Force, ForceMode2D.Impulse);
    }

    /// <summary>
    /// 오브젝트 풀에서 아이템을 꺼내온 후 오브젝트 세팅하는 함수
    /// </summary>
    /// <param name="p_Position"></param>
    public void GetItem(Vector3 p_Position)
    {
        int subType = Random.Range(0, (int)ItemType.TypeCount);

        ObjectType newObjectType = objectPool.Spawn((int)MainType.Item, subType);
        newObjectType.transform.position = p_Position;

        Rigidbody2D newRigidbody = newObjectType.GetComponent<Rigidbody2D>();
        newRigidbody.AddForce(Vector2.down, ForceMode2D.Impulse);
    }

    /// <summary>
    /// 오브젝트 풀에서 이펙트를 꺼내온 후 오브젝트 세팅하는 함수
    /// </summary>
    /// <param name="p_SubType"></param>
    /// <param name="p_Position"></param>
    public void GetEffect(int p_SubType, Vector3 p_Position)
    {
        ObjectType newObjectType = objectPool.Spawn((int)MainType.Effect, p_SubType);
        newObjectType.transform.position = p_Position;

        Animator newAnimator = newObjectType.GetComponent<Animator>();
        newAnimator.SetTrigger("On");
    }
    #endregion

    #region PublicFunction
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
    /// 오브젝트풀에 반환하는 함수
    /// </summary>
    /// <param name="p_ObjectType"></param>
    public void SetObject(ObjectType p_ObjectType)
    {
        objectPool.Despawn(p_ObjectType);
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

    /// <summary>
    /// 재시작 버튼 클릭시 로비씬으로 이동하는 함수
    /// </summary>
    public void SceneReload()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(2);
    }
    #endregion
}
