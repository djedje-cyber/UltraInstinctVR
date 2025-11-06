#if UNITY_2021_3_OR_NEWER

using System.Collections.Generic;

using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

namespace InriaTools
{
    [Overlay(typeof(SceneView), "EditorSceneViewStatsOverlay", "Editor Stats")]
    public class EditorSceneViewStatsOverlay : Overlay
    {
        #region Fields

        private static readonly string INCLUDE_INACTIVE_EDITOR_PREFS_KEY = "ToolsIncludeInactive";
        private IntegerField meshCountField = new("Meshes");
        private IntegerField triangleCountField = new("Triangles");
        private IntegerField verticesCountField = new("Vertices");
        private Toggle includeInactiveField = new("Include Inactive");
        private bool includeInactive = false;

        #endregion

        #region Methods

        public override void OnCreated()
        {
            base.OnCreated();
            InitFieldStyle(meshCountField);
            InitFieldStyle(triangleCountField);
            InitFieldStyle(verticesCountField);
            includeInactiveField.RegisterValueChangedCallback(ToggleShowInactive);
            Selection.selectionChanged += OnSelectionChanged;
            EditorApplication.playModeStateChanged += OnPlayModeChange;
            OnPlayModeChange();
        }

        public override void OnWillBeDestroyed()
        {
            Selection.selectionChanged -= OnSelectionChanged;
            EditorApplication.playModeStateChanged -= OnPlayModeChange;
            base.OnWillBeDestroyed();
        }

        public override VisualElement CreatePanelContent()
        {
            VisualElement root = new() { name = "Editor Stats" };
            root.Add(meshCountField);
            root.Add(triangleCountField);
            root.Add(verticesCountField);
            root.Add(includeInactiveField);
            return root;
        }

        private void InitFieldStyle(IntegerField field)
        {
            field.isReadOnly = true;

            field.labelElement.style.minWidth = 60;
            field.labelElement.style.maxWidth = 60;

            field[1].style.unityTextAlign = TextAnchor.MiddleRight;
            field[1].style.minWidth = 50;
        }

        private void ToggleShowInactive(ChangeEvent<bool> e)
        {
            includeInactive = e.newValue;
            EditorPrefs.SetBool(INCLUDE_INACTIVE_EDITOR_PREFS_KEY, includeInactive);
            OnSelectionChanged();
        }

        private void OnPlayModeChange(PlayModeStateChange _ = default)
        {
            includeInactive = EditorPrefs.GetBool(INCLUDE_INACTIVE_EDITOR_PREFS_KEY, false);
            includeInactiveField.value = includeInactive;
            OnSelectionChanged();
        }

        private void OnSelectionChanged()
        {
            Transform[] selected = Selection.GetFiltered<Transform>(SelectionMode.TopLevel);
            HashSet<Mesh> statics = new();
            int meshCount = 0;
            int triangleCount = 0;
            int verticesCount = 0;

            foreach (Transform s in selected)
            {
                MeshFilter[] mfs = s.GetComponentsInChildren<MeshFilter>(includeInactive);
                foreach (MeshFilter mf in mfs)
                {
                    MeshRenderer mr = mf.GetComponent<MeshRenderer>();
                    if (mf.sharedMesh == null || (!includeInactive && !(mf.gameObject.activeInHierarchy && mr.enabled)))
                        continue;

                    if (mr.isPartOfStaticBatch)
                    {
                        if (statics.Contains(mf.sharedMesh))
                            continue;
                        statics.Add(mf.sharedMesh);
                    }

                    meshCount += mf.sharedMesh.subMeshCount;
                    triangleCount += mf.sharedMesh.triangles.Length / 3;
                    verticesCount += mf.sharedMesh.vertexCount;
                }

                SkinnedMeshRenderer[] smrs = s.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive);
                foreach (SkinnedMeshRenderer smr in smrs)
                {
                    if (smr.sharedMesh == null || (!includeInactive && !(smr.gameObject.activeInHierarchy && smr.enabled)))
                        continue;

                    if (smr.isPartOfStaticBatch)
                    {
                        if (statics.Contains(smr.sharedMesh))
                            continue;
                        statics.Add(smr.sharedMesh);
                    }

                    meshCount += smr.sharedMesh.subMeshCount;
                    triangleCount += smr.sharedMesh.triangles.Length / 3;
                    verticesCount += smr.sharedMesh.vertexCount;
                }
            }

            meshCountField.value = meshCount;
            triangleCountField.value = triangleCount;
            verticesCountField.value = verticesCount;
        }

        #endregion
    }
}

#endif
