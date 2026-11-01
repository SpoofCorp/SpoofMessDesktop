using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SpoofMess.ViewElements;

public class VolumeToGeomConverter : IValueConverter
{
    private const string BaseSpeaker = "M 0 6.25 L 6 5 12 0 12 18.75 6 13.75 0 13.75 0 5";
    private const string LowWave = " M 14 6.25 S 18 9.5 14 13.75";
    private const string MidWave = " M 16 3.125 S 22 10 16 16.875";
    private const string HighWave = " M 18 0 S 25 10 18 20";

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is float vol)
        {
            if (vol > 0.75f)
                return Geometry.Parse(BaseSpeaker + LowWave + MidWave + HighWave);
            if (vol > 0.4f)
                return Geometry.Parse(BaseSpeaker + LowWave + MidWave);
            if (vol > 0.0f)
                return Geometry.Parse(BaseSpeaker + LowWave);
        }

        return Geometry.Parse(BaseSpeaker);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}