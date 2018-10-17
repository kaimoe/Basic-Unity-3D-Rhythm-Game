using System.Collections.Generic;
using UnityEngine;

public class Visualization : MonoBehaviour {

    private bool inited = false;
    private const int SAMPLE_SIZE = 1024;
    public Material materialLight;
    public Material materialDark;
    private Dictionary<string, Material> mats = new Dictionary<string, Material>();

    public float rmsValue;
    public float dbValue;
    public float pitchValue;

    public float visualModifier;
    public float smoothSpeed;
    public float keepPercentage;
    public float maxVisualScale;

    private AudioSource source;
    private float[] samples;
    private float[] spectrum;

    private Transform[] visualList;
    private Transform[] visualList2;
    private float[] visualScale;
    public int visualNum = 24;

	// Use this for initialization
	public void Init () {
        source = GetComponent<AudioSource>();
        samples = new float[SAMPLE_SIZE];
        spectrum = new float[SAMPLE_SIZE];

        visualScale = new float[visualNum];

        visualList = SpawnLine(1);//right line
        visualList2 = SpawnLine(-1);//left line

        mats.Add("light", materialLight);
        mats.Add("dark", materialDark);
        inited = true;
    }
	
	// Update is called once per frame
	void Update () {
        if (inited) {
            AnalyzeSound();
            UpdateVisual();
        }
    }

    //spawn line of cubes that serves as spectrum
    private Transform[] SpawnLine(int x) {
        Transform[] list = new Transform[visualNum];

        Vector3 startPos = new Vector3(7.9f * x, -1.7f, -9.5f);

        for (int i = 0; i < visualNum; i++) {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
            go.name = "Spectrum";
            list[i] = go.transform;
            list[i].position = startPos + (Vector3.forward * i);
            list[i].rotation = Quaternion.Euler(0, 0, 30 * -x);
        }

        return list;
    }

    //get rms, db, and spectrum data from audio source
    private void AnalyzeSound() {
        source.GetOutputData(samples, 0);

        //get rms
        float sum = 0;
        for (int i = 0; i < SAMPLE_SIZE; i++) {
            sum += samples[i] * samples[i];
        }
        rmsValue = Mathf.Sqrt(sum / SAMPLE_SIZE);

        //get db
        dbValue = 20 * Mathf.Log10(rmsValue / 0.1f);

        //get spectrum
        source.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
    }

    //set scale of each spectrum cube based on db
    private void UpdateVisual() {
        int visualIndex = 1;//intentionally skip the first (0) bar, as it's often capped at the ceiling level
        int spectrumIndex = 1;
        int averageSize = (int)((SAMPLE_SIZE * keepPercentage) / visualNum);

        while (visualIndex < visualNum) {
            int j = 0;
            float sum = 0;
            while (j < averageSize) {
                sum += spectrum[spectrumIndex];
                spectrumIndex++;
                j++;
            }

            float scaleY = sum / averageSize * visualModifier;//scale cubes on Y based on db
            visualScale[visualIndex] -= Time.deltaTime * smoothSpeed;
            if (visualScale[visualIndex] < scaleY)
                visualScale[visualIndex] = scaleY;

            if (visualScale[visualIndex] > maxVisualScale)//enforce volume ceiling
                visualScale[visualIndex] = maxVisualScale;

            Vector3 scale = Vector3.one + Vector3.up * visualScale[visualIndex];

            visualList[visualIndex].localScale = scale;
            visualList2[visualIndex].localScale = scale;
            visualIndex++;
        }
    }

    //change spectrum material per theme
    public void SetTheme(string theme) {
        foreach (Transform t in visualList)
            t.GetComponent<Renderer>().material = mats[theme];
        foreach (Transform t in visualList2)
            t.GetComponent<Renderer>().material = mats[theme];
    }
}
