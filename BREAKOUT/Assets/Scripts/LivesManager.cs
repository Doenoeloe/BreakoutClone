using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class LivesManager : MonoBehaviour
{
    public int lives = 3;
    public GameObject ballPrefab;
    public Transform ballSpawnPoint;
    public Image flashOverlay;
    public TextMeshProUGUI deathText;
    public Image[] heartImages;
    public Image deathImage;
    public Image deathOverlay;
    
    void OnEnable()  => GameEvents.OnBallDied += HandleDeath;
    void OnDisable() => GameEvents.OnBallDied -= HandleDeath;

    void HandleDeath()
    {
        lives--;
        UpdateHearts();
        StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        StartCoroutine(ScreenFlash());
        CameraShake.Instance.Shake(0.4f, 0.3f);

        Time.timeScale = 0.2f;
        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 1f;

        if (lives <= 0)
        {
            Debug.Log("Game Over!");
            StartCoroutine(TrueDeathSequence());
        }
        else
        {
            yield return new WaitForSeconds(0.5f);

            if (ballPrefab == null) { Debug.LogError("ballPrefab niet ingesteld!"); yield break; }
            if (ballSpawnPoint == null) { Debug.LogError("ballSpawnPoint niet ingesteld!"); yield break; }

            Instantiate(ballPrefab, ballSpawnPoint.position, Quaternion.identity);
        }
    }

    IEnumerator ScreenFlash()
    {
        deathText.alpha = 0f;
        deathText.transform.localScale = Vector3.one * 2f;
        float elapsed = 0f;
        while (elapsed < 0.05f)
        {
            float t = elapsed / 0.05f;
            flashOverlay.color = new Color(1f, 1f, 1f, Mathf.Lerp(0f, 1f, elapsed / 0.05f));
            deathText.alpha = t;
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        elapsed = 0f;
        
        while (elapsed < 0.4f)
        {
            float t = elapsed / 0.4f;
            flashOverlay.color = new Color(1f, 1f, 1f, Mathf.Lerp(1f, 0f, elapsed / 0.4f));
            deathText.alpha = Mathf.Lerp(1f, 0f, t);
            deathText.transform.localScale = Vector3.Lerp(Vector3.one * 2f, Vector3.one, t);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        flashOverlay.color = new Color(1f, 1f, 1f, 0f);
        deathText.alpha = 0f;
    }

    IEnumerator TrueDeathSequence()
    {
        deathImage.transform.localScale = Vector3.one * 25f;
        deathOverlay.transform.localScale = Vector3.one * 25f;
        float elapsed = 0f;
        while (elapsed < 5f)
        {
            deathImage.color = new Color(1f, 1f, 1f, Mathf.Lerp(0f, 1f, elapsed / 0.05f));
            deathOverlay.color = new Color(0f, 0f, 0f, Mathf.Lerp(0f, 0.8470588f, elapsed / 0.05f));
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        elapsed = 0f;
        
        while (elapsed < 5f)
        {
            float t = elapsed / 0.4f;
            deathImage.color = new Color(1f, 1f, 1f, Mathf.Lerp(1f, 0f, elapsed / 0.4f));
            deathOverlay.color = new Color(0f, 0f, 0f, Mathf.Lerp(8470588f, 0f, elapsed / 0.4f));
            deathImage.transform.localScale = Vector3.Lerp(Vector3.one * 25f, Vector3.one, t);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        deathImage.color = new Color(1f, 1f, 1f, 0f);
        deathOverlay.color = new Color(0f, 0f, 0f, 0f);
        yield return null;
    }
    void UpdateHearts()
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            heartImages[i].enabled = i < lives;
        }
    }
}