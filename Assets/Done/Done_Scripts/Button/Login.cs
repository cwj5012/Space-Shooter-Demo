using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProtoMsg;
using UnityEngine;
using UnityEngine.UI;
using global::Google.Protobuf;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour {
    public InputField name;
    public InputField password;
    public Text result;

    private NetManager nm;

    void Awake() {
        NetManager.login_result += ShowResult;
    }

    void Start() {
        GetComponent<Button>().onClick.AddListener(() => {
            var msg1 = new LoginRequest();
            msg1.Name = name.text;
            msg1.Password = password.text;
            var msg2 = new Request();
            msg2.Login = msg1;
            var msg3 = new Message();
            msg3.Userid = 30;
            msg3.Sequence = 10;
            msg3.Request = msg2;
            msg3.MsgType = MSG.LoginRequest;

            nm = GameObject.Find("NetManager").GetComponent<NetManager>();
            nm.write_data_queue.Enqueue(ProtoMsg.Util.SerializeToByte(msg3));
        });
    }

    public void ShowResult(ProtoMsg.Message msg) {
        if (msg.Response.Login.Result == 0) {
            nm.user_id = msg.Userid;
            result.text = "登录成功";
            SceneManager.LoadScene("MainMenu");
        }
        else if (msg.Response.Login.Result == 2) {
            result.text = "账号重复登录";
        }
        else if (msg.Response.Login.Result == 1) {
            result.text = "用户名或者密码错误";
        }
        else {
            result.text = "登录失败";
        }
    }

    void OnDestroy() {
        NetManager.login_result -= ShowResult;
    }
}