using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

/// <summary>
/// Manages the selection, deployment, and indicator display for defense mechanisms.
/// Handles interactions between the player and defense objects.
/// </summary>
public class DefenseManager : MonoBehaviour
{
    #region Serialized Fields (Unity Inspector)

    [Header("UI Elements")] [Tooltip("Toggle group containing defense selection toggles.")] [SerializeField]
    private ToggleGroup toggleGroup;

    [Header("Defense Settings")]
    [Tooltip("List of defense prefabs to be deployed (indexed by DefenseType).")]
    [SerializeField]
    private List<GameObject> defensePrefabs;

    [Header("Indicator Settings")] [Tooltip("LineRenderer used to display the AoE indicator.")] [SerializeField]
    private LineRenderer indicator;

    [Header("Other References")] [Tooltip("The main camera used for raycasting during deployment.")] [SerializeField]
    private Camera mainCamera;

    [Tooltip("The transform representing the player's base position.")] [SerializeField]
    private Transform playerBase;

    #endregion

    #region Private Fields

    private GameObject selectedDefense; // Tracks the currently selected defense prefab.
    private Vector3 center; // The center position for the AoE indicator.
    private float indicatorRadius = 0f; // Radius of the AoE indicator for the selected defense.
    private bool isIndicatorActive = false; // Flag to indicate whether the indicator should be visible.
    private bool isDeployAllowed = true; // Flag to determine if deployment is allowed at the current position.
    [SerializeField] string selectedKey; // Key of the selected defense prefab.


    #endregion

    #region Unity Lifecycle Methods

    /// <summary>
    /// Initializes the defense manager by creating object pools for each defense prefab.
    /// This prepares the pool manager with reusable objects to enhance performance.
    /// </summary>
    private void Start()
    {
        foreach (var prefab in defensePrefabs)
        {
            string key = prefab.name;
            PoolManager.Instance.CreatePool(key, prefab, 4); // Adjust pool size
        }
    }

    /// <summary>
    /// Updates the defense manager behavior every frame, handling defense selection, deployment, and deselection.
    /// </summary>
    private void Update()
    {
        // Update the indicator position if active.
        if (isIndicatorActive)
        {
            UpdateIndicatorPosition();
        }

        // Handle deployment on left-click if selected defense is valid and UI is not active.
        if (Input.GetMouseButtonDown(0) && selectedDefense != null && !IsPointerOverUI() && isDeployAllowed)
        {
            DeployDefense();
        }
        else if (Input.GetMouseButtonDown(0) && selectedDefense != null && !IsPointerOverUI() && !isDeployAllowed)
        {
            DeselectDefense();
        }

        // Handle deselection on right-click.
        if (Input.GetMouseButtonDown(1))
        {
            DeselectDefense();
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Handles the selection of a defense based on its index in the `defensePrefabs` list.
    /// Activates the AoE indicator if the selected defense has one.
    /// </summary>
    /// <param name="defenseTypeIndex">Index of the defense in the defensePrefabs list.</param>
    public void OnDefenseSelected(int defenseTypeIndex)
    {
        if (defenseTypeIndex >= 0 && defenseTypeIndex < defensePrefabs.Count)
        {
            selectedDefense = defensePrefabs[defenseTypeIndex];
            selectedKey = selectedDefense.name;
            
            // Check if an AoE indicator should be activated for the selected defense.
            if (selectedDefense.TryGetComponent(out Defense defense))
            {
                ActivateIndicator(defense.radius);
            }
        }
        else
        {
            // Invalid defense type index, clear selection and deactivate indicator.
            selectedDefense = null;
            DeactivateIndicator();
        }
    }

    #endregion

    #region Deployment and Indicator Management

    /// <summary>
    /// Deploys the currently selected defense at the point clicked by the player.
    /// </summary>
    private void DeployDefense()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Instantiate the defense at the hit location.
            GameObject defenseInstance = PoolManager.Instance.GetFromPool(selectedKey, hit.point, Quaternion.identity);

            // If an Anti-Rocket Shield is being deployed, adjust its behavior accordingly.
            if (selectedDefense.TryGetComponent(out AntiRocketShield antiRocketShield))
            {
                defenseInstance.GetComponent<Defense>().Deploy(playerBase.position);
            }
            else
            {
                defenseInstance.GetComponent<Defense>().Deploy(hit.point);
            }
            
            defenseInstance.GetComponent<Defense>().InitializePoolKey(selectedKey);
        }

        // Deselect defense after deployment.
        DeselectDefense();
    }

    /// <summary>
    /// Clears the current defense selection and deactivates the indicator.
    /// </summary>
    private void DeselectDefense()
    {
        foreach (Toggle toggle in toggleGroup.ActiveToggles())
        {
            toggle.isOn = false; // Deactivate all toggles in the group.
        }

        selectedDefense = null;
        DeactivateIndicator();
    }

    /// <summary>
    /// Activates the AoE indicator around the player's cursor.
    /// </summary>
    /// <param name="radius">The radius of the AoE indicator.</param>
    private void ActivateIndicator(float radius)
    {
        isIndicatorActive = true;
        indicator.positionCount = 51; // Set resolution for the circle (50 segments + 1 to close the loop).
        indicatorRadius = radius;
        indicator.loop = true;
        indicator.enabled = true;
    }

    /// <summary>
    /// Updates the position and shape of the indicator based on the player's cursor position.
    /// </summary>
    private void UpdateIndicatorPosition()
    {
        if (!isIndicatorActive) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Check if deployment location is valid for specific defenses.
            if (selectedDefense.TryGetComponent(out WaterMine waterMine))
            {
                if (hit.collider.CompareTag("Water"))
                {
                    center = hit.point;
                    isDeployAllowed = true;
                }
                else
                {
                    isDeployAllowed = false;
                    center = transform.position + Vector3.up * 1000f; // Move indicator far away.
                }
            }
            else if (selectedDefense.TryGetComponent(out SAMSite samSite))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("PlayerBase"))
                {
                    isDeployAllowed = true;
                    center = hit.point;

                }
                else
                {
                    isDeployAllowed = false;
                    center = transform.position + Vector3.up * 1000f; // Move indicator far away.
                }
                
            }
            else if (selectedDefense.TryGetComponent(out AntiRocketShield antiRocketShield))
            {
                center = playerBase.position; // Adjust center position for Anti-Rocket Shield.
            }
            else
            {
                center = hit.point; // Default behavior.
            }
            // Draw the circular AoE indicator.
            float angle = 0f;
            for (int i = 0; i < indicator.positionCount; i++)
            {
                float x = Mathf.Cos(Mathf.Deg2Rad * angle) * indicatorRadius;
                float z = Mathf.Sin(Mathf.Deg2Rad * angle) * indicatorRadius;
                indicator.SetPosition(i, new Vector3(x, 0, z) + center);
                angle += 360f / (indicator.positionCount - 1);
            }
        }
    }

    /// <summary>
    /// Deactivates the AoE indicator.
    /// </summary>
    private void DeactivateIndicator()
    {
        isIndicatorActive = false;
        indicator.enabled = false;
    }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Checks if the player's cursor is currently over a UI element.
    /// </summary>
    /// <returns>True if the cursor is over UI; otherwise, false.</returns>
    private bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    #endregion
}