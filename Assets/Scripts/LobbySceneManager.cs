using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbySceneManager : MonoBehaviour
{
    public void SceneMove()
    {
        SceneManager.LoadScene(2);
    }
}
