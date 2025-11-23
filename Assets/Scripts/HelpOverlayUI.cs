using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HelpOverlayUI : MonoBehaviour
{
    public CanvasGroup cg;
    public TextMeshProUGUI content;
    public Button closeButton;

    void Awake()
    {
        if (cg) { cg.alpha = 0f; cg.interactable = false; cg.blocksRaycasts = false; }
        if (closeButton) closeButton.onClick.AddListener(Hide);
        gameObject.SetActive(false);
    }

    public void Show(string txt = null)
    {
        if (content && !string.IsNullOrEmpty(txt)) content.text = txt;
        gameObject.SetActive(true);
        if (cg) { cg.alpha = 1f; cg.interactable = true; cg.blocksRaycasts = true; }
    }

    public void Hide()
    {
        if (cg) { cg.alpha = 0f; cg.interactable = false; cg.blocksRaycasts = false; }
        gameObject.SetActive(false);
    }
}
