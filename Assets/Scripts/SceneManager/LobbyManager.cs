using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Lobby���� �̺�Ʈ�� �����ϴ� Ŭ����
/// </summary>
public class LobbyManager : MonoBehaviour
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

    /// <summary>
    /// ���������� �ʱ�ȭ�ϴ� �Լ�
    /// </summary>
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
    /// ���� ��ư Ŭ�� �̺�Ʈ
    /// </summary>
    void StartBtnEvent()
    {
        PlayerPrefs.SetInt("lastCharacter", characterNum);
        SceneManager.LoadScene(2);
    }

    /// <summary>
    /// ���� ��ư Ŭ�� �̺�Ʈ
    /// </summary>
    void SettingBtnEvent()
    {
        settingObject.SetActive(!settingObject.activeSelf);
    }

    /// <summary>
    /// ���� ��ư Ŭ�� �̺�Ʈ
    /// </summary>
    void QuitBtnEvent()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // ���ø����̼� ����
#endif
    }

    /// <summary>
    /// ���� ��ư Ŭ�� �̺�Ʈ
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
    /// ���� ��ư Ŭ�� �̺�Ʈ
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
