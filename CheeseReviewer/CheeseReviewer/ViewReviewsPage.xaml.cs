using CheeseReviewer.DataModels;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

/// <summary>
/// Simple page that handles the "View Reviews" of the Cheese Reviewer application.
/// </summary>

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

        /// <summary>
        /// Gets the cheese list to populate the list in the Xaml file.
        /// </summary>
        async void getCheeses()
        {
            List<CheeseReviewerModel> cheeseReviewerInformation = await AzureManager.AzureManagerInstance.GetCheeseReviewInformation();
            CheeseReviewerList.ItemsSource = cheeseReviewerInformation;
        }
    }
}