using UnityEngine;

public class Stomp : MonoBehaviour {

    private bool touching = false;
    public GameObject Scripts;
    private Scoring score;

    // Use this for initialization
    void Start() {
        score = Scripts.GetComponent<Scoring>();
    }

    // Update is called once per frame
    void Update() {
        if (touching && Input.GetKeyDown("space")) {//check stomp hit
            score.NoteHit(tag);
            Destroy(gameObject);
            toggleTouching();
        }
    }

    private void OnTriggerEnter(Collider other) {
        switch (other.name) {
            case "Stomper"://monitor touching
                toggleTouching();
                break;
            case "Killbox"://die
                score.TakeHit(tag);
                Destroy(gameObject);
                break;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.name == "Stomper") {//monitor touching
            toggleTouching();
        }
    }

    private void toggleTouching() {
        touching = !touching;
    }
}
