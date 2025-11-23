using UnityEngine;
using TMPro;

public class ModelLabelUI : MonoBehaviour
{
    [Header("References")]
    public ModelSwitcher modelSwitcher;
    public TextMeshProUGUI labelText;

    private void Update()
    {
        if (modelSwitcher == null || labelText == null)
            return;

        labelText.text = $"Model: {modelSwitcher.GetCurrentModelName()}";
    }
}
