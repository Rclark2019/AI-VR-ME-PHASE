using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea] public string tooltipText = "";
    public RectTransform tooltipPanel;      // TooltipPanel
    public TMP_Text tooltipTextComponent;   // TooltipText
    public Vector2 offset = new(0, 48);

    public void OnPointerEnter(PointerEventData e)
    {
        if (!tooltipPanel || !tooltipTextComponent) return;

        tooltipTextComponent.text = tooltipText;
        tooltipPanel.gameObject.SetActive(true);
        tooltipPanel.position = (Vector2)transform.position + offset;
    }

    public void OnPointerExit(PointerEventData e)
    {
        if (tooltipPanel)
            tooltipPanel.gameObject.SetActive(false);
    }
}
