using UnityEngine;
using UnityEngine.Events;
using System.Collections;


namespace SengokuWarrior
{
    public class CameraTransitionHandlerer : MonoBehaviour
    {
        public enum FadeStatus
        {
            Fading,
            Faded
        }

        public static UnityAction fadeEnded;
        public FadeStatus state;
        public static CameraTransitionHandlerer _intance;

        void Awake()
        {
            if (_intance != this) GameObject.Destroy(_intance);
            _intance = this;
        }
        public IEnumerator BeginFadeIn()
        {
            state = FadeStatus.Fading;
            UIManager._instance.FadeScreen.alpha = 0;
            UIManager._instance.FadeScreen.gameObject.SetActive(true);
            Fade(0, 1);
            yield return new WaitUntil(() => !fadeInProgress());
            if (fadeEnded != null)fadeEnded.Invoke();

        }
        public IEnumerator BeginFadeOut()
        {
            state = FadeStatus.Fading;
            UIManager._instance.FadeScreen.gameObject.SetActive(true);
            Fade(UIManager._instance.FadeScreen.alpha, 0);
            yield return new WaitUntil(() => !fadeInProgress());
            if (fadeEnded != null) fadeEnded.Invoke();
            UIManager._instance.FadeScreen.gameObject.SetActive(false);

        }
        private void Fade(float from,float to)
        {
            iTween.ValueTo(this.gameObject,iTween.Hash("target", gameObject, "from", from, "to", to, "time", 1f,"onupdate",  "UpdateFadeOpacity", "oncomplete", "FadeEnded"));
        }
        public void UpdateFadeOpacity(float val)
        {
            UIManager._instance.FadeScreen.alpha = val;
        }
        private void FadeEnded()
        {         
            state = FadeStatus.Faded;
        }

        public bool fadeInProgress()
        {
            return state == FadeStatus.Fading;
        }
    }
}
