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

		int seconds = 0;
		for (int i = 0; i < m_TalkingDialog.Count; i++) {
			Invoke(m_TalkingDialog[i].m_Author.ToString + "Talk", seconds += 5);
		}

//		Invoke("HeroTalk", 0);
//		Invoke("KingTalk", 5);
//		ctd.m_DialogString = "I'm glad to see you!\n I'm waiting for you so long";

	}

	void KingTalk() {
		string str = "bye";
		SpriteRenderer sprite_rend = Utils.GetTheClassFromGO<SpriteRenderer>(m_King);
		Vector3 delta_dialog_p = new Vector3(sprite_rend.bounds.size.x / 4.0f, sprite_rend.bounds.size.y / 3.0f, 0.0f);
		CharactersTalkingDialog ctd = CharactersTalkingDialog.Instantiate(m_King.transform.position + delta_dialog_p);
		ctd.m_DialogString = str;
		ctd.m_IsRight = false;
		ctd.StartDialog();
	}
	void HeroTalk() {
		string str = "hey";
		SpriteRenderer sprite_rend = Utils.GetTheClassFromGO<SpriteRenderer>(m_Hero);
		Vector3 delta_dialog_p = new Vector3(-sprite_rend.bounds.size.x / 4.0f, sprite_rend.bounds.size.y / 3.0f, 0.0f);
		CharactersTalkingDialog ctd = CharactersTalkingDialog.Instantiate(m_King.transform.position + delta_dialog_p);
		ctd.m_DialogString = str;
		ctd.m_IsRight = true;
		ctd.StartDialog();
	}

	void GoToLevels() {
		//	TODO: to implement
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

