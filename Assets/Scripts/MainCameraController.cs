using UnityEngine;
using System.Collections;

public class MainCameraController : MonoBehaviour {

	public GameObject m_Monster;
	public GameObject m_Cake;

	Vector3 m_TargetPos;

	// Use this for initialization
	void Start () {
		m_TargetPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		float sp_delta = 7.5f;
		m_TargetPos.y = m_Monster.transform.position.y;
		float monster_start_pos_y = 2.5f;

		// Some hold on when starting
		if (m_TargetPos.y < monster_start_pos_y + 1.0f)
			m_TargetPos.y = monster_start_pos_y;

		m_TargetPos.y += sp_delta;


		Move();
	}

	void Move() {
		Vector3 pos = transform.localPosition;
		Vector3 delta_p = m_TargetPos - pos;
		delta_p *= Time.deltaTime / 1.0f;
		pos += delta_p;
		transform.localPosition = pos;
	}
}
