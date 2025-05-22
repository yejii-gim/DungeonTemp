using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour, IInteractable
{
    public float _shootPower = 50f;
    public Transform shootPosition;
    public Transform shootDirection;
    private Rigidbody _rb;
    private Quaternion _orgRotation;
    public string GetInteractPrompt()
    {
        return "S를 누르면 발사 됩니다.";
    }

    public void OnInteract()
    {
        if (CharcterManager.Instance.player.TryGetComponent<Rigidbody>(out _rb))
        {
            _orgRotation = CharcterManager.Instance.player.transform.rotation;
            CharcterManager.Instance.player.transform.position = shootPosition.position;
            CharcterManager.Instance.player.transform.rotation = transform.rotation;
            Invoke("Shoot", 0.5f);
        }
        
    }

    private void Shoot()
    {
        Vector3 dir =  shootDirection.position - shootPosition.position;
        _rb.AddForce(dir * _shootPower, ForceMode.Impulse);
        CharcterManager.Instance.player.transform.rotation = _orgRotation;
    }
}
