using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Lobby씬의 이벤트를 관리하는 클래스
/// </summary>
public class LobbySceneManager : MonoBehaviour
{
    /// <summary>
    /// 시작버튼 클릭 이벤트
    /// </summary>
    public void SceneMove()
    {
        SceneManager.LoadScene(2);
    }
}
