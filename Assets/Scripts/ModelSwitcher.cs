using UnityEngine;

public class ModelSwitcher : MonoBehaviour
{
    [Tooltip("List of available DataGenerators (Model A/B/C)")]
    [SerializeField] private DataGenerator[] generators;

    private int currentIndex = 0;

    // Cycle to the next model in the list.  Wraps around at the end.
    public void SwitchToNextModel()
    {
        if (generators == null || generators.Length == 0) return;
        currentIndex = (currentIndex + 1) % generators.Length;
    }

    public DataGenerator GetActiveGenerator()
    {
        if (generators == null || generators.Length == 0) return null;
        return generators[currentIndex];
    }

    // Returns a short key identifying the current model ("A", "B", "C", ...).
    public string GetActiveModelKey()
    {
        // Map index to letter; fallback to question mark
        char letter = (char)('A' + currentIndex);
        return letter.ToString();
    }

    // Returns the human‑readable model name from the generator.
    public string GetCurrentModelName()
    {
        var generator = GetActiveGenerator();
        return generator != null ? generator.ModelName : "Unknown Model";
    }
}