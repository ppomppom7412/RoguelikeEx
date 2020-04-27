using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum EnviornmentList { normal, fire, ice, fantacy };

public class EnviornmentManager : MonoBehaviour
{

    static public EnviornmentManager instance;
    static public EnviornmentManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<EnviornmentManager>();

                if (instance == null)
                {
                    GameObject manager = new GameObject();
                    manager.name = "EnviornmentManager";
                    instance = manager.AddComponent<EnviornmentManager>();
                }
            }

            return instance;
        }
    }

    public bool isDebug;

    public EnviornmentList selectEnviornment;

    public GameObject directionLight;
    public GameObject iceObject;
    public GameObject fireObject;
    public GameObject fantacyObject;

    private Enviornment [] myEnvios;
    private Enviornment currentEnviornment;
    private Enviornment previousEnviornment;

    void Awake() {
        myEnvios = new Enviornment[4];

        myEnvios[(int)EnviornmentList.normal] = new Enviornment();
        myEnvios[(int)EnviornmentList.fire] = new FireEnviornment();
        myEnvios[(int)EnviornmentList.ice] = new IceEnviornment();
        myEnvios[(int)EnviornmentList.fantacy] = new FantasyEnviornment();

        previousEnviornment = myEnvios[(int)EnviornmentList.normal];
        currentEnviornment = myEnvios[(int)EnviornmentList.normal];

    }
	
	void Update () {
		
	}

    public void ChangeEnviornment()
    {
        if (currentEnviornment == null)
        {
            if (isDebug) Debug.Log("currentEnviornment is null. It's can do push when starting engine");
            return;
        }

        currentEnviornment.ResetBuff();
        previousEnviornment = currentEnviornment;
        currentEnviornment = myEnvios[(int)selectEnviornment];

        //previousEnviornment.Unactive(); 비활성화 함수
        currentEnviornment.AddBuff(previousEnviornment.m_name);
        currentEnviornment.Active();

        if (isDebug) Debug.Log("Change "+previousEnviornment.m_name + " to " + currentEnviornment.m_name);
    }


    public void ActiveIceEnviornment()
    {
        if (isDebug) Debug.Log("Active Ice Enviornment");

        directionLight.GetComponent<Light>().color = new Color(0, 0.176f, 0.839f);
        iceObject.SetActive(true);

        //비활성화 함수 실행시 삭제
        fireObject.SetActive(false);    
        fantacyObject.SetActive(false);
    }

    public void ActiveFireEnviornment()
    {
        if (isDebug) Debug.Log("Active Fire Enviornment");

        directionLight.GetComponent<Light>().color = new Color(0.627f, 0.007f, 0.278f);
        fireObject.SetActive(true);

        //비활성화 함수 실행시 삭제
        iceObject.SetActive(false);
        fantacyObject.SetActive(false);
    }

    public void ActiveFantacyEnviornment()
    {
        if (isDebug) Debug.Log("Active Fantacy Enviornment");

        directionLight.GetComponent<Light>().color = new Color(1.0f, 0.564f, 0.207f);
        fantacyObject.SetActive(true);

        //비활성화 함수 실행시 삭제
        iceObject.SetActive(false);
        fireObject.SetActive(false);
    }

    public void ActiveNormalEnviornment()
    {
        if (isDebug) Debug.Log("Active Normal Enviornment");

        directionLight.GetComponent<Light>().color = new Color(1f, 0.956f, 0.839f);

        //비활성화 함수 실행시 삭제
        fantacyObject.SetActive(false);
        iceObject.SetActive(false);
        fireObject.SetActive(false);
    }
}
