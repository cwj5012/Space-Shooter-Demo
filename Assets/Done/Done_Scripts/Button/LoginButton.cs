using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginButton : MonoBehaviour {
    private NetManager nm;

    void Awake() {

//#if DEVELOPMENT_BUILD
//      Debug.logger.logEnabled = true;
//#else
//      Debug.unityLogger.logEnabled = false;
//#endif

#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif

        Resolution[] reslution = Screen.resolutions;

        Int32 standard_width = 800;
        Int32 standard_height = 450;
        // Int32 standard_width = 480;
        // Int32 standard_height = 270;
        // Int32 standard_width = 960;
        // Int32 standard_height = 540;
        Screen.SetResolution(standard_width, standard_height, false);
    }

    void Start() {
        GetComponent<Button>().onClick.AddListener(() => { SceneManager.LoadScene("Login"); });
    }
}