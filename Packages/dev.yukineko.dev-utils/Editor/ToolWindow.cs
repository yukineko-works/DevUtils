using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UdonSharp;
using UnityEditor;
using UnityEngine;
using VRC.Udon;

namespace yukineko.DevUtils
{
    internal class UdonBehaviourCounter : EditorWindow
    {
        [MenuItem("Window/YN DevUtils/Udon Behaviour Counter")]
        private static void ShowWindow()
        {
            GetWindow<UdonBehaviourCounter>("Udon Behaviour Counter");
        }

        private GameObject _rootObject;
        private bool _includeInactive = true;
        private Vector2 _scrollPosition;

        private List<UdonBehaviour> _udonBehavioursCache = new List<UdonBehaviour>();

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Udon Behaviour Counter", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            _rootObject = EditorGUILayout.ObjectField("Root Object", _rootObject, typeof(GameObject), true) as GameObject;
            _includeInactive = EditorGUILayout.Toggle("Include Inactive", _includeInactive);

            if (_rootObject == null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("Please assign a root object to count UdonBehaviours from.", MessageType.Info);
            }

            EditorGUILayout.Space();

            GUI.enabled = _rootObject != null;
            if (GUILayout.Button("Count"))
            {
                _udonBehavioursCache.Clear();
                _rootObject.GetComponentsInChildren(_includeInactive, _udonBehavioursCache);
            }
            GUI.enabled = true;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("UdonBehaviours found: ", _udonBehavioursCache.Count.ToString(), EditorStyles.boldLabel);
            EditorGUILayout.Space();

            if (_udonBehavioursCache.Count > 0)
            {
                EditorGUILayout.LabelField("UdonBehaviours breakdown", EditorStyles.boldLabel);
                var udonBehaviourCount = new Dictionary<string, int>();
                foreach (var udonBehaviour in _udonBehavioursCache)
                {
                    var udonBehaviourName = udonBehaviour.programSource.name;
                    if (udonBehaviourCount.ContainsKey(udonBehaviourName))
                    {
                        udonBehaviourCount[udonBehaviourName]++;
                    }
                    else
                    {
                        udonBehaviourCount[udonBehaviourName] = 1;
                    }
                }

                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
                EditorGUI.indentLevel++;
                foreach (var pair in udonBehaviourCount.OrderByDescending(pair => pair.Value))
                {
                    EditorGUILayout.LabelField($"{pair.Key}: {pair.Value}");
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.EndScrollView();
            }
        }
    }

    internal class UdonBehaviourFinder : EditorWindow
    {
        [MenuItem("Window/YN DevUtils/Udon Behaviour Finder")]
        private static void ShowWindow()
        {
            GetWindow<UdonBehaviourFinder>("Udon Behaviour Finder");
        }

        private GameObject _rootObject;
        private string _searchString;
        private bool _includeInactive = true;
        private Vector2 _scrollPosition;

        private List<UdonBehaviour> _udonBehavioursCache = new List<UdonBehaviour>();

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Udon Behaviour Finder", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            _rootObject = EditorGUILayout.ObjectField("Root Object", _rootObject, typeof(GameObject), true) as GameObject;
            _includeInactive = EditorGUILayout.Toggle("Include Inactive", _includeInactive);

            if (_rootObject == null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("Please assign a root object to find UdonBehaviours from.", MessageType.Info);
            }

            EditorGUILayout.Space();

            GUI.enabled = _rootObject != null;
            if (GUILayout.Button("Find"))
            {
                _udonBehavioursCache.Clear();
                _rootObject.GetComponentsInChildren(_includeInactive, _udonBehavioursCache);
            }
            GUI.enabled = true;

            if (_rootObject == null)
            {
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("UdonBehaviours found: ", _udonBehavioursCache.Count.ToString(), EditorStyles.boldLabel);
            EditorGUILayout.Space();
            _searchString = EditorGUILayout.TextField("Search", _searchString);
            EditorGUILayout.Space();

            if (_udonBehavioursCache.Count > 0)
            {
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
                foreach (var udonBehaviour in _udonBehavioursCache)
                {
                    if (string.IsNullOrEmpty(_searchString) || udonBehaviour.programSource.name.Contains(_searchString))
                    {
                        EditorGUILayout.BeginHorizontal();
                        var icon = PrefabUtility.GetIconForGameObject(udonBehaviour.gameObject);
                        var prefabStatus = PrefabUtility.GetPrefabInstanceStatus(udonBehaviour.gameObject);

                        EditorGUILayout.BeginVertical();
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label(icon, GUILayout.Width(20), GUILayout.Height(20));
                        EditorGUILayout.LabelField($"{udonBehaviour.name} ({udonBehaviour.programSource.name})");
                        EditorGUILayout.EndHorizontal();

                        var inPrefab = prefabStatus == PrefabInstanceStatus.Connected;
                        var rootObject = inPrefab ? PrefabUtility.GetNearestPrefabInstanceRoot(udonBehaviour.gameObject) : null;

                        if (inPrefab && rootObject != null && rootObject != udonBehaviour.gameObject)
                        {
                            EditorGUILayout.BeginHorizontal();
                            var parentPrefabIcon = PrefabUtility.GetIconForGameObject(rootObject);

                            GUILayout.Space(20);
                            GUILayout.Label(parentPrefabIcon, GUILayout.Width(20), GUILayout.Height(20));
                            EditorGUILayout.LabelField($"RootPrefab: {rootObject.name}");

                            EditorGUILayout.EndHorizontal();
                        }

                        EditorGUILayout.EndVertical();

                        if (Selection.activeObject == udonBehaviour)
                        {
                            GUI.backgroundColor = Color.cyan;
                        }

                        if (GUILayout.Button("Select", GUILayout.Width(120)))
                        {
                            Selection.activeObject = udonBehaviour;
                        }


                        GUI.backgroundColor = Color.white;

                        EditorGUILayout.EndHorizontal();
                        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
                    }
                }
                EditorGUILayout.EndScrollView();
            }
        }
    }
}
