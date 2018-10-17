using System.Collections.Generic;
using UnityEngine;
using CosmicMovement;

public class NoteSpawner : MonoBehaviour {
    public GameObject note;
    public GameObject holdNote;
    public GameObject field;
    public GameObject stomp;
    public GameObject fret;

    public PlayerController player;

    public SongManager song;

    private Dictionary<char, float> pos = new Dictionary<char, float>();
    private float holdScale;

    private List<Note> holdQueue = new List<Note>();

    private Container container;

    
    // Use this for initialization
    void Start () {

        container = GameObject.Find("Game Container").GetComponent<Container>();

        //scale hold notes on z, to make them long enough for the entire trail to be connected
        //this is done based on spawner distance from target, and beats shown in advance (distance and speed)
        //BPM is also technically a factor, but the change in speed is miniscule enough where it can be ignored
        holdScale = (2.15f * ((transform.position.z - GameObject.Find("Target").transform.position.z) / 34f)) * (4f / song.advanceBeats);

        //x offsets of notes
        pos.Add('c', 0);
        pos.Add('r', player.moveX);
        pos.Add('l', -player.moveX);
    }

    // Update is called once per frame
    void Update () {
        //check for active hold notes
        if (holdQueue.Count > 0)
            foreach (Note n in holdQueue.ToArray()) {
                if (n.End <= song.posInBeats + song.advanceBeats) {//note expired
                    holdQueue.Remove(n);
                    continue;
                }
                if (n.Beat < song.posInBeats + song.advanceBeats) {//note valid
                    SpawnHold(n);
                    Note incBeat = n;
                    incBeat.Beat += 0.25f;//spawn object every 16th note
                    holdQueue.Remove(n);
                    holdQueue.Add(incBeat);
                }
            }
	}

    public void HandleNote(Note n) {
        if (n.Type == "stomp") {//stomp
            if (container.EasyMode)
                return;
            SpawnStomp(n);
        } else if (n.Duration == 0) //single
            SpawnNote(n);
        else //hold
            holdQueue.Add(n);
    }

    //spawn fret (guide appearing at first beat of every bar)
    public void SpawnFret(float beat) {
        NoteInst(fret, new Vector3(transform.position.x, -0.3f, transform.position.z), Quaternion.Euler(45, 0, 0), beat);
    }

    //spawn single note
    private void SpawnNote(Note n) {
        Vector3 loc = transform.position + new Vector3(pos[n.Pos], 0f + (n.Up ? player.moveY : 0), 0f);
        NoteInst(n.Type == "note" ? note : field, loc, transform.rotation, n.Beat);
    }

    //spawn held note
    private void SpawnHold(Note n) {
        Vector3 loc = transform.position + new Vector3(pos[n.Pos], 0f + (n.Up ? player.moveY : 0), holdScale/2);
        GameObject newNote = NoteInst(n.Type == "note" ? holdNote : field, loc, transform.rotation, n.Beat, holdScale / 2);
        //scale on z
        newNote.transform.localScale = new Vector3(newNote.transform.lossyScale.x, newNote.transform.lossyScale.y, holdScale);
    }

    //spawn stomp note
    private void SpawnStomp(Note n) {
        Vector3 loc = transform.position + new Vector3(0, -1.6f, 0);
        NoteInst(stomp, loc, stomp.transform.rotation, n.Beat);
    }

    //instantiate and init note
    private GameObject NoteInst(GameObject obj, Vector3 pos, Quaternion rot, float beat, float offset = 0f) {
        GameObject newNote =  Instantiate(obj, pos, rot);
        newNote.SetActive(true);
        newNote.GetComponent<NoteMovement>().Init(beat, offset);
        return newNote;
    }
}