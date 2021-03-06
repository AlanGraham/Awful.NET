﻿// <copyright file="BookmarkAction.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Awful.Core.Entities.Threads;
using Awful.Core.Managers;
using Awful.Core.Utilities;
using Awful.Database.Context;
using Awful.Database.Entities;
using Awful.Webview;
using Awful.Webview.Entities.Themes;
using Microsoft.EntityFrameworkCore;

namespace Awful.UI.Actions
{
    /// <summary>
    /// Bookmark Action.
    /// </summary>
    public class BookmarkAction
    {
        private AwfulContext context;
        private BookmarkManager manager;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookmarkAction"/> class.
        /// </summary>
        /// <param name="client">AwfulClient.</param>
        /// <param name="context">AwfulContext.</param>
        public BookmarkAction(AwfulClient client, AwfulContext context)
        {
            this.manager = new BookmarkManager(client);
            this.context = context;
        }

        /// <summary>
        /// Get all Bookmarks for given user.
        /// </summary>
        /// <returns>List of Thread Bookmarks.</returns>
        public async Task<List<AwfulThread>> GetAllBookmarksCachedAsync()
        {
            var bookmarks = await this.context.BookmarkThreads.ToListAsync().ConfigureAwait(false);
            return bookmarks.OrderBy(n => n.SortOrder).ToList();
        }

        /// <summary>
        /// Get all Bookmarks for given user.
        /// </summary>
        /// <returns>List of Thread Bookmarks.</returns>
        /// <param name="forceRefresh">Force Refresh.</param>
        public async Task<List<AwfulThread>> GetAllBookmarksAsync(bool forceRefresh = false)
        {
            var bookmarks = await this.context.BookmarkThreads.ToListAsync().ConfigureAwait(false);
            if (!bookmarks.Any() || forceRefresh)
            {
                var threads = await this.manager.GetAllBookmarksAsync().ConfigureAwait(false);
                bookmarks = await this.context.AddAllBookmarkThreads(threads.Threads).ConfigureAwait(false);
            }

            return bookmarks.OrderBy(n => n.SortOrder).ToList();
        }

        /// <summary>
        /// Enable or disable bookmark notifications for a given thread.
        /// </summary>
        /// <param name="thread">The AwfulThread.</param>
        /// <returns>The AwfulThread with the updated value.</returns>
        public async Task<AwfulThread> EnableDisableBookmarkNotificationAsync(AwfulThread thread)
        {
            return await this.context.EnableDisableBookmarkNotificationsEnable(thread).ConfigureAwait(false);
        }

        /// <summary>
        /// Add Bookmark.
        /// </summary>
        /// <param name="threadId">Thread Id.</param>
        /// <returns>Result.</returns>
        public async Task<List<AwfulThread>> AddBookmarkAsync(int threadId)
        {
            var result = await this.manager.AddBookmarkAsync(threadId).ConfigureAwait(false);

            return await this.GetAllBookmarksAsync(true).ConfigureAwait(false);
        }

        /// <summary>
        /// Add Bookmark.
        /// </summary>
        /// <param name="thread">Thread.</param>
        /// <returns>Result.</returns>
        public async Task<List<AwfulThread>> AddBookmarkAsync(Core.Entities.Threads.Thread thread)
        {
            if (thread == null)
            {
                throw new ArgumentNullException(nameof(thread));
            }

            return await this.AddBookmarkAsync(thread.ThreadId).ConfigureAwait(false);
        }

        /// <summary>
        /// Remove Bookmark.
        /// </summary>
        /// <param name="threadId">Thread Id.</param>
        /// <returns>Result.</returns>
        public async Task<List<AwfulThread>> RemoveBookmarkAsync(int threadId)
        {
            var result = await this.manager.RemoveBookmarkAsync(threadId).ConfigureAwait(false);
            return await this.GetAllBookmarksAsync(true).ConfigureAwait(false);
        }

        /// <summary>
        /// Remove Bookmark.
        /// </summary>
        /// <param name="thread">Thread.</param>
        /// <returns>Result.</returns>
        public async Task<List<AwfulThread>> RemoveBookmarkAsync(Core.Entities.Threads.Thread thread)
        {
            if (thread == null)
            {
                throw new ArgumentNullException(nameof(thread));
            }

            return await this.RemoveBookmarkAsync(thread.ThreadId).ConfigureAwait(false);
        }
    }
}
