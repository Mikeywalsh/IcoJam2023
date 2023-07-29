using UnityEngine;

namespace Temporal
{
    public class BoolTemporal : Temporal<BoolTemporalState>
    {
        public bool Triggered;
        
        protected override BoolTemporalState GetState()
        {
            return new(Triggered);
        }
        
        protected override void SetState(BoolTemporalState state)
        {
            Debug.Log(state.State);
            Triggered = state.State;
        }

        public void Toggle()
        {
            Triggered = !Triggered;
        }

        public void TurnOn()
        {
            Triggered = true;
        }

        public void TurnOff()
        {
            Triggered = false;
        }
    }
}