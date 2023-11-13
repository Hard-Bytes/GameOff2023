using System.Collections.Generic;
using Project.Code.Domain;
using Project.Code.Utils;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(PitfallTrigger))]
    public class DeathTrapsEditor : UnityEditor.Editor
    {
        public List<string> validSources = new()
            { DamageSource.PitfallTrap.ToString(), DamageSource.LiquidTrap.ToString() };
        public int selectedIndex = 0;
        public override void OnInspectorGUI()
        {
            PitfallTrigger script = (PitfallTrigger)target;

            EditorGUILayout.PrefixLabel("Type");

            selectedIndex = EditorGUILayout.Popup(selectedIndex, validSources.ToArray());

            DamageSource.TryParse(validSources[selectedIndex],out script.pitfallType);
        }
    }
}