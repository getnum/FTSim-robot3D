using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Threading;

public class Vertical : MonoBehaviour {

    public Transform arm;
    public Transform arm_switch_ref;
	public Transform arm_danger_up;
    public Transform arm_danger_down;
    public Transform dropDown_ref;
	public Transform dropDown_imp;
	float speed;
	float signals;
    float pulseTrigger = 0;
    int pos_vertical;
    Vector3 down;

    Communication com;

    bool referenceSwitch, danger_up, danger_down;
	GameObject dangerSign;


    void OnTriggerEnter(Collider other)
    {
		if(other.transform == arm_switch_ref)
        {
			referenceSwitch = true;
            //Debug.Log("vertical-Reached the switch");
        }
		if (other.transform == arm_danger_up)
		{
			danger_up = true;
			//Debug.Log("vertical-Danger up");
		}
		if (other.transform == arm_danger_down)
        {
			danger_down = true;
            //Debug.Log("vertical-Danger bottom");
        }
    }

    void OnTriggerExit(Collider other)
    {
		if(other.transform == arm_switch_ref)
		{
			referenceSwitch = false;
		}
		if (other.transform == arm_danger_up)
		{
			danger_up = false;
		}
		if (other.transform == arm_danger_down)
		{
			danger_down = false;
		}
    }


    // Use this for initialization
    void Start()
    {
        com = GameObject.Find("Communication").GetComponent<Communication>();
        if (!com.paramsRead) {
			com.ReadParams ();
		}
		speed = com.vertical_speed;
		signals = com.vertical_signals;
		referenceSwitch = false;
		danger_up = false; 
		danger_down = false;
		dangerSign = GameObject.FindGameObjectWithTag ("Danger_dvig");
		down = new Vector3(0, 0, speed);
    }

	void go_down(float dt)
    {
		if (!danger_down)
        {
            arm.Translate(dt * down);

            pulseTrigger -= dt * down.z;
     
            if (pulseTrigger < 0)
            {
                pulseTrigger = 2.7f / signals + pulseTrigger;
				if (dropDown_imp.GetComponent<Dropdown>().value == 0)
                	com.vertical_imp_a(true);
            }
        }
    }

	void go_up(float dt)
    {
		if (!danger_up)
        {
            arm.Translate(dt * -down);
            
            pulseTrigger += dt * down.z;
            
            if (pulseTrigger > 2.7f / signals)
            {
                pulseTrigger = pulseTrigger - 2.7f / signals;
				if (dropDown_imp.GetComponent<Dropdown>().value == 0)
					com.vertical_imp_a(true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
		if (danger_up || danger_down) {
			dangerSign.SetActive (true);
            com.alarms_active(true);
        }
		else{

			dangerSign.SetActive (false);
		}

        com.danger_up(danger_up);
        com.danger_down(danger_down);

        if (dropDown_ref.GetComponent<Dropdown>().value == 0)
            com.vertical_ref(false);
            //com.vertical_ref(referenceSwitch);
                else
                    com.vertical_ref(dropDown_ref.GetComponent<Dropdown>().value == 2);

		if (dropDown_imp.GetComponent<Dropdown>().value == 0)
			com.vertical_imp_a(false);
		else
			com.vertical_imp_a(dropDown_imp.GetComponent<Dropdown>().value == 2);

        if (com.vertical_run())
        {
            if (com.vertical_dir())
            {
				go_down(Time.deltaTime);
            }
            else
            {
				go_up(Time.deltaTime);
            }
            //print("Vertical position of the arm: " + arm.localPosition);
            //Thread.Sleep(2000);
        }
        pos_vertical = (int)(arm.position.y * 10) - 23;
        com.position_vertical(pos_vertical);
        //print("Vertical position of the arm: " + pos_vertical);
    }
}
