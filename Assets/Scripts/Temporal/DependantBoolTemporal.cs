using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Temporal
{
    public abstract class DependantBoolTemporal : BoolTemporal
    {
        [SerializeField]
        protected List<BoolTemporal> BoolTemporals;

        public int TriggersRequired;

        public override int ExecutionOrder() => 1;
        
        protected override BoolTemporalState GetState()
        {
            Triggered = BoolTemporals.Count(temporal => temporal.Triggered) >= TriggersRequired;
            return new BoolTemporalState(Triggered);
        }
        
        protected override string GetInformationText()
        {
            var triggeredCount = BoolTemporals.Count(temporal => temporal.Triggered);

            return $"{triggeredCount}/{TriggersRequired}";
        }
        
        protected override bool ShouldDisplayInformationText()
        {
            return !Triggered;
        }
    }
}