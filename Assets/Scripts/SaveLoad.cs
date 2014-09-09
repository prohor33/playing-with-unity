using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveLoad {

	static int m_Version = 1;
	static String m_PathToSave = Application.persistentDataPath + "/game_state.dat";

	public static void Save() {
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(m_PathToSave);

		bf.Serialize(file, m_Version);

		Serialize(ref bf, ref file, m_Version, true);

		file.Close();
	}

	public static void Load() {
		try {
			LoadImpl();
		} catch (Exception e) {
			Debug.LogError("Exception caught: " + e + " All data reseted");
		}
	}

	public static void ResetAllData() {
		GameContr.control.ResetSavingData();

		Hero.m_CurrT = 0.0f;

		StatisticKeeper.m_AlreadySawKingScene = false;
	}

	// private members -------------------------------

	static void LoadImpl() {
		if (!File.Exists(m_PathToSave)) {
			Debug.LogError("Can't load file on path = " + m_PathToSave);
			
			GameContr.control.ResetSavingData();
			return;
		}
		
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Open(m_PathToSave, FileMode.Open);
		
		int version;
		version = (int)bf.Deserialize(file);
		Debug.Log("version = " + version);
		
		if (version > m_Version) {
			Debug.LogError("Can't load game file. Future version");
			return;
		}
		if (version < m_Version)
			Debug.LogWarning("Loading old version file");
		
		Serialize(ref bf, ref file, version, false);
		
		file.Close();
	}

	static void Serialize(ref BinaryFormatter bf, ref FileStream file, int version, bool save) {
		SerializeGameController(ref bf, ref file, version, save);
		SerializeDungeonSceneController(ref bf, ref file, version, save);
		SerializeStatisticKeeper(ref bf, ref file, version, save);
	}

	static void SerializeGameController(ref BinaryFormatter bf, ref FileStream file, int version, bool save) {

		if (save) {
			bf.Serialize(file, GameContr.control.m_LevelsCleared);
		} else {
			GameContr.control.m_LevelsCleared = (GameContr.LevelState[])bf.Deserialize(file);
			Debug.Log("Load " + GameContr.control.m_LevelsCleared.Length + " levels");
		}
	}

	static void SerializeDungeonSceneController(ref BinaryFormatter bf, ref FileStream file, int version, bool save) {
		
		if (save) {
			bf.Serialize(file, Hero.m_CurrT);
		} else {
			Hero.m_CurrT = (float)bf.Deserialize(file);
		}
	}

	static void SerializeStatisticKeeper(ref BinaryFormatter bf, ref FileStream file, int version, bool save) {
		
		if (save) {
			bf.Serialize(file, StatisticKeeper.m_AlreadySawKingScene);
		} else {
			StatisticKeeper.m_AlreadySawKingScene = (bool)bf.Deserialize(file);
		}
	}

}

