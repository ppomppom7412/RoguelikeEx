using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour {
    public GameObject[] stageMakers;

    // 스포너 메니저
    public GameObject spawnerManager;
    // 플레이어 소환포인트
    public GameObject playerSpawnPoint;


    public bool isDebug;

    public int previousIndex;
    public int totalIndex = 0;
    public int currentIndex = -1;
    public int CurrentIndex
    {
        get { return currentIndex; }
        set {
                previousIndex = currentIndex;

                if (stageMakers.Length > value && value >= 0)
                {
                    currentIndex = value;
                }
                else
                {
                    currentIndex = 0;
                }

            if (isDebug)
                Debug.Log("CurrentIndex Increase "+ previousIndex +" > "+ currentIndex);
        }
    }

	void Start () {
        //StageMakerAllOff();

        previousIndex = stageMakers.Length-1;
    }
	
	void Update () {
        InputKeyboard();

    }

    public void SetRandomStageMakerArr()
    {
        StageMakerAllOff();

        // 스테이지 메이커 배열위치 랜덤 변경
        RandomChangeArr();

        //UpdateStage(); 
    }

    // 키보드 값을 받는다.
    void InputKeyboard()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isDebug)
                Debug.Log("GetKeyDown(Space) in InputKeyboard()");

            //if (CurrentIndex >=0 && currentIndex < stageMakers.Length)
            //    stageMakers[CurrentIndex].SendMessage("SetUnactiveRooms");

            ++CurrentIndex;
            ++totalIndex;

            if (previousIndex >= 0 && previousIndex < stageMakers.Length)
                stageMakers[previousIndex].SendMessage("SetUnactiveRooms");

            if (currentIndex == 0)
                SetRandomStageMakerArr();

            UpdateStage();
        }
    }

    //모든 스테이지 메이커를 끈다
    void StageMakerAllOff()
    {
        for (int i = 0; i < stageMakers.Length; ++i) {
            stageMakers[i].SetActive(false);
        }
    }

    // Active 를 바꾼다.
    void SwitchingActive()
    {

        if (stageMakers.Length < previousIndex || previousIndex < 0)
            StageMakerAllOff();
        else
        {
            stageMakers[previousIndex].SetActive(false);
        }
        stageMakers[CurrentIndex].SetActive(true);

    }

    // 스테이지를 업데이트 시킨다.
    void UpdateStage()
    {
        SwitchingActive();

        //새롭게 방만들기
        stageMakers[CurrentIndex].SendMessage("StartBulingTheStage");

        //새롭게 환경적용 (중첩 시스템때문에 필요할때만 불러야한다.)
        stageMakers[CurrentIndex].SendMessage("SettingMyEnviornment");

        spawnerManager.SendMessage("ResetSpawners");
        playerSpawnPoint.GetComponent<PlayerSpawnPoint>().PlayerSpawn(stageMakers[CurrentIndex].GetComponent<StageMaker>().GetStartRoomPosition());
    }

    //두 배열위치를 바꾼다.
    void ReplaceArr(int _target, int _place)
    {
        if (_target < 0 || _target >= stageMakers.Length) return;
        if (_place < 0 || _place >= stageMakers.Length) return;

        GameObject temp = stageMakers[_place];

        stageMakers[_place] = stageMakers[_target];
        stageMakers[_target] = temp;
    }

    // 배열을 랜덤으로 바꾼다. //비율 추가 가능.
    public void RandomChangeArr()
    {
        int random = 0;
        for (int i = 0; i < stageMakers.Length; ++i)
        {
            random = Random.Range(0, stageMakers.Length - 1);
            ReplaceArr(i, random);
        }
    }
}
