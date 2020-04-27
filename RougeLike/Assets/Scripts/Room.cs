using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    static public StageManager stagemananger;

    public Pos size;
    public float interval;
    public Pos mapPos;

    private bool isSettingPos;
    private bool isExplore;

    public void Start()
    {
        if (size == null)
            size = new Pos(1, 1);

        isSettingPos = false;
        isExplore = false;

        if (stagemananger == null)
            stagemananger = GameObject.FindObjectOfType<StageManager>();
    }

    public void Update()
    {
        CheckSuccessRoom();
    }


    /// <summary>
    /// 방의 위치 설정 및 변경값 수정
    /// </summary>
    public void ReplaceRoom()
	{
        if (!isSettingPos)
        {
            isSettingPos = true;
            isExplore = false;

            Vector3 pos = new Vector3(mapPos.x, 0, mapPos.y);

            pos.x *= interval;
            pos.z *= interval;

            gameObject.transform.position = pos;
        }
    }

    /// <summary>
    /// 방의 상태 및 위치 초기화
    /// </summary>
    public void ResetRoom()
    {

        mapPos.Reset();
        gameObject.SetActive(false);
        isSettingPos = false;
    }

    public Vector3 GetWorldPositon()
    {
        return gameObject.transform.position;
    }

    /// <summary>
    /// 룸이 맵상에 등장할때 사용.
    /// </summary>
    public void OpenRoom()
    {
        UISetRoom();
        gameObject.SetActive(true);
    }

    // 방의 정보를 UI에 등록한다. <+첫/끝방 표시 포함>
    void UISetRoom()
    {
        if (MapUI.instance != null)
        {
            MapUI.instance.SetRoomdata(mapPos, size, "<" + mapPos.x.ToString() + ", " + mapPos.y.ToString() + ">");

            //첫/끝방 표시 추가 바람.
        }
    }

    //플레이어의 입장을 UI에 등록한다.
    void SetPlayerPosition()
    {
        if (MapUI.instance != null)
        {
            MapUI.instance.SetPlayerPostion(mapPos);
        }
    }

    //방에 있는 모든 몬스터가 사라졌을때, 해당 StageMaker의 함수를 불러준다.
    // 해당 방에 근접하는 복도와 방을 화면상에 노출시킨다.

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            SetPlayerPosition();
        }
    }

    //해당 룸을 성공적으로 마쳤을때
    void CheckSuccessRoom()
    {
        if (isExplore) return;
        if (stagemananger == null)
        {
            Debug.Log("All room not get StageManager!");
            return;
        }

        //Debug.Log("Room<"+mapPos.StringPos()+"> Player<"+MapUI.instance.playerPos.StringPos()+">");

        //조건 검사를 몬스터 사라졌을때로 변경하면 동일
        if (MapUI.instance.playerPos.x == mapPos.x && MapUI.instance.playerPos.y == mapPos.y)
        {
            Debug.Log("여기요! 제가 다 잡았소. 어서오시오.");
            stagemananger.stageMakers[stagemananger.currentIndex].SendMessage("ShowNearPassageAndRoom");
            isExplore = true;
        }

    }

}