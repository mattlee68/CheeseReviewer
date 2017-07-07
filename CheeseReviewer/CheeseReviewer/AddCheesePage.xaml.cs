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

            //file.Dispose();
            await MakePredictionRequest(file);
        }

        static byte[] GetImageAsByteArray(MediaFile file)
        {
            var stream = file.GetStream();
            BinaryReader binaryReader = new BinaryReader(stream);
            return binaryReader.ReadBytes((int)stream.Length);
        }

        async Task MakePredictionRequest(MediaFile file)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "2cc575735ee14d399ab05aa48bf0f305");

            string url = "https://westus.api.cognitive.microsoft.com/emotion/v1.0/recognize?";

            HttpResponseMessage response;
            string responseContent;

            //var emotionClient = new EmotionServiceClient("8a17bba660a14215ae411aa120b41291");


            byte[] byteData = GetImageAsByteArray(file);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(url, content);
                responseContent = response.Content.ReadAsStringAsync().Result;
                //Emotion[] emotionResult = await emotionClient.RecognizeAsync(content.ToString());
                //if (emotionResult.Any())
                //{
                //    emotionResultLabel.Text = emotionResult.FirstOrDefault().Scores.ToRankedList().FirstOrDefault().Key;
                //}
            }

            Debug.WriteLine(emotionResultLabel.Text);

            DisplayAlert("YO", "Yo", "OK");


        }
    }
}