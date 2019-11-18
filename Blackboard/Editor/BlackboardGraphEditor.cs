using Neon.XNodeBlackboards.Graphing.Blackboard.Editor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace Neon.XNodeBlackboards.Graphing.Blackboard {

    [CustomNodeGraphEditor(typeof(BlackBoardGraph), "URPG.Settings")]
    public class BlackboardGraphEditor : NodeGraphEditor {

        public override void RemoveNode(Node node) {
            if (node.GetType() == typeof(Blackboard)) {
                return;
            }
            base.RemoveNode(node);
        }

        public override void OnCreate() {
            base.OnCreate();
            if (target.nodes.Where(a => a.GetType() == typeof(Blackboard)).FirstOrDefault() == null) {
                Blackboard node = (Blackboard)target.AddNode(typeof(Blackboard));
                node.position = new Vector2(0,0);
                if (node.name == null || node.name.Trim() == "") node.name = NodeEditorUtilities.NodeDefaultName(typeof(Blackboard));
                //Normally we would check for autosave first, but since this should be standard in this graph we donot.
                //Also GetSettings() doesn't work when closing the EditorWindow of a previous graph until this graph is opened fully. 
                AssetDatabase.SaveAssets();
                NodeEditorWindow.RepaintAll();
                AssetDatabase.AddObjectToAsset(node, target);
                (target as BlackBoardGraph).Blackboard = node;
                AssetDatabase.SaveAssets();
            }
        }

        public override void OnOpen() {
            base.OnOpen();
            BlackboardNodeEditor.current = (BlackboardNodeEditor)BlackboardNodeEditor.GetEditor((target as BlackBoardGraph).Blackboard, window);
        }
    }
}

