using UnityEngine;
using System.Collections;

public class ObjectConveyor {
		
	public MonsterLevelController m_LevelContr;

	System.Collections.Generic.List<FallingObject> m_Objects =
		new System.Collections.Generic.List<FallingObject>();
	bool m_IsRight;
	float m_MinSpeed;
	bool m_NeedToScroll;
	float m_Scroll;
	float m_DeltaSpawnMove = 0.0f;
	SpawnerOnConveyor m_SpawnContr = new SpawnerOnConveyor();
	float m_Speed;
	float m_ScrollingSlowingCoef;

	public ObjectConveyor(bool is_right) {
		m_IsRight = is_right;
	}

	public void Restart() {
		m_MinSpeed = -2.0f;
		m_NeedToScroll = false;
		m_ScrollingSlowingCoef = 1.0f;
		DestroyFallingObjects();
		m_SpawnContr.m_GameContr = m_LevelContr;
		m_SpawnContr.Init();
	}

	public void FixedUpdate() {
		UpdateSpeed();
		if (m_NeedToScroll)
			MakeScroll();
		else
			Move();
		Spawn();
		m_SpawnContr.FixedUpdate();
	}

	public void SetMinSpeed(float min_speed) {
		m_MinSpeed = min_speed;
	}

	public void Scroll(float delta) {
		if (delta >= 0) {
			m_ScrollingSlowingCoef = 10.0f * Mathf.Max(delta, 1.0f);
			return;
		}
		m_NeedToScroll = true;
		m_Scroll = delta;
	}

	public void UnlinkObject(FallingObject fo) {
		m_Objects.Remove(fo);
	}

	public void AddObject(FallingObject falling_obj) {
		m_Objects.Add(falling_obj);
	}

	// Private members ------------------------------

	void Spawn() {
		if (m_SpawnContr.Spawn(m_DeltaSpawnMove, m_IsRight))
			m_DeltaSpawnMove = 0.0f;
	}

	void DestroyFallingObjects() {
		foreach (FallingObject fo in m_Objects)	{
			if (fo == null)
				continue;
			MonoBehaviour.Destroy(fo.gameObject);
			MonoBehaviour.Destroy(fo);
		}
		m_Objects.Clear();
	}

	void MakeScroll() {
		m_DeltaSpawnMove += Mathf.Abs(m_Scroll);
		float v = m_Scroll / Time.deltaTime;
		m_Speed = v;
		foreach (FallingObject fo in m_Objects)	{
			if (fo == null)
				continue;
			Vector3 pos = fo.gameObject.transform.position;
			pos.y += m_Scroll;
			fo.gameObject.transform.position = pos;
			fo.SetSpeed(v);
			fo.DisableSpeed();
		}
		m_NeedToScroll = false;
	}

	void Move() {
		m_DeltaSpawnMove += Mathf.Abs(m_Speed) * Time.deltaTime;
		foreach (FallingObject fo in m_Objects)	{
			if (fo == null)
				continue;
		  	fo.SetSpeed(m_Speed);
		}
	}

	void UpdateSpeed() {
		const float min_scrolling_slowing_coef = 1.0f;
		// Slow down
		if (Mathf.Abs(m_Speed) > Mathf.Abs(m_MinSpeed))
			m_Speed -= -m_ScrollingSlowingCoef * Mathf.Abs(m_Speed) * Time.deltaTime;
		else {
			m_Speed = m_MinSpeed;
			m_ScrollingSlowingCoef = min_scrolling_slowing_coef;
		}
		m_SpawnContr.SetSpeedDeltaSpawnMoveCoef(Mathf.Lerp(1.0f, 1.3f, m_Speed / m_MinSpeed / 10.0f));
	}
}


