using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasFaceCamera : MonoBehaviour
{
    public Transform cameraTransform; // ���w�n�¦V����v�� (�q�`�OARCamera)

    void Start()
    {
        // �p�G�S�]�w��v���A�N�۰ʧ�D��v��
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        // �C�@�V���¦V��v��
        Vector3 lookDirection = transform.position - cameraTransform.position;
        lookDirection.y = 0; // ��wy�b�A�����O���|��ۤW�U����
        transform.rotation = Quaternion.LookRotation(lookDirection);
    }
}
