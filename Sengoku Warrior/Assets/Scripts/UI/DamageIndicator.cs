using UnityEngine;
using UnityEngine.UI;
namespace SengokuWarrior
{
    public class DamageIndicator:MonoBehaviour
    {
        public Text text;


        public void Show(string amount, Color color)
        {
            text.text = amount;
            text.color = color;
            iTween.MoveBy(gameObject, iTween.Hash("time", 0.75f, "y", 0.25f, "oncomplete", "Destroy", "oncompletetarget", gameObject));
        }
        public void Destroy()
        {
            GameObject.Destroy(gameObject);
        }
    }
}
