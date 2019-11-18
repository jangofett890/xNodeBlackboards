using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace Neon.XNodeBlackboards.Graphing.Blackboard.Editor {
    [CustomNodeEditor(typeof(Blackboard))]
    public class BlackboardNodeEditor : NodeEditor {

        public static BlackboardNodeEditor current;

        private Vector2 ScrollView = new Vector2();
        private GenericMenu AddValueMenu = new GenericMenu();
        private BlackboardObject hoveredBBO = null;
        private BlackboardVariableInstance createdInstance = null;
        private Vector2 mouseOffset = new  Vector2();
        public Dictionary<BlackboardObject, Vector2> bboSizes { get { return _bboSizes; } }
        private Dictionary<BlackboardObject, Vector2> _bboSizes = new Dictionary<BlackboardObject, Vector2>();

        List<System.Type> basicTypes = new List<Type>() { typeof(bool), typeof(string), typeof(int), typeof(float), typeof(Vector2), typeof(Vector3), typeof(Vector4), typeof(GameObject), typeof(ScriptableObject) };
        List<System.Type> unserializableTypes = new List<Type>() { typeof(GameObject), typeof(Component) };

        Blackboard blackboard { get { return (Blackboard)target; } }
        public override void OnCreate() {
            base.OnCreate();

            foreach (System.Type type in basicTypes) {
                string typeName = type.Name;
                if (typeName == "GameObject") {
                    typeName = "Prefab";
                }
                AddValueMenu.AddItem(new GUIContent("Standard Types/" + typeName), false, () => {
                    BlackboardObject Value = null;
                    object val = null;
                    GameObject g;
                    if (typeName != "Prefab") {
                        if (type != typeof(string)) {
                            val = Activator.CreateInstance(type);
                        } else {
                            val = "";
                        }
                        Value = BlackboardSerializedVariable.Create("New " + typeName + " value", type, val);
                    } else {
                        g = new GameObject();
                        Value = BlackboardSerializedVariable.Create("New " + typeName + " value", type,  g);
                        UnityEngine.Object.DestroyImmediate(g);
                    }
                    AddValue(Value);
                });
            }

            foreach (System.Type type in unserializableTypes) {
                AddValueMenu.AddItem(new GUIContent("Scene Objects/" + type.Name), false, () => {
                    BlackboardNonSerializedVariable Value = null;
                    GameObject go;
                    Component co;
                    if (type.Name == "GameObject") {
                        go = new GameObject();
                        Value = BlackboardNonSerializedVariable.Create("New " + type.Name + " value", type, go);
                        UnityEngine.Object.DestroyImmediate(go);
                    }
                    if (type.Name == "Component") {
                        co = new Component();
                        Value = BlackboardNonSerializedVariable.Create("New " + type.Name + " value", type, co);
                        UnityEngine.Object.DestroyImmediate(co);
                    }
                    AddValue(Value);
                });
            }
        }


        private void AddValue(BlackboardObject value) {
            Undo.RecordObject(target.graph, "Create Blackboard Value");
            blackboard.AddValue(value);
            Undo.RegisterCreatedObjectUndo(value, "Create Blackboard Value");
            AssetDatabase.AddObjectToAsset(value, target.graph);
            if (NodeEditorPreferences.GetSettings().autoSave) AssetDatabase.SaveAssets();
            NodeEditorWindow.RepaintAll();
            return;
        }

        public void RemoveSelectedValue() {
            ClearSizes();
            foreach (UnityEngine.Object item in Selection.objects) {
                if (item is BlackboardObject) {
                    BlackboardObject bbo = item as BlackboardObject;
                    RemoveValue(bbo);
                }
            }
        }

        public void RemoveValue(BlackboardObject value) {
            Undo.RecordObject(value, "Delete Node");
            Undo.RecordObject(target, "Delete Node");
            blackboard.RemoveValue(value);
            Undo.DestroyObjectImmediate(value);
            if (NodeEditorPreferences.GetSettings().autoSave) AssetDatabase.SaveAssets();
        }

        public override void Controls(Event e) {
            switch (e.type) {
                case EventType.MouseDown:
                    if (hoveredBBO != null && IsHoveringTitle(hoveredBBO)) {
                        // If mousedown on node header, select or deselect
                        if (!Selection.Contains(hoveredBBO)) {
                            Selection.objects = new UnityEngine.Object[] { hoveredBBO };
                        }
                        e.Use();
                    }
                    break;
                case EventType.MouseDrag:
                    if (e.button == 0) {
                        if (hoveredBBO !=null && IsHoveringTitle(hoveredBBO) && Selection.Contains(hoveredBBO)) {
                            if (createdInstance == null) {
                                createdInstance = BlackboardVariableInstance.Create(hoveredBBO, target.graph as BlackBoardGraph);
                                mouseOffset = NodeEditorWindow.current.WindowToGridPosition(GetValuePosition(hoveredBBO)) - NodeEditorWindow.current.WindowToGridPosition(e.mousePosition);
                                e.Use();
                            } else {
                                createdInstance.position = mouseOffset + NodeEditorWindow.current.WindowToGridPosition(e.mousePosition);
                                e.Use();
                            }
                        } else if (hoveredBBO != null && Selection.Contains(hoveredBBO)) {
                            if(createdInstance != null) {
                                createdInstance.position = mouseOffset + NodeEditorWindow.current.WindowToGridPosition(e.mousePosition);
                                e.Use();
                            }
                        }
                    }
                    break;
                case EventType.MouseUp:
                    if (e.button == 0) {
                        if (hoveredBBO != null && createdInstance != null) {
                            hoveredBBO = null;
                            createdInstance = null;
                        }
                    } else if (e.button == 1 || e.button == 2) {
                        if (hoveredBBO != null && IsHoveringTitle(hoveredBBO)) {
                            if (!Selection.Contains(hoveredBBO)) Selection.objects = new UnityEngine.Object[] { hoveredBBO };
                            GenericMenu menu = new GenericMenu();
                            BlackboardObjectEditor.GetEditor(hoveredBBO, window).AddContextMenuItems(menu);
                            menu.DropDown(new Rect(Event.current.mousePosition, Vector2.zero));
                            e.Use(); // Fixes copy/paste context menu appearing in Unity 5.6.6f2 - doesn't occur in 2018.3.2f1 Probably needs to be used in other places.
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public override void OnBodyGUI() {
            if (target.graph.nodes[target.graph.nodes.Count - 1] != target && createdInstance == null) {
                window.MoveNodeToTop(target);
            }

            Event e = Event.current;
            Vector2 mousePos = Event.current.mousePosition;
            GUILayout.BeginVertical();

            for (int v = 0; v < blackboard.Values.Count; v++) {
                if (blackboard.Values[v] == null) continue;
                if (v >= blackboard.Values.Count) return;
                BlackboardObject value = blackboard.Values[v];
                value.BlackboardListPosition = v;
                BlackboardObjectEditor bboEditor = BlackboardObjectEditor.GetEditor(value, window);
                EditorGUIUtility.labelWidth = 84;
                float height = 0;
                if (_bboSizes != null) {
                    if (_bboSizes.Count == blackboard.Values.Count) {
                        for (int i = 0; i < v; i++) {
                            height += _bboSizes[blackboard.Values[v]].y;
                        }
                    }
                }
                Vector2 bboPos = new Vector2(0, 35 + height);
                GUILayout.BeginArea(new Rect(bboPos, new Vector2(GetWidth(), 100)));
                bool selected = NodeEditorWindow.current.SelectionCache.Contains(value);

                Color guiColor = GUI.color;

                if (selected) {
                    GUIStyle style = new GUIStyle(bboEditor.GetBodyStyle());
                    GUIStyle highlightStyle = new GUIStyle(NodeEditorResources.styles.nodeHighlight);
                    highlightStyle.padding = style.padding;
                    style.padding = new RectOffset();
                    GUI.color = bboEditor.GetTint();
                    GUILayout.BeginVertical(style);
                    GUI.color = NodeEditorPreferences.GetSettings().highlightColor;
                    GUILayout.BeginVertical(new GUIStyle(highlightStyle));
                } else {
                    GUIStyle style = new GUIStyle(bboEditor.GetBodyStyle());
                    GUI.color = bboEditor.GetTint();
                    GUILayout.BeginVertical(style);
                }

                GUI.color = guiColor;
                EditorGUI.BeginChangeCheck();
                bboEditor.OnHeaderGUI();
                bboEditor.OnBodyGUI();
                if (EditorGUI.EndChangeCheck()) {
                    if (BlackboardObjectEditor.onUpdateBBO != null) BlackboardObjectEditor.onUpdateBBO(value);
                    EditorUtility.SetDirty(value);
                    bboEditor.serializedObject.ApplyModifiedProperties();
                }

                GUILayout.EndVertical();

                if (e.type == EventType.Repaint) {
                    Vector2 size = GUILayoutUtility.GetLastRect().size;
                    if (bboSizes.ContainsKey(value)) bboSizes[value] = size;
                    else bboSizes.Add(value, size);
                }

                    if (selected) GUILayout.EndVertical();

                if (e.type != EventType.Layout) {
                    //Check if we are hovering this node
                    Vector2 bboSize = GUILayoutUtility.GetLastRect().size;
                    Rect windowRect = new Rect(bboPos, bboSize);
                    if (windowRect.Contains(mousePos)) hoveredBBO = value;
                }


                GUILayout.EndArea();
            }

            for (int v = 0; v < blackboard.Values.Count; v++) {
                Vector2 size = new Vector2();
                if (!bboSizes.TryGetValue(blackboard.Values[v], out size))
                    continue;
                try {
                    GUILayout.Space(size.y);
                } catch {
                    //I seriously don't know how to solve Getting control x pos in group issue
                }
            }
            try {
                if (GUILayout.Button("Add Variable")) {
                AddValueMenu.ShowAsContext();
                }
            } catch {
                //I seriously don't know how to solve Getting control x pos in group issue
            }
            GUILayout.EndVertical();
        }

        public void ClearSizes() {
            _bboSizes = new Dictionary<BlackboardObject, Vector2>();
        }

        bool IsHoveringTitle(BlackboardObject bbo) {
            Vector2 mousePos = Event.current.mousePosition;
            //Get node position
            Vector2 blackboardPos = NodeEditorWindow.current.GridToWindowPosition(blackboard.position);
            blackboardPos.y += 35;
            float height = 0;
            if (_bboSizes != null) {
                if (_bboSizes.Count == blackboard.Values.Count) {
                    for (int i = 0; i < bbo.BlackboardListPosition; i++) {
                        height += _bboSizes[blackboard.Values[i]].y;
                    }
                }
            }
            Vector2 bboPos = new Vector2(blackboardPos.x, blackboardPos.y + height);
            float width;
            Vector2 size;
            if (bboSizes.TryGetValue(bbo, out size)) width = size.x;
            else width = 200;
            Rect windowRect = new Rect(bboPos, new Vector2(width, 30));
            return windowRect.Contains(mousePos);
        }

        Vector2 GetValuePosition(BlackboardObject bbo) {
            Vector2 blackboardPos = NodeEditorWindow.current.GridToWindowPosition(blackboard.position);
            blackboardPos.y += 35;
            float height = 0;
            if (_bboSizes != null) {
                if (_bboSizes.Count == blackboard.Values.Count) {
                    for (int i = 0; i < bbo.BlackboardListPosition; i++) {
                        height += _bboSizes[blackboard.Values[i]].y;
                    }
                }
            }
            Vector2 bboPos = new Vector2(blackboardPos.x, blackboardPos.y + height);
            return bboPos;
        }
    }
}

