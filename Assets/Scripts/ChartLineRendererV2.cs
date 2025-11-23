using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Complete informative chart UI that tells users EVERYTHING they need to know.
/// Shows: Title, current value with label, axis labels with units, grid, and status.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class InformativeChartUI : MonoBehaviour
{
    [Header("Data Source")]
    public EnhancedLineChart sourceChart;
    public UILineRenderer lineRenderer;

    [Header("Visual Style")]
    [Range(1f, 10f)] public float lineWidth = 3f;
    public Color lineColor = Color.green;
    public Color gridColor = new Color(0.3f, 0.3f, 0.3f, 0.3f);
    public Color textColor = new Color(0.9f, 0.9f, 0.9f, 1f);
    public Color titleColor = Color.white;

    [Header("Display Options")]
    public bool showTitle = true;
    public bool showCurrentValue = true;
    public bool showGridLines = true;
    public bool showAxisLabels = true;
    [Range(3, 10)] public int numberOfAxisLabels = 5;

    [Header("Text Configuration")]
    [Tooltip("Override chart title (leave empty to use sourceChart.chartTitle)")]
    public string customTitle = "";
    [Tooltip("Label for current value (e.g., 'Current Accuracy:', 'Speed:')")]
    public string currentValueLabel = "Current:";
    [Tooltip("Show units on axis labels")]
    public bool showUnitsOnAxis = true;

    [Header("Layout Settings")]
    public float labelMargin = 65f;
    public float topMargin = 45f;
    public float bottomMargin = 35f;
    public float rightMargin = 15f;
    public bool useLayoutElement = true;
    public float preferredHeight = 220f;

    [Header("Performance")]
    [Range(1, 10)] public int updateInterval = 2;

    // UI Elements
    private TMP_Text titleText;
    private TMP_Text currentValueText;
    private TMP_Text currentValueLabelText;
    private GameObject currentValuePanel;
    private List<TMP_Text> yAxisLabels = new List<TMP_Text>();
    private List<TMP_Text> xAxisLabels = new List<TMP_Text>();
    private List<Image> gridLines = new List<Image>();
    private TMP_Text yAxisTitleText;
    private TMP_Text xAxisTitleText;
    
    private readonly List<Vector2> _uiPoints = new List<Vector2>();
    private int _frameCounter = 0;
    private bool _uiBuilt = false;
    private LayoutElement _layoutElement;

    private void Awake()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<UILineRenderer>();
            
        if (useLayoutElement)
        {
            _layoutElement = GetComponent<LayoutElement>();
            if (_layoutElement == null)
                _layoutElement = gameObject.AddComponent<LayoutElement>();
                
            _layoutElement.preferredHeight = preferredHeight;
            _layoutElement.flexibleHeight = 1;
        }
    }

    private void Start()
    {
        BuildUI();
    }

    private void BuildUI()
    {
        if (_uiBuilt) return;

        if (showTitle) CreateTitle();
        if (showCurrentValue) CreateCurrentValueDisplay();
        if (showGridLines) CreateGridLines();
        if (showAxisLabels) CreateAxisLabels();
        CreateAxisTitles();

        _uiBuilt = true;
    }

    private void CreateTitle()
    {
        GameObject titleObj = new GameObject("ChartTitle");
        titleObj.transform.SetParent(transform, false);

        RectTransform titleRT = titleObj.AddComponent<RectTransform>();
        titleRT.anchorMin = new Vector2(0, 1);
        titleRT.anchorMax = new Vector2(1, 1);
        titleRT.pivot = new Vector2(0.5f, 1);
        titleRT.anchoredPosition = new Vector2(0, -5);
        titleRT.sizeDelta = new Vector2(-20, 30);

        titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.fontSize = 14;
        titleText.fontStyle = FontStyles.Bold;
        titleText.color = titleColor;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.text = GetDisplayTitle();
    }

    private void CreateCurrentValueDisplay()
    {
        // Panel background
        GameObject panelObj = new GameObject("CurrentValuePanel");
        panelObj.transform.SetParent(transform, false);

        RectTransform panelRT = panelObj.AddComponent<RectTransform>();
        panelRT.anchorMin = new Vector2(1, 1);
        panelRT.anchorMax = new Vector2(1, 1);
        panelRT.pivot = new Vector2(1, 1);
        panelRT.anchoredPosition = new Vector2(-10, -topMargin + 5);
        panelRT.sizeDelta = new Vector2(120, 60);

        Image panelImg = panelObj.AddComponent<Image>();
        panelImg.color = new Color(0.15f, 0.15f, 0.15f, 0.9f);
        currentValuePanel = panelObj;

        // Label text (e.g., "Current Accuracy:")
        GameObject labelObj = new GameObject("ValueLabel");
        labelObj.transform.SetParent(panelObj.transform, false);

        RectTransform labelRT = labelObj.AddComponent<RectTransform>();
        labelRT.anchorMin = new Vector2(0, 0.6f);
        labelRT.anchorMax = new Vector2(1, 1);
        labelRT.sizeDelta = Vector2.zero;
        labelRT.offsetMin = new Vector2(5, 0);
        labelRT.offsetMax = new Vector2(-5, -3);

        currentValueLabelText = labelObj.AddComponent<TextMeshProUGUI>();
        currentValueLabelText.fontSize = 10;
        currentValueLabelText.color = new Color(0.7f, 0.7f, 0.7f, 1f);
        currentValueLabelText.alignment = TextAlignmentOptions.Top;
        currentValueLabelText.text = currentValueLabel;

        // Value text (e.g., "85.3%")
        GameObject valueObj = new GameObject("ValueText");
        valueObj.transform.SetParent(panelObj.transform, false);

        RectTransform valueRT = valueObj.AddComponent<RectTransform>();
        valueRT.anchorMin = new Vector2(0, 0);
        valueRT.anchorMax = new Vector2(1, 0.6f);
        valueRT.sizeDelta = Vector2.zero;
        valueRT.offsetMin = new Vector2(5, 3);
        valueRT.offsetMax = new Vector2(-5, 0);

        currentValueText = valueObj.AddComponent<TextMeshProUGUI>();
        currentValueText.fontSize = 22;
        currentValueText.fontStyle = FontStyles.Bold;
        currentValueText.color = Color.white;
        currentValueText.alignment = TextAlignmentOptions.Center;
    }

    private void CreateAxisTitles()
    {
        // Y-axis title (vertical, on the left)
        GameObject yTitleObj = new GameObject("YAxisTitle");
        yTitleObj.transform.SetParent(transform, false);

        RectTransform yTitleRT = yTitleObj.AddComponent<RectTransform>();
        yTitleRT.anchorMin = new Vector2(0, 0.5f);
        yTitleRT.anchorMax = new Vector2(0, 0.5f);
        yTitleRT.pivot = new Vector2(0.5f, 0.5f);
        yTitleRT.anchoredPosition = new Vector2(15, 0);
        yTitleRT.sizeDelta = new Vector2(150, 20);
        yTitleRT.localRotation = Quaternion.Euler(0, 0, 90);

        yAxisTitleText = yTitleObj.AddComponent<TextMeshProUGUI>();
        yAxisTitleText.fontSize = 11;
        yAxisTitleText.fontStyle = FontStyles.Bold;
        yAxisTitleText.color = textColor;
        yAxisTitleText.alignment = TextAlignmentOptions.Center;
        // Will be set in UpdateAxisTitles()

        // X-axis title (horizontal, at the bottom)
        GameObject xTitleObj = new GameObject("XAxisTitle");
        xTitleObj.transform.SetParent(transform, false);

        RectTransform xTitleRT = xTitleObj.AddComponent<RectTransform>();
        xTitleRT.anchorMin = new Vector2(0.5f, 0);
        xTitleRT.anchorMax = new Vector2(0.5f, 0);
        xTitleRT.pivot = new Vector2(0.5f, 0);
        xTitleRT.anchoredPosition = new Vector2((labelMargin - rightMargin) / 2, 3);
        xTitleRT.sizeDelta = new Vector2(150, 20);

        xAxisTitleText = xTitleObj.AddComponent<TextMeshProUGUI>();
        xAxisTitleText.fontSize = 11;
        xAxisTitleText.fontStyle = FontStyles.Bold;
        xAxisTitleText.color = textColor;
        xAxisTitleText.alignment = TextAlignmentOptions.Center;
        xAxisTitleText.text = "Time (seconds)";
    }

    private void CreateGridLines()
    {
        GameObject gridContainer = new GameObject("GridContainer");
        gridContainer.transform.SetParent(transform, false);
        
        RectTransform containerRT = gridContainer.AddComponent<RectTransform>();
        containerRT.anchorMin = Vector2.zero;
        containerRT.anchorMax = Vector2.one;
        containerRT.offsetMin = new Vector2(labelMargin, bottomMargin);
        containerRT.offsetMax = new Vector2(-rightMargin, -topMargin);
        
        gridContainer.transform.SetAsFirstSibling();

        // Horizontal grid lines
        for (int i = 0; i < numberOfAxisLabels; i++)
        {
            GameObject lineObj = new GameObject($"HGridLine_{i}");
            lineObj.transform.SetParent(gridContainer.transform, false);

            RectTransform lineRT = lineObj.AddComponent<RectTransform>();
            lineRT.anchorMin = new Vector2(0, 0);
            lineRT.anchorMax = new Vector2(1, 0);
            lineRT.pivot = new Vector2(0, 0.5f);
            lineRT.sizeDelta = new Vector2(0, 1);

            Image lineImg = lineObj.AddComponent<Image>();
            lineImg.color = gridColor;
            gridLines.Add(lineImg);
        }

        // Vertical grid lines
        for (int i = 0; i < numberOfAxisLabels; i++)
        {
            GameObject lineObj = new GameObject($"VGridLine_{i}");
            lineObj.transform.SetParent(gridContainer.transform, false);

            RectTransform lineRT = lineObj.AddComponent<RectTransform>();
            lineRT.anchorMin = new Vector2(0, 0);
            lineRT.anchorMax = new Vector2(0, 1);
            lineRT.pivot = new Vector2(0.5f, 0);
            lineRT.sizeDelta = new Vector2(1, 0);

            Image lineImg = lineObj.AddComponent<Image>();
            lineImg.color = gridColor;
            gridLines.Add(lineImg);
        }
    }

    private void CreateAxisLabels()
    {
        // Y-axis value labels
        for (int i = 0; i < numberOfAxisLabels; i++)
        {
            GameObject labelObj = new GameObject($"YLabel_{i}");
            labelObj.transform.SetParent(transform, false);

            RectTransform labelRT = labelObj.AddComponent<RectTransform>();
            labelRT.anchorMin = new Vector2(0, 0);
            labelRT.anchorMax = new Vector2(0, 0);
            labelRT.pivot = new Vector2(1, 0.5f);
            labelRT.sizeDelta = new Vector2(labelMargin - 25, 20);

            TMP_Text labelText = labelObj.AddComponent<TextMeshProUGUI>();
            labelText.fontSize = 10;
            labelText.color = textColor;
            labelText.alignment = TextAlignmentOptions.Right;
            labelText.textWrappingMode = TextWrappingModes.NoWrap;
            
            yAxisLabels.Add(labelText);
        }

        // X-axis time labels
        for (int i = 0; i < numberOfAxisLabels; i++)
        {
            GameObject labelObj = new GameObject($"XLabel_{i}");
            labelObj.transform.SetParent(transform, false);

            RectTransform labelRT = labelObj.AddComponent<RectTransform>();
            labelRT.anchorMin = new Vector2(0, 0);
            labelRT.anchorMax = new Vector2(0, 0);
            labelRT.pivot = new Vector2(0.5f, 1);
            labelRT.sizeDelta = new Vector2(50, 20);

            TMP_Text labelText = labelObj.AddComponent<TextMeshProUGUI>();
            labelText.fontSize = 10;
            labelText.color = textColor;
            labelText.alignment = TextAlignmentOptions.Center;
            labelText.textWrappingMode = TextWrappingModes.NoWrap;
            
            xAxisLabels.Add(labelText);
        }
    }

    private void LateUpdate()
    {
        _frameCounter++;
        if (_frameCounter < updateInterval)
            return;
        _frameCounter = 0;

        if (!_uiBuilt)
            BuildUI();

        UpdateChart();
    }

    private void UpdateChart()
    {
        if (sourceChart == null || lineRenderer == null)
            return;

        var points = sourceChart.Points;
        if (points == null || points.Count < 2)
        {
            if (lineRenderer != null)
            {
                lineRenderer.Points = null;
                lineRenderer.SetAllDirty();
            }
            return;
        }

        UpdateLine(points);
        UpdateAxisLabels(points);
        UpdateGridPositions();
        UpdateCurrentValue(points);
        UpdateAxisTitles();
    }

    private void UpdateLine(List<Vector2> points)
    {
        RectTransform rt = (RectTransform)transform;
        Rect r = rt.rect;

        float chartWidth = r.width - labelMargin - rightMargin;
        float chartHeight = r.height - topMargin - bottomMargin;
        float chartLeft = r.xMin + labelMargin;
        float chartBottom = r.yMin + bottomMargin;

        float minX = points[0].x;
        float maxX = points[points.Count - 1].x;
        float dx = Mathf.Max(0.0001f, maxX - minX);

        float minY = sourceChart.minValue;
        float maxY = sourceChart.maxValue;
        float dy = Mathf.Max(0.0001f, maxY - minY);

        _uiPoints.Clear();

        foreach (var p in points)
        {
            float tx = (p.x - minX) / dx;
            float ty = (p.y - minY) / dy;

            float x = chartLeft + tx * chartWidth;
            float y = chartBottom + ty * chartHeight;

            _uiPoints.Add(new Vector2(x, y));
        }

        ApplyLineStyle();
        lineRenderer.Points = _uiPoints.ToArray();
        lineRenderer.SetAllDirty();
    }

    private void UpdateAxisLabels(List<Vector2> points)
    {
        if (sourceChart == null) return;

        RectTransform rt = (RectTransform)transform;
        Rect r = rt.rect;

        float chartBottom = r.yMin + bottomMargin;
        float chartTop = r.yMax - topMargin;
        float chartHeight = chartTop - chartBottom;

        float chartLeft = r.xMin + labelMargin;
        float chartRight = r.xMax - rightMargin;
        float chartWidth = chartRight - chartLeft;

        // Y-axis labels with units
        for (int i = 0; i < yAxisLabels.Count; i++)
        {
            float t = (float)i / (yAxisLabels.Count - 1);
            float value = Mathf.Lerp(sourceChart.minValue, sourceChart.maxValue, t);
            
            string labelText = value.ToString("F0");
            if (showUnitsOnAxis && !string.IsNullOrEmpty(sourceChart.unit))
            {
                labelText += sourceChart.unit;
            }
            
            yAxisLabels[i].text = labelText;
            
            var labelRT = yAxisLabels[i].GetComponent<RectTransform>();
            float yPos = chartBottom + t * chartHeight;
            labelRT.anchoredPosition = new Vector2(chartLeft - 5, yPos);
        }

        // X-axis labels with time formatting
        if (points.Count > 0)
        {
            float minX = points[0].x;
            float maxX = points[points.Count - 1].x;

            for (int i = 0; i < xAxisLabels.Count; i++)
            {
                float t = (float)i / (xAxisLabels.Count - 1);
                float timeValue = Mathf.Lerp(minX, maxX, t);
                
                if (timeValue < 60)
                    xAxisLabels[i].text = timeValue.ToString("F0") + "s";
                else
                {
                    int minutes = Mathf.FloorToInt(timeValue / 60);
                    int seconds = Mathf.FloorToInt(timeValue % 60);
                    xAxisLabels[i].text = $"{minutes}:{seconds:D2}";
                }

                var labelRT = xAxisLabels[i].GetComponent<RectTransform>();
                float xPos = chartLeft + t * chartWidth;
                labelRT.anchoredPosition = new Vector2(xPos, chartBottom - 7);
            }
        }
    }

    private void UpdateAxisTitles()
    {
        if (sourceChart == null) return;

        // Y-axis title based on metric and unit
        if (yAxisTitleText != null)
        {
            string metricName = GetMetricName();
            string unit = sourceChart.unit ?? "";
            
            if (!string.IsNullOrEmpty(unit))
                yAxisTitleText.text = $"{metricName} ({unit})";
            else
                yAxisTitleText.text = metricName;
        }
    }

    private void UpdateGridPositions()
    {
        if (gridLines.Count == 0) return;

        RectTransform rt = (RectTransform)transform;
        Rect r = rt.rect;

        float chartBottom = bottomMargin;
        float chartTop = r.height - topMargin;
        float chartHeight = chartTop - chartBottom;
        float chartWidth = r.width - labelMargin - rightMargin;

        int halfCount = gridLines.Count / 2;

        // Horizontal grid lines
        for (int i = 0; i < halfCount; i++)
        {
            float t = (float)i / (halfCount - 1);
            float yPos = chartBottom + t * chartHeight;
            var lineRT = gridLines[i].GetComponent<RectTransform>();
            lineRT.anchoredPosition = new Vector2(0, yPos);
        }

        // Vertical grid lines
        for (int i = halfCount; i < gridLines.Count; i++)
        {
            int index = i - halfCount;
            float t = (float)index / (halfCount - 1);
            float xPos = t * chartWidth;
            var lineRT = gridLines[i].GetComponent<RectTransform>();
            lineRT.anchoredPosition = new Vector2(xPos, 0);
        }
    }

    private void UpdateCurrentValue(List<Vector2> points)
    {
        if (currentValueText == null || points.Count == 0) return;

        float latestValue = points[points.Count - 1].y;
        
        // Format value with appropriate precision
        string valueStr;
        if (sourceChart.unit == "%" || sourceChart.maxValue == 100)
            valueStr = latestValue.ToString("F1");
        else
            valueStr = latestValue.ToString("F2");
            
        if (sourceChart != null && !string.IsNullOrEmpty(sourceChart.unit))
        {
            valueStr += sourceChart.unit;
        }
        
        currentValueText.text = valueStr;

        // Update label
        if (currentValueLabelText != null && !string.IsNullOrEmpty(currentValueLabel))
        {
            currentValueLabelText.text = currentValueLabel;
        }

        // Color-coded status
        if (currentValuePanel != null && sourceChart != null)
        {
            var panelImg = currentValuePanel.GetComponent<Image>();
            if (panelImg != null)
            {
                float t = Mathf.InverseLerp(sourceChart.minValue, sourceChart.maxValue, latestValue);
                
                // Color scheme: Red (low) → Yellow (mid) → Green (high)
                Color statusColor;
                if (t < 0.33f)
                    statusColor = new Color(0.8f, 0.2f, 0.2f, 0.9f); // Red
                else if (t < 0.66f)
                    statusColor = new Color(0.8f, 0.7f, 0.2f, 0.9f); // Yellow
                else
                    statusColor = new Color(0.2f, 0.8f, 0.2f, 0.9f); // Green
                
                panelImg.color = statusColor;
                
                // Also color the value text for emphasis
                if (t < 0.33f)
                    currentValueText.color = new Color(1f, 0.8f, 0.8f, 1f);
                else
                    currentValueText.color = Color.white;
            }
        }
    }

    private void ApplyLineStyle()
    {
        if (lineRenderer == null) return;

        var colorProp = lineRenderer.GetType().GetProperty("color");
        if (colorProp != null)
            colorProp.SetValue(lineRenderer, lineColor);

        var thicknessProp = lineRenderer.GetType().GetProperty("LineThickness")
                         ?? lineRenderer.GetType().GetProperty("Thickness");
        if (thicknessProp != null)
        {
            thicknessProp.SetValue(lineRenderer, lineWidth);
        }
        else
        {
            var thicknessField = lineRenderer.GetType().GetField("lineThickness");
            if (thicknessField != null)
                thicknessField.SetValue(lineRenderer, lineWidth);
        }
    }

    private string GetDisplayTitle()
    {
        if (!string.IsNullOrEmpty(customTitle))
            return customTitle;
        
        if (sourceChart != null && !string.IsNullOrEmpty(sourceChart.chartTitle))
            return sourceChart.chartTitle;
            
        return "Chart";
    }

    private string GetMetricName()
    {
        if (sourceChart == null || string.IsNullOrEmpty(sourceChart.chartTitle))
            return "Value";
            
        // Extract metric name from title (e.g., "ACCURACY OVER TIME" → "Accuracy")
        string title = sourceChart.chartTitle;
        if (title.Contains("ACCURACY"))
            return "Accuracy";
        if (title.Contains("VELOCITY"))
            return "Velocity";
        if (title.Contains("FATIGUE"))
            return "Fatigue";
        if (title.Contains("CONFIDENCE"))
            return "Confidence";
            
        // Default: use first word
        int spaceIndex = title.IndexOf(' ');
        if (spaceIndex > 0)
            return title.Substring(0, spaceIndex);
            
        return title;
    }

    [ContextMenu("Rebuild UI")]
    public void RebuildUI()
    {
        foreach (var label in yAxisLabels)
            if (label != null) Destroy(label.gameObject);
        foreach (var label in xAxisLabels)
            if (label != null) Destroy(label.gameObject);
        foreach (var line in gridLines)
            if (line != null) Destroy(line.gameObject);
        if (titleText != null) Destroy(titleText.gameObject);
        if (currentValuePanel != null) Destroy(currentValuePanel);
        if (yAxisTitleText != null) Destroy(yAxisTitleText.gameObject);
        if (xAxisTitleText != null) Destroy(xAxisTitleText.gameObject);

        yAxisLabels.Clear();
        xAxisLabels.Clear();
        gridLines.Clear();
        
        _uiBuilt = false;
        BuildUI();
    }
}