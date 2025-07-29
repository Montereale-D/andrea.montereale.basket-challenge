using Gameplay.Bonus;
using UnityEditor;
using UnityEngine;

namespace DefaultNamespace
{
    [CustomEditor(typeof(BonusSettings))]
    public class BonusSettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            BonusSettings settings = (BonusSettings)target;

            if (GUILayout.Button("Normalize Probabilities"))
            {
                Undo.RecordObject(settings, "Normalize Probabilities");
                Normalize(settings);
                EditorUtility.SetDirty(settings);
            }
        }
        
        private void Normalize(BonusSettings settings)
        {
            float total = settings.commonBonusChance + settings.rareBonusChance + settings.veryRareBonusChance;
            if (total <= 0f) return;

            settings.commonBonusChance /= total;
            settings.rareBonusChance /= total;
            settings.veryRareBonusChance /= total;
        }
    }
}