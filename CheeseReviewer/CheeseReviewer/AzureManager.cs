using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using CheeseReviewer.DataModels;

/// <summary>
/// The main class that interacts with Azure.
/// </summary>

namespace CheeseReviewer
{
    public class AzureManager
    {

        private static AzureManager instance;
        private MobileServiceClient client;
        private IMobileServiceTable<CheeseReviewerModel> cheeseReviewTable;

        private AzureManager()
        {
            this.client = new MobileServiceClient("https://cheesereviewer.azurewebsites.net");
            this.cheeseReviewTable = this.client.GetTable<CheeseReviewerModel>();
        }

        /// <summary>
        /// Standard getter that returns the Azure Client.
        /// </summary>
        public MobileServiceClient AzureClient
        {
            get { return client; }
        }

        /// <summary>
        /// Checks if the instance is null, if not - it returns it.
        /// </summary>
        public static AzureManager AzureManagerInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AzureManager();
                }

                return instance;
            }
        }

        /// <summary>
        /// Returns the CheeseReviewer table (sorted by Cheese Rating and then Cheese Brand).
        /// </summary>
        /// <returns></returns>
        public async Task<List<CheeseReviewerModel>> GetCheeseReviewInformation()
        {
            return await this.cheeseReviewTable.OrderByDescending(CheeseReviewerModel => CheeseReviewerModel.Rating).ThenBy(CheeseReviewerModel => CheeseReviewerModel.Brand).ToListAsync();
        }

        /// <summary>
        /// Posts a CheeseReviewerModel to the Azure database.
        /// </summary>
        /// <param name="cheeseReviewerModel"></param>
        /// <returns></returns>
        public async Task PostCheeseReviewerInformation(CheeseReviewerModel cheeseReviewerModel)
        {
            await this.cheeseReviewTable.InsertAsync(cheeseReviewerModel);
        }
    }
}