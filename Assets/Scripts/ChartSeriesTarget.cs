using System.Reflection;
using UnityEngine;

public class ChartSeriesTarget : MonoBehaviour
{
    [Header("Target and optional explicit method names")]
    [Tooltip("Drag the component that actually draws the line (e.g., LineChartRenderer)")]
    public Object target;                 
    [Tooltip("Leave blank to auto-detect (AddPoint / Add / Append / ...).")]
    public string addMethod = "";
    [Tooltip("Leave blank to auto-detect (Clear / Reset / ClearData / ...).")]
    public string clearMethod = "";

    // cache
    MethodInfo _add;
    MethodInfo _clear;
    object[] _args = new object[1];

    static readonly string[] AddNames   = { "AddPoint", "Append", "AddSample", "AddValue", "Push", "Add", "AddDataPoint" };
    static readonly string[] ClearNames = { "Clear", "Reset", "ResetData", "ClearData", "ClearPoints" };

    void Awake() => Resolve();

    // ---- Compatibility aliases for your ChartFeederUI ----
    // ChartFeederUI calls Init() once per series:
    public void Init() => Resolve();
    // ChartFeederUI calls Push(value) each frame:
    public void Push(float v) => Add(v);
    // Optional: if your feeder calls a named clear
    public void ClearSeries() => Clear();

    // ---- Public API used by the aliases above ----
    public void Add(float v)
    {
        if (_add == null) Resolve();
        if (_add != null)
        {
            _args[0] = v;
            _add.Invoke(target, _args);
        }
    }

    public void Clear()
    {
        if (_clear == null) Resolve();
        _clear?.Invoke(target, null);
    }

    // ---- Wiring / reflection ------------------------------------------------
    public void Resolve()
    {
        _add   = FindAdd(addMethod);
        _clear = FindClear(clearMethod);

        if (_add == null)
            Debug.LogWarning($"ChartSeriesTarget: could not find an 'add point' method on '{(target ? target.name : "<null>")}'. " +
                             $"Set 'Add Method' in the inspector if the API name/signature is different.");

        if (_clear == null)
            Debug.LogWarning($"ChartSeriesTarget: could not find a 'clear' method on '{(target ? target.name : "<null>")}'. " +
                             $"Set 'Clear Method' in the inspector if the API name/signature is different.");
    }

    MethodInfo FindAdd(string explicitName)
    {
        if (TryFind(explicitName, true, out var mi)) return mi;
        foreach (var n in AddNames) if (TryFind(n, true, out mi)) return mi;
        return null;
    }

    MethodInfo FindClear(string explicitName)
    {
        if (TryFind(explicitName, false, out var mi)) return mi;
        foreach (var n in ClearNames) if (TryFind(n, false, out mi)) return mi;
        return null;
    }

    bool TryFind(string methodName, bool expectsFloat, out MethodInfo mi)
    {
        mi = null;
        if (target == null) return false;

        // If they dragged a Component, search it and siblings on same GO.
        if (target is Component comp)
        {
            if (TryFindOnType(comp.GetType(), methodName, expectsFloat, out mi)) return true;

            var all = comp.GetComponents<MonoBehaviour>();
            foreach (var c in all)
            {
                if (TryFindOnType(c.GetType(), methodName, expectsFloat, out mi))
                {
                    target = c; // repoint to the component that actually has the methods
                    return true;
                }
            }
            return false;
        }

        // ScriptableObject or other object
        return TryFindOnType(target.GetType(), methodName, expectsFloat, out mi);
    }

    bool TryFindOnType(System.Type t, string name, bool expectsFloat, out MethodInfo mi)
    {
        mi = null;
        if (t == null) return false;

        const BindingFlags BF = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        // if explicit name given, try it first
        if (!string.IsNullOrEmpty(name))
        {
            mi = expectsFloat
                ? t.GetMethod(name, BF, null, new[] { typeof(float) }, null)
                : t.GetMethod(name, BF, null, System.Type.EmptyTypes, null);
            if (mi != null) return true;
        }

        // discover common names
        if (expectsFloat)
        {
            foreach (var n in AddNames)
            {
                mi = t.GetMethod(n, BF, null, new[] { typeof(float) }, null);
                if (mi != null) return true;
            }
        }
        else
        {
            foreach (var n in ClearNames)
            {
                mi = t.GetMethod(n, BF, null, System.Type.EmptyTypes, null);
                if (mi != null) return true;
            }
        }
        return false;
    }
}
