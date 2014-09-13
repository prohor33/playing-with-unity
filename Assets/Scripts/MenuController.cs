using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour {

	void OnGUI () {
		Vector2 btn_size = new Vector2(Screen.width / 1.5f, Screen.height / 7);
		int btn_vert_shift = Screen.height / 20 + (int)btn_size.y;
		int btn_number = 4;
		Rect btn_pos = new Rect(Screen.width / 2 - btn_size.x / 2,
		                        Screen.height / 2 - (btn_size.y + (btn_number - 1) * btn_vert_shift) / 2 , btn_size.x, btn_size.y);

		GUIStyle disable_style = new GUIStyle();
//		disable_style.
		// TODO: make is not active when no game to continue
		if(GUI.Button(btn_pos, "Continue")) {
			if (StatisticKeeper.m_AlreadySawKingScene)
				Application.LoadLevel(Utils.dungeon_level);
			else
				Application.LoadLevel(Utils.king_level);
		}

		RectOffset btn_shift = new RectOffset(0, 0, -btn_vert_shift, btn_vert_shift);
		btn_pos = btn_shift.Add(btn_pos);
		if(GUI.Button(btn_pos, "New Game")) {
			SaveLoad.ResetAllData();
			Debug.Log("The data have been reset");
			Application.LoadLevel(Utils.king_level);
		}

		btn_pos = btn_shift.Add(btn_pos);
		if(GUI.Button(btn_pos, "About")) {
			//			Application.LoadLevel(2);
		}

		btn_pos = btn_shift.Add(btn_pos);
		if(GUI.Button(btn_pos, "Quit")) {
			Application.Quit();
		}

//		// Make a background box
//		Vector2 back_size = new Vector2(Screen.width / 2, Screen.height / 2);
//		GUI.Box(new Rect(Screen.width / 2 - back_size.x / 2,
//		                 Screen.height / 2 - back_size.y / 2, back_size.x, back_size.y), "Game Menu");
	}
}