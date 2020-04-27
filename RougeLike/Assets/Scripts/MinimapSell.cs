using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapSell : MonoBehaviour {
    private float insepttime = 0.3f;
    private Image myimage;
    private float counttime;
    void Start()
    {
        myimage = gameObject.GetComponent<Image>();
    }

    public void Off_AhpaValue()
    {
        if (!CheckMyImage())
        {
            myimage.color = new Color(myimage.color.r, myimage.color.g, myimage.color.b, 0f);
        }
    }

    public void UnActive_Sell(float _time = 1.0f)
    {
        if (!CheckMyImage()) return;
        if (counttime < _time ) return;

        counttime = _time;

        StopCoroutine(DieAway_Sell());
        StartCoroutine(DieAway_Sell());
    }

    public void Active_Sell()
    {
        if (!CheckMyImage()) return;

        counttime = 100.0f;

        myimage.color = new Color(myimage.color.r, myimage.color.g, myimage.color.b, 1.0f);
    }

    IEnumerator DieAway_Sell()
    {
        while (counttime > 0 && counttime <100f)
        {
            myimage.color = new Color(myimage.color.r, myimage.color.g, myimage.color.b, myimage.color.a -0.1f * insepttime);

            counttime -= 0.1f * insepttime;

            yield return new WaitForSeconds(0.1f);
        }
    }

    bool CheckMyImage()
    {
        if (!myimage)
        {
            myimage = gameObject.GetComponent<Image>();
            if (!myimage)
            {
                Debug.Log("MiniMapSell not found myImage with Image");
                return false;
            }
        }

        //나의 이미지를 찾으면 참
        return true;
    }
}
