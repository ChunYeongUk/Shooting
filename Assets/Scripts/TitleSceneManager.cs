using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Title���� �̺�Ʈ�� �����ϴ� Ŭ����
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
    /// ���������� �ʱ�ȭ�ϴ� �Լ�
    /// </summary>
    void Init()
    {
        Application.targetFrameRate = 60;

        waitForSceneLoad = new WaitForSeconds(0.01f);
    }

    /// <summary>
    /// �κ������ �̵��ϴ� �Լ�
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
