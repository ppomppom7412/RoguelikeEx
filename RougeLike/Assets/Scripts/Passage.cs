using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Passage : MonoBehaviour {

    public PassageData data;

    public void Start()
    {
    }

    public void SetPassageData(PassageData _data)
    {
        if (_data == null) return;

        data = _data;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 해당 방의 가까운 복도인지 확인한다.
    /// </summary>
    /// <param name="_room">해당할 방 위치</param>
    /// <returns>가까우면 true</returns>
    public bool FindNearRoomForPassage(Pos _room)
    {
        if (data.from.x == _room.x)
            if (data.from.y == _room.y)
                return true;

        if (data.to.x == _room.x)
            if (data.to.y == _room.y)
                return true;

        return false;
    }

    public void OpenPassage()
    {
        if (data.from.isNull()) return;
        if (data.to.isNull()) return;

        AddUIPassgae();
        gameObject.SetActive(true);
    }

    public void AddUIPassgae()
    {
        //MapUI에게 복도정보를 보낸다.
        if (MapUI.instance != null)
        {
            MapUI.instance.AddPassage(data, "[" + data.from.StringPos() + " >> " + data.to.StringPos() + "]");
        }

    }
}
