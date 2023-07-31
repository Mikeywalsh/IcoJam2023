namespace Temporal
{
    public class TimerBoolTemporal : BoolTemporal
    {
        public float CooldownSeconds;
        public float DurationSeconds;

        private int CooldownFrames => (int) (TemporalManager.FRAMES_PER_SECOND * CooldownSeconds);
        private int DurationFrames => (int) (TemporalManager.FRAMES_PER_SECOND * DurationSeconds);

        public override void Initialize(int bufferSize)
        {
            base.Initialize(bufferSize);
            // Buffer pre-filled
            LockedEnd = bufferSize - 1;

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