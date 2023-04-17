using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Title씬의 이벤트를 관리하는 클래스
/// </summary>
public class TitleSceneManager : MonoBehaviour
{
    [SerializeField] Slider loadingbar;

    WaitForSeconds waitForSceneLoad;

    void Awake()
    {
        Init();
        StartCoroutine(SceneLoad());
    }

    /// <summary>
    /// 전역변수를 초기화하는 함수
    /// </summary>
    void Init()
    {
        Application.targetFrameRate = 60;

        waitForSceneLoad = new WaitForSeconds(0.01f);
    }

    /// <summary>
    /// 로비씬으로 이동하는 함수
    /// </summary>
    /// <returns></returns>
    IEnumerator SceneLoad()
    {
        int loadingCount = 0;
        while(!loadingCount.Equals(50))
        {
            yield return waitForSceneLoad;
            loadingCount++;
            loadingbar.value = loadingCount * 0.02f;
        }

        SceneManager.LoadSceneAsync(1);
    }
}
