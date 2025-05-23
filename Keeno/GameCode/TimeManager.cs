
namespace Keeno
{
    class TimeManager
    {
        public float TimeOfDay { get {  return _timeOfDay; } }
        public float _timeOfDay = 0f;
        public float DayLengthSeconds { get { return _dayLengthSeconds; } }
        public const float _dayLengthSeconds = 10f;

        public TimeManager()
        {

        }
        public void UpdateTime(float deltaSeconds)
        {
            _timeOfDay += deltaSeconds;
            if (_timeOfDay >= _dayLengthSeconds)
                _timeOfDay -= _dayLengthSeconds;  // wrap around each day
        }
        public void RestartDay()
        {
            _timeOfDay = 0f;
        }
    }
}
