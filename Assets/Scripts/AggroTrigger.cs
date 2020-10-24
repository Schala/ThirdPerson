using System;
using UnityEngine;

public class AggroTrigger : MonoBehaviour
{
	public Action<Collider, bool> aggroCallback;

	private void OnTriggerEnter(Collider other) => aggroCallback(other, true);
	private void OnTriggerExit(Collider other) => aggroCallback(other, false);

	/*private void Start()
	{
		float radius = GetComponent<SphereCollider>().radius;
		GetComponent<MeshFilter>().transform.localScale = new Vector3(radius, radius, radius);
	}*/
}
