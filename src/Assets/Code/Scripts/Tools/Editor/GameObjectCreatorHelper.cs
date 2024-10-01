using UnityEditor;
using UnityEngine;

namespace Tools.Editor
{
    public static class GameObjectCreatorHelper
    {
        [MenuItem("GameObject/--- Spacer ---", priority = -100)]
        private static void CreateSpacer()
        {
            // Ask for spacer name.
            string spacerName = EditorInputDialog.Show("Create Spacer", "How should the spacer be named?", "");
            
            // Return if user cancelled.
            if(string.IsNullOrEmpty(spacerName))
                return;

            spacerName = spacerName.ToUpper();
            
            GameObject go = new GameObject($"= {spacerName}");
            //GameObject go = new GameObject($"----- {spacerName} -----");
            
            go.SetActive(false);

            go.tag = "EditorOnly";

            if (Selection.activeGameObject != null)
            {
                int selectionIndex = Selection.activeGameObject.transform.GetSiblingIndex();
                go.transform.SetSiblingIndex(selectionIndex + 1);
            }
        
            Undo.RegisterCreatedObjectUndo(go, $"Create new spacer {spacerName}");
        }
    }
}