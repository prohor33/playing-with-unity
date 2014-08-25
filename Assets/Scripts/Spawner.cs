using UnityEngine;
using System.Collections;

public class Spawner {

	public MonsterLevelController m_LevelContr;

	float m_FireballSpawnDeltaTime;

	public void Init()	{
		m_FireballSpawnDeltaTime = 0.0f;
	}

	// Called from level controller
	public void FixedUpdate () {
		UdateFireball();
	}

	void UdateFireball() {
		m_FireballSpawnDeltaTime += Time.fixedDeltaTime;
		if (m_FireballSpawnDeltaTime > GameContr.control.lvl_settings.GetDeltaFireballTime()) {
			m_FireballSpawnDeltaTime = 0.0f;
			SpawnFireball();
		}
	}

	void SpawnFireball() {
		bool from_right = Random.Range(0, 1) == 0;	// wtf?
		float from_right_sign = from_right ? 1.0f : -1.0f;

		float start_p_x = Camera.main.transform.position.x + from_right_sign * Utils.GetCameraSize().x / 2.0f;
		float start_p_y = Camera.main.transform.position.y + Utils.GetCameraSize().y / 2.0f;
		Vector3 p = new Vector3(start_p_x, start_p_y, 0.0f);
		const float min_start_v_x = 2.0f;
		const float max_start_v_x = 8.0f;
		float start_v_x = Random.Range(min_start_v_x, max_start_v_x);
		Vector3 v = new Vector3(start_v_x  * (-from_right_sign), 0.0f, 0.0f);
		GameObject go =	MonoBehaviour.Instantiate(Resources.Load("Prefabs/FallingBomb"),
		                                          p, Quaternion.identity) as GameObject;
		go.rigidbody2D.velocity = v;
		go.rigidbody2D.isKinematic = false;	// If isKinematic is enabled, Forces, collisions or joints will not affect the rigidbody anymore
		go.collider2D.isTrigger = true;
	}
}

