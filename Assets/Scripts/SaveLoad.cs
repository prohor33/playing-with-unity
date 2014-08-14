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

		SerializeGameController(ref bf, ref file, m_Version, true);

		file.Close();
	}

	public static void Load() {
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

		SerializeGameController(ref bf, ref file, version, false);

		file.Close();
	}

	// private members -------------------------------

	static void SerializeGameController(ref BinaryFormatter bf, ref FileStream file, int version, bool save) {

		if (save) {
			bf.Serialize(file, GameContr.control.m_LevelsCleared);
		} else {
			GameContr.control.m_LevelsCleared = (GameContr.LevelState[])bf.Deserialize(file);
			Debug.Log("Load " + GameContr.control.m_LevelsCleared.Length + " levels");
		}
	}
}

