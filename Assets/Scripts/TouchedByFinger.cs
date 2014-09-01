using UnityEngine;
using System.Collections;

public abstract class TouchedByFinger : MonoBehaviour {

	public TouchedByFinger() {
		m_IsMovingByFinger = false;
		m_TouchedFirstTime = false;
	}

	bool m_IsMovingByFinger;
	Vector3 m_FingerSpeed;
	float m_LastMovingByFingerTime;
	bool m_TouchedFirstTime;

	public void MoveByFinger(Vector3 delta_p) {
		if (!IsTouchable())
			return;

		if (m_IsMovingByFinger) {
			Vector3 speed = delta_p / (Time.time - m_LastMovingByFingerTime);
			rigidbody2D.velocity = speed;
			rigidbody2D.isKinematic = false; // If isKinematic is enabled, Forces, collisions or joints will not affect the rigidbody anymore
			m_LastMovingByFingerTime = Time.time;
		} else {
			// start moving by finger
			m_IsMovingByFinger = true;
			m_LastMovingByFingerTime = Time.time;
			if (!m_TouchedFirstTime) {
				TouchFirstTime();
				m_TouchedFirstTime = true;
			}
		}
	}
	public void EndMovingByFinger() {
		m_IsMovingByFinger = false;
		rigidbody2D.velocity = Vector3.zero;
	}

	protected abstract void TouchFirstTime();
	protected abstract bool IsTouchable();
}

