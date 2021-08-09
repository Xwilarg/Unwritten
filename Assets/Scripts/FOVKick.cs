using System;
using System.Collections;
using UnityEngine;

namespace Unwritten
{
    [Serializable]
    public class FOVKick
    {
        private float _originalFov;
        public float FOVIncrease = 3f;                  // The amount the field of view increases when going into a run
        public float TimeToIncrease = 1f;               // The amount of time the field of view will increase over
        public float TimeToDecrease = 1f;               // The amount of time the field of view will take to return to its original size
        public AnimationCurve IncreaseCurve;

        public void Setup(Camera camera)
        {
            _originalFov = Camera.main.fieldOfView;
        }

        public IEnumerator FOVKickUp()
        {
            float t = Mathf.Abs((Camera.main.fieldOfView - _originalFov)/FOVIncrease);
            while (t < TimeToIncrease)
            {
                Camera.main.fieldOfView = _originalFov + (IncreaseCurve.Evaluate(t/TimeToIncrease)*FOVIncrease);
                t += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }


        public IEnumerator FOVKickDown()
        {
            float t = Mathf.Abs((Camera.main.fieldOfView - _originalFov)/FOVIncrease);
            while (t > 0)
            {
                Camera.main.fieldOfView = _originalFov + (IncreaseCurve.Evaluate(t/TimeToDecrease)*FOVIncrease);
                t -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            Camera.main.fieldOfView = _originalFov;
        }
    }
}
