using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIImageFlicker : MonoBehaviour {

    CanvasGroup canvasGroup;

	// Use this for initialization
    void Awake()
    {
    }
	void Start () {



        canvasGroup = GetComponent<CanvasGroup>();
        if (!canvasGroup) Component.Destroy(this);
        else
        iTween.ValueTo(this.gameObject, iTween.Hash("target", gameObject, "from", canvasGroup.alpha, "to", 0, "time", 1f, "looptype", "pingpong", "easetype", "easeInQuad", "onupdatetarget", gameObject, "onupdate", "up"));


    }
    void up(float val)
    {
        if (canvasGroup != null)
        canvasGroup.alpha = val;
    }
    // Update is called once per frame
    void Update () {
		
	}


    void OnDestroy()
    {
        iTween.Stop(gameObject);
        if (canvasGroup) canvasGroup.alpha = 1;
    }
}
