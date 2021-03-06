﻿using UnityEngine;
using System.Collections;

//[System.Serializable]
//public class Borders {
//	public Vector2 m_Size;
//	public int a;
//}

public class FallingObject : TouchedByFinger {
	
	public float m_OrdinaryRotationSpeed;
	
	float m_RotationSpeed = 0.0f;
	float m_Speed;
	GameObject m_ChewingHead = null;
	float m_PercentChewed = 0.0f;	// 0 to 1.0
	const float m_FallingSpeed = -3.0f;
	enum FOState { InConveyor, Chewing, FallingDown, Destroyed };
	FOState m_State;
	float m_ChewingStartPos;
	float m_ChewingAngle;
	Quaternion m_ChewingStartRot;
	SpawnObjectType m_Type;

	public bool TryToChew(GameObject chew_by) {
		if (m_State != FOState.InConveyor)
			return false;
		m_ChewingHead = chew_by;
		m_State = FOState.Chewing;
		rigidbody2D.isKinematic = true;	// If isKinematic is enabled, Forces, collisions or joints will not affect the rigidbody anymore
		m_RotationSpeed = 0.0f;
		m_ChewingStartRot = transform.rotation;
		SetText("");
		return true;
	}

	public void Fall() {
		m_State = FOState.FallingDown;
		rigidbody2D.isKinematic = false;
		SetSpeed(m_FallingSpeed);
	}

	public void ImpactByBombBlowingUp(Vector3 bomb_p) {
		m_State = FOState.FallingDown;
		rigidbody2D.isKinematic = false;

		Vector3 force_dir = transform.position - bomb_p;
		const float blow_up_power = 30.0f;
		force_dir.Normalize();
		force_dir *= blow_up_power;
		rigidbody2D.velocity = force_dir;
		rigidbody2D.gravityScale = 4.0f;
	}

	public void SetRotationSpeed(float rot_speed) {
		m_RotationSpeed = rot_speed;
	}

	public void SetSpeed(float speed) {
		m_Speed = speed;

		Vector2 new_speed = new Vector2(0.0f, 1.0f) * m_Speed;
		if (rigidbody2D.velocity != new_speed)
			rigidbody2D.velocity = new_speed;
	}

	public float GetSpeed() {
		return m_Speed;
	}

	public void DisableSpeed() {
		Vector2 new_speed = new Vector2(0.0f, 0.0f);
		if (rigidbody2D.velocity != new_speed)
			rigidbody2D.velocity = new_speed;
	}

	public void SetChewingPos(float pos) {
		m_ChewingStartPos = pos;
	}
	public void SetChewingAngle(float angle) {
		m_ChewingAngle = angle;
	}

	public void UpdateChewingPos(Vector3 head_p, float progress /* from 0.0 to 1.0 */) {
		if (m_State != FOState.Chewing)
			return;

		float progress_and_back = MakeProgressAndBack(progress);

		float amplitude = 0.4f;
		float temp_delta = m_ChewingStartPos + progress_and_back * amplitude;
		Vector3 new_p = new Vector3(temp_delta * Mathf.Cos(m_ChewingAngle), temp_delta * Mathf.Sin(m_ChewingAngle), 0.0f);
		new_p += head_p;
		transform.position = new_p;

		// Try to add some random rotation
		float rot_max_angle = 10.0f;		
		Quaternion target_rot = Quaternion.Euler(0.0f, 0.0f, rot_max_angle + m_ChewingStartRot.eulerAngles.z);
		const float coef_rot_progress = 2.0f;
		float rot_progress = coef_rot_progress * progress;
		rot_progress -= (int)(rot_progress);
		rot_progress = MakeProgressAndBack(rot_progress);
		transform.rotation = Quaternion.Slerp(m_ChewingStartRot, target_rot, rot_progress);
	}

	public float GetChewingProcess() {
		return m_PercentChewed;
	}
	public void SetChewingProcess(float process) {
		m_PercentChewed = process;
	}

	public float GetMass() {
		return rigidbody2D.mass;
	}

	public void SetChewed() {	// When object is all chewed
		if (m_State != FOState.Chewing) {
			Debug.LogWarning("SetChewed() failed");
			return;
		}
		SelfDestroy();
	}

	public void SetType(SpawnObjectType type) {
		m_Type = type;
		if (m_Type == SpawnObjectType.Vase) {
			InitVase();
		}
	}

	// Private functions ---------------------------------

	void FixedUpdate () {
		transform.Rotate (0.0f, 0.0f, m_RotationSpeed); // TODO: Move from the FixedUpdate() function

		// Check if we reach the ground
		if (m_State == FOState.FallingDown) {
			const float min_pos = -3.0f;
			if (transform.position.y < min_pos) {
				SelfDestroy();
			}
		}
	}

	// Each frame
	void Update() {}

	void Start() {
		Init();
	}

	void Init() {
		m_RotationSpeed = m_OrdinaryRotationSpeed;
		m_State = FOState.InConveyor;
		SetSpeed(m_FallingSpeed);
		
		GUIText guiText = GetComponentInChildren(typeof(GUIText)) as GUIText;
		guiText.anchor = TextAnchor.MiddleCenter;
		guiText.fontSize = 30;
		SetText(((int)rigidbody2D.mass).ToString());
	}

	void ManageColliderSize() {
		BoxCollider2D bc = Utils.GetTheClassFromGO<BoxCollider2D>(gameObject);
		SpriteRenderer sr = Utils.GetTheClassFromGO<SpriteRenderer>(gameObject);
		// TODO: remove the hardcode!
		bc.size = new Vector2(1.8f, 4.8f);
	}

	void SelfDestroy() {
		m_State = FOState.Destroyed;
		Destroy(this.gameObject);
		Destroy(this);
	}

	void SetText(string str) {
		if (!GameContr.m_DebugGUITextIsOn)
			return;

		GUIText guiText = GetComponentInChildren(typeof(GUIText)) as GUIText;
		guiText.text = str;
	}

	static float MakeProgressAndBack(float x) {
		float progress_and_back = x;
		progress_and_back *= 2.0f;
		if (progress_and_back > 1.0f)
			progress_and_back = 2.0f - progress_and_back;
		return progress_and_back;
	}

	void InitVase() {
		LevelController.control.m_PointKeeper.AddVase();
		gameObject.tag = "Vase";
		gameObject.name = "FallingVase";
		gameObject.layer = LayerMask.NameToLayer("MovingByFingerObjects");
		ManageColliderSize();
	}

	bool UnlinkFromConveyor() {
		// Unlink object from conveyor
		return LevelController.control.m_RightConveyor.UnlinkObject(this) ||
		LevelController.control.m_LeftConveyor.UnlinkObject(this);
	}

	bool UnlinkFromHead() {
		return LevelController.control.m_MonsterContr.m_LeftHeadContr.UnlinkObject(this) ||
		LevelController.control.m_MonsterContr.m_RightHeadContr.UnlinkObject(this);
	}

	protected override void TouchFirstTime() {
		LevelController.control.m_PointKeeper.AddVaseSaved();
		bool unlink_ok = UnlinkFromConveyor() || UnlinkFromHead();
		print("unlink_ok = " + unlink_ok);
	}
	protected override bool IsTouchable() {
		return m_State != FOState.InConveyor;			
	}
}
