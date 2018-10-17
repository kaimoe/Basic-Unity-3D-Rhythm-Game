using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scoring : MonoBehaviour {

    public int combo = 0;
    public int maxCombo = 0;
    public int score = 0;
    public int hit = 0;
    public int miss = 0;
    public int noteCount = 0;

    public ParticleSystem hitSpark;

    public Text scoreText;
    public Text missText;

    public Text comboText;
    private bool flashing = false;
    public float flashDelay = 0.05f;
    private Dictionary<string, Color[]> colormap = new Dictionary<string, Color[]>();

    public CanvasGroup scorePanel;
    public Text Grade;
    public Text Score;
    public Text Misses;
    public Text TopCombo;
    public Text MaxScore;
    public AudioSource Beep;

    private void Start() {
        colormap.Add("GREAT", new Color[] { Color.black, Color.red, Color.green, Color.blue, Color.white });
        colormap.Add("GOOD", new Color[] { Color.black, Color.yellow });
        colormap.Add("MISS", new Color[] { Color.black, Color.red });
    }

    public void NoteHit(string tag) {
        noteCount++;
        hitSpark.Play();
        int acc = (tag == "NoteTail" ? 1 : 2);//2x score, 1x if tail
        hit++;
        combo++;
        score += acc;
        scoreText.text = "Score: " + score;
        print("hit note");
        UpdateCombo(acc == 2 ? "GREAT" : "GOOD");
    }

    public void TakeHit(string tag) {
        if (tag == "Note" || tag == "Stomp") {
            noteCount++;
            miss++;
        }
        combo = 0;
        missText.text = "Miss: " + miss;
        UpdateCombo("MISS");
    }

    private void UpdateCombo(string acc) {
        StopFlash();//stop previous popup, if any
        if (maxCombo < combo)
            maxCombo = combo;
        comboText.text = acc + (combo == 0 ? "" : " " + combo);
        flashing = true;
        StopCoroutine(QueueStop());
        StartCoroutine(FlashText(acc));
    }

    IEnumerator FlashText(string acc) {
        yield return 0;//start one frame later, to clear properly if text is already up
        StartCoroutine(QueueStop());
        while (flashing) {
            foreach (Color color in colormap[acc]) {
                if (!flashing)
                    break;
                comboText.color = color;
                yield return new WaitForSeconds(flashDelay);
            }
        }
    }

    IEnumerator QueueStop() {
        yield return new WaitForSeconds(1f);
        StopFlash();
    }

    private void StopFlash() {
        flashing = false;
        comboText.text = "";
    }

    public void ShowScore() {
        scorePanel.alpha = 1f;
        scorePanel.blocksRaycasts = true;
        MaxScore.text = "Max Score: " + noteCount * 2;
        StartCoroutine(StartTick());
    }

    IEnumerator StartTick() {
        float delay = 0.3f;
        yield return StartCoroutine(TickUp(Score, "Score: ", score));
        yield return new WaitForSeconds(delay);
        yield return StartCoroutine(TickUp(Misses, "Misses: ", miss));
        yield return new WaitForSeconds(delay);
        yield return StartCoroutine(TickUp(TopCombo, "Top Combo: ", maxCombo));
        yield return new WaitForSeconds(1f);
        Grade.text = CalculateGrade();
        Beep.Play();
    }

    IEnumerator TickUp(Text t, string s, int num) {
        int i = 0;
        while (i != num) {
            i += 10;
            if (i > num)
                i = num;
            t.text = s + i;
            Beep.Play();
            yield return new WaitForSeconds(0.1f);
        }
    }

    public string CalculateGrade() {
        float accuracy = (float)score / (noteCount * 2);
        if (accuracy >= 0.95 && !GameObject.Find("Game Container").GetComponent<Container>().EasyMode)
            return "S";
        else if (accuracy >= 0.9)
            return "A";
        else if (accuracy >= 0.85)
            return "B";
        else if (accuracy >= 0.8)
            return "C";
        else if (accuracy >= 0.7)
            return "D";
        else if (accuracy >= 0.6)
            return "E";
        else
            return "F";
    }
}
