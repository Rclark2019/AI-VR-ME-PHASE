using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(CanvasRenderer))]
public class UILineRenderer : Graphic
{
    public Vector2[] Points { get; set; }
    public float lineThickness = 2f;
    
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        
        if (Points == null || Points.Length < 2)
            return;
        
        for (int i = 0; i < Points.Length - 1; i++)
        {
            Vector2 point1 = Points[i];
            Vector2 point2 = Points[i + 1];
            
            DrawLine(vh, point1, point2, lineThickness, color);
        }
    }
    
    void DrawLine(VertexHelper vh, Vector2 start, Vector2 end, float thickness, Color col)
    {
        Vector2 dir = (end - start).normalized;
        Vector2 perpendicular = new Vector2(-dir.y, dir.x) * thickness * 0.5f;
        
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = col;
        
        int startIndex = vh.currentVertCount;
        
        vertex.position = start - perpendicular;
        vh.AddVert(vertex);
        
        vertex.position = start + perpendicular;
        vh.AddVert(vertex);
        
        vertex.position = end + perpendicular;
        vh.AddVert(vertex);
        
        vertex.position = end - perpendicular;
        vh.AddVert(vertex);
        
        vh.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
        vh.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
    }
}
