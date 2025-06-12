using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene("PlayerUI", LoadSceneMode.Additive);
    }

}
