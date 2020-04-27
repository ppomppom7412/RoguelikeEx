using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExtraMapdata {
    static int count = -1;
    public Pos m_pos;
    public Sprite m_image; 
    public string m_name;
    private bool isInstanced;
    private RectTransform recttr;

    public ExtraMapdata(Sprite _image, string _name, Pos _pos)
    {
        isInstanced = false;
        m_image = _image;
        m_name = _name;
        m_pos = _pos;

        count++;

        if (count > 9999) count = 0;

        GameObject samenameOb = GameObject.Find(_name);
        if (samenameOb != null)
        {
            m_name = "ExtraMapdata(" + count + ")"+m_name;
        }
    }

    /// <summary>
    /// 해당 데이터를 게임오브젝트에 등록한다.
    /// </summary>
    /// <param name="_ob"></param>
    public void RegisterObject(GameObject _ob)
    {
        Image obImage = _ob.GetComponent<Image>();
        if (obImage == null)
        {
            Debug.Log("not seccese full at "+_ob.name+ " add Image sprite");
            return;
        }

        _ob.name = m_name;
        obImage.sprite = m_image;
        recttr = _ob.GetComponent<RectTransform>();

        isInstanced = true;
    }

    public void MoveToDataObject(Vector3 _newvec)
    {
        if (recttr == null) return;

        recttr.localPosition = _newvec;
    }

    public void Destroy()
    {
        if (isInstanced)
            GameObject.Destroy(GameObject.Find(m_name));
    }
}
