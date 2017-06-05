using UnityEngine;

namespace PanzerNoob.Managers
{
    using Tools;

    public class TimeManager : GenericSingleton<TimeManager>
    {
        private float _deltaTime = 1f / 60f;
        public float DeltaTime {
            get { return _deltaTime; }
        }
        private float _timeScale = 1f;
        public float TimeScale {
            get { return _timeScale; }
            set { _timeScale = value; }
        }
        private float _timeStamp = 0f;
        public float TimeStamp {
            get { return _timeStamp; }
        }
        private float _now = 0f;
        protected override void Update()
        {
            _now = _timeStamp;
            _timeStamp += Time.deltaTime * _timeScale;
            _deltaTime = _timeStamp - _now;
        }
    }
}
