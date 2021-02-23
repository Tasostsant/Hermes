﻿using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Hermes.Model.Models;

namespace Hermes.View.mylistings
{
    public partial class MyListingsView : Page, IMyListingsView
    {
        private string ImagePathSrc = null;

        private readonly MyListingsPresenter _presenter;

        public MyListingsView()
        {
            InitializeComponent();

            _presenter = new MyListingsPresenter(this);

            //TODO change this with radio button
            _RefreshListings(-1);

            radioboxAllListings.IsChecked = true;
        }

        private void _RefreshListings(int type) 
        {
            _presenter.GetListings(type);
        }

        public List<Listing> Listings
        {
            set 
            {
                listviewListings.ItemsSource = null; // Needed to reset any attached items
                listviewListings.ItemsSource = value;
            }
        }
        public string GetImagePath
        {
            get
            {
                return ImagePathSrc;
            }
        }
        private void listviewListings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            
            Listing listing = (Listing)listviewListings.SelectedItem;

            if (listing != null)
            {
                txtboxUploadTitle.Text = listing.Name;
                txtboxUploadPrice.Text = listing.Price.ToString();
                txtboxUploadDescription.Text = listing.Description;

                if (listing.Active)
                {
                    btnDeleteListing.IsEnabled = true;
                    btnUploadUpload.IsEnabled = true;
                }
                else
                {
                    btnDeleteListing.IsEnabled = false;
                    btnUploadUpload.IsEnabled = false;
                }
                if ((listviewListings.SelectedItem as Listing).Active == true)
                    EnableAllButtons(true);
                else
                    EnableAllButtons(false);
            }
        }

        private void btnProfileMyProfile_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("View/profile/ProfileView.xaml", UriKind.RelativeOrAbsolute));
        }

        private void btnProfileHistory_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("View/history/HistoryView.xaml", UriKind.RelativeOrAbsolute));
        }

        private void btnProfileFavorites_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("View/favourites/FavoritesView.xaml", UriKind.RelativeOrAbsolute));
        }

        private void btnProfileListings_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("View/mylistings/MyListingsView.xaml", UriKind.RelativeOrAbsolute));
        }

     
        private void btnDeleteListing_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to deactivate this listing? \n this action can not be reverted!", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Information);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    Listing listing = (Listing)listviewListings.SelectedItem;
                    _presenter.DeactivateListing(listing.Id);
                    //TODO change this with radio button
                    _RefreshListings(1);
                    break;
                case MessageBoxResult.No:
                    break;
                
            }
        }

        private void btnUploadUpload_Click(object sender, RoutedEventArgs e)
        {
            if (txtboxUploadTitle.Text == "")
            {
                MessageBox.Show("List must have a title!", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            else 
            {
                Listing listing = (Listing)listviewListings.SelectedItem;
                
                _presenter.UpdateListing(listing.Id ,txtboxUploadTitle.Text, float.Parse(txtboxUploadPrice.Text), txtboxUploadDescription.Text, GetImagePath);
            }
        }

        private void radioboxActiveListings_Checked(object sender, RoutedEventArgs e)
        {
            _RefreshListings(1);
            btnDeleteListing.IsEnabled = false;
            btnUploadUpload.IsEnabled = false;
            resetListingInfo();
        }
        
        private void radioboxInactiveListings_Checked(object sender, RoutedEventArgs e)
        {
            _RefreshListings(0);
            btnDeleteListing.IsEnabled = false;
            btnUploadUpload.IsEnabled = false;
            resetListingInfo();
        }

        private void radioboxAllListings_Checked(object sender, RoutedEventArgs e)
        {
            _RefreshListings(-1);
            btnDeleteListing.IsEnabled = false;
            btnUploadUpload.IsEnabled = false;
            resetListingInfo();
        }

        private void resetListingInfo()
        {
            txtboxUploadTitle.Text = "";
            txtboxUploadPrice.Text = "";
            txtboxUploadDescription.Text = "";
            //add more..
        }

        private void btnProfileSignout_Click(object sender, RoutedEventArgs e)
        {
            _presenter.Logout();
            this.NavigationService.Navigate(new Uri("View/login/LoginView.xaml", UriKind.RelativeOrAbsolute));
        }

        public void EnableAllButtons(Boolean enable)
        {
            txtboxUploadTitle.IsEnabled = enable;
            txtboxUploadPrice.IsEnabled = enable;
            txtboxUploadLocation.IsEnabled = enable;
            txtboxUploadDescription.IsEnabled = enable;
            comboxUploadCategory.IsEnabled = enable;
            comboxUploadSubcategory.IsEnabled = enable;
            btnUploadImage.IsEnabled = enable;
            btnUploadUpload.IsEnabled = enable;
            comboxUploadLocation.IsEnabled = enable;
            radbtnUploadLocationSelect.IsEnabled = enable;
            radbtnUploadLocationType.IsEnabled = enable;

        }
        public void DisableAllButtons()
        {

        }

        private void btnUploadImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetImagePath();
                UploadImage.Source = new BitmapImage(new Uri(GetImagePath));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void SetImagePath()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog1 = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image files | *.jpg"
            };

            if (openFileDialog1.ShowDialog() == true)
            {
                ImagePathSrc = openFileDialog1.FileName;
            }
            else
            {
                ImagePathSrc = null;
            }
        }
    }
}
