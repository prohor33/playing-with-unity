using UnityEngine;
using System.Collections;

public class FallingBomb : MonoBehaviour {

	public void StopFalling() {
		rigidbody2D.velocity = Vector3.zero;
		rigidbody2D.isKinematic = true;

		const float heat_up_time = 20.0f;
		StartCoroutine(HeatingUp(heat_up_time));
	}

	void Start () {	
	}

	void FixedUpdate () {
		Vector3 screen_bottom_falling_p = new Vector3(0.0f, 0.15f, 0.0f);
		float min_falling_y = Camera.main.ViewportToWorldPoint(screen_bottom_falling_p).y;
		if (transform.position.y < min_falling_y)
			StopFalling();
	}

	void BlowUp() {
		const float blowing_up_time = 0.1f;
		StartCoroutine(BlowingUp(blowing_up_time));

		const float impact_radius = 5.0f;
		MonsterController mc = Utils.GetTheClassFromGO<MonsterController>("Monster");
		mc.m_LeftHeadContr.BlowUpBomb(transform.position, impact_radius);
		mc.m_RightHeadContr.BlowUpBomb(transform.position, impact_radius);
	}

	// TODO: more smart blowing up?
	void IncreaseSize(float progress /* 0.0 to 1.0 */) {
		const float start_scale = 1.0f;
		const float end_scale = 3.0f;
		float scale = Mathf.Lerp(start_scale, end_scale, progress);
		GameObject vfx_go = Utils.GetChildGO(gameObject, "VFX");
		vfx_go.transform.localScale = Vector3.one * scale;
	}

	// IEnumerators ----------------------------------
	
	IEnumerator HeatingUp(float time) {
		float t = 0.0f;
		float rate = 1.0f / time;
		while (t < 1.0f) {
			t += Time.deltaTime * rate;
			yield return null;
		}
		BlowUp();
	}

	IEnumerator BlowingUp(float time) {
		float t = 0.0f;
		float rate = 1.0f / time;
		while (t < 1.0f) {
			t += Time.deltaTime * rate;
			IncreaseSize(t);
			yield return null;
		}
		Destroy(gameObject);
	}
}

