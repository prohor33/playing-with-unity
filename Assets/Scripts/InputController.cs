using UnityEngine;
using System.Collections;

public class MyTouch {
	public MyTouch() {
		deltaPosition = Vector2.zero;
	}
	public MyTouch(Touch touch) {
		position = touch.position;
		phase = touch.phase;
		deltaPosition = touch.deltaPosition;
	}

	public Vector2 position;
	public Vector2 deltaPosition;
	public TouchPhase phase;
}

public class InputController {
	
	private static float m_MaxPickingDistance = 2000.0f;
	private static Transform m_PickedObject = null;

	// mouse
	private static Vector3 m_LastMousePos;

	public static bool Update () {
		if (HandleDragAndDrop())
			return true;
		return false;
	}

	public static bool HandleDragAndDrop() {

		if (!GameContr.m_RunOnMac) {
			foreach (Touch touch in Input.touches) {
				if (HandleOneTouch(new MyTouch(touch)))
					return true;
			}
		} else {
			MyTouch mouse_touch = new MyTouch();

			if (Input.GetMouseButtonDown(0)) {
				mouse_touch.phase = TouchPhase.Began;
				mouse_touch.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
				m_LastMousePos = Input.mousePosition;
			} else if (Input.GetMouseButton(0)) {
				mouse_touch.phase = TouchPhase.Moved;
				mouse_touch.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
				Vector2 delta_p = Input.mousePosition - m_LastMousePos;
				mouse_touch.deltaPosition = delta_p;
				m_LastMousePos = Input.mousePosition;
			} else {
				return false;
			}

			if (HandleOneTouch(mouse_touch))
				return true;
		}
		return false;
	}

	// private members

	static bool HandleOneTouch(MyTouch touch) {
//		Debug.Log("Touching at: " + touch.position);
		
		if (touch.phase == TouchPhase.Began) {
//			Debug.Log("Touch phase began at: " + touch.position);
			
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touch.position), Vector2.zero, 
			                                     10.0f, 1 << LayerMask.NameToLayer("FallingBomb"));
			if (hit.collider != null) { 
				m_PickedObject = hit.transform;
				Debug.Log("catch object" + m_PickedObject.gameObject.name);
			} else {
				m_PickedObject = null;
				Debug.Log("no object");
				return false;
			}
			return true;
		} else if (m_PickedObject && touch.phase == TouchPhase.Moved) {
//			Debug.Log("Touch phase Moved");
			
			Vector2 screen_delta = touch.deltaPosition;
			Vector2 world_delta = Utils.ScreenDeltaToWorld(screen_delta);

			Vector3 world_delta_3d = new Vector3(world_delta.x, world_delta.y, 0.0f);
			
			m_PickedObject.position += world_delta_3d;
			return true;
		} else if (m_PickedObject && touch.phase == TouchPhase.Ended) {
//			Debug.Log("Touch phase Ended");
			
			m_PickedObject = null;
			
			return true;
		}
		return false;
	}
}

