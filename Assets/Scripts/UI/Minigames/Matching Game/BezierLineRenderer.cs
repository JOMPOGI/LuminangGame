using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Luminang.UI.Minigames
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class BezierLineRenderer : MaskableGraphic
    {
        [Header("Points")]
        public RectTransform startPoint;
        public RectTransform endPoint;

        [Header("Curve Settings")]
        public float thickness = 5f;
        public int resolution = 20;
        public float curveIntensity = 100f;

        [Header("Animation")]
        [Range(0, 1)] public float progress = 1f;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            if (startPoint == null || endPoint == null) return;

            // Get local positions
            Vector2 p0 = transform.InverseTransformPoint(startPoint.position);
            Vector2 p3 = transform.InverseTransformPoint(endPoint.position);

            // Calculate control points for an S-curve
            // We push the control points horizontally to create the smooth transition
            Vector2 p1 = p0 + Vector2.right * curveIntensity;
            Vector2 p2 = p3 + Vector2.left * curveIntensity;

            List<Vector2> curvePoints = new List<Vector2>();
            for (int i = 0; i <= resolution; i++)
            {
                float t = (float)i / resolution * progress;
                curvePoints.Add(CalculateCubicBezierPoint(t, p0, p1, p2, p3));
            }

            // Draw segments
            for (int i = 0; i < curvePoints.Count - 1; i++)
            {
                AddLineSegment(vh, curvePoints[i], curvePoints[i + 1]);
            }
        }

        private Vector2 CalculateCubicBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector2 p = uuu * p0; // (1-t)^3 * P0
            p += 3 * uu * t * p1; // 3(1-t)^2 * t * P1
            p += 3 * u * tt * p2; // 3(1-t) * t^2 * P2
            p += ttt * p3;        // t^3 * P3

            return p;
        }

        private void AddLineSegment(VertexHelper vh, Vector2 start, Vector2 end)
        {
            Vector2 dir = (end - start).normalized;
            Vector2 normal = new Vector2(-dir.y, dir.x) * (thickness * 0.5f);

            int vertCount = vh.currentVertCount;

            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = color;

            vertex.position = start - normal;
            vh.AddVert(vertex);

            vertex.position = start + normal;
            vh.AddVert(vertex);

            vertex.position = end + normal;
            vh.AddVert(vertex);

            vertex.position = end - normal;
            vh.AddVert(vertex);

            vh.AddTriangle(vertCount, vertCount + 1, vertCount + 2);
            vh.AddTriangle(vertCount, vertCount + 2, vertCount + 3);
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                SetVerticesDirty();
            }
        }
        
        public void SetPoints(RectTransform start, RectTransform end)
        {
            startPoint = start;
            endPoint = end;
            SetVerticesDirty();
        }

        public void AnimateProgress(float targetProgress)
        {
            progress = targetProgress;
            SetVerticesDirty();
        }
    }
}
