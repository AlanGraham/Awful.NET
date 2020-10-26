﻿// <copyright file="SAclopediaEntryListViewModel.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Awful.Core.Entities.SAclopedia;
using Awful.Core.Tools;
using Awful.Database.Context;
using Awful.UI.Actions;
using Awful.UI.Entities;
using Awful.Webview;
using Xamarin.Forms;

namespace Awful.UI.ViewModels
{
    /// <summary>
    /// SAclopedia Entry List View Model.
    /// On load, if signed in, load new SAclopedia items.
    /// </summary>
    public class SAclopediaEntryListViewModel : AwfulViewModel
    {
        private SAclopediaAction saclopedia;

        /// <summary>
        /// Initializes a new instance of the <see cref="SAclopediaEntryListViewModel"/> class.
        /// </summary>
        /// <param name="handler">Awful Template Handler.</param>
        /// <param name="context">Awful Context.</param>
        public SAclopediaEntryListViewModel(TemplateHandler handler, AwfulContext context)
            : base(context)
        {
            this.saclopedia = new SAclopediaAction(this.Client, context, handler);
            if (this.IsSignedIn)
            {
                Task.Run(async () => await this.RefreshEntryList(false).ConfigureAwait(false));
            }
        }

        /// <summary>
        /// Gets the Selection Entry.
        /// </summary>
        public static Command<SAclopediaEntryItem> SelectionCommand
        {
            get
            {
                return new Command<SAclopediaEntryItem>((item) =>
                {
                    if (item != null)
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                            {
                                await Shell.Current.GoToAsync($"saclopediaentrypage?entryId={item.Id}&title={item.Title}").ConfigureAwait(false);
                            });
                    }
                });
            }
        }

        /// <summary>
        /// Gets the refresh command.
        /// </summary>
        public Command RefreshCommand
        {
            get
            {
                return new Command(async () => await this.RefreshEntryList(true).ConfigureAwait(false));
            }
        }

        /// <summary>
        /// Gets the SAclopedia Items.
        /// </summary>
        public List<SAclopediaGroup> Items { get; private set; } = new List<SAclopediaGroup>();

        /// <summary>
        /// Refreshes SAclopedia Items.
        /// </summary>
        /// <param name="refresh">Force refresh of cache.</param>
        /// <returns>A Task.</returns>
        public async Task RefreshEntryList(bool refresh)
        {
            this.IsBusy = true;
            var items = await this.saclopedia.LoadSAclopediaEntryItemsAsync(refresh).ConfigureAwait(false);
            this.Items = items.GroupBy(n => n.Title[0].ToString().ToUpperInvariant()).Select(n => new SAclopediaGroup(n.Key, n.ToList())).OrderBy(n => n.Name).ToList();
            this.OnPropertyChanged(nameof(this.Items));
            this.IsBusy = false;
        }
    }
}