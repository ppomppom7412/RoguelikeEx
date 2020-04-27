using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://unity3d.com/kr/learn/tutorials/topics/interface-essentials/adding-buttons-custom-inspector

[SerializeField]
public class Enviornment
{
    public string m_name;
    public int showcount;
    public int overlapcount;

    public Enviornment()
    {
        m_name = "normal";
    }

    public void ResetBuff()
    {
        showcount = 0;
    }

    virtual public void AddBuff(string _previname)
    {
        if (_previname == m_name)
        {
            overlapcount++;
        }
        else
        {

        }

        showcount = 1;

        Debug.Log("Buff:"+showcount+", overlap:"+overlapcount);
       
    }

    virtual public void Active()
    {
        EnviornmentManager.instance.ActiveNormalEnviornment();
    }
}

[SerializeField]
public class FireEnviornment: Enviornment
{

    public FireEnviornment()
    {
        m_name = "fire";
    }

    override public void AddBuff(string _previname)
    {
        if (_previname == m_name)
        {
            overlapcount++;
        }
        else
        {
            overlapcount = 0;
        }

        showcount = 10;

        Debug.Log("Buff:" + showcount + ", overlap:" + overlapcount);
    }

    override public void Active()
    {
        EnviornmentManager.instance.ActiveFireEnviornment();
    }

}

[SerializeField]
public class FantasyEnviornment : Enviornment
{
    public FantasyEnviornment()
    {
        m_name = "fantacy";
    }

    override public void AddBuff(string _previname)
    {
        if (_previname == m_name)
        {
            overlapcount++;
        }
        else
        {
            overlapcount = 0;
        }

        showcount = 20;

        Debug.Log("Buff:" + showcount + ", overlap:" + overlapcount);
    }

    override public void Active()
    {
        EnviornmentManager.instance.ActiveFantacyEnviornment();
    }
}


[SerializeField]
public class IceEnviornment : Enviornment
{
    public IceEnviornment()
    {
        m_name = "ice";
    }

    override public void AddBuff(string _previname)
    {
        if (_previname == m_name)
        {
            overlapcount++;
        }
        else
        {
            overlapcount = 0;
        }

        showcount = 30;

        Debug.Log("Buff:" + showcount + ", overlap:" + overlapcount);
    }

    override public void Active()
    {
        EnviornmentManager.instance.ActiveIceEnviornment();
    }
}