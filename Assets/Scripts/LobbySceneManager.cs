using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Lobby���� �̺�Ʈ�� �����ϴ� Ŭ����
/// </summary>
public class LobbySceneManager : MonoBehaviour
{
    /// <summary>
    /// ���۹�ư Ŭ�� �̺�Ʈ
    /// </summary>
    public void SceneMove()
    {
        SceneManager.LoadScene(2);
    }
}
