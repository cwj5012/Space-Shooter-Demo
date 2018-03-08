using System;
using UnityEngine;
using System.Collections;
using ProtoMsg;

[System.Serializable]
public class Done_Boundary {
    public float xMin, xMax, zMin, zMax;
}

public class Done_PlayerController : MonoBehaviour {
    public float speed;
    public float tilt;
    public Done_Boundary boundary;

    public GameObject shot;
    public Transform shotSpawn;
    public float fireRate;

    private float nextFire;

    // mod
    public GameObject[] players = new GameObject[2];

    float moveHorizontal = 0.0f;
    float moveVertical = 0.0f;
    public uint fire = 0;

    public uint plane_index;

    public uint server_plane_index;

    private NetManager nm;

    private bool last_move = false;
    private bool current = false;

    private UInt64 htime;
    private UInt64 vtime;

    private int hor_move = 0;
    private int ver_move = 0;
    private int hor_last_move = 0;
    private int ver_last_move = 0;
    private uint fire_ope = 0;
    private uint fire_last_ope = 0;
    private Int64 timestamp_ms = 0;
    private Int64 timestamp_ms_last = 0;
    private Int64 move_ms = 0;

    private float x = 0.0f;
    private float y = 0.0f;
    private float z = 0.0f;

    private float cur_speed = 0.0f;

    private static int plane_count = 0;

    void Start() {
        nm = GameObject.Find("NetManager").GetComponent<NetManager>();

        NetManager.plane_operate += PlaneOperate;
        NetManager.plane_destroy += PlaneDestroyFunc1;
        ++plane_count;

        speed = 6;

        PlaneOperateRequest msg1 = new PlaneOperateRequest();
        msg1.X = players[plane_index].transform.position.x;
        msg1.Y = players[plane_index].transform.position.y;
        msg1.Z = players[plane_index].transform.position.z;
        msg1.UserId = nm.user_id;
        msg1.IndexId = plane_index;
        Request msg2 = new Request();
        msg2.PlaneOperate = msg1;
        Message msg3 = new Message();
        msg3.Userid = nm.user_id;
        msg3.Request = msg2;
        msg3.MsgType = ProtoMsg.MSG.PlaneOperateRequest;
        nm.write_data_queue.Enqueue(ProtoMsg.Util.SerializeToByte(msg3));
    }

    void Update() {
        Debug.Log(nm.user_index[nm.user_id] + " " + plane_index);
        if (nm.user_index[nm.user_id] == plane_index) {
            // local operate
            hor_last_move = hor_move;
            ver_last_move = ver_move;
            fire_last_ope = fire_ope;

            hor_move = 0;
            hor_move += Input.GetKey(KeyCode.D) ? 1 : 0;
            hor_move += Input.GetKey(KeyCode.A) ? -1 : 0;

            ver_move = 0;
            ver_move += Input.GetKey(KeyCode.W) ? 1 : 0;
            ver_move += Input.GetKey(KeyCode.S) ? -1 : 0;

            fire_ope = 0;
            if (Input.GetButton("Fire1") && Time.time > nextFire) {
                fire_ope = 1;
                nextFire = Time.time + fireRate;
            }

            if (hor_last_move != hor_move || ver_last_move != ver_move || fire_last_ope != fire_ope) {
                // send to server
                PlaneOperateRequest msg1 = new PlaneOperateRequest();
                msg1.Horizontal = hor_move;
                msg1.Vertical = ver_move;
                msg1.X = players[plane_index].transform.position.x;
                msg1.Y = players[plane_index].transform.position.y;
                msg1.Z = players[plane_index].transform.position.z;

                msg1.UserId = nm.user_id;
                msg1.IndexId = plane_index;
                msg1.Fire = fire_ope;
                Request msg2 = new Request();
                msg2.PlaneOperate = msg1;
                Message msg3 = new Message();
                msg3.Userid = nm.user_id;
                msg3.Request = msg2;
                msg3.Sequence = ++nm.seq_id;
                msg3.TimestampMs = Utils.Time.GetTimeStampMs() + NetManager.timestamp_ms;
                msg3.MsgType = ProtoMsg.MSG.PlaneOperateRequest;

                nm.write_data_queue.Enqueue(ProtoMsg.Util.SerializeToByte(msg3));
                Debug.Log("send move");
            }
        }

        // get opeate from server
        while (nm.plane_operate_queue.Count > 0) {
            Byte[] bytes = (Byte[]) nm.plane_operate_queue.Dequeue();
            var msg = ProtoMsg.Util.ParseFromByte(bytes);

            moveHorizontal = msg.Response.PlaneOperate.Horizontal;
            moveVertical = msg.Response.PlaneOperate.Vertical;
            fire = msg.Response.PlaneOperate.Fire;
            server_plane_index = msg.Response.PlaneOperate.IndexId;
            timestamp_ms = msg.TimestampMs;
            x = msg.Response.PlaneOperate.X;
            y = msg.Response.PlaneOperate.Y;
            z = msg.Response.PlaneOperate.Z;
            break;
        }

        if (players[server_plane_index]) {
            if (timestamp_ms_last == 0) {
                timestamp_ms_last = timestamp_ms;
            }

            if (fire == 1) {
                fire = 0;
                if (shotSpawn) {
                    Instantiate(shot, players[server_plane_index].transform.position, shotSpawn.rotation);
                    players[server_plane_index].GetComponent<AudioSource>().Play();
                }
            }

            Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

            // 校准位置
            players[server_plane_index].transform.position =
                Vector3.MoveTowards(players[server_plane_index].transform.position, new Vector3(x, y, z),
                    1.0f * Time.deltaTime);

            // 移动
            players[server_plane_index].GetComponent<Rigidbody>().velocity = movement * speed;

            // players[server_plane_index].transform.Translate(movement * speed * Time.deltaTime * 1.0f);

            players[server_plane_index].GetComponent<Rigidbody>().position = new Vector3
            (
                Mathf.Clamp(players[server_plane_index].GetComponent<Rigidbody>().position.x,
                    boundary.xMin,
                    boundary.xMax),
                0.0f,
                Mathf.Clamp(players[server_plane_index].GetComponent<Rigidbody>().position.z,
                    boundary.zMin,
                    boundary.zMax)
            );

            players[server_plane_index].GetComponent<Rigidbody>().rotation =
                Quaternion.Euler(0.0f, 0.0f, players[server_plane_index].GetComponent<Rigidbody>().velocity.x * -tilt);
        }
    }

    void FixedUpdate() { }

    void LateUpdate() { }

    void OnDestroy() {
//        if (--plane_count == 0) {
        NetManager.plane_operate -= PlaneOperate;
        NetManager.plane_destroy -= PlaneDestroyFunc1;
//        }
    }

    private void PlaneOperate(ProtoMsg.Message msg) {
        // get opeate from server
        nm.plane_operate_queue.Enqueue(ProtoMsg.Util.SerializeToByte(msg));
    }

    public void PlaneDestroyFunc1(ProtoMsg.Message msg) {
        Debug.Log("飞机爆炸 " + server_plane_index);
        if (players[server_plane_index]) {
            Destroy(players[server_plane_index]);
        }
    }
}