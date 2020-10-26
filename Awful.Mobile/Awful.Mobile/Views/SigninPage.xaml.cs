﻿// <copyright file="SigninPage.xaml.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Awful.UI.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Awful.Mobile.Views
{
    /// <summary>
    /// Sigin Page.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SigninPage : ContentPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SigninPage"/> class.
        /// </summary>
        public SigninPage()
        {
            this.InitializeComponent();
            this.BindingContext = App.Container.Resolve<SigninViewModel>();
        }
    }
}