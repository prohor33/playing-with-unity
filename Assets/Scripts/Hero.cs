using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hero : MonoBehaviour {

	Animator m_Animator;
	DungeonSceneController m_LevelsSceneController;
	List<Vector3> m_Path = new List<Vector3>();

	// Load/Save Data
	public static float m_CurrT = -1.0f;

	float m_TargetT;

	enum HeroState { Stationary = 0, Moving };
	HeroState m_State;
	enum Direction { NoDirection = -2, Stationary = -1, Up = 0, Right = 1, Down = 2, Left = 3 };
	Direction m_Direction;
	Direction m_OldDirection;
	bool m_AlreadyMoved;

	public void Init(float scale, Vector3 pos, DungeonSceneController levels_scene_controller) {
		InitPath(scale, pos);
		if (m_CurrT < 0.0f)
			m_CurrT = 0.0f;
		m_TargetT = -1.0f; // there is no target
		m_State = HeroState.Stationary;
		m_Direction = Direction.Stationary;
		m_OldDirection = Direction.Stationary;
		m_LevelsSceneController = levels_scene_controller;
		gameObject.transform.localPosition = GetPosFromTrajectory(m_CurrT);
		gameObject.transform.localScale = new Vector3(scale, scale, scale);
		m_AlreadyMoved = false;
	}

	public void SetTarget(Vector3 p) {
		m_AlreadyMoved = true;

		float min_dist = 1000.0f;
		float min_t = -1.0f;

		int t_count = 200;
		float dt = 1.0f / t_count;
		for (float t = 0.0f; t < 1.0f; t += dt) {
			Vector3 curr_pos = GetPosFromTrajectory(t);
			float dist = Vector3.Distance(curr_pos, p);
			if (dist < min_dist) {
				min_dist = dist;
				min_t = t;
			}
		}

		if (min_t == m_CurrT)
			return;

		if (min_t < 0.0f) {
			Debug.LogError("SetTarget() error");
			return;
		}

		GoToPoint(min_t);
	}

	// private members -------------------------------

	void Start () {
		m_Animator = (Animator)gameObject.GetComponent(typeof(Animator));
		if (!m_Animator) {
			Debug.LogError("There is no animator attached");
		}
	}

	void InitPath(float scale, Vector3 pos) {
		Vector3 start_pos = new Vector3(4.2f, 22.8f, 0.0f);
		Vector3 delta_y_pos = new Vector3(0.0f, 4.20f, 0.0f);
		Vector3 delta_x_pos = new Vector3(8.5f, 0.0f, 0.0f);

		for (int i = 0; i < GameContr.control.m_LevelsCleared.Length * 2; i++) {
			m_Path.Add(start_pos - ((i % 4 == 1  || i % 4 == 2) ? delta_x_pos : Vector3.zero));
			if (i % 2 == 1)
				start_pos -= delta_y_pos;
		}

		for (int i = 0; i < m_Path.Count; i++) {
			m_Path[i] *= scale;
			m_Path[i] += pos;
		}
	}
	
	void Update () {
		for (int i = 0; i < m_Path.Count - 1; i++)
			Debug.DrawLine (m_Path[i], m_Path[i + 1], Color.red);
	}

	void FixedUpdate() {
		CheckForTargets();
		CheckForLevelPassingBy();
		UpdateDirection();
	}

	void GoToPoint(float t) {
		m_TargetT = t;
	}

	Vector3 GetPosFromTrajectory(float t /* from 0 to 1 */) {
		int section_index = GetSectionIndex(t);

		float dt = GetSectionT(t);

		return Vector3.Lerp(m_Path[section_index], m_Path[section_index + 1], dt);
	}

	int GetSectionIndex(float t) {

		if (t < 0.0f || t > 1.0f) {
			Debug.LogError("GetSectionIndex() 1");
			return -1;
		}

		int sections_n = m_Path.Count - 1;
		if (sections_n < 1) {
			Debug.LogError("GetSectionIndex() 2");
			return -1;
		}
		
		float section_t = 1.0f / sections_n;
		int section_index = (int)(t / section_t);
		return section_index;
	}

	float GetSectionT(float t) {
		int sections_n = m_Path.Count - 1;
		if (sections_n < 1) {
			Debug.LogError("GetSectionT()");
			return -1.0f;
		}
		
		float section_t = 1.0f / sections_n;
		int section_index = (int)(t / section_t);

		float dt = t - section_index * section_t;
		dt *= sections_n;	// normalize to (0, 1)
		
		if (dt < 0.0f)
			Debug.LogError("GetSectionT()");

		return dt;
	}

	void CheckForTargets() {
		const float hero_speed = 0.05f;
//		const float hero_speed = 0.5f;	// for debug!
		if (m_TargetT >= 0.0f) {
			// Stop previous
			StopCoroutine("MoveHero");

			MoveByRoute(m_TargetT, hero_speed);
			m_TargetT = -1.0f;
		}
	}

	void UpdateDirection() {
		Direction new_dir = Direction.NoDirection;
		if (m_Direction != m_OldDirection) {
			AnimatorStateInfo an_st_info = m_Animator.GetCurrentAnimatorStateInfo(0);
			if (an_st_info.IsName(m_OldDirection.ToString())) {	// if previous direction already set up
				new_dir = m_Direction;
				m_OldDirection = m_Direction;
			} else {
				// set up previous then
				new_dir = m_OldDirection;
			}
		}
		m_Animator.SetInteger("m_Direction", (int)new_dir);
    }

	void ChooseCurrentDirection(bool move_forvard) {
		int curr_sections_index = GetSectionIndex(m_CurrT);
		
		bool horisontal = curr_sections_index % 2 == 0;
		if (horisontal)
			m_Direction = (move_forvard != (curr_sections_index % 4 == 0)) ? Direction.Right : Direction.Left;
		else
			m_Direction = move_forvard ? Direction.Down : Direction.Up;
	}

	void MoveByRoute(float target_t, float speed) {
		m_State = HeroState.Moving;

		bool move_forvard = target_t > m_CurrT;
		float time = Mathf.Abs(target_t - m_CurrT) / speed;

		float delay_to_stop_prev_coroutine = 0.02f;
		StartCoroutine(MoveHeroWithDelay(gameObject.transform, m_CurrT /* fix */,
		                                 target_t, move_forvard, time,
		                                 delay_to_stop_prev_coroutine));
	}

	void CheckForLevelPassingBy() {
//		return;	// for debug!
		if (!m_LevelsSceneController)
			return;
		if (m_State != HeroState.Stationary || !m_AlreadyMoved)
			return;
		Vector3[] levels_doors_pos;
		m_LevelsSceneController.GetLevelDoorsPos(out levels_doors_pos);
		for (int i = 0; i < levels_doors_pos.Length; i++) {
			float dist = Vector3.Distance(levels_doors_pos[i], gameObject.transform.position);
			const float min_dist = 1.0f;
			if (dist < min_dist) {
				if (GameContr.control.LoadLevel(i))
					return;
			}
		}
	}
	
	// IEnumerators ----------------------------------

	IEnumerator MoveHeroWithDelay(Transform trans, float start_t, float end_t,
	                     bool move_forvard, float time, float delay) {
		yield return new WaitForSeconds(delay);
		if (IsInvoking("MoveHero"))
			StopCoroutine("MoveHero");
		StartCoroutine(MoveHero(trans, m_CurrT, end_t, move_forvard, time));
	}

	IEnumerator MoveHero(Transform trans, float start_t, float end_t, bool move_forvard, float time) {
		float t = 0.0f;
		float rate = 1.0f / time;
		while (t < 1.0f) {
			t += Time.deltaTime * rate;
			m_CurrT = Mathf.Lerp(start_t, end_t, t);
			trans.position = GetPosFromTrajectory(m_CurrT);
			ChooseCurrentDirection(move_forvard);
			yield return null;
		}
		m_State = HeroState.Stationary;
		m_Direction = Direction.Stationary;
	}
}

