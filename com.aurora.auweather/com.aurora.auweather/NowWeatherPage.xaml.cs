﻿// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.AuWeather.Models;
using Com.Aurora.AuWeather.Models.Settings;
using Com.Aurora.AuWeather.ViewModels.Events;
using Com.Aurora.Shared.Converters;
using Com.Aurora.Shared.Extensions;
using Com.Aurora.Shared.Helpers;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.System.Threading;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Com.Aurora.AuWeather
{
    public sealed partial class NowWeatherPage : Page
    {
        private const double FENGCHE_ZHUANSU = 0.1396263377777778;
        private const int WEATHERCANVAS_HEADEROFFSET = 528;
        private const int FIXED_TITLE_FONTSIZE = 96;
        private const int WEATHER_BEZIER_SCROLLOFFSET = 200;
        private const int NORMAL_SIZE_WIDTH = 720;
        private const int WIDE_SIZE_WIDTH = 864;
        private double verticalOffset;
        private double actualWidth;
        private bool isAnimating = false;
        private bool animated = false;
        private bool isFadeOut = false;
        /// <summary>
        /// use binary: 1111 1111 to implement every DetailGrid Animation status
        /// </summary>
        private int detailGridAnimation_FLAG = 0;

        private Point[] DetailGridPoint = new Point[9];

        public double SunRiseStrokeLength
        {
            get { return (double)GetValue(SunRiseStrokeLengthProperty); }
            set { SetValue(SunRiseStrokeLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SunRiseStrokeLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SunRiseStrokeLengthProperty =
            DependencyProperty.Register("SunRiseStrokeLength", typeof(double), typeof(NowWeatherPage), new PropertyMetadata(0d));


        public double AqiCircleStorkeLength
        {
            get { return (double)GetValue(AqiCircleStorkeLengthProperty); }
            set { SetValue(AqiCircleStorkeLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AqiCircleStorkeLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AqiCircleStorkeLengthProperty =
            DependencyProperty.Register("AqiCircleStorkeLength", typeof(double), typeof(NowWeatherPage), new PropertyMetadata(0d));

        public bool DetailsPanelIsNormalState = true;
        private bool rootIsWideState = false;
        private ThreadPoolTimer fengcheTimer;
        private ThreadPoolTimer immersiveTimer;
        private bool isImmersiveMode = false;
        private bool isImmersiveAllIn = false;
        private MainPage baba;

        public NowWeatherPage()
        {
            this.InitializeComponent();
            Context.FetchDataComplete += MModel_FetchDataComplete;
            Context.ParameterChanged += MModel_ParameterChanged;
            Context.FetchDataFailed += Context_FetchDataFailed;
            Context.TimeUpdated += Context_TimeUpdated;
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
            //DataContext = new NowWeatherPageViewModel();
        }

        private void Context_TimeUpdated(object sender, TimeUpdatedEventArgs e)
        {
            detailGridAnimation_FLAG -= 16;
            DetailGrid4Play();
            baba.ReloadTheme();
            if (e.IsDayNightChanged)
            {
                if (Context.Condition == (WeatherCondition.sunny | WeatherCondition.windy
                | WeatherCondition.calm |
                WeatherCondition.light_breeze |
                WeatherCondition.moderate |
                WeatherCondition.fresh_breeze |
                WeatherCondition.strong_breeze |
                WeatherCondition.high_wind |
                WeatherCondition.gale))
                {
                    WeatherCanvas.ChangeCondition(Context.Condition, Context.IsNight, Context.IsSummer);
                    if (!Context.IsNight)
                    {
                        SolidColorBrush s;
                        if (Context.Theme == ElementTheme.Dark)
                        {
                            var d = this.Resources.ThemeDictionaries["Dark"] as ResourceDictionary;
                            s = (SolidColorBrush)d["SystemControlForegroundBaseHighBrush"];
                        }
                        else
                        {
                            s = (SolidColorBrush)Resources["SystemControlForegroundBaseHighBrush"];
                        }
                        baba.ChangeColor(s);
                    }
                    else
                    {
                        baba.ChangeColor(new SolidColorBrush(Colors.White));
                    }
                }
            }
        }

        private async void Context_FetchDataFailed(object sender, FetchDataFailedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, new Windows.UI.Core.DispatchedHandler(async () =>
             {
                 var loader = new ResourceLoader();
                 var d = new MessageDialog(e.Message);
                 d.Title = loader.GetString("Error");
                 if (e.Message == loader.GetString("Cities_null"))
                 {
                     d.Commands.Add(new UICommand(loader.GetString("Setting"), new UICommandInvokedHandler(NavigateToSettings)));

                 }
                 else
                 {
                     d.Commands.Add(new UICommand(loader.GetString("Refresh"), new UICommandInvokedHandler(DataFailed_Refresh)));
                 }
                 d.Commands.Add(new UICommand(loader.GetString("Quit"), new UICommandInvokedHandler(QuitAll)));
                 await d.ShowAsync();
             }));
        }

        private void DataFailed_Refresh(IUICommand command)
        {
            Context.RefreshAsync();
        }

        private void QuitAll(IUICommand command)
        {
            App.Current.Exit();
        }

        private void NavigateToSettings(IUICommand command)
        {
            baba.Navigate(typeof(SettingsPage));
        }

        private async void Current_Resuming(object sender, object e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, new Windows.UI.Core.DispatchedHandler(() =>
              {
                  Context.RefreshCurrentTime();
                  Context.CurrentTimeRefreshTask();
              }));
        }

        private void MModel_ParameterChanged(object sender, ParameterChangedEventArgs e)
        {

        }

        internal void Refresh()
        {
            Context.RefreshAsync();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            baba = e.Parameter as MainPage;
            baba.ChangeColor(Colors.Transparent, Colors.White, new SolidColorBrush(Colors.White));
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            this.Page_Unloaded(null, null);
        }

        private async void MModel_FetchDataComplete(object sender, FetchDataCompleteEventArgs e)
        {
            var loader = new ResourceLoader();
            detailGridAnimation_FLAG = 0;
            CalcDetailGridPosition();
            isAnimating = true;
            animated = true;
            ScrollableRoot.ViewChanged += ScrollableRoot_ViewChanged;
            UpdateIndicator.Text = loader.GetString("RefreshComplete");
            TempraturePathAnimation.Completed += (s, v) =>
            {
                isAnimating = false;
                RefreshCompleteAni.Begin();
            };
            TempraturePathAnimation.Begin();
            AQIAni.Begin();
            if (rootIsWideState)
            {
                DetailGrid0Play();
                DetailGrid1Play();
                DetailGrid2Play();
                DetailGrid3Play();
                DetailGrid4Play();
                DetailGrid6Play();
                DetailGrid7Play();
                DetailGrid8Play();
            }
            Forecast0.SetCondition(Context.Forecast0, Context.IsNight);
            Forecast1.SetCondition(Context.Forecast1, Context.IsNight);
            Forecast2.SetCondition(Context.Forecast2, Context.IsNight);
            Forecast3.SetCondition(Context.Forecast3, Context.IsNight);
            Forecast4.SetCondition(Context.Forecast4, Context.IsNight);
            if (Context.Condition == (WeatherCondition.sunny | WeatherCondition.windy
                | WeatherCondition.calm |
                WeatherCondition.light_breeze |
                WeatherCondition.moderate |
                WeatherCondition.fresh_breeze |
                WeatherCondition.strong_breeze |
                WeatherCondition.high_wind |
                WeatherCondition.gale) && !Context.IsNight)
            {
                SolidColorBrush s;
                if (Context.Theme == ElementTheme.Dark)
                {
                    var d = this.Resources.ThemeDictionaries["Dark"] as ResourceDictionary;
                    s = (SolidColorBrush)d["SystemControlForegroundBaseHighBrush"];
                }
                else
                {
                    s = (SolidColorBrush)Resources["SystemControlForegroundBaseHighBrush"];
                }
                baba.ChangeColor(s);
            }
            else
            {
                baba.ChangeColor(new SolidColorBrush(Colors.White));
            }
            WeatherCanvas.ChangeCondition(Context.Condition, Context.IsNight, Context.IsSummer);
            if (Context.Aqi == null)
            {
                AQIPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                AQIPanel.Visibility = Visibility.Visible;
            }
            if (Context.Comf == null && Context.Cw == null && Context.Drsg == null)
            {
                SuggestionPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                SuggestionPanel.Visibility = Visibility.Visible;
            }
            await Task.Delay(1000);
            ScrollableRoot.RefreshComplete();
        }

        #region DetailGrid Animation
        private void DetailGrid0Play()
        {
            if ((detailGridAnimation_FLAG & 1) == 0)
            {
                DetailTempratureIn.Begin();
                detailGridAnimation_FLAG++;
            }
        }
        private void DetailGrid1Play()
        {
            if (isImmersiveMode)
            {
                return;
            }
            if ((detailGridAnimation_FLAG & 2) == 0)
            {
                if (fengcheTimer != null)
                {
                    fengcheTimer.Cancel();
                }
                fengcheTimer = ThreadPoolTimer.CreatePeriodicTimer((work) =>
                                  {
                                      if (!Context.EnableDynamic)
                                      {
                                          return;
                                      }
                                      var task = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() =>
                                                                         {
                                                                             fengchezhuan.Angle += FENGCHE_ZHUANSU * Context.Wind.Speed.MPS;
                                                                         }));

                                  }, TimeSpan.FromMilliseconds(16));
                detailGridAnimation_FLAG += 2;
            }
        }
        private void DetailGrid2Play()
        {
            if ((detailGridAnimation_FLAG & 4) == 0)
            {
                WaterDropTransAni.Begin();
                detailGridAnimation_FLAG += 4;
            }
        }
        private void DetailGrid3Play()
        {
            if ((detailGridAnimation_FLAG & 8) == 0)
            {
                PcpnTransAni.Begin();
                detailGridAnimation_FLAG += 8;
            }
        }
        private void DetailGrid4Play()
        {
            if ((detailGridAnimation_FLAG & 16) == 0)
            {
                SunRiseAni.Begin();
                detailGridAnimation_FLAG += 16;
            }
        }
        private void DetailGrid6Play()
        {
            if ((detailGridAnimation_FLAG & 32) == 0)
            {
                VisTransAni.Begin();
                detailGridAnimation_FLAG += 32;
            }
        }
        private void DetailGrid7Play()
        {
            if ((detailGridAnimation_FLAG & 64) == 0)
            {
                PressureTransAni.Begin();
                detailGridAnimation_FLAG += 64;
            }
        }
        private void DetailGrid8Play()
        {
            if ((detailGridAnimation_FLAG & 128) == 0)
            {
                detailGridAnimation_FLAG += 128;
            }
        }
        #endregion

        #region Hold Bezier
        private void RelativePanel_LayoutUpdated(object sender, object e)
        {
            if (actualWidth != ScrollableRoot.ActualWidth || isAnimating)
            {
                actualWidth = ScrollableRoot.ActualWidth;
                SetPathPoint1(BezierControl1, actualWidth, 1f / 21f);
                SetPathPoint2(BezierControl1, actualWidth, 2f / 21f);
                SetPathPoint3(BezierControl1, actualWidth, 3f / 21f);
                SetPathPoint1(BezierControl2, actualWidth, 4f / 21f);
                SetPathPoint2(BezierControl2, actualWidth, 5f / 21f);
                SetPathPoint3(BezierControl2, actualWidth, 6f / 21f);
                SetPathPoint1(BezierControl3, actualWidth, 7f / 21f);
                SetPathPoint2(BezierControl3, actualWidth, 8f / 21f);
                SetPathPoint3(BezierControl3, actualWidth, 9f / 21f);
                SetPathPoint1(BezierControl4, actualWidth, 10f / 21f);
                SetPathPoint2(BezierControl4, actualWidth, 11f / 21f);
                SetPathPoint3(BezierControl4, actualWidth, 12f / 21f);
                SetPathPoint1(BezierControl5, actualWidth, 13f / 21f);
                SetPathPoint2(BezierControl5, actualWidth, 14f / 21f);
                SetPathPoint3(BezierControl5, actualWidth, 15f / 21f);
                SetPathPoint1(BezierControl6, actualWidth, 16f / 21f);
                SetPathPoint2(BezierControl6, actualWidth, 17f / 21f);
                SetPathPoint3(BezierControl6, actualWidth, 18f / 21f);
                SetPathPoint1(BezierControl7, actualWidth, 19f / 21f);
                SetPathPoint2(BezierControl7, actualWidth, 20f / 21f);
                SetPathPoint3(BezierControl7, actualWidth, 1);
                SetEndPoint(endPoint1, actualWidth);
            }

            if (verticalOffset != ScrollableRoot.VerticalOffset && ScrollableRoot.VerticalOffset < 536)
            {
                verticalOffset = ScrollableRoot.VerticalOffset;
                var offset = verticalOffset > WEATHERCANVAS_HEADEROFFSET ? WEATHERCANVAS_HEADEROFFSET : verticalOffset;
                offset /= WEATHERCANVAS_HEADEROFFSET;
                offset = EasingHelper.CircleEase(Windows.UI.Xaml.Media.Animation.EasingMode.EaseOut, offset);
                NowTemp.FontSize = FIXED_TITLE_FONTSIZE - 48 * offset;
                var horizotaloffset = ButtonOffset.Visibility == Visibility.Visible ? 72 : 0;
                TempAniTrans.X = -(actualWidth - NowTemp.ActualWidth - horizotaloffset - 32) * offset / 2;
                if (verticalOffset > 2 && !isFadeOut)
                {
                    isFadeOut = true;
                    TempratureOut.Begin();
                }
                else if (verticalOffset < 2 && isFadeOut)
                {
                    isFadeOut = false;
                    TempratureIn.Begin();
                }
                ScrollPathPoint(verticalOffset);
            }
        }

        private void SetEndPoint(LineSegment endPoint1, double actualWidth)
        {
            var p = endPoint1.Point;
            p.X = actualWidth;
            endPoint1.Point = p;
        }
        private void SetPathPoint1(BezierSegment control, double actualWidth, float v)
        {
            var p = control.Point1;
            p.X = actualWidth * v;
            control.Point1 = p;
        }
        private void SetPathPoint2(BezierSegment control, double actualWidth, float v)
        {
            var p = control.Point2;
            p.X = actualWidth * v;
            control.Point2 = p;
        }
        private void SetPathPoint3(BezierSegment control, double actualWidth, float v)
        {
            var p = control.Point3;
            p.X = actualWidth * v;
            control.Point3 = p;
        }
        #endregion

        #region Scroll Bezier
        private void ScrollPathPoint(double verticalOffset)
        {
            var offset = verticalOffset > WEATHER_BEZIER_SCROLLOFFSET ? WEATHER_BEZIER_SCROLLOFFSET : verticalOffset;
            offset = -64 * (1 - offset / WEATHER_BEZIER_SCROLLOFFSET);
            double[] results = new double[] { Context.TempraturePath0 * offset, Context.TempraturePath1 * offset, Context.TempraturePath2 * offset,
                Context.TempraturePath3 * offset, Context.TempraturePath4 * offset, Context.TempraturePath5 * offset };
            CalculateY0(offset, PathFigure, results[0]);
            CalculateY1(offset, BezierControl1, results[0]);
            CalculateY2(offset, BezierControl2, results[0], results[1]);
            CalculateY2(offset, BezierControl3, results[1], results[2]);
            CalculateY2(offset, BezierControl4, results[2], results[3]);
            CalculateY2(offset, BezierControl5, results[3], results[4]);
            CalculateY2(offset, BezierControl6, results[4], results[5]);
            CalculateY1(offset, BezierControl7, results[5]);
        }
        private void CalculateY0(double offset, PathFigure pathFigure, double result)
        {
            var p = pathFigure.StartPoint;
            p.Y = result;
            pathFigure.StartPoint = p;
        }
        private void CalculateY1(double offset, BezierSegment control, double result)
        {
            var p = control.Point1;
            p.Y = result;
            control.Point1 = p;
            p = control.Point2;
            p.Y = result;
            control.Point2 = p;
            p = control.Point3;
            p.Y = result;
            control.Point3 = p;
        }
        private void CalculateY2(double offset, BezierSegment control, double result1, double result2)
        {
            var p = control.Point1;
            p.Y = result1;
            control.Point1 = p;
            p = control.Point2;
            p.Y = result2;
            control.Point2 = p;
            p = control.Point3;
            p.Y = result2;
            control.Point3 = p;
        }
        #endregion

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Context.FetchDataComplete -= MModel_FetchDataComplete;
            Context.ParameterChanged -= MModel_ParameterChanged;
            if (immersiveTimer != null)
            {
                immersiveTimer.Cancel();
                immersiveTimer = null;
            }
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
            if (fengcheTimer != null)
            {
                fengcheTimer.Cancel();
                fengcheTimer = null;
            }
        }

        #region DetailsPanel Change Layout
        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (DetailsPanel.ActualWidth < NORMAL_SIZE_WIDTH && !DetailsPanelIsNormalState)
            {
                DetailsPanelGotoNormalState();
            }
            else if (DetailsPanel.ActualWidth >= NORMAL_SIZE_WIDTH && DetailsPanelIsNormalState)
            {
                DetailsPanelGotoWideState();
            }
        }

        private void DetailsPanelGotoNormalState()
        {
            DetailsPanelIsNormalState = true;
            var length = GridLength.Auto;
            Column2.Width = length;
            UIHelper.Change_Row_Column(DetailGrid2, 1, 0);
            UIHelper.Change_Row_Column(DetailGrid3, 1, 1);
            UIHelper.Change_Row_Column(DetailGrid4, 2, 0);
            UIHelper.Change_Row_Column(DetailGrid5, 2, 1);
            UIHelper.Change_Row_Column(DetailGrid6, 3, 0);
            UIHelper.Change_Row_Column(DetailGrid7, 3, 1);
            UIHelper.Change_Row_Column(DetailGrid8, 3, 2);
            UIHelper.ReverseVisibility(DetailGrid8);
        }

        private void DetailsPanelGotoWideState()
        {
            DetailsPanelIsNormalState = false;
            var length = new GridLength(1, GridUnitType.Star);
            Column2.Width = length;
            UIHelper.Change_Row_Column(DetailGrid2, 0, 2);
            UIHelper.Change_Row_Column(DetailGrid3, 1, 0);
            UIHelper.Change_Row_Column(DetailGrid4, 1, 1);
            UIHelper.Change_Row_Column(DetailGrid5, 1, 2);
            UIHelper.Change_Row_Column(DetailGrid6, 2, 0);
            UIHelper.Change_Row_Column(DetailGrid7, 2, 1);
            UIHelper.Change_Row_Column(DetailGrid8, 2, 2);
            UIHelper.ReverseVisibility(DetailGrid8);
        }
        #endregion

        private void CalcDetailGridPosition()
        {
            DetailGridPoint[0] = DetailGrid0.GetPositioninParent(ScrollableRoot);
            DetailGridPoint[1] = DetailGrid1.GetPositioninParent(ScrollableRoot);
            DetailGridPoint[2] = DetailGrid2.GetPositioninParent(ScrollableRoot);
            DetailGridPoint[3] = DetailGrid3.GetPositioninParent(ScrollableRoot);
            DetailGridPoint[4] = DetailGrid4.GetPositioninParent(ScrollableRoot);
            DetailGridPoint[5] = DetailGrid5.GetPositioninParent(ScrollableRoot);
            DetailGridPoint[6] = DetailGrid6.GetPositioninParent(ScrollableRoot);
            DetailGridPoint[7] = DetailGrid7.GetPositioninParent(ScrollableRoot);
            DetailGridPoint[8] = DetailGrid8.GetPositioninParent(ScrollableRoot);
            for (int i = 0; i < DetailGridPoint.Length; i++)
            {
                DetailGridPoint[i].Y += ScrollableRoot.VerticalOffset;
            }
        }

        private void ScrollableRoot_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            CalcDetailGridPosition();
            DetailGridPlay(ScrollableRoot.ActualHeight + ScrollableRoot.VerticalOffset - 240);
        }

        private void DetailGridPlay(double offsetProgress)
        {
            if (offsetProgress > DetailGridPoint[0].Y)
            {
                DetailGrid0Play();
            }
            if (offsetProgress > DetailGridPoint[1].Y)
            {
                DetailGrid1Play();
            }
            if (offsetProgress > DetailGridPoint[2].Y)
            {
                DetailGrid2Play();
            }
            if (offsetProgress > DetailGridPoint[3].Y)
            {
                DetailGrid3Play();
            }
            if (offsetProgress > DetailGridPoint[4].Y)
            {
                DetailGrid4Play();
            }
            if (offsetProgress > DetailGridPoint[6].Y)
            {
                DetailGrid6Play();
            }
            if (offsetProgress > DetailGridPoint[7].Y)
            {
                DetailGrid7Play();
            }
            if (offsetProgress > DetailGridPoint[8].Y)
            {
                DetailGrid8Play();
            }
        }

        #region Root Mode Changing
        private void Root_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if ((Window.Current.Content as Frame).ActualWidth < NORMAL_SIZE_WIDTH)
            {
                RootGotoNarrowState();
            }
            else if ((Window.Current.Content as Frame).ActualWidth < WIDE_SIZE_WIDTH)
            {
                ScrollViewerConverter.isLargeMode = true;
                RootGotoNormalState();
            }
            else if ((Window.Current.Content as Frame).ActualWidth >= WIDE_SIZE_WIDTH && !rootIsWideState)
            {
                RootGotoWideState();
            }
        }

        private void RootGotoNarrowState()
        {
            if (isImmersiveMode)
            {
                isImmersiveMode = false;
                ImmersiveBackButton_Click(null, null);
            }
            if (rootIsWideState)
            {
                rootIsWideState = false;
                LargeModeSubPanel.Content = null;
                WeatherPanel.Children.Add(DetailsPanel);
            }
            if (ScrollViewerConverter.isLargeMode)
                ScrollViewerConverter.isLargeMode = false;
            UIHelper.ChangeTitlebarButtonColor(Colors.Transparent, Colors.White);
        }

        private void RootGotoWideState()
        {
            Color c;
            if (Context.Theme == ElementTheme.Dark)
            {
                var d = this.Resources.ThemeDictionaries["Dark"] as ResourceDictionary;
                c = (Color)d["SystemBaseHighColor"];
            }
            else
            {
                c = (Color)Resources["SystemBaseHighColor"];
            }
            UIHelper.ChangeTitlebarButtonColor(Colors.Transparent, c);
            ScrollViewerConverter.isLargeMode = true;
            rootIsWideState = true;
            WeatherPanel.Children.Remove(DetailsPanel);
            LargeModeSubPanel.Content = DetailsPanel;
            if (animated)
            {
                if (fengcheTimer != null)
                {
                    fengcheTimer.Cancel();
                    detailGridAnimation_FLAG -= 2;
                }
                DetailGrid0Play();
                DetailGrid1Play();
                DetailGrid2Play();
                DetailGrid3Play();
                DetailGrid4Play();
                DetailGrid6Play();
                DetailGrid7Play();
                DetailGrid8Play();
            }
        }

        private void RootGotoNormalState()
        {
            if (rootIsWideState)
            {
                rootIsWideState = false;
                LargeModeSubPanel.Content = null;
                WeatherPanel.Children.Add(DetailsPanel);
            }
        }
        #endregion

        private async void ImmersiveButton_Click(object sender, RoutedEventArgs e)
        {
            isImmersiveMode = true;
            ImmersiveWidthIn.From = MainCanvas.ActualWidth;
            ImmersiveWidthIn.To = Root.ActualWidth;
            ImmersiveHeightIn.From = MainCanvas.ActualHeight;
            ImmersiveHeightIn.To = Root.ActualHeight;
            UIHelper.ChangeTitlebarButtonColor(Colors.Transparent, Colors.White);
            Application.Current.Resuming += Current_Resuming;
            ImmersiveTransAni.Completed += (s, args) =>
            {
                MainCanvas.Width = double.NaN;
                MainCanvas.Height = double.NaN;
                if (immersiveTimer != null)
                {
                    immersiveTimer.Cancel();

                }
                immersiveTimer = ThreadPoolTimer.CreateTimer(async (task) =>
                    {
                        var t = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, new Windows.UI.Core.DispatchedHandler(() =>
                        {
                            ImmersiveAllIn.Begin();

                            Window.Current.CoreWindow.PointerCursor = null;
                        }));
                        await Task.Delay(160);
                        isImmersiveAllIn = true;
                    }, TimeSpan.FromSeconds(1));
            };
            ImmersiveTransAni.Begin();
            if (fengcheTimer != null)
            {
                fengcheTimer.Cancel();
                detailGridAnimation_FLAG -= 2;
            }
            await Task.Delay(1000);
            WeatherCanvas.ImmersiveIn(await Context.GetCurrentBackground());
        }

        private void MainCanvas_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (isImmersiveMode)
            {
                if (isImmersiveAllIn)
                {
                    Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
                    ImmersiveAllBack.Begin();
                    isImmersiveAllIn = false;
                }
                if (immersiveTimer != null)
                {
                    immersiveTimer.Cancel();

                }
                immersiveTimer = ThreadPoolTimer.CreateTimer(async (task) =>
                    {
                        var t = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, new Windows.UI.Core.DispatchedHandler(() =>
                        {
                            ImmersiveAllIn.Begin();
                            Window.Current.CoreWindow.PointerCursor = null;
                        }));
                        await Task.Delay(160);
                        isImmersiveAllIn = true;

                    }, TimeSpan.FromSeconds(2));
            }
        }

        private void ImmersiveBackButton_Click(object sender, RoutedEventArgs e)
        {
            if (immersiveTimer != null)
            {
                immersiveTimer.Cancel();
                immersiveTimer = null;
            }
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
            ImmersiveHeightBack.From = MainCanvas.ActualHeight;
            ImmersiveWidthBack.From = MainCanvas.ActualWidth;
            ImmersiveHeightBack.To = 640 - ScrollableRoot.VerticalOffset < 160 ? 160 : 640 - ScrollableRoot.VerticalOffset;
            ImmersiveWidthBack.To = rootIsWideState ? Root.ActualWidth / 2 : Root.ActualWidth;
            App.Current.Resuming -= Current_Resuming;
            ImmersiveBackAni.Completed += (s, args) =>
            {
                if (isFadeOut)
                {
                    TempratureOut.Begin();
                }
                Binding HeightBinding = new Binding();
                HeightBinding.Source = ScrollableRoot;
                HeightBinding.Path = new PropertyPath("VerticalOffset");
                HeightBinding.Converter = new ScrollViewerConverter();
                BindingOperations.SetBinding(MainCanvas, HeightProperty, HeightBinding);
                MainCanvas.Width = double.NaN;
                DetailGrid1Play();
            };
            ImmersiveBackAni.Begin();
            isImmersiveMode = false;
            WeatherCanvas.ImmersiveOut();
        }

        private void ScrollableRoot_RefreshStart(object sender, Shared.Controls.RefreshStartEventArgs e)
        {
            var loader = new ResourceLoader();
            UpdateIndicator.Text = loader.GetString("RefreshStart");
            Context.RefreshAsync();
        }

        private void AQIDetailButton_Click(object sender, RoutedEventArgs e)
        {
            if (O3Grid.Visibility == Visibility.Collapsed)
            {
                O3Grid.Visibility = Visibility.Visible;
                NO2Grid.Visibility = Visibility.Visible;
                COGrid.Visibility = Visibility.Visible;
            }
            else
            {
                O3Grid.Visibility = Visibility.Collapsed;
                NO2Grid.Visibility = Visibility.Collapsed;
                COGrid.Visibility = Visibility.Collapsed;
            }
        }

        private void Flyout_Opened(object sender, object e)
        {
            if (immersiveTimer != null)
            {
                immersiveTimer.Cancel();
            }
        }

        private void Flyout_Closed(object sender, object e)
        {
            if (immersiveTimer != null)
            {
                immersiveTimer.Cancel();
            }
            immersiveTimer = ThreadPoolTimer.CreateTimer(async (task) =>
            {
                var t = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, new Windows.UI.Core.DispatchedHandler(() =>
                {
                    ImmersiveAllIn.Begin();

                    Window.Current.CoreWindow.PointerCursor = null;
                }));
                await Task.Delay(160);
                isImmersiveAllIn = true;
            }, TimeSpan.FromSeconds(1));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            baba.NavigatetoSettings(typeof(Cities));
            Color c;
            SolidColorBrush s;
            if (Context.Theme == ElementTheme.Dark)
            {
                var d = this.Resources.ThemeDictionaries["Dark"] as ResourceDictionary;
                c = (Color)d["SystemBaseHighColor"];
                s = (SolidColorBrush)d["SystemControlForegroundBaseHighBrush"];
            }
            else
            {
                c = (Color)Resources["SystemBaseHighColor"];
                s = (SolidColorBrush)Resources["SystemControlForegroundBaseHighBrush"];
            }
            baba.ChangeColor(Colors.Transparent, c, s);
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            var loader = new ResourceLoader();
            UpdateIndicator.Text = loader.GetString("RefreshStart");
        }
    }
}
