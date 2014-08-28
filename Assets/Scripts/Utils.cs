using UnityEngine;
using System.Collections;

public class Utils {

	public static int menu_level = 0;
	public static int king_level = 1;
	public static int level_select_level = 2;
	public static int monster_level = 3;

	public static Sprite LoadSprite(string spr_name) {
		Sprite sprite = Resources.Load<Sprite>(spr_name);
		if (!sprite)
			Debug.LogError("Can't load " + spr_name);
		return sprite;
	}

	public static Sprite GetSpriteFromGO(GameObject go) {
		SpriteRenderer sr = (SpriteRenderer)go.GetComponent(typeof(SpriteRenderer));
		if (!sr) {
			Debug.LogError("Can't find SpriteRenderer");
		}
		return sr.sprite;
	}
	public static Sprite GetSpriteFromGO(string go_name) {
		return GetSpriteFromGO(GameObject.Find(go_name));
	}

	public static GameObject CreateGOWithSprite(string name) {
		Sprite sprite = Resources.Load<Sprite>(name.ToLower());
		if (!sprite)
			Debug.LogWarning("Can't load " + name.ToLower());
		
		GameObject go = new GameObject();
		go.name = name;
		go.AddComponent("SpriteRenderer");
		SpriteRenderer sr = (SpriteRenderer)go.GetComponent(typeof(SpriteRenderer));
		if (!sr) {
			Debug.LogError("Can't find SpriteRenderer");
		}
		sr.sprite = sprite;
		return go;
	}

	public static GameObject LoadSpriteIntoGO(string spr_name, string game_obj_name) {
		Sprite sprite = LoadSprite(spr_name);
		
		GameObject go = GameObject.Find(game_obj_name);
		if (!go) {
			Debug.LogError("Can't find " + game_obj_name);
		}

		SpriteRenderer sr = (SpriteRenderer)go.GetComponent(typeof(SpriteRenderer));
		if (!sr) {
			Debug.LogError("Can't find SpriteRenderer");
		}
		sr.sprite = sprite;
		return go;
	}

	public static float AttachSriteToCameraInGO(GameObject go, KeyCode axis, float part = 1.0f, int offset_inside_image = 0) {
		
		SpriteRenderer sr = (SpriteRenderer)go.GetComponent(typeof(SpriteRenderer));
		if (!sr) {
			Debug.LogError("Can't find SpriteRenderer");
		}

		Sprite sprite = sr.sprite;

		Camera cam = Camera.main;
		float height = 2f * cam.orthographicSize;
		float width = height * cam.aspect;

		float real_part;
		float scale = 1.0f;

		if (axis == KeyCode.X) {
			real_part = (sprite.textureRect.width - 2 * offset_inside_image) / sprite.textureRect.width;
			scale = width / sr.bounds.size.x / real_part * part;
		} else if (axis == KeyCode.Y) {
			real_part = (sprite.textureRect.height - 2 * offset_inside_image) / sprite.textureRect.height;
			scale = height / sr.bounds.size.y / real_part * part;
		} else {
			Debug.LogError("Wrong axis");
		}

		go.transform.localScale = new Vector3(scale, scale, 1.0f);
		return scale;
	}

	public static void ScaleSpriteInGO(GameObject go, float scale) {
		go.transform.localScale = new Vector3(scale, scale, 1.0f);
	}

	public static float ScreenPixelsToUnitX(float x) {
		float ratio = GetCameraSize().x / Camera.main.pixelWidth;
		return x * ratio;
	}
	public static float UnitToScreenPixelsX(float x) {
		float ratio = GetCameraSize().x / Camera.main.pixelWidth;
		return x / ratio;
	}
	public static float ScreenPixelsToUnitY(float y) {
		float ratio = GetCameraSize().y / Camera.main.pixelHeight;
		return y * ratio;
	}
	public static float UnitToScreenPixelsY(float y) {
		float ratio = GetCameraSize().y / Camera.main.pixelHeight;
		return y / ratio;
	}

	public static Vector2 ScreenDeltaToWorld(Vector2 v) {
		return new Vector2(ScreenPixelsToUnitX(v.x), ScreenPixelsToUnitY(v.y));
	}

	public static Vector2 GetCameraSize() {
		Camera cam = Camera.main;
		float height = 2f * cam.orthographicSize;
		float width = height * cam.aspect;	// The aspect ratio (width divided by height)
		return new Vector2(width, height);
	}
	public static Vector2 GetCameraPos() {
		return Camera.main.transform.position;
	}

	public static T GetTheClassFromGO<T>(string obj_name) {
		GameObject go = GameObject.Find(obj_name);
		return GetTheClassFromGO<T>(go);
	}
	public static T GetTheClassFromGO<T>(GameObject go) {
		Component component = go.GetComponentInChildren(typeof(T));
		T target_object = (T)System.Convert.ChangeType(component, typeof(T));
		if (target_object == null)
			Debug.LogError("Can't find " + go.ToString());
		return target_object;
	}
	
	public static GameObject FindActiveGO(string name) {
		GameObject go = GameObject.Find(name);
		if (!go) {
			Debug.LogError("Can't find " + name);
		}
		return go;
	}

	public static void LoadBackground(string back_go_name, string sprite_name,
	                                  Vector2 indent_to_actual_image, bool align_top,
	                                  float sp_bar_indent_y = 0) {
		GameObject go = Utils.LoadSpriteIntoGO(sprite_name, back_go_name);
		Utils.AttachSriteToCameraInGO(go, KeyCode.X, 1.0f, (int)indent_to_actual_image.x);
		
		SpriteRenderer sr = (SpriteRenderer)go.GetComponent(typeof(SpriteRenderer));
		Sprite sprite = sr.sprite;
		float image_pixels_to_unit = sr.bounds.size.y / sprite.textureRect.height;

		float shift_y = Camera.main.orthographicSize - sr.bounds.size.y / 2.0f
			+ indent_to_actual_image.y * image_pixels_to_unit;
		shift_y *= align_top ? 1.0f : -1.0f;
		float pos_y = Camera.main.transform.position.y + shift_y - Utils.ScreenPixelsToUnitY(sp_bar_indent_y);
		go.transform.position = new Vector3(0.0f, pos_y, 0.0f);
	}

	public static GameObject GetChildGO(GameObject FromGameObject, string WithName) {
		Transform[] ts = FromGameObject.transform.GetComponentsInChildren<Transform	>();
		foreach (Transform t in ts) if (t.gameObject.name == WithName) return t.gameObject;
		return null;
	}
}

