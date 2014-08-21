using UnityEngine;
using System.Collections;

public class DynamicLevelResourceLoader : MonoBehaviour {

	LevelController m_LevelController;

	void Start() {
		m_LevelController = Utils.GetTheClassFromGO<LevelController>("LevelController");
	}

	void LoadLevelResources(int level) {
		LoadLevelBackground(level);
	}

	void LoadLevelBackground(int level) {
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

		// Scale sprite to fit with height
		float new_size_y = Utils.GetCameraSize().y + (LevelController.m_MaxCameraPosition - Utils.GetCameraPos().y);
		float scale = new_size_y / sprite_rend.bounds.size.y;
		Utils.ScaleSpriteInGO(obj, scale);
		
		float new_pos_y = new_size_y / 2.0f + Utils.GetCameraPos().y - Utils.GetCameraSize().y / 2.0f;
		sprite_rend.transform.position = new Vector3(0.0f, new_pos_y, 0.0f);
	}

	// Starting point
	void OnLevelWasLoaded() {
		LoadLevelResources(GameContr.control.m_LevelPlaying);
	}
}

