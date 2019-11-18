using Neon.XNodeBlackboards.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Neon.XNodeBlackboards.Graphing.Blackboard {
    [System.Serializable]
    public class BlackboardNonSerializedVariable : BlackboardObject {
        //The ID used to find the actual object
        [SerializeField] string ID;

        public override object Value {
            get {
                if (!string.IsNullOrEmpty(ID)) {
                    return NonSerializableHelper.FindObjectWithID(ID, Type);
                } else {
                    return null;
                }
            }
            set {
                if (value != null) {
                    if (value.GetType() == Type || value.GetType().IsSubclassOf(typeof(Component))) {
                        if (value.GetType().IsSubclassOf(typeof(Component))) {
                            //Since object is a monobehavior we have to change the type every time a new MonoBehavior is selected
                            type = value.GetType().AssemblyQualifiedName;
                        }

                        if (value.GetType() == typeof(GameObject)) {
                            ID = NonSerializableHelper.AddIDToObject((GameObject)value);
                        }
                        if (value.GetType().IsSubclassOf(typeof(Component))) {
                            ID = NonSerializableHelper.AddIDToObject((Component)value);
                        }
                    } else {
                        Debug.LogWarning("Trying to set blackboard value with incorrect type");
                    }
                } else {
                    ID = "";
                    if (Type.IsSubclassOf(typeof(Component))) {
                        type = typeof(Component).AssemblyQualifiedName;
                    }
                }
            }
        }

        public static new BlackboardNonSerializedVariable Create(string Name, Type type, object Value) {
            BlackboardNonSerializedVariable o = CreateInstance<BlackboardNonSerializedVariable>();
            o.name = Name;
            o.Type = type;
            o.Value = Value;

            return o;
        }
    }
}
