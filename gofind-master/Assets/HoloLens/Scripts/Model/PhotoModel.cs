using System;
using UnityEngine;

namespace Assets.HoloLens.Scripts.Model
{
    /*
* Various EventArgs has been created so that if changes in the Model has been made, a callback can be
* invoked to the controller which then sends it to the view
*/
    public class PhotoChangedEventArgs : EventArgs
    {
    }
    public class PhotoMenuChangedEventArgs : EventArgs
    {
        public bool flag { get; private set; }

        public PhotoMenuChangedEventArgs(bool flag)
        {
            this.flag = flag;
        }
    }
    
    public interface IPhotoModel
    {
        // Dispatched when years changes
        event EventHandler<PhotoChangedEventArgs> OnPictureChanged;
        event EventHandler<PhotoMenuChangedEventArgs> VisibilityChange;
 
        void ChangeVisibility(bool flag);

    
        // Texture
        Texture Photo { get; set; }
    }
    /*
 * Models are used to store information of different UI Menus.
 * Model informations can changed by the controller.
 * An Interface has been also implemented so that the controller han can access only the interface functions
 */
    public class PhotoModel:IPhotoModel
    {
        //Imported from existing code
        public const string DATA_URL_PREFIX = "data:";
        public const string IMAGE_PNG = "image/png;";
        public const string IMAGE_JPEG = "image/jpeg;";
        public const string DATA_URL_POST_IMAGE_SEQUENCE = "base64,";
        private Texture photo;

        private bool showHideMenu;
        
        public event EventHandler<PhotoChangedEventArgs> OnPictureChanged = (sender, e) => { };
        public event EventHandler<PhotoMenuChangedEventArgs> VisibilityChange = (sender, e) => { };
        /*
                * Eventhandler is used to to send events
                 * This method is used for changing the visibility of the menu
                 */ 

        public void ChangeVisibility(bool flag)
        {
            showHideMenu = flag;
            var eventArgs = new PhotoMenuChangedEventArgs(showHideMenu);
            
            // Dispatch the 'position changed' event
            VisibilityChange(this, eventArgs);
        }

        public Texture Photo {
            get { return photo; }
            set
            {
                // Only if the photo changes
                if (photo != value)
                {
                    // Set new position
                    photo = value;
                }
            
                // Dispatch the photochanged event
                var eventArgs = new PhotoChangedEventArgs();
                OnPictureChanged(this, eventArgs);
            }
        }
    }
}