using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Gatherman.Picture;
using Android.Support.V4.Content;

namespace Gatherman.Droid.Picture
{
    class CameraManager
    {
        public enum FileFormatEnum
        {
            PNG,
            JPEG
        }

        public void BringUpCamera(string imageId, FileFormatEnum fileType)
        {
            var intent = new Intent(MediaStore.ActionImageCapture);

            Intent takePictureIntent = new Intent(MediaStore.ActionImageCapture);
            // Ensure that there's a camera activity to handle the intent
            if (takePictureIntent.ResolveActivity(Forms.Context.PackageManager) != null)
            {
                try
                {
                    var documentsDirectry = Forms.Context.GetExternalFilesDir(Android.OS.Environment.DirectoryPictures);
                    cameraFile = new Java.IO.File(documentsDirectry, imageId + "." + fileType.ToString());

                    if (cameraFile != null)
                    {
                        using (var mediaStorageDir = new Java.IO.File(documentsDirectry, string.Empty))
                        {
                            if (!mediaStorageDir.Exists())
                            {
                                if (!mediaStorageDir.Mkdirs())
                                    throw new IOException("Couldn't create directory, have you added the WRITE_EXTERNAL_STORAGE permission?");
                            }
                        }
                    }
                }
                catch (IOException ex)
                {
                    // Error occurred while creating the File
                    App.WriteOutput(ex.ToString());
                }

                //NOTE: Make sure the authority matches what you put in the AndroidManifest.xml file
                Android.Net.Uri photoURI = FileProvider.GetUriForFile(((Activity)Forms.Context), "com.yourcompany.appname.fileprovider", cameraFile);
                takePictureIntent.PutExtra(MediaStore.ExtraOutput, photoURI);
                ((Activity)Forms.Context).StartActivityForResult(takePictureIntent, 0);
            }
        }

        public void BringUpPhotoGallery(string imageId, FileFormatEnum fileType)
        {
            var imageIntent = new Intent();
            imageIntent.SetType("image/*");

            if (imageId != null)
            {
                imageIntent.PutExtra("fileName", imageId + "." + fileType.ToString());
            }

            imageIntent.SetAction(Intent.ActionGetContent);
            ((Activity)Forms.Context).StartActivityForResult(Intent.CreateChooser(imageIntent, "Select photo"), 1);
        }
    }
}


