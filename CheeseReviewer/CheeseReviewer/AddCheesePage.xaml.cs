﻿using CheeseReviewer.DataModels;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CheeseReviewer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddCheesePage : ContentPage
    {

        int cheeseRating;


        public AddCheesePage()
        {
            InitializeComponent();
            price.SetBinding(Entry.TextProperty, new Binding("Price", converter: new DecimalConverter()));
        }


        void EditorTextChanged(object sender, TextChangedEventArgs e)
        {
            var oldText = e.OldTextValue;
            var newText = e.NewTextValue;
        }


        void OnSliderValueChanged(object sender, ValueChangedEventArgs args)
        {
            double stepValue = 1.0;
            var newStep = Math.Round(args.NewValue / stepValue);
            double newValue = newStep * stepValue;
            sliderValue.Text = newValue.ToString();
            cheeseRating = int.Parse(sliderValue.Text);
        }

        void OnAdd(object sender, ValueChangedEventArgs args)
        {
            if (!ValidateInputs()) return;
            PostReviewAsync();
        }

        void TakePhoto(object sender, ValueChangedEventArgs args)
        {

        }

        async Task PostReviewAsync()
        {
            CheeseReviewerModel model = new CheeseReviewerModel()
            {
                Brand = brand.Text,
                Type = type.Text,
                Location = location.Text,
                Price = Double.Parse(price.Text),
                Rating = cheeseRating,
                Comments = comments.Text
            };

            await AzureManager.AzureManagerInstance.PostCheeseReviewerInformation(model);

            await DisplayAlert("Success", "Successfully reviewed a cheese!", "OK");

            await this.Navigation.PopAsync();
        }

        Boolean ValidateInputs()
        {
            if (String.IsNullOrEmpty(brand.Text))
            {
                DisplayAlert("Cheese Brand Empty", "You need to fill out the cheese's brand to proceed", "OK");
                return false;
            }
            else if (String.IsNullOrEmpty(type.Text))
            {
                DisplayAlert("Cheese Type Empty", "You need to fill out the cheese's type to proceed", "OK");
                return false;
            }
            else if (String.IsNullOrEmpty(location.Text))
            {
                DisplayAlert("Cheese Purchase Location Empty", "You need to fill out the cheese's purchase location to proceed", "OK");
                return false;
            }
            else if (String.IsNullOrEmpty(price.Text))
            {
                DisplayAlert("Cheese Price Empty", "You need to fill out the cheese's price to proceed", "OK");
                return false;
            }
            return true;
        }

        private async void LoadCamera(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                PhotoSize = PhotoSize.Medium,
                Directory = "Sample",
                Name = $"{DateTime.UtcNow}.jpg"
            });

            if (file == null)
                return;

            image.Source = ImageSource.FromStream(() =>
            {
                return file.GetStream();
            });

            file.Dispose();
        }
    }
}