using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using ProtoMsg;

public class NetManager : MonoBehaviour {
    public uint seq_id = 0;
    public uint user_id = 0;
    public uint plane_index = 99;
    public uint user_count = 99;
    public Dictionary<uint, uint> user_index = new Dictionary<uint, uint>();

    public Queue read_data_queue = new Queue();
    public Queue write_data_queue = new Queue();
    public Queue plane_operate_queue = new Queue();

    public Dictionary<uint, GameObject> cubes = new Dictionary<uint, GameObject>();

    private Thread read_data_thread;
    private Thread write_data_thread;
    private NetworkStream stream;
    private TcpClient client;

    public delegate void LoginResult(ProtoMsg.Message msg);

    public static LoginResult login_result;

    public delegate void RegisterResult(ProtoMsg.Message msg);

    public static RegisterResult register_result;

    public delegate void CubeOperate(ProtoMsg.Message msg);

    public static CubeOperate cube_operate;

    public delegate void CubeCreate(ProtoMsg.Message msg);

    public static CubeCreate cube_create;

    // 匹配成功
    public delegate void MatchComplete(ProtoMsg.Message msg);

    public static MatchComplete match_complete;

    // 飞机操作
    public delegate void PlaneOperate(ProtoMsg.Message msg);

    public static PlaneOperate plane_operate;

    public static int plane_operate_count;

    // 陨石创建
    public delegate void WaveCreate(ProtoMsg.Message msg);

    public static WaveCreate wave_create;

    // 离开场景
    public delegate void ExitScene(ProtoMsg.Message msg);

    public static ExitScene exit_scene;

    // 时间同步
    public delegate void TimeSync(ProtoMsg.Message msg);

    public static TimeSync time_sync;

    // 飞机爆炸
    public delegate void PlaneDestroy(ProtoMsg.Message msg);

    public static PlaneDestroy plane_destroy;

    // 
    public static Int64 timestamp_ms;

    void ReadDataThread() {
        Byte[] read_data = new Byte[1024];
        Int32 bytes = 0;
        while (true) {
            // 读取服务端发送过来的消息
            bytes = stream.Read(read_data, 0, 1024);
            // 截取消息长度
            for (int i = 0; i < bytes;) {
                Int32 message_length = BitConverter.ToInt32(read_data.Skip(i).Take(4).ToArray(), 0);
                byte[] newA = read_data.Skip(i + 4).Take(message_length - 4).ToArray(); // 截取实际取到的长度
                i = i + message_length;
                read_data_queue.Enqueue(newA);
            }

            // Thread.Sleep(50);
        }
    }

    void WriteDataThread() {
        while (true) {
            while (write_data_queue.Count > 0) {
                // 从队列中取出消息
                Byte[] bytes = (Byte[]) write_data_queue.Dequeue();

                // 新建一个数组，长度 = 包长度4字节 + 消息内容长度
                Byte[] newB = new Byte[4 + bytes.Length];
                Byte[] newC = new Byte[4];
                newC = BitConverter.GetBytes(4 + bytes.Length);

                // 拼接包长度 + 消息内容
                newC.CopyTo(newB, 0);
                bytes.CopyTo(newB, 4);

                // 发送消息
                stream.Write(newB, 0, 4 + bytes.Length);
            }

            //Thread.Sleep(50);
        }
    }

    // Use this for initialization
    void Start() {
        DontDestroyOnLoad(gameObject);

        Int32 port = 7000;
        // string server = "111.231.98.252";
        // string server = "192.168.48.194";
        string server = "127.0.0.1";
        client = new TcpClient(server, port);
        stream = client.GetStream();

        read_data_thread = new Thread(new ThreadStart(ReadDataThread));
        read_data_thread.Start();
        write_data_thread = new Thread(new ThreadStart(WriteDataThread));
        write_data_thread.Start();

        plane_operate += DoNothing;
    }

    // Update is called once per frame
    void Update() {
        while (read_data_queue.Count > 0) {
            Byte[] bytes = (Byte[]) read_data_queue.Dequeue();
            DealMessage(bytes); // deal all message from server
        }
    }

    void OnDestroy() {
        read_data_thread.Abort();
        write_data_thread.Abort();
        stream.Close();
        client.Close();
        Debug.Log("client close");
    }

    private void DoNothing(ProtoMsg.Message msg) { }

    void DealMessage(Byte[] bytes) {
        var msg = ProtoMsg.Util.ParseFromByte(bytes);
        switch (msg.MsgType) {
            case ProtoMsg.MSG.LoginResponse:
                login_result(msg);
                break;
            case ProtoMsg.MSG.RegisterResponse:
                register_result(msg);
                break;
            case ProtoMsg.MSG.MatchCompleteResponse:
                match_complete(msg);
                break;
            case ProtoMsg.MSG.PlaneOperateResponse:
                plane_operate(msg);
                break;
            case ProtoMsg.MSG.WaveCreateResponse:
                wave_create(msg);
                break;
            case ProtoMsg.MSG.ExitSceneResponse:
                exit_scene(msg);
                break;
            case ProtoMsg.MSG.TimeSyncResponse:
                time_sync(msg);
                break;
            case ProtoMsg.MSG.PlaneDestroyResponse:
                plane_destroy(msg);
                break;
            default:
                break;
        }
    }
}