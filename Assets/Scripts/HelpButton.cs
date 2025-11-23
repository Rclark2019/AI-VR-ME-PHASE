using UnityEngine;
using UnityEngine.UI;

public class HelpButton : MonoBehaviour
{
    [Header("References")]
    public Button helpButton;
    public GameObject infoPanelPrefab;
    
    [Header("Help Content")]
    public string helpTitle = "Information";
    
    [TextArea(10, 30)]
    public string helpContent = "This panel displays important information about the system...";
    
    [Header("Button Icon (Optional)")]
    public bool useIconText = true;
    public string iconText = "ⓘ";
    
    private GameObject activeInfoPanel;
    
    private void Start()
    {
        if (helpButton == null)
            helpButton = GetComponent<Button>();
        
        if (helpButton != null)
            helpButton.onClick.AddListener(ShowInfoPanel);
        
        // Set icon if using text
        if (useIconText && helpButton != null)
        {
            var tmpText = helpButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (tmpText != null)
                tmpText.text = iconText;
        }
    }
    
    public void ShowInfoPanel()
    {
        if (activeInfoPanel != null)
        {
            Debug.Log("Info panel already open for this help button");
            return;
        }
        
        if (infoPanelPrefab == null)
        {
            Debug.LogError("InfoPanel prefab not assigned to HelpButton!");
            
            // Show toast notification if available
            if (ToastManager.Instance != null)
                ToastManager.Instance.Show("Help system not configured", 2f);
            
            return;
        }
        
        // Find the main canvas
        Canvas canvas = FindMainCanvas();
        if (canvas == null)
        {
            Debug.LogError("No Canvas found in scene!");
            return;
        }
        
        // Instantiate the info panel
        activeInfoPanel = Instantiate(infoPanelPrefab, canvas.transform);
        activeInfoPanel.transform.SetAsLastSibling(); // Render on top
        
        // Set the content
        InfoPanelController controller = activeInfoPanel.GetComponent<InfoPanelController>();
        if (controller != null)
        {
            controller.SetContent(helpTitle, helpContent);
        }
        else
        {
            Debug.LogWarning("InfoPanelController not found on prefab!");
        }
    }
    
    private Canvas FindMainCanvas()
        {
            // Try to find canvas in parents first
            Canvas canvas = GetComponentInParent<Canvas>();
            
            if (canvas == null)
            {
    #if UNITY_2023_1_OR_NEWER
                canvas = FindAnyObjectByType<Canvas>();
    #else
                canvas = FindObjectOfType<Canvas>();
    #endif
            }
            
            return canvas;
        }
    
    private void OnDestroy()
    {
        // Clean up if button is destroyed
        if (activeInfoPanel != null)
            Destroy(activeInfoPanel);
    }
}
