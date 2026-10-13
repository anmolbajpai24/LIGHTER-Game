using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject target;
    private Camera arenaCamera;
    private void Start()
    {
        arenaCamera = GetComponent<Camera>();
        transform.position = new Vector3(10, 14, -12);
        transform.LookAt(new Vector3(0, 0, 0));
        arenaCamera.orthographic = true;
        arenaCamera.clearFlags = CameraClearFlags.SolidColor;
        arenaCamera.backgroundColor = new Color(.025f, .035f, .06f);
    }
    private void LateUpdate()
    {
        // The full arena stays in view on phones and narrow desktop windows.
        arenaCamera.orthographicSize = Mathf.Max(7.2f, 8.3f / arenaCamera.aspect);
    }
}
