using UnityEngine;
using UnityEngine.UI;

public class DefenseSelector : MonoBehaviour
{
    [Header("Cooldown Settings")]
    [SerializeField] private float cooldownDuration = 5f; // Time for cooldown
    [SerializeField] private Image cooldownImage; // Image to show cooldown progress

    private Toggle toggle;
    private bool isOnCooldown = false;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();

        // Ensure the cooldown image starts at full
        if (cooldownImage != null)
        {
            cooldownImage.fillAmount = 1f;
        }

        // Listen for toggle state changes
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    private void OnDestroy()
    {
        // Cleanup listeners to prevent memory leaks
        toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
    }

    private void OnToggleValueChanged(bool isOn)
    {
        if (isOn && !isOnCooldown)
        {
            DeployDefense();
        }
        else if (isOn && isOnCooldown)
        {
            Debug.Log($"Defense is on cooldown! Wait {cooldownDuration} seconds.");
            toggle.isOn = false; // Reset toggle state
        }
    }

    private void DeployDefense()
    {
        Debug.Log("Defense deployed!");

        // Start cooldown after deployment
        StartCooldown();
    }

    private void StartCooldown()
    {
        if (!isOnCooldown)
        {
            isOnCooldown = true;
            toggle.interactable = false; // Disable the toggle during cooldown
            cooldownImage.fillAmount = 0f; // Set cooldown image to empty
            StartCoroutine(CooldownRoutine());
        }
    }

    private System.Collections.IEnumerator CooldownRoutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < cooldownDuration)
        {
            elapsedTime += Time.deltaTime;

            // Update cooldown visual
            if (cooldownImage != null)
            {
                cooldownImage.fillAmount = elapsedTime / cooldownDuration; // Increment fill amount
            }

            yield return null;
        }

        // Reset cooldown
        isOnCooldown = false;
        toggle.interactable = true; // Re-enable the toggle
        if (cooldownImage != null)
        {
            cooldownImage.fillAmount = 1f; // Reset image to full
        }

        Debug.Log("Defense ready for deployment again!");
    }
}
