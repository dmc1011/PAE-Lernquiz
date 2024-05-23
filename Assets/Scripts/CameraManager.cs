using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        cam.backgroundColor = Global.Colors.Background;
    }

}
