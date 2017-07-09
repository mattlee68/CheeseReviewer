using CheeseReviewer.DataModels;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using Newtonsoft.Json;

/// <summary>
/// This class deals with the "Adding Cheese" page of the application. It takes the information from the user and posts it to the database stored in Azure.
/// </summary>

namespace CheeseReviewer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddCheesePage : ContentPage
    {

        // Fields, used to store the cheese's rating (by the slider) and the emotion (connected to Cognitive Services).
        int cheeseRating;
        string emotion = "None";

        // Constants used for the Emotion API - expires 7 Aug 2017.
        private const string APIKey = "8a17bba660a14215ae411aa120b41291";
        private const string url = "https://westus.api.cognitive.microsoft.com/emotion/v1.0";

        
        public AddCheesePage()
        {
            InitializeComponent();

            // Setup price binding for decimal entry only.
            price.SetBinding(Entry.TextProperty, new Binding("Price", converter: new DecimalConverter()));

            // Setup emotion label.
            emotionResultLabel.Text = "Emotion: " +emotion;
        }


        /// <summary>
        /// Deals with only integer values for the slider and updating the cheeseRating variable.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void OnSliderValueChanged(object sender, ValueChangedEventArgs args)
        {
            double stepValue = 1.0;
            var newStep = Math.Round(args.NewValue / stepValue);
            double newValue = newStep * stepValue;
            sliderValue.Text = newValue.ToString();
            cheeseRating = int.Parse(sliderValue.Text);
        }

        /// <summary>
        /// When the user clicks add, checks all compulsory fields are not blank and then posts information.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void OnAdd(object sender, ValueChangedEventArgs args)
        {
            if (!ValidateInputs()) return;
            PostReviewAsync();
        }


        /// <summary>
        /// Creates a model and posts the information to the Azure servers.
        /// </summary>
        /// <returns></returns>
        async Task PostReviewAsync()
        {
            CheeseReviewerModel model = new CheeseReviewerModel()
            {
                Brand = brand.Text,
                Type = type.Text,
                Location = location.Text,
                Price = Double.Parse(price.Text),
                Rating = cheeseRating,
                Comments = comments.Text,
                Emotion = emotion
            };

            await AzureManager.AzureManagerInstance.PostCheeseReviewerInformation(model);

            // Display alert to user.
            await DisplayAlert("Success", "Successfully reviewed a cheese!", "OK");

            // Redirect back to the main page.
            await this.Navigation.PopAsync();
        }

        /// <summary>
        /// Validates that none of the compulsory inputs are empty. If any entry is missing, it will tell the user.
        /// </summary>
        /// <returns>Returns true if everything is okay. False if missing an entry</returns>
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

        /// <summary>
        /// Loads up the camera, gets the photo then submits it to the Emotion API.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void LoadCamera(object sender, EventArgs e)
        {
            // Wait for the camera to load/initialise.
            await CrossMedia.Current.Initialize();

            // If we cannot access the camera then stop.
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            // Take the photo and store it.
            MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                PhotoSize = PhotoSize.Medium,
                Directory = "Sample",
                Name = $"{DateTime.UtcNow}.jpg"
            });

            // If nothing was created, then stop.
            if (file == null)
                return;

            // Otherwise, get the image and make the request to get the emotion.
            image.Source = ImageSource.FromStream(() =>
            {
                return file.GetStream();
            });
            await MakePredictionRequest(file);
        }

        /// <summary>
        /// Calls the Emotion API and updates the labels/variables regarding the user's emotion.
        /// </summary>
        /// <param name="file">The image of to be detected</param>
        /// <returns></returns>
        async Task MakePredictionRequest(MediaFile file)
        {
            EmotionServiceClient emotionServiceClient = new EmotionServiceClient(APIKey, url);
            try
            {
                byte[] byteData = GetImageAsByteArray(file);

                using (Stream stream = new MemoryStream(byteData))
                {
                    // Using the Emotion class, get the result.
                    Emotion[] emotionResult = await emotionServiceClient.RecognizeAsync(stream);
                    if (emotionResult.Any())
                    {
                        // If there an emotion detected, store it and update all labels/variables.
                        string result = emotionResult.FirstOrDefault().Scores.ToRankedList().FirstOrDefault().Key;
                        emotion = result;
                        emotionResultLabel.Text = "Emotion Result: " +result;
                        DisplayAlert("Success", "Our magical algorithm detected your expression as: " + result, "Ok");
                    }
                    else
                    {
                        // Did not find an emotion so tell the user and stop.
                        DisplayAlert("Sorry", "We could not identify an emotion with your face", "Ok");
                    }

                    // Can't forget to dispose of the file!
                    file.Dispose();
                }            
            }catch(Exception e)
            {
                // Something went wrong - most likely connecting to the API.
                Debug.WriteLine("Something went wrong");
                Debug.WriteLine(e);
            }
            return;

        }

        /// <summary>
        /// Converts the image into a Byte[]
        /// </summary>
        /// <param name="file"></param>
        /// <returns>Image in the form of a Byte[]</returns>
        static byte[] GetImageAsByteArray(MediaFile file)
        {
            var stream = file.GetStream();
            BinaryReader binaryReader = new BinaryReader(stream);
            return binaryReader.ReadBytes((int)stream.Length);
        }
    }
}