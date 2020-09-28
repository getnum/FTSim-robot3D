using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

public class Panel : MonoBehaviour
{

    public Transform key;
    public Transform button1;
    public Transform button2;
    public Transform button3;
    public Transform button4;
    public Transform green;
    public Transform red_left;
    public Transform red_right;
    public Transform yellow_right;
    public Transform yellow_left;
	public Transform prefab;
	public Transform spawnPoint;

    Communication com;

    public bool puck = false;
    public bool black_l_u = false;
    public bool black_r_u = false;
    public int motor = 0;

    public Hand hand;
    public Table table;
    public Horizontal horizontal;
    public Vertical vertical;

    // Use this for initialization
    void Start()
    {
        com = GameObject.Find("Communication").GetComponent<Communication>();
    }

    public void reset()
    {
		Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
    }

    public void newPart()
    {
        // Get position of Odlagalisce
        Vector3 pos = spawnPoint.position;
        prefab.tag = "Player";
        Instantiate(prefab, new Vector3(pos.x, 8f, pos.z), Quaternion.identity);
        prefab.tag = "Untagged";
        print("The button add new puck was pressed");
    }
	public void removePart()
	{
		GameObject [] ply = GameObject.FindGameObjectsWithTag("Player");
		if (ply != null && ply.Length > 0) {
			GameObject.Destroy(ply[0]);
		}
	}

    // Update is called once per frame
    void Update()
    {
        //update lights
        red_left.GetComponent<Button>().interactable = com.red_left();
        red_right.GetComponent<Button>().interactable = com.red_right();
        yellow_left.GetComponent<Button>().interactable = com.yellow_left();
        yellow_right.GetComponent<Button>().interactable = com.yellow_right();

        com.black_l_u(button1.GetComponent<Toggle>().isOn);
        com.black_r_u(button2.GetComponent<Toggle>().isOn);
        com.black_r_d(button3.GetComponent<Toggle>().isOn);
        com.black_l_d(button4.GetComponent<Toggle>().isOn);
        com.green(green.GetComponent<Toggle>().isOn);
        com.key(key.GetComponent<Toggle>().isOn);

        com.mode = key.GetComponent<Toggle>().isOn;

        if (com.new_puck_command() && puck == false)
        {
            puck = true;
            newPart();
            removePart();
        }
    }
}