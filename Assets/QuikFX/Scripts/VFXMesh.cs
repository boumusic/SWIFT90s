using System.Collections.Generic;
using UnityEngine;
using Pataya.Utility;

namespace Pataya.QuikFX
{
    public enum VFXMeshPivot
    {
        TopCircleCenter,
        BottomCircleCenter,
        Center,
        SeamTop,
        SeamBottom,
        SeamCenter
    }

    public class VFXMesh : MonoBehaviour
    {
        #region Fields

        [Header("Mesh Regenerate")]

        [Tooltip("Should the mesh be regenerated in playmode (Update() method) ?")]
        public bool updateInPlayMode = false;

        [Header("Pivot")]
        [Tooltip("Position of the pivot point.")]
        public VFXMeshPivot pivot;

        [Header("Ring")]
        [Tooltip("The number of 'sides' of the circle.")]
        [Range(3, 50f)] public int ringAmount = 20;

        [Tooltip("Remaps the position of the ring vertices, allows to change the distribution of the topology.")]
        public AnimationCurve ringPositionRemap = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0, 0, 1), new Keyframe(1, 1, 1, 0) });

        [Header("Loop")]
        [Tooltip("The number of intermediate vertices between the inner edge and the outer edge.")]
        [Range(2f, 30f)] public int loopAmount = 3;

        [Tooltip("Remaps the position of the loop vertices, allows to change the distribution of the topology.")]
        public AnimationCurve loopPositionRemap = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0, 0, 1), new Keyframe(1, 1, 1, 0) });

        [Header("Width")]
        [Tooltip("Distance from center of the inner edge.")]
        [Range(0, 20)] public float minRadius = 0.5f;

        [Tooltip("Distance from center of the outer edge.")]
        [Range(0, 20)] public float maxRadius = 1f;

        [Tooltip("Remaps the width of the circle.")]
        public AnimationCurve widthRemap = new AnimationCurve(new Keyframe[] { new Keyframe(0, 1, 0, 0), new Keyframe(1, 1, 0, 0) });

        [Tooltip("Influence of the width remap curve.")]
        [Range(0f, 1f)] public float widthRemapInfluence = 1f;

        [Header("Depth")]
        [Tooltip("Position of the last loop in the Z-Axis.")]
        [Range(-20f, 20)] public float outerLoopDepth = 0f;

        [Tooltip("Remaps the position of the loop vertices on their vertical axis, which changes the shape of the cone.")]
        public AnimationCurve loopHeightPositionRemap = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0, 0, 1), new Keyframe(1, 1, 1, 0) });

        [Tooltip("Position of the last edge in the Z-Axis.")]
        [Range(-5f, 5f)] public float maxLoopDepth = 0f;

        [Tooltip("Remaps the position of the loop vertices on their depth axis, which changes the shape of the cone / circle.")]
        public AnimationCurve loopDepthPositionRemap = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0, 0, 2), new Keyframe(0.5f, 1, 0, 0), new Keyframe(1, 0, -2, 0) });

        [Tooltip("The higher the U coordinate, the further it is in the Z-Axis.")]
        [Range(-10f, 10f)] public float depthByU = 0f;

        [Tooltip("Number of spirals, only relevant if using Depth by U variable.")]
        [Range(1, 5)] public int spiralAmount = 1;

        [Header("Ring Offset")]
        [Tooltip("Position of the last ring in degree.")]
        [Range(0, 360f)] public float arc = 360f;

        [Tooltip("Offset each loop vertex on the right or the left.")]
        [Range(-360, 360)] public float twist = 0f;

        [Header("UVs")]
        [Tooltip("Invert the U and the V coordinates.")]
        public bool invertUAndV = false;

        [Tooltip("Change the current shader with a shader displaying the UV coordinates.")]
        public bool displayUVs = false;

        [Header("Normals")]
        [Tooltip("Flip the normals direction.")]
        public bool flipNormals = false;

        [Tooltip("Draw the normals. Color is based on their orientation.")]
        public bool drawNormalDebug = false;

        [Tooltip("Size of the normal debug.")]
        [Range(0f, 2f)] public float normalSizeDebug = 0.5f;

        [Header("Vertices")]
        [Tooltip("Draw the vertices.")]
        public bool drawVertexDebug = false;

        [Tooltip("Size of the debug vertices.")]
        [Range(0f, 0.03f)] public float vertexSizeDebug = 0.03f;

        [Tooltip("Color of the debug vertices.")]
        public Color vertexColorDebug = Color.black;

        [Header("Mesh Data")]
        public Mesh mesh;
        public List<Vector3> vertices = new List<Vector3>();
        public List<Vector3> normals = new List<Vector3>();
        public List<int> triangles = new List<int>();
        public List<Vector2> uv = new List<Vector2>();

        public bool initialized = false;

        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;
        public Shader lastShader;

        #endregion

        #region MonoBehaviour Callbacks

        private void OnValidate()
        {
            UpdateMesh();
            GenerateMesh();
        }

        private void OnDrawGizmos()
        {
            DrawNormals();
            DrawVertices();
            AddMeshComponents();
        }

        private void Update()
        {
            if (updateInPlayMode)
            {
                UpdateMesh();
            }
        }

        #endregion

        #region Void Methods

        private void UpdateMesh()
        {
            ShaderUpdate();

            if (mesh == null)
            {
                GenerateMesh();
            }
        }

        public void GenerateMesh()
        {
            ClearAll();
            AddMainVertices();
            GenerateTriangles();
            ApplyMesh();
        }

        private void AddMainVertices()
        {
            float angle = 360f / (RingAmountMultipliedByCircle() - 1);
            float mul = arc / 360f;

            for (int l = 0; l < loopAmount; l++)
            {
                for (int r = 0; r < RingAmountMultipliedByCircle(); r++)
                {
                    float curveRemapEvaluate = CustomMethod.Interpolate(0f, 1f, 0f, RingAmountMultipliedByCircle() - 1, r);
                    float curveValue = ringPositionRemap.Evaluate(curveRemapEvaluate);
                    float u = CustomMethod.Interpolate(0, 1f, 0, RingAmountMultipliedByCircle() - 1, r);
                    Quaternion q = Quaternion.AngleAxis(spiralAmount * angle * (curveValue * (RingAmountMultipliedByCircle() - 1)) * mul, Vector3.back);
                    GenerateVertex(l, q, u, r);
                }
            }
        }
        
        private void GenerateVertex(int l, Quaternion q, float u, int r)
        {
            float normalizedLoopIndex = CustomMethod.Interpolate(0f, 1f, 0, loopAmount - 1, l);
            float normalizedRingIndex = CustomMethod.Interpolate(0f, 1f, 0, RingAmountMultipliedByCircle() - 1, r);

            float currDepth = Mathf.LerpUnclamped(0, ClampedOuterLoopDepth(), loopPositionRemap.Evaluate(normalizedLoopIndex));
            float addedDepth = Mathf.LerpUnclamped(0, maxLoopDepth, loopDepthPositionRemap.Evaluate(normalizedLoopIndex));

            float indexWidth = Mathf.InverseLerp(0, ClampedOuterLoopDepth(), currDepth);

            float widthMultiplier = Mathf.Lerp(1, widthRemap.Evaluate(normalizedRingIndex), widthRemapInfluence);
            float currentWidth = Mathf.LerpUnclamped(minRadius, Mathf.LerpUnclamped(minRadius, maxRadius, widthMultiplier), loopHeightPositionRemap.Evaluate(indexWidth));

            float currentTwist = Mathf.LerpUnclamped(0, twist, normalizedLoopIndex);

            Quaternion twistRot = Quaternion.AngleAxis(currentTwist, Vector3.back);

            Vector3 newVert = (q * twistRot) * (Vector3.up * currentWidth) + new Vector3(0, 0, currDepth + depthByU * u + addedDepth);

            float maxOffset = 0.5f;
            float randomOffset = Random.Range(-maxOffset, maxOffset);
            Vector3 debugOffset = new Vector3(0, 0, randomOffset);

            AddVertex(newVert);

            if (!invertUAndV)
            {
                uv.Add(new Vector2(u, normalizedLoopIndex));
            }

            else
            {
                uv.Add(new Vector2(normalizedLoopIndex, u));
            }
        }

        private void GenerateTriangles()
        {
            for (int l = 0; l < loopAmount - 1; l++)
            {
                for (int r = 0; r < RingAmountMultipliedByCircle() - 1; r++)
                {
                    int loopAdd = l * (RingAmountMultipliedByCircle());
                    int a = r + loopAdd;
                    int b = r + RingAmountMultipliedByCircle() + loopAdd;
                    int c = r + RingAmountMultipliedByCircle() + 1 + loopAdd;
                    int d = r + 1 + loopAdd;
                    AddTriangle(a, b, c);
                    AddTriangle(a, c, d);
                }
            }
        }
        
        private void AddTriangle(int a, int b, int c)
        {
            if (!flipNormals)
            {
                triangles.Add(a);
                triangles.Add(b);
                triangles.Add(c);
            }

            else
            {
                triangles.Add(c);
                triangles.Add(b);
                triangles.Add(a);
            }
        }

        private void AddVertex(Vector3 vert)
        {
            vertices.Add(vert - PivotOffset());
        }

        private void ClearAll()
        {
            vertices.Clear();
            triangles.Clear();
            uv.Clear();

            if(mesh)
            {
                mesh.triangles = triangles.ToArray();
                mesh.vertices = vertices.ToArray();
                mesh.uv = uv.ToArray();
            }
        }

        private void ApplyMesh()
        {
            if (mesh == null) mesh = new Mesh();
            mesh.name = "VFX Mesh";

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uv.ToArray();
            mesh.RecalculateNormals();
            FixNormals();
            
            if (meshFilter)
            {
                if (meshFilter.sharedMesh != mesh)
                {
                    meshFilter.sharedMesh = mesh;
                }
            }
        }

        private void AddMeshComponents()
        {
            if (meshFilter == null)
            {
                MeshFilter getComp = GetComponent<MeshFilter>();
                if (getComp == null)
                {
                    meshFilter = gameObject.AddComponent<MeshFilter>();
                }

                else
                {
                    meshFilter = getComp;
                }
            }

            if (meshRenderer == null)
            {
                MeshRenderer getComp = GetComponent<MeshRenderer>();
                if (getComp == null)
                {
                    meshRenderer = gameObject.AddComponent<MeshRenderer>();
                }

                else
                {
                    meshRenderer = getComp;
                }
            }

            else
            {
                if (meshRenderer.sharedMaterial == null)
                {
                    meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
                }
            }
        }

        private void ShaderUpdate()
        {
            if (meshRenderer)
            {
                if (meshRenderer.sharedMaterial)
                {
                    if (meshRenderer.sharedMaterial.shader != Shader.Find("QuikFX/DisplayUVs"))
                    {
                        lastShader = meshRenderer.sharedMaterial.shader;
                    }

                    if (displayUVs)
                    {
                        meshRenderer.sharedMaterial.shader = Shader.Find("QuikFX/DisplayUVs");
                    }

                    else
                    {
                        if (lastShader)
                        {
                            if (meshRenderer.sharedMaterial.shader != lastShader)
                            {
                                meshRenderer.sharedMaterial.shader = lastShader;
                            }
                        }
                    }
                }
            }
        }

        private void FixNormals()
        {
            if(vertices.Count > 0)
            {
                normals = new List<Vector3>();
                for (int i = 0; i < mesh.normals.Length; i++)
                {
                    normals.Add(mesh.normals[i]);
                }

                for (int l = 0; l < loopAmount; l++)
                {
                    int correctIndex = l * (ringAmount + 1);
                    Vector3 correctNormal = normals[correctIndex];
                    normals[correctIndex + ringAmount] = correctNormal;
                    mesh.normals = normals.ToArray();
                }
            }

        }

        private void DrawNormals()
        {
            if (drawNormalDebug)
            {
                for (int i = 0; i < normals.Count; i++)
                {
                    Vector3 normal = normals[i];
                    Color normalColor = new Color(normal.x, normal.y, normal.z, 1f);
                    Gizmos.color = normalColor;
                    Vector3 start = transform.rotation * vertices[i] + transform.position;
                    Vector3 dest = start + (transform.rotation * (normal * normalSizeDebug));
                    Gizmos.DrawLine(start, dest);
                }
            }
        }

        private void DrawVertices()
        {
            if (drawVertexDebug)
            {
                for (int i = 0; i < vertices.Count; i++)
                {
                    Gizmos.color = vertexColorDebug;
                    Gizmos.DrawSphere(transform.rotation * vertices[i] + transform.position, vertexSizeDebug);
                }
            }
        }

        #endregion

        #region Return Method

        public int RingAmountMultipliedByCircle()
        {
            return (ringAmount + 1) * spiralAmount;
        }

        /// <summary>
        /// Breaks stuff if depth = 0f, and breaks other stuff if fixed sooo... I'll just clamp it to 0.0001f :>
        /// </summary>
        /// <returns></returns>
        private float ClampedOuterLoopDepth()
        {
            if (outerLoopDepth == 0)
            {
                return 0.0001f;
            }

            else return outerLoopDepth;
        }

        private Vector3 PivotOffset()
        {
            switch (pivot)
            {
                case VFXMeshPivot.TopCircleCenter:
                    return Vector3.zero;

                case VFXMeshPivot.BottomCircleCenter:
                    return new Vector3(0, 0, outerLoopDepth);

                case VFXMeshPivot.Center:
                    return new Vector3(0, 0, outerLoopDepth / 2);

                case VFXMeshPivot.SeamCenter:
                    float y = Mathf.LerpUnclamped(minRadius, maxRadius, 0.5f);
                    float z = Mathf.LerpUnclamped(0f, outerLoopDepth, 0.5f);
                    return Quaternion.AngleAxis(twist / 2, Vector3.back) * new Vector3(0, y, z);

                case VFXMeshPivot.SeamTop:
                    return new Vector3(0, minRadius, 0);

                case VFXMeshPivot.SeamBottom:
                    return Quaternion.AngleAxis(twist, Vector3.back) * new Vector3(0, maxRadius, outerLoopDepth);
            }

            return Vector3.zero;
        }

        #endregion
    }
}

