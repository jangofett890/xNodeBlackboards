using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Neon.xNodeBlackboards.Graphing.Blackboard {
    [Serializable]
    public class BlackboardObject : ScriptableObject {

        [SerializeField] protected string type;
        [SerializeField] protected string value;
        [SerializeField] UnityEngine.Object objVal;
        [SerializeField] public Blackboard Blackboard;
        public int BlackboardListPosition = 0;

        public virtual object Value {
            get {
                if (value != null && Type != null || objVal != null && Type != null) {
                    if (typeof(UnityEngine.Object).IsAssignableFrom(Type)) {
                        return objVal;
                    } else if (Type == typeof(int)) {
                        if (!string.IsNullOrEmpty(value)) {
                            return int.Parse(value);
                        }
                    } else if (Type == typeof(float)) {
                        if (!string.IsNullOrEmpty(value)) {
                            return float.Parse(value);
                        }
                    } else if (Type == typeof(string)) {
                        return value;
                    } else if (Type == typeof(bool)) {
                        if (!string.IsNullOrEmpty(value)) {
                            return bool.Parse(value);
                        }
                    } else {
                        return JsonUtility.FromJson(value, Type);
                    }
                }
                return null;
            }
            set {
                if (value == null) {
                    return;
                } else if (value == null) {
                    objVal = null;
                    return;
                }
                if (value.GetType() == Type || typeof(UnityEngine.Object).IsAssignableFrom(Type)) {
                    if (typeof(UnityEngine.Object).IsAssignableFrom(Type)) {
                        objVal = (UnityEngine.Object)value;
                    } else if (Type == typeof(int) || Type == typeof(float) || Type == typeof(string) || Type == typeof(bool)) {
                        this.value = value.ToString();
                    } else {
                        this.value = JsonUtility.ToJson(value);
                    }
                } else {
                    Debug.LogWarning("Trying to set blackboard value with incorrect type");
                }

            }
        }
        public virtual System.Type Type { get { 
                return System.Type.GetType(type, true); } protected set { type = value.AssemblyQualifiedName; } }

        public static BlackboardObject Create(string Name, Type type, object Value) {
            BlackboardObject o = CreateInstance<BlackboardObject>();
            o.name = Name;
            o.Type = type;
            o.Value = Value;

            return o;
        }
        protected void OnEnable() {
            Init();
        }
        public virtual void Init() { }

    }

}
