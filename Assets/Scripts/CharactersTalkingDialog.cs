using UnityEngine;
using System.Collections;
using UnityEditor;

public class CharactersTalkingDialog : MonoBehaviour {

	string m_DialogString;
	float m_ShowTime;

	public static GameObject Instantiate(Vector3 p) {
		GameObject go =	MonoBehaviour.Instantiate(Resources.Load("Prefabs/CharactersTalkingDialog"),
		                                          p, Quaternion.identity) as GameObject;
		SpriteRenderer sr = Utils.GetTheClassFromGO<SpriteRenderer>(go);
		sr.enabled = true;
		return go;
	}

	public void SetString(string str) {
		m_DialogString = str;
	}
	public void StartDialog() {
		StartCoroutine(ShowDialog(m_ShowTime));
	}

	void Start() {
		m_DialogString = "Hey! Glad to see you!";
		m_ShowTime = 5.0f;
	}

	void FixedUpdate() {

	}

	void OnGUI() {
		Vector3 p = Camera.main.WorldToScreenPoint(transform.position);
		// TODO: set dynamic size
		int size_x = 50;
		int size_y = 50;
		Rect r = new Rect(p.x - size_x / 2.0f, Screen.height - (p.y + size_y / 2.0f), size_x, size_y);

		GUIStyle style = new GUIStyle();
		style.normal.textColor = Color.black;
		GUI.Label(r, m_DialogString, style);
	}

	// IEnumerators ----------------------------------
	
	IEnumerator ShowDialog(float time) {
		float t = 0.0f;
		float rate = 1.0f/time;
		while (t < 1.0f) {
			t += Time.deltaTime * rate;
			yield return null;
		}
		Destroy(gameObject);
	}
}

