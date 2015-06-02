﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GearSelectCtrl : MonoBehaviour {
	public Controls controls;
	public string menuPanelName;
	
	private GSManager gsManager;
	
	// UI state
	private bool menuMoved = false;
	private bool menuLock = false;
	private GameObject prevBtn;
	private GameObject[,] currMenuPtr;
	private int locX = 0;
	private int locY = 0;
	private int[] currItemArr;
	private int currItemArrIndex = 0;

	// inventory data
	private PlayerData playerData;
	private int[][] items = new int[8][];
	private int weaponsIndex = 0;
	private int helmetsIndex = 0;
	private int armorsIndex = 0;
	private int actionSlot1Index = 0;
	private int actionSlot2Index = 0;
	private int actionSlot3Index = 0;
	private int[] weapons = new int[20];
	private int[] helmets = new int[13];
	private int[] armors = new int[9];
	private int[] actionSlot1 = new int[10];
	private int[] actionSlot2 = new int[10];
	private int[] actionSlot3 = new int[10];

	void Start () {
		gsManager = GameObject.Find("GSManager").GetComponent<GSManager>();
		Farts serv = gameObject.AddComponent<Farts>();

		currMenuPtr = new GameObject[7, 1];
		currMenuPtr[0, 0] = GameObject.Find("/Canvas/" + menuPanelName + "/WeaponSlot");
		currMenuPtr[1, 0] = GameObject.Find("/Canvas/" + menuPanelName + "/HelmetSlot");
		currMenuPtr[2, 0] = GameObject.Find("/Canvas/" + menuPanelName + "/ArmorSlot");
		currMenuPtr[3, 0] = GameObject.Find("/Canvas/" + menuPanelName + "/ActionSlot1");
		currMenuPtr[4, 0] = GameObject.Find("/Canvas/" + menuPanelName + "/ActionSlot2");
		currMenuPtr[5, 0] = GameObject.Find("/Canvas/" + menuPanelName + "/ActionSlot3");
		currMenuPtr[6, 0] = GameObject.Find("/Canvas/" + menuPanelName + "/BtnReady");

		var pointer = new PointerEventData(EventSystem.current);
		ExecuteEvents.Execute(prevBtn, pointer, ExecuteEvents.pointerExitHandler); // unhighlight previous button
		ExecuteEvents.Execute(currMenuPtr[locY, locX], pointer, ExecuteEvents.pointerEnterHandler); //highlight current button
		prevBtn = currMenuPtr[locY, locX];

		// begin to load player gear
		if (menuPanelName == "P1Gear") {
			playerData = gsManager.dummyPlayerDataList [0];
		} else if (menuPanelName == "P2Gear") {
			playerData = gsManager.dummyPlayerDataList [1];
		} else if (menuPanelName == "P3Gear") {
			playerData = gsManager.dummyPlayerDataList [2];
		} else {
			playerData = gsManager.dummyPlayerDataList [3];
		}

		items [0] = weapons;
		items [1] = helmets;
		items [2] = armors;
		items [3] = actionSlot1;
		items [4] = actionSlot2;
		items [5] = actionSlot3;
		items [7] = new int[1]{0}; // empty slot to cover left and right controls on ready btn

		// load weapon data
		for (int i = 0; i < weapons.Length; ++i) {
			weapons[i] = playerData.inventory[i + 10];
		}

		// load helmet data
		for (int i = 0; i < helmets.Length; ++i) {
			helmets[i] = playerData.inventory[i + 39];
		}

		// load armor data
		for (int i = 0; i < armors.Length; ++i) {
			armors[i] = playerData.inventory[i + 31];
		}

		// load action slot 1
		for (int i = 0; i < actionSlot1.Length; ++i) {
			actionSlot1[i] = playerData.inventory[i];
		}

		// load action slot 2
		for (int i = 0; i < actionSlot2.Length; ++i) {
			actionSlot2[i] = playerData.inventory[i];
		}

		// load action slot 3
		for (int i = 0; i < actionSlot3.Length; ++i) {
			actionSlot3[i] = playerData.inventory[i];
		}

		// print items
		/*for (int i = 0; i < items.Length; ++i) {
			switch (i) {
			case 0:
				Debug.Log ("WEAPONS");
				break;
			case 1:
				Debug.Log ("HELMETS");
				break;
			case 2:
				Debug.Log ("ARMORS");
				break;
			case 3:
				Debug.Log ("ACTION SLOTS");
				break;
			}

			for (int j = 0; j < items[i].Length; ++j) {
				Debug.Log (items[i][j]);
			}
		}*/

		currItemArr = items [locY];

		/*Image weaponIF;
		Sprite newSprite = Resources.Load<Sprite>("ItemFrames/HuntersRifle");
		Debug.Log (newSprite);
		weaponIF = GameObject.Find ("WeaponIF").GetComponent<Image> ();
		Debug.Log (weaponIF);
		Debug.Log (weaponIF.sprite);
		weaponIF.sprite = newSprite;
		Debug.Log (weaponIF.sprite);*/
	}

	// handles menu joystick movement control
	void MenuMove (float hori, float vert) {
		if (vert == 0 && hori == 0)
		{
			menuMoved = false;
		} else if (menuMoved == false) {
			menuMoved = true;
			
			if (vert < 0)
			{
				locY = (locY + 1) % (currMenuPtr.GetLength(0));
				if (locY <= 3) {
					currItemArr = items[locY];
				}
				Debug.Log (currItemArr);
			} else if (vert > 0) {
				--locY;
				if (locY < 0)
				{
					locY = currMenuPtr.GetLength(0) - 1;
				}
				currItemArr = items[locY];
				Debug.Log (currItemArr);
			}
			
			if (hori > 0)
			{
				currItemArrIndex = (currItemArrIndex + 1) % (currItemArr.Length);
				Debug.Log (currItemArr[currItemArrIndex]);
			}
			else if (hori < 0)
			{
				--currItemArrIndex;
				if (currItemArrIndex < 0)
				{
					currItemArrIndex = currItemArr.Length - 1;
				}
				Debug.Log (currItemArr[currItemArrIndex]);
			}
			
			var pointer = new PointerEventData(EventSystem.current);
			ExecuteEvents.Execute(prevBtn, pointer, ExecuteEvents.pointerExitHandler); // unhighlight previous button
			ExecuteEvents.Execute(currMenuPtr[locY, locX], pointer, ExecuteEvents.pointerEnterHandler); //highlight current button
			prevBtn = currMenuPtr[locY, locX];
			
			//Debug.Log(locX + "," + locY);
		}
	}

	void SlotSwap() {
		if (locY > 3 && locY < 6) {

		}
	}

	void Update () {
		// UI controls
		if (menuLock == false) {
			// check for joystick movement
			MenuMove (Input.GetAxisRaw (controls.hori), Input.GetAxisRaw (controls.vert));
			
			// check for button presses
			if (Input.GetButtonUp (controls.joyAttack)) {
				var pointer = new PointerEventData (EventSystem.current);
				ExecuteEvents.Execute (currMenuPtr [locY, locX], pointer, ExecuteEvents.submitHandler);
			}
		}
	}
}
