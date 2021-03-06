using System;
using UnityEngine;
using UnityEngine.UI;

public static class MapDataParser {
	public static SceneryBlock start;
	public static SceneryBlock end;

	public static void ParseSaveString(string SaveString) {
		MapData.ClearData();
		string[] SaveStringLines = SaveString.Split('\n');
		int i = 0;
		if(Global.inLevelEditor) {
			GameObject.Find("InputField_Save").GetComponent<InputField>().text = SaveStringLines[i].Split('\t')[0].Split(':')[1];
		}
		while(SaveStringLines[i] != "Terminal") {
			i++;
		}
		i++;
		while(SaveStringLines[i] != "Room") {
			parseTerminalRooms(SaveStringLines[i++]);
		}
		i++;
		while(SaveStringLines[i] != "Scenery") {
			parseRoom(SaveStringLines[i++]);
		}
		i++;
		while(SaveStringLines[i] != "Monster") {
			parseScenery(SaveStringLines[i++]);
		}
		i++;
		while(i < SaveStringLines.Length-1) {
			parseMonster(SaveStringLines[i++]);
		}
		if(Global.inLevelEditor) {
			Mode.setTileMode();
			UndoRedoStack.currentState = SaveString;
		} else {
			Loadgear loadgear = GameObject.Find("/Loadgear").GetComponent<Loadgear>();
			loadgear.LoadPlayers();
		}
		LevelPathCheck.checkPath();
	}

	private static void parseRoom(string SaveString) {
		string[] type = SaveString.Split(':');
		string[] rooms = type[1].Trim().Split(' ');
		foreach(string rm in rooms) {
			string[] rmParams = rm.Split(',');
			if(rmParams.Length != 6) {
				continue;
			}

			Vector3 pos1 = new Vector3(float.Parse(rmParams[0]),
			                           float.Parse(rmParams[1]),
			                           float.Parse(rmParams[2]));
			Vector3 pos2 = new Vector3(float.Parse(rmParams[3]),
			                         float.Parse(rmParams[4]),
			                         float.Parse(rmParams[5]));
			ARTFRoom room = new ARTFRoom(pos1, pos2);
			room.placedThisSession = true;
			MapData.TheFarRooms.add(room);
		}
	}

	private static void parseScenery(string SaveString) {
		string[] type = SaveString.Split(':');
		string[] blocks = type[1].Trim().Split(' ');
		foreach(string blk in blocks) {
			string[] blkParams = blk.Split(',');
			if(blkParams.Length != 4) {
				continue;
			}
			Vector3 pos = new Vector3(float.Parse(blkParams[0]),
			                          float.Parse(blkParams[1]),
			                          float.Parse(blkParams[2]));
			SceneryBlock nBlk = null;
			if(MapData.SceneryBlocks.isAddValid(type[0], pos, (DIRECTION)Enum.Parse(typeof(DIRECTION), blkParams[3]))) {
				nBlk = new SceneryBlock(type[0], pos, (DIRECTION)Enum.Parse(typeof(DIRECTION), blkParams[3]));
				nBlk.SceneryBlockInfo.placedThisSession = true;
				MapData.SceneryBlocks.add(nBlk);
			}
			if(type[0] == "LevelEditor/Other/PlayerStartingLocation") {
				start = nBlk;
			}
			if(type[0] == "LevelEditor/Other/PlayerEndingLocation") {
				end = nBlk;
			}
		}
	}

	private static void parseMonster(string SaveString) {
		string[] type = SaveString.Split(':');
		string[] blocks = type[1].Trim().Split(' ');
		foreach(string blk in blocks) {
			string[] blkParams = blk.Split(',');
			if(blkParams.Length != 5) {
				continue;
			}
			Vector3 pos = new Vector3(float.Parse(blkParams[0]),
			                          float.Parse(blkParams[1]),
			                          float.Parse(blkParams[2]));
			MonsterBlock nBlk = new MonsterBlock(type[0], pos, (DIRECTION)Enum.Parse(typeof(DIRECTION), blkParams[3]));
			nBlk.MonsterBlockInfo.placedThisSession = true;
			nBlk.MonsterBlockInfo.Tier = Convert.ToInt32(blkParams[4]);
			if(!Global.inLevelEditor) {
				nBlk.GameObj.GetComponent<Enemy>().SetMonster(Convert.ToInt32(blkParams[4]));
			}
			MapData.MonsterBlocks.add(nBlk);
		}
	}

	private static void parseTerminalRooms(string SaveString) {
		string[] rms = SaveString.Split(' ');
		string[] rmParams = rms[0].Split(',');
		Vector3 pos1 = new Vector3(float.Parse(rmParams[0]),
			                           float.Parse(rmParams[1]),
			                           float.Parse(rmParams[2]));
		Vector3 pos2 = new Vector3(float.Parse(rmParams[3]),
			                           float.Parse(rmParams[4]),
			                           float.Parse(rmParams[5]));
		ARTFTerminalRoom rm = new ARTFTerminalRoom(pos1, pos2);
		rm.placedThisSession = true;
		MapData.StartingRoom = rm;
		MapData.TheFarRooms.add(rm);

		rmParams = rms[1].Split(',');
		pos1 = new Vector3(float.Parse(rmParams[0]),
		                           float.Parse(rmParams[1]),
		                           float.Parse(rmParams[2]));
		pos2 = new Vector3(float.Parse(rmParams[3]),
		                           float.Parse(rmParams[4]),
		                           float.Parse(rmParams[5]));
		rm = new ARTFTerminalRoom(pos1, pos2);
		rm.placedThisSession = true;
		MapData.EndingRoom = rm;
		MapData.TheFarRooms.add(rm);
	}
}