using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace Neon.xNodeBlackboards.Graphing.Blackboard {
    [CreateNodeMenu("")]
    public class BlackboardVariableInstance : Node {
        [SerializeField] public BlackboardObject value;

        public static BlackboardVariableInstance Create(BlackboardObject value, BlackBoardGraph graph) {
            Node.graphHotfix = graph;
            Undo.RecordObject(graph, "Create Blackboard Value Instance");
            BlackboardVariableInstance node = CreateInstance<BlackboardVariableInstance>();
            node.value = value;
            node.graph = graph;
            if (node.name == null || node.name.Trim() == "") node.name = NodeEditorUtilities.NodeDefaultName(typeof(BlackboardVariableInstance));
            node.AddDynamicOutput(value.Type, ConnectionType.Multiple, TypeConstraint.InheritedInverse, "ValueOut");
            node.AddDynamicInput(value.Type, ConnectionType.Override, TypeConstraint.Inherited, "ValueIn");
            graph.nodes.Add(node);
            Undo.RegisterCreatedObjectUndo(node, "Create Blackboard Value Instance");
            AssetDatabase.AddObjectToAsset(node, graph);
            return node;
        }

        protected override void Init() {
            base.Init();
            BlackBoardGraph g = (graph as BlackBoardGraph);
            g.BlackboardValuesSet -= UpdateValue;
            g.BlackboardValuesSet += UpdateValue;
            g.BlackboardValuesRetrievedExternally -= UpdateValue;
            g.BlackboardValuesRetrievedExternally += UpdateValue;
        }

        public void UpdateValue() {
            if (GetPort("ValueIn").IsConnected) {
                value.Value = GetPort("ValueIn").GetInputValue();
            }
        }

        public override void OnCreateConnection(NodePort from, NodePort to) {
            if (to.fieldName == "ValueIn") {
                if (from.ValueType != value.Type && !value.Type.IsAssignableFrom(from.ValueType)) {
                    return;
                }
                if (value != null) {
                    if (from.GetOutputValue() != null) {
                        value.Value = from.GetOutputValue();
                    }
                }
            }
        }

        public override object GetValue(NodePort port) {
            if (port.fieldName == "ValueOut") {
                return value.Value;
            } else if (port.fieldName == "ValueIn") {
                value.Value = port.GetInputValue();
                return value.Value;
            } else {
                return base.GetValue(port);
            }
        }
    }
}

