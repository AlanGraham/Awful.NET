﻿// <copyright file="BaseViewModel.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace Awful.UI.ViewModels
{
    /// <summary>
    /// Base View Model.
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        private LayoutState currentState;
        private string customState;
        private bool isBusy = false;
        private bool isRefreshing = false;
        private string title = string.Empty;
        private string loadingText = "Loading...";

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the current state of the view.
        /// </summary>
        public LayoutState CurrentState
        {
            get { return this.currentState; }
            set { this.SetProperty(ref this.currentState, value); }
        }

        /// <summary>
        /// Gets or sets the current state of the view.
        /// </summary>
        public string CustomState
        {
            get { return this.customState; }
            set { this.SetProperty(ref this.customState, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the view is busy.
        /// </summary>
        public bool IsBusy
        {
            get { return this.isBusy; }
            set { this.SetProperty(ref this.isBusy, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the view is refreshing.
        /// </summary>
        public bool IsRefreshing
        {
            get { return this.isRefreshing; }
            set { this.SetProperty(ref this.isRefreshing, value); }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title
        {
            get { return this.title; }
            set { this.SetProperty(ref this.title, value); }
        }

        /// <summary>
        /// Gets or sets the loading text.
        /// </summary>
        public string LoadingText
        {
            get { return this.loadingText; }
            set { this.SetProperty(ref this.loadingText, value); }
        }

        /// <summary>
        /// Set State.
        /// </summary>
        /// <param name="state">State.</param>
        /// <param name="customState">Custom State.</param>
        public void SetState(LayoutState state, string customState = "")
        {
            this.CurrentState = state;
            if (state == LayoutState.Custom)
            {
                this.CustomState = customState;
            }
            else
            {
                this.CustomState = string.Empty;
            }
        }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
            {
                return false;
            }

            backingStore = value;
            onChanged?.Invoke();
            this.OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// On Property Changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    var changed = this.PropertyChanged;
                    if (changed == null)
                    {
                        return;
                    }

                    changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }
                catch
                {

                }
            });
        }
    }
}
