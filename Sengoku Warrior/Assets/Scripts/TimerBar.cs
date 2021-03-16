using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerBar : MonoBehaviour {

    public static TimerBar ShownAbar = null;

    public UnityEngine.UI.Image FillImage;

    void Start()
    {
        Hide();
    }

    public void SetFill(float percentage)
    {
        FillImage.fillAmount = 1 * percentage;
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }
    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

}
