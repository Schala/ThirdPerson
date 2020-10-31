using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float zoomSpeed = 0.5f;

    [Range(0.5f, 6f)]
    public float startZoom = 3.5f;

    CinemachineFollowZoom followZoom;

    private void Awake() => followZoom = GetComponent<CinemachineFollowZoom>();

    private void Start() => followZoom.m_Width = startZoom;

	void Update()
    {
        if (Input.mouseScrollDelta.y == 0) return;
        followZoom.m_Width += Input.mouseScrollDelta.y * -zoomSpeed;
        followZoom.m_Width = Mathf.Clamp(followZoom.m_Width, 0.5f, 6f);
    }
}
