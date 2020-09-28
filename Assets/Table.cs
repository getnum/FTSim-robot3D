using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Timers;
using System.Threading;

public class Table : MonoBehaviour {

    public Transform table;
    public Transform table_switch_ref;
	public Transform table_danger_CCW;
	public Transform table_danger_CW;
    public Transform dropDown_ref;
	public Transform dropDown_imp;
	float speed;
	float signals;
	int pos_sides;

	Vector3 rotateCW;

    float pulseTrigger = 0;
    Communication com;
	bool referenceSwitch, danger_ccw, danger_cw;
	GameObject dangerSign;  

	public void SetSpeed(int s){
		speed = s;
	}
	public void SetSignals(int s){
		signals = s;
	}




    void OnTriggerEnter(Collider other)

    {
		if (other.transform == table_switch_ref)
		{
			referenceSwitch = true;
			//Debug.Log("Table reference reached.");
		}
		if (other.transform == table_danger_CCW)
        {
			danger_ccw = true;
            //Debug.Log("Reached the CCW limit");
        }
		if (other.transform == table_danger_CW)
        {
			danger_cw = true;
            //Debug.Log("Reached the CW limit");
        }
    }

    void OnTriggerExit(Collider other)
    {
		if (other.transform == table_switch_ref)
		{
			referenceSwitch = false;
		}
		if (other.transform == table_danger_CCW)
		{
			danger_ccw = false;
		}
		if (other.transform == table_danger_CW)
		{
			danger_cw = false;
		}
    }  

    // Use this for initialization
    void Start()
    {  
		com = GameObject.Find("Communication").GetComponent<Communication>();
		if (!com.paramsRead) {
			com.ReadParams ();
		}
		speed = com.table_speed;
		signals = com.table_signals;

		rotateCW = new Vector3(0, speed, 0);
        
		referenceSwitch = false;
		danger_ccw = false; 
		danger_cw = false;
		dangerSign = GameObject.FindGameObjectWithTag ("Danger_miza");
		dangerSign.SetActive (false);
    }

	void rotate_CW(float dt)
    {
		if (!danger_cw) {
			table.Rotate (dt * rotateCW);

			pulseTrigger -= dt * rotateCW.y;
			if (pulseTrigger < 0) {                
				pulseTrigger = 300f / signals + pulseTrigger;
				if (dropDown_imp.GetComponent<Dropdown>().value == 0)
					com.table_imp_a (true);
			}
		} 
    }

	void rotate_CCW(float dt)
    {
		if (!danger_ccw)
        {
            table.Rotate(dt * - rotateCW);

            pulseTrigger += dt * rotateCW.y;
            if (pulseTrigger > 300f / signals)
            {
                pulseTrigger = pulseTrigger - 300f / signals ;
				if (dropDown_imp.GetComponent<Dropdown>().value == 0)
					com.table_imp_a(true);
            }
        }
    }



    // Update is called once per frame
    void Update()
    {
		if (danger_cw || danger_ccw) {
			dangerSign.SetActive (true);
			com.alarms_active(true);
		}
		else{
			dangerSign.SetActive (false);	
		}

		com.danger_ccw(danger_ccw);
		com.danger_cw(danger_cw);

		if (dropDown_ref.GetComponent<Dropdown>().value == 0)
			com.table_ref(false);
			//com.table_ref(referenceSwitch);
		else
			com.table_ref(dropDown_ref.GetComponent<Dropdown>().value == 2);

		if (dropDown_imp.GetComponent<Dropdown>().value == 0)
			com.table_imp_a(false);
		else
			com.table_imp_a(dropDown_imp.GetComponent<Dropdown>().value == 2);


		if (com.table_run())
		{
			if (com.table_dir())
			{
				rotate_CCW(Time.deltaTime);
			}
			else
			{
				rotate_CW(Time.deltaTime);
			}
			//print("Sides position of the table: " + table.localRotation.eulerAngles.y);
			//Thread.Sleep(2000);
		}
		pos_sides = (int)table.localRotation.eulerAngles.y;
		com.position_sides(pos_sides);
	}

}
