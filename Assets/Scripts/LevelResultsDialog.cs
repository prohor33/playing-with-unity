using UnityEngine;
using System.Collections;

public class LevelResultsDialog : MonoBehaviour {

	AudioClip m_StampAudio;

	GameObject m_PassedGO;
	Texture2D m_RestartIcon;
	Texture2D m_NextIcon;
	Vector3 m_RestartButtonsP;
	Vector3 m_NextButtonsP;
	GUISkin m_ButtonsSkin;

	System.Collections.Generic.List<GameObject> m_TextObjects = new System.Collections.Generic.List<GameObject>();

	public static LevelResultsDialog Instantiate() {
		Vector3 p = new Vector3();
		GameObject go =	MonoBehaviour.Instantiate(Resources.Load("Prefabs/LevelResultDialog"),
		                                          p, Quaternion.identity) as GameObject;
		SpriteRenderer sr = Utils.GetTheClassFromGO<SpriteRenderer>(go);
		sr.enabled = true;
		return Utils.GetTheClassFromGO<LevelResultsDialog>(go);
	}

	void Start () {
		print("Init");
		StartCoroutine(DecreasePassed(0.5f));
	}

	void Update () {

	}

	void Awake() {
		print("LevelResultsDialog Awake");
		LoadResources();
		Init();
	}
	
	void Init() {
		ScaleToTheScreen();
		InitGUIPos();

		SpriteRenderer sr = Utils.GetTheClassFromGO<SpriteRenderer>(gameObject);
		sr.enabled = false;

		m_PassedGO = Utils.GetChildGO(gameObject, "Passed");

		UpdateData();

		sr.enabled = true;
	}

	void InitGUIPos() {
		Vector3 cam_p = Camera.main.transform.position;
		cam_p.z = 0.0f;
		transform.position = cam_p;

		float shift_y = Utils.GetCameraSize().y / 24.0f;

		GameObject star_go = Utils.GetChildGO(gameObject, "Star1");
		SpriteRenderer sr = (SpriteRenderer)star_go.GetComponentInChildren(typeof(SpriteRenderer));
		Vector3 anchor_p = new Vector3(sr.bounds.min.x, sr.bounds.min.y - shift_y, 0.0f);
		m_RestartButtonsP = Camera.main.WorldToScreenPoint(anchor_p);

		star_go = Utils.GetChildGO(gameObject, "Star3");
		sr = (SpriteRenderer)star_go.GetComponentInChildren(typeof(SpriteRenderer));
		anchor_p = new Vector3(sr.bounds.min.x, sr.bounds.min.y - shift_y, 0.0f);
		m_NextButtonsP = Camera.main.WorldToScreenPoint(anchor_p);
	}

	void LoadResources() {
		m_StampAudio = Resources.Load("Audio/stamp") as AudioClip;
		m_RestartIcon = Resources.Load("replay_icon2") as Texture2D;
		m_NextIcon = Resources.Load("next_icon") as Texture2D;
		m_ButtonsSkin = Resources.Load("GUISkins/ResultsDialogButton") as GUISkin;
	}

	void ScaleToTheScreen() {
		SpriteRenderer sr = (SpriteRenderer)Utils.GetChildGO(gameObject, "Back").GetComponentInChildren(typeof(SpriteRenderer));
		Sprite sprite = sr.sprite;
		
		Camera cam = Camera.main;
		float height = 2f * cam.orthographicSize;
		float width = height * cam.aspect;
		
		float real_part;
		float scale = 1.0f;
		float offset_inside_image = 0.0f;
		float part = 0.8f;

		real_part = (sprite.textureRect.width - 2 * offset_inside_image) / sprite.textureRect.width;
		scale = width / sr.bounds.size.x / real_part * part;

		
		gameObject.transform.localScale = new Vector3(scale, scale, 1.0f);
	}

	void UpdateData() {
		PointKeeper pk = LevelController.control.m_PointKeeper;

		for (int i = 0; i < 3; i++) {
			GameObject stars1_go = Utils.GetChildGO(gameObject, "Star" + (i + 1).ToString());
			Utils.LoadSpriteIntoGO(pk.GetStarSpriteName(i), stars1_go);
		}

		DrawTextNearIcon("StopWatch", pk.TimeBonus.ToString());
		DrawTextNearIcon("Bomb", pk.BombsSaved.ToString() + "/" + pk.BombsWas.ToString());
		DrawTextNearIcon("Vase", pk.VaseSaved.ToString() + "/" + pk.VaseWas.ToString());
	}

	void DrawTextNearIcon(string icon_name, string text) {
		GameObject stopwatch_go = Utils.GetChildGO(gameObject, icon_name);
		SpriteRenderer sr = (SpriteRenderer)stopwatch_go.GetComponentInChildren(typeof(SpriteRenderer));
		Vector3 anchor_p = new Vector3(sr.bounds.max.x, stopwatch_go.transform.position.y, 0.0f);
		
		GameObject stop_watch_text = new GameObject();
		m_TextObjects.Add(stop_watch_text);
		stop_watch_text.name = icon_name + "GUIText";
		stop_watch_text.transform.position = Camera.main.WorldToViewportPoint(anchor_p);
		stop_watch_text.AddComponent<GUIText>();
		stop_watch_text.guiText.color = Color.black;
		stop_watch_text.guiText.fontSize = 15;
		stop_watch_text.guiText.fontStyle = FontStyle.Bold;
		stop_watch_text.guiText.text = text;
		stop_watch_text.guiText.anchor = TextAnchor.MiddleLeft;
		stop_watch_text.guiText.pixelOffset = new Vector2(-1, -1);
	}

	void DrawPointsText() {
		PointKeeper pk = LevelController.control.m_PointKeeper;
		GameObject stopwatch_go = Utils.GetChildGO(gameObject,"Star2");
		SpriteRenderer sr = (SpriteRenderer)stopwatch_go.GetComponentInChildren(typeof(SpriteRenderer));
		Vector3 anchor_p = new Vector3(sr.bounds.center.x, sr.bounds.max.y, 0.0f);
		
		GameObject score_text = new GameObject();
		m_TextObjects.Add(score_text);
		score_text.name = "ScoreGUIText";
		score_text.transform.position = Camera.main.WorldToViewportPoint(anchor_p);
		score_text.AddComponent<GUIText>();
		score_text.guiText.color = Color.black;
		score_text.guiText.fontSize = 26;
		score_text.guiText.fontStyle = FontStyle.Bold;
		score_text.guiText.text = "Score: " + pk.Points.ToString();
		score_text.guiText.anchor = TextAnchor.LowerCenter;
		score_text.guiText.pixelOffset = new Vector2(0, 10);
	}

	void DecreaseSize(float progress) {
		float max_scale = 5.0f;
		float min_scale = 1.0f;
		float scale = Mathf.Lerp(max_scale, min_scale, progress);
		m_PassedGO.transform.localScale = new Vector3(scale, scale, scale);
	}

	void HidePassedSprite() {
		SpriteRenderer sr = (SpriteRenderer)m_PassedGO.GetComponentInChildren(typeof(SpriteRenderer));
		sr.enabled = false;

		DrawPointsText();
	}

	void OnGUI () {
		Vector2 menu_btn_size = new Vector2(Screen.height / 11, Screen.height / 15);
		Rect menu_btn_rect = new Rect(m_NextButtonsP.x,
		                              Screen.height - m_NextButtonsP.y,
		                              menu_btn_size.x, menu_btn_size.y);

		GUI.skin = m_ButtonsSkin;

		// Next button
		if(GUI.Button(menu_btn_rect, m_NextIcon)) {
			Finish();
			Application.LoadLevel(Utils.dungeon_level);
		}
		
		Rect re_btn_rect = new Rect(m_RestartButtonsP.x,
		                            Screen.height - m_RestartButtonsP.y,
		                            menu_btn_size.x, menu_btn_size.y);
		// Restart button
		if(GUI.Button(re_btn_rect, m_RestartIcon)) {
			Finish();
			LevelController.control.RestartGame();
		}
	}

	void Finish() {
		foreach (GameObject go in m_TextObjects) {
			Destroy(go);
		}
		Destroy(gameObject);
	}

	// Enumerators

	IEnumerator DecreasePassed(float time) {
		SpriteRenderer sr = (SpriteRenderer)m_PassedGO.GetComponentInChildren(typeof(SpriteRenderer));
		sr.enabled = true;
		float t = 0.0f;
		float rate = 1.0f / time;
		while (t < 1.0f) {
			t += Time.deltaTime * rate;
			DecreaseSize(t);
			yield return null;
		}
		// Play sound
		audio.PlayOneShot(m_StampAudio, 1.0f);
		const float passed_show_time = 2.0f;
		Invoke("HidePassedSprite", passed_show_time);
	}
}

