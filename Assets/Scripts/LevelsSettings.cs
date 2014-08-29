using UnityEngine;
using System.Collections;

public class LevelsSettings {

	public float GetHeadEatMassPerSec() {
		return m_HeadEatMassPerSecond[GameContr.control.m_LevelPlaying];
	}

	public float GetDeltaSpawnMove() {
		return m_DeltaSpawnMove[GameContr.control.m_LevelPlaying];
	}

	public float GetDeltaFireballTime() {
		return m_DeltaFireballTime[GameContr.control.m_LevelPlaying];
	}

	public string[] m_BackgroundSpriteNames = {"back3", "back4", "back2", "back1",
		"background", "background", "background", "background"};
	
	//	private members --------------------------

	float[] m_HeadEatMassPerSecond = {2.0f, 3.0f, 4.0f, 6.0f, 8.0f, 12.0f, 15.0f};
	float[] m_DeltaSpawnMove = {6.0f, 10.0f, 10.0f, 10.0f, 10.0f, 11.0f, 12.0f, 13.0f, 14.0f};
	float[] m_DeltaFireballTime = {8.0f, 10.0f, 10.0f, 10.0f, 10.0f, 11.0f, 12.0f, 13.0f, 14.0f};

}

