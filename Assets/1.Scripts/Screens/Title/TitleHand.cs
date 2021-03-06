﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// Handles all logic for the title screen.
public class TitleHand : MonoBehaviour {
    private GSManager gsManager;
    
    /* TITLE SCREEN */
    private GameObject loadingBG;
    private Button btnStartGame;

    /* LEVEL SELECT */
    private GameObject btnLevel1;
	private GameObject btnLevel2;
	private GameObject btnLevel3;
	private GameObject fieldLevelId;
	private GameObject btnGoToLevel;

	void Start ()
    {  
		gsManager = GameObject.Find("GSManager").GetComponent<GSManager>();
        btnStartGame = GameObject.Find("BtnStartGame").GetComponent<Button>();
        btnLevel1 = GameObject.Find("BtnLevel1");
		btnLevel2 = GameObject.Find("BtnLevel2");
		btnLevel3 = GameObject.Find("BtnLevel3");
		fieldLevelId = GameObject.Find ("FieldLevelId");
		btnGoToLevel = GameObject.Find("BtnGoToLevel");

		// hide LoadingBG. can't hide in inspector because Find() can't find hidden objects.
        loadingBG = GameObject.Find("LoadingBG");
        loadingBG.SetActive(false);

        // hide level select
		hideLevelSelect ();

        // add functions to be called when buttons are clicked
        btnStartGame.onClick.AddListener(() =>
            {
				showLevelSelect();
            }
        );

        btnLevel1.GetComponent<Button>().onClick.AddListener(() =>
            {
				hideLevelSelect ();
                gsManager.currLevelId = "5713573250596864";
                gsManager.LoadScene("InventorySelector");
			}
		);

		btnLevel2.GetComponent<Button>().onClick.AddListener(() =>
		    {
				hideLevelSelect ();
				gsManager.LoadLevel("4876504257265664");
			}
		);

		btnLevel3.GetComponent<Button>().onClick.AddListener(() =>
			{
				hideLevelSelect ();
				//gsManager.LoadLevel("aaron-MultiRoomTest");
				Application.LoadLevel("loot-test-MultiRoomTest");
			}
		);

		btnGoToLevel.GetComponent<Button>().onClick.AddListener (() =>
		    {
				hideLevelSelect();
				gsManager.currLevelId = fieldLevelId.GetComponent<InputField> ().text;
				gsManager.LoadLevel (fieldLevelId.GetComponent<InputField> ().text);
			}
		);
	}

	void hideLevelSelect () {
		btnLevel1.SetActive(false);
		btnLevel2.SetActive(false);
		btnLevel3.SetActive(false);
		fieldLevelId.SetActive (false);
		btnGoToLevel.SetActive (false);
	}

	void showLevelSelect () {
		btnLevel1.SetActive(true);
		btnLevel2.SetActive(true);
		btnLevel3.SetActive(true);
		fieldLevelId.SetActive (true);
		btnGoToLevel.SetActive (true);
	}
}
