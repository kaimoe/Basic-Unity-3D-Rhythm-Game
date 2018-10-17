using UnityEngine;

public class RandomAnim : MonoBehaviour {

    private Animator animator;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        animator.Play(Random.Range(0, 4).ToString());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
