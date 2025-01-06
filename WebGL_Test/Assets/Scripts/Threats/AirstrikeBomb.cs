using System;
using UnityEngine;

public class AirstrikeBomb : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Bomb hit the base");
            Destroy(gameObject);
        }
    }
}
