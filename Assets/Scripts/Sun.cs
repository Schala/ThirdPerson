using UnityEngine;

public class Sun : MonoBehaviour
{
    public float slowFactor = 4f;
    float sunDelta;

	private void Awake() => sunDelta = Time.deltaTime / slowFactor;
	void Update() => transform.RotateAround(Vector3.zero, Vector3.right, sunDelta);
}
