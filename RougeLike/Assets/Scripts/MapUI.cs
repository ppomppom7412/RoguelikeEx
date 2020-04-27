using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapUI : MonoBehaviour {
    static public MapUI instance;
    [SerializeField]
    private Pos playerpos;
    public Pos playerPos
    {
        private set { playerpos.x = value.x;
            playerpos.y = value.y;
        }
        get { return playerpos; }
    }
    private List<ExtraMapdata> registerdatas;

    public GameObject uiObject;
    public GameObject uiPlayer;

    private Pos mapsize;
    private Image myimage;
    private float interSize = 40;
    private bool isUpdate = false;


    //public Sprite roomsprite;

    public void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        myimage = gameObject.GetComponent<Image>();
        registerdatas = new List<ExtraMapdata>();

        playerpos = new Pos();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            BlinkPanel();
        }
        if (isUpdate)
        {
            isUpdate = !isUpdate;

            BlinkPanel();
        }

        //디버깅용 입력
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
                SetPlayerPostion(playerpos.x, playerpos.y - 1);
            if (Input.GetKeyDown(KeyCode.UpArrow))
                SetPlayerPostion(playerpos.x, playerpos.y + 1);
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                SetPlayerPostion(playerpos.x - 1, playerpos.y);
            if (Input.GetKeyDown(KeyCode.RightArrow))
                SetPlayerPostion(playerpos.x + 1, playerpos.y);
        }
    }

    void UpdatePlayerPos()
    {
        ExtraMapdata playerdata = registerdatas.Find((item) => item.m_name == "Player_Sell");
        if (playerdata == null)
        {
            //새롭게 생성.
            //GameObject ob = Instantiate(uiPlayer, transform);
            uiPlayer.SetActive(true);
            RectTransform rectTr = uiPlayer.GetComponent<RectTransform>();

            if (rectTr == null)
            {
                Debug.Log("생성된 오브젝트의 RectTransform이 존재하지 않습니다.");
                //return;

                rectTr = uiPlayer.AddComponent<RectTransform>();
            }
            uiPlayer.AddComponent<MinimapSell>();

            playerdata = new ExtraMapdata(uiPlayer.GetComponent<Image>().sprite, "Player_Sell", playerpos);
            playerdata.RegisterObject(uiPlayer);
            registerdatas.Add(playerdata);
        }

        // 위치 조정
        Pos senter = new Pos(mapsize.x / 2, mapsize.y / 2);
        float x = (playerpos.x - senter.x) * (interSize * 2);
        float y = (playerpos.y - senter.y) * (interSize * 2);

        playerdata.MoveToDataObject(new Vector3(x, y, 0)); 
    }

    /// <summary>
    /// 새로운 정보를 받아 맞는 오브젝트를 생성한다.
    /// </summary>
    /// <param name="_mappos"></param>
    /// <param name="_image"></param>
    /// <param name="_name"></param>
    public void AddExtradata(Pos _mappos, Sprite _image , string _name)
    {
        GameObject ob = Instantiate(uiObject, transform);
        RectTransform rectTr = ob.GetComponent<RectTransform>();
        Pos senter = new Pos(mapsize.x / 2, mapsize.y / 2);
        ExtraMapdata extra = new ExtraMapdata(_image, _name, _mappos);
        float x, y;

        x = (_mappos.x - senter.x) * (interSize * 2);
        y = (_mappos.y - senter.y) * (interSize * 2);
        if (rectTr == null)
        {
            Debug.Log("생성된 오브젝트의 RectTransform이 존재하지 않습니다.");
            //return;

            rectTr = ob.AddComponent<RectTransform>();
        }

        rectTr.localPosition = new Vector3(x, y, 0);
        ob.AddComponent<MinimapSell>();

        registerdatas.Add(extra);
        extra.RegisterObject(ob);
    }

    /// <summary>
    /// 새로운 맵 크기를 UI에 등록한다.
    /// </summary>
    /// <param name="_mapsize"></param>
    public void SetMapSize(Pos _mapsize)
    {
        mapsize = new Pos(_mapsize.x, _mapsize.y);

        Clear_map();
    }

    /// <summary>
    /// UI상에 복도를 추가적으로 배치한다.
    /// </summary>
    /// <param name="_pass">복도 정보</param>
    /// <param name="_name">복도 이름</param>
    public void AddPassage(PassageData _pass, string _name = "passage")
    {
        // 시작과 끝을 기준으로 방향을 확인함
        // 시작과 끝, 간격을 기준으로 생성
        GameObject ob = Instantiate(uiObject, transform);
        RectTransform rectTr = ob.GetComponent<RectTransform>();
        Pos senter = new Pos(mapsize.x / 2, mapsize.y / 2);
        float x, y;

        if (rectTr == null)
        {
            Debug.Log("생성된 오브젝트의 RectTransform이 존재하지 않습니다.");
            return;

            //rectTr = ob.AddComponent<RectTransform>();
        }

        x = (_pass.from.x + _pass.to.x)*0.5f;
        y = (_pass.from.y + _pass.to.y)*0.5f;

        if (_pass.from.x != _pass.to.x)
        {
            rectTr.sizeDelta = (new Vector2(interSize , interSize/2));
        }
        else if (_pass.from.y != _pass.to.y)
        {
            rectTr.sizeDelta = (new Vector2(interSize/2, interSize ));
        }
        else
        {
            Destroy(ob);
            return;
        }

        x = (x - senter.x) * (interSize * 2 );
        y = (y - senter.y ) * (interSize * 2 );

        rectTr.localPosition = new Vector3(x , y , 0);
        ob.AddComponent<MinimapSell>();
        ob.name = _name;

        isUpdate = true;
    }

    /// <summary>
    /// 방 사이즈 및 정보를 UI에 등록시킨다.
    /// </summary>
    /// <param name="_mappos">지도상 위치</param>
    /// <param name="_roomsize">방의 크기</param>
    /// <param name="_name">방 이름</param>
    public void SetRoomdata(Pos _mappos, Pos _roomsize, string _name = "Room")
    {
        // 룸 사이즈에 따른 생성 필요.
        // 왼쪽 위를 기준으로 방이 생성됨.

        GameObject ob = Instantiate(uiObject, transform);
        RectTransform rectTr = ob.GetComponent<RectTransform>();
        Pos senter = new Pos(mapsize.x / 2, mapsize.y / 2);
        float x, y;

        x = (_mappos.x - senter.x ) * (interSize * 2 );
        y = (_mappos.y - senter.y) * (interSize * 2 );
        x += (_roomsize.x - 1) * (interSize );
        y += (_roomsize.y - 1) * (interSize );
        if (rectTr == null)
        {
            Debug.Log("생성된 오브젝트의 RectTransform이 존재하지 않습니다.");
            //return;

            rectTr = ob.AddComponent<RectTransform>();
        }

        rectTr.sizeDelta = (new Vector2(interSize * (_roomsize.x + (_roomsize.x -1)), interSize * (_roomsize.y + (_roomsize.y - 1))));
        rectTr.localPosition =  new Vector3( x , y , 0);
        ob.AddComponent<MinimapSell>();
        ob.name = _name;

        isUpdate = true;
    }

    /// <summary>
    /// 미니맵을 5초간 노출 시킨다. 그후 일정 시간내에 꺼준다.
    /// </summary>
    public void BlinkPanel()
    {
        StopCoroutine(CallMinimap(5f));
        StartCoroutine(CallMinimap(5f));
    }

    IEnumerator CallMinimap(float _nexttime = 1f)
    {
        Transform childTr;
        uiPlayer.GetComponent<MinimapSell>().Active_Sell();

        for (int child = 0; child < transform.GetChildCount(); ++child)
        {
            childTr = transform.GetChild(child);

            if (childTr.GetComponent<MinimapSell>())
                childTr.GetComponent<MinimapSell>().Active_Sell();
        }

        yield return new WaitForSeconds(_nexttime);

        DieAway_Panel();
    }

    void DieAway_Panel()
    {
        Transform childTr;
        uiPlayer.GetComponent<MinimapSell>().UnActive_Sell(3f);

        for (int child = 0; child < transform.GetChildCount(); ++child)
        {
            childTr = transform.GetChild(child);

            if (childTr.GetComponent<MinimapSell>())
                childTr.GetComponent<MinimapSell>().UnActive_Sell(3f);
        }
    }

    void Clear_map()
    {
        for (int count = 0; count < transform.GetChildCount(); count++)
        {
            Destroy(transform.GetChild(count).gameObject);
        }
    }

    public void SetPlayerPostion(Pos _newpos)
    {
        playerpos.x = _newpos.x;
        playerpos.y = _newpos.y;
        UpdatePlayerPos();
    }

    public void SetPlayerPostion(int _x, int _y)
    {
        playerpos.x = _x;
        playerpos.y = _y;
        UpdatePlayerPos();
    }
}