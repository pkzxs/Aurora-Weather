﻿// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.AuWeather.ViewModels.Events;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Com.Aurora.AuWeather.ViewModels;
using System.Threading.Tasks;
using Com.Aurora.AuWeather.Models;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace Com.Aurora.AuWeather.SettingOptions
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class PreferencesSetting : Page
    {
        private bool rootIsWideState;
        private License.License license;

        public PreferencesSetting()
        {
            this.InitializeComponent();
            Context.FetchDataComplete += Context_FetchDataComplete;
            App.Current.Suspending += Current_Suspending;
            license = new License.License();
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
                   Week.ItemsSource = Context.Week;
                   RefreshFreq.ItemsSource = Context.RefreshFreq;
                   Theme.ItemsSource = Context.Theme;

                   Separator0.PlaceholderText = Context.Separator;
                   Separator1.PlaceholderText = Context.Separator;

                   UseWeekDay.IsOn = Context.UseWeekDay;
                   Showtt.IsOn = Context.Showtt;
                   EnableSecond.IsOn = Context.EnableSecond;
                   ShowImmersivett.IsOn = Context.ShowImmersivett;
                   DisableDynamic.IsOn = Context.DisableDynamic;
                   EnableEveryDay.IsOn = Context.EnableEveryDay;
                   EnableAlarm.IsOn = Context.EnableAlarm;
                   EnablePulltoRefresh.IsOn = Context.EnablePulltoRefresh;
                   ThemeasRiseSet.IsOn = Context.ThemeasRiseSet;
                   EnableFullScreen.IsOn = Context.EnableFullScreen;
                   AlwaysShowBackground.IsOn = Context.AlwaysShowBackground;

                   UseWeekDay.Toggled += Bool_Toggled;
                   Showtt.Toggled += Showtt_Toggled;
                   EnableSecond.Toggled += Bool_Toggled;
                   ShowImmersivett.Toggled += Bool_Toggled;
                   DisableDynamic.Toggled += Bool_Toggled;
                   EnableEveryDay.Toggled += Bool_Toggled;
                   EnableAlarm.Toggled += Bool_Toggled;
                   EnablePulltoRefresh.Toggled += Bool_Toggled;
                   ThemeasRiseSet.Toggled += ThemeasRiseSet_Toggled;
                   EnableFullScreen.Toggled += Bool_Toggled;
                   AlwaysShowBackground.Toggled += Bool_Toggled;

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
                   Week.SelectedIndex = Context.Week.SelectedIndex;
                   Theme.SelectedIndex = Context.Theme.SelectedIndex;
                   RefreshFreq.SelectedIndex = Context.RefreshFreq.SelectedIndex;

                   StartPicker.Time = Context.StartTime;
                   EndPicker.Time = Context.EndTime;

                   Temp.SelectionChanged += Enum_SelectionChanged;
                   Wind.SelectionChanged += Enum_SelectionChanged;
                   Speed.SelectionChanged += Enum_SelectionChanged;
                   Length.SelectionChanged += Enum_SelectionChanged;
                   Pressure.SelectionChanged += Enum_SelectionChanged;
                   Theme.SelectionChanged += Theme_SelectionChanged;
                   RefreshFreq.SelectionChanged += Enum_SelectionChanged;
                   Year.SelectionChanged += Format_SelectionChanged;
                   Month.SelectionChanged += Format_SelectionChanged;
                   Day.SelectionChanged += Format_SelectionChanged;
                   Hour.SelectionChanged += Format_SelectionChanged;
                   Minute.SelectionChanged += Format_SelectionChanged;
                   Week.SelectionChanged += Format_SelectionChanged;

                   StartPicker.TimeChanged += StartPicker_TimeChanged;
                   EndPicker.TimeChanged += EndPicker_TimeChanged;

                   if (Context.Theme.SelectedIndex == 1)
                   {
                       AutoThemeSwitch.Visibility = Visibility.Visible;
                       if (!Context.ThemeasRiseSet)
                       {
                           StartThemeSwitch.Visibility = Visibility.Visible;
                           EndThemeSwitch.Visibility = Visibility.Visible;
                       }

                   }

                   switch ((DataSource)Context.Data[Context.Data.SelectedIndex].Value)
                   {
                       case DataSource.HeWeather:
                           HeWeatherRadio.IsChecked = true;
                           break;
                       case DataSource.Caiyun:
                           CaiyunRadio.IsChecked = true;
                           break;
                       default:
                           HeWeatherRadio.IsChecked = true;
                           break;
                   }

                   if (!license.IsPurchased)
                   {
                       EnableEveryDay.IsOn = false;
                       EnableEveryDay.IsEnabled = false;
                       EnableAlarm.IsOn = false;
                       EnableAlarm.IsEnabled = false;
                       RefreshFreq.IsEnabled = false;
                       RefreshFreq.SelectedIndex = 0;
                       LockText.Visibility = Visibility.Visible;
                   }
                   CaiyunRadio.Checked += CaiyunRadio_Checked;
                   HeWeatherRadio.Checked += HeWeatherRadio_Checked;
               }));
        }

        private void Theme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Context.SetEnumValue(((sender as ComboBox).SelectedItem as EnumSelector).Value);
            if ((RequestedTheme)(((sender as ComboBox).SelectedItem as EnumSelector).Value) == Models.RequestedTheme.Auto)
            {
                AutoThemeSwitch.Visibility = Visibility.Visible;
                if (!Context.ThemeasRiseSet)
                {
                    StartThemeSwitch.Visibility = Visibility.Visible;
                    EndThemeSwitch.Visibility = Visibility.Visible;
                }
            }
            else
            {
                AutoThemeSwitch.Visibility = Visibility.Collapsed;
                StartThemeSwitch.Visibility = Visibility.Collapsed;
                EndThemeSwitch.Visibility = Visibility.Collapsed;
            }
            Context.ReloadTheme();
            (((this.Parent as Frame).Parent as Grid).Parent as SettingOptionsPage).ReloadTheme();
        }

        private async void Format_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Context.SetFormatValue((sender as ComboBox).Name, (sender as ComboBox).SelectedItem as string);
            await Task.Delay(1000);
            ((Window.Current.Content as Frame).Content as MainPage).ReCalcPaneFormat();
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

        private void ThemeasRiseSet_Toggled(object sender, RoutedEventArgs e)
        {
            Context.SetBool("ThemeasRiseSet", (sender as ToggleSwitch).IsOn);
            if ((sender as ToggleSwitch).IsOn)
            {
                StartThemeSwitch.Visibility = Visibility.Collapsed;
                EndThemeSwitch.Visibility = Visibility.Collapsed;
            }
            else
            {
                StartThemeSwitch.Visibility = Visibility.Visible;
                EndThemeSwitch.Visibility = Visibility.Visible;
            }
            Context.ReloadTheme();
            (((this.Parent as Frame).Parent as Grid).Parent as SettingOptionsPage).ReloadTheme();
        }
        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ActualWidth >= 720)
            {
                RootGotoWideState();
            }
            else
            {
                RootGotoNarrowState();
            }
        }

        private void RootGotoNarrowState()
        {
            if (rootIsWideState)
            {
                RightPanel.Children.Remove(RightPanelChild);
                LeftPanel.Children.Insert(1, RightPanelChild);
                rootIsWideState = false;
            }
        }

        private void RootGotoWideState()
        {
            if (!rootIsWideState)
            {
                LeftPanel.Children.Remove(RightPanelChild);
                RightPanel.Children.Insert(0, RightPanelChild);
                rootIsWideState = true;
            }
        }

        private void StartPicker_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
        {
            Context.SetStart(e.NewTime);
            Context.ReloadTheme();
            (((this.Parent as Frame).Parent as Grid).Parent as SettingOptionsPage).ReloadTheme();
        }

        private void EndPicker_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
        {
            Context.SetEnd(e.NewTime);
            Context.ReloadTheme();
            (((this.Parent as Frame).Parent as Grid).Parent as SettingOptionsPage).ReloadTheme();
        }

        private async void CaiyunRadio_Checked(object sender, RoutedEventArgs e)
        {
            await Context.SetSource(DataSource.Caiyun);
        }

        private async void HeWeatherRadio_Checked(object sender, RoutedEventArgs e)
        {
            await Context.SetSource(DataSource.HeWeather);
        }
    }
}
