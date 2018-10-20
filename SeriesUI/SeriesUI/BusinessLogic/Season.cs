using System.Collections.Generic;

namespace SeriesUI.BusinessLogic
{
    internal class Season
    {
        public Season(int sequence)
        {
            Sequence = sequence;
            Episodes = new List<Episode>();
        }

        public int Sequence { get; }

        public List<Episode> Episodes { get; set; }
    }
}