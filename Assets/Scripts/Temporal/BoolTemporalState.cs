namespace Temporal
{
    public record BoolTemporalState : TemporalState
    {
        public bool Triggered { get; }

        public BoolTemporalState(bool triggered)
        {
            Triggered = triggered;
        }
    }
}