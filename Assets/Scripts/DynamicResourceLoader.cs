using UnityEngine;
using System.Collections;

public class DynamicResourceLoader : MonoBehaviour {

	LevelController m_LevelController;

	void Start() {
		GameObject go = GameObject.Find("LevelController");
		m_LevelController = go.GetComponentInChildren(typeof(LevelController)) as LevelController;
		if (!m_LevelController)
			Debug.LogError("Can't find LevelController");
	}

	static void LoadLevel(int level) {


		GameObject obj = GameObject.FindWithTag("Background");
		if (!obj) {
			Debug.LogError("Can't find Background object");
			return;
		}

		SpriteRenderer sprite_rend = obj.GetComponentInChildren(typeof(SpriteRenderer)) as SpriteRenderer;
		string sprite_name = GameContr.control.lvl_settings.m_BackgroundSpriteNames[level];
		Sprite sprite = Resources.Load<Sprite>(sprite_name);
		if (!sprite)
			Debug.LogWarning("Can't load " + sprite_name);
		sprite_rend.sprite = sprite;
		// Scale sprite to fit with width?
//		float scale = Utils.GetCameraSize().x / sprite_rend.bounds.size.x;
//		Utils.ScaleSpriteInGO(obj, scale);

//		float delta_y = sprite_rend.bounds.size.y
	}

	void OnLevelWasLoaded() {
		LoadLevel(GameContr.control.m_LevelPlaying);
	}
}

