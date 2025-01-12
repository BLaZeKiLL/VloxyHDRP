using UnityEngine;
using UnityEditor;
using CodeBlaze.Vloxy.Game;

namespace CodeBlaze.Editor
{
    public class WorldEditor : EditorWindow
    {
        private WorldProfile WorldProfileInstance;
        private ShapeProfile ShapeProfileInstance;
        private ContinentalProfile ContinentalProfileInstance;
        private SquishProfile SquishProfileInstance;
        private TreeProfile TreeProfileInstance;

        private SerializedObject WorldProfileObject;
        private SerializedObject ShapeProfileObject;
        private SerializedObject ContinentalProfileObject;
        private SerializedObject SquishProfileObject;
        private SerializedObject TreeProfileObject;

        private const string WorldProfileKey = "WorldEditor_WorldProfileKey";
        private const string ShapeProfileKey = "WorldEditor_ShapeProfile";
        private const string ContinentalProfileKey = "WorldEditor_ContinentalProfile";
        private const string SquishProfileKey = "WorldEditor_SquishProfile";
        private const string TreeProfileKey = "WorldEditor_TreeProfile";

        private Vector2 scrollPosition; // Scroll position for the scroll view

        [MenuItem("Tools/WorldEditor")]
        private static void ShowWindow()
        {
            var window = GetWindow<WorldEditor>();
            window.titleContent = new GUIContent("WorldEditor");
            window.Show();
        }

        private void OnEnable()
        {
            // Load saved object references
            WorldProfileInstance = LoadObjectFromPrefs<WorldProfile>(WorldProfileKey);
            ShapeProfileInstance = LoadObjectFromPrefs<ShapeProfile>(ShapeProfileKey);
            ContinentalProfileInstance = LoadObjectFromPrefs<ContinentalProfile>(ContinentalProfileKey);
            SquishProfileInstance = LoadObjectFromPrefs<SquishProfile>(SquishProfileKey);
            TreeProfileInstance = LoadObjectFromPrefs<TreeProfile>(TreeProfileKey);

            // Update serialized objects
            UpdateSerializedObjects();
        }

        private void OnDisable()
        {
            SaveObjectToPrefs(WorldProfileInstance, WorldProfileKey);
            SaveObjectToPrefs(ShapeProfileInstance, ShapeProfileKey);
            SaveObjectToPrefs(ContinentalProfileInstance, ContinentalProfileKey);
            SaveObjectToPrefs(SquishProfileInstance, SquishProfileKey);
            SaveObjectToPrefs(TreeProfileInstance, TreeProfileKey);
        }

        private void OnGUI()
        {
            GUILayout.Label("World Profile Objects", EditorStyles.boldLabel);

            // Object selectors for each ScriptableObject type
            DrawObjectSelectors();

            // Update serialized objects if instances are assigned
            UpdateSerializedObjects();

            GUILayout.Space(16);

            if (GUILayout.Button("Update World Profiles")) UpdateWorldProfiles();

            GUILayout.Space(4);

            // Scrollable area for the properties
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            // Edit fields for each ScriptableObject
            DrawObjectEditors();

            EditorGUILayout.EndScrollView();
        }

        private void DrawObjectSelectors()
        {
            WorldProfileInstance = (WorldProfile)EditorGUILayout.ObjectField(
                "World Profile", WorldProfileInstance, typeof(WorldProfile), false
            );
            ShapeProfileInstance = (ShapeProfile)EditorGUILayout.ObjectField(
                "Shape Profile", ShapeProfileInstance, typeof(ShapeProfile), false
            );
            ContinentalProfileInstance = (ContinentalProfile)EditorGUILayout.ObjectField(
                "Continental Profile", ContinentalProfileInstance, typeof(ContinentalProfile), false
            );
            SquishProfileInstance = (SquishProfile)EditorGUILayout.ObjectField(
                "Squish Profile", SquishProfileInstance, typeof(SquishProfile), false
            );
            TreeProfileInstance = (TreeProfile)EditorGUILayout.ObjectField(
                "Tree Profile", TreeProfileInstance, typeof(TreeProfile), false
            );
        }

        private void DrawObjectEditors()
        {
            if (WorldProfileInstance != null)
            {
                GUILayout.Label("World Profile", EditorStyles.boldLabel);
                DrawSerializedProperties(WorldProfileObject);
            }

            GUILayout.Space(16);

            if (ShapeProfileInstance != null)
            {
                GUILayout.Label("Shape Profile", EditorStyles.boldLabel);
                DrawSerializedProperties(ShapeProfileObject);
            }

            GUILayout.Space(16);

            if (ContinentalProfileInstance != null)
            {
                GUILayout.Label("Continental Profile", EditorStyles.boldLabel);
                DrawSerializedProperties(ContinentalProfileObject);
            }

            GUILayout.Space(16);

            if (SquishProfileInstance != null)
            {
                GUILayout.Label("Squish Profile", EditorStyles.boldLabel);
                DrawSerializedProperties(SquishProfileObject);
            }
        }

        private void UpdateSerializedObjects()
        {
            if (WorldProfileInstance != null && (WorldProfileObject == null || WorldProfileObject.targetObject != WorldProfileInstance))
                WorldProfileObject = new SerializedObject(WorldProfileInstance);

            if (ShapeProfileInstance != null && (ShapeProfileObject == null || ShapeProfileObject.targetObject != ShapeProfileInstance))
                ShapeProfileObject = new SerializedObject(ShapeProfileInstance);

            if (ContinentalProfileInstance != null && (ContinentalProfileObject == null || ContinentalProfileObject.targetObject != ContinentalProfileInstance))
                ContinentalProfileObject = new SerializedObject(ContinentalProfileInstance);

            if (SquishProfileInstance != null && (SquishProfileObject == null || SquishProfileObject.targetObject != SquishProfileInstance))
                SquishProfileObject = new SerializedObject(SquishProfileInstance);

            if (TreeProfileInstance != null && (TreeProfileObject == null ||
            TreeProfileObject.targetObject != TreeProfileInstance))
                TreeProfileObject = new SerializedObject(TreeProfileInstance);
        }

        private void UpdateWorldProfiles()
        {
            var world_data = FindAnyObjectByType<WorldData>();

            if (world_data == null)
            {
                Debug.LogWarning("WorldData instance not found in active scene");
                return;
            }

            if (WorldProfileInstance != null)
            {
                world_data.WorldProfile = WorldProfileInstance;
            }

            if (ShapeProfileInstance != null)
            {
                world_data.ShapeProfile = ShapeProfileInstance;
            }

            if (ContinentalProfileInstance != null)
            {
                world_data.ContinentalProfile = ContinentalProfileInstance;
            }

            if (SquishProfileInstance != null)
            {
                world_data.SquishProfile = SquishProfileInstance;
            }

            if (TreeProfileInstance != null)
            {
                world_data.TreeProfile = TreeProfileInstance;
            }
        }

        private void DrawSerializedProperties(SerializedObject serializedObject)
        {
            if (serializedObject == null) return;

            serializedObject.Update();
            SerializedProperty property = serializedObject.GetIterator();

            property.NextVisible(true); // Skip "m_Script" property
            while (property.NextVisible(false))
            {
                EditorGUILayout.PropertyField(property, true);
            }

            if (serializedObject.ApplyModifiedProperties())
            {
                EditorUtility.SetDirty(serializedObject.targetObject);
            }
        }

        private void SaveObjectToPrefs(Object obj, string key)
        {
            if (obj != null)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                EditorPrefs.SetString(key, path);
            }
            else
            {
                EditorPrefs.DeleteKey(key);
            }
        }

        private T LoadObjectFromPrefs<T>(string key) where T : Object
        {
            string path = EditorPrefs.GetString(key, string.Empty);
            if (!string.IsNullOrEmpty(path))
            {
                return AssetDatabase.LoadAssetAtPath<T>(path);
            }

            return null;
        }
    }
}