using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasFaceCamera : MonoBehaviour
{
    public Transform cameraTransform; // 指定要朝向的攝影機 (通常是ARCamera)

    void Start()
    {
        // 如果沒設定攝影機，就自動找主攝影機
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        // 每一幀都朝向攝影機
        Vector3 lookDirection = transform.position - cameraTransform.position;
        lookDirection.y = 0; // 鎖定y軸，讓面板不會跟著上下亂轉
        transform.rotation = Quaternion.LookRotation(lookDirection);
    }
}
