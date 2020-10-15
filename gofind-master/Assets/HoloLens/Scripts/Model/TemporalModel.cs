using System;

namespace Assets.HoloLens.Scripts.Model
{
    public class SearchYearChangedEventArgs : EventArgs
    {
    }
    
    public class TemporalMenuChangedEventArgs : EventArgs
    {
        public bool flag { get; private set; }

        public TemporalMenuChangedEventArgs(bool flag)
        {
            this.flag = flag;
        }
    }
    
    public interface ITemporalModel
    {
        // Dispatched when years changes
        event EventHandler<SearchYearChangedEventArgs> OnYearchanged;
        event EventHandler<TemporalMenuChangedEventArgs> VisibilityChange;

        void ChangeVisibility(bool flag);

    
        // Search Text
        string UpperBound { get; set; }
        string LowerBound { get; set; }
    }

    public class TemporalModel: ITemporalModel
    {
        // Backing field for the Years
        private string upperBound;
        private string lowerBound;
        
        private bool showTextBox;

        
        public event EventHandler<SearchYearChangedEventArgs> OnYearchanged = (sender, e) => { };
        public event EventHandler<TemporalMenuChangedEventArgs> VisibilityChange = (sender, e) => { };

        public void ChangeVisibility(bool flag)
        {
            showTextBox = flag;
            var eventArgs = new TemporalMenuChangedEventArgs(showTextBox);
            
            // Dispatch the 'position changed' event
            VisibilityChange(this, eventArgs);
        }

        public string UpperBound
        {
            get { return upperBound; }
            set
            {
                // Only if the latitude changes
                if (upperBound != value)
                {
                    // Set new position
                    upperBound = value;
                }
            
                // Dispatch the 'position changed' event
                var eventArgs = new SearchYearChangedEventArgs();
                OnYearchanged(this, eventArgs);
            }

        }
        
        public string LowerBound
        {
            get { return lowerBound; }
            set
            {
                // Only if the latitude changes
                if (lowerBound != value)
                {
                    // Set new position
                    lowerBound = value;
                }
            
                // Dispatch the 'position changed' event
                var eventArgs = new SearchYearChangedEventArgs();
                OnYearchanged(this, eventArgs);
            }
        }
        
    }
}