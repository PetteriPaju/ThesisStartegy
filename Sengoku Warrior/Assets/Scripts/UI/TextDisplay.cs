using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextDisplay : MonoBehaviour
{

    public UnityEngine.UI.Text text;
    private bool allowInput = false;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown && allowInput)
        {
            End();
            allowInput = false;
        }
    }

    public void Show(string Text)
    {
        iTween.ScaleFrom(gameObject, iTween.Hash("y", 0, "time", 1f));
        allowInput = false;
        text.text = Text;
        this.gameObject.SetActive(true);
        StartCoroutine(ShowDelay());
    }


    private IEnumerator ShowDelay()
    {
        yield return new WaitForSeconds(2);
        allowInput = true;

    }

    public void End()
    {
      
        iTween.ScaleTo(gameObject, iTween.Hash("y", 0, "time", 1f,"oncomplete","Hide"));
      
    }
    public void Hide()
    {

        gameObject.SetActive(false);
    }
    }
