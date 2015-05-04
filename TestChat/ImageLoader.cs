using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace TestChat {
    class ImageLoader {
        public ImageLoader () {
            CutImage ();
            LoadImage ();
        }
        Image[] image=new Image[106];
        CroppedBitmap[] objImg=new CroppedBitmap[65];
        double scale=2;

        private void CutImage () {
            int count=0;

            BitmapImage src=new BitmapImage ();
            src.BeginInit ();
            src.UriSource=new Uri ("pack://application:,,,/Image/Tiles.png", UriKind.Absolute);
            src.CacheOption=BitmapCacheOption.OnLoad;
            src.EndInit ();

            for (int i=0; i<5; i++)
                for (int j=0; j<13; j++)
                    objImg[count++]=new CroppedBitmap (src, new Int32Rect (j*32, i*48, 32, 48));
        }

        private void LoadImage () {
            for (int i=0; i<106; i++) {
                if (i<53) {
                    image[i]=new Image ();
                    image[i].Source=objImg[i];
                    image[i].Width=objImg[i].Width*scale;
                    image[i].Height=objImg[i].Height*scale;
                } else {
                    image[i]=new Image ();
                    image[i].Source=objImg[i-53];
                    image[i].Width=objImg[i-53].Width*scale;
                    image[i].Height=objImg[i-53].Height*scale;
                }
            }
        }
        public Image[] getImage {
            get { return image; }
            set { ;}
        }
    }
}
