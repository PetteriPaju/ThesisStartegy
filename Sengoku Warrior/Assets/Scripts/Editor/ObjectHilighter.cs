using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
namespace SengokuWarrior
{
    static class ObjectHilighter
    {
        private static Dictionary<SpriteRenderer, Material> Originals = new Dictionary<SpriteRenderer, Material>();
        public static UnityEngine.Object currentEditor;

        public static void SetColors(List<SpriteRenderer> rends, UnityEngine.Object current)
        {
            foreach (SpriteRenderer rend in rends)
            {
                Add(rend, current);
            }
            
        }

        public static void Add(SpriteRenderer rend, UnityEngine.Object current)
        {
            if (current != currentEditor) Clear();

            currentEditor = current;

            if (!Originals.ContainsKey(rend))
            {
                Originals.Add(rend, rend.sharedMaterial);
                rend.material = EditorTools.CurrentInspectedGrid.EditorSelectedMaterial;
            }
        }
        public static void Clear()
        {
            foreach (var rend in Originals.Keys)
            {
                rend.material = Originals[rend];
            }
            Originals.Clear();
        }
        public static void ClearIfLast(UnityEngine.Object current)
        {
            if(current == currentEditor)
            Clear();
        }


    }




public class FileModificationWarning : UnityEditor.AssetModificationProcessor
    {
        static string[] OnWillSaveAssets(string[] paths)
        {
            ObjectHilighter.Clear();
            return paths;
        }
    }
}
