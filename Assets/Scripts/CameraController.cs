using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float zoomSpeed = 0.5f;
    CinemachineFollowZoom followZoom;

    private void Awake() => followZoom = GetComponent<CinemachineFollowZoom>();

    void Update()
    {
        if (Input.mouseScrollDelta.y == 0) return;
        followZoom.m_Width += Input.mouseScrollDelta.y * -zoomSpeed;
        followZoom.m_Width = Mathf.Clamp(followZoom.m_Width, 0.5f, 6f);
    }
}
