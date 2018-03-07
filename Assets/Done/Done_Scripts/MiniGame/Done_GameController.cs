using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ProtoMsg;

public class Done_GameController : MonoBehaviour {
    public GameObject[] hazards;
    public Vector3 spawnValues;
    public int hazardCount;
    public float spawnWait;
    public float startWait;
    public float waveWait;

    public Text scoreText;
    public Text restartText;
    public Text gameOverText;

    private bool gameOver;
    private bool restart;
    private int score;

    // mod

    private NetManager nm;

    private int create = 0;
    private float x = 0;
    private float y = 0;
    private float z = 0;
    private float angel = 0;
    private uint type = 0;

    public Button exit_button;

    void Awake() {
        nm = GameObject.Find("NetManager").GetComponent<NetManager>();

        NetManager.wave_create += CreateWaveFunc;
        NetManager.exit_scene += ExitSceneFunc;
        NetManager.plane_destroy += PlaneDestroyFunc;

        exit_button.GetComponent<Button>().onClick.AddListener(() => {
            var msg1 = new ExitSceneRequest();
            var msg2 = new Request();
            msg2.ExitScene = msg1;
            var msg3 = new Message();
            msg3.Userid = nm.user_id;
            msg3.Request = msg2;
            msg3.MsgType = ProtoMsg.MSG.ExitSceneRequest;
            nm.write_data_queue.Enqueue(ProtoMsg.Util.SerializeToByte(msg3));
        });
    }

    void Start() {
        gameOver = false;
        restart = false;
        restartText.text = "";
        gameOverText.text = "";
        score = 0;
        UpdateScore();
//        StartCoroutine(SpawnWaves());
    }

    void Update() {
//        if (restart) {
//            if (Input.GetKeyDown(KeyCode.R)) {
//                Application.LoadLevel(Application.loadedLevel);
//            }
//        }
    }


//    IEnumerator SpawnWaves() {
//        yield return new WaitForSeconds(startWait);
//        while (true) {
//
//            yield return new WaitForSeconds(waveWait);
//
//            if (gameOver) {
//                restartText.text = "Press 'R' for Restart";
//                restart = true;
//                break;
//            }
//        }
//    }

    void OnDestroy() {
        NetManager.wave_create -= CreateWaveFunc;
        NetManager.exit_scene -= ExitSceneFunc;
        NetManager.plane_destroy -= PlaneDestroyFunc;
    }

    public void AddScore(int newScoreValue) {
        score += newScoreValue;
        UpdateScore();
    }

    void UpdateScore() {
        scoreText.text = "Score: " + score;
    }

    public void GameOver() {
        var msg1 = new PlaneDestroyRequest();
        msg1.IndexId = nm.user_index[nm.user_id];
        var msg2 = new Request();
        msg2.PlaneDestroy = msg1;
        var msg3 = new Message();
        msg3.Userid = nm.user_id;
        msg3.Request = msg2;
        msg3.MsgType = ProtoMsg.MSG.PlaneDestroyRequest;
        nm.write_data_queue.Enqueue(ProtoMsg.Util.SerializeToByte(msg3));
    }

    // 创建陨石
    public void CreateWaveFunc(ProtoMsg.Message msg) {
        x = msg.Response.WaveCreate.X;
        y = msg.Response.WaveCreate.Y;
        z = msg.Response.WaveCreate.Z;
        angel = msg.Response.WaveCreate.Angel;
        type = msg.Response.WaveCreate.Type;
        create = 1;

        if (create == 1) {
            create = 0;
            GameObject hazard = hazards[type];
            Vector3 spawnPosition = new Vector3(x, y, z);
            Quaternion spawnRotation = Quaternion.identity;
            GameObject go = Instantiate(hazard, spawnPosition, spawnRotation);
        }
    }

    // 返回主菜单
    public void ExitSceneFunc(ProtoMsg.Message msg) {
        SceneManager.LoadScene("MainMenu");
    }

    // 碰撞陨石
    public void PlaneDestroyFunc(ProtoMsg.Message msg) {
        gameOverText.text = "Game Over!";
        gameOver = true;
    }
}