using UnityEngine;

namespace Tools
{
    public class TooltipAttribute : PropertyAttribute
    {
        public readonly string text;

        public TooltipAttribute(string text)
        {
            this.text = text;
        }
    }
}
