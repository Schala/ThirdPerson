using UnityEngine;

public class HUD : MonoBehaviour
{
    void LateUpdate() => transform.LookAt(transform.position + Camera.main.transform.forward);
}
