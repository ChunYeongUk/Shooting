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
    /// ���������� �ʱ�ȭ�ϴ� �Լ�
    /// </summary>
    /// <param name="p_Player"></param>
    public void Init(Player p_Player)
    {
        player = p_Player;
        pointerList = new List<PointerEventData>();
    }

    /// <summary>
    /// ��ġ�� ���̽�ƽ�� �Ѵ� �Լ�
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
    /// �÷��̾ �̵������� �����ϴ� �Լ�
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
    /// ��ġ ������ ���̽�ƽ�� ���� �Լ�
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
    /// ���� Ŭ�� ���θ� Ȯ���ϴ� �Լ�
    /// </summary>
    /// <returns></returns>
    IEnumerator DoubleTabCheck()
    {
        isDoubleTab = true;
        yield return waitForDoubleTab;
        isDoubleTab = false;
    }
}
