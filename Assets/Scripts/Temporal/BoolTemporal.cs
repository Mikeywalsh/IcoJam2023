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

        protected void TryToggle()
        {
            if (Reversing || IsLocked())
            {
                return;
            }
            Triggered = !Triggered;
            OnStateChanged();
            OnInteractedWith();
            PlaySound(Triggered);
        }

        protected virtual void OnStateChanged()
        {
        }
        
        protected void TryTurnOn()
        {
            if (Reversing || IsLocked())
            {
                return;
            }
            if (!Triggered)
            {
                PlaySound(true);
            }
            Triggered = true;
            OnStateChanged();
            OnInteractedWith();
        }

        protected void TryTurnOff()
        {
            if (Reversing || IsLocked())
            {
                return;
            }
            Triggered = false;
            OnStateChanged();
            OnInteractedWith();
            PlaySound(false);
        }

        private void PlaySound(bool on)
        {
            AudioManager.Play(on ? "button-on" : "button-off");
        }
    }
}