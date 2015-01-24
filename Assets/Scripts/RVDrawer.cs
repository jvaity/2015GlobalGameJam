using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RVDrawer : MonoBehaviour
{
    public class DrawingTask
    {
        public enum DrawObjectType
        {
            Line,
            Cross,
            Sphere,
            Polygon,
            Axis,
            AABB,
            OBB,
            String
        }
        public DrawObjectType drawObjectType;
        public Vector3 centre;
        public Vector3[] verts;
        public Color color;

        public int lineWidth;

        public int xAxisLineWidth, yAxisLineWidth, zAxisLineWidth;

        public float duration;
        public float startTime;

        public bool fillShape;
        public int[] triangles;

        public bool depthEnabled;

        public float size;

        public Vector3 normalPlane;

        public string text;
        public TextMesh textMeshObj;

		public bool is2D;

        //Draw Line
        public DrawingTask(Vector3 from, Vector3 to, Color color, int lineWidth, float duration, bool depthEnabled)
        {
            this.drawObjectType = DrawObjectType.Line;

            //to.z = 0.0f;
            //from.z = 0.0f;

            this.verts = new Vector3[2] { from, to };
            this.color = color;

            this.lineWidth = lineWidth;

            this.duration = duration;
            startTime = Time.time;

            this.depthEnabled = depthEnabled;
        }

        //Draw Cross
        public DrawingTask(Vector3 crossPosition, Color color, float size, Vector3 planeNormal, float duration, bool depthEnabled)
        {
            this.drawObjectType = DrawObjectType.Cross;

            this.verts = new Vector3[1] { crossPosition };
            this.color = color;

            this.size = size;
            this.lineWidth = 1;

            planeNormal.Normalize();
            this.normalPlane = planeNormal;

            this.duration = duration;
            startTime = Time.time;

            this.depthEnabled = depthEnabled;
        }

        //Draw Polygon
        public DrawingTask(Vector3[] verts, Color color, int lineWidth, float duration, bool fillShape, bool depthEnabled)
        {
            this.drawObjectType = DrawObjectType.Polygon;

            this.verts = verts;
            this.color = color;

            this.lineWidth = lineWidth;

            this.duration = duration;
            startTime = Time.time;

            this.fillShape = fillShape;

            if (fillShape)
                this.triangles = TriangulatePolygon(this.verts);
            else
                this.triangles = null;

            this.depthEnabled = depthEnabled;
        }

        //Draw Equalateral Polygon
        public DrawingTask (Vector3 centre, Vector3 planeNormal, float radius, int numSides, Color color, int lineWidth, float duration, bool fillShape, bool depthEnabled)
        {
            this.drawObjectType = DrawObjectType.Polygon;

            this.size = radius;
            this.color = color;

            this.lineWidth = lineWidth;

            planeNormal.Normalize();
            this.normalPlane = planeNormal;

            this.duration = duration;
            this.startTime = Time.time;

            this.fillShape = fillShape;

            this.depthEnabled = depthEnabled;

            float angleDiff = 360.0f / (float)numSides;
            this.verts = new Vector3[numSides];

            float currentAngle = 0.0f;
            for (int i = 0; i < numSides; i++)
            {
                float cos = Mathf.Cos(Mathf.Deg2Rad * currentAngle);
                float sin = Mathf.Sin(Mathf.Deg2Rad * currentAngle);

                float x = (size * cos) + centre.x;
                float y = (size * sin) + centre.y;

                this.verts[i] = new Vector3(x, y, centre.z);

                currentAngle += angleDiff;
            }

            Quaternion angles = Quaternion.FromToRotation(Vector3.forward, planeNormal);
            for (int i = 0; i < verts.Length; i++)
                verts[i] = RVExtensions.RotatePointAroundPivot(verts[i], centre, angles);

            if (fillShape)
                this.triangles = TriangulatePolygon(this.verts);
            else
                this.triangles = null;
        }

        //Draw Arc
		public DrawingTask(Vector3 centre, Vector3 planeNormal, float radius, float angleStart, float angle, int numSides, Color color, int lineWidth, float duration, bool fillShape, bool depthEnabled)
		{
			this.drawObjectType = DrawObjectType.Polygon;

			this.size = radius;
			this.color = color;

			this.lineWidth = lineWidth;

			planeNormal.Normalize();
			this.normalPlane = planeNormal;

			this.duration = duration;
			this.startTime = Time.time;

            this.fillShape = fillShape;

			this.depthEnabled = depthEnabled;

			float angleDiff = angle / (float)numSides;
			this.verts = new Vector3[numSides + 1];

			float currentAngle = 0.0f;
			for (int i = 0; i < numSides; i++)
			{
				float cos = Mathf.Cos(Mathf.Deg2Rad * currentAngle);
				float sin = Mathf.Sin(Mathf.Deg2Rad * currentAngle);

				float x = (size * cos) + centre.x;
				float y = (size * sin) + centre.y;

				this.verts[i] = new Vector3(x, y, centre.z);

				currentAngle += angleDiff;
			}
			this.verts[verts.Length - 1] = centre;

			Quaternion angles = Quaternion.FromToRotation(Vector3.forward, planeNormal);
            Quaternion angleAxis = Quaternion.AngleAxis(angleStart, planeNormal);

            for (int i = 0; i < verts.Length; i++)
            {
                verts[i] = RVExtensions.RotatePointAroundPivot(verts[i], centre, angles);
                verts[i] = RVExtensions.RotatePointAroundPivot(verts[i], centre, angleAxis);
            }

            if (fillShape)
                this.triangles = TriangulatePolygon(this.verts);
            else
                this.triangles = null;
		}

        //Draw Arc using Direction Vector
        public DrawingTask(Vector3 centre, Vector3 planeNormal, float radius, Vector3 centreDirVec, float minAngle, float maxAngle, int numSides, Color color, int lineWidth, float duration, bool fillShape, bool depthEnabled)
        {
            this.drawObjectType = DrawObjectType.Polygon;

            this.size = radius;
            this.color = color;

            this.lineWidth = lineWidth;

            planeNormal.Normalize();
            this.normalPlane = planeNormal;

            this.duration = duration;
            this.startTime = Time.time;

            this.fillShape = fillShape;

            this.depthEnabled = depthEnabled;

            float angleDiff = (Mathf.Abs(minAngle) + Mathf.Abs(maxAngle)) / (float)numSides;
            this.verts = new Vector3[numSides + 2];

            float currentAngle = minAngle;

            for (int i = 0; i <= numSides; i++)
            {
                verts[i] = centre + (centreDirVec.normalized * radius);
                verts[i] = RVExtensions.RotatePointAroundPivot(verts[i], centre, currentAngle * planeNormal);

                currentAngle += angleDiff;
            }
            this.verts[verts.Length - 1] = centre;

            if (fillShape)
                this.triangles = TriangulatePolygon(this.verts);
            else
                this.triangles = null;
        }

        //Draw Arc using From-To Vectors
        public DrawingTask(Vector3 centre, Vector3 planeNormal, float radius, Vector3 fromVec, Vector3 toVec, int numSides, Color color, int lineWidth, float duration, bool fillShape, bool depthEnabled)
        {
            this.drawObjectType = DrawObjectType.Polygon;

            this.size = radius;
            this.color = color;

            this.lineWidth = lineWidth;

            planeNormal.Normalize();
            this.normalPlane = planeNormal;

            this.duration = duration;
            this.startTime = Time.time;

            this.fillShape = fillShape;

            this.depthEnabled = depthEnabled;

            float angleDiff = RVExtensions.SignedAngle(fromVec, toVec, planeNormal) / (float)numSides;
            this.verts = new Vector3[numSides + 2];

            float currentAngle = 0.0f;

            for (int i = 0; i <= numSides; i++)
            {
                verts[i] = centre + (fromVec.normalized * radius);
                verts[i] = RVExtensions.RotatePointAroundPivot(verts[i], centre, currentAngle * planeNormal);

                currentAngle += angleDiff;
            }
            this.verts[verts.Length - 1] = centre;

            if (fillShape)
                this.triangles = TriangulatePolygon(this.verts);
            else
                this.triangles = null;
        }

        //Draw sphere
        public DrawingTask(Vector3 centre, float radius, int numSides, Color color, int lineWidth, float duration, bool fillShape, bool depthEnabled)
        {
            this.drawObjectType = DrawObjectType.Sphere;

            this.centre = centre;

            this.size = radius;
            this.color = color;

            this.lineWidth = lineWidth;

            this.duration = duration;
            this.startTime = Time.time;

            this.fillShape = fillShape;
            this.depthEnabled = depthEnabled;

            if (!this.fillShape)
            {
                float angleDiff = 360.0f / (float)numSides;
                this.verts = new Vector3[numSides];

                float currentAngle = 0.0f;
                for (int i = 0; i < numSides; i++)
                {
                    float cos = Mathf.Cos(Mathf.Deg2Rad * currentAngle);
                    float sin = Mathf.Sin(Mathf.Deg2Rad * currentAngle);

                    float x = (size * cos) + centre.x;
                    float y = (size * sin) + centre.y;

                    this.verts[i] = new Vector3(x, y, centre.z);

                    currentAngle += angleDiff;
                }
            }
        }

        //Draw Axis
        public DrawingTask(Transform t, float size, float duration = 0.0f, bool depthEnabled = true)
        {
            this.drawObjectType = DrawObjectType.Axis;

            this.size = size;

            this.duration = duration;
            this.startTime = Time.time;

            this.depthEnabled = depthEnabled;

            this.lineWidth = 1;
            this.xAxisLineWidth = lineWidth;
            this.yAxisLineWidth = lineWidth;
            this.zAxisLineWidth = lineWidth;

            this.verts = new Vector3[6];

            this.verts[0] = t.TransformPoint(-Vector3.right * size);
            this.verts[1] = t.TransformPoint(Vector3.right * size);
            this.verts[2] = t.TransformPoint(Vector3.up * size);
            this.verts[3] = t.TransformPoint(-Vector3.up * size);
            this.verts[4] = t.TransformPoint(Vector3.forward * size);
            this.verts[5] = t.TransformPoint(-Vector3.forward * size);
        }

        //Draw Axis
        public DrawingTask(Transform t, float size, int xLineWidth, int yLineWidth, int zLineWidth, float duration = 0.0f, bool depthEnabled = true)
        {
            this.drawObjectType = DrawObjectType.Axis;

            this.size = size;

            this.duration = duration;
            this.startTime = Time.time;

            this.depthEnabled = depthEnabled;

            this.lineWidth = 1;
            this.xAxisLineWidth = xLineWidth;
            this.yAxisLineWidth = yLineWidth;
            this.zAxisLineWidth = zLineWidth;

            this.verts = new Vector3[6];

            this.verts[0] = t.TransformPoint(-Vector3.right * size);
            this.verts[1] = t.TransformPoint(Vector3.right * size);
            this.verts[2] = t.TransformPoint(Vector3.up * size);
            this.verts[3] = t.TransformPoint(-Vector3.up * size);
            this.verts[4] = t.TransformPoint(Vector3.forward * size);
            this.verts[5] = t.TransformPoint(-Vector3.forward * size);
        }
        //Draw Axis
        public DrawingTask(Vector3 center, Vector3 upDir, Vector3 forwardDir, float size, float duration = 0.0f, bool depthEnabled = true)
        {
            this.drawObjectType = DrawObjectType.Axis;

            this.size = size;

            this.duration = duration;
            this.startTime = Time.time;

            this.depthEnabled = depthEnabled;

            this.lineWidth = 1;
            this.xAxisLineWidth = lineWidth;
            this.yAxisLineWidth = lineWidth;
            this.zAxisLineWidth = lineWidth;

            this.verts = new Vector3[6];

            upDir.Normalize();
            forwardDir.Normalize();

            Vector3 rightDir = Vector3.Cross((center + upDir) - center, (center + forwardDir) - center).normalized;

            this.verts[0] = center - (rightDir * size);
            this.verts[1] = center + (rightDir * size);
            this.verts[2] = center - (upDir * size);
            this.verts[3] = center + (upDir * size);
            this.verts[4] = center - (forwardDir * size);
            this.verts[5] = center + (forwardDir * size);
        }

        public DrawingTask(Vector3 center, Vector3 upDir, Vector3 forwardDir, float size, int xLineWidth, int yLineWidth, int zLineWidth, float duration = 0.0f, bool depthEnabled = true)
        {
            this.drawObjectType = DrawObjectType.Axis;

            this.size = size;

            this.duration = duration;
            this.startTime = Time.time;

            this.depthEnabled = depthEnabled;

            this.lineWidth = 1;
            this.xAxisLineWidth = xLineWidth;
            this.yAxisLineWidth = yLineWidth;
            this.zAxisLineWidth = zLineWidth;

            this.verts = new Vector3[6];

            upDir.Normalize();
            forwardDir.Normalize();

            Vector3 rightDir = Vector3.Cross((center + upDir) - center, (center + forwardDir) - center).normalized;

            this.verts[0] = center - (rightDir * size);
            this.verts[1] = center + (rightDir * size);
            this.verts[2] = center - (upDir * size);
            this.verts[3] = center + (upDir * size);
            this.verts[4] = center - (forwardDir * size);
            this.verts[5] = center + (forwardDir * size);
        }

        //Draw AABB
        public DrawingTask(Bounds bounds, Color color, int lineWidth, float duration, bool fillShape, bool depthEnabled)
        {
            this.drawObjectType = DrawObjectType.OBB;

            this.color = color;
            this.lineWidth = lineWidth;

            this.duration = duration;
            this.startTime = Time.time;

            this.fillShape = fillShape;

            this.depthEnabled = depthEnabled;

            this.verts = new Vector3[16];

            Vector3[] boxVerts = new Vector3[8];

            #region Initialize Box Verts
            boxVerts[0] = bounds.center;
            boxVerts[0].z -= bounds.extents.z;
            boxVerts[0].y += bounds.extents.y;
            boxVerts[0].x -= bounds.extents.x;

            boxVerts[1] = bounds.center;
            boxVerts[1].z -= bounds.extents.z;
            boxVerts[1].y += bounds.extents.y;
            boxVerts[1].x += bounds.extents.x;

            boxVerts[2] = bounds.center;
            boxVerts[2].z += bounds.extents.z;
            boxVerts[2].y += bounds.extents.y;
            boxVerts[2].x += bounds.extents.x;

            boxVerts[3] = bounds.center;
            boxVerts[3].z += bounds.extents.z;
            boxVerts[3].y += bounds.extents.y;
            boxVerts[3].x -= bounds.extents.x;

            boxVerts[4] = bounds.center;
            boxVerts[4].z -= bounds.extents.z;
            boxVerts[4].y -= bounds.extents.y;
            boxVerts[4].x -= bounds.extents.x;

            boxVerts[5] = bounds.center;
            boxVerts[5].z -= bounds.extents.z;
            boxVerts[5].y -= bounds.extents.y;
            boxVerts[5].x += bounds.extents.x;

            boxVerts[6] = bounds.center;
            boxVerts[6].z += bounds.extents.z;
            boxVerts[6].y -= bounds.extents.y;
            boxVerts[6].x += bounds.extents.x;

            boxVerts[7] = bounds.center;
            boxVerts[7].z += bounds.extents.z;
            boxVerts[7].y -= bounds.extents.y;
            boxVerts[7].x -= bounds.extents.x;
            #endregion

            #region Initialize Faces
            this.verts[0] = boxVerts[0];
            this.verts[1] = boxVerts[1];
            this.verts[2] = boxVerts[2];
            this.verts[3] = boxVerts[3];

            this.verts[4] = boxVerts[4];
            this.verts[5] = boxVerts[5];
            this.verts[6] = boxVerts[6];
            this.verts[7] = boxVerts[7];

            this.verts[8] = boxVerts[0];
            this.verts[9] = boxVerts[4];

            this.verts[10] = boxVerts[1];
            this.verts[11] = boxVerts[5];

            this.verts[12] = boxVerts[2];
            this.verts[13] = boxVerts[6];

            this.verts[14] = boxVerts[3];
            this.verts[15] = boxVerts[7];
            #endregion
        }

        //Draw OBB
        public DrawingTask(BoxCollider boxCollider, Color color, int lineWidth, float duration, bool fillShape, bool depthEnabled)
        {
            this.drawObjectType = DrawObjectType.AABB;

            this.color = color;
            this.lineWidth = lineWidth;

            this.duration = duration;
            this.startTime = Time.time;

            this.fillShape = fillShape;

            this.depthEnabled = depthEnabled;

            this.verts = new Vector3[16];

            Transform transform = boxCollider.transform;
            GameObject temp = new GameObject();
            
            Transform worldCentreTrans = temp.transform;
            worldCentreTrans.position = transform.TransformPoint(boxCollider.center);
            worldCentreTrans.rotation = transform.rotation;
            worldCentreTrans.localScale = transform.localScale;
            //RVDrawer.QDrawAxis(worldCentreTrans, 0.1f);

            //Vector3 worldCentre = transform.TransformPoint(boxCollider.center);
            Vector3 localExtents = boxCollider.size / 2.0f;

            Vector3[] boxVerts = new Vector3[8];

            float zExtents = localExtents.z * worldCentreTrans.lossyScale.z;
            float yExtents = localExtents.y * worldCentreTrans.lossyScale.y;
            float xExtents = localExtents.x * worldCentreTrans.lossyScale.x;

            #region Initialize Box Verts
            Vector3 tempVec = -xExtents * worldCentreTrans.right + yExtents * worldCentreTrans.up - zExtents * worldCentreTrans.forward;
            boxVerts[0] = worldCentreTrans.position + tempVec;

            tempVec = xExtents * worldCentreTrans.right + yExtents * worldCentreTrans.up - zExtents * worldCentreTrans.forward;
            boxVerts[1] = worldCentreTrans.position + tempVec;

            tempVec = xExtents * worldCentreTrans.right + yExtents * worldCentreTrans.up + zExtents * worldCentreTrans.forward;
            boxVerts[2] = worldCentreTrans.position + tempVec;

            tempVec = -xExtents * worldCentreTrans.right + yExtents * worldCentreTrans.up + zExtents * worldCentreTrans.forward;
            boxVerts[3] = worldCentreTrans.position + tempVec;

            tempVec = -xExtents * worldCentreTrans.right - yExtents * worldCentreTrans.up - zExtents * worldCentreTrans.forward;
            boxVerts[4] = worldCentreTrans.position + tempVec;

            tempVec = xExtents * worldCentreTrans.right - yExtents * worldCentreTrans.up - zExtents * worldCentreTrans.forward;
            boxVerts[5] = worldCentreTrans.position + tempVec;

            tempVec = xExtents * worldCentreTrans.right - yExtents * worldCentreTrans.up + zExtents * worldCentreTrans.forward;
            boxVerts[6] = worldCentreTrans.position + tempVec;

            tempVec = -xExtents * worldCentreTrans.right - yExtents * worldCentreTrans.up + zExtents * worldCentreTrans.forward;
            boxVerts[7] = worldCentreTrans.position + tempVec;

            DestroyImmediate(temp);
            #endregion

            if (!this.fillShape)
            {
                #region Initialize Faces
                //Top face
                this.verts[0] = boxVerts[0];
                this.verts[1] = boxVerts[1];
                this.verts[2] = boxVerts[2];
                this.verts[3] = boxVerts[3];

                //Bottom Face
                this.verts[4] = boxVerts[4];
                this.verts[5] = boxVerts[5];
                this.verts[6] = boxVerts[6];
                this.verts[7] = boxVerts[7];

                //Four verticles
                this.verts[8] = boxVerts[0];
                this.verts[9] = boxVerts[4];

                this.verts[10] = boxVerts[1];
                this.verts[11] = boxVerts[5];

                this.verts[12] = boxVerts[2];
                this.verts[13] = boxVerts[6];

                this.verts[14] = boxVerts[3];
                this.verts[15] = boxVerts[7];
                #endregion
            }
            else
            {
                #region Initialize Faces
                this.verts = new Vector3[24];

                verts[0] = boxVerts[0];
                verts[1] = boxVerts[1];
                verts[2] = boxVerts[2];
                verts[3] = boxVerts[3];

                verts[4] = boxVerts[4];
                verts[5] = boxVerts[5];
                verts[6] = boxVerts[6];
                verts[7] = boxVerts[7];

                verts[8] = boxVerts[0];
                verts[9] = boxVerts[1];
                verts[10] = boxVerts[4];
                verts[11] = boxVerts[5];

                verts[12] = boxVerts[0];
                verts[13] = boxVerts[3];
                verts[14] = boxVerts[7];
                verts[15] = boxVerts[4];

                verts[16] = boxVerts[3];
                verts[17] = boxVerts[2];
                verts[18] = boxVerts[6];
                verts[19] = boxVerts[7];

                verts[20] = boxVerts[2];
                verts[21] = boxVerts[1];
                verts[22] = boxVerts[5];
                verts[23] = boxVerts[6];


                List<int> triangleList = new List<int>();
                for (int i = 0; i < verts.Length; i+= 4)
                    triangleList.AddRange(TriangulatePolygon(new Vector3[4] { verts[i], verts[i + 1], verts[i + 2], verts[i + 3] }));

                this.triangles = triangleList.ToArray();
                #endregion
            }
        }

        //Draw String
        public DrawingTask(Vector3 pos, string text, bool is2D, Color color, int fontSize, float duration, bool depthEnabled)
        {
            this.drawObjectType = DrawObjectType.String;

            this.verts = new Vector3[1] { pos };

            this.color = color;
            this.lineWidth = fontSize;

            this.duration = duration;
            this.startTime = Time.time;

            this.depthEnabled = depthEnabled;

            this.textMeshObj = null;
            this.text = text;
			this.is2D = is2D;
        }

        private int[] TriangulatePolygon(Vector3[] vertArray)
        {
            Camera mainCamera = Camera.main;

            if (mainCamera)
            {
                Vector3[] screenSpaceVerts = new Vector3[vertArray.Length];

                for (int i = 0; i < vertArray.Length; i++)
                    screenSpaceVerts[i] = mainCamera.WorldToScreenPoint(vertArray[i]);

                Vector2[] screenSpaceVerts2D = new Vector2[vertArray.Length];

                for (int i = 0; i < vertArray.Length; i++)
                    screenSpaceVerts2D[i] = new Vector2(screenSpaceVerts[i].x, screenSpaceVerts[i].y);

                Triangulator triangulator = new Triangulator(screenSpaceVerts2D);
                return triangulator.Triangulate();
            }
            else
                return null;
        }
    }

    private static List<DrawingTask> drawingTasks;
    private static Material lineMaterial;

    private static Stack<TextMesh> textMeshObjectPool;

    void Awake()
    {
        lineMaterial = new Material(Shader.Find("Sprites/Default"));
        drawingTasks = new List<DrawingTask>();
    }

    void OnPostRender()
    {
        List<DrawingTask> timedOutTasks = new List<DrawingTask>();

        foreach (DrawingTask task in drawingTasks)
        {
            if (Time.time - task.startTime > task.duration)
            {
                timedOutTasks.Add(task);
                if (task.drawObjectType == DrawingTask.DrawObjectType.String)
                {
                    if (task.textMeshObj != null)
                    {
                        task.textMeshObj.gameObject.SetActive(false);
                        textMeshObjectPool.Push(task.textMeshObj);
                    }
                }
                continue;
            }

            switch (task.drawObjectType)
            {
                case DrawingTask.DrawObjectType.Line:       DrawLine(task);     break;
                case DrawingTask.DrawObjectType.Cross:      DrawCross(task);    break;
                case DrawingTask.DrawObjectType.Sphere:     DrawSphere(task);   break;
                case DrawingTask.DrawObjectType.Polygon:    DrawPolygon(task);  break;
                case DrawingTask.DrawObjectType.Axis:       DrawAxis(task);     break;
                case DrawingTask.DrawObjectType.AABB:       DrawAABB(task);     break;
                case DrawingTask.DrawObjectType.OBB:        DrawOBB(task);      break;
            }
        }

        foreach (DrawingTask t in timedOutTasks)
            drawingTasks.Remove(t);
    }

    public static void RemoveTask(DrawingTask task)
    {
        if (drawingTasks == null)
        {
            drawingTasks = new List<DrawingTask>();
            return;
        }

        if (drawingTasks.Contains(task))
            drawingTasks.Remove(task);
    }

    public static DrawingTask QDrawLine(Vector3 from, Vector3 to, Color color, int lineWidth = 1, float duration = 0.0f, bool depthEnabled = true)
    {
        if (drawingTasks == null)
            drawingTasks = new List<DrawingTask>();

        drawingTasks.Add(new DrawingTask(from, to, color, lineWidth, duration, depthEnabled));

        return drawingTasks[drawingTasks.Count - 1];
    }

    public static DrawingTask QDrawCross(Vector3 position, Color color, float size, Vector3 normalDir, int lineWidth = 1, float duration = 0.0f, bool depthEnabled = true)
    {
        if (drawingTasks == null)
            drawingTasks = new List<DrawingTask>();

        drawingTasks.Add(new DrawingTask(position, color, size, normalDir, duration, depthEnabled));

        return drawingTasks[drawingTasks.Count - 1];
    }

    public static DrawingTask QDrawCross(Vector3 position, Color color, float size, int lineWidth = 1, float duration = 0.0f, bool depthEnabled = true)
    {
        if (drawingTasks == null)
            drawingTasks = new List<DrawingTask>();

        drawingTasks.Add(new DrawingTask(position, color, size, Vector3.forward, duration, depthEnabled));

        return drawingTasks[drawingTasks.Count - 1];
    }

    public static DrawingTask QDrawArc(Vector3 center, Vector3 planeNormal, float radius, float angle1, float angle2, int numSides, Color color, int lineWidth = 1, float duration = 0.0f, bool fillShape = false, bool depthEnabled = true)
	{
		if (drawingTasks == null)
			drawingTasks = new List<DrawingTask>();

		drawingTasks.Add(new DrawingTask(center, planeNormal, radius, angle1, angle2, numSides, color, lineWidth, duration, fillShape, depthEnabled));

        return drawingTasks[drawingTasks.Count - 1];
	}

    public static DrawingTask QDrawArc(Vector3 centre, Vector3 planeNormal, float radius, Vector3 centreDirVec, float minAngle, float maxAngle, int numSides, Color color, int lineWidth = 1, float duration = 0.0f, bool fillShape = false, bool depthEnabled = true)
    {
        if (drawingTasks == null)
            drawingTasks = new List<DrawingTask>();

        drawingTasks.Add(new DrawingTask(centre, planeNormal, radius, centreDirVec, minAngle, maxAngle, numSides, color, lineWidth, duration, fillShape, depthEnabled));

        return drawingTasks[drawingTasks.Count - 1];
    }

    public static DrawingTask QDrawArc(Vector3 centre, Vector3 planeNormal, float radius, Vector3 fromVec, Vector3 toVec, int numSides, Color color, int lineWidth = 1, float duration = 0.0f, bool fillShape = false, bool depthEnabled = true)
    {
        if (drawingTasks == null)
            drawingTasks = new List<DrawingTask>();

        drawingTasks.Add(new DrawingTask(centre, planeNormal, radius, fromVec, toVec, numSides, color, lineWidth, duration, fillShape, depthEnabled));

        return drawingTasks[drawingTasks.Count - 1];
    }

    public static DrawingTask QDrawPolygonAtPoint(Vector3 center, Vector3 planeNormal, float radius, int numSides, Color color, int lineWidth = 1, float duration = 0.0f, bool fillShape = false, bool depthEnabled = true)
    {
        if (drawingTasks == null)
            drawingTasks = new List<DrawingTask>();

        drawingTasks.Add(new DrawingTask(center, planeNormal, radius, numSides, color, lineWidth, duration, fillShape, depthEnabled));

        return drawingTasks[drawingTasks.Count - 1];
    }

    public static DrawingTask QDrawPolygon(Vector3[] verts, Color color, int lineWidth = 1, float duration = 0.0f, bool fillShape = false, bool depthEnabled = true)
    {
        if (drawingTasks == null)
            drawingTasks = new List<DrawingTask>();

        drawingTasks.Add(new DrawingTask(verts, color, lineWidth, duration, fillShape, depthEnabled));

        return drawingTasks[drawingTasks.Count - 1];
    }

    public static DrawingTask QDrawSphere(Vector3 center, float radius, Color color, float duration = 0.0f, bool fillShape = false, bool depthEnabled = true)
    {
        if (drawingTasks == null)
            drawingTasks = new List<DrawingTask>();

        if (fillShape)
            drawingTasks.Add(new DrawingTask(center, -Camera.main.transform.forward, radius, 16, color, 1, duration, true, depthEnabled));
        else
            drawingTasks.Add(new DrawingTask(center, radius, 32, color, 1, duration, false, depthEnabled));

        return drawingTasks[drawingTasks.Count - 1];
    }

    public static DrawingTask QDrawAxis(Transform t, float size, float duration = 0.0f, bool depthEnabled = true)
    {
        if (drawingTasks == null)
            drawingTasks = new List<DrawingTask>();

        drawingTasks.Add(new DrawingTask(t, size, duration, depthEnabled));

        return drawingTasks[drawingTasks.Count - 1];
    }

    public static DrawingTask QDrawAxis(Transform t, float size, int xLineWidth, int yLineWidth, int zLineWidth, float duration = 0.0f, bool depthEnabled = true)
    {
        if (drawingTasks == null)
            drawingTasks = new List<DrawingTask>();

        drawingTasks.Add(new DrawingTask(t, size, xLineWidth, yLineWidth, zLineWidth, duration, depthEnabled));

        return drawingTasks[drawingTasks.Count - 1];
    }

    public static DrawingTask QDrawAxis(Vector3 center, Vector3 upDir, Vector3 forwardDir, float size, float duration = 0.0f, bool depthEnabled = true)
    {
        if (drawingTasks == null)
            drawingTasks = new List<DrawingTask>();

        drawingTasks.Add(new DrawingTask(center, upDir, forwardDir, size, duration, depthEnabled));

        return drawingTasks[drawingTasks.Count - 1];
    }

    public static DrawingTask QDrawAxis(Vector3 center, Vector3 upDir, Vector3 forwardDir, float size, int xLineWidth, int yLineWidth, int zLineWidth, float duration = 0.0f, bool depthEnabled = true)
    {
        if (drawingTasks == null)
            drawingTasks = new List<DrawingTask>();

        drawingTasks.Add(new DrawingTask(center, upDir, forwardDir, size, xLineWidth, yLineWidth, zLineWidth, duration, depthEnabled));

        return drawingTasks[drawingTasks.Count - 1];
    }

    public static DrawingTask QDrawAABB(Bounds bounds, Color color, int lineWidth = 1, float duration = 0.0f, bool fillShape = false, bool depthEnabled = true)
    {
        if (drawingTasks == null)
            drawingTasks = new List<DrawingTask>();

        drawingTasks.Add(new DrawingTask(bounds, color, lineWidth, duration, fillShape, depthEnabled));

        return drawingTasks[drawingTasks.Count - 1];
    }

    public static DrawingTask QDrawOBB(BoxCollider boxCollider, Color color, int lineWidth = 1, float duration = 0.0f, bool fillShape = false, bool depthEnabled = true)
    {
        if (drawingTasks == null)
            drawingTasks = new List<DrawingTask>();

        if (boxCollider != null)
            drawingTasks.Add(new DrawingTask(boxCollider, color, lineWidth, duration, fillShape, depthEnabled));
        else
            return null;

        return drawingTasks[drawingTasks.Count - 1];
    }

    public static DrawingTask QDrawString(Vector3 pos, string text, Color color, int fontSize = 250, float duration = 0.0f, bool depthEnabled = true)
    {
        if (drawingTasks == null)
            drawingTasks = new List<DrawingTask>();

        if (textMeshObjectPool == null)
            textMeshObjectPool = new Stack<TextMesh>();

        drawingTasks.Add(new DrawingTask(pos, text, false, color, fontSize, duration, depthEnabled));

        return drawingTasks[drawingTasks.Count - 1];
    }

	public static DrawingTask QDrawString2D(Vector2 pos, string text, Color color, int fontSize = 250, float duration = 0f, bool depthEnabled = true)
	{
		if (drawingTasks == null)
			drawingTasks = new List<DrawingTask>();

		if (textMeshObjectPool == null)
			textMeshObjectPool = new Stack<TextMesh>();

		drawingTasks.Add(new DrawingTask(pos, text, true, color, fontSize, duration, depthEnabled));

		return drawingTasks[drawingTasks.Count - 1];
	}

    private static void DrawLine(Vector3 from, Vector3 to, Color color, int lineWidth)
    {
        lineMaterial.SetPass(0);

        if (lineWidth > 1.0f)
        {
            Camera mainCamera = Camera.main;

            if (mainCamera)
            {                
                Vector3 fromInScreenSpace = mainCamera.WorldToScreenPoint(from);
                Vector3 toInScreenSpace = mainCamera.WorldToScreenPoint(to);

                Vector3 d = toInScreenSpace - fromInScreenSpace;
                d.Normalize();
                Vector3 perp = new Vector3(-d.y, d.x);
                perp.Normalize();

                Vector3 p1 = fromInScreenSpace + (perp * (lineWidth / 2));
                Vector3 p2 = toInScreenSpace + (perp * (lineWidth / 2));
                Vector3 p3 = toInScreenSpace + (perp * -(lineWidth / 2));
                Vector3 p4 = fromInScreenSpace + (perp * -(lineWidth / 2));

                p1 = mainCamera.ScreenToWorldPoint(p1);
                p2 = mainCamera.ScreenToWorldPoint(p2);
                p3 = mainCamera.ScreenToWorldPoint(p3);
                p4 = mainCamera.ScreenToWorldPoint(p4);

                GL.Begin(GL.QUADS);
                GL.Color(color);
                GL.Vertex(p1);
                GL.Vertex(p2);
                GL.Vertex(p3);
                GL.Vertex(p4);
                GL.End();
            }
            else
            {
                GL.Begin(GL.LINES);
                GL.Color(color);
                GL.Vertex(from);
                GL.Vertex(to);
                GL.End();
            }
        }
        else
        {
            GL.Begin(GL.LINES);
            GL.Color(color);
            GL.Vertex(from);
            GL.Vertex(to);
            GL.End();
        }
    }

    private static void DrawLine(DrawingTask task)
    {
        if (task.drawObjectType != DrawingTask.DrawObjectType.Line)
            return;

        DrawLine(task.verts[0], task.verts[1], task.color, task.lineWidth);
    }
    
    private static void DrawFilledTriangle(Vector3 a, Vector3 b, Vector3 c, Color color)
    {
        lineMaterial.SetPass(0);

        GL.Begin(GL.TRIANGLES);
        GL.Color(color);
        GL.Vertex(a);
        GL.Vertex(b);
        GL.Vertex(c);
        GL.End();
    }

    private static void DrawCross(Vector3 position, Vector3 planeNormal, float size, Color color, int lineWidth)
    {
        Vector3 x1 = new Vector3(position.x + (size / 2), position.y + (size / 2), position.z);
        Vector3 x2 = new Vector3(position.x - (size / 2), position.y - (size / 2), position.z);

        Vector3 y1 = new Vector3(position.x - (size / 2), position.y + (size / 2), position.z);
        Vector3 y2 = new Vector3(position.x + (size / 2), position.y - (size / 2), position.z);

        Quaternion angle = Quaternion.FromToRotation(Vector3.forward, planeNormal);
        x1 = RVExtensions.RotatePointAroundPivot(x1, position, angle);
        x2 = RVExtensions.RotatePointAroundPivot(x2, position, angle);
        y1 = RVExtensions.RotatePointAroundPivot(y1, position, angle);
        y2 = RVExtensions.RotatePointAroundPivot(y2, position, angle);

        DrawLine(x1, x2, color, lineWidth);
        DrawLine(y1, y2, color, lineWidth);
    }

    private static void DrawCross(DrawingTask task)
    {
        if (task.drawObjectType != DrawingTask.DrawObjectType.Cross)
            return;

        DrawCross(task.verts[0], task.normalPlane, task.size, task.color, task.lineWidth);
    }

    private static void DrawPolygon(Vector3[] verts, Color color, int lineWidth)
    {
        for (int i = 0; i < verts.Length; i++)
        {
            if (i != verts.Length - 1)
                DrawLine(verts[i], verts[i + 1], color, lineWidth);
            else
                DrawLine(verts[i], verts[0], color, lineWidth);
        }
    }

    private static void DrawFilledPolygon(Vector3[] vertArray, int[] triangles, Color color)
    {
        if (vertArray.Length < 3)
            return;

        if (triangles.Length % 3 == 0)
            for (int i = 0; i < triangles.Length; i+= 3)
                DrawFilledTriangle(vertArray[triangles[i]], vertArray[triangles[i + 1]], vertArray[triangles[i + 2]], color);
    }

    private static void DrawPolygon(DrawingTask task)
    {
        if (task.drawObjectType != DrawingTask.DrawObjectType.Polygon)
            return;

        if (!task.fillShape)
            DrawPolygon(task.verts, task.color, task.lineWidth);
        else
            DrawFilledPolygon(task.verts, task.triangles, task.color);
    }

    private static void DrawSphere(DrawingTask task)
    {
        if (task.drawObjectType != DrawingTask.DrawObjectType.Sphere)
            return;

        //print("Center " + task.centre.z);

        Vector3[] vertsXZ = new Vector3[task.verts.Length];
        Vector3[] vertsYZ = new Vector3[task.verts.Length];

        Quaternion anglesToXZ = Quaternion.FromToRotation(Vector3.forward, Vector3.up);
        Quaternion anglesToYZ = Quaternion.FromToRotation(Vector3.forward, Vector3.right);

        for (int i = 0; i < task.verts.Length; i++)
        {
            vertsXZ[i] = RVExtensions.RotatePointAroundPivot(task.verts[i], task.centre, anglesToXZ);
            vertsYZ[i] = RVExtensions.RotatePointAroundPivot(task.verts[i], task.centre, anglesToYZ);
        }

        DrawPolygon(task.verts, task.color, task.lineWidth);
        DrawPolygon(vertsXZ, task.color, task.lineWidth);
        DrawPolygon(vertsYZ, task.color, task.lineWidth);
    }

    private static void DrawAxis(DrawingTask task)
    {
        if (task.drawObjectType != DrawingTask.DrawObjectType.Axis)
            return;

        DrawLine(task.verts[0], task.verts[1], Color.red, task.xAxisLineWidth);
        DrawLine(task.verts[2], task.verts[3], Color.green, task.yAxisLineWidth);
        DrawLine(task.verts[4], task.verts[5], Color.blue, task.zAxisLineWidth);
    }

    private static void DrawAABB(DrawingTask task)
    {
        if (task.drawObjectType != DrawingTask.DrawObjectType.AABB)
            return;

        DrawOBB(task);
    }

    private static void DrawOBB(DrawingTask task)
    {
        if (task.drawObjectType != DrawingTask.DrawObjectType.OBB && task.drawObjectType != DrawingTask.DrawObjectType.AABB)
            return;

        if (!task.fillShape)
        {
            Vector3[] topFace = new Vector3[4] { task.verts[0], task.verts[1], task.verts[2], task.verts[3] };
            Vector3[] bottomFace = new Vector3[4] { task.verts[4], task.verts[5], task.verts[6], task.verts[7] };

            DrawPolygon(topFace, task.color, task.lineWidth);
            DrawPolygon(bottomFace, task.color, task.lineWidth);

            for (int i = 8; i < task.verts.Length; i += 2)
                DrawLine(task.verts[i], task.verts[i + 1], task.color, task.lineWidth);
        }
        else
        {
            int tCount = 0;
            for (int i = 0; i < task.verts.Length; i+= 4)
            {
                Vector3[] faceArray = new Vector3[4] {task.verts[i], task.verts[i + 1], task.verts[i + 2], task.verts[i + 3]};
                int[] triangleArray = new int[6] {task.triangles[tCount],
                                                  task.triangles[tCount++],
                                                  task.triangles[tCount++],
                                                  task.triangles[tCount++],
                                                  task.triangles[tCount++],
                                                  task.triangles[tCount++]};

                DrawFilledPolygon(faceArray, task.triangles, task.color);
            }
        }
    }

    
}
