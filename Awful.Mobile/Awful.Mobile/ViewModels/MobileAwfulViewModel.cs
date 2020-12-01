﻿// <copyright file="MobileAwfulViewModel.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using Autofac;
using Awful.Core.Tools;
using Awful.Database.Context;
using Awful.Mobile.Pages;
using Awful.UI.Actions;
using Awful.UI.ViewModels;
using Xamarin.Forms;

namespace Awful.Mobile.ViewModels
{
    /// <summary>
    /// Mobile Awful View Model.
    /// Used as the base for other View Models in the app.
    /// Contains base level functions like handling windows.
    /// </summary>
    public class MobileAwfulViewModel : AwfulViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileAwfulViewModel"/> class.
        /// </summary>
        /// <param name="context">Awful Context.</param>
        public MobileAwfulViewModel(AwfulContext context)
            : base(context)
        {
            this.SettingsAction = new SettingsAction(context);
        }

        /// <summary>
        /// Gets a value indicating whether the device
        /// is a "large" format (Tablet, Desktop)
        /// or not.
        /// </summary>
        public static bool IsLargeDevice => false;

        /// <summary>
        /// Gets the Settings Actions. Used to handle settings.
        /// </summary>
        public SettingsAction SettingsAction { get; internal set; }

        /// <summary>
        /// Starts up app with correct page.
        /// Checks if the user exists in the database.
        /// If so, go to main page.
        /// Else, go to login.
        /// </summary>
        /// <param name="platformProperties">Platform Properties.</param>
        public static void StartApp(IPlatformProperties platformProperties)
        {
            if (platformProperties == null)
            {
                throw new ArgumentNullException(nameof(platformProperties));
            }

            var user = System.IO.File.Exists(platformProperties.CookiePath);
            if (!user)
            {
                App.Current.MainPage = new LoginPage();
            }
            else
            {
                SetMainAppPage();
            }
        }

        /// <summary>
        /// Close Modal Async.
        /// </summary>
        /// <returns>A <see cref="Task"/>.</returns>
        public static Task PopModalAsync()
        {
            if (IsLargeDevice)
            {
                FlyoutPage flyout = (FlyoutPage)App.Current.MainPage;
                Page mp = (Page)flyout.Detail;
                Device.BeginInvokeOnMainThread(async () => await mp.Navigation.PopModalAsync().ConfigureAwait(false));
            }
            else
            {
                TabbedPage tabbedPage = (TabbedPage)App.Current.MainPage;
                Device.BeginInvokeOnMainThread(async () => await tabbedPage.CurrentPage.Navigation.PopModalAsync().ConfigureAwait(false));
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Display Alert to User.
        /// </summary>
        /// <param name="title">Title of message.</param>
        /// <param name="message">Message to user.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public static Task DisplayAlertAsync(string title, string message)
        {
            Device.BeginInvokeOnMainThread(async () => await App.Current.MainPage.DisplayAlert(title, message, "Close").ConfigureAwait(false));
            return Task.CompletedTask;
        }

        /// <summary>
        /// Push Modal Page to current navigation stack.
        /// If on tablet, pushes on top of Detail.
        /// </summary>
        /// <param name="page">Page to navigate to.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public static Task PushModalAsync(Page page)
        {
            if (IsLargeDevice)
            {
                FlyoutPage flyout = (FlyoutPage)App.Current.MainPage;
                Page mp = (Page)flyout.Detail;
                Device.BeginInvokeOnMainThread(async () => await mp.Navigation.PushModalAsync(page).ConfigureAwait(false));
            }
            else
            {
                TabbedPage tabbedPage = (TabbedPage)App.Current.MainPage;
                Device.BeginInvokeOnMainThread(async () => await tabbedPage.CurrentPage.Navigation.PushModalAsync(page).ConfigureAwait(false));
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Push Page to current navigation stack.
        /// If on tablet, pushes on top of Master.
        /// </summary>
        /// <param name="page">Page to navigate to.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public static Task PushPageAsync(Page page)
        {
            if (IsLargeDevice)
            {
                FlyoutPage flyout = (FlyoutPage)App.Current.MainPage;
                if (flyout.Flyout is NavigationPage navPage)
                {
                    if (navPage.CurrentPage is TabbedPage tb)
                    {
                        Device.BeginInvokeOnMainThread(async () => await tb.CurrentPage.Navigation.PushAsync(page).ConfigureAwait(false));
                    }
                }
            }
            else
            {
                TabbedPage tabbedPage = (TabbedPage)App.Current.MainPage;
                Device.BeginInvokeOnMainThread(async () => await tabbedPage.CurrentPage.Navigation.PushAsync(page).ConfigureAwait(false));
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Set Detail Page for Master Detail if on Tablet, else push navigation.
        /// </summary>
        /// <param name="page">Page to navigate to.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public static Task PushDetailPageAsync(Page page)
        {
            if (IsLargeDevice)
            {
                FlyoutPage flyout = (FlyoutPage)App.Current.MainPage;
                flyout.Detail = new NavigationPage(page);
                if (flyout.FlyoutLayoutBehavior == FlyoutLayoutBehavior.Popover)
                {
                    Device.BeginInvokeOnMainThread(() => flyout.IsPresented = false);
                }
            }
            else
            {
                TabbedPage tabbedPage = (TabbedPage)App.Current.MainPage;
                Device.BeginInvokeOnMainThread(async () => await tabbedPage.CurrentPage.Navigation.PushAsync(page).ConfigureAwait(false));
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Refresh post page.
        /// Used on Thread/PM Post pages to callback
        /// to the last page.
        /// </summary>
        /// <returns>A <see cref="Task"/>.</returns>
        public static async Task RefreshForumPageAsync()
        {
            ForumThreadListPageViewModel vm = null;
            if (IsLargeDevice)
            {
                FlyoutPage flyout = (FlyoutPage)App.Current.MainPage;
                Page mp = (Page)flyout.Detail;
                vm = mp.BindingContext as ForumThreadListPageViewModel;
            }
            else
            {
                TabbedPage tabbedPage = (TabbedPage)App.Current.MainPage;
                var navigationPage = tabbedPage.CurrentPage as NavigationPage;
                if (navigationPage != null)
                {
                    vm = navigationPage.CurrentPage.BindingContext as ForumThreadListPageViewModel;
                }
            }

            if (vm != null)
            {
                await vm.RefreshForums().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Refresh post page.
        /// </summary>
        /// <returns>A <see cref="Task"/>.</returns>
        public static async Task RefreshPostPageAsync()
        {
            ForumThreadPageViewModel vm = null;
            if (IsLargeDevice)
            {
                FlyoutPage flyout = (FlyoutPage)App.Current.MainPage;
                Page mp = (Page)flyout.Detail;
                vm = mp.BindingContext as ForumThreadPageViewModel;
            }
            else
            {
                TabbedPage tabbedPage = (TabbedPage)App.Current.MainPage;
                var navigationPage = tabbedPage.CurrentPage as NavigationPage;
                if (navigationPage != null)
                {
                    vm = navigationPage.CurrentPage.BindingContext as ForumThreadPageViewModel;
                }
            }

            if (vm != null)
            {
                await vm.RefreshThreadAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Sets the main page of the application.
        /// Runs async on UI thread.
        /// </summary>
        /// <returns>A <see cref="Task"/>.</returns>
        public static Task SetMainAppPageAsync()
        {
            Device.BeginInvokeOnMainThread(() => SetMainAppPage());
            return Task.CompletedTask;
        }

        /// <summary>
        /// Sets the main page of the application.
        /// Based on the device type.
        /// MasterPage is the tabbed view.
        /// MainPage is a Flyout containing the MasterPage,
        /// And the detail page on the same screen.
        /// </summary>
        public static void SetMainAppPage()
        {
            if (MobileAwfulViewModel.IsLargeDevice)
            {
                App.Current.MainPage = new MainPage();
            }
            else
            {
                App.Current.MainPage = new MasterPage();
            }
        }

        /// <summary>
        /// Setup the theme of the app on load.
        /// </summary>
        /// <returns>A <see cref="Task"/>.</returns>
        public async Task SetupThemeAsync()
        {
            var options = await this.SettingsAction.LoadSettingOptionsAsync().ConfigureAwait(false);
            if (options != null)
            {
                this.SettingsAction.SetAppTheme(options.DeviceColorTheme);
            }
            else
            {
                this.SettingsAction.SetAppTheme(Webview.Entities.Themes.DeviceColorTheme.Light);
            }
        }
    }
}
