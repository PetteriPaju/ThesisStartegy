using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnPassedIndicator : MonoBehaviour {

    public UnityEngine.UI.Text text;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Show(string team, Color color)
    {
        text.text = team + "'s Turn";
        text.color = color;
        this.gameObject.SetActive(true);
    }

    public void End()
    {
        gameObject.SetActive(false);
    }
}
