using UnityEngine;
using System.Collections;
using System;
using System.IO; 
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary; 
using System.Runtime.Serialization;
using System.Reflection;
using System.Text;
using System.Linq;

public class TileMapController : MonoBehaviour {
	public int grid_x;
	public int grid_z;
	public float tileSize = 1.0f;
	Camera UICamera;
	public HashSet<Vector3> selectedTiles = new HashSet<Vector3>();
	Vector3 currTile;
	public Vector3 shiftOrigin = Global.nullVector3;
	string selectedItem = null;
	GameObject currentObj;
	public bool placeRoomClicked = false;
	public float secondX;
	public float secondZ;
	public bool suppressDragSelecting;
	Vector3 clickOrigin = Global.nullVector3;
	
	void Start() {	
		UICamera = GameObject.Find("UICamera").GetComponent<Camera>();
		grid_x = 100;
		grid_z = 100;
		buildMesh();
	}

	void Update() {
		RayToScene();
		Plane ground = new Plane(Vector3.up, Vector3.zero);
		Ray ray = new Ray();
		ray.origin = UICamera.transform.position;
		ray.direction = UICamera.transform.forward;
		float distance;
		Vector3 camFocus;
		if(ground.Raycast(ray, out distance)) {
			camFocus = ray.GetPoint(distance).Round(1);
			camFocus.x -= (grid_x / 2) * transform.localScale.x;
			camFocus.z -= (grid_z / 2) * transform.localScale.z;
			transform.position = camFocus;
		}

	}

	public void fillInRoom() {
		MapData.addRoom(new Vector3(shiftOrigin.x, 0, shiftOrigin.z), new Vector3(secondX, 0, secondZ));
	}

	public bool fillInStartRoom() {
		return MapData.addStartRoom(new Vector3(shiftOrigin.x, 0, shiftOrigin.z), new Vector3(secondX, 0, secondZ));
	}

	public bool fillInEndRoom() {
		return MapData.addEndRoom(new Vector3(shiftOrigin.x, 0, shiftOrigin.z), new Vector3(secondX, 0, secondZ));
	}

	public Vector3 getCenterOfSelectedArea(){
		return (new Vector3(Mathf.Floor(shiftOrigin.x + secondX) / 2f, 0f , Mathf.Floor(shiftOrigin.z + secondZ) / 2f));
	}
	
	void RayToScene() {
		/* get world coordinates with respect to mouse position by raycast */
		Ray ray = UICamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hitInfo;
		
		/* getting raycast info and logic */
		//&& UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject () == false
		if(Physics.Raycast(ray, out hitInfo, Mathf.Infinity)) {
			//selectedItem gets set by UIHandler_ItemButtons calling setSelectedItem()
			//in this script. the !Input.GetMouseButton (0) check below will indicate
			//that a drag has ended, and so we can drop the object on the map.
			if(selectedItem != null && !Input.GetMouseButton(0)) {
				int x = Mathf.RoundToInt(hitInfo.transform.position.x / tileSize);
				int z = Mathf.RoundToInt(hitInfo.transform.position.z / tileSize);
				
				Vector3 obj_pos = new Vector3(x, 0f, z);
				Vector3 obj_rot = new Vector3(0f, 90f, 0f);
				//output_tileMap.instantiateItemObject (selectedItem, obj_pos, obj_rot);
				MapData.addObject(selectedItem, obj_pos, obj_rot.toDirection());

				clearSelectedItem();
			} else {
				/* check whether the ray hits an object or the tile map */
				switch(hitInfo.collider.gameObject.name) {
				case "TileMap":
					snapToGrid(hitInfo.point.x, hitInfo.point.z);
					break;
				default: 
					snapToGrid(hitInfo.collider.gameObject.transform.position.x, hitInfo.collider.transform.position.z);
					break;
				}
			}
		}
	}
	
	/* snap mouse selection to grid */
	void snapToGrid(float xf, float zf) {
		int x = Mathf.RoundToInt(xf / tileSize);
		int z = Mathf.RoundToInt(zf / tileSize);

		Vector3 stgVector = new Vector3(x, 0, z);
		if(Input.GetMouseButton(0)) {
			if(clickOrigin == Global.nullVector3) {
				clickOrigin = stgVector;
			}
		} else {
			clickOrigin = Global.nullVector3;
		}

		//for selecting single tiles, and for shift-clicking to select a group of tiles
		//
		//check whether mouse is pressed AND the tile hasn't been selected AND weather we're over a screen UI element
		if(Input.GetMouseButtonDown(0) && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() == false) {
			//Control functionality: selects tiles and adds to hashset
			if(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {
				//If the tile already has been selected, deselect it
				if(selectedTiles.Contains(stgVector)) {
					deselect(stgVector);
				}
				//Otherwise, select it
				else {
					selectTile(stgVector);
				}
				return;
			}
			//Shift functionality: selects all tiles between last selected tile, and shift clicked tile
			else if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
				//If no tiles have been selected ever, just select that tile
				if(shiftOrigin == Global.nullVector3) {
					selectTile(stgVector);
				}
				//Deselect other tiles, then select all tiles between bounds
				else {
					deselectAll();
					Vector3 vec = new Vector3(x, 0, z);
					Vector3 max = vec.getMaxVals(shiftOrigin);
					Vector3 min = vec.getMinVals(shiftOrigin);
					// Debug.Log (shiftOrigin + " to " + x + ", " + z);
					for(int xx = (int) min.x; xx <= (int) max.x; xx++) {
						for(int zz = (int) min.z; zz <= (int) max.z; zz++) {
							selectedTiles.Add(new Vector3(xx, 0, zz));
						}
					}
					
				}
				//Normal click functionality: Deselect all selected, select target	
			} else {
				deselectAll();
				selectTile(stgVector);
			}
			
			secondX = Mathf.RoundToInt(xf / tileSize);
			secondZ = Mathf.RoundToInt(zf / tileSize);
		}

		//if user is holding down left mouse button, and dragging,
		//we make a box of selected tile and have it resize as
		//the mouse moves around
//		Debug.Log("sfgsdfgs" + suppressDragSelecting);
		if(clickOrigin != stgVector
		   && suppressDragSelecting == false
		   && Input.GetMouseButton(0)
		   && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() == false) {
			//If no tiles have been selected ever, just select that tile
			if(shiftOrigin == Global.nullVector3) {
				selectTile(stgVector);
			}
			
			//Deselect other tiles, then select all tiles between bounds 
			else {
				deselectAll();
				Vector3 vec = new Vector3(x, 0, z);
				Vector3 max = vec.getMaxVals(shiftOrigin);
				Vector3 min = vec.getMinVals(shiftOrigin);
				//Debug.Log (shiftOrigin + " to " + x + ", " + z);
				for(int xx = (int) min.x; xx <= (int) max.x; xx++) {
					for(int zz = (int) min.z; zz <= (int) max.z; zz++) {
						selectedTiles.Add(new Vector3(xx, 0, zz));
					}
				}
				secondX = Mathf.RoundToInt(xf / tileSize);
				secondZ = Mathf.RoundToInt(zf / tileSize);
			}
			
			//			print("we draggin a box");
		}


		
		if(placeRoomClicked) {
			fillInRoom();
			placeRoomClicked = false;
		}
		//if we've clicked room button, fill in selected area with a room
		//		if (placeRoomClicked){
		//			fillInRoom(selectedTiles, shiftOrigin.x, shiftOrigin.z, x, z);
		//			placeRoomClicked = false;
		//		}
		
	}
	
	/* Add selected tile index to a list to be access by the camera script for rendering 
	 * and update the last selected tile in case of shift click */
	public void selectTile(Vector3 add) {
		selectedTiles.Add(add);
		shiftOrigin = add;
	}
	
	/*deselects all tiles */
	void deselectAll() {
		selectedTiles.Clear();
	}
	
	/*deselects tile passed into function */
	public void deselect(Vector3 remove) {
		selectedTiles.Remove(remove);
	}
	
	public void setSelectedItem(string s) {
		selectedItem = s;
	}
	
	public void clearSelectedItem() {
		selectedItem = null;
	}
	
	public HashSet<Vector3> getSelectedTiles() {
		return selectedTiles;
	}
	
	void buildMesh() {
		
		/* number of vertices in each x z rows and the total number of vertices */
		int vx = grid_x - 1;
		int vz = grid_z - 1;
		//		int vert_total = vx * vz;
		
		/* Initialization */
		Vector3[] vertices = new Vector3[4];
		int[] triangles = new int[2 * 3];
		Vector3[] normals = new Vector3[4];
		
		/* store the 4 corners of the mesh */
		vertices[0] = new Vector3(0, 0, 0);
		vertices[1] = new Vector3(tileSize * vx, 0, 0);
		vertices[2] = new Vector3(0, 0, tileSize * vz);
		vertices[3] = new Vector3(tileSize * vx, 0, tileSize * vz);
		
		/* Arrange the vertices in counterclockwise order to produce the correct normal, used for raycasting and rendering
		 backface culling */
		triangles[0] = 0;
		triangles[2] = 1;
		triangles[1] = 2;
		
		triangles[3] = 1;
		triangles[4] = 2;
		triangles[5] = 3;
		
		for(int i = 0; i < 4; ++i) {
			normals[i] = Vector3.up;
		}
		
		/* create mesh */
		Mesh mesh = new Mesh();
		
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.normals = normals;
		
		MeshFilter mesh_filter = GetComponent<MeshFilter>();
		MeshCollider mesh_collider = GetComponent<MeshCollider>();
		
		mesh_filter.mesh = mesh;
		mesh_collider.sharedMesh = mesh;
		
	}
	
}
