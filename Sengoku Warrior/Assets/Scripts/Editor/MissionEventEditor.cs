using UnityEditor;
using UnityEngine;

namespace SengokuWarrior
{
    public class MissionEventEditor
    {
        protected MissionEvent target;
        protected Mission miss;
        private bool conditionFold = false;

        private void initialize(MissionEvent evt, Mission miss)
        {
            this.target = evt;
            this.miss = miss;
        }

        public virtual void OnGUI()
        {
        }

        public static MissionEventEditor Create(MissionEvent evt, Mission miss)
        {
            MissionEventEditor editor = null;
            System.Type type = evt.GetType();
            if (type == typeof(ME_KillCharacter))
            {
                editor = new ME_KillCharacterEditor() as MissionEventEditor;
            }
            else if (type == typeof(ME_GiveItem))
            {
                editor = new ME_GiveItemsEditor() as MissionEventEditor;
            }
            else if (type == typeof(ME_Visu16))
            {
                editor = new ME_Visu16Editor() as MissionEventEditor;
            }
            if (editor != null)
                editor.initialize(evt, miss);

            return editor;
        }

    }


    public class ME_KillCharacterEditor : MissionEventEditor
    {
        public override void OnGUI()
        {

            ME_KillCharacter evt = (ME_KillCharacter)target;


            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+", GUILayout.Width(25)))
            {

                GenericMenu CreateMenu = new GenericMenu();
                int _i = 0;
                for (int i = 0; i < miss.Teams.Count; i++)
                {
                    int _q = 0;
                    for (int q = 0; q < miss.Teams[i].members.Count; q++)
                    {
                        _i = i;
                        _q = q;
                        CreateMenu.AddItem(new GUIContent("Team " + i + "/" + miss.Teams[_i].members[_q]._Name), false, Add, miss.Teams[_i].members[_q].id);
                    }
                }
                CreateMenu.ShowAsContext();

            }
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < evt.charactersToKill.Count; i++)
            {
                Character chara = miss.FindCharacter(evt.charactersToKill[i]);


                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    Undo.RegisterCompleteObjectUndo(miss, "Modify Condition");
                    evt.charactersToKill.RemoveAt(i);
                }
                GUILayout.Space(-45);
                EditorGUILayout.LabelField(chara._Name);

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

            }
            EditorGUILayout.Space();
            base.OnGUI();
            EditorGUILayout.Space();
        }
        void Add(object obj)
        {
            ME_KillCharacter evt = (ME_KillCharacter)target;
            int id = (int)obj;

            if (!evt.charactersToKill.Contains(id))
            {
                evt.charactersToKill.Add(id);
            }

        }
    }

    public class ME_GiveItemsEditor : MissionEventEditor
    {
        public override void OnGUI()
        {

            ME_GiveItem evt = (ME_GiveItem)target;


 

            EditorGUILayout.BeginHorizontal();
            if (evt.CharacterId != -1)
            {
                EditorGUILayout.LabelField("Character :" + miss.FindCharacter(evt.CharacterId)._Name);
            }
            else
            {
                EditorGUILayout.HelpBox("Please Set Character!", MessageType.Warning);
            }
            if (GUILayout.Button("Select"))
            {
                GenericMenu CreateMenu = new GenericMenu();
                int _i = 0;
                for (int i = 0; i < miss.Teams.Count; i++)
                {
                    int _q = 0;
                    for (int q = 0; q < miss.Teams[i].members.Count; q++)
                    {
                        _i = i;
                        _q = q;
                        CreateMenu.AddItem(new GUIContent("Team " + i + "/" + miss.Teams[_i].members[_q]._Name), false, AddCharacter, miss.Teams[_i].members[_q].id.ToInt());
                    }
                }
                CreateMenu.ShowAsContext();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (evt.itemToGive != null)
            {
                EditorGUILayout.LabelField("Item :" + evt.itemToGive.ItemName);
            }
            else
            {
                EditorGUILayout.HelpBox("Please Set Item!", MessageType.Warning);
            }


            if (GUILayout.Button("Select"))
            {

                GenericMenu CreateMenu = new GenericMenu();
                int _i = 0;
                for (int i = 0; i < EditorTools.ItemDatabase.ItemNames.Length; i++)
                {
                    _i = i;
                    CreateMenu.AddItem(new GUIContent(EditorTools.ItemDatabase.ItemNames[i]), false, AddItem, _i);

                }
                CreateMenu.ShowAsContext();

            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.BeginChangeCheck();
            evt.amount = EditorGUILayout.IntField(evt.amount);

            if (EditorGUI.EndChangeCheck())
            {
                evt.amount = Mathf.Max(1, evt.amount);
            }

           
            EditorGUILayout.Space();
            base.OnGUI();
            EditorGUILayout.Space();
        }
        void AddItem(object obj)
        {
            ME_GiveItem evt = (ME_GiveItem)target;
            int index = (int)obj;

            evt.itemToGive =  EditorTools.ItemDatabase.items[index];
            evt.amount = 1;

        }
        void AddCharacter(object obj)
        {
            ME_GiveItem evt = (ME_GiveItem)target;
            int id = (int)obj;

            evt.CharacterId = id;
        }
    }

    public class ME_Visu16Editor : MissionEventEditor
    {
        public override void OnGUI()
        {

            ME_Visu16 evt = (ME_Visu16)target;

            evt.scene = EditorGUILayout.ObjectField("Scene", evt.scene, typeof(Visu16.VisuScene), false) as Visu16.VisuScene;

            EditorGUILayout.Space();
            base.OnGUI();
            EditorGUILayout.Space();
        }
      
    }
}
