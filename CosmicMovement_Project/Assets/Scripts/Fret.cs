using UnityEngine;

public class Fret : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other) {
        if (other.name == "Killbox")
            Destroy(gameObject);
    }
}
