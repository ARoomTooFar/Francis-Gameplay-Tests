using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ARTFRoomManager {

	protected internal List<ARTFRoom> roomList = new List<ARTFRoom>();

	public ARTFRoomManager() {
	}

	//Shallow Copy Constructor. Testing only
	protected internal ARTFRoomManager(ARTFRoomManager rmMan) {
		this.roomList = rmMan.roomList;
	}
	
	public void clear() {
		foreach(ARTFRoom rm in roomList) {
			rm.remove();
		}
		roomList.Clear();
	}

	public string SaveString {
		get {
			string retVal = "rooms:";
			foreach(ARTFRoom rm in roomList) {
				if(rm.Equals(MapData.StartingRoom) || rm.Equals(MapData.EndingRoom)){
					continue;
				}
				retVal += rm.SaveString + " ";
			}
			return retVal + "\n";
		}
	}

	#region ManipulationFunctions
	#region Add
	/*
	 * public void add(Vector3 pos1, Vector3 pos2)
	 * 
	 * Adds a room with two corners at the given positions
	 */
	public void add(Vector3 pos1, Vector3 pos2) {
		add(new ARTFRoom(pos1, pos2));
	}

	/*
	 * public void add(ARTFRoom rm)
	 * 
	 * Adds a room
	 */
	public void add(ARTFRoom rm) {
		//add the new room to the list of rooms
		roomList.Add(rm);
	}
	#endregion Add

	#region Remove
	/*
	 * public void remove(Vector3 pos)
	 * 
	 * Removes a room at the given position from the stored data
	 */
	public void remove(Vector3 pos) {
		remove(find(pos));
	}

	/*
	 * public void remove(ARTFRoom rm)
	 * 
	 * Removes the specified room from the stored data
	 */
	public void remove(ARTFRoom rm) {
		rm.remove();
		roomList.Remove(rm);
	}
	#endregion Remove

	#region Move
	/*
	 * public void move(Vector3 pos, Vector3 offset)
	 * 
	 * Moves the room at position by a value specified by offset
	 */
	public void move(Vector3 pos, Vector3 offset) {
		move(find(pos), offset);
	}

	/*
	 * public void move(ARTFRoom rm, Vector3 offset)
	 * 
	 * Moves the room by a value specified by offset
	 */
	public void move(ARTFRoom rm, Vector3 offset) {
		rm.move(offset);
	}
	#endregion Move

	/*
	 * public bool resize(Vector3 oldCor, Vector3 newCor)
	 * 
	 * Resizes the room at oldCor by moving oldCor to newCor
	 */
	public void resize(Vector3 oldCor, Vector3 newCor) {
		//get the room at corner
		ARTFRoom rm = find(oldCor);
		//if it doesn't exist, abort
		if(rm == null) {
			return;
		}
		//if corner is not a corner of the room, abort
		if(!rm.isCorner(oldCor)) {
			return;
		}
		//tell the room to resize itself
		rm.resize(oldCor, newCor);
	}
	#endregion ManipulationFunctions

	#region ValidationFunctions
	#region Intersection
	/*
	 * public bool doAnyRoomsIntersect(ARTFRoom rm)
	 * 
	 * Checks to see if a given room intersects with
	 * any rooms already in the list.
	 */
	public bool doAnyRoomsIntersect(Square rm, Square ignore) {
		//for each extant room
		foreach(ARTFRoom other in roomList) {
			//if the room is the room we're checking, move on
			if(other.Equals(ignore))
				continue;

			//if the rooms intersect
			if(rm.Intersect(other)) {
				return true;
			}
		}
		return false;
	}
	#endregion Intersection

	#region Move
	/*
	 * public bool isMoveValid(Vector3 oldPos, Vector3 newPos)
	 * 
	 * Checks to see if a room at oldPos intersects with
	 * any rooms already in the list if it is moved by offset.
	 */
	public bool isMoveValid(Vector3 oldPos, Vector3 newPos) {
		return isMoveValid(find(oldPos), newPos - oldPos);
	}

	/*
	 * public bool isMoveValid(ARTFRoom rm, Vector3 offset)
	 * 
	 * Checks to see if a given room intersects with
	 * any rooms already in the list if it is moved by offset.
	 */
	public bool isMoveValid(ARTFRoom rm, Vector3 offset) {
		if (rm == null) {
			return false;
		}
		Square testSquare = new Square (rm.LLCorner, rm.URCorner);
		testSquare.move(offset);

		Square roomSquare;
		foreach (ARTFRoom room in roomList) {
			if(rm.LLCorner == room.LLCorner){
				continue;
			}
			roomSquare = new Square(room.LLCorner, room.URCorner);
			if(testSquare.Intersect(roomSquare)){
				return false;
			}
		}
		return true;
	}
	#endregion Move

	#region Resize
	/*
	 * public bool isResizeValid(Vector3 oldCor, Vector3 newCor)
	 * 
	 * Checks to see if a room at oldCor intersects with
	 * any rooms already in the list if it is resized
	 */
	public bool isResizeValid(Vector3 oldCor, Vector3 newCor) {
		return isResizeValid(find(oldCor), oldCor, newCor);
	}

	/*
	 * public bool isResizeValid(ARTFRoom rm, Vector3 oldCor, Vector3 newCor)
	 * 
	 * Checks to see if a given room intersects with
	 * any rooms already in the list if it is resized
	 */
	public bool isResizeValid(ARTFRoom rm, Vector3 oldCor, Vector3 newCor) {
		if (rm == null) {
			return false;
		}
		Square testSquare = new Square (rm.LLCorner, rm.URCorner);
		testSquare.resize (oldCor, newCor);
		if (testSquare.LLCorner.x >= testSquare.URCorner.x) {
			return false;
		}
		if (testSquare.LLCorner.z >= testSquare.URCorner.z) {
			return false;
		}
		if(!isSquareValid(testSquare)) {
			return false;
		}
		if(testSquare.Cost - rm.Cost > Money.money) {
			return false;
		}
		if(doAnyRoomsIntersect(testSquare, rm)) {
			return false;
		}
		return true;
	}
	#endregion Resize
	public bool isAddValid(Vector3 cor1, Vector3 cor2) {
		return isAddValid(new Square(cor1, cor2));
	}

	public bool isAddValid(Square rm){
		if(!isSquareValid(rm)) {
			return false;
		}
		if(rm.Cost > Money.money) {
			return false;
		}
		if(doAnyRoomsIntersect(rm, rm)) {
			return false;
		}
		return true;
	}

	public bool isSquareValid(Square sq){
		if(sq.UsableArea < 25) {
			return false;
		}
		if(sq.Length < 3+2) {
			return false;
		}
		if(sq.Height < 3+2) {
			return false;
		}
		return true;
	}

	#endregion Validation

	/*
	 * public ARTFRoom find(Vector3 pos)
	 * 
	 * Gets the room at a given position
	 */
	protected internal ARTFRoom find(Vector3 pos) {
		//for each extant room
		foreach(ARTFRoom rm in roomList) {
			//if the position is
			if(rm.inRoom(pos)) {
				return rm;
			}
		}
		return null;
	}

}