using Gameplay.Player.AI;
using UnityEditor;
using UnityEngine;

namespace DefaultNamespace
{
    [CustomEditor(typeof(AiSettings))]
    public class AiSettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            AiSettings aiSettings = (AiSettings)target;

            if (GUILayout.Button("Normalize Probabilities"))
            {
                Undo.RecordObject(aiSettings, "Normalize AI Probabilities");
                Normalize(aiSettings);
                EditorUtility.SetDirty(aiSettings);
            }
        }

        private void Normalize(AiSettings settings)
        {
            float total = settings.perfectChance + settings.backboardChance +
                          settings.noPerfectChance + settings.noBackboardChance;

            if (total <= 0f) return;

            settings.perfectChance /= total;
            settings.backboardChance /= total;
            settings.noPerfectChance /= total;
            settings.noBackboardChance /= total;
        }
    }
}