using System;

namespace Temporal
{
    public class BoolTemporal : Temporal<BoolTemporalState>
    {
        public bool Triggered;

        protected override void Start()
        {
            Array.Fill(TemporalBuffer, new BoolTemporalState(false));
        }

        protected override BoolTemporalState GetState()
        {
            return new BoolTemporalState(Triggered);
        }
        
        protected override void SetState(BoolTemporalState state)
        {
            Triggered = state.Triggered;
        }

        public void Toggle()
        {
            TemporalBuffer[CurrentFrame - 1] = new BoolTemporalState(!Triggered);
            Triggered = !Triggered;
        }

        public void TurnOn()
        {
            Triggered = true;
        }

        public void TurnOff()
        {
            TemporalBuffer[CurrentFrame - 1] = new BoolTemporalState(false);
            Triggered = false;
        }
    }
}