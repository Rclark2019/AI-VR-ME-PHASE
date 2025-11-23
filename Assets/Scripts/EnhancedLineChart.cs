using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class EnhancedLineChart : MonoBehaviour
{
    [Header("Chart Settings")]
    public string chartTitle = "Chart";
    public Color lineColor = Color.green;
    public float minValue = 0f;
    public float maxValue = 100f;
    public string unit = "%";
    [Tooltip("Maximum number of points kept in the buffer (older points trimmed).")]
    public int maxDataPoints = 300;

    public List<Vector2> Points { get; private set; } = new List<Vector2>();

    public event Action OnDataChanged;

    public void ClearChart()
    {
        Points.Clear();
        NotifyChanged();
    }

    public void AddDataPoint(float x, float y)
    {
        Points.Add(new Vector2(x, y));
        TrimIfNeeded();
        NotifyChanged();
    }

    public void AddDataPoint(Vector2 p) => AddDataPoint(p.x, p.y);

    public void AddDataPoints(IEnumerable<Vector2> pts)
    {
        Points.AddRange(pts);
        TrimIfNeeded();
        NotifyChanged();
    }

    private void TrimIfNeeded()
    {
        if (maxDataPoints <= 0) return;
        if (Points.Count <= maxDataPoints) return;

        int overflow = Points.Count - maxDataPoints;
        Points.RemoveRange(0, overflow);
    }

    private void NotifyChanged()
    {
        OnDataChanged?.Invoke();
    }
}
