using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// http://naplandgames.com/blog/2016/11/18/unity-3d-tutorial-procedural-map-generation-beginner-level/
/// 로그라이크 알고리즘
/// </summary>


public class StageMaker : MonoBehaviour {

    public EnviornmentList myEnviornment;

    public GameObject VertPassage;
    public GameObject HoriPassage;
    public Pos mapSize;
    public bool isDebug;

    [SerializeField]
    private List<PassageData> passageInfo;      //방과 방사이를 잇는 통로 정보
    [SerializeField]
    private int[,] map;                         //방 위치 정보를 담는 맵
    [SerializeField]
    private float roomInterval = 4;             //방 사이 간격
    private Vector3 startRoomPosition;          // 시작방 위치

    #region stageinfo
    public GameObject[] roomObjects;
    public int startRoomnum = 0;
    public int maxRoomCount;
    #endregion

    void Start () {

        //시작 방 설정이 존재할 시 0번으로 이동
        if (isDebug) Debug.Log("SettingStartRoom() Starting....");
        SettingStartRoom();

        //StartBulingTheStage();
    }
	
	void Update () {
		
	}

    /// <summary> 
    /// 현재 스테이지 생성
    /// </summary>
    public void StartBulingTheStage()
    {
        if (isDebug) Debug.Log("현재 스테이지 생성를 생성합니다.");

        //SetUnactiveRooms();

        //초기화
        map = new int[mapSize.x, mapSize.y];
        passageInfo = new List<PassageData>();

        for (int m = 0; m < mapSize.x; ++m)
            for (int n = 0; n < mapSize.y; ++n)
                map[m, n] = -1;

        // 가진 복도를 모두 지운다.
        if (isDebug) Debug.Log("RemovePassageObjects() Starting....");
        RemovePassageObjects();

        //시작방을 제외한 나머지 방의 배열 변경
        if (isDebug) Debug.Log("RandomChangeRoomNumder() Starting....");
        RandomChangeRoomNumder();

        //맵 위에 랜덤으로 방 배치
        if (isDebug) Debug.Log("SettingRandomPosForMap() Starting....");
        SettingRandomPosForMap();

        //맵을 기준으로 방을 생성
        if (isDebug) Debug.Log("CreateNewStage() Starting....");
        CreateNewStage();

        //등록된 PassageInfo를 통한 복도생성
        if (isDebug) Debug.Log("CreateNewPassage() Starting....");
        CreateNewPassage();

        // 첫번째 방을 열어준다.
        ShowFirstRoom();
        MapUI.instance.SetPlayerPostion(mapSize.x / 2, mapSize.y / 2);

        if (isDebug) Debug.Log("현재 스테이지 생성을 완료하였습니다.");
    }

    //시작 방을 설정한다.
    void SettingStartRoom()
    {
        if (startRoomnum >= roomObjects.Length)
            startRoomnum = roomObjects.Length-1;

        if (startRoomnum > 0) 
            ReplaceRoomArr(startRoomnum, 0); //시작방 번호를 지정했을시에 해당 방을 0번으로 바꾼다.

    }

    //방을 랜덤으로 위치 시키기
    void SettingRandomPosForMap()
    {
        //필요한 데이터 선언 및 초기화
        Pos newpos = new Pos();
        Pos senter = new Pos();
        Pos lastpos = new Pos();
        int select = -1;
        int roomcount = 0;
        Pos getRoomSize;

        List<int> nonselectnum = new List<int>(); //선택되지 않은 방번호를 담는다
        List<Pos> checkList = new List<Pos>();
        PassageData pass = null;

        for (int s = 0; s < roomObjects.Length-1; ++s)
            nonselectnum.Add(s);        //모든 방번호를 담는다.(처음과 끝을 제외한)

        //MapUI에게 방사이즈를 보낸다.
        if (MapUI.instance != null)
        {
            MapUI.instance.SetMapSize(mapSize);
        }

        // 처음방 설정
        nonselectnum.RemoveAt(0);                        //'0'을 가진 리스트항목을 지운다.
        senter.x = (int)(mapSize.x / 2);
        senter.y = (int)(mapSize.y / 2);
        roomObjects[0].GetComponent<Room>().mapPos = new Pos(senter.x, senter.y);
        checkList.Clear();

        getRoomSize = CheckTheObjectHaveRoomScript(roomObjects[0]).size;
        if (getRoomSize == null) return;

        for (int x = 0; x < getRoomSize.x; ++x)
        {
            for (int y = 0; y < getRoomSize.y; ++y)
            {
                //맵 중앙 좌표에 시작방 번호를 담는다.
                map[senter.x + x, senter.y + y] = 0;
                ++roomcount;

                //시작 방이 이어질 방의 갯수에 따라 담는다.
                checkList.Add(new Pos(senter.x + x, senter.y + y));
                checkList.Add(new Pos(senter.x + x, senter.y + y));
                checkList.Add(new Pos(senter.x + x, senter.y + y));
            }
        }

        #region 방 랜덤배치 설명
        // 0. 랜덤값, 중앙좌표를 비롯한 사용될 변수를 초기화한다.
        // 1. 큐의 맨 위에 것을 꺼내 그 위치를 중앙좌표으로 삼는다.
        // 2. (0,1) (0,-1) (1,0) (-1,0) 중 하나의 값을 받아온다
        // 3. 중앙좌표에 나온 좌표를 더한 값을 좌표로 선택된 방번호를 맵에 담는다.
        // 3-1. 만약 이미 그 좌표에 값이 존재한다면 <1로 돌아간다>
        // 3-2. 근접하는 방의 갯수에 따라 생성여부를 확인해도 된다.
        // 3-3. 이때 이어지는 방끼리 통로를 잇는다.
        // 4. 이미 사용된 방번호는 확인리스트에서 제외한다.
        // 5. 이어질 방 갯수에 따라 해당 방번호를 여러개 큐에 담는다. (0~3)
        #endregion

        //큐가 비어있으면 혹은 스테이지 최대갯수를 넘기면(미설정) 루프를 종료한다.
        while (checkList.Count > 0 && nonselectnum.Count >0 && roomcount< maxRoomCount)
        {
            // <0>
            select = -1;
            newpos.Reset();
            senter.Reset();

            select = nonselectnum[Random.Range(0, nonselectnum.Count)];

            // <1>
            senter = checkList[0];
            if (senter.isNull()) continue;  //받아온 좌표가 없으면
            checkList.RemoveAt(0);

            if (!CheckTheEmptymap(select, senter, out newpos)) continue;

            // <4>
            getRoomSize = CheckTheObjectHaveRoomScript(roomObjects[select]).size;
            if (getRoomSize == null) continue;

            //map[newpos.x, newpos.y] = select; <5>번과정에서 합류
            roomObjects[select].GetComponent<Room>().mapPos = new Pos(newpos.x, newpos.y);
            nonselectnum.Remove(select);

            lastpos.x = newpos.x;
            lastpos.y = newpos.y;

            // < 3.3>
            pass = new PassageData();
            pass.from = new Pos(senter.x, senter.y);    // from
            pass.to = new Pos(newpos.x, newpos.y);       // to
            passageInfo.Add(pass);  // 3.1 때문에 중복되는 통로가 생기지 않는다.
            if (isDebug) Debug.Log("PassageData From" + pass.from.StringPos() + " To" + pass.to.StringPos());

            // < 수정 부분 >
            // <5>
            for (int x = 0; x < getRoomSize.x; ++x)
            {
                for (int y = 0; y < getRoomSize.y; ++y)
                {
                    map[newpos.x+x, newpos.y+y] = select;
                    ++roomcount;

                    for (int random = Random.Range(1, 3); random > 0; --random)
                    checkList.Add(new Pos(newpos.x+x, newpos.y+y));

                }
            }
        }
        // <추가된 코드>
            CompulsTheRoomAtCreating(roomObjects.Length-1, lastpos);

    }

    // ------------- < 추가된 함수 >
    // 맵크기와 맵의 빈 공간을 찾아 할당 후 할당여부 반환
    bool CheckTheEmptymap(int _roomnum, Pos _senter, out Pos _newPos)
    {
        int count = Random.Range(0, 4);
        Pos posvalue = new Pos();
        _newPos = posvalue;

        for (int i = 3; i >= 0; --i)
        {
            SelectPlusMius(out posvalue.x, out posvalue.y, count);
            posvalue.x += _senter.x;
            posvalue.y += _senter.y;
            if (MakesureTheroomIsEmpty(posvalue, _roomnum))
            {
                _newPos.x = posvalue.x;
                _newPos.y = posvalue.y;
                return true;
            }

            ++count;

            if (count >= 4) count = 0;
        }

        return false;
    }

    /// <summary>
    /// 모든 방을 꺼준다.
    /// </summary>
    public void SetUnactiveRooms()
    {
        for (int i = 0; i < roomObjects.Length; ++i)
        {
            if (CheckTheObjectHaveRoomScript(roomObjects[i]))
                roomObjects[i].GetComponent<Room>().ResetRoom();
        }
    }

    //가진 복도를 지운다.
    void RemovePassageObjects()
    {
        for (int i = transform.GetChildCount()-1; i >= 0; --i) 
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    //map을 기준으로 방(프리팹)을 생성한다.
    //오브젝트 풀로 변경할 수 있으면 변경해보자.
    void CreateNewStage()
    { 

        Room createRoom = null;

        for (int x = 0; x < mapSize.x; ++x)  {
            for (int y = 0; y < mapSize.y; ++y) {
                if (MakesureTheroomIsEmpty(x,y)) continue;

                //roomObjects[map[x, y]].GetComponent<Room>().mapPos = new Pos(x, y);
                //roomObjects[map[x, y]].SetActive(true);
                roomObjects[map[x, y]].GetComponent<Room>().ReplaceRoom();
            }
        }
        startRoomPosition = roomObjects[0].GetComponent<Room>().GetWorldPositon();
    }

    //passageInfo를 기준으로 통로를 연결한다. (중복되는 통로X / 하지만 확인바람)
    //오브젝트 풀로 변경할 수 있으면 변경해보자.
    void CreateNewPassage()
    {
		int index = 0;
        GameObject passOb;
		while (index < passageInfo.Count)
        {
            if (passageInfo[index].CheckHorizon())
            {
                passOb = Instantiate(HoriPassage, new Vector3(passageInfo[index].SenterX(roomInterval), 0, passageInfo[index].SenterY(roomInterval)), Quaternion.identity, transform);   
            }
			else if (passageInfo[index].CheckVertical())
            {
                passOb = Instantiate(VertPassage, new Vector3(passageInfo[index].SenterX(roomInterval) , 0, passageInfo[index].SenterY(roomInterval)), Quaternion.identity, transform);
            }
            else
            {
                if (isDebug) Debug.Log("PassageInfo 생성실패");
                ++index;
                continue;
            }
			if (isDebug) Debug.Log("PassageInfo : " + passageInfo[index].from.StringPos() + " >> " + passageInfo[index].to.StringPos());


            //passageInfo.RemoveAt(0);

            if (!passOb.GetComponent<Passage>())
                passOb.AddComponent<Passage>().SetPassageData(passageInfo[index]);
            else
                passOb.GetComponent<Passage>().SetPassageData(passageInfo[index]);

            passOb.SetActive(false);

            ++index;
        }
    }

    // Room스크립트가 있는 지 확인해준다.
    Room CheckTheObjectHaveRoomScript(GameObject _ob)
    {
        if (_ob.GetComponent<Room>() == null)
        {
            Debug.Log(_ob.name + " 오브젝트에 Room 스크립트가 없습니다.");
            return null;
        }

        return _ob.GetComponent<Room>();
    }

    //방 배열을 랜덤으로 바꾼다.
    void RandomChangeRoomNumder()
    {
        int random = 1;
        for (int i=1; i< roomObjects.Length-1; ++i)
        {
            random = Random.Range(1, roomObjects.Length-2);
            ReplaceRoomArr(i, random);
        }
    }

    //두 방의 배열위치를 바꾼다.
    void ReplaceRoomArr(int _target, int _place)
    {
        if (_target < 0 || _target >= roomObjects.Length) return;
        if (_place < 0 || _place >= roomObjects.Length) return;

        GameObject temp = roomObjects[_place];

        roomObjects[_place] = roomObjects[_target];
        roomObjects[_target] = temp;
    }

    // (0,1) (0,-1) (1,0) (-1,0) 중 하나의 값을 받아온다
    void RandomSelectPlusMius(out int _x, out int _y) 
    {
        int r = Random.Range(0, 4);

        switch (r)
        {
            case 0:
                _x = 0;
                _y = 1;
                break;
            case 1:
                _x = 0;
                _y = -1;
                break;
            case 2:
                _x = 1;
                _y = 0;
                break;
            case 3:
                _x = -1;
                _y = 0;
                break;
            default:
                _x = 0;
                _y = 1;
                break;
        }
    }

    // ---------------- < 추가된 함수 >
    // (0,1) (0,-1) (1,0) (-1,0) 중 하나의 값을 받아온다
    void SelectPlusMius(out int _x, out int _y, int _count)
    {
        switch (_count)
        {
            case 0:
                _x = 0;
                _y = 1;
                break;
            case 1:
                _x = 0;
                _y = -1;
                break;
            case 2:
                _x = 1;
                _y = 0;
                break;
            case 3:
                _x = -1;
                _y = 0;
                break;
            default:
                _x = 0;
                _y = 1;
                break;
        }
    }

    // 맵의 x,y에 비어있는지 확인. 비어있으면 true.
    bool MakesureTheroomIsEmpty(int _x, int _y)
    {
        if (_x < 0 || _x >= mapSize.x) return false;
        if (_y < 0 || _y >= mapSize.y) return false;

        if (map[_x, _y] != -1) return false;

        return true;
    }

    // 맵의 x,y에 비어있는지 확인. 비어있으면 true.
    bool MakesureTheroomIsEmpty(Pos mypos, int roomnum)
    {
        if (mypos.x < 0 || mypos.x >= mapSize.x) return false;
        if (mypos.y < 0 || mypos.y >= mapSize.y) return false;

        if (map[mypos.x, mypos.y] != -1) return false;
        if (roomObjects.Length < roomnum) return false;

        Pos roomsize = CheckTheObjectHaveRoomScript(roomObjects[roomnum]).size;
        if (roomsize == null) return false;

        for (int x = 0; x < roomsize.x; ++x)
        {
            for (int y = 0; y < roomsize.y; ++y)
            {
                if (!MakesureTheroomIsEmpty(mypos.x + x, mypos.y + y)) return false;
            }
        }
            return true;
    }

    // 해당 값을 가진 좌표를 찾는다.
    Pos LookForAddressByValue(int _value)
    {
        Pos result = new Pos();
        for (int x = 0; x < mapSize.x; ++x) {
            for (int y = 0; y < mapSize.y; ++y) {

                if (map[x, y] == _value) {
                    result.y = y;
                    result.x = x;
                    break;
                }
            }
        }

        return result;
    }

    // ---------- 추가된 함수
    //강제로 방붙이기(1x1칸 기준)
    void CompulsTheRoomAtCreating(int _roomnum, Pos _last)
    {
        //탐색
        List<Pos> checkList = new List<Pos>();
        Pos stoppos = new Pos();
        Pos currentpos = new Pos(_last.x, _last.y);
        Pos savepos = new Pos();

        checkList.Add(currentpos);

        while (checkList.Count > 0)
        {
            currentpos = checkList[0];
            checkList.RemoveAt(0);

            if (CheckTheEmptymap(_roomnum, currentpos, out stoppos))
            {
                if (isDebug)
                    Debug.Log("Find LastRoom Spot! CompulsTheRoomAtCreating("+stoppos.x+", "+stoppos.y+")");
                checkList.Clear();
                break;
            }

            for (int i = 0; i < 4; ++i)
            {
                SelectPlusMius(out savepos.x, out savepos.y, i);
                checkList.Add(new Pos(currentpos.x + savepos.x, currentpos.y + savepos.y));
            }

            savepos.Reset();
            stoppos.Reset();
            currentpos.Reset();
        }

        if (currentpos.isNull())
        {
            if (isDebug)
                Debug.Log("fail CompulsTheRoomAtCreating()");
            return;
        }

        savepos = CheckTheObjectHaveRoomScript(roomObjects[_roomnum]).size;
        roomObjects[_roomnum].GetComponent<Room>().mapPos = new Pos(stoppos.x, stoppos.y);

        PassageData pass = new PassageData();
        pass.from = new Pos(currentpos.x, currentpos.y);        // from
        pass.to = new Pos(stoppos.x, stoppos.y);                // to
        passageInfo.Add(pass);  
        if (isDebug)
            Debug.Log("Last PassageData From" + pass.from.StringPos() + " To" + pass.to.StringPos());

        for (int x = 0; x < savepos.x; ++x)
            for (int y = 0; y < savepos.y; ++y)
                map[stoppos.x + x, stoppos.y + y] = _roomnum;

    }

    // ---------- 추가된 함수
    // 자신의 환경에 따라 환경시스템 변경
    void SettingMyEnviornment()
    {
        EnviornmentManager.Instance.selectEnviornment = myEnviornment;
        EnviornmentManager.Instance.ChangeEnviornment();
    }

    /// <summary>
    /// 플레이어가 시작하는 방의 위치를 보낸다.
    /// </summary>
    /// <returns></returns>
    public Vector3 GetStartRoomPosition()
    {
        return startRoomPosition;
    }

    //room에서 몬스터를 다 잡을 시에 메시지를 보내자.
    //가까이에 있는 복도와 방을 보여준다.
    void ShowNearPassageAndRoom()
    {
        for (int i = 0; i < transform.GetChildCount(); ++i)
        {
            if (passageInfo[i].to.x == MapUI.instance.playerPos.x && passageInfo[i].to.y == MapUI.instance.playerPos.y)
            {
                //복도에서 반대편 룸을 열어주자.
                CheckTheObjectHaveRoomScript(roomObjects[map[passageInfo[i].from.x, passageInfo[i].from.y]]).OpenRoom();
            }
            else if (passageInfo[i].from.x == MapUI.instance.playerPos.x && passageInfo[i].from.y == MapUI.instance.playerPos.y)
            {
                CheckTheObjectHaveRoomScript(roomObjects[map[passageInfo[i].to.x, passageInfo[i].to.y]]).OpenRoom();
            }
            else
                continue;

            transform.GetChild(i).GetComponent<Passage>().OpenPassage();
        }

    }

    void ShowFirstRoom()
    {
        Room first = CheckTheObjectHaveRoomScript(roomObjects[0]);
        if (first != null)
        {
            first.OpenRoom();
        }
    }
}

