using UnityEngine;
using System.Collections;

public class GameContr : MonoBehaviour {

	public static GameContr control;
	public LevelsSettings lvl_settings = new LevelsSettings();

	public enum LevelState { Blocked, Mist, Cleared };

	// Saving data -----------------------------
	public LevelState[] m_LevelsCleared;
	// -----------------------------------------

	public int m_LevelPlaying;

	public void PassLevel() {
		m_LevelsCleared[m_LevelPlaying] = LevelState.Cleared;
		if (m_LevelPlaying + 1 < m_LevelsCleared.Length)
			m_LevelsCleared[m_LevelPlaying + 1] = LevelState.Mist;
	}

	public bool LoadLevel(int level) {
//		if (m_LevelsCleared[level] == LevelState.Blocked)
//			return false;
		m_LevelPlaying = level;
		Application.LoadLevel(2);	// Level scene
		return true;
	}

	public void ResetSavingData() {
		m_LevelsCleared = new LevelState[m_LevelsNumber];
		m_LevelsCleared[0] = LevelState.Mist;
	}

	// private members --------------------------------------

	const int m_LevelsNumber = 4;

	void Awake() {
		if (control == null) {
			DontDestroyOnLoad(gameObject);
			control = this;
		} else if (control != this) {
			Destroy(gameObject);
		}
	}

	void Start() {
		Debug.Log("GameController starting");
	}
	
	void Update() {

	}

	void OnEnable() {
		SaveLoad.Load();
		// or
//		ResetSavingData();
	}

	void OnDisable() {
		SaveLoad.Save();
	}

	void ResetTempData() {
		m_LevelPlaying = -1;
	}
}

