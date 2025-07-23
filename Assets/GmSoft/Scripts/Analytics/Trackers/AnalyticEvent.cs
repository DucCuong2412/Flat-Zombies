using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace gmsoft
{
    public class AnalyticEvent
    {
        public int analyticsLevel = 2;
        public string name;
        public bool hasParams;
        [ShowIf("hasParams"), ShowInInspector]
        public Dictionary<string, string> @params = new Dictionary<string, string>();
    }
}
