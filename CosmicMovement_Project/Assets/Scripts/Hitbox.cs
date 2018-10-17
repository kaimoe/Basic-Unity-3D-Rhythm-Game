using UnityEngine;

public class Hitbox : MonoBehaviour {

    private bool touching = false;
    public Scoring score;
    public PlayerController player;
    private bool easyMode;

	// Use this for initialization
	void Start () {
        easyMode = player.easyMode;
	}
	
	// Update is called once per frame
	void Update () {
        if (touching && ((tag == "Note" && easyMode) || player.IsHitDown())) {//check note hit (or easy mode)
            score.NoteHit(tag);
            Destroy(tag == "NoteTail" ? transform.parent.gameObject : gameObject);
        } else if (touching && tag == "NoteHold" && (easyMode || player.IsHitHeld())) {//check hold note (or easy mode)
            score.NoteHit(tag);
            Destroy(gameObject);
        }

    }

    private void OnTriggerEnter(Collider other) {
        switch (other.name) {
            case "Hitbox"://track player contact
                if (tag == "Field")
                    score.TakeHit(tag);
                else
                    toggleTouching();
                break;
            case "Killbox"://die
                if (tag != "Field" && tag != "NoteTail")
                    score.TakeHit(tag);
                Destroy(gameObject);
                break;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.name == "Hitbox")//track player contact
            toggleTouching();
    }

    private void toggleTouching() {
        touching = !touching;
    }
}
