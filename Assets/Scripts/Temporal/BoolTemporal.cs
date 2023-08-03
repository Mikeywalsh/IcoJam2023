namespace Temporal
{
    public abstract class BoolTemporal : Temporal<BoolTemporalState>
    {
        public bool Triggered;

        private BoolTemporalState _initialState;

        protected override BoolTemporalState GetState()
        {
            return new BoolTemporalState(Triggered);
        }
        
        protected override void SetState(BoolTemporalState state)
        {
            var oldState = Triggered;
            
            Triggered = state.Triggered;
            
            if (oldState != state.Triggered)
            {
                OnStateChanged();
            }
        }

        protected virtual void OnStateChanged()
        {
        }
    }
}