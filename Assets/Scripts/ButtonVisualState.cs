using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Provides visual feedback for button hover/press states.
/// Attach to any Button GameObject to get color-based feedback.
/// </summary>
[RequireComponent(typeof(Image))]
public class ButtonVisualState : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("State Colors")]
    public Color normal = new Color(0.24f, 0.24f, 0.24f, 1f);
    public Color hover = new Color(0.30f, 0.30f, 0.30f, 1f);
    public Color down = new Color(0.18f, 0.18f, 0.18f, 1f);
    
    private Image img;
    
    void Awake()
    { 
        img = GetComponent<Image>(); 
        if (img) 
            img.color = normal; 
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    { 
        if (img) 
            img.color = hover; 
    }
    
    public void OnPointerExit(PointerEventData eventData)
    { 
        if (img) 
            img.color = normal; 
    }
    
    public void OnPointerDown(PointerEventData eventData)
    { 
        if (img) 
            img.color = down; 
    }
    
    public void OnPointerUp(PointerEventData eventData)
    { 
        if (img) 
            img.color = hover; 
    }
}
