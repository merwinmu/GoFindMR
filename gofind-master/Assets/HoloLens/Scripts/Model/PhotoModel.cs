using System;
using UnityEngine;

namespace Assets.HoloLens.Scripts.Model
{
    
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