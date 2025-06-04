
namespace Keeno
{
    /// <summary>
    /// Class in charge of the "Day/Night" cycle. (Although there isn't reallyu a Night in game).
    /// I could have put this class to better use, keeping Game1 cleaner,
    /// but some code has been left in the Game1. (due to time constraint
    /// this was a low priority).
    /// </summary>
    class TimeManager
    {
        public float TimeOfDay { get {  return _timeOfDay; } }
        public float _timeOfDay = 0f;
        public float DayLengthSeconds { get { return _dayLengthSeconds; } }
        public const float _dayLengthSeconds = 450f;

        public TimeManager() { }
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
