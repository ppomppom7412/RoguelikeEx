using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pos
{
    public int x;
    public int y;

    public Pos(int _x = -1, int _y=-1)
    {
        x = _x;
        y = _y;
    }

    public Pos(Pos _pos)
    {
        x = _pos.x;
        y = _pos.y;
    }

    /// <summary>
    /// x,y 값 모두 -1(null)으로 리셋
    /// </summary>
    public void Reset()
    {
        x = -1;
        y = -1;
    }

    /// <summary>
    /// Pos의 데이터가 -1(null)인지 확인한다.
    /// </summary>
    /// <returns>null이면 true </returns>
    public bool isNull()
    {
        if (x == -1 && y == -1) return true;
        if (this == null) return true;

        return false;
    }

    /// <summary>
    /// 현재 좌표를 string으로 보내준다.
    /// </summary>
    /// <returns></returns>
    public string StringPos()
    {
        return "[" + x + "," + y + "]";
    }

}
