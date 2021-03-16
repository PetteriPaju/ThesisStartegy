using UnityEditor;
using UnityEngine;

namespace SengokuWarrior
{
    [InitializeOnLoad]
    public class OnSceneGUIHelper
    {
        public delegate void UpdateDelegate(SceneView sceneView);
        public static UpdateDelegate updateMethod;
        public static UpdateDelegate updateMethodSecondary;

        public static void Register(UpdateDelegate method)
        {
            updateMethod = method;
        }
        public static void RegisterSecondary(UpdateDelegate method)
        {
            updateMethodSecondary = method;
        }
        public static void UnRegister(UpdateDelegate method)
        {
            updateMethod -= method;
        }
        public static void UnRegisterSecondary(UpdateDelegate method)
        {
            updateMethodSecondary -= method;
        }
        static OnSceneGUIHelper()
        {
            SceneView.onSceneGUIDelegate += Update;
        }

        static void Update(SceneView sceneView)
        {
            if (updateMethod != null) updateMethod.Invoke(sceneView);
            if (updateMethodSecondary != null) updateMethodSecondary.Invoke(sceneView);
        }

    }
}
