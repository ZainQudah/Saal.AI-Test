using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenEffects : MonoBehaviour
{
    #region Serialized Fields (Unity Inspector)

    [Header("Flash Effect")] [Tooltip("The UI Image used to display screen flashes.")] [SerializeField]
    private Image screenFlashImage;

    [Tooltip("Duration of the flash effect (in seconds).")] [SerializeField]
    private float flashDuration = 0.5f;

    [Tooltip("Color used for red screen flashes.")] [SerializeField]
    private Color redFlashColor = new Color(1f, 0f, 0f, 0.5f);

    [Tooltip("Color used for green screen flashes.")] [SerializeField]
    private Color greenFlashColor = new Color(0f, 1f, 0f, 0.5f);

    [Header("Camera Shake")] [Tooltip("The camera to apply the shake effect to.")] [SerializeField]
    private Transform cameraTransform;

    [Tooltip("Duration of the camera shake (in seconds).")] [SerializeField]
    private float shakeDuration = 0.3f;

    [Tooltip("Strength of the camera shake.")] [SerializeField]
    private float shakeMagnitude = 0.2f;

    #endregion

    #region Private Fields

    private Vector3 originalCameraPosition; // Stores the initial camera position for restoring after shake.
    private Coroutine currentFlashCoroutine; // Tracks active flash coroutine to avoid overlap.

    #endregion

    #region Singleton for Global Access

    public static ScreenEffects Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Triggers a red screen flash.
    /// </summary>
    [ContextMenu("Test Red Flash")]
    public void TriggerRedFlash()
    {
        TriggerFlash(redFlashColor);
        TriggerCameraShake();
    }

    /// <summary>
    /// Triggers a green screen flash.
    /// </summary>
    [ContextMenu("Test Green Flash")]
    public void TriggerGreenFlash()
    {
        TriggerFlash(greenFlashColor);
    }

    /// <summary>
    /// Triggers a camera shake effect.
    /// </summary>
    [ContextMenu("Test Camera Shake")]
    public void TriggerCameraShake()
    {
        if (cameraTransform != null)
        {
            StartCoroutine(CameraShakeRoutine());
        }
    }

    #endregion

    #region Private Methods

    private void TriggerFlash(Color flashColor)
    {
        if (currentFlashCoroutine != null)
        {
            StopCoroutine(currentFlashCoroutine);
        }

        currentFlashCoroutine = StartCoroutine(FlashRoutine(flashColor));
    }

    private IEnumerator FlashRoutine(Color flashColor)
    {
        screenFlashImage.color = flashColor;
        screenFlashImage.gameObject.SetActive(true);

        float halfDuration = flashDuration / 2f;
        for (float t = 0; t < halfDuration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(0f, flashColor.a, t / halfDuration);
            SetFlashAlpha(alpha);
            yield return null;
        }

        for (float t = 0; t < halfDuration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(flashColor.a, 0f, t / halfDuration);
            SetFlashAlpha(alpha);
            yield return null;
        }

        screenFlashImage.gameObject.SetActive(false);
        currentFlashCoroutine = null;
    }

    private void SetFlashAlpha(float alpha)
    {
        Color color = screenFlashImage.color;
        color.a = alpha;
        screenFlashImage.color = color;
    }

    private IEnumerator CameraShakeRoutine()
    {
        originalCameraPosition = cameraTransform.position;

        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            Vector3 shakeOffset = new Vector3(
                Random.Range(-shakeMagnitude, shakeMagnitude),
                Random.Range(-shakeMagnitude, shakeMagnitude),
                0f
            );

            cameraTransform.position = originalCameraPosition + shakeOffset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        cameraTransform.position = originalCameraPosition;
    }

    #endregion
}