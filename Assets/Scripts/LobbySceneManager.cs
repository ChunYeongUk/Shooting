using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Lobby씬의 이벤트를 관리하는 클래스
/// </summary>
public class LobbySceneManager : MonoBehaviour
{
    [SerializeField] Button startBtn;
    [SerializeField] Button settingBtn;
    [SerializeField] Button quitBtn;
    [SerializeField] Button nextBtn;
    [SerializeField] Button prevBtn;

    [SerializeField] GameObject settingObject;

    [SerializeField] RawImage characterImage;
    [SerializeField] Texture[] characters;
    int characterNum;

    private void Awake()
    {
        Init();
    }

    void Init()
    {
        startBtn.onClick.AddListener(StartBtnEvent);
        settingBtn.onClick.AddListener(SettingBtnEvent);
        quitBtn.onClick.AddListener(QuitBtnEvent);
        nextBtn.onClick.AddListener(NextBtnEvent);
        prevBtn.onClick.AddListener(PrevBtnEvent);

        characterNum = PlayerPrefs.GetInt("lastCharacter", 0);
        characterImage.texture = characters[characterNum];
    }

    /// <summary>
    /// 시작 버튼 클릭 이벤트
    /// </summary>
    void StartBtnEvent()
    {
        PlayerPrefs.SetInt("lastCharacter", characterNum);
        SceneManager.LoadScene(2);
    }

    /// <summary>
    /// 설정 버튼 클릭 이벤트
    /// </summary>
    void SettingBtnEvent()
    {
        settingObject.SetActive(!settingObject.activeSelf);
    }

    /// <summary>
    /// 종료 버튼 클릭 이벤트
    /// </summary>
    void QuitBtnEvent()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit(); // 어플리케이션 종료
#endif
    }

    /// <summary>
    /// 다음 버튼 클릭 이벤트
    /// </summary>
    void NextBtnEvent()
    {
        characterNum++;
        if(characterNum.Equals(characters.Length))
        {
            characterNum = 0;
        }
        characterImage.texture = characters[characterNum];
    }

    /// <summary>
    /// 이전 버튼 클릭 이벤트
    /// </summary>
    void PrevBtnEvent()
    {
        characterNum--;
        if (characterNum.Equals(-1))
        {
            characterNum = characters.Length - 1;
        }
        characterImage.texture = characters[characterNum];
    }
}
