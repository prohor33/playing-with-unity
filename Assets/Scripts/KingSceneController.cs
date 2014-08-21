using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KingSceneController : MonoBehaviour {

	GameObject m_Hero;
	GameObject m_King;
	Animator m_HeroAnimator;

	enum Direction { NoDirection = -2, Stationary = -1, Up = 0, Right = 1, Down = 2, Left = 3 };
	Direction m_HeroDirection;
	Direction m_OldHeroDirection;
	Vector3 m_KingTargetP;
	Vector3 m_ExitTargetP;

	enum Task { GoToKing = 0, Talk, GoOut };
	Task m_CurrentTask;

//	const float m_ReplicaTimeLength = 4;
	const float m_ReplicaTimeLength = 0.2f;	// for debug

	public class Replica {

		public Replica(Person person, string phrase) {
			m_Author = person;
			m_Phrase = phrase;
		}

		public enum Person { King = 0, Hero };
		public Person m_Author;
		public string m_Phrase;

		public override string ToString()
		{
			return m_Author + ": " + m_Phrase;
		}
	};
	List<Replica> m_TalkingDialog = new List<Replica>();
	int m_CurrentReplicaNumber;

	void Start() {
		m_Hero = Utils.FindActiveGO("Hero");
		m_King = Utils.FindActiveGO("TheKing");

		m_HeroAnimator = Utils.GetTheClassFromGO<Animator>(m_Hero);

		Init();
	}
	
	void Update() {

	}

	void FixedUpdate() {
		UpdateHeroDirection();
	}

	void Init() {
		m_HeroDirection = Direction.Left;
		m_OldHeroDirection = Direction.Stationary;
		m_KingTargetP = m_King.transform.position + new Vector3(6.0f, 0.0f, 0.0f);
		m_ExitTargetP = new Vector3(8.0f, m_King.transform.position.y, 0.0f);
		m_Hero.transform.position = m_ExitTargetP + new Vector3(5.0f, 0.0f, 0.0f);
		m_CurrentTask = Task.GoToKing;
		const float scale_hero = 1.9f;
		Utils.ScaleSpriteInGO(m_Hero, scale_hero);

		LoadKingSceneBackground();
		InitTalkingDialog();

		StartMovingHero();
	}

	void InitTalkingDialog() {
		m_TalkingDialog.Add(new Replica(Replica.Person.King, "Hi!"));
		m_TalkingDialog.Add(new Replica(Replica.Person.Hero, "Hello!"));
		m_TalkingDialog.Add(new Replica(Replica.Person.King, "How are you?"));
		m_TalkingDialog.Add(new Replica(Replica.Person.Hero, "I'm fine, thanks"));
		m_TalkingDialog.Add(new Replica(Replica.Person.Hero, "You?"));
	}

	void UpdateHeroDirection() {
		Direction new_dir = Direction.NoDirection;
		if (m_HeroDirection != m_OldHeroDirection) {
			AnimatorStateInfo an_st_info = m_HeroAnimator.GetCurrentAnimatorStateInfo(0);
			if (an_st_info.IsName(m_OldHeroDirection.ToString())) {	// if previous direction already set up
				new_dir = m_HeroDirection;
				m_OldHeroDirection = m_HeroDirection;
			}
		}
		m_HeroAnimator.SetInteger("m_Direction", (int)new_dir);
	}

	void StartMovingHero(bool to_the_king = true) {
		Vector3 target_p = to_the_king ? m_KingTargetP : m_ExitTargetP;

//		const float speed = 4.0f;
		const float speed = 40.0f;	// for debug!
		float time = (target_p - m_Hero.transform.position).magnitude / speed;
		StartCoroutine(MoveHero(m_Hero.transform, m_Hero.transform.position, target_p, time));
	}

	void EndMoving() {
		if (m_CurrentTask == Task.GoToKing)
			StartTalking();
		if (m_CurrentTask == Task.GoOut)
			GoToLevels();
	}

	void StartTalking() {
		m_CurrentTask = Task.Talk;
		m_CurrentReplicaNumber = 0;

		float seconds = 1.0f;
		for (int i = 0; i < m_TalkingDialog.Count; i++) {
			Invoke(m_TalkingDialog[i].m_Author.ToString() + "Talk", seconds);

			seconds += m_ReplicaTimeLength;
		}
		Invoke("GoOut", seconds);
	}

	void KingTalk() {
		string str = m_TalkingDialog[m_CurrentReplicaNumber].m_Phrase;
		SpriteRenderer sprite_rend = Utils.GetTheClassFromGO<SpriteRenderer>(m_King);
		Vector3 delta_dialog_p = new Vector3(sprite_rend.bounds.size.x / 4.0f, sprite_rend.bounds.size.y / 3.0f, 0.0f);
		CharactersTalkingDialog ctd = CharactersTalkingDialog.Instantiate(m_King.transform.position + delta_dialog_p);
		ctd.m_DialogString = str;
		ctd.m_IsRight = false;
		ctd.m_ShowTime = m_ReplicaTimeLength;
		ctd.StartDialog();
		m_CurrentReplicaNumber++;
	}
	void HeroTalk() {
		string str = m_TalkingDialog[m_CurrentReplicaNumber].m_Phrase;
		SpriteRenderer sprite_rend = Utils.GetTheClassFromGO<SpriteRenderer>(m_Hero);
		Vector3 delta_dialog_p = new Vector3(-sprite_rend.bounds.size.x / 4.0f, sprite_rend.bounds.size.y / 3.0f, 0.0f);
		CharactersTalkingDialog ctd = CharactersTalkingDialog.Instantiate(m_King.transform.position + delta_dialog_p);
		ctd.m_DialogString = str;
		ctd.m_IsRight = true;
		ctd.m_ShowTime = m_ReplicaTimeLength;
		ctd.StartDialog();
		m_CurrentReplicaNumber++;
	}

	void GoOut() {
		m_CurrentTask = Task.GoOut;
		StartMovingHero(false);
//		const float go_out_time = 5.0f;
		const float go_out_time = 1.0f; // for debug
		Invoke("GoToLevels", go_out_time);
	}

	void GoToLevels() {
		Application.LoadLevel(Utils.level_select_level);
	}

	void LoadKingSceneBackground() {
		int indent_to_actual_image_x = 5;
		int indent_to_actual_image_y = 105;
		GameObject go = Utils.LoadSpriteIntoGO("king_scene_back", "Background");
		Utils.AttachSriteToCameraInGO(go, KeyCode.X, 1.0f, indent_to_actual_image_x);
		
		SpriteRenderer sr = (SpriteRenderer)go.GetComponent(typeof(SpriteRenderer));
		Sprite sprite = sr.sprite;
		float image_pixels_to_unit = sr.bounds.size.y / sprite.textureRect.height;
		
//		int indent_y = (int)(Screen.height / 7.0f);	// special indent for bar on the top
		int indent_y = 0;

		bool align_top = false;
		float shift_y = Camera.main.orthographicSize - sr.bounds.size.y / 2.0f
			+ indent_to_actual_image_y * image_pixels_to_unit;
		shift_y *= align_top ? 1.0f : -1.0f;
		float pos_y = Camera.main.transform.position.y + shift_y - Utils.ScreenPixelsToUnit(indent_y);
		go.transform.position = new Vector3(0.0f, pos_y, 0.0f);
	}

	// IEnumerators ----------------------------------
	
	IEnumerator MoveHero(Transform trans, Vector3 start_p, Vector3 end_p, float time) {
		float t = 0.0f;
		float rate = 1.0f/time;
		while (t < 1.0f) {
			t += Time.deltaTime * rate;
			trans.position = Vector3.Lerp(start_p, end_p, t);
			yield return null;
		}
		m_HeroDirection = Direction.Stationary;
		EndMoving();
	}
}

