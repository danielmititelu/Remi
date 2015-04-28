using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace TestChat {
    class ImageLoader {

        CroppedBitmap[] objImg = new CroppedBitmap[65];

        private void CutImage () {
            int count=0;

            BitmapImage src=new BitmapImage ();
            src.BeginInit ();
            src.UriSource=new Uri ("pack://application:,,,/Image/Tiles.png", UriKind.Absolute);
            src.CacheOption=BitmapCacheOption.OnLoad;
            src.EndInit ();

            for (int i=0; i<4; i++)
                for (int j=0; j<12; j++)
                    objImg[count++]=new CroppedBitmap (src, new Int32Rect (j*32, i*48, 32, 48));
        }
    }
}
