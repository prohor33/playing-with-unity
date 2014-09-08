using UnityEngine;
using System.Collections;

public class LevelController : MonoBehaviour {
	
	public GameObject m_GameOverText;
	public GameObject m_Monster;
	public MonsterController m_MonsterContr;
	public GameObject m_FallingObject;	
	public GameObject m_FPSText;
	public int m_Level = 1;
	public Texture2D m_RestartIcon;

	public static LevelController control;

	public const float m_MaxCameraPosition = 20.0f;

	public PointKeeper m_PointKeeper;
	public ObjectConveyor m_LeftConveyor;
	public ObjectConveyor m_RightConveyor;

	enum GameState {GameFinished, Game};
	GameState m_GameState;
	Spawner m_Spawner = new Spawner();
	float m_LevelStartTime;

	public void GameOver() {
		GameContr.control.PassLevel();
//		SetGameOverText("You Won!");
		m_GameState = GameState.GameFinished;

		float time_gone = Time.time - m_LevelStartTime;
		LevelController.control.m_PointKeeper.SetTimeGone((int)time_gone);

		m_LeftConveyor.DestroyFallingObjects();
		m_RightConveyor.DestroyFallingObjects();
		
		StartLevelResultsDialog();
	}

	public void RestartGame() {
		m_LeftConveyor.DestroyFallingObjects();
		m_RightConveyor.DestroyFallingObjects();

		StartNewGame();
	}

	public ObjectConveyor GetConveyor(bool right) {
		if (right)
			return m_RightConveyor;
		else
			return m_LeftConveyor;
	}

	public int GetLevel() {
		return m_Level;
	}

	// Private members --------------------------

	void Init() {
		m_PointKeeper = new PointKeeper();
	}

	void StartNewGame() {
		m_MonsterContr = (MonsterController)m_Monster.GetComponent(typeof(MonsterController));
		SetGameOverText("");
		m_GameState = GameState.Game;
		m_LevelStartTime = Time.time;
		m_MonsterContr.Restart();
		InitConveyoers();
		InitSpawner();
		Init();
//		GameOver();
	}

	// Use this for initialization
	void Start () {
		StartNewGame();
	}

	void Awake() {
		if (control == null) {
			control = this;
		} else if (control != this) {
			Destroy(gameObject);
		}
	}

	// Update is called once per frame
	void Update () {
		HandleInput();
		CountFPS();
	}

	void OnGUI () {
		Vector2 menu_btn_size = new Vector2(Screen.height / 11, Screen.height / 15);
		int menu_btn_shift = Screen.height / 40;
		Vector2 menu_btn_pos = new Vector2(Screen.width - menu_btn_shift, menu_btn_shift);
		Rect menu_btn_rect = new Rect(menu_btn_pos.x - menu_btn_size.x,
		                         menu_btn_pos.y, menu_btn_size.x, menu_btn_size.y);

		if(GUI.Button(menu_btn_rect, "Menu")) {
			Application.LoadLevel(Utils.menu_level);
		}

		Rect re_btn_rect = new Rect(menu_btn_shift,
		                         menu_btn_pos.y, menu_btn_size.x, menu_btn_size.y);
		if(GUI.Button(re_btn_rect, m_RestartIcon)) {
			RestartGame();
		}
	}

	void FixedUpdate() {
		UpdateConveyor();
		UpdateSpawner();
	}

	void SetGameOverText(string text) {
		m_GameOverText.guiText.text = text;
	}

	void UpdateConveyor() {
		m_RightConveyor.FixedUpdate();
		m_LeftConveyor.FixedUpdate();
	}

	void UpdateSpawner() {
		m_Spawner.FixedUpdate();
	}

	void InitConveyoers() {
		m_LeftConveyor = new ObjectConveyor(false);
		m_RightConveyor  = new ObjectConveyor(true);
		m_LeftConveyor.m_LevelContr = this;
		m_LeftConveyor.Restart();
		m_RightConveyor.m_LevelContr = this;
		m_RightConveyor.Restart();
	}

	void InitSpawner() {
		m_Spawner.m_LevelContr = this;
		m_Spawner.Init();
	}

	void HandleInput() {
		if (Input.GetKey(KeyCode.N)) {
			RestartGame();
		}
		if (Input.GetKey(KeyCode.G)) {
			GameOver();
		}
		if (Input.GetKey(KeyCode.F)) {
			m_MonsterContr.GetTheHead(true).Fall();
		}
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit();
		}
		// Controllers for debug
		float scroll = -0.4f;
		if(Input.GetKey(KeyCode.LeftArrow))
			m_LeftConveyor.Scroll(scroll);
		if(Input.GetKey(KeyCode.RightArrow))
			m_RightConveyor.Scroll(scroll);
		HandleTouches();
	}

	void HandleTouches() {
		if (InputController.Update())
			return;

		for (int i = 0; i < Input.touchCount; i++) {
			Touch touch = Input.GetTouch(i);
			if (touch.phase != TouchPhase.Moved)
				continue;
			Vector2 touch_p = touch.position;
			Vector2 touch_delta_p = touch.deltaPosition;
			float coef_scrolling = 2f * Camera.main.orthographicSize / Screen.height;
			float scroll = touch_delta_p.y * coef_scrolling;
			bool is_right_screen_part = touch_p.x > Screen.width / 2.0f;
			if (is_right_screen_part)
				m_RightConveyor.Scroll(scroll);
			else
				m_LeftConveyor.Scroll(scroll);
		}		
	}

	float m_FPSChangedTime;
	void CountFPS() {
		float fps = 1.0f / Time.deltaTime;
		m_FPSChangedTime += Time.deltaTime;
		if (m_FPSChangedTime > 0.25f) {
			m_FPSText.guiText.text = ((int)fps).ToString();
			m_FPSChangedTime = 0.0f;
		}
	}

	void StartLevelResultsDialog() {
		LevelResultsDialog lrd = LevelResultsDialog.Instantiate();
	}
}
