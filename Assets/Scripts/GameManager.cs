using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	float MaxTime = 15;
	float time;
	float meteorCounter = 0;
	public GUIText CountDownTimer;
	public GUIText victoryText;
	public GUIText[] playerVictories;
	public Transform prefabMeteor;
	public GUITexture suddenDeathTexture;
	public AudioSource alarm;
	Transform[] meteors = null;
	public Transform[] prefabs;
	Transform[] ships;
	// Use this for initialization
	void Start () {

		ships = new Transform[2];
		if (PlayerPrefs.HasKey ("ship1"))
		{
			ships[0] = Instantiate (prefabs[PlayerPrefs.GetInt("ship1")], new Vector3(-3, 0, 0.5f), Quaternion.LookRotation(Vector3.right)) as Transform;
		}
		else
		{
			ships[0] = Instantiate (prefabs[0], new Vector3(-3, 0, 0.5f), Quaternion.LookRotation(Vector3.right)) as Transform;
		}
		if (PlayerPrefs.HasKey ("ship2"))
		{
			ships[1] = Instantiate (prefabs[PlayerPrefs.GetInt("ship2")], new Vector3(3, 0, 0.5f), Quaternion.LookRotation(Vector3.left)) as Transform;
		}
		else
		{
			ships[1] = Instantiate (prefabs[0], new Vector3(3, 0, 0.5f), Quaternion.LookRotation(Vector3.left)) as Transform;
		}

		if (PlayerPrefs.HasKey("suddenDeathTime"))
		{
			MaxTime = PlayerPrefs.GetInt("suddenDeathTime");
		}

		if (!PlayerPrefs.HasKey ("player1Wins"))
		{
			PlayerPrefs.SetInt ("player1Wins", 0);
			PlayerPrefs.SetInt ("player2Wins", 0);

		}
		else
		{
			playerVictories[0].text = "";
			playerVictories[1].text = "";
			for(int i = 0; i < PlayerPrefs.GetInt ("player1Wins"); ++i)
			{
				playerVictories[0].text += "o ";
			}
			for(int i = 0; i < PlayerPrefs.GetInt ("player2Wins"); ++i)
			{
				playerVictories[1].text += "o ";
			}
		}

		meteors = new Transform[3];
		for(int i = 0; i < 3; ++i)
		{
			meteors[i] = null;
		}
		time = MaxTime;
	}
	
	// Update is called once per frame
	void Update () {
		GetShipInput();
		if (victoryText.text == "" && MaxTime != 0)
		{
			time -= Time.deltaTime;
			if (time >= 0)
			{
				if (alarm.isPlaying)
				{
					victoryText.GetComponents<AudioSource>()[0].volume = 1;
					alarm.Stop();
				}
				CountDownTimer.text = "Timer: " + ((int)time);
				Color tempColor = suddenDeathTexture.color;
				tempColor.a = 0;
				suddenDeathTexture.color = tempColor;
			}
			else if (time <= -5)
			{
				time = MaxTime;
				for(int i = 0; i < meteorCounter; ++i)
				{
					Destroy(meteors[i].gameObject);
					meteors[i] = null;
				}
			}
			else
			{
				if (!alarm.isPlaying)
				{
					victoryText.GetComponents<AudioSource>()[0].volume = 0;
					alarm.Play ();
				}
				if (suddenDeathTexture.color.a < .25f)
				{
					Color tempColor = suddenDeathTexture.color;
					tempColor.a += .025f;
					suddenDeathTexture.color = tempColor;
				}
				CountDownTimer.text = "SUDDEN DEATH!";
				CountDownTimer.color = Color.red;
				if (meteors[0] == null)
				{
					meteorCounter = Mathf.Min(3, meteorCounter+1);

					if (meteorCounter == 1)
					{
						meteors[0] = Instantiate(prefabMeteor, new Vector3(0, 5, 0), Quaternion.identity) as Transform;
					}
					else if (meteorCounter == 2)
					{
						meteors[0] = Instantiate(prefabMeteor, new Vector3(-1.5f, 5, 0), Quaternion.identity) as Transform;
						meteors[1] = Instantiate(prefabMeteor, new Vector3(1.5f, 5, 0), Quaternion.identity) as Transform;
					}
					else
					{
						meteors[0] = Instantiate(prefabMeteor, new Vector3(6, 5, 0), Quaternion.identity) as Transform;
						meteors[1] = Instantiate(prefabMeteor, new Vector3(0, 5, 0), Quaternion.identity) as Transform;
						meteors[2] = Instantiate(prefabMeteor, new Vector3(-6, 5, 0), Quaternion.identity) as Transform;
					}
				}
			}
		}
		else if (CountDownTimer.text != "")
		{
			if (alarm.isPlaying)
			{
				alarm.Stop();
			}
			Color tempColor = suddenDeathTexture.color;
			tempColor.a = 0;
			suddenDeathTexture.color = tempColor;
			CountDownTimer.text = "";
		}


	}

	void GetShipInput()
	{
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			ships[1].SendMessageUpwards ("Jump");
		}
		if (Input.GetKeyDown(KeyCode.W))
		{
			ships[0].SendMessageUpwards ("Jump");
		}
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			ships[1].SendMessageUpwards ("RotateShip", false);
		}
		if (Input.GetKey(KeyCode.RightArrow))
		{
			ships[1].SendMessageUpwards ("RotateShip", true);
		}
		if (Input.GetKey(KeyCode.A))
		{
			ships[0].SendMessageUpwards ("RotateShip", true);
		}
		if (Input.GetKey(KeyCode.D))
		{
			ships[0].SendMessageUpwards ("RotateShip", false);
		}
		if (Input.GetKey (KeyCode.G))
		{
			SceneManager.LoadScene(1);
		}
		if (Input.GetKey(KeyCode.S))
		{
			ships[0].SendMessageUpwards ("Slam");
		}
		if (Input.GetKey(KeyCode.DownArrow))
		{
			ships[1].SendMessageUpwards ("Slam");
		}
	}
}
