using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using ResizableFlyout.Services;
using ResizableFlyout.ViewModels;

namespace ResizableFlyout.Views
{
    public sealed partial class MainPage : Page
    {
        private ItemViewModel _lastSelectedItem;

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var items = MasterListView.ItemsSource as List<ItemViewModel>;
            Menu.ItemsSource = DataService.GetAllMenuItems();

            if (items == null)
            {
                items = new List<ItemViewModel>();

                foreach (var item in DataService.GetAllItems())
                {
                    items.Add(ItemViewModel.FromItem(item));
                }

                MasterListView.ItemsSource = items;
            }

            if (e.Parameter != null && (e.Parameter is string && !string.IsNullOrEmpty((string)e.Parameter)))
            {
                // Parameter is item ID
                var id = (int)e.Parameter;
                _lastSelectedItem = items.FirstOrDefault(item => item.ItemId == id);
            }

            UpdateForVisualState(AdaptiveStates.CurrentState);

            // Don't play a content transition for first item load.
            // Sometimes, this content will be animated as part of the page transition.
            DisableContentTransitions();
        }

        private void OnLayoutRootLoaded(object sender, RoutedEventArgs e)
        {
            // Assure we are displaying the correct item. This is necessary in certain adaptive cases.
            MasterListView.SelectedItem = _lastSelectedItem;
        }

        private void OnAdaptiveStatesCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            UpdateForVisualState(e.NewState, e.OldState);
        }

        private void UpdateForVisualState(VisualState newState, VisualState oldState = null)
        {
            var isNarrow = newState == Mobile;

            if (isNarrow && oldState == Desktop && _lastSelectedItem != null)
            {
                // Resize down to the detail item. Don't play a transition.
                Frame.Navigate(typeof(DetailPage), _lastSelectedItem.ItemId, new SuppressNavigationTransitionInfo());
            }

            EntranceNavigationTransitionInfo.SetIsTargetElement(MasterListView, isNarrow);
            if (DetailContentPresenter != null)
            {
                EntranceNavigationTransitionInfo.SetIsTargetElement(DetailContentPresenter, !isNarrow);
            }
        }

        private void OnMasterListViewItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedItem = (ItemViewModel)e.ClickedItem;
            _lastSelectedItem = clickedItem;

            if (AdaptiveStates.CurrentState == Mobile)
            {
                // Use "drill in" transition for navigating from master list to detail view
                Frame.Navigate(typeof(DetailPage), clickedItem.ItemId, new DrillInNavigationTransitionInfo());
            }
            else
            {
                // Play a refresh animation when the user switches detail items.
                EnableContentTransitions();
            }
        }

        private void EnableContentTransitions()
        {
            DetailContentPresenter.ContentTransitions.Clear();
            DetailContentPresenter.ContentTransitions.Add(new EntranceThemeTransition());
        }

        private void DisableContentTransitions()
        {
            DetailContentPresenter?.ContentTransitions.Clear();
        }

        private void OnFilterFlyoutOpened(object sender, object e)
        {
            MenuFlyout menuFlyout = sender as MenuFlyout;
            Style style = new Style { TargetType = typeof(MenuFlyoutPresenter) };
            style.Setters.Add(new Setter(MinWidthProperty, MasterColumn.ActualWidth));

            //Assuming we didn't set a different Compact Pane size other than using the default one
            var paneSize = Convert.ToInt32((Application.Current.Resources["SplitViewCompactPaneThemeLength"] as double?).Value);
            style.Setters.Add(new Setter(MarginProperty, new Thickness(paneSize, 0, 0, 0)));

            menuFlyout.MenuFlyoutPresenterStyle = style;
        }
    }
}
