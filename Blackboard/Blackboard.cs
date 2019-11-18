using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Neon.xNodeBlackboards.Graphing.Blackboard {
    [NodeTint(0.1f, 0.1f, 0.1f)]
    [CreateNodeMenu("")]
    [Serializable]
    public class Blackboard : Node {

        public List<BlackboardObject> Values { get { return ((BlackBoardGraph)graph).I_BlackboardValues; } }

        public virtual BlackboardObject AddValue(BlackboardObject value) {
            value.Blackboard = this;
            ((BlackBoardGraph)graph).I_BlackboardValues.Add(value);
            return value;
        }
        public void RemoveValue(BlackboardObject value) {
            ((BlackBoardGraph)graph).I_BlackboardValues.Remove(value);
            if (Application.isPlaying) Destroy(value);
        }

        protected override void Init() {
            name = "Blackboard";
        }
    }
}

