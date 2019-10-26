using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    public float Speed = 600.0f;
    public float LifeTime = 3.0f;
    public int damage = 25;

    void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        gameObject.SetActive(false) ;
    }
}