using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class AutoWriteText : Text {

    private IEnumerator coroutine;
    public void Clear()
    {
        text = string.Empty;
    }

    public void Type (string input)
    {
        Clear();
        if (coroutine != null)
        StopCoroutine(coroutine);
        coroutine = TypeCoroutine(input);
        StartCoroutine(coroutine);
    }

    private IEnumerator TypeCoroutine(string text)
    {
       
        string remainingText = text;
     
        while (remainingText.Length > 0)
        {
            char letter = remainingText.First();
            remainingText = remainingText.Remove(0,1);
            addLetter(letter);
            yield return new WaitForSecondsRealtime(0.04f);
   
        }

    }

    private void addLetter(char letter)
    {
        text += letter;
    }



}
