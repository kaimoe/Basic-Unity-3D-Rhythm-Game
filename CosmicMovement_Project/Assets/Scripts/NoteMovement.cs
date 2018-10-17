using UnityEngine;

public class NoteMovement : MonoBehaviour {

    private Vector3 startPos;
    private Vector3 targetPos;

    public SongManager song;
    public Transform target;
    public Transform killbox;

    private float songPos;
    private float advBeats;
    private float targetBeat;
    private float killboxDistance;
    private bool targetReached = false;

    private float noteBeat;
    private float targetOffset;


    public void Init(float b, float t = 0) {
        noteBeat = b;
        targetOffset = t;
    }

    // Use this for initialization
    void Start () {
        startPos = transform.position;
        targetPos = new Vector3(transform.position.x, transform.position.y, target.position.z + targetOffset);
        advBeats = song.advanceBeats;
        targetBeat = noteBeat;
        killboxDistance = targetPos.z - startPos.z;//used when target reached
    }
	
	// Update is called once per frame
	void Update () {
        //continue movement after target reached (until the killbox)
        if (transform.position.z == targetPos.z) {
            targetReached = true;
            startPos = targetPos;//start at target
            targetPos = new Vector3(targetPos.x, targetPos.y, killbox.position.z);//end at killbox
            killboxDistance = killboxDistance / (targetPos.z - startPos.z);
            advBeats = advBeats / killboxDistance;
            targetBeat = noteBeat*2 + advBeats;
        }

        songPos = song.posInBeats + (targetReached ? noteBeat : 0);

        //interpolate note position based on beat and song position
        transform.position = Vector3.Lerp(
            startPos,
            targetPos,
            (advBeats - (targetBeat - songPos)) / advBeats);

    }
}
