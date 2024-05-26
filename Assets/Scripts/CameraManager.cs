using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        cam.backgroundColor = UIDesign.Colors.Global.Background;
    }

}
