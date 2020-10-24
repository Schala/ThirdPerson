using UnityEngine;

public class ConsoleReadyBehaviour : MonoBehaviour
{
    public string ConsoleString() => $"\"{gameObject.name}\" ({gameObject.GetInstanceID():X8})";
}
