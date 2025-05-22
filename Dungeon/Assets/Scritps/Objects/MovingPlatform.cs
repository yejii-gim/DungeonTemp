using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private float _moveDistance = 5f;
    [SerializeField] private float _speed = 3f;

    private bool isActive = false;
    private Vector3 preivousPos; // 이전 프레임의 플랫폼 위치 저장용
    private Vector3 startPos; // 처음 시작 위치
    private void Start()
    {
        startPos = transform.position;
        preivousPos = startPos;
    }

    private void Update()
    {
        if (!isActive) return; 

        // 오른쪽 방향으로 이동
        transform.position += Vector3.right * _speed * Time.deltaTime;
        // 지정된 거리만큼 이동하면 방향 거꾸로 지정
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

        // 플레이어도 이동
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
