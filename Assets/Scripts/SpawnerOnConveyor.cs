using UnityEngine;
using System.Collections;
using System.Collections.Generic;

enum SpawnObjectType { Piano = 0, Samovar, Cupboard, Last /*should be last*/ };

class SpawnObjectsInfo {
	public SpawnObjectsInfo(SpawnObjectType type) {
		m_Type = type;
		switch (m_Type) {
		case SpawnObjectType.Samovar:
			m_Chance = 1.0f;
			m_Mass = 3.0f;
			m_SpriteName = "coin1";
			break;
		case SpawnObjectType.Cupboard:
			m_Chance = 0.3f;
			m_Mass = 5.0f;
			m_SpriteName = "coin2";
			break;
		case SpawnObjectType.Piano:
			m_Chance = 0.1f;
			m_Mass = 10.0f;
			m_SpriteName = "coin3";
			break;
		default:
			Debug.LogWarning("wrong type");
			break;
		}
	}
	public float m_Chance;
	public SpawnObjectType m_Type;
	public float m_Mass;
	public string m_SpriteName;
}

public class SpawnerOnConveyor {
	
	public LevelController m_GameContr;

	float m_DeltaSpawnMove;	// set from lvl_settings
	float m_SpeedDeltaSpawnMoveCoef = 1.0f;
	float m_LeftOrRightPrivilege;	// from -1.0 to 1.0, from left to right
	bool m_PrivilegeChangingTrand;	// true = increasing, false = decreasing

	Dictionary<SpawnObjectType, SpawnObjectsInfo> m_SpawnObjects = new Dictionary<SpawnObjectType, SpawnObjectsInfo>();

	public bool Spawn(float spawn_move, bool is_right) {

		float privilege = Mathf.Abs(m_LeftOrRightPrivilege);
		privilege *= is_right == (m_LeftOrRightPrivilege > 0.0) ? 1.0f : -1.0f;
		const float max_privilege = 10.0f;
		privilege *= max_privilege;

//		Debug.Log(m_LeftOrRightPrivilege + " " + privilege + " " + m_DeltaSpawnMove);

		// Should we  do this?
		float min_delta_spawn_move = 4.0f;
		float max_delta_spawn_move = 15.0f;

		float res_delta_spawn_move = m_DeltaSpawnMove - privilege;
		res_delta_spawn_move = Mathf.Clamp(res_delta_spawn_move,
		                                   min_delta_spawn_move, max_delta_spawn_move);


		if (spawn_move > res_delta_spawn_move * m_SpeedDeltaSpawnMoveCoef) {
			
			Vector3 pos = new Vector3(3.0f * (is_right ? 1.0f : -1.0f), 30.0f, 0.0f);
			
			GameObject go =	MonoBehaviour.Instantiate (m_GameContr.m_FallingObject, pos, Quaternion.identity) as GameObject;
			FallingObject falling_obj = (FallingObject) go.GetComponent(typeof(FallingObject));
			m_GameContr.GetConveyor(is_right).AddObject(falling_obj);

			falling_obj.SetRotationSpeed((Random.value - 0.5f) * 10.0f);
			falling_obj.transform.rotation = Quaternion.Euler(0.0f, 0.0f, Random.value * 360.0f);

			// select object

			SpawnObjectType obj_type = GetObjectType(is_right);
			SpawnObjectsInfo obj_info = m_SpawnObjects[obj_type];

			go.rigidbody2D.mass = obj_info.m_Mass;

			SpriteRenderer sprite_rend = falling_obj.GetComponentInChildren(typeof(SpriteRenderer)) as SpriteRenderer;
			Sprite sprite = Resources.Load<Sprite>(obj_info.m_SpriteName);
			if (!sprite)
				Debug.LogWarning("Can't load Cake");
			sprite_rend.sprite = sprite;

			float scale = 1.0f;
			go.transform.localScale = new Vector3(scale, scale, 1.0f);

			return true;
		} else {
			return false;
		}
	}

	public void Init() {
		m_SpawnObjects.Clear();
		for (int i = 0; i < (int)SpawnObjectType.Last; i++)
			m_SpawnObjects[(SpawnObjectType)i] = new SpawnObjectsInfo((SpawnObjectType)i);

		m_DeltaSpawnMove = GameContr.control.lvl_settings.GetDeltaSpawnMove();
		m_LeftOrRightPrivilege = 0.0f;
		m_PrivilegeChangingTrand = true;
	}

	public void SetSpeedDeltaSpawnMoveCoef(float value) {
		m_SpeedDeltaSpawnMoveCoef = value;
	}

	// Calling it from ObjectConveyour
	public void FixedUpdate() {
		float chance_of_changin_trand = 0.1f * Time.deltaTime;
		if (Random.Range(0.0f, 1.0f) < chance_of_changin_trand)
			m_PrivilegeChangingTrand = !m_PrivilegeChangingTrand;

		const float privilege_changing_speed = 0.4f;
		float delta_privilege = privilege_changing_speed * Time.deltaTime;
		delta_privilege *= m_PrivilegeChangingTrand ? 1.0f : -1.0f;
		m_LeftOrRightPrivilege += delta_privilege;
		m_LeftOrRightPrivilege = Mathf.Clamp(m_LeftOrRightPrivilege, -1.0f, 1.0f);
	}

	SpawnObjectType GetObjectType(bool is_right) {
		float sum = 0.0f;
		for (int i = 0; i < (int)SpawnObjectType.Last; i++) {
			sum += m_SpawnObjects[(SpawnObjectType)i].m_Chance;
		}
		float rand = Random.Range(0, sum);
		SpawnObjectType res_obj_type = SpawnObjectType.Last;
		sum = 0.0f;
		for (int i = 0; i < (int)SpawnObjectType.Last; i++) {
			float chance = m_SpawnObjects[(SpawnObjectType)i].m_Chance;
			if (sum < rand && rand <= (sum + chance)) {
				res_obj_type = (SpawnObjectType)i;
				break;
			}
			sum += chance;
		}
		if (res_obj_type == SpawnObjectType.Last)
			Debug.LogWarning("error");
		return res_obj_type;
	}

}



