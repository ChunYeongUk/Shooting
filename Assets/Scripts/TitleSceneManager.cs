using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleSceneManager : MonoBehaviour, IPointerDownHandler
{
    int twinkleCount;
    [SerializeField] private TMP_Text text;
    WaitForSeconds waitForTwinkle = new WaitForSeconds(0.1f);

    void Awake()
    {
        Application.targetFrameRate = 60;
        StartCoroutine(TextTwinkle());
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SceneManager.LoadScene(1);
    }

    IEnumerator TextTwinkle()
    {
        Color twinkleColor = Color.black;

        while (true)
        {
            yield return waitForTwinkle;

            if (twinkleCount < 5)
            {
                twinkleColor.a -= 0.1f * twinkleCount;
                text.color = twinkleColor;
            }
            else
            {
                twinkleColor.a = 0.1f * twinkleCount;
                text.color = twinkleColor;
                if(twinkleCount.Equals(10))
                {
                    twinkleCount = -1;
                }
            }

            twinkleCount++;
        }
    }
}
