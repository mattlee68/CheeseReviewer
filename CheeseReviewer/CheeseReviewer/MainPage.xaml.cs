using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CheeseReviewer
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        async void OnView(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ViewReviewsPage());
        }

        async void OnAdd(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddCheesePage());
        }
    }
}
