using System;
using System.Collections.Generic;
using System.Linq;
using SeriesUI.Common;

namespace SeriesUI.BusinessLogic
{
    [Serializable]
    public class Season
    {
        public Season(int sequence)
        {
            Sequence = sequence;
            Episodes = new List<Episode>();
        }

        public int Sequence { get; }

        public List<Episode> Episodes { get; set; }

        public CompletenessState Completeness
        {
            get
            {
                // Select the worst completeness
                var seasonCompleteness = Episodes.Where(c => c.Completeness > CompletenessState.NotApplicable)
                    .Select(x => x.Completeness).DefaultIfEmpty(CompletenessState.Complete).Max();
                return seasonCompleteness;
            }
        }

        #region Class methods

        public void ToggleAllSubs(SubTitle subTitle)
        {
            // [!] Somehow, using:
            // c.SubTitles[subTitle] = !Episodes[0].SubTitles[subTitle];
            // results in all subs having a value unequal to the first

            if (Episodes.Count > 0)
            {
                var checkedValue = !Episodes[0].SubTitles[subTitle];

                switch (subTitle)
                {
                    case SubTitle.Nl:
                        Episodes.ToList().ForEach(c => c.SubTitleNl = checkedValue);
                        break;
                    case SubTitle.En:
                        Episodes.ToList().ForEach(c => c.SubTitleEn = checkedValue);
                        break;
                    default:
                        throw new ArgumentException("Invalid subtitle provided");
                }
            }
        }

        public void ToggleDownloaded()
        {
            if (Episodes.Count > 0)
            {
                var checkedValue = !Episodes[0].Downloaded;

                Episodes.ToList().ForEach(c => c.Downloaded = checkedValue);
            }
        }

        public void SetAllSubs(SubTitle sub)
        {
            foreach (var episode in Episodes)
                episode.SubTitles[sub] = true;
        }

        public void SetAllEpisodesDownloaded()
        {
            foreach (var episode in Episodes)
                episode.Downloaded = true;
        }

        #endregion
    }
}