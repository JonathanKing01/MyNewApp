using MyNewApp.Model;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MyNewApp.DataModels;


namespace MyNewApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomVision : ContentPage
    {
        string tag = "";
        string tagId = "";
        MediaFile file;
        string iterationId = "";

        public CustomVision()
        {
            InitializeComponent();
        }

        private async void failedPrediction()
        {
            await postResultsAsync(tag,false);
        }

        private async void successfulPrediction()
        {
            await Upload(false);
        }

        private async void submitTag()
        {
            string newTag = newTagEntry.Text;


            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Training-Key", "cbf5bf3443f544e4b590c3e85ebde223");

            string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Training/projects/76ee5ef7-4837-4a7a-b9e1-18bfb5b9bb45/tags?name="+newTag;

            HttpResponseMessage response;
            HttpContent emptyContent = new StringContent("");
            response = await client.PostAsync(url,emptyContent);

            var responseString = await response.Content.ReadAsStringAsync();
            TagModel responseModel = JsonConvert.DeserializeObject<TagModel>(responseString);

            string getUrl = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Training/projects/76ee5ef7-4837-4a7a-b9e1-18bfb5b9bb45/tags";
            var getResponse = await client.GetAsync(getUrl);
            var getString = await getResponse.Content.ReadAsStringAsync();
            TagsModel tagsList = JsonConvert.DeserializeObject<TagsModel>(getString);

            tag = newTag;
            tagId = tagsList.Tags.Find(x => x.Name == tag).Id;
            await Upload(true);
        }

        private async void loadCamera(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
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


            await MakePredictionRequest(file);



        }

        static byte[] GetImageAsByteArray(MediaFile file)
        {
            var stream = file.GetStream();
            BinaryReader binaryReader = new BinaryReader(stream);
            return binaryReader.ReadBytes((int)stream.Length);
        }

        async Task postResultsAsync(string prediction, bool result)
        {

            NotMountainModel model = new NotMountainModel()
            {
                Result = result,
                Prediction = prediction
            };

            await AzureManager.AzureManagerInstance.PostMountainInformation(model);
        }

        async Task Upload(bool batch)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Training-Key", "cbf5bf3443f544e4b590c3e85ebde223");
            HttpResponseMessage response;

            byte[] byteData = GetImageAsByteArray(file);

            string postUrl = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Training/projects/76ee5ef7-4837-4a7a-b9e1-18bfb5b9bb45/images/image?tagIds={" + tagId + "}";

            using (var content = new ByteArrayContent(byteData))
            {

                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                if (!batch)
                {
                    response = await client.PostAsync(postUrl, content);
                }
                else
                {
                    response = await client.PostAsync(postUrl, content);
                    response = await client.PostAsync(postUrl, content);
                    response = await client.PostAsync(postUrl, content);
                    response = await client.PostAsync(postUrl, content);
                    response = await client.PostAsync(postUrl, content);
                }
            }


            await postResultsAsync(tag, true);

            //Get rid of file once we have finished using it
            file.Dispose();

        }

        private async void Train()
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Training-Key", "cbf5bf3443f544e4b590c3e85ebde223");
            HttpResponseMessage response;
            string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Training/projects/76ee5ef7-4837-4a7a-b9e1-18bfb5b9bb45/train";

            HttpContent emptyContent = new StringContent("");
            response = await client.PostAsync(url, emptyContent);
        }

        async Task GetIterationsId()
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Training-Key", "cbf5bf3443f544e4b590c3e85ebde223");
            HttpResponseMessage iterationResponse;
            string iterationsUrl = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Training/projects/76ee5ef7-4837-4a7a-b9e1-18bfb5b9bb45/iterations";

            iterationResponse = await client.GetAsync(iterationsUrl);
            var iterationsString = await iterationResponse.Content.ReadAsStringAsync();
            List<IterationModel> iterationsList = JsonConvert.DeserializeObject<List<IterationModel>>(iterationsString);
            iterationId = iterationsList.FindLast(x => x.Status == "Completed").Id;
        }

        async Task MakePredictionRequest(MediaFile file)
        {
            var client = new HttpClient();
            await GetIterationsId();

            client.DefaultRequestHeaders.Add("Prediction-Key", "b751bcbfca2447c6b5f320fe5c287524");

            string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Prediction/76ee5ef7-4837-4a7a-b9e1-18bfb5b9bb45/image?iterationId="+iterationId;

            HttpResponseMessage response;

            byte[] byteData = GetImageAsByteArray(file);

            using (var content = new ByteArrayContent(byteData))
            {

                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(url, content);


                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    EvaluationModel responseModel = JsonConvert.DeserializeObject<EvaluationModel>(responseString);

                    double max = responseModel.Predictions.Max(m => m.Probability);
                    var tagModel = responseModel.Predictions.Find(m => m.Probability.Equals(max));
                    tag = tagModel.Tag;
                    tagId = tagModel.TagId;

                    TagLabel.Text = (max >= 0.5) ? "Is it a " + tag + "?" : "I don't know what that is" + iterationId;

                }
                
                // Enable options buttons only after picture has been taken
                YesButton.IsEnabled = true;
                NoButton.IsEnabled = true;
                SumbitButton.IsEnabled = true;
                TrainButton.IsEnabled = true;
                
            }
        }
    }
}