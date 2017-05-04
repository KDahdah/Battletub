using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class VictoryScreenManager : MonoBehaviour {

	public GUIText victoryText;
	public GUIText p1victories;
	public GUIText p2victories;
	// Use this for initialization
	void Start () 
	{
		if ((PlayerPrefs.HasKey("player1Wins")) && (PlayerPrefs.HasKey("player2Wins")))
		{
			if (PlayerPrefs.GetInt("player1Wins") > PlayerPrefs.GetInt("player2Wins"))
			{
				victoryText.text = "Player 1 wins the match!";
			}
			else if (PlayerPrefs.GetInt("player1Wins") < PlayerPrefs.GetInt("player2Wins"))
			{
				victoryText.text = "Player 2 wins the match!";
			}
			else
			{
				victoryText.text = "Tie match!";
			}

			p1victories.text = "P1 Victories: " + PlayerPrefs.GetInt("player1Wins");
			p2victories.text = "P2 Victories: " + PlayerPrefs.GetInt("player2Wins");
		}
		else
		{
			victoryText.text = "invalid";
			p1victories.text = "invalid";
			p2victories.text = "invalid";
		}
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.Return))
		{
			SceneManager.LoadScene(0);
		}
	}
}
