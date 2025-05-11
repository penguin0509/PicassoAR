using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollowCanvas : MonoBehaviour
{
    public Transform targetTransform; // Picasso1
    public float positionSmoothSpeed = 5f; // 位置平滑速度
    public float rotationSmoothSpeed = 5f; // 旋轉平滑速度

    private Vector3 offset; // 初始相對位置

    void Start()
    {
        if (targetTransform == null)
        {
            Debug.LogError("SmoothFollowCanvas：沒有設定targetTransform！");
            return;
        }
        // 一開始記下與目標的相對位移
        offset = transform.position - targetTransform.position;
    }

    void Update()
    {
        if (targetTransform == null) return;

        // 平滑處理位置
        Vector3 desiredPosition = targetTransform.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * positionSmoothSpeed);

        // 平滑處理旋轉，讓面板一直面向攝影機
        Vector3 lookDirection = transform.position - Camera.main.transform.position;
        lookDirection.y = 0; // 鎖定Y，不要上下亂轉
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSmoothSpeed);
    }
}
