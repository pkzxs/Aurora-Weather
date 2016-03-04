﻿using Com.Aurora.AuWeather.LunarCalendar;
using Com.Aurora.AuWeather.Models;
using Com.Aurora.AuWeather.Models.HeWeather;
using System;
using System.Text;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Com.Aurora.Shared.Converters
{

    public class TempratureConverterWithoutDecoration : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Temprature)
            {
                switch (TempratureConverter.Parameter)
                {
                    default: return (value as Temprature).Celsius + "°";
                    case 0: return (value as Temprature).Celsius + "°";
                    case 1: return (value as Temprature).Fahrenheit + "°";
                    case 2: return (value as Temprature).Kelvin;
                }
            }
            return "X";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class TempratureConverter : IValueConverter
    {
        public static int Parameter { get; private set; } = 0;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch (Parameter)
            {
                default: return "C";
                case 0: return "C";
                case 1: return "F";
                case 2: return "K";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public static void ChangeParameter(int newPar)
        {
            if (newPar < 3 && newPar > -1)
            {
                Parameter = newPar;
            }
        }
    }

    public class WindSpeedConverter : IValueConverter
    {
        public static WindParameter WindParameter { get; private set; } = WindParameter.BeaufortandText;
        public static SpeedParameter SpeedParameter { get; private set; } = SpeedParameter.KMPH;
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != default(Wind))
            {
                var wind = value as Wind;
                StringBuilder sb = new StringBuilder();
                switch (WindParameter)
                {
                    case WindParameter.BeaufortandText:
                    case WindParameter.BeaufortandDegree:
                        sb = SetBeaufort(wind.Scale, sb);
                        break;
                    case WindParameter.SpeedandText:
                    case WindParameter.SpeedandDegree:
                        sb = SetSpeed(wind.Speed, sb);
                        break;
                    default:
                        break;
                }
                return sb.ToString();
            }
            return null;
        }
        private StringBuilder SetSpeed(Speed speed, StringBuilder sb)
        {
            switch (SpeedParameter)
            {
                case SpeedParameter.KMPH:
                    sb.Append(speed.KMPH.ToString("0.0") + " km/h");
                    break;
                case SpeedParameter.MPS:
                    sb.Append(speed.MPS.ToString("0.0") + " m/s");
                    break;
                case SpeedParameter.Knot:
                    sb.Append(speed.Knot.ToString("0.0") + " kn");
                    break;
                default:
                    break;
            }
            return sb;
        }
        private StringBuilder SetBeaufort(WindScale scale, StringBuilder sb)
        {
            switch (scale)
            {
                case WindScale.unknown:
                    sb.Append("...");
                    break;
                case WindScale.zero:
                    sb.Append("无风");
                    break;
                case WindScale.one:
                    sb.Append("平静");
                    break;
                case WindScale.two:
                    sb.Append("微风");
                    break;
                case WindScale.three:
                    sb.Append("轻风");
                    break;
                case WindScale.four:
                    sb.Append("和风");
                    break;
                case WindScale.five:
                    sb.Append("清风");
                    break;
                case WindScale.six:
                    sb.Append("强风");
                    break;
                case WindScale.seven:
                    sb.Append("疾风");
                    break;
                case WindScale.eight:
                    sb.Append("大风");
                    break;
                case WindScale.nine:
                    sb.Append("烈风");
                    break;
                case WindScale.ten:
                    sb.Append("狂风");
                    break;
                case WindScale.eleven:
                    sb.Append("暴风");
                    break;
                case WindScale.twelve:
                    sb.Append("飓风");
                    break;
                case WindScale.thirteen:
                    sb.Append("台风");
                    break;
                case WindScale.fourteen:
                case WindScale.fifteen:
                    sb.Append("强台飓风");
                    break;
                case WindScale.sixteen:
                case WindScale.seventeen:
                    sb.Append("超强台飓风");
                    break;
                case WindScale.eighteen:
                    sb.Append("极强台飓风");
                    break;
                default:
                    break;
            }
            return sb;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public static void ChangeParameter(WindParameter windFormat, SpeedParameter speedFormat)
        {
            WindParameter = windFormat;
            SpeedParameter = speedFormat;
        }
    }

    public class WindDirectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != default(Wind))
            {
                var wind = value as Wind;
                StringBuilder sb = new StringBuilder();
                switch (WindSpeedConverter.WindParameter)
                {
                    case WindParameter.BeaufortandText:
                    case WindParameter.SpeedandText:
                        sb = SetText(wind.Direction, sb);
                        break;
                    case WindParameter.BeaufortandDegree:
                    case WindParameter.SpeedandDegree:
                        sb = SetDegree(wind.Degree, sb);
                        break;
                    default:
                        break;
                }
                return sb.ToString();
            }
            return null;
        }
        private StringBuilder SetDegree(uint degree, StringBuilder sb)
        {
            sb.Append(degree);
            sb.Append('°');
            return sb;
        }

        private StringBuilder SetText(WindDirection direction, StringBuilder sb)
        {
            switch (direction)
            {
                case WindDirection.unknown:
                    sb.Append("...");
                    break;
                case WindDirection.north:
                    sb.Append("北风");
                    break;
                case WindDirection.east:
                    sb.Append("东风");
                    break;
                case WindDirection.west:
                    sb.Append("西风");
                    break;
                case WindDirection.south:
                    sb.Append("南风");
                    break;
                case WindDirection.northeast:
                    sb.Append("东北风");
                    break;
                case WindDirection.northwest:
                    sb.Append("西北风");
                    break;
                case WindDirection.southeast:
                    sb.Append("东南风");
                    break;
                case WindDirection.southwest:
                    sb.Append("西南风");
                    break;
                default:
                    sb.Append("...");
                    break;
            }
            return sb;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class TemraturePathConverter : IValueConverter
    {
        private const float _factor = -64;
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is float)
            {
                Point? point = new Point(0, (float)value * _factor);
                return point;
            }
            return new Point(0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class TempraturePathEndConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch ((string)parameter)
            {
                case "0": return new Point(0, (double)value);
                case "1": return new Point(((Size)value).Width, ((Size)value).Height);
                default: return new Point(0, 0);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class HourMinuteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            //TODO
            if (value is DateTime)
            {
                return ((DateTime)value).ToString("HH:mm");
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class PoptoThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is float)
            {
                return 2 + ((float)value) * 4;
            }
            return 2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class ScrollViewerConverter : IValueConverter
    {
        public static bool isLargeMode = false;
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (isLargeMode)
                return 480 - (double)value < 160 ? 160 : 480 - (double)value;
            else
                return 480 - (double)value < 112 ? 112 : 480 - (double)value;

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class ConditiontoTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var condition = (WeatherCondition)value;
            switch (condition)
            {
                case WeatherCondition.unknown:
                    return "...";
                case WeatherCondition.sunny:
                    return "晴";
                case WeatherCondition.cloudy:
                    return "多云";
                case WeatherCondition.few_clouds:
                    return "少云";
                case WeatherCondition.partly_cloudy:
                    return "大部多云";
                case WeatherCondition.overcast:
                    return "阴";
                case WeatherCondition.windy:
                    return "有风";
                case WeatherCondition.calm:
                    return "轻风";
                case WeatherCondition.light_breeze:
                    return "清风";
                case WeatherCondition.moderate:
                    return "和风";
                case WeatherCondition.fresh_breeze:
                    return "劲风";
                case WeatherCondition.strong_breeze:
                    return "强风";
                case WeatherCondition.high_wind:
                    return "疾风";
                case WeatherCondition.gale:
                    return "大风";
                case WeatherCondition.strong_gale:
                    return "烈风";
                case WeatherCondition.storm:
                    return "暴风";
                case WeatherCondition.violent_storm:
                    return "飓风";
                case WeatherCondition.hurricane:
                    return "台风";
                case WeatherCondition.tornado:
                    return "龙卷风";
                case WeatherCondition.tropical_storm:
                    return "热带风暴";
                case WeatherCondition.shower_rain:
                    return "阵雨";
                case WeatherCondition.heavy_shower_rain:
                    return "强阵雨";
                case WeatherCondition.thundershower:
                    return "雷阵雨";
                case WeatherCondition.heavy_thunderstorm:
                    return "雷暴";
                case WeatherCondition.hail:
                    return "冰雹";
                case WeatherCondition.light_rain:
                    return "小雨";
                case WeatherCondition.moderate_rain:
                    return "中雨";
                case WeatherCondition.heavy_rain:
                    return "大雨";
                case WeatherCondition.extreme_rain:
                    return "暴雨";
                case WeatherCondition.drizzle_rain:
                    return "毛毛雨";
                case WeatherCondition.storm_rain:
                    return "暴风雨";
                case WeatherCondition.heavy_storm_rain:
                    return "大暴雨";
                case WeatherCondition.severe_storm_rain:
                    return "严重降水";
                case WeatherCondition.freezing_rain:
                    return "冻雨";
                case WeatherCondition.light_snow:
                    return "小雪";
                case WeatherCondition.moderate_snow:
                    return "中雪";
                case WeatherCondition.heavy_snow:
                    return "大雪";
                case WeatherCondition.snowstorm:
                    return "暴风雪";
                case WeatherCondition.sleet:
                    return "雨夹雪";
                case WeatherCondition.rain_snow:
                    return "雨雪";
                case WeatherCondition.shower_snow:
                    return "阵雪";
                case WeatherCondition.snow_flurry:
                    return "短时小雪";
                case WeatherCondition.mist:
                    return "薄雾";
                case WeatherCondition.foggy:
                    return "雾";
                case WeatherCondition.haze:
                    return "霾";
                case WeatherCondition.sand:
                    return "沙尘";
                case WeatherCondition.dust:
                    return "扬尘";
                case WeatherCondition.volcanic_ash:
                    return "火山灰";
                case WeatherCondition.duststorm:
                    return "尘暴";
                case WeatherCondition.sandstorm:
                    return "沙尘暴";
                case WeatherCondition.hot:
                    return "变热";
                case WeatherCondition.cold:
                    return "变冷";
                default:
                    return "...";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class BodyTempratureAniConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return 64;
            float temp = ((Temprature)value).Celsius;
            temp = temp < -15 ? -15 : temp;
            temp = temp > 40 ? 40 : temp;
            temp += 15;
            temp /= 55;
            return 56 * (1 - temp);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class HumidityAniConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return 64;
            float temp = (uint)value;
            temp /= 100;
            return 56 * (1 - temp);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class PcpnTransAniConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return 64;
            float temp = (float)value;
            temp = temp > 150 ? 150 : temp;
            temp /= 150;
            return 56 * (1 - temp);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class PrecipitationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((float)value).ToString("0.#") + " mm";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class SunRiseAniConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return new DoubleCollection { 98.96666666, 0, (double)value * 98.96666666, 1000 };
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class SunRiseAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? HorizontalAlignment.Left : HorizontalAlignment.Right;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class SunSetAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class SunRiseTextAlignMentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? -90 : 90;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class SunSetTextAlignMentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? 90 : -90;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class MoonPhaseProgressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var p = (value as CalendarInfo).LunarDay;
            string uri;
            if (p < 2)
            {
                uri = "ms-appx:///Assets/MoonPhase/moon0.png";
            }
            else if (p < 6)
            {
                uri = "ms-appx:///Assets/MoonPhase/moon1.png";
            }
            else if (p < 9)
            {
                uri = "ms-appx:///Assets/MoonPhase/moon2.png";
            }
            else if (p < 13)
            {
                uri = "ms-appx:///Assets/MoonPhase/moon3.png";
            }
            else if (p < 17)
            {
                uri = "ms-appx:///Assets/MoonPhase/moon4.png";
            }
            else if (p < 21)
            {
                uri = "ms-appx:///Assets/MoonPhase/moon5.png";
            }
            else if (p < 24)
            {
                uri = "ms-appx:///Assets/MoonPhase/moon6.png";
            }
            else if (p < 28)
            {
                uri = "ms-appx:///Assets/MoonPhase/moon7.png";
            }
            else
            {
                uri = "ms-appx:///Assets/MoonPhase/moon0.png";
            }
            return new BitmapImage(new Uri(uri));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class MoonPhaseTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var p = (value as CalendarInfo).LunarDay;
            string result;
            if (p < 2)
            {
                result = "新月";
            }
            else if (p < 6)
            {
                result = "峨眉月";
            }
            else if (p < 9)
            {
                result = "上弦月";
            }
            else if (p < 13)
            {
                result = "上凸月";
            }
            else if (p < 17)
            {
                result = "满月";
            }
            else if (p < 21)
            {
                result = "下凸月";
            }
            else if (p < 24)
            {
                result = "下弦月";
            }
            else if (p < 28)
            {
                result = "残月";
            }
            else
            {
                result = "新月";
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class VisibilityConverter : IValueConverter
    {
        public static LengthParameter LengthParameter { get; private set; } = LengthParameter.KM;
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var l = value as Length;
            switch (LengthParameter)
            {
                case LengthParameter.KM: return l.KM.ToString("0.##") + " km";
                case LengthParameter.M: return l.M.ToString("0.##") + " m";
                case LengthParameter.Mile: return l.Mile.ToString("0.##") + " mile";
                case LengthParameter.NM: return l.NM.ToString("0.##") + " nm";
                default: return "0km";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class PressureConverter : IValueConverter
    {
        public static PressureParameter PressureParameter { get; private set; } = PressureParameter.Atm;
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var l = value as Pressure;
            switch (PressureParameter)
            {
                case PressureParameter.Atm: return l.Atm.ToString("0.####") + " Atm";
                case PressureParameter.Hpa: return l.HPa.ToString("0.####") + " Hpa";
                case PressureParameter.Torr: return l.Torr.ToString("0.####") + " Torr";
                case PressureParameter.CmHg: return l.CmHg.ToString("0.####") + " CmHg";
                default:
                    return "0 Atm";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class PressureAniConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var p = value as Pressure;
            return (p.Atm - 1) * 900;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class VisibilityAniConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var vis = (value as Length).KM;
            if (vis > 15)
            {
                return 1;
            }
            else if (vis > 8)
            {
                return 0.8;
            }
            else if (vis > 4)
            {
                return 0.6;
            }
            else if (vis > 1)
            {
                return 0.4;
            }
            else if (vis > 0.5)
            {
                return 0.2;
            }
            else
            {
                return 0.1;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class MainPaneBGConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var month = DateTime.Now.Month;
            var uri = new Uri("ms-appx:///Assets/MonthlyPic/" + month + ".png");
            return new BitmapImage(uri);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class DateNowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return DateTime.Now.ToString("yyyy-M-d dddd");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class LunarCalendarConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                var calendar = value as CalendarInfo;
                return "农历 " + calendar.LunarYearSexagenary + "年" + calendar.LunarMonthText + "月" + calendar.LunarDayText;
            }
            return "...";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class PaneHamburgerForeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var month = DateTime.Now.Month;
            if (month / 2 == 0 || month == 3)
            {
                return new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            }
            return new SolidColorBrush(Color.FromArgb(255, 240, 240, 240));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class AqiCircleAniConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return new DoubleCollection { (double)value * 109.97333333, 1000 };
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class AqiCircleColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var q = (value as AQI).Qlty;
            switch (q)
            {
                case AQIQuality.unknown:
                    return Color.FromArgb(255, 240, 240, 240);
                case AQIQuality.one:
                    return Color.FromArgb(255, 54, 204, 0);
                case AQIQuality.two:
                    return Color.FromArgb(255, 134, 134, 0);
                case AQIQuality.three:
                    return Color.FromArgb(255, 204, 204, 0);
                case AQIQuality.four:
                    return Color.FromArgb(255, 255, 134, 0);
                case AQIQuality.five:
                    return Color.FromArgb(255, 255, 51, 0);
                case AQIQuality.six:
                    return Color.FromArgb(255, 255, 0, 0);
                default:
                    return Color.FromArgb(255, 240, 240, 240);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class AQIQualityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var q = (value as AQI).Qlty;
            switch (q)
            {
                case AQIQuality.unknown:
                    return "...";
                case AQIQuality.one:
                    return "优";
                case AQIQuality.two:
                    return "良";
                case AQIQuality.three:
                    return "轻度污染";
                case AQIQuality.four:
                    return "中度污染";
                case AQIQuality.five:
                    return "重度污染";
                case AQIQuality.six:
                    return "严重污染";
                default:
                    return "...";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class AQIValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value as AQI).Aqi;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class PM25Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value as AQI).Pm25;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class PM10Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value as AQI).Pm10;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class SO2Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value as AQI).So2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class SO2ProgressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value as AQI).So2 > 500 ? 1 : (value as AQI).So2 / 500f;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class PM25ProgressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value as AQI).Pm25 > 500 ? 1 : (value as AQI).Pm25 / 500f;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class PM10ProgressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value as AQI).Pm10 > 500 ? 1 : (value as AQI).Pm10 / 500f;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class AqiProgressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value as AQI).Aqi > 500 ? 1 : (value as AQI).Aqi / 500f;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class ImmersiveDateTimeConverter : IValueConverter
    {
        public static string DateTimeConverterParameter { get; private set; } = "H:mm";
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((DateTime)value).ToString(DateTimeConverterParameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class SuggestionBrfConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value as Suggestion).Brief;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class SuggestionTxtConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value as Suggestion).Text;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
