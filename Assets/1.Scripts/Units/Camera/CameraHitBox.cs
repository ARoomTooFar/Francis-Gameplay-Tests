﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraHitBox : MonoBehaviour {
	public List<Character> areaUnits;
	public Player[] allPlayers;
	public float avgX,avgZ,totalX,totalZ;
	public bool same;
	public int enemyCount;

	private GSManager manager;

	//private AudioSource battle;
	//private AudioSource environment;

	// Use this for initialization
	void Start () {
		// GetComponent<Transform>();
		//allPlayers = FindObjectsOfType(typeof(Player)) as Player[];
		//for(int x = 0; x < allPlayers.Length; x++){
		//	areaUnits.Add(allPlayers[x]);
		//}

		this.manager = GameObject.Find("GSManager").GetComponent<GSManager>();

		//environment = GameObject.Find ("PerspectiveAngledCamera").GetComponent<AudioSource> ();
		//battle = GetComponent<AudioSource> ();

	}

	// Update is called once per frame
	void Update () {		
		avgMake();
	}
	void avgMake(){
		if (this.manager == null)
			return;

		totalX = 0;
		totalZ = 0;
		int count = 0;
		foreach(Player unit in manager.players) {
			if (unit == null) continue;
			totalX+=unit.transform.position.x;
			totalZ+=unit.transform.position.z;
			count++;
		}
		if(count > 0){
			avgX = totalX/count;
			avgZ = totalZ/count;
			transform.position = new Vector3(avgX,transform.position.y,avgZ);
		}
	}

	bool TransitionIn(AudioSource musik, float rate, float done) {
		if(musik.volume < 0.2f) musik.volume += 0.1f * Time.deltaTime;
		else if (musik.volume < done) musik.volume += rate * Time.deltaTime;
		return musik.volume >= done;
	}

	bool TransitionOut(AudioSource musik, float rate, float done) {
		if (musik.volume > 0.2f) musik.volume -= rate * Time.deltaTime;
		else if (musik.volume > done) musik.volume -= 0.1f * Time.deltaTime;

		if(musik.volume <= done) {
			musik.Stop();
			musik.volume = 1;
			return true;
		}
		return false;
	}


}
