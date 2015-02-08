﻿using UnityEngine;
using System.Collections;
using System;
using System.IO; 
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary; 
using System.Runtime.Serialization;
using System.Text;

//This class listens to save/deploy/load buttons
public class UIHandler_FileIO : MonoBehaviour
{
	Output_TileMap output_tileMap;

	public Button Button_Save = null;
	public Button Button_Deploy = null;
	public Button Button_Load = null;
	MouseHandler_TileSelection tileSelection;
//	Dictionary<string, Vector3> savedState;
	DataHandler_Items data;
	BinaryWriter bin;
	private StreamWriter writer; // This is the writer that writes to the file
	private string assetText;
	static ItemClass itemClass = new ItemClass ();

	private Farts serv;
	long levelId;

	//object in hierarchy that holds itemObjects
	Transform itemObjects;

	void Start ()
	{
		output_tileMap = GameObject.Find ("TileMap").GetComponent("Output_TileMap") as Output_TileMap;

		serv = gameObject.AddComponent<Farts> ();
		Button_Save.onClick.AddListener (() => {
			saveFile (); });
		Button_Load.onClick.AddListener (() => {
			loadFile ();});


		tileSelection = GameObject.Find ("TileMap").GetComponent ("MouseHandler_TileSelection") as MouseHandler_TileSelection;
//		savedState = new Dictionary<string, Vector3> ();

		data = GameObject.Find ("ItemObjects").GetComponent ("DataHandler_Items") as DataHandler_Items;

		itemObjects = GameObject.Find ("ItemObjects").GetComponent ("Transform") as Transform;

	}

	public void saveFile ()
	{

		
//		BinaryFormatter bf = new BinaryFormatter ();
//		FileStream file = File.Create ("Assets/Resources/savedLevel.txt");
		if (itemClass.getItemList ().Count != 0) {
//			bf.Serialize (file, itemClass.getItemList ());
//			File.WriteAllText("Assets/Resources/savedLevel.txt", bf.Serialize (file, itemClass.getItemList ()));
//			Debug.Log ("Items saved right now: " + itemClass.getItemList ().Count);
			Debug.Log ("saving list " + itemClass.getItemList ().GetHashCode());

			BinaryFormatter bf = new BinaryFormatter ();
			MemoryStream stream = new MemoryStream (2048);
			bf.Serialize (stream, itemClass.getItemList ());
			string tmp = System.Convert.ToBase64String (stream.ToArray ());
			PlayerPrefs.SetString ("levelData", tmp);


			//give error: SecurityException: No valid crossdomain policy available to allow access
//			string ulLevelData = serv.newLevel(123, "Level Name", tmp);
//			Debug.Log(ulLevelData);

//			string levelData = bf.Serialize (file, itemClass.getItemList ());
//			levelId = serv.newLevel(123, "Level Name", levelData);
		} else {
			Debug.Log ("ItemClass.itemList is empty. Nothing to write.");
		}
//		file.Close ();

//		if (data.getItemDictionary ().Count != 0) {
//			savedState.Clear ();
//			savedState = new Dictionary<string, Vector3> (data.getItemDictionary ());
//		} else {
//			Debug.Log ("Nothing to save");
//		}
	}

	public void loadFile ()
	{
		if (itemClass.getItemList ().Count != 0) {
//		if (File.Exists ("Assets/Resources/savedLevel.txt")) {
//			data.wipeItemObjects ();
			wipeItemObjects ();

//			data.clearItemDictionary ();
			itemClass.clearItemList ();


			BinaryFormatter bf = new BinaryFormatter ();
			string tmp = PlayerPrefs.GetString ("levelData", string.Empty);
			MemoryStream memoryStream = new MemoryStream (System.Convert.FromBase64String (tmp));
			List<ItemClass.ItemStruct> savedFile = (List<ItemClass.ItemStruct>)bf.Deserialize (memoryStream);

		//download levelData from server
		//			string levelData = "";
		//			levelData = serverConnect.getLevel(levelId); //string is level ID

			


//			BinaryFormatter bf = new BinaryFormatter ();
//			FileStream file = File.Open ("Assets/Resources/savedLevel.txt", FileMode.Open);
//			List<ItemClass.ItemStruct> savedFile = (List<ItemClass.ItemStruct>)bf.Deserialize (file);
//			file.Close ();

//			Debug.Log ((List<ItemClass.ItemStruct>)bf.Deserialize);
			for (int i = 0; i < savedFile.Count; i++) {
				Vector3 pos = new Vector3 (savedFile [i].x, savedFile [i].y, savedFile [i].z);
				Vector3 rot = new Vector3 (savedFile [i].xrot, savedFile [i].yrot, savedFile [i].zrot);
				string name = savedFile [i].item.Substring (0, savedFile [i].item.IndexOf ('_'));
//				tileSelection.placeItems (name, pos);
				output_tileMap.instantiateItemObject(name, pos, rot);
			}
		} else {
			Debug.Log ("ItemClass.itemList is empty. Cannot Load.");
		}
//		} else {
//			Debug.Log ("savedLevel.txt does not exist. Cannot load.");
//		}


		
//		if (savedState.Count != 0) {
//			data.wipeItemObjects ();
//			data.clearItemDictionary ();
//			foreach (KeyValuePair<string, Vector3> entry in savedState) {
//				string key = entry.Key.Substring (0, entry.Key.IndexOf ('_'));
//
//				tileSelection.placeItems (key, entry.Value);
//			}
//		} else {
//			Debug.Log ("Nothing to load");
//		}
	}

	public void wipeItemObjects ()
	{
		foreach (Transform child in itemObjects) {
			GameObject.Destroy (child.gameObject);
		}
		itemClass.resetNameCounter ();
	}

//	public Transform getItemObjects ()
//	{
//		return itemObjects;
//	}
		

}
