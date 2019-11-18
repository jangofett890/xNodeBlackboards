using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Neon.xNodeBlackboards.Graphing.Blackboard.Editor.Internal;
using System;
using UnityEditor;
using XNodeEditor;
using System.Linq;

namespace Neon.xNodeBlackboards.Graphing.Blackboard.Editor {
    [CustomBlackboardObjectEditor(typeof(BlackboardObject))]
    public class BlackboardObjectEditor : BlackboardObjectEditorBase<BlackboardObjectEditor, BlackboardObjectEditor.CustomBlackboardObjectEditorAttribute, BlackboardObject> {

        private readonly Color DEFAULTCOLOR = new Color32(90, 97, 105, 255);

        /// <summary> Fires every whenever a node was modified through the editor </summary>
        public static Action<BlackboardObject> onUpdateBBO;

#if ODIN_INSPECTOR
        internal static bool inNodeEditor = false;
#endif

        public virtual void OnHeaderGUI() {
            GUILayout.Label(target.name, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        }

        /// <summary> Draws standard field editors for all public fields </summary>
        public virtual void OnBodyGUI() {
#if ODIN_INSPECTOR
            inNodeEditor = true;
#endif

            // Unity specifically requires this to save/update any serial object.
            // serializedObject.Update(); must go at the start of an inspector gui, and
            // serializedObject.ApplyModifiedProperties(); goes at the end.
            serializedObject.Update();
            string[] excludes = { "m_Script", "graph", "position", "ports" };

#if ODIN_INSPECTOR
            InspectorUtilities.BeginDrawPropertyTree(objectTree, true);
            GUIHelper.PushLabelWidth(84);
            objectTree.Draw(true);
            InspectorUtilities.EndDrawPropertyTree(objectTree);
            GUIHelper.PopLabelWidth();
#else
            if (typeof(UnityEngine.Object).IsAssignableFrom(target.Type)) {
                if (target.GetType() == typeof(BlackboardNonSerializedVariable)) {
                    target.Value = EditorGUILayout.ObjectField((UnityEngine.Object)target.Value, target.Type, true);
                } else {
                    target.Value = EditorGUILayout.ObjectField((UnityEngine.Object)target.Value, target.Type, false);
                }
            } else {
                switch (target.Type.Name) {
                    case ("Int32"):
                        target.Value = EditorGUILayout.IntField((int)target.Value);
                        break;
                    case ("Single"):
                        target.Value = EditorGUILayout.FloatField((float)target.Value);
                        break;
                    case ("Vector2"):
                        target.Value = EditorGUILayout.Vector2Field("", (Vector2)target.Value);
                        break;
                    case ("Vector3"):
                        target.Value = EditorGUILayout.Vector3Field("", (Vector3)target.Value);
                        break;
                    case ("Vector4"):
                        target.Value = EditorGUILayout.Vector4Field("", (Vector4)target.Value);
                        break;
                    case ("String"):
                        target.Value = EditorGUILayout.TextField("", (string)target.Value);
                        break;
                    case ("Boolean"):
                        target.Value = EditorGUILayout.Toggle("", (bool)target.Value);
                        break;
                    default:
                        EditorGUILayout.LabelField(target.Type.Name);
                        break;
                }
            }
            // Iterate through serialized properties and draw them like the Inspector (But with ports)
            //SerializedProperty iterator = serializedObject.GetIterator();
            //bool enterChildren = true;
            //while (iterator.NextVisible(enterChildren)) {
            //    enterChildren = false;
            //    if (excludes.Contains(iterator.name)) continue;
            //    EditorGUILayout.PropertyField(iterator, true);
            //}
#endif

            serializedObject.ApplyModifiedProperties();

#if ODIN_INSPECTOR
            // Call repaint so that the graph window elements respond properly to layout changes coming from Odin    
            if (GUIHelper.RepaintRequested) {
                GUIHelper.ClearRepaintRequest();
                window.Repaint();
            }
#endif

#if ODIN_INSPECTOR
            inNodeEditor = false;
#endif
        }

        public virtual int GetWidth() {
            Type type = target.GetType();
            int width;
            if (type.TryGetAttributeWidth(out width)) return width;
            else return 208;
        }

        /// <summary> Returns color for target node </summary>
        public virtual Color GetTint() {
            // Try get color from [NodeTint] attribute
            Type type = target.GetType();
            Color color;
            if (type.TryGetAttributeTint(out color)) return color;
            // Return default color (grey)
            else return DEFAULTCOLOR;
        }

        public virtual GUIStyle GetBodyStyle() {
            return NodeEditorResources.styles.nodeBody;
        }

        /// <summary> Add items for the context menu when right-clicking this node. Override to add custom menu items. </summary>
        public virtual void AddContextMenuItems(GenericMenu menu) {
            // Actions if only one node is selected
            if (Selection.objects.Length == 1 && Selection.activeObject is BlackboardObject) {
                BlackboardObject bbo = Selection.activeObject as BlackboardObject;
                menu.AddItem(new GUIContent("Rename"), false, () => {
                    if (BlackboardNodeEditor.current != null) {
                        RenamePopup.Show(Selection.activeObject, BlackboardNodeEditor.current.GetWidth());
                    } else {
                        RenamePopup.Show(Selection.activeObject);
                    }
                } );
            }

            menu.AddItem(new GUIContent("Remove"), false, () => { BlackboardNodeEditor.current.RemoveSelectedValue();  });

            // Custom sctions if only one node is selected
            if (Selection.objects.Length == 1 && Selection.activeObject is BlackboardObject) {
                BlackboardObject bbo = Selection.activeObject as BlackboardObject;
                menu.AddCustomContextMenuItems(bbo);
            }
        }

        /// <summary> Rename the node asset. This will trigger a reimport of the node. </summary>
        public void Rename(string newName) {
            if (newName == null || newName.Trim() == "") newName = "New " + target.Type + " value";
            target.name = newName;
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(target));
        }

        [AttributeUsage(AttributeTargets.Class)]
        public class CustomBlackboardObjectEditorAttribute : Attribute, BlackboardObjectEditorBase<BlackboardObjectEditor, BlackboardObjectEditor.CustomBlackboardObjectEditorAttribute, BlackboardObject>.IBlackboardObjectEditorAttrib {
            private Type inspectedType;
            /// <summary> Tells a NodeEditor which Node type it is an editor for </summary>
            /// <param name="inspectedType">Type that this editor can edit</param>
            public CustomBlackboardObjectEditorAttribute(Type inspectedType) {
                this.inspectedType = inspectedType;
            }

            public Type GetInspectedType() {
                return inspectedType;
            }
        }
    }
}

