using UnityEngine;
using Cinemachine;

public class FaceActiveCamera : MonoBehaviour
{
    public Camera mainCamera;

    void Start()
    {

    }

    void LateUpdate()
    {
        // 每帧检查激活的主摄像机
        UpdateMainCamera();

        // 让字体面向当前激活的摄像机
        if (mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                             mainCamera.transform.rotation * Vector3.up);
        }
    }

    void UpdateMainCamera()
    {
        // 查找当前激活的主摄像机
        if (Camera.main != null && Camera.main != mainCamera)
        {
            mainCamera = Camera.main;
        }
    }
}
