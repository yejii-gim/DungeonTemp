using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private float _moveDistance = 5f;
    [SerializeField] private float _speed = 3f;

    private bool isActive = false;
    private Vector3 preivousPos; // ���� �������� �÷��� ��ġ �����
    private Vector3 startPos; // ó�� ���� ��ġ
    private void Start()
    {
        startPos = transform.position;
        preivousPos = startPos;
    }

    private void Update()
    {
        if (!isActive) return; 

        // ������ �������� �̵�
        transform.position += Vector3.right * _speed * Time.deltaTime;
        // ������ �Ÿ���ŭ �̵��ϸ� ���� �Ųٷ� ����
        if (Vector3.Distance(startPos, transform.position) >= _moveDistance)
        {
            _speed *= -1;
        }

    }

    void LateUpdate()
    {
        if (!isActive) return;

        Vector3 distance = transform.position - preivousPos;
        preivousPos = transform.position;

        // �÷��̾ �̵�
        CharcterManager.Instance.player.transform.position += distance;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            isActive = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            isActive = false;
        }
    }
}
