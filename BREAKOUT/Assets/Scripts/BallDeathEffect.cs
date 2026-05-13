using UnityEngine;
using System.Collections;

public class BallDeathEffect : MonoBehaviour
{
    public GameObject explosionParticlePrefab;

    void OnEnable() => GameEvents.OnBallDied += PlayDeathEffect;
    void OnDisable() => GameEvents.OnBallDied -= PlayDeathEffect;

    void PlayDeathEffect()
    {
        StartCoroutine(DeathAnimation());
    }

    IEnumerator DeathAnimation()
    {
        if (explosionParticlePrefab != null)
            Instantiate(explosionParticlePrefab, transform.position, Quaternion.identity);

        float elapsed = 0f;
        Vector3 startScale = transform.localScale;

        while (elapsed < 0.2f)
        {
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, elapsed / 0.2f);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}