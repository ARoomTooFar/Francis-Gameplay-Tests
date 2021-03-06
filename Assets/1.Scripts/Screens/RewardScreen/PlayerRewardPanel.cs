﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Rewired;



//controls reward panel for each player
public class PlayerRewardPanel : MonoBehaviour {
	
	//must get from gamestate manager
	public List<string> loot; //list of looted items
	
	//get from somewhere
	public int total; //total points player has for voting
	
	
	public List<int> points; //points player has allocated to each item
	Transform lootList;
	RectTransform lootListRect;
	float newRowYPos;
	Vector2 iconDimens;
	Sprite icon;
	List<GameObject> highlights; //the highlighting on a selected item
	List<Text> pointsText; //the text for the points
	int activeEntry; //the entry currently selected
	RectTransform scrollView; //the container for the scroll field
	Text totalText;
	KeyCode up;
	KeyCode down;
	KeyCode add;
	KeyCode subtract;
	
//	public Controls controls;
	public Rewired.Player cont;
	bool joyControlsOn = true;
	bool keyboardControlsOn = true;
	
	
	//joystick/button input setup
	public void setUpInputs(string upString, string downString, string addString, string subtractString, Rewired.Player c){
		up = (KeyCode)System.Enum.Parse(typeof(KeyCode), upString);
		down = (KeyCode)System.Enum.Parse(typeof(KeyCode), downString);
		add = (KeyCode)System.Enum.Parse(typeof(KeyCode), addString);
		subtract = (KeyCode)System.Enum.Parse(typeof(KeyCode), subtractString);
//		controls = c;
		cont = c;
		
//		controls.joyUsed = true;
	}
	
	void Start () {
		lootList = transform.Find("LootScroller/ScrollView/LootList");
		lootListRect = lootList.GetComponent<RectTransform>();
		loot = new List<string>();
		highlights = new List<GameObject>();
		points = new List<int>();
		pointsText = new List<Text>();
		totalText = transform.Find("Title/Text").GetComponent<Text>() as Text;

		total = 13;

		//fill with placeholders if nothing in loot list
		if(loot.Count == 0){
			loot.Add("targetingVisor");
			loot.Add("militarySpikeHelmet");
			loot.Add("fist");
			loot.Add("pruningBlade");
			loot.Add("shockNet");
			loot.Add("sprint");
			loot.Add("trashHelmetBucket");
			loot.Add("trashHelmetLightBulb");
		}

		populateList();
		highlights[0].SetActive(true); //initialize top entry to be highlighted
		activeEntry = 0;
		
		for(int i = 0; i < points.Count; i++){
			pointsText[i].text = "0";
		}
		
	}
	
	//populate list with looted items
	void populateList(){
		//spawn an icon just to get dimensions, then destroy it
		GameObject newLootItem = Instantiate(Resources.Load("RewardScreen/LootEntry")) as GameObject;
		iconDimens = newLootItem.GetComponent<RectTransform>().sizeDelta;
		Destroy (newLootItem);
		
		//create y-position of top entry
		newRowYPos = 0f - iconDimens.y / 2 - 5f;
		for(int i = 0; i < loot.Count; i++){
			makeNewEntry(loot[i]);
			highlights[i].SetActive(false);
		}
	}
	
	void Update(){
		if(joyControlsOn)
			takeJoyInputs();
		if(keyboardControlsOn)
			takeKeyboardInputs();
		
		updateHighlightedEntry();
		updateTexts();
		
	}
	
	//
	//Arcade controls
	//
	bool waitingUp = false;
	bool waitingDown = false;
	
	void takeJoyInputs(){
		//go up
//		if (waitingUp == false && Input.GetAxisRaw (controls.vert) > 0) {
		if (waitingUp == false && cont.GetAxisRaw ("Move Vertical") < 0) {
			if (activeEntry > 0)
				activeEntry -= 1;
			waitingUp = true;
//		}else if (waitingUp == true && !(Input.GetAxisRaw (controls.vert) > 0)){
		}else if (waitingUp == true && !(cont.GetAxisRaw ("Move Vertical") < 0)){
			waitingUp = false;
		}
		
		//go down
//		if (waitingDown == false && Input.GetAxisRaw (controls.vert) < 0) {
		if (waitingDown == false && cont.GetAxisRaw ("Move Vertical") > 0) {
			if(activeEntry < highlights.Count - 1)
				activeEntry += 1;
			waitingDown = true;
//		}else if (waitingDown == true && !(Input.GetAxisRaw (controls.vert) < 0)){
		}else if (waitingDown == true && !(cont.GetAxisRaw ("Move Vertical") > 0)){
			waitingDown = false;
		}
		
		//adds points to an item
//		if (!Input.GetKeyDown(controls.attack) && (Input.GetButtonDown(controls.joyAttack))) {
		if (cont.GetButtonDown ("Fire")){
			if(total > 0){
				points[activeEntry] += 1;
				total -= 1;
			}
		}
		
		//subtracts points from an item
//		if (Input.GetKeyUp (controls.secItem) || Input.GetButtonUp(controls.joySecItem))  {
		if (cont.GetButtonUp ("Item")){
			if(points[activeEntry] > 0){
				total += 1;
				points[activeEntry] -= 1;
			}
		}
	}
	
	//
	//Keyboard controls
	//
	void takeKeyboardInputs(){
		
		//moves selector up and down list
		if(Input.GetKeyDown(down)){
			if(activeEntry < highlights.Count - 1)
				activeEntry += 1;
		}else if(Input.GetKeyDown(up)){
			if(activeEntry > 0)
				activeEntry -= 1;
		}
		
		//adds or subtracts points from an item
		if(Input.GetKeyDown(add)){
			if(total > 0){
				points[activeEntry] += 1;
				total -= 1;
			}
		}else if(Input.GetKeyDown(subtract)){
			if(points[activeEntry] > 0){
				total += 1;
				points[activeEntry] -= 1;
			}
		}
	}
	
	void updateTexts(){
		pointsText[activeEntry].text = points[activeEntry].ToString();
		totalText.text = total.ToString();
	}
	
	void updateHighlightedEntry(){
		for(int i = 0; i < highlights.Count; i++){
			if(i != activeEntry){
				highlights[i].SetActive(false);
			}else{
				highlights[i].SetActive(true);
			}
		}
	}
	
	//itemName is name of item
	void makeNewEntry(string itemName){
		GameObject newLootItem = Instantiate(Resources.Load("RewardScreen/LootEntry")) as GameObject;
		RectTransform lootItemRect = newLootItem.GetComponent<RectTransform>();
		
		//grow loot list rect to accommodate more entries
		lootListRect.sizeDelta = new Vector2(lootListRect.sizeDelta.x, lootListRect.sizeDelta.y + iconDimens.y);
		
		//parent entry to loot list scroll pane
		newLootItem.transform.SetParent(lootList, false);
		
		//set entry's position
		lootItemRect.anchoredPosition = new Vector2(0f + iconDimens.x / 2, newRowYPos);
		
		//give it the proper icon
		icon = Resources.Load<Sprite>("RewardScreen/NewIcons/" + itemName);
		newLootItem.transform.Find("Icon").GetComponent<Image>().sprite = icon;
		
		//add entry's height to list for highlighting
		highlights.Add(newLootItem.transform.Find("Selector").gameObject);
		
		//add points text to list for them
		pointsText.Add(newLootItem.transform.Find("Points/Text").gameObject.GetComponent<Text>() as Text);
		points.Add(0);
		
		//increment y position for next iteration
		newRowYPos -= iconDimens.y;
	}
	
}

