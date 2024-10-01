using UnityEngine;

namespace Tools
{
    public class ConditionalFieldAttribute : PropertyAttribute
    {
        public string ConditionalFieldName { get; private set; }
        public bool VisibleWhenTrue { get; private set; }


        public ConditionalFieldAttribute(string conditionalFieldName, bool visibleWhenTrue = true)
        {
            ConditionalFieldName = conditionalFieldName;
            VisibleWhenTrue = visibleWhenTrue;
        }
    }
}