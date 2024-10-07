using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace yukineko.DevUtils
{
    internal static class ProjectMenu
    {
        [MenuItem("Assets/YN DevUtils/Copy GUID", true)]
        private static bool CopyGuidValidate()
        {
            return Selection.activeObject != null && !AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(Selection.activeObject));
        }

        [MenuItem("Assets/YN DevUtils/Copy GUID")]
        private static void CopyGuid()
        {
            var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(Selection.activeObject));
            EditorGUIUtility.systemCopyBuffer = guid;
        }
    }
}