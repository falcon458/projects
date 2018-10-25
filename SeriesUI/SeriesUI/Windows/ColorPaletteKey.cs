using SeriesUI.BusinessLogic;

namespace SeriesUI.Windows
{
    public class ColorPaletteKey
    {
        public ColorPaletteKey(CompletenessState completeness, bool active)
        {
            Completeness = completeness;
            Active = active;
        }

        public CompletenessState Completeness { get; }

        public bool Active { get; }

        // The default GetHashCode does not work as intended
        public override int GetHashCode()
        {
            return Completeness.GetHashCode() * 100 + Active.GetHashCode();
        }
    }
}