using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollowCanvas : MonoBehaviour
{
    public Transform targetTransform; // Picasso1
    public float positionSmoothSpeed = 5f; // ��m���Ƴt��
    public float rotationSmoothSpeed = 5f; // ���७�Ƴt��

    private Vector3 offset; // ��l�۹��m

    void Start()
    {
        if (targetTransform == null)
        {
            Debug.LogError("SmoothFollowCanvas�G�S���]�wtargetTransform�I");
            return;
        }
        // �@�}�l�O�U�P�ؼЪ��۹�첾
        offset = transform.position - targetTransform.position;
    }

    void Update()
    {
        if (targetTransform == null) return;

        // ���ƳB�z��m
        Vector3 desiredPosition = targetTransform.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * positionSmoothSpeed);

        // ���ƳB�z����A�����O�@�����V��v��
        Vector3 lookDirection = transform.position - Camera.main.transform.position;
        lookDirection.y = 0; // ��wY�A���n�W�U����
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSmoothSpeed);
    }
}
