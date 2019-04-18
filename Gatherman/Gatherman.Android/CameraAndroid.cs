using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using System.IO;
using Android.Provider;
using Android.Support.V4.Content;
using Xamarin.Forms;
using Gatherman.Picture;
using static Gatherman.Droid.Picture.CameraManager;
using Java.IO;

[assembly: Dependency(typeof(XamarinFormsCamera.Droid.CameraAndroid))]
namespace XamarinFormsCamera.Droid
{
    public class CameraAndroid : CameraInterface
    {
        private Java.IO.File cameraFile;

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
                    //TODO Essayer de changer avec System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
                    var documentsDirectory = Forms.Context.GetExternalFilesDir(Android.OS.Environment.DirectoryPictures);
                    cameraFile = new Java.IO.File(documentsDirectory, imageId + "." + fileType.ToString());

                    if (cameraFile != null)
                    {
                        using (var mediaStorageDir = new Java.IO.File(documentsDirectory, string.Empty))
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
                    //App.WriteOutput(ex.ToString());
                }
                //NOTE: Make sure the authority matches what you put in the AndroidManifest.xml file
                Android.Net.Uri photoURI = FileProvider.GetUriForFile(((Activity)Forms.Context), "com.Gatherman.fileprovider", cameraFile);
                takePictureIntent.PutExtra(MediaStore.ExtraOutput, photoURI);
                ((Activity)Forms.Context).StartActivityForResult(takePictureIntent, 0);
            }
            //throw new NotImplementedException();
        }

        public void BringUpPhotoGallery()
        {
            //throw new NotImplementedException();
        }
    }

}