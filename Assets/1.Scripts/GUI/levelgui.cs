﻿//Andrew Miller
//Created 1/17/15
//
//Levelgui.CS provides the bulk of the logic for the game state manager. Here scene transitions will occure when the proper conditions are met
//this script uses functions in the gamestate.cs script.
//

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class levelgui : MonoBehaviour {

	private string levelName;
	private string newName = "Player Name";


	// Initialize scene
	void Start () 
	{
		levelName = gamestate.Instance.getLevel ();
		print ("Loaded: " + gamestate.Instance.getLevel ());
		print ("There are still " + gamestate.Instance.getNumPlayers() + " in list");
		//finds and relocates the players to a game object called "StartPostion", if they need to be placed in the scene.
		//GameObject.FindGameObjectWithTag ("Player1").transform.position = GameObject.Find ("StartPosition");
		//GameObject.FindGameObjectWithTag("Player2").transform.position = GameObject.Find("StartPosition");
		//GameObject.FindGameObjectWithTag("Player3").transform.position = GameObject.Find("StartPosition");
		//GameObject.FindGameObjectWithTag("Player4").transform.position = GameObject.Find("StartPosition");
	}
	
	//--------------------------------
	//OnGUI()
	//--------------------------------
	//Provides scene transition conditions
	//--------------------------------
	public void OnGUI()
	{
		switch (levelName)
		{
			case "PlayerSelect":
				
				//Players must log in here, choosing a name that matches the list that is synced online.
				//Players must provide a password that also syncs with a list online.
				//Once these are both done add the player to the active list of players in this game session.

				//If they don't have a character, make one with a random name.
				//Let the player choose from a selection of staring items.
				//Give the player a code they can use online to claim the character.
				//Add to the player list (File) and sync with web.
				//Add to the active player list for this session.
				

				//Set # of players. Add a player by pressing the "attack" button on appropriate control station.
				//Ready after longin press "Attack" button.
				
				//If all the players in the game pass the ready check they it will set the party as all ready
				//moveToScene ("LevelSelect");
			

				break;

			case "LevelSelect":
				
				//Instantiate Players in the scene from the active player list.
				//All players must trigger the dungoeon of their choice.
				//Save this choice as chosenLevel

				//When all have chosen the same dungeon moveToScene("InventorySelection");

				//creates button to move between LevelSelect and InventorySelect
				if (GUI.Button (new Rect (30, 30, 150, 30), "Inventory Select"))
				{
					moveToScene ("InventorySelect");
				}
				break;

			case "InventorySelect":
				//Players can choose a loadout (including an empty one) and modify it.
				//Players can also assign a bait item to their dungeons here.
				//All players ready up to moveToScene("chosenLevel");

				//creates button to move between InventorySelect and LevelSelect
				if (GUI.Button (new Rect (30, 30, 150, 30), "Game Scene"))
				{
					moveToScene ("GameScene");
				}
				break;

			case "GameScene":

				break;

			case "RewardScene":

				break;

			case "GameOverScene":
				
				break;

			case "Credits":
				break;
		}
	}

	//--------------------------------
	//moveToScene()
	//--------------------------------
	//Moves to specified scene which is passed as a string.
	//--------------------------------
	public void moveToScene(string aScene)
	{
		levelName = aScene;
		gamestate.Instance.resetPartyReady(); //resets the ready up state.
		gamestate.Instance.setPlayerNotReady("all"); //resets all player ready statuses to false.
		print("moving to "+ aScene);
		gamestate.Instance.setLevel(aScene);
		foreach (Player plr in gamestate.Instance.players) { //preserves all the players in the List of player in the game
			DontDestroyOnLoad(plr);
		}
		DontDestroyOnLoad (gamestate.Instance);
		Application.LoadLevel(aScene);
	}

	//--------------------------------
	//moveToSceneWC()
	//--------------------------------
	//Moves to specified scene which is passed as a string. Preforms a ready check first. If it does not pass it will not move
	//on to the next scene.
	//--------------------------------
	public void moveToSceneWC(string aScene)
	{
		//checks to see if all the players in the game are ready.
		readyCheck ();
		//if all the players are ready move to the next scene.
		if(gamestate.Instance.getPartyReady()){
			moveToScene(aScene);
		} else {
			print ("Did not pass ready check, make sure everyone is ready");
		}
	}


	//-------------------------------
	//moveToSceneAndQuit()
	//-------------------------------
	//Moves to the tile screen and clears the player lists and resets the game state manager
	//-------------------------------
	public void moveToSceneAndQuit()
	{
		gamestate.Instance.resetPartyReady ();
		gamestate.Instance.setPlayerNotReady ("all");
		print ("Quiting... Moving to title Screen");
		gamestate.Instance.setLevel ("TitleScreen");
		DestroyObject (gamestate.Instance);
		Application.LoadLevel("TitleScreen");
	}



	//-------------------------------
	//readyCheck()
	//-------------------------------
	//Check to see if all the player in the game are ready. If so it sets the party status to ready.
	//-------------------------------
	public void readyCheck()
	{	
		//gets the number of players from the GSM
		int numberPlayers = gamestate.Instance.getNumPlayers();
		int readyCount = 0;

		//checks each player to see if they are ready, if they are it will add to the ready count
		if(gamestate.Instance.getPlayerReadyStatus(1))
		{
			print("Player 1 is ready in check");
			readyCount ++;
		}
		if(gamestate.Instance.getPlayerReadyStatus(2))
		{
			print("Player 2 is ready in check");
			readyCount ++;
		}
		if(gamestate.Instance.getPlayerReadyStatus(3))
		{
			print("Player 3 is ready in check");
			readyCount ++;
		}
		if(gamestate.Instance.getPlayerReadyStatus(4))
		{
			print("Player 4 is ready in check");
			readyCount ++;
		}

		//checks to see if the ready count and the player count are the same, if they are then all players are ready.
		if(readyCount == numberPlayers)
		{
			print ("All players are ready " + readyCount + " / " + numberPlayers + ".");
			gamestate.Instance.setPartyReady();
		} else {
			print ("Not all players are ready " + readyCount + " / " + numberPlayers + ".");
		}

	}

	//------------------------------
	//victoryCheck()
	//------------------------------
	//Checks to see if all the players that are alive have reached the end of the dungeon. If so Victory has been achived
	//and all the players in the game will be sent to the Rewards scene.
	//------------------------------

	public void victoryCheck()
	{
		//checks every player living player to see if they are in the victory zone, this victory property in the player class
		//toggled by entering and exiting a zone in the exit portion of the the dungeon.
		bool win = gamestate.Instance.getVictory();
		if (win) 
		{
			print ("The players have completed the dungeon!");
			moveToScene("RewardScreen");
		}else{
			print ("Not all living players have reached the end");
		}
	}

	//------------------------------
	//chickenCheck()
	//------------------------------
	//Checks to see if all the players that are alive have retreated to the beginning of the dungeon. If so they will be returned to
	//the level selection scene.
	//------------------------------
	public void chickenCheck()
	{
		bool chicken = gamestate.Instance.areChicken();
		if(chicken)
		{
			moveToScene("LevelSelect");
		}else{
			print ("Not all living players have retreated to the beginning");
		}
	}

	//-------------------------------
	//gameOverCheck()
	//-------------------------------
	//Checks to see if all the players in the game are dead, if so then it will move on to the game over scene.
	//-------------------------------
	public void gameOverCheck()
	{
		bool allDead = gamestate.Instance.getPartyDead();
		if(allDead)
		{
			print ("All Players are dead");
			moveToScene ("GameOverScreen");
		} else {
			print ("Not all players are dead");
		}
	}
	
}




