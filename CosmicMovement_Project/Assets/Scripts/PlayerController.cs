using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float moveX = 3f;
    public float moveY = 4f;
    private bool paused = false;
    public bool easyMode;
    //private Vector3 defLoc;

    public CanvasGroup pauseScreen;

    public SongManager song;

    // Use this for initialization
    void Start () {
        //defLoc = transform.position;
        easyMode = GameObject.Find("Game Container").GetComponent<Container>().EasyMode;
    }

    // Update is called once per frame
    void Update () {
        if (!paused) {
            /*//Key down movement
            if (Input.GetAxisRaw("Horizontal") > 0 && transform.position.x == defLoc.x)
                transform.Translate(movementValue, 0, 0);
            if (Input.GetAxisRaw("Horizontal") < 0 && transform.position.x == defLoc.x)
                transform.Translate(-movementValue, 0, 0);
            if (Input.GetAxisRaw("Vertical") > 0 && transform.position.y == defLoc.y)
                transform.Translate(0, movementValue, 0);

            //Key up return
            if (Input.GetAxisRaw("Horizontal") == 0 && transform.position.x != defLoc.x)
                transform.position = new Vector3(defLoc.x, transform.position.y, transform.position.z);
            if (Input.GetAxisRaw("Vertical") == 0 && transform.position.y != defLoc.y)
                transform.Translate(0, -movementValue, 0);*/

            //Key down movement
            if (Input.GetKeyDown("d") || Input.GetKeyDown("right"))
                transform.Translate(moveX, 0, 0);
            if (Input.GetKeyDown("a") || Input.GetKeyDown("left"))
                transform.Translate(-moveX, 0, 0);
            if (Input.GetKeyDown("w") || Input.GetKeyDown("up"))
                transform.Translate(0, moveY, 0);

            //Key up return
            if (Input.GetKeyUp("d") || Input.GetKeyUp("right"))
                transform.Translate(-moveX, 0, 0);
            if (Input.GetKeyUp("a") || Input.GetKeyUp("left"))
                transform.Translate(moveX, 0, 0);
            if (Input.GetKeyUp("w") || Input.GetKeyUp("up"))
                transform.Translate(0, -moveY, 0);
        }

        if (Input.GetKeyDown("escape")) {
            TogglePause();
        }
    }

    //send puase message, and relevant actions
    public void TogglePause() {
        if (!paused) {
            Time.timeScale = 0;
            song.SendMessage("OnPauseGame");
            SetPauseScreen(true);
        } else {
            Time.timeScale = 1;
            song.SendMessage("OnResumeGame");
            SetPauseScreen(false);
        }
        paused = !paused;
    }

    private void SetPauseScreen(bool state) {
        pauseScreen.alpha = (state ? 1f : 0f);
        pauseScreen.blocksRaycasts = state;
    }

    public bool IsHitDown() {
        return Input.GetKeyDown("z") || Input.GetKeyDown("x") || Input.GetKeyDown(",") || Input.GetKeyDown(".");
    }

    public bool IsHitHeld() {
        return Input.GetKey("z") || Input.GetKey("x") || Input.GetKey(",") || Input.GetKey(".");
    }

    public void LoadMenu() {
        SceneManager.LoadScene("Scenes/Menu", LoadSceneMode.Single);
    }

    public void ReloadLevel() {
        SceneManager.LoadScene("Scenes/Level", LoadSceneMode.Single);
    }
}
