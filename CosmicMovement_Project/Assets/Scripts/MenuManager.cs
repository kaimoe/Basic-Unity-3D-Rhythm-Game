using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;
using CosmicMovement;

public class MenuManager : MonoBehaviour {
    private int selInt = 0;
    private int prevInt = -1;
    private string[] songIDs;

    private List<Song> songs;
    private bool loaded = false;

    private AudioSource source;
    public Image thumbnail;

    public Text title;
    public Text artist;
    public Text bpmlength;
    public Text description;
    public Text difficulty;
    public Image easyModeButton;
    public Text easyModeText;

    private Color green = new Color(0.3776166f, 1f, 0f, 0.65f);

    public GameObject containerPrefab;
    private GameObject containerObj;
    private Container container;

    public CanvasGroup mainMenu;
    public CanvasGroup songSelect;

    // Use this for initialization
    void Start () {
        LoadSongList();
        songIDs = SongTitles(songs);

        source = GetComponent<AudioSource>();

        containerObj = GameObject.Find("Game Container");
        if (containerObj == null) {//spawn container if nonexistant
            containerObj = Instantiate(containerPrefab);
            containerObj.name = "Game Container";
            container = containerObj.GetComponent<Container>();
        } else {//change selection to previous song
            container = containerObj.GetComponent<Container>();
            SongSelect(false);
            selInt = songs.IndexOf(containerObj.GetComponent<Container>().Song);
            UpdateEasyMode();
        }

        loaded = true;
    }

    void OnGUI() {
        if (!loaded)
            return;

        if (songSelect.blocksRaycasts) {
            GUILayout.BeginVertical("Box");//spawn buttons for songs
            selInt = GUI.SelectionGrid(new Rect(200, 150, 400, 300), selInt, songIDs, 1);


            GUILayout.EndVertical();

            CheckSelection(selInt);
        }
    }

    public void SongSelect(bool backingOut) {
        ToggleCanvas(mainMenu);
        ToggleCanvas(songSelect);
        if (backingOut)
            source.Stop();
    }

    public void ToggleCanvas(CanvasGroup group) {
        group.alpha = (!group.blocksRaycasts ? 1f : 0f);
        group.blocksRaycasts = !group.blocksRaycasts;
    }

    public void SwitchEasyMode() {
        container.EasyMode = !container.EasyMode;
        UpdateEasyMode();
    }

    private void UpdateEasyMode() {
        easyModeText.text = "Easy Mode: " + (container.EasyMode ? "On" : "Off");
        easyModeButton.color = container.EasyMode ? green : Color.white;
    }

    //start chosen song
    public void StartClicked() {
        container.LoadSong(FetchSong(songIDs[selInt]));
    }

    //load the song list
    private void LoadSongList() {
        using (StreamReader r = new StreamReader(Application.dataPath + "/StreamingAssets/songs.json")) {
            string json = r.ReadToEnd();
            var s = JsonConvert.DeserializeObject<List<Song>>(json);
            print(string.Format("Loaded {0} songs", s.Count));
            songs = s;
        }
    }

    //produce song title array for buttons
    private string[] SongTitles(List<Song> songs) {
        List<string> ids = new List<string>();
        foreach (Song s in songs) {
            ids.Add(s.Title);
        }

        return ids.ToArray();
    }

    //get song from title
    private Song FetchSong(string title) {
        foreach (Song s in songs) {
            if (s.Title == title)
                return s;
        }
        return new Song();
    }
    
    //check for selection change
    private void CheckSelection(int sel) {
        if (sel == prevInt)
            return;
        prevInt = sel;
        Song song = FetchSong(songIDs[selInt]);
        print("Selection switched to " + song.ID);

        //change song
        source.clip = Resources.Load<AudioClip>("Audio/Preview/" + song.ID);
        source.Play();

        //change thumbnail
        thumbnail.sprite = Resources.Load<Sprite>("Images/Thumbnails/" + song.ID);

        //change info text
        title.text = song.Title;
        artist.text = song.Artist;
        bpmlength.text = song.BPM + "BPM – " + song.Length;
        description.text = song.Description;
        string diff = "";
        for (int i = 0; i < song.Difficulty; i++)
            diff += "★";
        for (int i = 10; i > song.Difficulty; i--)
            diff += "☆";
        difficulty.text = diff;
    }

    public void Quit() {
        Application.Quit();
    }
}
