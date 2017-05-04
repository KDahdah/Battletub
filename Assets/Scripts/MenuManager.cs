using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

	int menuIndex = 0;
	int numRounds = 1;
	int suddenDeathTime = 0;
	int ship1Index = 0;
	int ship2Index = 0;
	public GUIText[] menuItems;
	string[] shipNames = {"duck", "uboat", "sailboat"};
	// Use this for initialization
	void Start () 
	{
		menuItems[1].text += "    " + numRounds + " wins";
		menuItems[2].text += "    none";
		menuItems[3].text += "    " + shipNames[ship1Index];
		menuItems[4].text += "    " + shipNames[ship2Index];
		PlayerPrefs.SetInt ("rounds", numRounds);
		PlayerPrefs.SetInt ("suddenDeathTime", suddenDeathTime);
		PlayerPrefs.SetInt ("ship1", ship1Index);
		PlayerPrefs.SetInt ("ship2", ship2Index);
		PlayerPrefs.SetInt ("player1Wins", 0);
		PlayerPrefs.SetInt ("player2Wins", 0);
	}
	
	// Update is called once per frame
	void Update () 
	{
		int lastMenuIndex = menuIndex;
		if (Input.GetKeyDown (KeyCode.UpArrow) || Input.GetKeyDown (KeyCode.W))
		{
			menuIndex = (menuIndex > 0) ? (menuIndex-1) : (menuItems.Length-1);
		}
		else if (Input.GetKeyDown (KeyCode.DownArrow) || Input.GetKeyDown (KeyCode.S))
		{
			menuIndex = (menuIndex < (menuItems.Length-1)) ? (menuIndex+1) : 0;
		}
		else if (Input.GetKeyDown (KeyCode.Return))
		{
			if (menuIndex == 0)
			{
				SceneManager.LoadScene(1);
			}
		}
		else if (Input.GetKeyDown (KeyCode.RightArrow) || Input.GetKeyDown (KeyCode.D))
		{
			if (menuIndex == 1)
			{
				numRounds = (numRounds < 5) ? (numRounds+1) : 1;
				menuItems[1].text = menuItems[1].text.Substring(0, menuItems[1].text.Length-10);
				menuItems[1].text += "    " + numRounds + " wins";
				PlayerPrefs.SetInt ("rounds", numRounds);
			}
			else if (menuIndex == 2)
			{
				suddenDeathTime +=15;
				if (suddenDeathTime > 60)
				{
					suddenDeathTime = 0;
					menuItems[2].text = menuItems[2].text.Substring(0, menuItems[2].text.Length-6);
					menuItems[2].text += "    none";
				}
				else if (suddenDeathTime == 15)
				{
					menuItems[2].text = menuItems[2].text.Substring(0, menuItems[2].text.Length-8);
					menuItems[2].text += "    " + suddenDeathTime;
				}
				else
				{
					menuItems[2].text = menuItems[2].text.Substring(0, menuItems[2].text.Length-6);
					menuItems[2].text += "    " + suddenDeathTime;
				}
				PlayerPrefs.SetInt ("suddenDeathTime", suddenDeathTime);
			}
			else if (menuIndex == 3)
			{
				menuItems[3].text = menuItems[3].text.Substring (0, menuItems[3].text.Length-(4 + shipNames[ship1Index].Length));
				ship1Index = (ship1Index < 2) ? (ship1Index+1) : 0;
				menuItems[3].text += "    " + shipNames[ship1Index];
				PlayerPrefs.SetInt ("ship1", ship1Index);
			}
			else if (menuIndex == 4)
			{
				menuItems[4].text = menuItems[4].text.Substring (0, menuItems[4].text.Length-(4 + shipNames[ship2Index].Length));
				ship2Index = (ship2Index < 2) ? (ship2Index+1) : 0;
				menuItems[4].text += "    " + shipNames[ship2Index];
				PlayerPrefs.SetInt ("ship2", ship2Index);
			}
		}
		else if (Input.GetKeyDown (KeyCode.LeftArrow) || Input.GetKeyDown (KeyCode.A))
		{
			if (menuIndex == 1)
			{
				numRounds = (numRounds > 1) ? (numRounds-1) : 5;
				menuItems[1].text = menuItems[1].text.Substring(0, menuItems[1].text.Length-10);
				menuItems[1].text += "    " + numRounds + " wins";
				PlayerPrefs.SetInt ("rounds", numRounds);
			}
			else if (menuIndex == 2)
			{
				suddenDeathTime -=15;
				if (suddenDeathTime == 0)
				{
					menuItems[2].text = menuItems[2].text.Substring(0, menuItems[2].text.Length-6);
					menuItems[2].text += "    none";
				}
				else if (suddenDeathTime < 0)
				{
					suddenDeathTime = 60;
					menuItems[2].text = menuItems[2].text.Substring(0, menuItems[2].text.Length-8);
					menuItems[2].text += "    " + suddenDeathTime;
				}
				else
				{
					menuItems[2].text = menuItems[2].text.Substring(0, menuItems[2].text.Length-6);
					menuItems[2].text += "    " + suddenDeathTime;
				}
				PlayerPrefs.SetInt ("suddenDeathTime", suddenDeathTime);

			}
			else if (menuIndex == 3)
			{
				menuItems[3].text = menuItems[3].text.Substring (0, menuItems[3].text.Length-(4 + shipNames[ship1Index].Length));
				ship1Index = (ship1Index > 0) ? (ship1Index-1) : 2;
				menuItems[3].text += "    " + shipNames[ship1Index];
				PlayerPrefs.SetInt ("ship1", ship1Index);
			}
			else if (menuIndex == 4)
			{
				menuItems[4].text = menuItems[4].text.Substring (0, menuItems[4].text.Length-(4 + shipNames[ship2Index].Length));
				ship2Index = (ship2Index > 0) ? (ship2Index-1) : 2;
				menuItems[4].text += "    " + shipNames[ship2Index];
				PlayerPrefs.SetInt ("ship2", ship2Index);
			}
		}

		if (menuIndex != lastMenuIndex)
		{
			menuItems[lastMenuIndex].text = menuItems[lastMenuIndex].text.Substring(2);
			menuItems[menuIndex].text = "> " + menuItems[menuIndex].text;
		}
	}
}
