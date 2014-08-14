using UnityEngine;
using System.Collections;

public class Utils {

	// TODO: to delete?
	public static float X320ToAnyRes(float x) {
		return x / 320.0f * Screen.width;
	}
	public static float Y480ToAnyRes(float y) {
		return y / 480.0f * Screen.height;
	}

	public static float ScreenPixelsToUnit(float x) {
		float ratio = 2.0f * Camera.main.orthographicSize / Camera.main.pixelHeight;
		return x * ratio;
	}
	public static float UnitToScreenPixels(float x) {
		float ratio = 2.0f * Camera.main.orthographicSize / Camera.main.pixelHeight;
		return x / ratio;
	}

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

	public static Vector2 GetCameraSize() {
		Camera cam = Camera.main;
		float height = 2f * cam.orthographicSize;
		float width = height * cam.aspect;
		return new Vector2(width, height);
	}
	public static Vector2 GetCameraPos() {
		return Camera.main.transform.position;
	}

	public static T FindTheClassInObject<T>(string obj_name) {
		GameObject go = GameObject.Find(obj_name);
		Component component = go.GetComponentInChildren(typeof(T));
		T target_object = (T)System.Convert.ChangeType(component, typeof(T));
		if (target_object == null)
			Debug.LogError("Can't find " + obj_name);
		return target_object;
	}
}

