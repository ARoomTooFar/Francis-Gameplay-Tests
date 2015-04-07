﻿using UnityEngine;
using System.Collections;

public static class StartEndRoom{
	static TileMapController tilemapcont;

	public static bool startRoomPlaced = false;
	public static bool endRoomPlaced = false;
	public static Vector3 playerStartingLocation;
	public static Vector3 dungeonEndingLocation;

	public static GameObject StartRoomCheckMark;
	public static GameObject EndRoomCheckMark;

	static StartEndRoom(){
		tilemapcont = GameObject.Find ("TileMap").GetComponent ("TileMapController") as TileMapController;
		StartRoomCheckMark = GameObject.Find("StartRoomCheckMark");
		EndRoomCheckMark = GameObject.Find("EndRoomCheckMark");
	}
	
	public static void placeStartRoom(){
		if(!startRoomPlaced){
			startRoomPlaced = tilemapcont.fillInStartRoom();
			if(startRoomPlaced){
				playerStartingLocation = tilemapcont.getCenterOfSelectedArea();
				GameObject g = GameObject.Instantiate(Resources.Load("Prefabs/PlayerStartingLocation"), playerStartingLocation, Quaternion.identity) as GameObject;
				StartRoomCheckMark.SetActive(true);
			}
		}else{
			Debug.Log ("Start room already exists");
		}
	}

	public static void placeEndRoom(){
		if(!endRoomPlaced){
			endRoomPlaced = tilemapcont.fillInEndRoom();
			if(endRoomPlaced){
				dungeonEndingLocation = tilemapcont.getCenterOfSelectedArea();
				GameObject g = GameObject.Instantiate(Resources.Load("Prefabs/EndingRoomLocation"), dungeonEndingLocation, Quaternion.identity) as GameObject;
				EndRoomCheckMark.SetActive(true);
			}
		}else{
			Debug.Log ("End room already exists");
		}
	}
}
