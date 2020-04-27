using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PassageData {

    public Pos from;
    public Pos to;

    public PassageData()
    {

    }

    public bool CheckHorizon()
    {
        if (from.x == to.x && from.y != to.y) return true;
        return false;

    }
    public bool CheckVertical()
    {
        if (from.y == to.y && from.x != to.x) return true;
        return false;
    }

    public float SenterX(float _interval)
    {
        //Debug.Log("SenterX : "+ (from.x + to.x) *0.5f);
        return (from.x + to.x) *0.5f * _interval;
    }
    public float SenterY(float _interval)
    {
        //Debug.Log("SenterY : " + (from.y + to.y) * 0.5f);
        return (from.y + to.y) * 0.5f * _interval;
    }
}
