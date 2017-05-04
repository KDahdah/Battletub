using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class WaveChallengeManager : MonoBehaviour {

	public Transform sailboatPrefab;
	public Transform prefabMeteor;

	Transform ship;
	float timeToWave = 0;
	float lastWave = -1;
	bool wavesHitting = true;
	int numberOfWaves = 0;
	Transform[] waveObjects;
	const float LOW_X = -4;
	const float HIGH_X = 4;
	// Use this for initialization
	void Start () 
	{
		waveObjects = new Transform[5];
		ship = Instantiate (sailboatPrefab, new Vector3(0, 0, 0.5f), Quaternion.LookRotation(Vector3.right)) as Transform;
	}
	
	// Update is called once per frame
	void Update () 
	{
		RandomEvent();
		GetShipInput();
	}

	void RandomEvent()
	{
		if (wavesHitting && (Time.time > (lastWave + 3)))
		{
			wavesHitting = false;
			lastWave = Time.time;
			timeToWave = Random.Range(2, 5);
			ClearWaves();
		}

		if (!wavesHitting && (Time.time > (lastWave + timeToWave)))
		{
			wavesHitting = true;
			lastWave = Time.time;
			AddWaves();
		}
	}

	void ClearWaves()
	{
		for(int i = 0; i < numberOfWaves; ++i)
		{
			Destroy(waveObjects[i].gameObject);
		}
	}

	void AddWaves()
	{
		numberOfWaves = Random.Range (1, waveObjects.Length);

		for(int i = 0; i < numberOfWaves; ++i)
		{
			waveObjects[i] = Instantiate(prefabMeteor, new Vector3(Random.Range (LOW_X, HIGH_X), 5, 0), Quaternion.identity) as Transform;
			waveObjects[i].GetComponent<Renderer>().enabled = true;
		}
	}

	void GetShipInput()
	{
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			ship.SendMessageUpwards ("Jump");
		}
		if (Input.GetKeyDown(KeyCode.W))
		{
			ship.SendMessageUpwards ("Jump");
		}
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			ship.SendMessageUpwards ("RotateShip", true);
		}
		if (Input.GetKey(KeyCode.RightArrow))
		{
			ship.SendMessageUpwards ("RotateShip", false);
		}
		if (Input.GetKey(KeyCode.A))
		{
			ship.SendMessageUpwards ("RotateShip", true);
		}
		if (Input.GetKey(KeyCode.D))
		{
			ship.SendMessageUpwards ("RotateShip", false);
		}
		if (Input.GetKey (KeyCode.G))
		{
			SceneManager.LoadScene(1);
		}
		if (Input.GetKey(KeyCode.S))
		{
			ship.SendMessageUpwards ("Slam");
		}
		if (Input.GetKey(KeyCode.DownArrow))
		{
			ship.SendMessageUpwards ("Slam");
		}
	}
}
