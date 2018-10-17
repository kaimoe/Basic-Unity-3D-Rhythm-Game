using UnityEngine;
using UnityEngine.SceneManagement;
using CosmicMovement;

public class Container : MonoBehaviour {

    public Song Song { get; set; }
    public bool EasyMode = false;

    private void Start() {
        DontDestroyOnLoad(this.gameObject);
    }

    public void LoadSong(Song s) {
        Song = s;
        SceneManager.LoadScene("Scenes/Level");
    }
}
