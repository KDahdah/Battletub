using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class FloatShips : MonoBehaviour {

    Water water;
	int waterVertIndex = 0;
	bool isFloating = true;
	bool isSinking = false;
	bool isSlamming = false;
	GUIText victoryText;
	public float SinkSpeed;
	public float splashInvincibilityTime;
	float splashTime = -999;
	public float waveStrength = 10;

	public float coolDownTime;
	float lastJumpTime = -999;
	public string matString;
	Material childMaterial;
	float timeOfRoundEnd = -1;
	public float balanceModifier = 1;
	public float slamModifier = 1;
	public Vector3 savedVelocity = Vector3.zero;
	// Use this for initialization
	void Start () 
    {
		if (matString == "")
		{
			childMaterial = transform.GetChild (0).GetComponent<Renderer>().material;
		}
		else
		{
			childMaterial = transform.GetChild (0).Find (matString).GetComponent<Renderer>().material;
		}
		victoryText = GameObject.Find ("VictoryText").GetComponent<GUIText>();
        water = GameObject.Find("Water").GetComponent<Water>();
		waterVertIndex = water.findVertexIndexFromX(transform.position.x);
		Vector3 tempPos = transform.position;
		tempPos.y = water.getVertexY (waterVertIndex);
		transform.position = tempPos;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if ((timeOfRoundEnd != -1) && (Time.time > (timeOfRoundEnd + 5)))
		{
			timeOfRoundEnd = -1;

			if ((PlayerPrefs.GetInt ("player1Wins") == PlayerPrefs.GetInt ("rounds")) || (PlayerPrefs.GetInt ("player2Wins") == PlayerPrefs.GetInt ("rounds")))
			{
				SceneManager.LoadScene(2);
			}
			else
			{
				SceneManager.LoadScene(1);
			}
			return;
		}
		if ((Time.time > (lastJumpTime + coolDownTime)) && (childMaterial.color == Color.gray))
		{
			childMaterial.color = Color.yellow;
		}
		if (!isSinking)
		{
			if (isFloating)
			{
				Vector3 normal = new Vector3(water.getVertexY(waterVertIndex-1) - water.getVertexY(waterVertIndex+1),-(water.getVertexX(waterVertIndex-1) - water.getVertexX(waterVertIndex+1)),0);
				if (Time.time > (splashInvincibilityTime + splashTime))
				{
					if (Vector3.Angle (Vector3.up, normal) > 5)
					{
						if (transform.position.x > 0)
						{
							if (transform.up.x >= 0)
							{
								transform.Rotate(Vector3.left*(normal.y*waveStrength));
				
							}
							else
							{
								transform.Rotate(Vector3.right*(normal.y*waveStrength));
			
							}
						}
						else
						{
							if (transform.up.x <= 0)
							{
								transform.Rotate(Vector3.left*(normal.y*waveStrength));
		
							}
							else
							{
								transform.Rotate(Vector3.right*(normal.y*waveStrength));
	
							}
						}

					}
					else
					{
						if (transform.position.x > 0)
						{
							if (transform.up.x > 0)
							{
								transform.Rotate(Vector3.left*.05f);
							}
							else
							{
								transform.Rotate(Vector3.right*.05f);
							}
						}
						else
						{
							if (transform.up.x < 0)
							{
								transform.Rotate(Vector3.left*.05f);
							}
							else
							{
								transform.Rotate(Vector3.right*.05f);
							}
						}
					}
				}

				isFloating = true;
				Vector3 tempVec = transform.position;
				tempVec.y = water.getVertexY(waterVertIndex);
				transform.position = tempVec;

				savedVelocity = transform.GetComponent<Rigidbody>().velocity;
				transform.GetComponent<Rigidbody>().velocity = Vector3.zero;

				if (Vector3.Angle (transform.up, Vector3.up) > 75)
				{
					isSinking = true;
					isFloating = false;
					if (victoryText.text == "")
					{
						if (transform.position.x > 0)
						{
							victoryText.GetComponents<AudioSource>()[0].Stop ();
							Camera.main.GetComponent<AudioSource>().Stop ();
							victoryText.GetComponents<AudioSource>()[1].Play ();
							victoryText.text = "Player 1 Wins!";
							timeOfRoundEnd = Time.time;
							PlayerPrefs.SetInt ("player1Wins", PlayerPrefs.GetInt("player1Wins")+1);
						}
						else
						{
							victoryText.GetComponents<AudioSource>()[0].Stop ();
							Camera.main.GetComponent<AudioSource>().Stop ();
							victoryText.GetComponents<AudioSource>()[1].Play ();
							victoryText.text = "Player 2 Wins!";
							timeOfRoundEnd = Time.time;
							PlayerPrefs.SetInt ("player2Wins", PlayerPrefs.GetInt("player2Wins")+1);
						}
					}
				}
			}
			if((transform.GetComponent<Rigidbody>().velocity.y < 0) && (!isFloating) && (transform.transform.position.y < water.getVertexY(waterVertIndex)))
			{
				if (isSlamming)
				{
					splashTime = Time.time;
					lastJumpTime = Time.time;
					childMaterial.color = Color.gray;
					GetComponents<AudioSource>()[0].Play();
				}
				isSlamming = false;
				isFloating = true;
				Vector3 tempVec = transform.position;
				tempVec.y = water.getVertexY(waterVertIndex);
				transform.position = tempVec;

				savedVelocity = transform.GetComponent<Rigidbody>().velocity;
				transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
			}
		}
	}

	void Jump()
	{
		if (isFloating && !isSinking && (Time.time > (coolDownTime + lastJumpTime)))
		{
			if (Vector3.Angle(transform.up, Vector3.up) < 20)
			{
				GetComponents<AudioSource>()[1].Play();
				isFloating = false;
				transform.GetComponent<Rigidbody>().velocity = Vector3.up *( 5 + (water.getWaveSpringSpeed(waterVertIndex)*30));
			}
		}
	}

	void RotateShip(bool rotateLeft)
	{
		if (isFloating && !isSinking)
		{
			if (rotateLeft)
			{
				transform.Rotate(Vector3.left*balanceModifier);
			}
			else
			{
				transform.Rotate(Vector3.right*balanceModifier);
			}
		}
	}

	void Slam()
	{
		if (!isFloating && !isSlamming)
		{
			isSlamming = true;
			transform.GetComponent<Rigidbody>().velocity += (Vector3.down*slamModifier);
		}
	}

	void setType(string shipType)
	{
		if (shipType == "duck")
		{
			balanceModifier = 5;
			slamModifier = 8;
		}
		else if (shipType == "sailboat")
		{
			balanceModifier = 3;
			slamModifier = 11;
		}
		else if (shipType == "uboat")
		{
			balanceModifier = 1;
			slamModifier = 14;
		}
	}
}
