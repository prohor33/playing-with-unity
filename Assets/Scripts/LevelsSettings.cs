using UnityEngine;
using System.Collections;

// Idea: Move all the harcode stuff to the level.cnfg file?

public class LevelsSettings {

	static public float GetHeadEatMassPerSec() {
		return m_HeadEatMassPerSecond[GameContr.control.m_LevelPlaying];
	}
	static public float GetDeltaSpawnMove() {
		return m_DeltaSpawnMove[GameContr.control.m_LevelPlaying];
	}
	static public float GetDeltaFireballTime() {
		return m_DeltaFireballTime[GameContr.control.m_LevelPlaying];
	}
	static public float GetVaseFallingAccelerationCoef() {
		return m_VaseFallingAccelerationCoef[GameContr.control.m_LevelPlaying];
	}
	static public int[] GetStarPoints() {
		return m_StarPoints[GameContr.control.m_LevelPlaying];
	}
	static public int GetMaxTime() {
		return m_MaxTime[GameContr.control.m_LevelPlaying];
	}

	static public string[] m_BackgroundSpriteNames = {"back3", "back4", "back2", "back1",
		"background", "background", "background", "background"};
	
	//	private members --------------------------

	static float[] m_HeadEatMassPerSecond = {2.0f, 3.0f, 4.0f, 6.0f, 8.0f, 12.0f, 15.0f};
	static float[] m_DeltaSpawnMove = {6.0f, 10.0f, 10.0f, 10.0f, 10.0f, 11.0f, 12.0f, 13.0f, 14.0f};
	static float[] m_DeltaFireballTime = {8.0f, 10.0f, 10.0f, 10.0f, 10.0f, 11.0f, 12.0f, 13.0f, 14.0f};
	static float[] m_VaseFallingAccelerationCoef = {1.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f};
	static int[][] m_StarPoints = 
	{
		new int[] {100, 200, 300},
		new int[] {100, 200, 300},
		new int[] {100, 200, 300},
		new int[] {100, 200, 300},
		new int[] {100, 200, 300},
		new int[] {100, 200, 300},
		new int[] {100, 200, 300},
		new int[] {100, 200, 300},
		new int[] {100, 200, 300},
		new int[] {100, 200, 300},
	};
	static int[] m_MaxTime = {100, 100, 100, 100, 100, 100, 100, 100, 100};
}

