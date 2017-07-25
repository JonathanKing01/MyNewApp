using Microsoft.WindowsAzure.MobileServices;
using MyNewApp.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyNewApp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AzureTables : ContentPage
	{

        MobileServiceClient client = AzureManager.AzureManagerInstance.AzureClient;

        public AzureTables()
        {
            InitializeComponent();
        }

        async void Handle_ClickedAsync(object sender, System.EventArgs e)
        {
            List<NotMountainModel> notMountainInformation = await AzureManager.AzureManagerInstance.GetMountainInformation();

            NotMountainList.ItemsSource = notMountainInformation;
        }
    }
}