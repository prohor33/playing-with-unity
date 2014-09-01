using UnityEngine;
using System.Collections;

public class PointKeeper {
	public PointKeeper() {
		Reset();
	}
	
	public void Reset() {
		m_Points = 0;
		m_BombsSaved = 0;
		m_BombsWas = 0;
		m_VaseSaved = 0;
		m_VaseWas = 0;
		m_TimeGone = 0;
	}
	
	// set from level settings
	private int[] m_StarPoints = new int[3] { 100, 200, 300 };
	private int m_MaxTime = 100;
	
	private int m_Points;
	private int m_BombsSaved;
	private int m_BombsWas;
	private int m_VaseSaved;
	private int m_VaseWas;
	private int m_TimeGone;
	
	public int Points {
		get {
			const float point_for_second = 0.2f;
			const float point_for_bomb = 5.0f;
			const float point_for_vase = 20.0f;
			int  bonus_time = m_MaxTime - m_TimeGone;
			bonus_time = bonus_time >= 0 ? bonus_time : 0;
			m_Points = (int)(bonus_time * point_for_second +
			                 point_for_bomb * m_BombsSaved +
			                 point_for_vase * m_VaseSaved);
			return m_Points;
		}
	}
	public int BombsSaved {
		get { return m_BombsSaved; }
	}
	public int BombsWas {
		get { return m_BombsWas; }
	}
	public int VaseSaved {
		get { return m_VaseSaved; }
	}
	public int VaseWas {
		get { return m_VaseWas; }
	}
	public int[] StarPoints {
		get {
			return m_StarPoints;
		}
	}
	public int TimeBonus {
		get {
			return (m_MaxTime - m_TimeGone) >= 0 ? m_MaxTime - m_TimeGone : 0;
		}
	}
	
	public void AddBomb() {
		m_BombsWas++;
	}
	public void AddBombSaved() {
		m_BombsSaved++;
	}
	public void AddVase() {
		m_VaseWas++;
	}
	public void AddVaseSaved() {
		m_VaseSaved++;
	}
	public void SetTimeGone(int time) {
		m_TimeGone = time;
	}
	public string GetStarSpriteName(int star_numb) {
		if (star_numb > 2 || star_numb < 0)
			return "";
		string[] spr_names = new string[3] { "star_empty", "star_half", "star_full" };
		bool draw_full = Points >= m_StarPoints[star_numb];
		if (draw_full)
			return spr_names[2];
		int prev_point = star_numb >= 1 ? m_StarPoints[star_numb - 1] : 0;
		bool draw_half = Points >= (prev_point + (m_StarPoints[star_numb] - prev_point) / 2);
		if (draw_half)
			return spr_names[1];
		return spr_names[0];
	}
}

