using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Timers;
using System.Threading;

public class Horizontal : MonoBehaviour {

    public Transform arm;

	public Transform arm_switch_ref;
	public Transform arm_danger_forward;
	public Transform arm_danger_backward;
    public Transform dropDown_ref;
    public Transform dropDown_imp;

	float speed;
	float signals;
    float pulseTrigger = 0;
    
    int pos_horizontal;

    //2.67 dist: 2.67/77 per pulse
    Vector3 forward_vec;
    Communication com;
    Panel pan;

	bool referenceSwitch, danger_forward, danger_backward;
	GameObject dangerSign;


    void OnTriggerEnter(Collider other)
    {
		if(other.transform == arm_switch_ref)
		{
			referenceSwitch = true;
			//Debug.Log("horizont-Reached the switch");
		}
		if (other.transform == arm_danger_forward)
		{
			danger_forward = true;
			//Debug.Log("horizont-Danger forward");
		}
		if (other.transform == arm_danger_backward)
		{
			danger_backward = true;
			//Debug.Log("horizont-Danger backward");
		}
    }

    void OnTriggerExit(Collider other)
    {
		if(other.transform == arm_switch_ref)
		{
			referenceSwitch = false;
		}
		if (other.transform == arm_danger_forward)
		{
			danger_forward = false;
		}
		if (other.transform == arm_danger_backward)
		{
			danger_backward = false;
		}
    }

    // Use this for initialization
    void Start()
    {
        com = GameObject.Find("Communication").GetComponent<Communication>();
        pan = GameObject.Find("Panel").GetComponent<Panel>();
		if (!com.paramsRead) {
			com.ReadParams ();
		}
		speed = com.horizontal_speed;
		signals = com.horizontal_signals;
		danger_forward = false; 
		danger_backward = false;
		dangerSign = GameObject.FindGameObjectWithTag ("Danger_izteg");
		forward_vec = new Vector3(0, -speed, 0);
    }

    void reverse(float dt)
    {
		if (!danger_backward)
        {
            arm.Translate(dt * -forward_vec);
           // Debug.Log("arm reversing");

            pulseTrigger -= dt * forward_vec.y;
            if (pulseTrigger > 2.5f / signals)
            {
                pulseTrigger = pulseTrigger - 2.5f / signals;
                if (dropDown_imp.GetComponent<Dropdown>().value == 0)
                    com.arm_imp(true);
                //Debug.Log("triggering close");
            }
        }
    }

    void forward(float dt)
    {
		if (!danger_forward)
        { 
            arm.Translate(dt * forward_vec);
            //Debug.Log("arm forwarding");

            pulseTrigger += dt * forward_vec.y;
            if (pulseTrigger < 0)
            {
                pulseTrigger = 2.5f / signals + pulseTrigger;
                if (dropDown_imp.GetComponent<Dropdown>().value == 0)
                    com.arm_imp(true);
                //Debug.Log("triggering close");
            }
        }
    }



    // Update is called once per frame
    void Update()
    {
		if (danger_forward || danger_backward) {
			dangerSign.SetActive (true);
            com.alarms_active(true);
        }
		else{
			dangerSign.SetActive (false);	
		}

        com.danger_forward(danger_forward);
        com.danger_backward(danger_backward);

        if (dropDown_ref.GetComponent<Dropdown>().value == 0)
            com.arm_ref(false);
            //com.arm_ref(referenceSwitch);
        else
            com.arm_ref(dropDown_ref.GetComponent<Dropdown>().value == 2);

		if (dropDown_imp.GetComponent<Dropdown>().value == 0)
			com.arm_imp(false);
		else
			com.arm_imp(dropDown_imp.GetComponent<Dropdown>().value == 2);


		if (com.arm_run())
        {
            if (com.arm_dir())
            {
                reverse(Time.deltaTime);
            }
            else
            {
                forward(Time.deltaTime);
            }
            //print("Horizontal position of the arm: " + arm.localPosition);
            //Thread.Sleep(2000);
        }
        pos_horizontal = (int)(arm.localPosition.y * 10) + 7;
        com.position_horizontal(pos_horizontal);
    }

}
