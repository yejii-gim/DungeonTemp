using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private GameObject _laserLight;
    private bool isAttack = false;
    private void Update()
    {
        RaycastHit hit;
        bool isHit = false;
        if (Physics.Raycast(transform.position, transform.right, out hit, 10f))
        {
            if (hit.collider.CompareTag("Player"))
            {
                isHit = true;
                if (!isAttack)
                {
                    isAttack = true;
                    CharcterManager.Instance.player.condition.TakePhysicalDamage(1);
                }
                _laserLight.SetActive(true);
            }
        }
        if(!isHit) // 플레이어가 레이저에서 벗어난 상태
        {
            isAttack = false;
            _laserLight.SetActive(false);
        }
        Debug.DrawRay(transform.position, transform.right * 10f);
    }
}
