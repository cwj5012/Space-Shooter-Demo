using System.IO;
using System.Text;
using ProtoMsg;
using UnityEngine;
using UnityEngine.UI;
using global::Google.Protobuf;
using UnityEngine.SceneManagement;

public class MatchBegin : MonoBehaviour {
    public Text match_button;
    public Text match_info;
    public Dropdown player_count;

    public uint math_player_count = 0;
    private bool is_match = false;
    private NetManager nm;

    void Awake() {
        NetManager.match_complete += MatchGameSuccess;
        NetManager.time_sync += TimeSyncFunc;
    }

    void Start() {
        player_count.onValueChanged.AddListener(x => { math_player_count = (uint) x; });

        // 校准客户端时间
        DoTimeSync();

        GetComponent<Button>().onClick.AddListener(() => {
            nm = GameObject.Find("NetManager").GetComponent<NetManager>();

            if (!is_match) {
                var msg1 = new MatchBeginRequest();
                msg1.Id = math_player_count;
                var msg2 = new Request();
                msg2.MatchBegin = msg1;
                var msg3 = new Message();
                msg3.Userid = nm.user_id;
                msg3.Request = msg2;
                msg3.MsgType = MSG.MatchBeginRequest;

                nm.write_data_queue.Enqueue(ProtoMsg.Util.SerializeToByte(msg3));
            }
            else {
                var msg1 = new MatchCancelRequest();
                msg1.Id = math_player_count;
                var msg2 = new Request();
                msg2.MatchCancel = msg1;
                var msg3 = new Message();
                msg3.Userid = nm.user_id;
                msg3.Request = msg2;
                msg3.MsgType = MSG.MatchCancelRequest;

                nm.write_data_queue.Enqueue(ProtoMsg.Util.SerializeToByte(msg3));
            }

            is_match = !is_match;

            if (is_match) {
                match_button.text = "取消匹配";
                match_info.text = "正在匹配......";
            }
            else {
                match_button.text = "开始匹配";
                match_info.text = "";
            }
        });
    }

    void Update() {
    }

    void OnDestroy() {
        NetManager.match_complete -= MatchGameSuccess;
        NetManager.time_sync -= TimeSyncFunc;
    }

    public void MatchGameSuccess(ProtoMsg.Message msg) {
        nm.plane_index = msg.Response.MatchComplete.Id;
        nm.user_count = msg.Response.MatchComplete.UserCount;
        nm.user_index[nm.user_id] = msg.Response.MatchComplete.Id;
        SceneManager.LoadScene("Done_Main");
    }

    private void TimeSyncFunc(ProtoMsg.Message msg) {
        // 往返时间差
        long delay = Utils.Time.GetTimeStampMs() - NetManager.timestamp_ms;
        long diff = msg.TimestampMs - NetManager.timestamp_ms;
        Debug.Log("网络延迟 = " + delay);
        Debug.Log("服务端时间 - 客户端时间 = " + diff);
        Debug.Log("客户端时间校准 = " + (diff + delay / 2));
    }

    private void DoTimeSync() {
        nm = GameObject.Find("NetManager").GetComponent<NetManager>();
        NetManager.timestamp_ms = Utils.Time.GetTimeStampMs();
        TimeSyncRequest msg1 = new TimeSyncRequest();
        Request msg2 = new Request();
        msg2.TimeSync = msg1;
        Message msg3 = new Message();
        msg3.Userid = nm.user_id;
        msg3.TimestampMs = NetManager.timestamp_ms;
        msg3.Request = msg2;
        msg3.MsgType = ProtoMsg.MSG.TimeSyncRequest;
        nm.write_data_queue.Enqueue(ProtoMsg.Util.SerializeToByte(msg3));
    }
}