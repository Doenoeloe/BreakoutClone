using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LivesManager : MonoBehaviour
{
    public int lives = 3;
    public GameObject ballPrefab;
    public Transform ballSpawnPoint;
    public Image flashOverlay;
    public TextMeshProUGUI deathText;
    public TextMeshProUGUI continueText;
    public Image[] heartImages;
    public Image deathImage;
    public Image deathOverlay;

    private bool animationDone = false;
    void OnEnable() => GameEvents.OnBallDied += HandleDeath;
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

            if (ballPrefab == null)
            {
                Debug.LogError("ballPrefab niet ingesteld!");
                yield break;
            }

            if (ballSpawnPoint == null)
            {
                Debug.LogError("ballSpawnPoint niet ingesteld!");
                yield break;
            }

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
        continueText.alpha = 0f;
        float elapsed = 0f;

        // Fade IN
        while (elapsed < 0.05f)
        {
            deathImage.color = new Color(1f, 1f, 1f, Mathf.Lerp(0f, 1f, elapsed / 0.05f));
            deathOverlay.color = new Color(0f, 0f, 0f, Mathf.Lerp(0f, 0.847f, elapsed / 0.05f));
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        // Blink until key press
        StartCoroutine(BlinkText());
        yield return new WaitUntil(() => Input.anyKeyDown);

        StopCoroutine(BlinkText());
        continueText.alpha = 0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator BlinkText()
    {
        int currentEffect = 0;
        float elapsed = 0f;

        while (true)
        {
            currentEffect = Random.Range(0, 4);
            elapsed = 0f;

            switch (currentEffect)
            {
                // Wobbly bounce
                case 0:
                    while (elapsed < 1.2f)
                    {
                        float bounce = Mathf.Sin(elapsed * 8f) * 10f;
                        continueText.alpha = 1f;
                        continueText.transform.localPosition = new Vector3(
                            continueText.transform.localPosition.x,
                            -353 + bounce,
                            0f
                        );
                        continueText.transform.localScale = Vector3.one * (1f + Mathf.Sin(elapsed * 6f) * 0.1f);
                        elapsed += Time.unscaledDeltaTime;
                        yield return null;
                    }

                    break;

                // Glitchy flicker
                case 1:
                    while (elapsed < 1.2f)
                    {
                        continueText.alpha = Random.value > 0.15f ? 1f : 0f;
                        continueText.transform.localPosition = new Vector3(
                            -0f + Random.Range(-6f, 6f),
                            continueText.transform.localPosition.y,
                            0f
                        );
                        continueText.transform.localScale = Vector3.one * Random.Range(0.95f, 1.05f);
                        elapsed += Time.unscaledDeltaTime;
                        yield return null;
                    }

                    break;

                // Spinning chaos
                case 2:
                    while (elapsed < 1.2f)
                    {
                        float spin = elapsed * 360f;
                        continueText.alpha = 1f;
                        continueText.transform.localRotation = Quaternion.Euler(0f, 0f, spin);
                        continueText.transform.localScale = Vector3.one * (1f + Mathf.Sin(elapsed * 4f) * 0.3f);
                        elapsed += Time.unscaledDeltaTime;
                        yield return null;
                    }

                    continueText.transform.localRotation = Quaternion.identity;
                    break;

                // Blink fade
                case 3:
                    while (elapsed < 0.6f)
                    {
                        continueText.alpha = Mathf.Lerp(0f, 1f, elapsed / 0.6f);
                        continueText.transform.localScale = Vector3.one;
                        continueText.transform.localRotation = Quaternion.identity;
                        elapsed += Time.unscaledDeltaTime;
                        yield return null;
                    }

                    elapsed = 0f;
                    while (elapsed < 0.6f)
                    {
                        continueText.alpha = Mathf.Lerp(1f, 0f, elapsed / 0.6f);
                        elapsed += Time.unscaledDeltaTime;
                        yield return null;
                    }

                    break;
            }
        }
    }

    void UpdateHearts()
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            heartImages[i].enabled = i < lives;
        }
    }
}