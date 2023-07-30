namespace Temporal
{
    public class TimerBoolTemporal : BoolTemporal
    {
        public float CooldownSeconds;
        public float DurationSeconds;

        private int CooldownFrames => (int) (TemporalManager.FRAMES_PER_SECOND * CooldownSeconds);
        private int DurationFrames => (int) (TemporalManager.FRAMES_PER_SECOND * DurationSeconds);

        protected override void Start()
        {
            // Buffer pre-filled
            LockedEnd = TemporalManager.MAX_LEVEL_FRAMES - 1;

            for (var i = 0; i < TemporalBuffer.Length; i++)
            {
                if (i % CooldownFrames <= DurationFrames)
                {
                    TemporalBuffer[i] = new BoolTemporalState(true);
                } 
                else
                {
                    TemporalBuffer[i] = new BoolTemporalState(false);
                }
            }
        }
    }
}