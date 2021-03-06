﻿using BaconBackend.Collectors;
using BaconBackend.DataObjects;
using Baconit.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Baconit.Panels.SettingsPanels
{
    public sealed partial class SubredditViewSettings : UserControl, IPanel
    {
        bool m_takeChangeAction = false;
        IPanelHost m_host;

        public SubredditViewSettings()
        {
            this.InitializeComponent();
            App.BaconMan.SubredditMan.OnSubredditsUpdated += SubredditMan_OnSubredditsUpdated;
        }

        public void PanelSetup(IPanelHost host, Dictionary<string, object> arguments)
        {
            m_host = host;
        }

        public void OnNavigatingFrom()
        {
            // Ignore
        }

        public void OnPanelPulledToTop(Dictionary<string, object> arguments)
        {
            OnNavigatingTo();
        }

        public async void OnNavigatingTo()
        {
            // Set the status bar color and get the size returned. If it is not 0 use that to move the
            // color of the page into the status bar.
            double statusBarHeight = await m_host.SetStatusBar(null, 0);
            ui_contentRoot.Margin = new Thickness(0, -statusBarHeight, 0, 0);
            ui_contentRoot.Padding = new Thickness(0, statusBarHeight, 0, 0);

            m_takeChangeAction = false;

            ui_showFullTitles.IsOn = App.BaconMan.UiSettingsMan.SubredditList_ShowFullTitles;
            SetDefaultSortType(App.BaconMan.UiSettingsMan.SubredditList_DefaultSortType);
            SetDefaultSortTimeType(App.BaconMan.UiSettingsMan.SubredditList_DefaultSortTimeType);
            SetSubredditList();

            m_takeChangeAction = true;
        }

        public void OnCleanupPanel()
        {
            // Ignore for now.
        }

        /// <summary>
        /// Fired when the panel should try to reduce memory if possible. This will only be called
        /// while the panel isn't visible.
        /// </summary>
        public void OnReduceMemory()
        {
            // Ignore for now.
        }

        private void SubredditMan_OnSubredditsUpdated(object sender, BaconBackend.Managers.OnSubredditsUpdatedArgs e)
        {
            m_takeChangeAction = false;
            SetSubredditList();
            m_takeChangeAction = true;
        }

        private void SetSubredditList()
        {
            // Get the subreddits
            List<Subreddit> subreddits = App.BaconMan.SubredditMan.SubredditList;

            // Get the current defaults
            string defaultSubreddit = App.BaconMan.UiSettingsMan.SubredditList_DefaultSubredditDisplayName;

            List<string> displayNames = new List<string>();
            int count = 0;
            int defaultDispNameIndex = -1;
            foreach(Subreddit sub in subreddits)
            {
                displayNames.Add(sub.DisplayName);

                if(sub.DisplayName.Equals(defaultSubreddit))
                {
                    defaultDispNameIndex = count;
                }
                count++;
            }

            // We couldn't find the subreddit, add it to the bottom
            if(defaultDispNameIndex == -1)
            {
                displayNames.Add(defaultSubreddit);
                defaultDispNameIndex = displayNames.Count - 1;
            }

            ui_defaultSubreddit.ItemsSource = displayNames;
            ui_defaultSubreddit.SelectedIndex = defaultDispNameIndex;
        }


        private void ShowFullTitles_Toggled(object sender, RoutedEventArgs e)
        {
            if (!m_takeChangeAction)
            {
                return;
            }

            App.BaconMan.UiSettingsMan.SubredditList_ShowFullTitles = ui_showFullTitles.IsOn;

            // Show message
            App.BaconMan.MessageMan.ShowMessageSimple("Please Note", "You will have to refresh any open subreddits for this to apply.");
        }

        private void DefaultSubreddit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!m_takeChangeAction)
            {
                return;
            }

            App.BaconMan.UiSettingsMan.SubredditList_DefaultSubredditDisplayName = (string)ui_defaultSubreddit.SelectedItem;
        }

        private void DefaultSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!m_takeChangeAction || ui_defaultSort.SelectedIndex == -1)
            {
                return;
            }

            switch (ui_defaultSort.SelectedIndex)
            {
                case 0:
                    App.BaconMan.UiSettingsMan.SubredditList_DefaultSortType = SortTypes.Hot;
                    break;
                case 1:
                    App.BaconMan.UiSettingsMan.SubredditList_DefaultSortType = SortTypes.New;
                    break;
                case 2:
                    App.BaconMan.UiSettingsMan.SubredditList_DefaultSortType = SortTypes.Rising;
                    break;
                case 3:
                    App.BaconMan.UiSettingsMan.SubredditList_DefaultSortType = SortTypes.Controversial;
                    break;
                case 4:
                    App.BaconMan.UiSettingsMan.SubredditList_DefaultSortType = SortTypes.Top;
                    break;
            }
        }

        private void SetDefaultSortType(SortTypes type)
        {
            switch(type)
            {
                case SortTypes.Hot:
                    ui_defaultSort.SelectedIndex = 0;
                    break;
                case SortTypes.Controversial:
                    ui_defaultSort.SelectedIndex = 3;
                    break;
                case SortTypes.New:
                    ui_defaultSort.SelectedIndex = 1;
                    break;
                case SortTypes.Rising:
                    ui_defaultSort.SelectedIndex = 2;
                    break;
                case SortTypes.Top:
                    ui_defaultSort.SelectedIndex = 4;
                    break;
            }
        }

        private void DefaultTimeSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!m_takeChangeAction || ui_defaultTimeSort.SelectedIndex == -1)
            {
                return;
            }

            switch (ui_defaultTimeSort.SelectedIndex)
            {
                case 0:
                    App.BaconMan.UiSettingsMan.SubredditList_DefaultSortTimeType = SortTimeTypes.Hour;
                    break;
                case 1:
                    App.BaconMan.UiSettingsMan.SubredditList_DefaultSortTimeType = SortTimeTypes.Day;
                    break;
                case 2:
                    App.BaconMan.UiSettingsMan.SubredditList_DefaultSortTimeType = SortTimeTypes.Week;
                    break;
                case 3:
                    App.BaconMan.UiSettingsMan.SubredditList_DefaultSortTimeType = SortTimeTypes.Month;
                    break;
                case 4:
                    App.BaconMan.UiSettingsMan.SubredditList_DefaultSortTimeType = SortTimeTypes.Year;
                    break;
                case 5:
                    App.BaconMan.UiSettingsMan.SubredditList_DefaultSortTimeType = SortTimeTypes.AllTime;
                    break;
            }
        }

        private void SetDefaultSortTimeType(SortTimeTypes type)
        {
            switch (type)
            {
                case SortTimeTypes.Hour:
                    ui_defaultTimeSort.SelectedIndex = 0;
                    break;
                case SortTimeTypes.Day:
                    ui_defaultTimeSort.SelectedIndex = 1;
                    break;
                case SortTimeTypes.Week:
                    ui_defaultTimeSort.SelectedIndex = 2;
                    break;
                case SortTimeTypes.Month:
                    ui_defaultTimeSort.SelectedIndex = 3;
                    break;
                case SortTimeTypes.Year:
                    ui_defaultTimeSort.SelectedIndex = 4;
                    break;
                case SortTimeTypes.AllTime:
                    ui_defaultTimeSort.SelectedIndex = 5;
                    break;
            }
        }
    }
}
