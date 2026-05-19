using LifeLine.MVVM.Models.MSSQL_DB;
using LifeLine.Utils.Helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace LifeLine.Utils.ValueConverter
{
    class EmptyImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is byte[] employee)
            {
                //if (employee == null)
                //{
                //    string imagePas = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "C:\\Users\\texno\\source\\repos\\LifeLine\\LifeLine\\Res\\", "Kotozila.jpg");

                //    if (File.Exists(imagePas))
                //    {
                //        //return new BitmapImage(new Uri(imagePas, UriKind.Absolute));
                //        using (Image image = Image.FromFile(imagePas))
                //        {
                //            return FileHelper.ImageToBytes(image);
                //        }
                //    }
                //}

                string imagePas = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "C:\\Users\\texno\\source\\repos\\LifeLine\\LifeLine\\Res\\", "Kotozila.jpg");

                if (File.Exists(imagePas))
                {
                    //return new BitmapImage(new Uri(imagePas, UriKind.Absolute));
                    //using (Image image = Image.FromFile(imagePas))
                    //{
                    //    return FileHelper.ImageToBytes(image);
                    //}
                }
            }
            else
            {
                return value;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
