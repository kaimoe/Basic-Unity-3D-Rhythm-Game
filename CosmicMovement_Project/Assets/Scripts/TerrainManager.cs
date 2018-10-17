using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour {

    public GameObject[] Terrain;
    public Light terrainLight;

    public Material lightMaterial;
    public Material darkMaterial;

    private Dictionary<string, Material> mats = new Dictionary<string, Material>();
    private Dictionary<string, Color> lights = new Dictionary<string, Color>();
    private bool paused = false;

    private void Awake() {
        mats.Add("light", lightMaterial);
        mats.Add("dark", darkMaterial);
        lights.Add("light", new Color(0f, 0.9647f, 1f));
        lights.Add("dark", new Color(0.6745f, 0f, 1f));
    }

    public void Kill() {
        foreach (GameObject go in Terrain)
            Destroy(go);
        paused = true;
    }

    // Update is called once per frame
    void Update () {
        if (paused)
            return;

        foreach (GameObject t in Terrain) {//terrain infinite scrolling
            if (t.transform.position.z > -500)
                t.transform.Translate(Vector3.back * 7);
            else
                t.transform.Translate(Vector3.forward * 1900);
        }
    }

    private void OnPauseGame() {
        paused = true;
    }

    private void OnResumeGame() {
        paused = false;
    }

    public void SetTheme(string theme) {
        foreach (GameObject t in Terrain)
            foreach (Renderer r in t.GetComponentsInChildren<Renderer>())
                r.material = mats[theme];

        terrainLight.color = lights[theme];
        GetComponent<Visualization>().SetTheme(theme);
    }
}
