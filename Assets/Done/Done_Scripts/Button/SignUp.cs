using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProtoMsg;
using UnityEngine;
using UnityEngine.UI;
using global::Google.Protobuf;

public class SignUp : MonoBehaviour {
    public InputField name;
    public InputField password;
    public InputField email;
    public InputField phone;
    public Text result;

    private NetManager nm;

    void Awake() {
        NetManager.register_result += ShowRegisterResult;
    }

    void Start() {
        GetComponent<Button>().onClick.AddListener(() => {
            var msg1 = new RegisterResquest();
            msg1.Name = name.text;
            msg1.Password = password.text;
            msg1.Email = email.text;
            msg1.Phone = phone.text;
            var msg2 = new Request();
            msg2.Register = msg1;
            var msg3 = new Message();
            msg3.Request = msg2;
            msg3.MsgType = MSG.RegisterRequest;
            MemoryStream stream = new MemoryStream();

            nm = GameObject.Find("NetManager").GetComponent<NetManager>();
            nm.write_data_queue.Enqueue(ProtoMsg.Util.SerializeToByte(msg3));
        });
    }

    void Update() {
    }

    public void ShowRegisterResult(ProtoMsg.Message msg) {
        if (msg.Response.Register.Result == 0) {
            result.text = "注册成功";
        }
        else {
            result.text = "注册失败";
        }
    }

    void OnDestroy() {
        NetManager.register_result -= ShowRegisterResult;
    }
}