using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelsSceneController : MonoBehaviour {

	public GUISkin m_LevelButtonSkin;
	public GUISkin m_BackButtonSkin;

	// private members -------------------------------------

	GameObject m_Dungeon;
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
	}
	
	void Update () {
		HandleInputs();
	}
	
	void OnGUI () {
		Rect btn_back_pos = new Rect(0.0f, 0.0f, Screen.width / 3, Screen.width / 10);
		RectOffset btn_back_offset = new RectOffset((int)-Screen.width / 10, (int)Screen.width / 10, (int)-Screen.height / 40, (int)Screen.height / 40);
		btn_back_pos = btn_back_offset.Add(btn_back_pos);
		if(GUI.Button(btn_back_pos, "Back to menu", m_BackButtonSkin.button)) {
			Application.LoadLevel(0);
		}
	}

	void LoadLevelsDoors() {

		Vector3 start_pos = new Vector3(0.0f, 6.15f, 0.0f);
		Vector3 delta_y_pos = new Vector3(0.0f, -4.18f, 0.0f);
		float dungeon_scale = m_Dungeon.transform.localScale.x;
		start_pos *= dungeon_scale;
		start_pos += m_Dungeon.transform.position;
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
		int indent_to_actual_image_x = 242;
		int indent_to_actual_image_y = 176;
		GameObject go = Utils.LoadSpriteIntoGO(game_obj_name.ToLower(), game_obj_name);
		Utils.AttachSriteToCameraInGO(go, KeyCode.X, 1.0f, indent_to_actual_image_x);

		SpriteRenderer sr = (SpriteRenderer)go.GetComponent(typeof(SpriteRenderer));
		Sprite sprite = sr.sprite;
		float image_pixels_to_unit = sr.bounds.size.y / sprite.textureRect.height;

		int indent_y = (int)(Screen.height / 7.0f);	// special indent for bar on the top
//		int indent_y = 0;

		float pos_y = Camera.main.transform.position.y + Camera.main.orthographicSize - sr.bounds.size.y / 2.0f
			+ indent_to_actual_image_y * image_pixels_to_unit - Utils.ScreenPixelsToUnit(indent_y);
		go.transform.position = new Vector3(0.0f, pos_y, 0.0f);

		m_Dungeon = go;
	}

	void InitHero() {
		GameObject go = (GameObject)GameObject.Find("Hero");
		if (!go) {
			Debug.LogError("Cant' find hero");
		}
		m_Hero = (Hero)go.GetComponent(typeof(Hero));
		m_Hero.Init(m_Dungeon.transform.localScale.x, m_Dungeon.transform.position, this);
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
}



