﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraAdjuster : MonoBehaviour {
	//public Player p1; //
	public CameraHitBox camHitBox;
	public int playerCount;
	//public int layerMask = 1 << 8;//Layer of walls
	//Average X, Z (Pulled from Player Locations)
	//Base X,Y,Z (To be adjusted by the multiplier Value)
    //Value of Adjustment
	public Vector3 diffSpot;
	public GameObject[] visibleEnemies;
	//public float avgX,avgZ,baseY,baseX,baseZ,adjVal,supBaseY,avgNum,avgPX,avgPZ;


	public GameObject[] playerList;


	void Start () {
		//layerMask = ~layerMask; //Inverse bits
		//Debug.Log(layerMask);
		transform.rotation = Quaternion.Euler(Global.initCameraRotation);
		transform.position = Global.initCameraPosition;
		diffSpot = transform.position - camHitBox.transform.position;
		//transform.rotation = Quaternion.Euler(90,0,0);
		//arbitrarily decided point
		//adjVal = 2.5f;
		//base height
		//baseY = 30f;
		//Base case reminder
		//supBaseY = baseY;
		//Adjusted value points
		//baseX = baseY/2 + adjVal;
		//baseZ = -(baseY/2 + adjVal);
		//if(p1 != null || 
		//Average the x's and z's for the target location of the camera's focus
		//avgX = (p1.transform.position.x + p2.transform.position.x + p3.transform.position.x + p4.transform.position.x)/4 + baseX; // 
		//avgZ = (p1.transform.position.z + p2.transform.position.z + p3.transform.position.z + p4.transform.position.z)/4 + baseZ; //
	
//		playerList = new List<GameObject>();
//		playerList = GameObject.FindGameObjectsWithTag ("Player");
		playerList = new GameObject[4];
		playerList[0] = GameObject.FindGameObjectWithTag ("Player1");
		playerList[1] = GameObject.FindGameObjectWithTag ("Player2");
		playerList[2] = GameObject.FindGameObjectWithTag ("Player3");
		playerList[3] = GameObject.FindGameObjectWithTag ("Player4");
	}
	/*void seeable(Character target){
		RaycastHit hit;
		
		Vector3 dir= transform.position-target.transform.position;
		/*Ray ray = Camera.main.ScreenPointToRay(target.transform.position);
        if (Physics.Raycast(ray, out hit,distance*3/4))
			Debug.DrawLine(transform.position,hit.point,Color.red);
          //  print("Hit something");
	    if( Physics.Raycast(target.transform.position-new Vector3(0,-1f,0), dir, out hit, 1000, layerMask)){
			if(hit.collider.tag == "Wall" || hit.collider.tag == "Door" /*|| hit.collider.tag == "Prop"){
				hit.collider.gameObject.GetComponent<Wall>().toggleShow();
				if(hit.collider.tag == "Door"){
					hit.collider.gameObject.GetComponent<Door>().toggleShow();
				}
				//Debug.Log(hit.collider.name+", "+hit.collider.tag);
			}
			Debug.DrawLine(target.transform.position-new Vector3(0,-1f,0),transform.position,Color.red);
		} else {
			//Debug.Log(hit.collider.name+", "+hit.collider.tag);
			//Debug.DrawLine(transform.position,target.transform.position,Color.blue);
		}
		/*if (Physics.Raycast(transform.position,(transform.position - target.transform.position
		                           ).normalized, out hit, distance, layerMask)) {
			Debug.DrawRay(transform.position, (transform.position - target.transform.position
		                           ).normalized, Color.yellow);
			Debug.Log("Did Hit");
		} else {
			Debug.DrawRay(transform.position, (transform.position - target.transform.position
		                           ).normalized *1000, Color.white);
			Debug.Log("Did not Hit");
		}
	}*/
	// Update is called once per frame
	void Update () {

		List<float> xs = new List<float>();
		List<float> zs = new List<float>();
		for(int i = 0; i < playerList.Length; i++){
			if(playerList[i] == null) continue;
			xs.Add(playerList[i].transform.position.x);
			zs.Add(playerList[i].transform.position.z);
		}
		
		xs.Sort();
		zs.Sort();
		
		float minX = xs[xs.Count - 1];
		float maxX = xs[0];
		
		float minZ = zs[zs.Count - 1];
		float maxZ = zs[0];

		float xRange = Mathf.Abs (maxX - minX);
		float zRange = Mathf.Abs (maxZ - minZ);

		float zoomFactor = Mathf.Max(xRange, zRange) * 1.1f;

		float orthoSize = this.gameObject.GetComponent<Camera>().orthographicSize;
		float minAllowedOrthoSize = 13f;

//		print (zoomFactor + ", " + minAllowedOrthoSize);

		if(zoomFactor > minAllowedOrthoSize){
			this.gameObject.GetComponent<Camera>().orthographicSize = Mathf.Lerp(orthoSize, zoomFactor, Time.deltaTime * 4f);
//			GameObject.Find("UICamera").gameObject.GetComponent<Camera>().orthographicSize = Mathf.Lerp(orthoSize, zoomFactor, Time.deltaTime);
		}else{
			this.gameObject.GetComponent<Camera>().orthographicSize = Mathf.Lerp(orthoSize, minAllowedOrthoSize, Time.deltaTime * 4f);
//			GameObject.Find("UICamera").gameObject.GetComponent<Camera>().orthographicSize = Mathf.Lerp(orthoSize, minAllowedOrthoSize, Time.deltaTime);

		}

//		Vector3 heightAdd = new Vector3(0f, heightFactor, 0f);

		transform.position = camHitBox.transform.position + diffSpot;
		diffSpot = transform.position - camHitBox.transform.position;


	}
	void avgMake(){
		
	}
	void scrollCheck(){
	}
}
