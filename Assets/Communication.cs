using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.IO;
using EasyModbus;

public class Communication : MonoBehaviour
{
    //Ip-Address and Port of OpenPLC are currently hard-coded
    //In the future user should be able to change it externally
    public static ModbusClient modbusClient; 
    
    public bool[] readCoils;
    public static int port;
    public static string ip;

    public bool mode = true;

    //public bool puck;
    public bool val;

    //Connection with OpenPLC
    private static bool connectionEstablished = false;

    public string file_params = "config.txt";
    public float table_speed = 5f;
    public float table_signals = 200f;
    public float horizontal_speed = 0.2f;
    public float horizontal_signals = 120f;
    public float vertical_speed = 0.42f;
    public float vertical_signals = 105f;
    public float hand_speed = 0.25f;
    public float hand_signals = 22f;
    public bool paramsRead;

    public void ReadParams()
    {
        string line;
        if (File.Exists(file_params))
        {
            //Read the text from directly from the test.txt file
            StreamReader reader = new StreamReader(file_params);

            while ((line = reader.ReadLine()) != null)
            {
                string[] lineparts = line.Split(':');
                if (lineparts[0] == "table_speed")
                {
                    table_speed = System.Convert.ToSingle(lineparts[1]);
                }
                else if (lineparts[0] == "table_num_impulses")
                {
                    table_signals = System.Convert.ToSingle(lineparts[1]);
                }
                else if (lineparts[0] == "lift_speed")
                {
                    vertical_speed = System.Convert.ToSingle(lineparts[1]);
                }
                else if (lineparts[0] == "lift_num_impulses")
                {
                    vertical_signals = System.Convert.ToSingle(lineparts[1]);
                }
                else if (lineparts[0] == "extend_speed")
                {
                    horizontal_speed = System.Convert.ToSingle(lineparts[1]);
                }
                else if (lineparts[0] == "extend_num_impulses")
                {
                    horizontal_signals = System.Convert.ToSingle(lineparts[1]);
                }
                else if (lineparts[0] == "hand_speed")
                {
                    hand_speed = System.Convert.ToSingle(lineparts[1]);
                }
                else if (lineparts[0] == "hand_num_impulses")
                {
                    hand_signals = System.Convert.ToSingle(lineparts[1]);
                }
            }
            reader.Close();
            paramsRead = true;
        }
        else
        {
            Debug.Log("File with params not found!");
        }
    }

    // Reads data from OpenPLC
    public bool read(int index)
    {
        if (connectionEstablished == true)
        {
            readCoils = modbusClient.ReadDiscreteInputs(0, 22); //Read 15 Coils from Server, starting with address 0

            return readCoils[index];
            //for (int i = 0; i < readCoils.Length; i++)
            //    print("Value of Coil " + i + " " + readCoils[i].ToString());
        }
        return false;
    }

    public void write(int register, bool val)
    {
        if (connectionEstablished == true)
        {
            if (!mode || register > 13 || register == 8)
            {
                modbusClient.WriteSingleCoil(register, val);
            }
        }
    }

    public void write_number(int register, int val)
    {
        if (connectionEstablished == true)
        {
            //print(register + " " + val);
            modbusClient.WriteSingleRegister(register, val);
        }
    }

    //outputs
    public void table_ref(bool val) { write(0, val); }
    public void table_imp_a(bool val) { write(1, val); }
    public void arm_ref(bool val) { write(2, val); }
    public void arm_imp(bool val) { write(3, val); }
    public void vertical_ref(bool val) { write(4, val); }
    public void vertical_imp_a(bool val) { write(5, val); }
    public void hand_ref(bool val) { write(6, val); }
    public void hand_imp(bool val) { write(7, val); }

    //buttons
    public void key(bool val) { write(8, val); }
    public void green(bool val) { write(9, val); }
    public void black_l_u(bool val) { write(10, val); }
    public void black_l_d(bool val) { write(11, val); }
    public void black_r_u(bool val) { write(12, val); }
    public void black_r_d(bool val) { write(13, val); }

    //puck
    public void new_puck(bool val) { write(14, val); }
    public void puck_grabbed(bool val) { write(15, val); }

    //danger
    public void danger_forward(bool val) { write(16, val); }
    public void danger_backward(bool val) { write(17, val); }
    public void danger_up(bool val) { write(18, val); }
    public void danger_down(bool val) { write(19, val); }
    public void danger_open(bool val) { write(20, val); }
    public void danger_close(bool val) { write(21, val); }
    public void danger_ccw(bool val) { write(22, val); }
    public void danger_cw(bool val) { write(23, val); }

    public void alarms_active(bool val) { write(24, val); }

    public void position_vertical(int val) { write_number(0, val); }
    public void position_horizontal(int val) { write_number(1, val); }
    public void position_sides(int val) { write_number(2, val); }
    public void position_hand(int val) { write_number(3, val); }


    //inputs
    public bool table_dir() { return read(0); }
    public bool table_run() { return read(1); }
    public bool arm_dir() { return read(2); }
    public bool arm_run() { return read(3); }
    public bool vertical_dir() { return read(4); }
    public bool vertical_run() { return read(5); }
    public bool hand_dir() { return read(6); }
    public bool hand_run() { return read(7); }

    //lights
    public bool red_left() { return read(8); }
    public bool yellow_left() { return read(9); }
    public bool red_right() { return read(10); }
    public bool yellow_right() { return read(11); }
    public bool new_puck_command() { return read(14); }
    

    // Method implements connection to OpenPLC
    public static void Connect()
    {
        try
        {
            port = Int32.Parse(openplcConnection.port);
            ip = openplcConnection.ip;
        }
        catch (Exception e)
        {
            print(e.Message);
        }

        print("Connecting to OpenPLC ...\n");

        modbusClient = new ModbusClient(ip, port);

        try
        {
            modbusClient.Connect(); //Connect TO ModbusClient on IP address and Port
            print("Successfully connected to IP " + ip + " on port " + port.ToString());
        }
        catch (Exception e)
        {
            print(e.Message);
        }
    }

    public static void Disconnect()
    {
        if (connectionEstablished == true)
        {
            modbusClient = new ModbusClient(ip, port);
            try
            {
                modbusClient.Disconnect();
                print("Successfully disconnected from IP " + ip + " on port " + port.ToString());
            }
            catch (Exception e)
            {
                print(e.Message);
            }

        }
    }
    void Start()
    {
        paramsRead = false;
        Application.runInBackground = true;
        ReadParams();
    }

    private void Update()
    {
        if (openplcConnection.openplc_connect == true)
        {
            try
            {
                Connect();
                connectionEstablished = true;
            }
            catch (COMException e)
            {
                print(e.ToString());
                connectionEstablished = false;
            }
        }
        else {
            try
            {
                Disconnect();
                connectionEstablished = false;
            }
            catch (COMException e)
            {
                print(e.ToString());
                connectionEstablished = false;
            }
        }
    }

}