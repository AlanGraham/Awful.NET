﻿using System;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;

namespace Template10.Services.WindowService
{
    public class WindowService : IWindowService
    {
        public static WindowService Instance { get; } = new WindowService();

        WindowHelper _helper = new WindowHelper();

        public Task ShowAsync<T>(object param = null, ViewSizePreference size = ViewSizePreference.UseHalf) => _helper.ShowAsync<T>(param, size);
    }
}