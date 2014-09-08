using UnityEngine;
using System.Collections;

public class MonsterController : MonoBehaviour {

	public GameObject m_LeftHead;
	public GameObject m_RightHead;
	public LevelController m_LvlController;
	public float m_HeadMaxDeltaPos;

	public HeadController m_LeftHeadContr;
	public HeadController m_RightHeadContr;
	Vector3 m_TargetPos;
	const float m_StartPosY = 2.5f;
	float m_MassEaten = 0.0f;

	enum MonsterState {Destroyed, Normal, AttackingTheCake};
	MonsterState m_State;

	public void Restart() {
		m_LeftHeadContr = (HeadController)m_LeftHead.GetComponent(typeof(HeadController));
		m_RightHeadContr = (HeadController)m_RightHead.GetComponent(typeof(HeadController));
		m_MassEaten = 0.0f;
		m_State = MonsterState.Normal;
		m_TargetPos.y = m_StartPosY;
		m_LeftHeadContr.Restart();
		m_RightHeadContr.Restart();
	}

	public void EatMass(float mass) {
		if (!IsOk())
			return;
		m_MassEaten += mass;
	}

	public HeadController GetTheHead(bool right) {
		if (right)
			return m_RightHeadContr;
		return m_LeftHeadContr;
	}

	// Use this for initialization
	void Start () {
		Restart();
	}

	void FixedUpdate() {
		CheckHeadsMoving();
		Move();
	}

	void CheckHeadsMoving() {
		if (!IsOk())
			return;

		float left_head_p = m_LeftHead.transform.localPosition.y;
		float right_head_p = m_RightHead.transform.localPosition.y;

		// Check for falling one head
		if (Mathf.Abs(left_head_p - right_head_p) > m_HeadMaxDeltaPos)
			MakeOneHeadFalling(left_head_p < right_head_p);

		// Check if monster is destroyed
		const float delta_min = -1.0f;
		if (Mathf.Max(left_head_p, right_head_p) < m_LeftHeadContr.GetMinPos() + delta_min &&
		    IsHeadsOk()) {
			DestroyTheMonster();
		}

		// Update heads progresess
		float max_up_p = 2.0f;
		float left_head_progress = (left_head_p - (m_LeftHeadContr.GetMinPos() + delta_min)) / max_up_p;
		float right_head_progress = (right_head_p - (m_RightHeadContr.GetMinPos() + delta_min)) / max_up_p;
		m_LeftHeadContr.UpdateHealth(left_head_progress);
		m_RightHeadContr.UpdateHealth(right_head_progress);
	}

	void SetTarget(float y) {
		m_TargetPos.y = y;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.D)) {
			DestroyTheMonster();
		}
	}

	void Move() {
		Vector3 pos = transform.localPosition;
		Vector3 delta_p = m_TargetPos - pos;
		delta_p *= Time.deltaTime / 1.0f;
		pos += delta_p;
		transform.localPosition = pos;
	}

	void MakeOneHeadFalling(bool left) {
		if (left)
			m_LeftHeadContr.Fall();
		else
			m_RightHeadContr.Fall();
	}

	void DestroyTheMonster() {
		if (!IsHeadsOk() || m_State == MonsterState.Destroyed)
			return;
		Debug.Log("DestroyTheMonster");
		m_LvlController.GameOver();
		m_LeftHeadContr.MonsterIsDestroying();
		m_RightHeadContr.MonsterIsDestroying();
		SetTarget(-10.0f);
		m_State = MonsterState.Destroyed;
	}

	bool IsHeadsOk() {
		return m_LeftHeadContr.IsOk() && m_RightHeadContr.IsOk();
	}

	bool IsOk() {
		return m_State == MonsterState.Normal;
	}
}
