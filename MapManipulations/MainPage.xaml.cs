using MapManipulations.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Services.Maps;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace MapManipulations
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public MainPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private Geopoint point = null;
        private async void btnGetCurrentLocation_Click(object sender, RoutedEventArgs e)
        {
            btnGetCurrentLocation.IsEnabled = false;
            Geolocator locator = new Geolocator();
            Geoposition position = await locator.GetGeopositionAsync();
            point = position.Coordinate.Point;
            txtLatitude.Text = point.Position.Latitude.ToString();
            txtLongitude.Text = point.Position.Longitude.ToString();
            btnGetCurrentLocation.IsEnabled = true;
            myMap.Center = point;
            myMap.ZoomLevel = mySlider.Value;
        }

        private void btnZoomIn_Click(object sender, RoutedEventArgs e)
        {
            if (myMap.ZoomLevel < (myMap.MaxZoomLevel-0.5))
                myMap.ZoomLevel += 0.5;
        }

        private void btnZoomOut_Click(object sender, RoutedEventArgs e)
        {
            if (myMap.ZoomLevel > (myMap.MinZoomLevel + 0.5))
            myMap.ZoomLevel -= 0.5;
        }
        private async void myMap_MapTapped(Windows.UI.Xaml.Controls.Maps.MapControl sender, Windows.UI.Xaml.Controls.Maps.MapInputEventArgs args)
        {
            txtLatitude.Text = args.Location.Position.Altitude.ToString();
            txtLongitude.Text = args.Location.Position.Longitude.ToString();
            MapLocationFinderResult result = await MapLocationFinder.FindLocationsAtAsync(args.Location);
            await new MessageDialog(result.Locations[0].ToString()).ShowAsync();
        }

        private async void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            await myMap.TrySetViewAsync(point, 10);
        }

        private async void txtLocation_LostFocus(object sender, RoutedEventArgs e)
        {
            btnGotoCurrentLocation.IsEnabled = false;
            btnGetCurrentLocation.IsEnabled = false;

            MapLocationFinderResult result = await MapLocationFinder.FindLocationsAsync(txtLocation.Text, point);
            await myMap.TrySetViewAsync(result.Locations[0].Point, mySlider.Value);

            btnGotoCurrentLocation.IsEnabled = true;
            btnGetCurrentLocation.IsEnabled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Routes));
        }
    }
}
