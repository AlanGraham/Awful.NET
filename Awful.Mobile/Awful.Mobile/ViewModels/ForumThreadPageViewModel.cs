﻿// <copyright file="ForumThreadPageViewModel.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Awful.Core.Entities.Threads;
using Awful.Core.Utilities;
using Awful.Database.Context;
using Awful.Database.Entities;
using Awful.Mobile.Controls;
using Awful.Mobile.Pages;
using Awful.UI.Actions;
using Awful.UI.Entities;
using Awful.Webview;
using Awful.Webview.Entities.Themes;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Awful.Mobile.ViewModels
{
    /// <summary>
    /// Forum Thread Page View Model.
    /// </summary>
    public class ForumThreadPageViewModel : AwfulWebviewViewModel
    {
        private TemplateHandler handler;
        private ThreadPostActions threadPostActions;
        private ThreadActions threadActions;
        private ThreadPost threadPost;
        private Command refreshCommand;
        private DefaultOptions defaults;

        /// <summary>
        /// Initializes a new instance of the <see cref="ForumThreadPageViewModel"/> class.
        /// </summary>
        /// <param name="handler">Awful Handler.</param>
        /// <param name="context">Awful Context.</param>
        public ForumThreadPageViewModel(TemplateHandler handler, AwfulContext context)
            : base(context)
        {
            this.handler = handler;
        }

        /// <summary>
        /// Gets or sets the current state of the view.
        /// </summary>
        public ThreadPost ThreadPost
        {
            get { return this.threadPost; }
            set { this.SetProperty(ref this.threadPost, value); }
        }

        /// <summary>
        /// Gets the refresh command.
        /// </summary>
        public Command RefreshCommand
        {
            get
            {
                return this.refreshCommand ??= new Command(async () =>
                {
                    await this.RefreshThreadAsync().ConfigureAwait(false);
                });
            }
        }

        /// <summary>
        /// Gets the reply to thread command.
        /// </summary>
        public Command ReplyToThreadCommand
        {
            get
            {
                return new Command(async () =>
                {
                    if (this.ThreadPost != null)
                    {
                        await PushModalAsync(new ThreadReplyPage(this.Thread.ThreadId)).ConfigureAwait(false);
                    }
                });
            }
        }

        /// <summary>
        /// Gets the First Page Command.
        /// </summary>
        public Command FirstPageCommand
        {
            get
            {
                return new Command(async () =>
                {
                    if (this.ThreadPost != null)
                    {
                        await this.LoadTemplate(this.threadPost.ThreadId, 1).ConfigureAwait(false);
                    }
                });
            }
        }

        /// <summary>
        /// Gets the Previous Page Command.
        /// </summary>
        public Command PreviousPageCommand
        {
            get
            {
                return new Command(async () =>
                {
                    if (this.ThreadPost != null)
                    {
                        if (this.threadPost.CurrentPage - 1 >= 1)
                        {
                            await this.LoadTemplate(this.threadPost.ThreadId, this.threadPost.CurrentPage - 1).ConfigureAwait(false);
                        }
                    }
                });
            }
        }

        /// <summary>
        /// Gets the Next Page Command.
        /// </summary>
        public Command NextPageCommand
        {
            get
            {
                return new Command(async () =>
                {
                    if (this.ThreadPost != null)
                    {
                        if (this.threadPost.CurrentPage + 1 <= this.threadPost.TotalPages)
                        {
                            await this.LoadTemplate(this.threadPost.ThreadId, this.threadPost.CurrentPage + 1).ConfigureAwait(false);
                        }
                    }
                });
            }
        }

        /// <summary>
        /// Gets the Last Page Command.
        /// </summary>
        public Command LastPageCommand
        {
            get
            {
                return new Command(async () =>
                {
                    if (this.ThreadPost != null)
                    {
                        await this.LoadTemplate(this.threadPost.ThreadId, this.threadPost.TotalPages).ConfigureAwait(false);
                    }
                });
            }
        }

        /// <summary>
        /// Refreshes the thread.
        /// </summary>
        /// <returns>A <see cref="Task"/>.</returns>
        public async Task RefreshThreadAsync()
        {
            if (this.ThreadPost != null)
            {
                this.IsRefreshing = true;
                await this.LoadTemplate(this.threadPost.ThreadId, this.threadPost.CurrentPage).ConfigureAwait(false);
                this.IsRefreshing = false;
            }
        }

        /// <summary>
        /// Loads Thread Template into webview.
        /// </summary>
        /// <param name="threadId">Thread Id to load.</param>
        /// <param name="pageNumber">Page Number to load.</param>
        /// <param name="gotoNewestPost">Go to newest post on page. Ignores page number.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public async Task LoadTemplate(int threadId, int pageNumber, bool gotoNewestPost = false)
        {
            this.IsBusy = true;
            this.defaults = await this.GenerateDefaultOptionsAsync().ConfigureAwait(false);
            this.ThreadPost = await this.threadPostActions.GetThreadPostsAsync(threadId, pageNumber, gotoNewestPost).ConfigureAwait(false);
            this.Title = this.ThreadPost.Name;
            if (this.Thread != null && this.Thread.RepliesSinceLastOpened > 0)
            {
                var test = ((this.ThreadPost.TotalPages - 1) * EndPoints.DefaultNumberPerPage) + this.ThreadPost.Posts.Count;
                this.Thread.ReplyCount = test;
                var seenReplies = this.Thread.ReplyCount - this.Thread.RepliesSinceLastOpened;
                var seenPages = seenReplies / EndPoints.DefaultNumberPerPage;
                if (this.ThreadPost.CurrentPage > seenPages)
                {
                    var readReplies = ((this.ThreadPost.CurrentPage - (seenPages + 1)) * EndPoints.DefaultNumberPerPage) + this.ThreadPost.Posts.Count;
                    var totalReplies = this.Thread.RepliesSinceLastOpened - readReplies;
                    if (totalReplies < 0)
                    {
                        totalReplies = 0;
                    }

                    this.Thread.RepliesSinceLastOpened = totalReplies;
                    this.Thread.OnPropertyChanged("RepliesSinceLastOpened");
                }
            }

            var source = new HtmlWebViewSource();
            source.Html = this.threadPostActions.RenderThreadPostView(this.ThreadPost, defaults);
            Device.BeginInvokeOnMainThread(() => this.WebView.Source = source);
            this.IsBusy = false;
        }

        /// <summary>
        /// Loads the thread into the VM.
        /// </summary>
        /// <param name="thread"><see cref="AwfulThread"/>.</param>
        public void LoadThread(AwfulThread thread)
        {
            if (thread == null)
            {
                throw new ArgumentNullException(nameof(thread));
            }

            this.Thread = thread;
            this.Title = thread.Name;
        }

        /// <inheritdoc/>
        public override async Task OnLoad()
        {
            this.threadPostActions = new ThreadPostActions(this.Client, this.Context, this.handler);
            this.threadActions = new ThreadActions(this.Client, this.Context);
            if (this.Thread != null && this.threadPost == null)
            {
                await this.LoadTemplate(this.Thread.ThreadId, 0, this.Thread.HasSeen).ConfigureAwait(false);
            }
        }

        protected void HandleDataFromJavascript(string data)
        {
            var json = JsonConvert.DeserializeObject<WebViewDataInterop>(data);
            switch (json.Type)
            {
                case "showPostMenu":
                    Device.BeginInvokeOnMainThread(async () => {
                        var result = await App.Current.MainPage.DisplayActionSheet("Post Options", "Cancel", null, "Share", "Mark Read", "Quote Post").ConfigureAwait(false);
                        switch (result)
                        {
                            case "Share":
                                await Share.RequestAsync(new ShareTextRequest
                                {
                                    Uri = string.Format(CultureInfo.InvariantCulture, EndPoints.ShowPost, json.Id),
                                    Title = this.Title,
                                }).ConfigureAwait(false);
                                break;
                            case "Mark Read":
                                _ = Task.Run(async () => await this.MarkPostAsUnreadAsync(json.Id).ConfigureAwait(false)).ConfigureAwait(false);
                                break;
                            case "Quote Post":
                                await PushModalAsync(new ThreadReplyPage(this.Thread.ThreadId, json.Id, false)).ConfigureAwait(false);
                                break;
                        }
                    });
                    break;
            }
        }

        private async Task MarkPostAsUnreadAsync(int postId)
        {
            var postIndex = this.ThreadPost.Posts.FirstOrDefault(n => n.PostId == postId);
            if (postIndex != null)
            {
                var result = await this.threadActions.MarkPostAsLastReadAsAsync(this.Thread.ThreadId, postIndex.PostIndex).ConfigureAwait(false);
            }
        }
    }
}
