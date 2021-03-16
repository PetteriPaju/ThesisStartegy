using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPbar : MonoBehaviour {

    private UnityEngine.UI.Image image;
    public Gradient colors = new Gradient();

    public static List<HPbar> allInstances = new List<HPbar>();


    void OnEnable()
    {
        image = GetComponent<UnityEngine.UI.Image>();
    }

    public void UpdateBar(float percentage)
    {
        image = GetComponent<UnityEngine.UI.Image>();
        if (image)
        {
            image.fillAmount = percentage;
            image.color = colors.Evaluate(1 * percentage);
        }

    }

    public static void DestroyAllInstances()
    {
        foreach(HPbar bar in allInstances)
        {
            if (bar.gameObject)
            GameObject.Destroy(bar.gameObject);
        }

        allInstances.Clear();
    }

}
