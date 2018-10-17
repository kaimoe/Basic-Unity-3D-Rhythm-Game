using UnityEngine;

//NOT IN USE
public class Killbox : MonoBehaviour {

    private Scoring score;

    // Use this for initialization
    void Start () {
        score = GameObject.Find("Scripts").GetComponent<Scoring>();
    }

    // Update is called once per frame
    void Update () {
		
	}

    private void OnTriggerEnter(Collider other) {
        Destroy(other.gameObject);
        if (other.tag != "Field" && other.tag != "NoteTail") {
            print("killed note");
            score.TakeHit(tag);
        }
    }
}
