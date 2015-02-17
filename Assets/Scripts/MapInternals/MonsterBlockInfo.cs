using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/*
 * Class to hold information about monsters
 * 
 */
public class MonsterBlockInfo {

	/*
     * Constructor
     */
	private MonsterBlockInfo(string blockID) {
		this.BlockID = blockID;
	}

	//static dictionary containing info for every block
	private static Dictionary<string, MonsterBlockInfo> infoDictionary;

	/*
     * private static bool loadBlockInfo()
     * 
     * loads information about blocks from whatever file
     */
	private static void loadBlockInfo() {
		infoDictionary = new Dictionary<string, MonsterBlockInfo>();
		//load data
		infoDictionary.Add("scenery1", new MonsterBlockInfo("monster1"));
		//return true;
	}
    
	/*
     * public static SceneryBlockInfo get(string blockID)
     * 
     * Returns the info for the block with ID blockID.
     * Returns null if info does not exist.
     * 
     * Also loads the data if it hasn't been loaded yet.
     */
	public static MonsterBlockInfo get(string blockID) {
		if(infoDictionary == null) {
			loadBlockInfo();
		}

		try {
			return infoDictionary[blockID];
		} catch(Exception) {
			return null;
		}
	}

	public string BlockID {
		get;
		private set;
	}
}


