﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Com.Aurora.Shared.Helpers
{
    public static class UIHelper
    {
        public static void SetControlPositioninGrid(DependencyObject d, int row, int column)
        {
            d.SetValue(Grid.RowProperty, row);
            d.SetValue(Grid.ColumnProperty, column);
        }

        public static void ReverseVisibility(UIElement e)
        {
            if (e.Visibility == Visibility.Collapsed)
            {
                e.Visibility = Visibility.Visible;
            }
            else
            {
                e.Visibility = Visibility.Collapsed;
            }
        }
    }
}