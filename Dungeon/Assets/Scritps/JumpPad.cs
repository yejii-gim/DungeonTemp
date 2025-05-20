using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private float jumpForce = 5f;
    private Rigidbody _rb;
    private void Start()
    {
        _rb = CharcterManager.Instance.player.GetComponent<Rigidbody>();
        Debug.Log(_rb);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}
