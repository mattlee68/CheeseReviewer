using CheeseReviewer.DataModels;
using Microsoft.WindowsAzure.MobileServices;
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
	public partial class ViewReviewsPage : ContentPage
	{

        MobileServiceClient client = AzureManager.AzureManagerInstance.AzureClient;

        public ViewReviewsPage ()
		{
			InitializeComponent ();
            getCheeses();
        }

        async void getCheeses()
        {
            List<CheeseReviewerModel> cheeseReviewerInformation = await AzureManager.AzureManagerInstance.GetCheeseReviewInformation();
            CheeseReviewerList.ItemsSource = cheeseReviewerInformation;
        }
    }
}