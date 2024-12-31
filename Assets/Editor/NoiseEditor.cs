using CodeBlaze.Vloxy.Game;
using UnityEditor;
using UnityEngine;

namespace CodeBlaze.Editor {

    [CustomEditor(typeof(FastNoiseLiteProfile))]
    public class NoiseEditor : UnityEditor.Editor {

        private FastNoiseLite FNL;
        private Texture2D Image;

        private int PreviewScale = 1;

        private void OnEnable() {
            Image = new Texture2D(256, 256);
        }
        
        private void GeneratePreview() {
            var profile = (FastNoiseLiteProfile) target;

            FNL = FastNoiseLiteExtensions.FromProfile(profile);   

            for (var x = 0; x < Image.width; x++) {
                for (var y = 0; y < Image.height; y++) {
                    var noise_value = FNL.GetNoise(x * PreviewScale, y * PreviewScale);

                    var height = ((float) noise_value + 1) / 2;
                    
                    Image.SetPixel(x, y, new Color(height, height, height));
                }
            }

            Image.Apply();
        }

        public override void OnInspectorGUI() {
            EditorGUILayout.LabelField("Vloxy Noise Editor");
            
            EditorGUI.BeginChangeCheck();
            
            DrawDefaultInspector();
            
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Preview");
            
            PreviewScale = Mathf.RoundToInt(EditorGUILayout.Slider("Scale", PreviewScale, 1f, 100f));
            
            EditorGUILayout.Separator();
            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(Image);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            if (GUILayout.Button( "Update Preview")) GeneratePreview();
        }

    }

}