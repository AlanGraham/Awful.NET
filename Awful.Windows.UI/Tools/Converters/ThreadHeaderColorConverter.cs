﻿// <copyright file="ThreadHeaderColorConverter.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Awful.Windows.UI.Tools.Converters
{
    /// <summary>
    /// Convert Thread Header Color.
    /// </summary>
    public class ThreadHeaderColorConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var item = (bool)value;
            if (item)
            {
                return new SolidColorBrush(Colors.Yellow);
            }

            return new SolidColorBrush(Color.FromArgb(255, 65, 91, 100));
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}