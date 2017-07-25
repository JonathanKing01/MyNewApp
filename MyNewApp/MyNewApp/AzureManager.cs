using Microsoft.WindowsAzure.MobileServices;
using MyNewApp.DataModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyNewApp
{
    public class AzureManager
    {

        private static AzureManager instance;
        private MobileServiceClient client;
        private IMobileServiceTable<NotMountainModel> NotMountainTable;

        private AzureManager()
        {
            this.client = new MobileServiceClient("https://mymountain.azurewebsites.net/");
            this.NotMountainTable = this.client.GetTable<NotMountainModel>();
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

        public async Task<List<NotMountainModel>> GetMountainInformation()
        {
            return await this.NotMountainTable.ToListAsync();
        }

        public async Task PostMountainInformation(NotMountainModel notMountainModel)
        {
            await this.NotMountainTable.InsertAsync(notMountainModel);
        }

        public async Task UpdateMountainInformation(NotMountainModel notMountainModel)
        {
            await this.NotMountainTable.UpdateAsync(notMountainModel);
        }
        public async Task DeleteMountainInformation(NotMountainModel notMountainModel)
        {
            await this.NotMountainTable.DeleteAsync(notMountainModel);
        }
    }
}
