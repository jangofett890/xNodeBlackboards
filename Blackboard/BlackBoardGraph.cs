using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XNode;

namespace Neon.xNodeBlackboards.Graphing.Blackboard {
    public class BlackBoardGraph : NodeGraph {

        public Action BlackboardValuesSet = () => { };
        public Action BlackboardValuesRetrievedExternally = () => { };
        [SerializeField] public Blackboard Blackboard;
        [SerializeField] private List<BlackboardObject> blackboardValues = new List<BlackboardObject>();
        private List<BlackboardObject> bbvOld = new List<BlackboardObject>();

        //Internal use ONLY do not use this to get the values if you are outside of the graph as they will not update properly.
        internal List<BlackboardObject> I_BlackboardValues { 
            get {
                return blackboardValues; 
            } set {
                if (bbvOld != blackboardValues) {
                    blackboardValues = value;
                    bbvOld = blackboardValues;
                    BlackboardValuesSet.Invoke();
                } else {
                    blackboardValues = value;
                }

            } 
        }

        //Please Cache for performance
        public List<BlackboardObject> BlackboardValues {
            get {
                BlackboardValuesRetrievedExternally.Invoke();
                return blackboardValues;
            }
        }

        public override void RemoveNode(Node node) {
            if (node.GetType() == typeof(Blackboard)) {
                return;
            }
            base.RemoveNode(node);
        }
    }
}

