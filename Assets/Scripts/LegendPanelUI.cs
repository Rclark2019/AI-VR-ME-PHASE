using UnityEngine;
using UnityEngine.UI;

public class LegendPanelUI : MonoBehaviour
{
    [SerializeField] private Image goodDot;
    [SerializeField] private Image cautionDot;
    [SerializeField] private Image alertDot;
    [SerializeField] private Color goodColor = new Color(0f, 1f, 0f);
    [SerializeField] private Color cautionColor = new Color(1f, 0.78f, 0f);
    [SerializeField] private Color alertColor = new Color(1f, 0.2f, 0.2f);

    private void Awake()
    {
        if (goodDot != null) goodDot.color = goodColor;
        if (cautionDot != null) cautionDot.color = cautionColor;
        if (alertDot != null) alertDot.color = alertColor;
    }
}