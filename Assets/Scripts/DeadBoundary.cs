using UnityEngine;
using System.Collections;

public class DeadBoundary : MonoBehaviour {

	void OnTriggerExit2D(Collider2D other)
	{
		Destroy(other.gameObject);
//		Debug.Log("OnTriggerExit");
	}
//	void OnTriggerEnter2D(Collider2D other)
//	{
//		Debug.Log("OnTriggerEnter");
//	}
}
