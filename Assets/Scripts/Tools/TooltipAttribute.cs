using UnityEngine;

namespace Tools
{
    public class TooltipAttribute : PropertyAttribute
    {
        public readonly string Text;

        public TooltipAttribute(string text)
        {
            this.Text = text;
        }
    }
}
