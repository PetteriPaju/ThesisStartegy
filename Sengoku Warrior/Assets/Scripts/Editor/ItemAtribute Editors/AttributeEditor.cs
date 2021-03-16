using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


namespace SengokuWarrior
{
    public abstract class AttributeEditor : Editor
    {
        public virtual bool CanDelete() { return true; }
    }

    [CustomEditor(typeof(CommonAttributes))]
    public class CommonAttributeEditor : AttributeEditor
    {

        public override bool CanDelete()
        {
            return false;
        }

        public override void OnInspectorGUI()
        {
            CommonAttributes atr = (CommonAttributes)target;
            EditorGUILayout.BeginVertical();
            EditorGUI.BeginChangeCheck();
            string _name = EditorGUILayout.TextField("Game name", atr._name);
            float val = EditorGUILayout.FloatField("Value", atr.value);
            bool stack = EditorGUILayout.Toggle("Is Stackable", atr.isStackable);
            Sprite sp = EditorGUILayout.ObjectField("Sprite", atr.sprite, typeof(Sprite), false) as Sprite;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Description", GUILayout.Width(115));
            string decr = EditorGUILayout.TextArea(atr.Description, EditorStyles.textArea, GUILayout.Height(EditorGUIUtility.singleLineHeight * 2));
            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {

                Undo.RecordObject(atr, "Modify attribute");
                atr.value = Mathf.Max(0, val);
                atr.isStackable = stack;
                atr._name = _name;
                atr.sprite = sp;
                atr.Description = decr;
                EditorUtility.SetDirty(target);
            }

            EditorGUILayout.EndVertical();
        }
    }
    [CustomEditor(typeof(WeaponAttribute))]
    public class WeaponAttributeEditor : AttributeEditor
    {
        public override bool CanDelete()
        {
            return true;
        }

        public override void OnInspectorGUI()
        {
            WeaponAttribute atr = (WeaponAttribute)target;
            EditorGUILayout.BeginVertical();
            EditorGUI.BeginChangeCheck();
            int val = EditorGUILayout.IntSlider("Range", atr.range,1, 50);
            Stats.WeaponType _t = (Stats.WeaponType)EditorGUILayout.EnumPopup("Weapon Type", atr.WeaponType) ;
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(atr, "Modify attribute");
                atr.range = val;
                atr.WeaponType = _t;
                EditorUtility.SetDirty(target);
            }
            EditorGUILayout.EndVertical();
        }
    }
    [CustomEditor(typeof(StatsAttribute))]
    public class StatAttributeEditor : AttributeEditor
    {
        bool toggle = true;

        public override bool CanDelete()
        {
            return true;
        }

        public override void OnInspectorGUI()
        {
            StatsAttribute atr = (StatsAttribute)target;
            EditorGUILayout.BeginVertical();
     
            toggle = EditorGUILayout.Foldout(toggle, "Stat Changes");
            if (toggle)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < atr.stats.Length; i++)
                {
                  EditorGUILayout.BeginHorizontal();
                    EditorGUI.BeginChangeCheck();
                    float val = EditorGUILayout.FloatField(((Stats.StatType)i).ToString(), atr.stats[i]);
                    if (EditorGUI.EndChangeCheck())
                    {
                    Undo.RecordObject(atr, "Modify attribute");
                    atr.stats[i] = val;
                    atr.enabled[i] = true;
                    EditorUtility.SetDirty(target);
                    }
                    EditorGUI.BeginChangeCheck();
                    bool enabled = EditorGUILayout.Toggle(atr.enabled[i]);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(atr, "Modify attribute");
                        atr.enabled[i] = enabled;
                        EditorUtility.SetDirty(target);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
   
            EditorGUILayout.EndVertical();
        }
    }
    [CustomEditor(typeof(GearSlotAttribute))]
    public class GearSlotAttributeEditor : AttributeEditor
    {
        bool toggle = false;
        string[] _names = new string[] { "Head", "Chest", "Legs", "LHand", "RHand", "Ring" };

        public override bool CanDelete()
        {
            return true;
        }

        public override void OnInspectorGUI()
        {
            GearSlotAttribute atr = (GearSlotAttribute)target;
            EditorGUILayout.BeginVertical();

            toggle = EditorGUILayout.Foldout(toggle,"Gear Slots (Slot Name / Slot / Fill)");
            if (toggle)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < atr.PossibleSlots.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(_names[i]);
                    EditorGUI.BeginChangeCheck();
                    bool slotBoolA = EditorGUILayout.Toggle(atr.PossibleSlots[i]);
                    bool slotBoolB = EditorGUILayout.Toggle(atr.UnequipLots[i]);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RegisterCompleteObjectUndo(atr, "Modify Attribute");
                        atr.UnequipLots[i] = slotBoolB;
                        atr.PossibleSlots[i] = slotBoolA;
                        EditorUtility.SetDirty(target);
                    }
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndVertical();
        }
    }

    [CustomEditor(typeof(Use_RecoverAttribute))]
    public class UrecoverAttributeEditor : AttributeEditor
    {
        bool toggle = true;

        public override bool CanDelete()
        {
            return true;
        }

        public override void OnInspectorGUI()
        {
            Use_RecoverAttribute atr = (Use_RecoverAttribute)target;
            EditorGUILayout.BeginVertical();

            toggle = EditorGUILayout.Foldout(toggle, "Recover when used");
            if (toggle)
            {
                EditorGUI.indentLevel++;

                for (int i = 0; i < atr.Recover.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUI.BeginChangeCheck();
                    float val = EditorGUILayout.FloatField(((Stats.StatType)i).ToString(), atr.Recover[i]);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(atr, "Modify attribute");
                        atr.Recover[i] = Mathf.Max(0, val);

                        if (atr.Recover[i] != 0)
                        {
                            atr.enabled[i] = true;
                        }
                        else
                        {
                            atr.enabled[i] = false;
                        }
                        EditorUtility.SetDirty(target);
                    }
                    EditorGUI.BeginChangeCheck();
                    bool enabled = EditorGUILayout.Toggle(atr.enabled[i]);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(atr, "Modify attribute");
                        atr.enabled[i] = enabled;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.BeginChangeCheck();
                bool use = EditorGUILayout.Toggle("Consume?", atr.destoryOnUse);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(atr, "Modify attribute");
                    atr.destoryOnUse = use;
                    EditorUtility.SetDirty(target);
                }

                EditorGUILayout.Space();
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
    }
}
