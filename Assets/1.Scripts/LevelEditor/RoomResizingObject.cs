using UnityEngine;
using System;
using System.Collections;

public class RoomResizingObject : ClickEvent {
	
	public LayerMask draggingLayerMask = LayerMask.GetMask("Walls");
	Camera UICamera;
	TileMapController tilemapcont;
	Shader focusedShader;
	
	void Start() {
		UICamera = GameObject.Find("UICamera").GetComponent<Camera>();
		tilemapcont = Camera.main.GetComponent<TileMapController>();	
		focusedShader = Shader.Find("Transparent/Bumped Diffuse");
	}
	
	public override IEnumerator onClick(Vector3 initPosition) {
		if(!Mode.isRoomMode()) {
			return false;
		}
		
		//for the ghost-duplicate
		GameObject itemObjectCopy = null;
		Transform thing = this.transform;
		Vector3 position = this.gameObject.transform.position;
		UICamera.GetComponent<CameraDraws>().room = MapData.TheFarRooms.find(position);
		UICamera.GetComponent<CameraDraws>().roomResizeOrigin = position;
		tilemapcont.suppressDragSelecting = true;
		while(Input.GetMouseButton(0)) { 
			//if user wants to cancel the drag
			if(Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButton(1)) {
				Debug.Log("Cancel");
				Destroy(itemObjectCopy);
				return false;
			}
			
			Ray ray = UICamera.ScreenPointToRay(Input.mousePosition);
			float distance;
			Global.ground.Raycast(ray, out distance);
			
			Vector3 mouseChange = initPosition - Input.mousePosition;

			position = ray.GetPoint(distance).Round();
				
			//if mouse left deadzone
			if(Math.Abs(mouseChange.x) > Global.mouseDeadZone 
				|| Math.Abs(mouseChange.y) > Global.mouseDeadZone 
				|| Math.Abs(mouseChange.z) > Global.mouseDeadZone) {
					
				if(itemObjectCopy == null) {
					//create copy of item object
					itemObjectCopy = Instantiate(this.gameObject, getPosition(), getRotation()) as GameObject;
						
					//update the item object things
					//shader has to be set in this loop, or transparency won't work
					//itemObjectCopy.gameObject.GetComponentInChildren<Renderer>().material.shader = focusedShader;
					foreach(Renderer rend in itemObjectCopy.GetComponentsInChildren<Renderer>()) {
						rend.material.shader = focusedShader;
						Color trans = rend.material.color;
						trans.a = .5f;
						rend.material.color = trans;
					}
				} else {
					itemObjectCopy.transform.position = position;
					itemObjectCopy.transform.rotation = getRotation();
				}

				UICamera.GetComponent<CameraDraws>().roomResize = position;
			}	

			yield return null; 
		}
		
		tilemapcont.suppressDragSelecting = false;
		UICamera.GetComponent<CameraDraws>().room = null;
		UICamera.GetComponent<CameraDraws>().roomResize = Global.nullVector3;
		UICamera.GetComponent<CameraDraws>().roomResizeOrigin = Global.nullVector3;
		//destroy the copy
		Destroy(itemObjectCopy);
		tilemapcont.deselect(getPosition());
		MapData.resizeRoom(this.gameObject, getPosition(), position - getPosition());
		tilemapcont.selectTile(position);
	}
	
	public Vector3 getPosition() {
		return this.transform.position;
	}
	
	public Quaternion getRotation() {
		return this.transform.rotation;
	}
}
