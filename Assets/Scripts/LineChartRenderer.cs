using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(CanvasRenderer))]
public class LineChartRenderer : Graphic
{
    [Header("Series")]
    [SerializeField] private int maxDataPoints = 300;
    [SerializeField] private float minValue = 0f;
    [SerializeField] private float maxValue = 1f;
    [SerializeField] private float lineWidth = 2f;
    [SerializeField] private Color gridColor = new Color(0.4f, 0.4f, 0.4f, 0.3f);
    [SerializeField] private bool showGrid = true;
    [SerializeField] private int gridLines = 4;

    private readonly List<float> _points = new List<float>();

    // -------- Public API --------

    public int MaxDataPoints
    {
        get => maxDataPoints;
        set
        {
            maxDataPoints = Mathf.Max(1, value);
            TrimIfNeeded();
            SetVerticesDirty();
        }
    }

    public float MinValue
    {
        get => minValue;
        set { minValue = value; SetVerticesDirty(); }
    }

    public float MaxValue
    {
        get => maxValue;
        set { maxValue = value; SetVerticesDirty(); }
    }

    public void AddDataPoint(float value)
    {
        AddPoint(value);
    }

    public void AddPoint(float value)
    {
        float v = Mathf.Clamp(value, minValue, maxValue);
        _points.Add(v);
        TrimIfNeeded();
        SetVerticesDirty();
    }

    public void Clear()
    {
        _points.Clear();
        SetVerticesDirty();
    }

    public void ClearDataPoints()
    {
        Clear();
    }

    public void ClearData()
    {
        Clear();
    }

    // -------- Internal helpers --------

    private void TrimIfNeeded()
    {
        if (maxDataPoints <= 0) maxDataPoints = 1;
        int extra = _points.Count - maxDataPoints;
        if (extra > 0)
        {
            _points.RemoveRange(0, extra);
        }
    }

    // -------- Rendering --------

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (_points.Count < 2)
            return;

        Rect r = rectTransform.rect;

        // Grid lines
        if (showGrid && gridLines > 0)
        {
            float stepY = r.height / (gridLines + 1);
            for (int i = 1; i <= gridLines; i++)
            {
                float y = r.yMin + stepY * i;
                AddQuad(vh,
                    new Vector2(r.xMin, y - 0.5f),
                    new Vector2(r.xMax, y + 0.5f),
                    gridColor);
            }
        }

        float range = Mathf.Max(0.0001f, maxValue - minValue);
        float dx = r.width / Mathf.Max(1, _points.Count - 1);

        Vector2 prev = Vector2.zero;
        bool hasPrev = false;

        for (int i = 0; i < _points.Count; i++)
        {
            float t = (_points[i] - minValue) / range;
            float x = r.xMin + dx * i;
            float y = Mathf.Lerp(r.yMin, r.yMax, t);

            Vector2 current = new Vector2(x, y);

            if (hasPrev)
            {
                AddLineSegment(vh, prev, current, lineWidth, color);
            }

            prev = current;
            hasPrev = true;
        }
    }

    private static void AddLineSegment(VertexHelper vh, Vector2 a, Vector2 b, float width, Color c)
    {
        Vector2 dir = (b - a).normalized;
        Vector2 normal = new Vector2(-dir.y, dir.x);
        float half = width * 0.5f;

        Vector2 v0 = a + normal * half;
        Vector2 v1 = a - normal * half;
        Vector2 v2 = b - normal * half;
        Vector2 v3 = b + normal * half;

        int start = vh.currentVertCount;

        vh.AddVert(v0, c, new Vector2(0, 0));
        vh.AddVert(v1, c, new Vector2(0, 1));
        vh.AddVert(v2, c, new Vector2(1, 1));
        vh.AddVert(v3, c, new Vector2(1, 0));

        vh.AddTriangle(start + 0, start + 1, start + 2);
        vh.AddTriangle(start + 2, start + 3, start + 0);
    }

    private static void AddQuad(VertexHelper vh, Vector2 bottomLeft, Vector2 topRight, Color c)
    {
        int start = vh.currentVertCount;

        vh.AddVert(new Vector2(bottomLeft.x, bottomLeft.y), c, new Vector2(0, 0));
        vh.AddVert(new Vector2(bottomLeft.x, topRight.y), c, new Vector2(0, 1));
        vh.AddVert(new Vector2(topRight.x, topRight.y), c, new Vector2(1, 1));
        vh.AddVert(new Vector2(topRight.x, bottomLeft.y), c, new Vector2(1, 0));

        vh.AddTriangle(start + 0, start + 1, start + 2);
        vh.AddTriangle(start + 2, start + 3, start + 0);
    }
}
