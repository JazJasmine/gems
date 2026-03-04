
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace Gems
{
    namespace Games
    {
        public class CircleGemProgress : UdonSharpBehaviour
        {
            [SerializeField] GameObject unknownGameobject;
            [SerializeField] GameObject knownGameobject;
            [SerializeField] Image progressRingFill;
            [SerializeField] GameObject[] purityStars;

            [Header("Tween Settings")]
            public float durationToFull = 0.12f;
            public float durationToTarget = 0.22f;
            public float step = 0.02f;

            float from;
            float initialTo;
            float to;
            float t;
            float duration;

            bool isTweening;
            bool wrapPhase1;
            bool wrapPending;

            public void SetFillAmount(float amount)
            {
                progressRingFill.fillAmount = amount;
            }

            public void SetFillSmooth(float target)
            {
                // Start a new tween from current visual state
                from = progressRingFill.fillAmount;
                initialTo = target;
                to = target;

                bool wrap = to < progressRingFill.fillAmount;

                if (wrap)
                {
                    wrapPending = true;
                    wrapPhase1 = true;

                    StartTween(from, 1f, durationToFull);
                }
                else
                {
                    wrapPending = false;
                    StartTween(from, to, durationToTarget);
                }
            }

            private void StartTween(float _from, float _to, float _duration)
            {
                from = _from;
                to = _to;
                duration = _duration;
                t = 0f;

                if (!isTweening)
                {
                    isTweening = true;
                    TweenStep();
                }
            }

            public void TweenStep()
            {
                if (!isTweening) return;

                t += (step / duration);
                float t01 = Mathf.Clamp01(t);
                float eased = 1f - Mathf.Pow(1f - t01, 3f);

                progressRingFill.fillAmount = Mathf.Lerp(from, to, eased);

                if (t01 >= 1f)
                {
                    isTweening = false;

                    progressRingFill.fillAmount = to;
                    if (wrapPending && wrapPhase1)
                    {
                        wrapPhase1 = false;
                        progressRingFill.fillAmount = 0f;
                        StartTween(0f, initialTo, durationToTarget);
                        return;
                    }

                    wrapPending = false;
                    return;
                }

                SendCustomEventDelayedSeconds(nameof(TweenStep), step);
            }

            public void SetPurity(string purity, int purityId)
            {
                foreach (var obj in purityStars)
                {
                    obj.SetActive(false);
                }

                for (var i = 0; i < purityId + 1; i++)
                {
                    purityStars[i].SetActive(true);
                }
            }

            public Color Color
            {
                get => progressRingFill.color;
                set
                {
                    progressRingFill.color = value;
                }
            }

            public void DebugFill()
            {
                var randomTarget = Random.value;
                SetFillSmooth(randomTarget);
            }

            public void DebugFill1()
            {
                SetFillSmooth(1);
            }

            public bool Unknown
            {
                set
                {
                    unknownGameobject.SetActive(value);
                    knownGameobject.SetActive(!value);
                }
            }
        }
    }
}