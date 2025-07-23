using Sirenix.OdinInspector;
using System;

namespace gmsoft
{
    [Serializable]
    public class UIEvent : AnalyticEvent
    {
        [PropertyOrder(-1)]
        public UIEventTracker.PointerState pointerState;
    }
}
