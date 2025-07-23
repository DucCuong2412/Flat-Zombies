using Sirenix.OdinInspector;
using System;

namespace gmsoft
{
    [Serializable]
    public class LifeCycleEvent : AnalyticEvent
    {
        [PropertyOrder(-1)]
        public LifeCycleEventTracker.LifeCycle lifeCycle;
    }
}
