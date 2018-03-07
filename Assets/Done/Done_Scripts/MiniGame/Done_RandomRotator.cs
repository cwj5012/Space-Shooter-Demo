using UnityEngine;
using System.Collections;

public class Done_RandomRotator : MonoBehaviour {
    public float tumble;

    void Start() {
        // 随机方向旋转
        // GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere * tumble;
        // 固定方向旋转
        GetComponent<Rigidbody>().angularVelocity = (new Vector3(1, 1, 1)) * tumble;
    }
}