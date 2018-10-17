using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour {

    public Sprite[] sprites;
    public Image image;

    public MenuManager menu;
    private CanvasGroup group;

    private int i = 0;

    //Use this for initialization
    void Start() {
        group = GetComponent<CanvasGroup>();
    }

    public void Next() {
        if (++i == sprites.Length) {
            menu.ToggleCanvas(group);
            i = 0;
        }
        image.sprite = sprites[i];
    }

    public void Previous() {
        if (--i < 0) {
            menu.ToggleCanvas(group);
            i = 0;
        }
        image.sprite = sprites[i];
    }
}
