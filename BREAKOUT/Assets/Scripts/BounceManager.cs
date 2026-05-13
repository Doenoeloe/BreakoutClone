using UnityEngine;

public class BounceManager : MonoBehaviour
{
    [Header("Particles")]
    public GameObject bounceParticlePrefab;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (bounceParticlePrefab != null)
        {
            ContactPoint2D contact = collision.GetContact(0);
            Instantiate(bounceParticlePrefab, contact.point, Quaternion.identity);
        }
    }
}