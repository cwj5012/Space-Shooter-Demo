using System.Collections;
using System.Collections.Generic;
using ProtoMsg;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    private NetManager nm;

    public GameObject[] players = new GameObject[2];

    public Text plane_postion;

    void Awake() {
        Application.targetFrameRate = 60;

        nm = GameObject.Find("NetManager").GetComponent<NetManager>();

        if (nm.user_count == 1) {
            players[nm.plane_index].SetActive(true);
            players[nm.plane_index].GetComponent<Done_PlayerController>().plane_index = 0;
        }
        else {
            players[0].SetActive(true);
            players[0].GetComponent<Done_PlayerController>().plane_index = 0;
            players[1].SetActive(true);
            players[1].GetComponent<Done_PlayerController>().plane_index = 1;
        }
    }

    void Start() { }

    void OnDestroy() { }

    void Update() {
        string text0 = "plane 1\n" +
                       "x: " + 0 + "\n" +
                       "y: " + 0 + "\n" +
                       "z: " + 0 + "\n";

        if (players[0]) {
            text0 = "plane 1\n" +
                    "x: " + players[0].transform.position.x.ToString() + "\n" +
                    "y: " + 0 + "\n" +
                    "z: " + players[0].transform.position.z.ToString() + "\n";
        }

        string text1 = "plane 2\n" +
                       "x: " + 0 + "\n" +
                       "y: " + 0 + "\n" +
                       "z: " + 0 + "\n";
        if (players[1]) {
            text1 = "plane 2\n" +
                    "x: " + players[1].transform.position.x.ToString() + "\n" +
                    "y: " + 0 + "\n" +
                    "z: " + players[1].transform.position.z.ToString() + "\n";
        }

        plane_postion.text = text0 + text1;
    }
}