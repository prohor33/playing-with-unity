using UnityEngine;
using System.Collections;

public class CharactersTalkingDialog : MonoBehaviour {

	public string m_DialogString;
	public float m_ShowTime;
	public bool m_IsRight = true;


	Rect m_TextRect;

	public static CharactersTalkingDialog Instantiate(Vector3 p) {
		GameObject go =	MonoBehaviour.Instantiate(Resources.Load("Prefabs/CharactersTalkingDialog"),
		                                          p, Quaternion.identity) as GameObject;
		SpriteRenderer sr = Utils.GetTheClassFromGO<SpriteRenderer>(go);
		sr.enabled = true;
		return Utils.GetTheClassFromGO<CharactersTalkingDialog>(go);
	}

	// call after all settings is set up
	public void StartDialog() {
		Setup();
		StartCoroutine(ShowDialog(m_ShowTime));
	}

	void Awake() {
		Init();
	}

	void Init() {
		m_DialogString = "Hey";
		m_ShowTime = 5.0f;
//		m_ShowTime = 50.0f;	// for debug!
		m_IsRight = true;
	}

	void Setup() {		
		SpriteRenderer sprite_rend = Utils.GetTheClassFromGO<SpriteRenderer>(gameObject);
		Vector3 delta_im = (sprite_rend.bounds.max - sprite_rend.bounds.min) / 2.0f;
		Vector3 p_min = Camera.main.WorldToScreenPoint(sprite_rend.bounds.min + delta_im);
		Vector3 p_max = Camera.main.WorldToScreenPoint(sprite_rend.bounds.max + delta_im);
		transform.position += delta_im;
		const float real_dialog_coef = 2.3f / 5.0f;
		p_max.y += (p_max.y - p_min.y) * real_dialog_coef;
		p_min.y = Screen.height - p_min.y;
		p_max.y = Screen.height - p_max.y;
		
		m_TextRect = new Rect(p_min.x, p_max.y, p_max.x - p_min.x, p_min.y - p_max.y);

		if (!m_IsRight) {
			Vector3 scale = gameObject.transform.localScale;
			scale.x *= -1.0f;
			gameObject.transform.localScale = scale;
		}
	}

	void FixedUpdate() {

	}

	void OnGUI() {
		GUIStyle style = new GUIStyle();
		style.normal.textColor = Color.black;
		style.alignment = TextAnchor.MiddleCenter;
		style.clipping = TextClipping.Clip;
		style.stretchHeight = true;
//		Font myFont = (Font)Resources.Load("Fonts/comic", typeof(Font));
		//style.font = myFont;
		style.fontSize = 12;
		style.fontStyle = FontStyle.Bold;
		GUI.Label(m_TextRect, m_DialogString, style);
	}

	// IEnumerators ----------------------------------
	
	IEnumerator ShowDialog(float time) {
		float t = 0.0f;
		float rate = 1.0f / time;
		while (t < 1.0f) {
			t += Time.deltaTime * rate;
			yield return null;
		}
		Destroy(gameObject);
	}
}

