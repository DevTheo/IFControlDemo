using System;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation.Metadata;

namespace IFControl.Interfaces.Support
{
    public interface IBasicImageSupport
    {
        bool SupportsImageDisplay { get; }
        //IList Images { get; } /*List<>*/
        void SetImage(int imageNum, [ReadOnlyArray] byte[] imageData);

        [DefaultOverloadAttribute]
        void DrawPictureByNum(int imageNum /*int x = -1, int y = -1, int width = -1, int height = -1*/);
        void DrawPictureByNum(int imageNum, int x /*, int y = -1, int width = -1, int height = -1*/);
        void DrawPictureByNum(int imageNum, int x, int y /*, int width = -1, int height = -1*/);
        void DrawPictureByNum(int imageNum, int x, int y, int width /*, int height = -1*/);        
        void DrawPictureByNum(int imageNum, int x, int y, int width, int height);

        [DefaultOverloadAttribute]
        void DrawPicture([ReadOnlyArray] byte[] imageData /*, int x = -1, int y = -1, int width = -1, int height = -1*/);
        void DrawPicture([ReadOnlyArray] byte[] imageData , int x /*, int y = -1, int width = -1, int height = -1*/);
        void DrawPicture([ReadOnlyArray] byte[] imageData, int x, int y /*, int width = -1, int height = -1*/);
        void DrawPicture([ReadOnlyArray] byte[] imageData, int x, int y, int width /*, int height = -1*/);
        void DrawPicture([ReadOnlyArray] byte[] imageData, int x, int y, int width, int height);


    }
}
