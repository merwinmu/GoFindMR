using System;


namespace Assets.HoloLens.Scripts.Model
{
    /*
* Various EventArgs has been created so that if changes in the Model has been made, a callback can be
* invoked to the controller which then sends it to the view
*/
    public class SearchYearChangedEventArgs : EventArgs
    {
    }

    public class SliderValueEventArgs : EventArgs
    {
        public float value;
    }

    public class TemporalMenuChangedEventArgs : EventArgs
    {
        public bool flag { get; private set; }

        public TemporalMenuChangedEventArgs(bool flag)
        {
            this.flag = flag;
        }
    }
    


    
    /*
* Models are used to store information of different UI Menus.
* Model informations can changed by the controller.
* An Interface has been also implemented so that the controller han can access only the interface functions
*/
    public interface ITemporalModel
    {
        // Dispatched when years changes
        event EventHandler<SearchYearChangedEventArgs> OnYearchanged;
        event EventHandler<TemporalMenuChangedEventArgs> VisibilityChange;
        /*
                 * Eventhandler is used to to send events
                  * This method is used for changing the visibility of the menu
                  */ 
        void ChangeVisibility(bool flag);

    
        // Search Text
        DateTime UpperBound { get; set; }
        DateTime LowerBound { get; set; }
        
        void setUpperBound(float value);
        void setLowerBound(float value);

        float getUpperBound();
        float getLowerBound();
    }

    public class TemporalModel : ITemporalModel
    {
        // Backing field for the Years
        private DateTime upperBound;
        private DateTime lowerBound;

        private bool showTextBox;


        public event EventHandler<SearchYearChangedEventArgs> OnYearchanged = (sender, e) => { };
        public event EventHandler<TemporalMenuChangedEventArgs> VisibilityChange = (sender, e) => { };

        //Used for Hiding UI Object
        public void ChangeVisibility(bool flag)
        {
            showTextBox = flag;
            var eventArgs = new TemporalMenuChangedEventArgs(showTextBox);

            // Dispatch the 'position changed' event
            VisibilityChange(this, eventArgs);
        }

        // Saving Upper and Lower Bound Input
        public DateTime UpperBound
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

        public DateTime LowerBound
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

        public float upperbound;
        public float lowerbound;

        public void setUpperBound(float value)
        {
            upperbound = value;
        }

        public float getUpperBound()
        {
            return this.upperbound;
        }

        public void setLowerBound(float value)
        {
            lowerbound = value;
        }
        public float getLowerBound()
        {
            return this.lowerbound;
        }
        
    }
}