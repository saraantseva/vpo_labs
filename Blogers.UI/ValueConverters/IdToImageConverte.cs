using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Blogers.UI.ValueConverters
{
    public class IdToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int id)
            {
                
                var imagesFolder = Path.Combine(FileSystem.AppDataDirectory, "Images");
                var filePath = Path.Combine(imagesFolder, $"{id}.jpg");


                if (File.Exists(filePath))
                {
                    return ImageSource.FromFile(filePath);
                }
            }

            return ImageSource.FromFile("placeholder.png");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
