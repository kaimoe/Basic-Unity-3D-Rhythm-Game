using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System.IO;
using Newtonsoft.Json;
using CosmicMovement;

public class SongManager : MonoBehaviour {

    private Song song;

    public Visualization spectrum;

    public Camera mainCamera;
    public Camera orthoCamera;
    public VideoPlayer videoPlayer;

    public float songPosition;
    public float posInBeats;
    public float secPerBeat;
    public float dsptimesong = 0;
    public float advanceBeats = 4;
    public float startDelay = 2f;

    private float fretBeat = 4f;

    private int noteIndex = 0;
    private int eventIndex = 0;

    public NoteSpawner spawner;
    private List<Note> notes;
    private List<SongEvent> events = new List<SongEvent>();
    private AudioSource source;
    private TerrainManager terrain;

    private bool paused = false;
    private float pauseDsp;
    private bool inited = false;
    private bool finished = false;

    // Use this for initialization
    void Start () {
        Time.timeScale = 1f;
        song = GameObject.Find("Game Container").GetComponent<Container>().Song;
        LoadNotes(song.ID);
        LoadEvents(song.ID);
        secPerBeat = 60f / song.BPM;

        terrain = GetComponent<TerrainManager>();

        source = GetComponent<AudioSource>();
        source.clip = Resources.Load<AudioClip>("Audio/Music/" + song.ID);
        videoPlayer.clip = Resources.Load<VideoClip>("Video/" + song.ID);
        videoPlayer.Prepare();
        videoPlayer.Play();//to sync up
        videoPlayer.Pause();

        if (song.Ortho) {
            mainCamera.enabled = false;
            orthoCamera.enabled = true;
            terrain.Kill();
        } else {
            spectrum.Init();
            terrain.SetTheme(song.Theme);
        }

        StartCoroutine(DelayedStart(startDelay));
	}

    IEnumerator DelayedStart(float delay) {
        yield return new WaitForSeconds(delay);
        source.Play();
        dsptimesong = (float)AudioSettings.dspTime;
        videoPlayer.Play();
        inited = true;
    }
	
	// Update is called once per frame
	void Update () {
        if (paused || !inited || finished)//don't update if paused, haven't started, or finished
            return;

        songPosition = (float)(AudioSettings.dspTime - dsptimesong);//current position in seconds...
        posInBeats = songPosition / secPerBeat;//... and in beats

        //check for pending note(s)
        while (noteIndex < notes.Count && notes[noteIndex].Beat < posInBeats + advanceBeats) {
            spawner.HandleNote(notes[noteIndex]);
            noteIndex++;
        }

        //check for fret
        if (fretBeat <= posInBeats + advanceBeats) {
            spawner.SpawnFret(fretBeat);
            fretBeat += 4f;
        }
        
        //check for event
        while (eventIndex < events.Count && events[eventIndex].Beat < posInBeats) {
            HandleEvent(events[eventIndex]);
            eventIndex++;
        }

        //check for end of song
        if (!source.isPlaying && !finished) {
            GetComponent<Scoring>().ShowScore();
            finished = true;
        }
    }

    private void OnPauseGame() {
        pauseDsp = (float)AudioSettings.dspTime;
        source.Pause();
        videoPlayer.Pause();
        paused = true;
    }

    private void OnResumeGame() {
        dsptimesong = dsptimesong + ((float)AudioSettings.dspTime - pauseDsp);
        source.UnPause();
        videoPlayer.Play();
        paused = false;
    }

    //load the note list
    private void LoadNotes(string id) {
        using (StreamReader r = new StreamReader(Application.dataPath + string.Format("/StreamingAssets/Notes/{0}.json", id))) {
            string json = r.ReadToEnd();
            notes = JsonConvert.DeserializeObject<List<Note>>(json);
            print(string.Format("Loaded {0} notes", notes.Count));
        }
    }

    //load the event list (if any)
    private void LoadEvents(string id) {
        string path = Application.dataPath + string.Format("/StreamingAssets/Events/{0}.json", id);
        if (File.Exists(path))
            using (StreamReader r = new StreamReader(path)) {
                string json = r.ReadToEnd();
                events = JsonConvert.DeserializeObject<List<SongEvent>>(json);
                print(string.Format("Loaded {0} events", events.Count));
            }
        else
            print("No event list found");
    }

    //handle song event accordingly
    private void HandleEvent(SongEvent ev) {
        switch (ev.Type) {
            case "Theme":
                terrain.SetTheme(ev.Parameter);
                break;
        }
    }
}
