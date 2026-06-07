using UnityEngine;
namespace ShoelaceStudios.ImprovedTimers.Timers
{
    public class AdjustableCountdownTimer : CountdownTimer
    {
        public AdjustableCountdownTimer(float value) : base(value)
        {
            CurrentTickMultiplier = 1f;
        }

        public float CurrentTickMultiplier;

        public override void Tick()
        {
            if (IsRunning && CurrentTime > 0)
            {
                CurrentTime -= Time.deltaTime * CurrentTickMultiplier;
            }

            if (IsRunning && CurrentTime <= 0)
            {
                Stop();
            }
        }

        public override void Reset()
        {
            base.Reset();
            CurrentTickMultiplier = 1f;
        }
    }
}
