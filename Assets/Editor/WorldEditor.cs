using UnityEngine;
using UnityEditor;
using CodeBlaze.Vloxy.Game;

namespace CodeBlaze.Editor
{
    public class WorldEditor : EditorWindow
    {
        private ShapeNoiseProfile ShapeNoiseProfileInstance;
        private ContinentalNoiseProfile ContinentalNoiseProfileInstance;
        private SquishNoiseProfile SquishNoiseProfileInstance;

        private SerializedObject ShapeNoiseProfileObject;
        private SerializedObject ContinentalNoiseProfileObject;
        private SerializedObject SquishNoiseProfileObject;

        private const string ShapeNoiseProfileKey = "WorldEditor_ShapeNoiseProfile";
        private const string ContinentalNoiseProfileKey = "WorldEditor_ContinentalNoiseProfile";
        private const string SquishNoiseProfileKey = "WorldEditor_SquishNoiseProfile";

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
            ShapeNoiseProfileInstance = LoadObjectFromPrefs<ShapeNoiseProfile>(ShapeNoiseProfileKey);
            ContinentalNoiseProfileInstance = LoadObjectFromPrefs<ContinentalNoiseProfile>(ContinentalNoiseProfileKey);
            SquishNoiseProfileInstance = LoadObjectFromPrefs<SquishNoiseProfile>(SquishNoiseProfileKey);

            // Update serialized objects
            UpdateSerializedObjects();
        }

        private void OnDisable()
        {
            SaveObjectToPrefs(ShapeNoiseProfileInstance, ShapeNoiseProfileKey);
            SaveObjectToPrefs(ContinentalNoiseProfileInstance, ContinentalNoiseProfileKey);
            SaveObjectToPrefs(SquishNoiseProfileInstance, SquishNoiseProfileKey);
        }

        private void OnGUI()
        {
            GUILayout.Label("World Profile Objects", EditorStyles.boldLabel);

            // Object selectors for each ScriptableObject type
            ShapeNoiseProfileInstance = (ShapeNoiseProfile)EditorGUILayout.ObjectField("Shape Noise Profile", ShapeNoiseProfileInstance, typeof(ShapeNoiseProfile), false);
            ContinentalNoiseProfileInstance = (ContinentalNoiseProfile)EditorGUILayout.ObjectField("Continental Noise Profile", ContinentalNoiseProfileInstance, typeof(ContinentalNoiseProfile), false);
            SquishNoiseProfileInstance = (SquishNoiseProfile)EditorGUILayout.ObjectField("Squish Noise Profile", SquishNoiseProfileInstance, typeof(SquishNoiseProfile), false);

            // Update serialized objects if instances are assigned
            UpdateSerializedObjects();

            GUILayout.Space(16);

            if (GUILayout.Button("Update World Profiles")) UpdateWorldProfiles();

            GUILayout.Space(4);

            // Scrollable area for the properties
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            // Edit fields for each ScriptableObject
            if (ShapeNoiseProfileInstance != null)
            {
                GUILayout.Label("Shape Profile", EditorStyles.boldLabel);
                DrawSerializedProperties(ShapeNoiseProfileObject);
            }

            GUILayout.Space(16);

            if (ContinentalNoiseProfileInstance != null)
            {
                GUILayout.Label("Continental Profile", EditorStyles.boldLabel);
                DrawSerializedProperties(ContinentalNoiseProfileObject);
            }

            GUILayout.Space(16);

            if (SquishNoiseProfileInstance != null)
            {
                GUILayout.Label("Squish Profile", EditorStyles.boldLabel);
                DrawSerializedProperties(SquishNoiseProfileObject);
            }

            EditorGUILayout.EndScrollView();
        }

        private void UpdateSerializedObjects()
        {
            if (ShapeNoiseProfileInstance != null && (ShapeNoiseProfileObject == null || ShapeNoiseProfileObject.targetObject != ShapeNoiseProfileInstance))
                ShapeNoiseProfileObject = new SerializedObject(ShapeNoiseProfileInstance);

            if (ContinentalNoiseProfileInstance != null && (ContinentalNoiseProfileObject == null || ContinentalNoiseProfileObject.targetObject != ContinentalNoiseProfileInstance))
                ContinentalNoiseProfileObject = new SerializedObject(ContinentalNoiseProfileInstance);

            if (SquishNoiseProfileInstance != null && (SquishNoiseProfileObject == null || SquishNoiseProfileObject.targetObject != SquishNoiseProfileInstance))
                SquishNoiseProfileObject = new SerializedObject(SquishNoiseProfileInstance);
        }

        private void UpdateWorldProfiles()
        {
            var world_data = FindAnyObjectByType<WorldData>();

            if (world_data == null)
            {
                Debug.LogWarning("WorldData instance not found in active scene");
                return;
            }

            if (ShapeNoiseProfileInstance != null)
            {
                world_data.ShapeNoiseProfile = ShapeNoiseProfileInstance;
            }

            if (ContinentalNoiseProfileInstance != null)
            {
                world_data.ContinentalNoiseProfile = ContinentalNoiseProfileInstance;
            }

            if (SquishNoiseProfileInstance != null)
            {
                world_data.SquishNoiseProfile = SquishNoiseProfileInstance;
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