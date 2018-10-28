using System;
using SeriesUI.Common;

namespace SeriesUI.Configuration
{
    public class ColorPaletteKey
    {
        public ColorPaletteKey(CompletenessState completeness, bool active, Type type)
        {
            Completeness = completeness;
            Active = active;
            ObjectType = type;
        }

        private CompletenessState Completeness { get; }

        private bool Active { get; }

        private Type ObjectType { get; }

        // [!] The default GetHashCode does not work as intended
        public override int GetHashCode()
        {
            return ObjectType.GetHashCode() * 1000 + Completeness.GetHashCode() * 100 + Active.GetHashCode();
        }
    }
}