using UnityEngine;

public class RotatehealthBarUI : MonoBehaviour
{
    public Transform MainCamera;

    private void LateUpdate()
    {
        transform.LookAt(transform.position + MainCamera.forward);
    }
}
