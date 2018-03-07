using UnityEngine;
using System.Collections;

public class Done_DestroyByContact : MonoBehaviour {
    public GameObject explosion;
    public GameObject playerExplosion;
    public int scoreValue;
    private Done_GameController gameController;

    private NetManager nm;

    void Start() {
        nm = GameObject.Find("NetManager").GetComponent<NetManager>();
        GameObject gameControllerObject = GameObject.FindGameObjectWithTag("GameController");
        if (gameControllerObject != null) {
            gameController = gameControllerObject.GetComponent<Done_GameController>();
        }

        if (gameController == null) {
            Debug.Log("Cannot find 'GameController' script");
        }
    }

    // 陨石碰撞
    void OnTriggerEnter(Collider other) {
        if (other.tag == "Boundary" || other.tag == "Enemy") {
            return;
        }

        if (explosion != null) {
            Instantiate(explosion, transform.position, transform.rotation);
        }

        if (other.tag == "Player" && other.GetComponent<Done_PlayerController>().server_plane_index == nm.plane_index) {
            Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
            gameController.GameOver();
        }

        gameController.AddScore(scoreValue);
//        Destroy(other.gameObject);
        Destroy(gameObject);
    }
}