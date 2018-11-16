using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SeriesUI.Annotations;
using SeriesUI.Common;

namespace SeriesUI.BusinessLogic
{
    [Serializable]
    public class Episode : INotifyPropertyChanged
    {
        private bool downloaded;

        public Episode()
        {
            SubTitles = new Dictionary<SubTitle, bool>
            {
                [SubTitle.Nl] = false,
                [SubTitle.En] = false
            };
        }

        public Episode(string title) : this()
        {
            Title = title;
        }

        public DateTime AirDate { get; set; }

        public string Title { get; set; }

        public string Number { get; set; }

    public bool Downloaded
        {
            get => downloaded;
            set
            {
                downloaded = value;
                OnPropertyChanged();
            }
        }

        // [!] setter is really needed if we want to copy it from the GUI
        public Dictionary<SubTitle, bool> SubTitles { get; set; }

        /// <summary>
        ///     Separate property per language to allow OnPropertyChanged call
        /// </summary>
        public bool SubTitleNl
        {
            get => SubTitles[SubTitle.Nl];
            set
            {
                SubTitles[SubTitle.Nl] = value;
                OnPropertyChanged(nameof(SubTitleNl));
            }
        }

        /// <summary>
        ///     Separate property per language to allow OnPropertyChanged call
        /// </summary>
        public bool SubTitleEn
        {
            get => SubTitles[SubTitle.En];
            set
            {
                SubTitles[SubTitle.En] = value;
                OnPropertyChanged(nameof(SubTitleEn));
            }
        }

        #region Class methods

        public CompletenessState Completeness
        {
            get
            {
                var result = CompletenessState.Complete;

                // Ignore future episodes
                if (AirDate.Date >= DateTime.Today.Date) result = CompletenessState.NotApplicable;
                else if (!Downloaded) result = CompletenessState.NotDownloaded;
                else if (!SubTitles[SubTitle.Nl] && !SubTitles[SubTitle.En])
                    result = CompletenessState.NotSubbed;
                else if (!SubTitles[SubTitle.Nl]) result = CompletenessState.NotSubbedNl;

                return result;
            }
        }

        #endregion

        // [!] tag needed to avoid errors during save
        [field: NonSerialized] public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}