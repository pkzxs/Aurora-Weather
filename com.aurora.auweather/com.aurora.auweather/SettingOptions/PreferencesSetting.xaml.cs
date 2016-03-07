﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Com.Aurora.AuWeather.ViewModels.Events;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Com.Aurora.AuWeather.ViewModels;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace Com.Aurora.AuWeather.SettingOptions
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class PreferencesSetting : Page
    {
        public PreferencesSetting()
        {
            this.InitializeComponent();
            Context.FetchDataComplete += Context_FetchDataComplete;
            App.Current.Suspending += Current_Suspending;
        }

        private void Current_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            Context.SaveAll();
        }

        private void Context_FetchDataComplete(object sender, FetchDataCompleteEventArgs e)
        {
            var task = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, new Windows.UI.Core.DispatchedHandler(() =>
               {
                   Temp.ItemsSource = Context.Temperature;
                   Wind.ItemsSource = Context.Wind;
                   Speed.ItemsSource = Context.Speed;
                   Length.ItemsSource = Context.Length;
                   Pressure.ItemsSource = Context.Pressure;
                   Year.ItemsSource = Context.Year;
                   Month.ItemsSource = Context.Month;
                   Day.ItemsSource = Context.Day;
                   Hour.ItemsSource = Context.Hour;
                   Minute.ItemsSource = Context.Minute;

                   Separator0.PlaceholderText = Context.Separator;
                   Separator1.PlaceholderText = Context.Separator;

                   UseWeekDay.IsOn = Context.UseWeekDay;
                   Showtt.IsOn = Context.Showtt;
                   EnableSecond.IsOn = Context.EnableSecond;
                   ShowImmersivett.IsOn = Context.ShowImmersivett;
                   DisableDynamic.IsOn = Context.DisableDynamic;
                   EnableEveryDay.IsOn = Context.EnableEveryDay;
                   EnableAlarm.IsOn = Context.EnableAlarm;
                   DisableDynamic.IsOn = Context.DisableDynamic;

                   Temp.SelectedIndex = Context.Temperature.SelectedIndex;
                   Wind.SelectedIndex = Context.Wind.SelectedIndex;
                   Speed.SelectedIndex = Context.Speed.SelectedIndex;
                   Length.SelectedIndex = Context.Length.SelectedIndex;
                   Pressure.SelectedIndex = Context.Pressure.SelectedIndex;
                   Year.SelectedIndex = Context.Year.SelectedIndex;
                   Hour.SelectedIndex = Context.Hour.SelectedIndex;
                   Month.SelectedIndex = Context.Month.SelectedIndex;
                   Day.SelectedIndex = Context.Day.SelectedIndex;
                   Minute.SelectedIndex = Context.Minute.SelectedIndex;

                   Temp.SelectionChanged += Enum_SelectionChanged;
                   Wind.SelectionChanged += Enum_SelectionChanged;
                   Speed.SelectionChanged += Enum_SelectionChanged;
                   Length.SelectionChanged += Enum_SelectionChanged;
                   Pressure.SelectionChanged += Enum_SelectionChanged;
                   Year.SelectionChanged += Format_SelectionChanged;
                   Month.SelectionChanged += Format_SelectionChanged;
                   Day.SelectionChanged += Format_SelectionChanged;
                   Hour.SelectionChanged += Format_SelectionChanged;
                   Minute.SelectionChanged += Format_SelectionChanged;
               }));
        }

        private void Format_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Context.SetFormatValue((sender as ComboBox).Name, (sender as ComboBox).SelectedItem as string);
        }

        private void Enum_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Context.SetEnumValue(((sender as ComboBox).SelectedItem as EnumSelector).Value);
        }

        private void Separator0_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Separator1.Text != Separator0.Text)
            {
                Separator1.Text = Separator0.Text;
                if (Separator0.Text.Length == 0)
                {
                    Context.SetSeparator(Separator0.PlaceholderText);
                }
                else
                {
                    Context.SetSeparator(Separator0.Text);
                }
            }
        }

        private void Separator1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Separator0.Text != Separator1.Text)
            {
                Separator0.Text = Separator1.Text;
                if (Separator1.Text.Length == 0)
                {
                    Context.SetSeparator(Separator1.PlaceholderText);
                }
                else
                {
                    Context.SetSeparator(Separator1.Text);
                }
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Context.SaveAll();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            Context.SaveAll();
        }

        private void Showtt_Toggled(object sender, RoutedEventArgs e)
        {
            Context.Settt(Showtt.IsOn);
        }

        private void Bool_Toggled(object sender, RoutedEventArgs e)
        {
            Context.SetBool((sender as ToggleSwitch).Name, (sender as ToggleSwitch).IsOn);
        }
    }
}