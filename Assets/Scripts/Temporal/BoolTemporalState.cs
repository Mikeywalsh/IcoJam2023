namespace Temporal
{
    public record BoolTemporalState : TemporalState
    {
        public bool State { get; }

        public BoolTemporalState(bool state)
        {
            State = state;
        }
    }
}