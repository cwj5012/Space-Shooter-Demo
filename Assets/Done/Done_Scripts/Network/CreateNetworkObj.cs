using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateNetworkObj : MonoBehaviour {
    public static int count = 0;

    // Use this for initialization
    void Start() {
        // Debug.unityLogger.logEnabled = false;

        if (count > 0) {
            return;
        }

        GameObject go = new GameObject("NetManager");
        DontDestroyOnLoad(go);
        go.AddComponent<NetManager>();
        ++count;
    }

    // Update is called once per frame
    void Update() {
    }
}