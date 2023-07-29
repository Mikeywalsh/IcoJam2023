using System.Collections.Generic;
using UnityEngine;

namespace Temporal
{
    public abstract class DependantBoolTemporal : BoolTemporal
    {
        [SerializeField]
        protected List<BoolTemporal> BoolTemporals;

        protected override BoolTemporalState GetState()
        {
            Triggered = BoolTemporals.TrueForAll(temporal => temporal.Triggered);
            return new BoolTemporalState(Triggered);
        }
    }
}