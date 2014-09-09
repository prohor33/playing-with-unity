using UnityEngine;
using System.Collections;
using System;

public class HeadController : MonoBehaviour {

	public bool m_IsRightHead;
	public GameObject m_Monster;
	public LevelController m_LevelController;
	public AudioClip m_ImpactAudio;

	MonsterController m_MonsterContr;
	System.Collections.Generic.List<FallingObject> m_ChewingObjects = new System.Collections.Generic.List<FallingObject>();
	float m_MassChewingObjs = 0.0f;
	const float m_StartPosX = 4.0f;
	const float m_StartPosY = 3.0f;
	const float m_MinPosY = 1.5f;
	const float m_MaxPosY = 4.0f;

	public enum HeadState {Falling, Stationary, Chewing, Destroyed};
	HeadState m_State;

	float m_V;	// Velocity
	float m_P_to_apply;	// Impulse to apply
	float m_dY;	//	Deviation
	float m_F_to_apply;	// Force to apply
	const float m_HeadMassItself = 100.0f;
	float m_EatMassPerSecond;
	float m_BackoffY;	// When the head get kicked it's pull out a little bit
	Animator m_Animator;

	Rect m_ProgressRect;
	Color m_ProgressColor;
	
	public void Restart() {
		m_MonsterContr = (MonsterController)m_Monster.GetComponent(typeof(MonsterController));
		m_State = HeadState.Stationary;
		DestroyChewingObjects();

		Init();
		RestartPhysics();
	}

	public void Fall() {
		if (!IsOk())
			return;
		m_State = HeadState.Falling;
		Vector3 new_p = new Vector3(m_IsRightHead ? m_StartPosX : -m_StartPosX, m_StartPosY, 0.0f);
		Vector3 appear_p = new Vector3(0.0f, 0.0f, 0.0f);
		StartCoroutine(FallAndThanAppearAgain(transform, transform.localPosition, new_p, appear_p, 3.0f));
		FallChewingObjects();
		RestartPhysics();
	}

	public float GetMassChewingObjs() {
		return m_MassChewingObjs;
	}

	public void MonsterIsDestroying() {
		m_State = HeadState.Destroyed;
		Vector3 end_p = new Vector3(m_IsRightHead ? m_StartPosX : -m_StartPosX, -10.0f, 0.0f);
		StartCoroutine(DestroyTheHead(transform, transform.position, end_p, 3.0f));
		FallChewingObjects();
	}

	public bool IsOk() {
		return m_State == HeadState.Stationary || m_State == HeadState.Chewing;
	}

	public float GetMinPos() {
		return m_MinPosY;
	}
	public float GetMaxPos() {
		return m_MaxPosY;
	}

	public void ApplyImpulse(float impulse, bool with_feedback = true) {
		m_P_to_apply += impulse;

		// Affect another head
		if (with_feedback) {
			const float affect_coef = -0.3f;
			m_MonsterContr.GetTheHead(!m_IsRightHead).ApplyImpulse(affect_coef * impulse, false);

			// And get feedback
			ApplyImpulse(-affect_coef * impulse, false);
		}
	}
	public void ApplyForce(float force, bool with_feedback = true) {
		m_F_to_apply += force;

		// Affect another head
		if (with_feedback) {
			const float affect_coef = -0.3f;
			m_MonsterContr.GetTheHead(!m_IsRightHead).ApplyForce(affect_coef * force, false);

			// And get feedback
			ApplyForce(-affect_coef * force, false);
		}
	}

	public void BlowUpBomb(Vector3 bomb_p, float impact_radius) {

		System.Collections.Generic.List<FallingObject> m_ObjectsToRemove = new System.Collections.Generic.List<FallingObject>();

		foreach (FallingObject fo in m_ChewingObjects)	{
			Vector3 fo_p = fo.gameObject.transform.position;
			float distance = Vector3.Distance(fo_p, bomb_p);
			if (distance > impact_radius)
				continue;
			fo.ImpactByBombBlowingUp(bomb_p);
			m_ObjectsToRemove.Add(fo);
		}

		foreach (FallingObject fo in m_ObjectsToRemove) {
			m_MassChewingObjs -= fo.GetMass() * (1.0f - fo.GetChewingProcess());
			m_ChewingObjects.Remove(fo);
		}
	}

	public bool UnlinkObject(FallingObject fo) {
		return m_ChewingObjects.Remove(fo);
	}

	public void UpdateKillProgress(float progress) {
		int max_size = (int)(Screen.height * 2.0f / 3.0f);
		int min_size = 0;
		int size = (int)Mathf.Lerp(min_size, max_size, progress);
		m_ProgressRect.yMin = m_ProgressRect.yMax - size;
		m_ProgressColor = Color.Lerp(Color.red, Color.green, progress);
	}

	// Private functions ---------------------------------------
	
	void Start () {
		m_Animator = (Animator)gameObject.GetComponentInChildren(typeof(Animator));

		Restart();

		GUIText guiText = GetComponentInChildren(typeof(GUIText)) as GUIText;
		guiText.anchor = TextAnchor.MiddleCenter;
		guiText.fontSize = 30;

		m_EatMassPerSecond = LevelsSettings.GetHeadEatMassPerSec();
	}

	void Init() {
		transform.localPosition = new Vector3(m_IsRightHead ? m_StartPosX : -m_StartPosX, m_StartPosY, 0.0f);
		int progress_p_shift = 5;
		int progress_width = 10;
		int progress_p_x = !m_IsRightHead ? progress_p_shift : Screen.width - progress_p_shift - progress_width;
		m_ProgressRect = new Rect(progress_p_x, Screen.height - 25, progress_width, 0);
	}

	void Update () {
		if (Input.GetKey(KeyCode.F)) {
			FallChewingObjects();
		}
	}

	void FixedUpdate() {
		UpdatePhysics();
		Move();
		UpdateMassText();
		UpdateChewingObjects();
		UpdateAnimation();
	}
	
	void Move() {
		if (!IsOk())
			return;

		transform.localPosition = new Vector3(m_IsRightHead ? m_StartPosX : -m_StartPosX,
		                                      m_StartPosY + m_dY + m_BackoffY, 0.0f);
	}

	void OnTriggerEnter2D(Collider2D other) 
	{
		switch (other.tag) {
		case "FallingBomb":
			// do nothing
			break;
		case "Vase":
		case "FallingObject":
			TryToChewObject(other.gameObject);
			break;
		}
	}

	void TryToChewObject(GameObject go) {
		if (!IsOk())
			return;
		FallingObject falling_obj = (FallingObject) go.GetComponent(typeof(FallingObject));

		// Set up chewing
		float max_mass = 150.0f;
		float percent_full = Mathf.Clamp01(m_MassChewingObjs / max_mass);
		float max_chewing_start_p = 4.0f;
		float chewing_start_p = percent_full * max_chewing_start_p + 1.0f;
		falling_obj.SetChewingPos(chewing_start_p);
		float min_angle_range = 0.0f;
		float max_angle_range = ((float)Math.PI) * 2.0f / 3.0f;
		float angle_range = Mathf.Lerp(min_angle_range, max_angle_range, 1.0f - percent_full);
		float chewing_angle = UnityEngine.Random.Range(Mathf.PI / 2.0f - angle_range, Mathf.PI / 2.0f + angle_range);
		falling_obj.SetChewingAngle(chewing_angle);

		// Try to chew
		bool res_ok = falling_obj.TryToChew(this.gameObject);
		if (!res_ok)
			return;

		StartChewObjects(falling_obj);
	}

	void StartChewObjects(FallingObject falling_obj) {
		GameObject go = falling_obj.gameObject;
		m_ChewingObjects.Add(falling_obj);
		m_MassChewingObjs += falling_obj.GetMass();
		float speed_was = falling_obj.GetSpeed();
		ApplyImpulse(go.rigidbody2D.mass * speed_was);
		
		// Report to conveyor
		m_LevelController.GetConveyor(m_IsRightHead).UnlinkObject(falling_obj);

		// Back off
		StartCoroutine(Backoff(0.3f));

		// Play sound
		audio.PlayOneShot(m_ImpactAudio, 1.0f);
	}

	void FallChewingObjects() {
		foreach (FallingObject fo in m_ChewingObjects)	{
			fo.Fall();
		}
		m_ChewingObjects.Clear();
		m_MassChewingObjs = 0.0f;
	}

	void DestroyChewingObjects() {	// For restart
		foreach (FallingObject fo in m_ChewingObjects)	{
			Destroy(fo.gameObject);
		}
		m_ChewingObjects.Clear();
		m_MassChewingObjs = 0.0f;
	}

	float GetAngle(Vector3 v1, Vector3 v2) {
		Vector3 v2P = Vector3.Cross(v2, Vector3.forward);
		float angle = Vector3.Angle(v1, v2);
		float sign = (Vector3.Dot(v1, v2P) > 0.0f) ? 1.0f : -1.0f;
		return angle * sign * Mathf.PI / 180.0f;
	}

	void UpdateMassText() {
		if (!GameContr.m_DebugGUITextIsOn)
			return;

		GUIText guiText = GetComponentInChildren(typeof(GUIText)) as GUIText;
		guiText.text = ((int)m_MassChewingObjs).ToString();
	}

	void UpdatePhysics() {
		float dt = Time.deltaTime;
		float m = m_MassChewingObjs + m_HeadMassItself;

		// Mass force
		ApplyForce(-m * 9.8f);
		// Spring force
		//const float k = 500.0f;
		const float k = 800.0f;
		ApplyForce(-k * m_dY);

		// Viscocity
//		const float visc_factor = 50.0f;
		const float visc_factor = 400.0f;
		ApplyForce(-visc_factor * m_V, false);

		m_V += m_F_to_apply * dt / m;
		m_F_to_apply = 0.0f;

		const float p_coeff = 0.0f;	// don't need it
		m_V += m_P_to_apply * p_coeff / m;
		m_P_to_apply = 0.0f;

		m_dY += m_V * dt;
	}

	void RestartPhysics() {
		m_V = 0.0f;
		m_P_to_apply = 0.0f;
		m_F_to_apply = 0.0f;
		m_dY = 0.0f;
		m_BackoffY = 0.0f;
	}

	void MoveChewingObjects() {
		foreach (FallingObject fo in m_ChewingObjects)	{
			float period_t = 3.0f;
			float delta = Time.time - ((int)(Time.time / period_t)) * period_t;
			delta /= period_t;

			fo.UpdateChewingPos(transform.position, delta);
		}
	}

	void EatMass(float mass, FallingObject fo = null) {	// if fo != null => destroy the object
		m_MonsterContr.EatMass(mass);
		if (fo) {
			m_ChewingObjects.Remove(fo);
			Destroy(fo);
		}
		m_MassChewingObjs -= mass;
		if (m_MassChewingObjs < 0.001f)
			m_State = HeadState.Stationary;
		else
			m_State = HeadState.Chewing;
	}

	void UpdateChewingObjects() {
		if (!IsOk())
			return;
		MoveChewingObjects();
		ChewObjects();
	}

	void ChewObjects() {
		float mass_to_eat = m_EatMassPerSecond * Time.deltaTime;

		while (true) {
			if (m_ChewingObjects.Count == 0)
				return;

			FallingObject fo = m_ChewingObjects[0];
			float progress = fo.GetChewingProcess();
			if (progress >= 1.0f) {
				fo.SetChewed();
				EatMass(0.0f, fo);
				continue;
			}
			float mass = fo.GetMass();
			float mass_left = (1.0f - progress) * mass;
			if (mass_to_eat < mass_left) {
				EatMass(mass_to_eat);
				progress = (progress * mass + mass_to_eat) / mass;
				fo.SetChewingProcess(progress);
				break;
			}
			fo.SetChewingProcess(1.0f);
			fo.SetChewed();
			EatMass(mass_left, fo);
			mass_to_eat -= mass_left;
		}
	}

	void SetBackoffY(float value) {
		m_BackoffY = value;
	}

	void UpdateAnimation() {
		m_Animator.SetInteger("m_IsChewing", (m_State == HeadState.Chewing) ? 1 : 0);
	}

	void OnGUI() {
		if (m_State == HeadState.Chewing || m_State == HeadState.Stationary)
			GUIUtils.DrawRect(m_ProgressRect, m_ProgressColor);
	}

	// IEnumerators ----------------------------------
	
	IEnumerator MoveObject(Transform trans, Vector3 start_p, Vector3 end_p, float time) {
		float i = 0.0f;
		float rate = 1.0f/time;
		while (i < 1.0f) {
			i += Time.deltaTime * rate;
			trans.localPosition = Vector3.Lerp(start_p, end_p, i);
			yield return null;
		}
	}

	IEnumerator FallAndThanAppearAgain(Transform trans, Vector3 start_p,
	                                   Vector3 end_p, Vector3 appear_p, float time) {
		Vector3 fall_p = start_p;
		fall_p.y = -7.0f;
		yield return StartCoroutine(MoveObject(trans, start_p, fall_p, 3.0f));
		yield return StartCoroutine(AppearAgain(trans, appear_p, end_p, time));
	}
	IEnumerator AppearAgain(Transform trans, Vector3 start_p,
	                                   Vector3 end_p, float time) {
		yield return StartCoroutine(MoveObject(trans, start_p, end_p, time));
		m_State = HeadState.Stationary;
		yield return null;
	}

	IEnumerator DestroyTheHead(Transform trans, Vector3 start_p,
	                                   Vector3 end_p, float time) {
		yield return StartCoroutine(MoveObject(trans, start_p, end_p, time));
	}

	IEnumerator Backoff(float time) {
		const float backoff_size = -0.2f;
		const float back_time_coeff = 0.1f;
		yield return StartCoroutine(LerpValue(SetBackoffY, 0.0f, backoff_size, time * back_time_coeff));
		yield return StartCoroutine(LerpValue(SetBackoffY, backoff_size, 0.0f, time * (1.0f - back_time_coeff)));
	}
	
	IEnumerator LerpValue(Action<float> valueCallback, float start_p, float end_p, float time) {
		float i = 0.0f;
		float rate = 1.0f/time;
		while (i < 1.0f) {
			i += Time.deltaTime * rate;
			valueCallback(Mathf.Lerp(start_p, end_p, i));
			yield return null;
		}
	}
}
