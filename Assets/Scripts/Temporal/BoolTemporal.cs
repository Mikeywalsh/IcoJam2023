using UnityEngine;

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
            Triggered = state.Triggered;
        }

        protected void TryToggle()
        {
            Debug.Log("trying toggle");
            if (Reversing || IsLocked())
            {
                return;
            }
            Triggered = !Triggered;
            OnInteractedWith();
        }

        protected void TryTurnOn()
        {
            if (Reversing || IsLocked())
            {
                return;
            }
            Triggered = true;
            OnInteractedWith();
        }

        protected void TryTurnOff()
        {
            if (Reversing || IsLocked())
            {
                return;
            }
            Triggered = false;
            OnInteractedWith();
        }
    }
}