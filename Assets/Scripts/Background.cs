using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    List<Transform> backgroundList = new List<Transform>();

    WaitForSeconds waitForScroll = new WaitForSeconds(0.016f);

    float yUnit;

    void Awake()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            backgroundList.Add(transform.GetChild(i));
        }

        Sprite sprite = backgroundList[0].gameObject.GetComponent<SpriteRenderer>().sprite;
        float height = sprite.rect.height;
        float pixelsperUnit = sprite.pixelsPerUnit;
        yUnit = height / pixelsperUnit;

        StartCoroutine(Scroll());
    }

    IEnumerator Scroll()
    {
        while(true)
        {
            yield return waitForScroll;

            for (int i = 0; i < backgroundList.Count; i++)
            {
                backgroundList[i].position += Vector3.down * 0.06f;
                if (backgroundList[i].position.y < -yUnit)
                {
                    backgroundList[i].position += Vector3.up * yUnit * 2;
                }
            }
        }
    }
}
