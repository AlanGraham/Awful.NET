﻿using Mazui.Core.Managers;
using Mazui.Core.Models.Threads;
using Mazui.Database.Functions;
using Mazui.Services;
using Mazui.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Utils;
using Windows.UI.Xaml.Navigation;

namespace Mazui.ViewModels
{
    public class BookmarkViewModel : ForumThreadListBaseViewModel
    {
        #region Properties

        private ObservableCollection<Thread> _bookmarkedThreads = new ObservableCollection<Thread>();


        public ObservableCollection<Thread> BookmarkedThreads
        {
            get { return _bookmarkedThreads; }
            set
            {
                Set(ref _bookmarkedThreads, value);
            }
        }

        private ThreadManager _threadManager;
        #endregion

        #region Methods
        public async void PullToRefresh_ListView(object sender, EventArgs e)
        {
            await Refresh();
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            if (WebManager == null)
            {
                await LoginUser();
            }

            _threadManager = new ThreadManager(WebManager);

            if (BookmarkedThreads != null && BookmarkedThreads.Any())
            {
                return;
            }

            await LoadInitialList();
        }

        public async Task LoadInitialList()
        {
            IsLoading = true;
            string error = "";
            try
            {
                var bookmarks = await ForumsDatabase.GetBookmarkedThreadsFromDb();
                if (bookmarks != null && bookmarks.Any())
                {
                    BookmarkedThreads = bookmarks.OrderBy(node => node.OrderNumber).ToObservableCollection();
                }
                if ((!BookmarkedThreads.Any() || SettingsService.Instance.LastRefresh > (DateTime.UtcNow.AddHours(1.00))))
                {
                    await Refresh();
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            if (!string.IsNullOrEmpty(error)) await ResultChecker.SendMessageDialogAsync($"Failed to get Bookmarks: {error}", false);
            IsLoading = false;
        }

        public async Task NavigateToThread(Thread thread)
        {
            Selected = thread;
            await NavigationService.NavigateAsync(typeof(Views.ThreadPage), JsonConvert.SerializeObject(thread));
        }

        public async Task Refresh()
        {
            IsLoading = true;
            string error = "";
            try
            {
                var pageNumber = 1;
                var hasItems = false;
                var oldList = true;
                while (!hasItems)
                {
                    var bookmarkResult = await _threadManager.GetBookmarksAsync(pageNumber);
                    var bookmarks = JsonConvert.DeserializeObject<List<Thread>>(bookmarkResult.ResultJson);
                    if (!bookmarks.Any())
                    {
                        hasItems = true;
                    }
                    else
                    {
                        pageNumber++;
                    }

                    if (oldList)
                    {
                        BookmarkedThreads = new ObservableCollection<Thread>();
                        oldList = false;
                    }

                    foreach (var bookmark in bookmarks)
                    {
                        BookmarkedThreads.Add(bookmark);
                    }
                }
                SettingsService.Instance.LastRefresh = DateTime.UtcNow;
                await ForumsDatabase.RefreshBookmarkedThreads(BookmarkedThreads.ToList());
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            if (!string.IsNullOrEmpty(error)) await ResultChecker.SendMessageDialogAsync($"Failed to get Bookmarks: {error}", false);
            IsLoading = false;
        }
        #endregion
    }
}
