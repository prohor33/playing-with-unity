using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DungeonSceneController : MonoBehaviour {

	public GUISkin m_LevelButtonSkin;
	public GUISkin m_BackButtonSkin;

	// private members -------------------------------------

	const bool m_IsDaknessEnable = true;

	GameObject m_DungeonBack;
	GameObject m_Darkness;
	Hero m_Hero;
	Vector3[] m_LevelDoorsPos;

	public void GetLevelDoorsPos(out Vector3[] level_door_pos) {
		level_door_pos = m_LevelDoorsPos;
	}

	// Use this for initialization
	void Start() {
		LoadDungeonBackground();
		LoadLevelsDoors();
		InitHero();
		LoadDarkness();
	}
	
	void Update () {
		HandleInputs();
	}

	void FixedUpdate() {
		UpdateCamera();
		UpdateDarkness();
	}

	void LoadLevelsDoors() {

		Vector3 start_pos = new Vector3(0.0f, 22.86f, 0.0f);
		Vector3 delta_y_pos = new Vector3(0.0f, -4.2f, 0.0f);
		float dungeon_scale = m_DungeonBack.transform.localScale.x;
		start_pos *= dungeon_scale;
		start_pos += m_DungeonBack.transform.position;
		delta_y_pos *= dungeon_scale;

		int doors_on_the_floor = 1;
		Vector3 pos = start_pos;
		Vector3 start_floor_pos = pos;
		int length = GameContr.control.m_LevelsCleared.Length;
		m_LevelDoorsPos = new Vector3[length];
		
		for (int i = 0; i < length; i++) {

			if (i % doors_on_the_floor == 0) {
				pos = start_floor_pos;
				start_floor_pos += delta_y_pos;
			} else {
//				pos += delta_x_pos;
			}
			
			Vector3 real_world_pos = pos;
			
			
			string spr_name = "";
			switch (GameContr.control.m_LevelsCleared[i]) {
			case GameContr.LevelState.Blocked:
				spr_name = "level_brumed";
				break;
			case GameContr.LevelState.Cleared:
				spr_name = "level_passed";
				break;
			case GameContr.LevelState.Mist:
				spr_name = "level_brumed";
				break;
			default:
				Debug.LogError("wrong level type");
				break;
			}
			
			GameObject go = Utils.CreateGOWithSprite(spr_name);
			Utils.ScaleSpriteInGO(go, dungeon_scale);
			go.transform.position = real_world_pos;
			m_LevelDoorsPos[i] = real_world_pos;

			if (GameContr.control.m_LevelsCleared[i] == GameContr.LevelState.Blocked) {
				// Load lock
				GameObject lock_go = Utils.CreateGOWithSprite("lock");
				const float tmp_lock_scale = 1.4f;
				Utils.ScaleSpriteInGO(lock_go, dungeon_scale * tmp_lock_scale);
				Vector3 p_lock_shift = new Vector3(0.8f, -0.3f, 0.0f);
				lock_go.transform.position = real_world_pos + p_lock_shift;
				SpriteRenderer spr_renderer = (SpriteRenderer)lock_go.GetComponentInChildren(typeof(SpriteRenderer));
				if (!spr_renderer)
					Debug.LogError("there is no sprite renderer");
				spr_renderer.sortingOrder = 1;
			}
		}
	}

	void LoadDungeonBackground() {
		string game_obj_name = "Dungeon";
		Utils.LoadBackground(game_obj_name, game_obj_name.ToLower(), new Vector2(242, 0), true);
		m_DungeonBack = GameObject.Find(game_obj_name);
	}

	void LoadDarkness() {
		GameObject go = GameObject.Find("Darkness");
		if (!m_IsDaknessEnable) {
			Destroy(go);
			return;
		}
		go.transform.position = Camera.main.transform.position;
		go.transform.localScale = m_Hero.transform.localScale;
		m_Darkness = go;
	}

	void InitHero() {
		GameObject go = (GameObject)GameObject.Find("Hero");
		if (!go) {
			Debug.LogError("Cant' find hero");
		}
		m_Hero = (Hero)go.GetComponent(typeof(Hero));
		m_Hero.Init(m_DungeonBack.transform.localScale.x, m_DungeonBack.transform.position, this);
	}

	void HandleInputs() {
		// left click
		if (Input.GetMouseButtonDown(0)) {
			if (m_Hero) {
				m_Hero.SetTarget(Camera.main.ScreenToWorldPoint(Input.mousePosition));
			}
		}
		HandleTouches();
	}

	void HandleTouches() {
		if (Input.touchCount == 0)
			return;
		Touch touch = Input.GetTouch(0);
		Vector2 pos = touch.position;
		m_Hero.SetTarget(Camera.main.ScreenToWorldPoint(pos));
	}

	void UpdateCamera() {
		Vector3 cam_p = Camera.main.transform.position;
		cam_p.y = m_Hero.transform.position.y;
		Camera.main.transform.position = cam_p;
	}

	void UpdateDarkness() {
		if (!m_IsDaknessEnable)
			return;
		m_Darkness.transform.position = m_Hero.transform.position;
	}

	void OnGUI () {
		Rect btn_back_pos = new Rect(0.0f, 0.0f, Screen.width / 3, Screen.width / 10);
		RectOffset btn_back_offset = new RectOffset((int)-Screen.width / 10, (int)Screen.width / 10, (int)-Screen.height / 40, (int)Screen.height / 40);
		btn_back_pos = btn_back_offset.Add(btn_back_pos);
		if(GUI.Button(btn_back_pos, "Back to menu", m_BackButtonSkin.button)) {
			Application.LoadLevel(Utils.menu_level);
		}
	}
}



