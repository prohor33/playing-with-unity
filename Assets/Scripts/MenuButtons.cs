using UnityEngine;
using System.Collections;

public class MenuButtons : MonoBehaviour {

	// private members --------------------

	bool m_AlreadyCatched = false;
	GameObject m_ChainRight;

	void Start () {
		Init();
	}

	void Init() {
		m_ChainRight = GameObject.FindGameObjectWithTag("ChainRight");
	}

	void FixedUpdate () {
		UpdateCatch();
	}

	void UpdateCatch() {
		if (m_AlreadyCatched)
			return;
		if (m_ChainRight.transform.localPosition.y < -13.0f)
			BeCatched();
	}

	void BeCatched() {
		m_AlreadyCatched = true;

		EnableHingeJoint("ChainLeft");
		EnableHingeJoint("ChainRight");
	}

	void EnableHingeJoint(string go_name) {
		GameObject chain_go = GameObject.FindGameObjectWithTag(go_name);
		HingeJoint2D hj = chain_go.GetComponent(typeof(HingeJoint2D)) as HingeJoint2D;
		hj.enabled = true;
	}

	void BreakOneChain() {
		HingeJoint2D hj = m_ChainRight.GetComponent(typeof(HingeJoint2D)) as HingeJoint2D;
		hj.enabled = false;
	}
}

