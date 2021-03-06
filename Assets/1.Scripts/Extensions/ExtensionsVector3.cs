﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public static class ExtensionsVector3
{
	/*
	 * public static Vector3 Round(this Vector3 vec)
	 * 
	 * Extension method for Vector3
	 * 
	 * returns a new Vector3 with x, y and z rounded to the nearest integer.
	 */
	public static Vector3 Round(this Vector3 vec, int decimals = 0){
		Vector3 retVal = new Vector3((float)Math.Round (vec.x, decimals),
		                             (float)Math.Round (vec.y, decimals),
		                             (float)Math.Round (vec.z, decimals));
		return retVal;
	}

	/*
	 * public static string toCSV(this Vector3 vec)
	 * 
	 * Extension method for Vector3
	 * 
	 * Returns a comma seperated list of a Vector3's values as a string
	 * Decimals with abs value < 1 have a leading 0
	 */
	public static string toCSV(this Vector3 vec){
		return string.Format ("{0},{1},{2}", vec.x, vec.y, vec.z);
	}

	/*
	 * public static Vector3 RotateTo(this Vector3 vec, DIRECTION dir)
	 * 
	 * Extension method for Vector3
	 * 
	 * Returns a new vector rotated around 0,0,0 to face in DIRECTION dir
	 * Assumes starting direction is North
	 * Currently only supports Cardinal Directions
	 */
	public static Vector3 RotateTo(this Vector3 vec, DIRECTION dir){
		if(!dir.isCardinal()) {
			throw new UnityException("Invalid Direction Arguement to Vector3.RotateTo. Must be Cardinal Direction.");
		}
		Vector3 retVal = vec.Copy();
		switch (dir) {
		case DIRECTION.North:
			retVal = Quaternion.Euler(0, 0, 0) * vec;
			break;
		case DIRECTION.East:
			retVal = Quaternion.Euler(0, 90, 0) * vec;
			break;
		case DIRECTION.South:
			retVal = Quaternion.Euler(0, 180, 0) * vec;
			break;
		case DIRECTION.West:
			retVal = Quaternion.Euler(0, 270, 0) * vec;
			break;
		}
		return retVal.Round();
	}

	/*
	 * public static Vector3 getMinVals(this Vector3 vec, Vector3 other)
	 * 
	 * Extension method for Vector3
	 * 
	 * Returns a new Vector3 with the smallest x, y, and z values of the input vectors
	 */
	public static Vector3 getMinVals(this Vector3 vec, Vector3 other){
		Vector3 retVal = new Vector3(Mathf.Min(vec.x, other.x),
		                             Mathf.Min(vec.y, other.y),
		                             Mathf.Min(vec.z, other.z));
		return retVal;
	}

	/*
	 * public static Vector3 getMaxVals(this Vector3 vec, Vector3 other)
	 * 
	 * Extension method for Vector3
	 * 
	 * Returns a new Vector3 with the largest x, y, and z values of the input vectors
	 */
	public static Vector3 getMaxVals(this Vector3 vec, Vector3 other){
		Vector3 retVal = new Vector3(Mathf.Max(vec.x, other.x),
		                             Mathf.Max(vec.y, other.y),
		                             Mathf.Max(vec.z, other.z));
		return retVal;
	}

	/*
	 * public static Vector3 Copy(this Vector3 vec)
	 * 
	 * Extension method for Vector3
	 * 
	 * Returns a new Vector3 with the same x/y/z as the input
	 */
	public static Vector3 Copy(this Vector3 vec){
		Vector3 retVal = new Vector3(vec.x, vec.y, vec.z);
		return retVal;
	}

	/*
	 * public static Vector3 toDirection(this Vector3 vec)
	 * 
	 * Extension method for Vector3
	 * 
	 * Returns the closest direction based on the y value of the Vector3
	 */
	public static DIRECTION toDirection(this Vector3 vec){
		float val = vec.y;
		val %= 360;
		val /= 45;
		int intval = Mathf.RoundToInt(val);

		return ((DIRECTION2)intval).toDir1();
	}

	public static Vector3 moveinDir(this Vector3 vec, DIRECTION dir, int num = 1){
		Vector3 retVal = vec.Copy();
		string dirStr = dir.ToString();
		if(dirStr.Contains("North")) {
			retVal.z += num;
		}
		if(dirStr.Contains("South")) {
			retVal.z -= num;
		}
		if(dirStr.Contains("East")) {
			retVal.x += num;
		}
		if(dirStr.Contains("West")) {
			retVal.x -= num;
		}
		return retVal;
	}

}


