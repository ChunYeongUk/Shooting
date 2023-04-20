using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    List<PointerEventData> pointerList;

    [SerializeField] RectTransform joystickTran;
    Player player;

    Vector2 firstPos;

    bool isDoubleTab;

    WaitForSeconds waitForDoubleTab = new WaitForSeconds(0.2f);

    /// <summary>
    /// 전역변수를 초기화하는 함수
    /// </summary>
    /// <param name="p_Player"></param>
    public void Init(Player p_Player)
    {
        player = p_Player;
        pointerList = new List<PointerEventData>();
    }

    /// <summary>
    /// 터치시 조이스틱을 켜는 함수
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (pointerList.Count.Equals(0))
        {
            firstPos = eventData.position;

            joystickTran.anchoredPosition = firstPos;
            joystickTran.gameObject.SetActive(true);
            player.TouchStart();
        }
        pointerList.Add(eventData);
    }

    /// <summary>
    /// 플레이어에 이동방향을 전달하는 함수
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if (!pointerList.Count.Equals(0))
        {
            if (eventData.Equals(pointerList[0]))
            {
                player.ChangeMoveDir(eventData.position - firstPos);
            }
        }
    }

    /// <summary>
    /// 터치 해제시 조이스틱을 끄는 함수
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!pointerList.Count.Equals(0))
        {
            if (eventData.Equals(pointerList[0]))
            {
                player.TouchEnd();
                player.ChangeMoveDir(Vector2.zero);
                joystickTran.gameObject.SetActive(false);
                pointerList.Clear();
                if(!isDoubleTab)
                {
                    StartCoroutine(DoubleTabCheck());
                }
                else
                {
                    isDoubleTab = false;
                    player.SkillAttack();
                }
            }
        }
    }

    /// <summary>
    /// 더블 클릭 여부를 확인하는 함수
    /// </summary>
    /// <returns></returns>
    IEnumerator DoubleTabCheck()
    {
        isDoubleTab = true;
        yield return waitForDoubleTab;
        isDoubleTab = false;
    }
}
