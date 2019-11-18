using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Neon.xNodeBlackboards.Utility {
    public static class NonSerializableHelper {
        public static object FindObjectWithID(string ID, Type type) {
            if (type == typeof(GameObject)) {
                GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
                foreach (GameObject go in allObjects) {
                    if (go.name == "_ID") {
                        if (go.transform.GetChild(0).gameObject.name == ID) {
                            return go.transform.parent.gameObject;
                        }
                    }
                }
                return null;
            } else if (type.IsSubclassOf(typeof(Component))) {
                GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
                foreach (GameObject go in allObjects) {
                    if (go.name == "_ID" + type.FullName) {
                        if (go.transform.GetChild(0).gameObject.name == ID) {
                            return go.transform.parent.gameObject.GetComponent(type);
                        }
                    }
                }
                return null;
            } else {
                return null;
            }
        }
        public static void AddIDToObject(GameObject go, string ID) {
            foreach (Transform child in go.transform) {
                if (child.name == "_ID") {
                    child.GetChild(0).gameObject.name = ID;
                    return;
                }
            }

            GameObject newIDHolder = new GameObject();
            GameObject newID = new GameObject();
            newIDHolder.name = "_ID";
            newID.name = ID;

            newIDHolder.hideFlags = HideFlags.HideInHierarchy;
            newID.hideFlags = HideFlags.HideInHierarchy;

            newID.transform.parent = newIDHolder.transform;

            newIDHolder.transform.parent = go.transform;
        }

        public static string AddIDToObject(GameObject go) {
            foreach (Transform child in go.transform) {
                if (child.name == "_ID") {
                    string id = child.GetChild(0).gameObject.name;
                    if (id != "" && id != "New Game Object") {
                        return id;
                    }
                }
            }

            GameObject newIDHolder = new GameObject();
            GameObject newID = new GameObject();
            newIDHolder.name = "_ID";
            newID.name = Guid.NewGuid().ToString();

            newIDHolder.hideFlags = HideFlags.HideInHierarchy;
            newID.hideFlags = HideFlags.HideInHierarchy;

            newID.transform.parent = newIDHolder.transform;

            newIDHolder.transform.parent = go.transform;
            return newID.name;
        }

        public static void AddIDToObject(Component go, string ID) {
            foreach (Transform child in go.transform) {
                if (child.name == "_ID" + go.GetType().FullName) {
                    child.GetChild(0).gameObject.name = ID;
                    return;
                }
            }

            GameObject newIDHolder = new GameObject();
            GameObject newID = new GameObject();
            newIDHolder.name = "_ID" + go.GetType().FullName;
            newID.name = ID;

            newIDHolder.hideFlags = HideFlags.HideInHierarchy;
            newID.hideFlags = HideFlags.HideInHierarchy;

            newID.transform.parent = newIDHolder.transform;

            newIDHolder.transform.parent = go.transform;
        }

        public static string AddIDToObject(Component go) {
            foreach (Transform child in go.transform) {
                if (child.name == "_ID" + go.GetType().FullName) {
                    string id = child.GetChild(0).gameObject.name;
                    if (id != "" && id != "New Game Object") {
                        return id;
                    }
                }
            }

            GameObject newIDHolder = new GameObject();
            GameObject newID = new GameObject();
            newIDHolder.name = "_ID" + go.GetType().FullName;
            newID.name = Guid.NewGuid().ToString();

            newIDHolder.hideFlags = HideFlags.HideInHierarchy;
            newID.hideFlags = HideFlags.HideInHierarchy;

            newID.transform.parent = newIDHolder.transform;

            newIDHolder.transform.parent = go.transform;
            return newID.name;
        }
    }
}