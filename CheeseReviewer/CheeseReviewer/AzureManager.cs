using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using CheeseReviewer.DataModels;

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

        public MobileServiceClient AzureClient
        {
            get { return client; }
        }

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

        public async Task<List<CheeseReviewerModel>> GetCheeseReviewInformation()
        {
            return await this.cheeseReviewTable.ToListAsync();
        }
    }
}