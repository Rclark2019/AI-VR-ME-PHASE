using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class InfoPanelController : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI contentText;
    public Button closeButton;
    public GameObject backgroundBlocker;
    
    [Header("Animation")]
    public float fadeInDuration = 0.2f;
    public CanvasGroup canvasGroup;
    
    private void Awake()
    {
        // Ensure we have a canvas group for fading
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }
    
    private void Start()
    {
        // Wire up close button
        if (closeButton != null)
            closeButton.onClick.AddListener(Close);
        
        // Click background to close
        if (backgroundBlocker != null)
        {
            Button bgButton = backgroundBlocker.GetComponent<Button>();
            if (bgButton == null)
                bgButton = backgroundBlocker.AddComponent<Button>();
            
            // Make background transparent but clickable
            Image bgImage = backgroundBlocker.GetComponent<Image>();
            if (bgImage == null)
                bgImage = backgroundBlocker.AddComponent<Image>();
            bgImage.color = new Color(0, 0, 0, 0.5f);
            
            bgButton.onClick.AddListener(Close);
        }
        
        // Start with fade in
        StartCoroutine(FadeIn());
    }
    

    public void SetContent(string title, string content)
    {
        if (titleText != null)
            titleText.text = title;
        
        if (contentText != null)
            contentText.text = content;
    }
    
    private IEnumerator FadeIn()
    {
        canvasGroup.alpha = 0f;
        float elapsed = 0f;
        
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            yield return null;
        }
        
        canvasGroup.alpha = 1f;
    }
    
    public void Close()
    {
        Destroy(gameObject);
    }
}
