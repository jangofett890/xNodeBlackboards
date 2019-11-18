using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace Neon.xNodeBlackboards.Graphing.Blackboard {
    [CreateNodeMenu("Blackboard/Get Blackboard Values from Graph")]
    public class GetBlackboardValueFromGraph : Node {
        [Input(connectionType = ConnectionType.Override)] public BlackBoardGraph bbGraph;

        public override void OnCreateConnection(NodePort from, NodePort to) {
            if (to == GetPort("bbGraph") && from.ValueType.IsAssignableFrom(typeof(BlackBoardGraph))) {
                bbGraph = (BlackBoardGraph)from.GetOutputValue();
                foreach (BlackboardObject bbo in bbGraph.BlackboardValues) {
                    if (HasPort(bbo.name)) {
                        //TODO Increment names
                        Debug.Log("Trying to add a port with a variable of the same name.");
                        continue;
                    } else {
                        AddDynamicOutput(bbo.Type, ConnectionType.Multiple, TypeConstraint.InheritedInverse, bbo.name);
                    }
                }
            }
        }

        public override void OnRemoveConnection(NodePort port) {
            if (port == GetPort("bbGraph")) {
                ClearDynamicPorts();
            }
        }

        public override object GetValue(NodePort port) {
            if (bbGraph != null) {
                foreach(BlackboardObject bbo in bbGraph.BlackboardValues) {
                    if (bbo.name == port.fieldName) {
                        return bbo.Value;
                    }
                }
            }
            return base.GetValue(port);    
        }
    }
}

