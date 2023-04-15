using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Lobby���� �̺�Ʈ�� �����ϴ� Ŭ����
/// </summary>
public class LobbySceneManager : MonoBehaviour
{
    [SerializeField] Button startBtn;
    [SerializeField] Button settingBtn;
    [SerializeField] Button quitBtn;

    [SerializeField] GameObject settingObject;

    private void Awake()
    {
        Init();
    }

    void Init()
    {
        startBtn.onClick.AddListener(StartBtnEvent);
        settingBtn.onClick.AddListener(SettingBtnEvent);
        quitBtn.onClick.AddListener(QuitBtnEvent);
    }

    /// <summary>
    /// ���� ��ư Ŭ�� �̺�Ʈ
    /// </summary>
    void StartBtnEvent()
    {
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
}
