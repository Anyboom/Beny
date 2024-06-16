using Beny.Base;

namespace Beny.Models
{
    public class FootballEventTag : NotifyPropertyChanged
    {
        private int _footballEventId;
        private FootballEvent _footballEvent;

        private int _tagId;
        private Tag _tag;

        public int FootballEventId
        {
            get
            {
                return _footballEventId;
            }
            set
            {
                _footballEventId = value;

                OnPropertyChanged(nameof(FootballEventId));
            }
        }

        public FootballEvent FootballEvent
        {
            get
            {
                return _footballEvent;
            }
            set
            {
                _footballEvent = value;

                OnPropertyChanged(nameof(FootballEvent));
            }
        }

        public int TagId
        {
            get
            {
                return _tagId;
            }
            set
            {
                _tagId = value;

                OnPropertyChanged(nameof(TagId));
            }
        }

        public Tag Tag
        {
            get
            {
                return _tag;
            }
            set
            {
                _tag = value;

                OnPropertyChanged(nameof(Tag));
            }
        }
    }
}
