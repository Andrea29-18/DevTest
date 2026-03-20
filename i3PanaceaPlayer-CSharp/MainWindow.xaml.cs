using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace i3PanaceaPlayer
{
    public partial class MainWindow : Window
    {
        private static readonly HttpClient client = new HttpClient();
        private string authToken = "";
        private const string BaseUrl = "http://api.i3panacea.com";

        public MainWindow()
        {
            InitializeComponent();
        }

        // ── LOGIN ──────────────────────────────────────────────
        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorText.Text = "";
            string room = RoomNumberBox.Text.Trim();
            string pin = PinBox.Password.Trim();

            if (string.IsNullOrEmpty(room) || string.IsNullOrEmpty(pin))
            {
                ErrorText.Text = "Please enter room number and PIN.";
                return;
            }

            try
            {
                var body = new { room_number = room, pin = pin };
                var json = JsonConvert.SerializeObject(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{BaseUrl}/api/auth/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    dynamic result = JsonConvert.DeserializeObject(responseJson);
                    authToken = result.token;

                    WelcomeText.Text = $"Welcome, {result.user.display_name}";
                    LoginPanel.Visibility = Visibility.Collapsed;
                    ContentPanel.Visibility = Visibility.Visible;
                    MainPanel.Visibility = Visibility.Visible;

                    await LoadContent();
                }
                else
                {
                    ErrorText.Text = "Invalid room number or PIN.";
                }
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"Connection error: {ex.Message}";
            }
        }

        // ── LOAD CONTENT ───────────────────────────────────────
        private async System.Threading.Tasks.Task LoadContent()
        {
            try
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {authToken}");

                var response = await client.GetAsync($"{BaseUrl}/api/content?page=1&page_size=50");

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ContentResponse>(responseJson);
                    ContentList.ItemsSource = result.Items;
                }
                else
                {
                    PlayerStatus.Text = "Failed to load content.";
                }
            }
            catch (Exception ex)
            {
                PlayerStatus.Text = $"Error: {ex.Message}";
            }
        }

        // ── PLAY VIDEO ─────────────────────────────────────────
        private void ContentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ContentList.SelectedItem is ContentItem selected)
            {
                try
                {
                    PlayerStatus.Visibility = Visibility.Collapsed;
                    VideoPlayer.Source = new Uri(selected.StreamUrl);
                    VideoPlayer.Play();
                }
                catch (Exception ex)
                {
                    PlayerStatus.Visibility = Visibility.Visible;
                    PlayerStatus.Text = $"Cannot play this stream: {selected.Title}\n{ex.Message}";
                }
            }
        }

        // ── LOGOUT ─────────────────────────────────────────────
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            VideoPlayer.Stop();
            VideoPlayer.Source = null;
            ContentList.ItemsSource = null;
            authToken = "";
            client.DefaultRequestHeaders.Clear();

            LoginPanel.Visibility = Visibility.Visible;
            ContentPanel.Visibility = Visibility.Collapsed;
            MainPanel.Visibility = Visibility.Collapsed;
            PlayerStatus.Visibility = Visibility.Visible;
            PlayerStatus.Text = "Select a video to play";
            RoomNumberBox.Text = "";
            PinBox.Password = "";
            ErrorText.Text = "";
        }
    }

    // ── MODELS ─────────────────────────────────────────────────
    public class ContentResponse
    {
        [JsonProperty("items")]
        public List<ContentItem> Items { get; set; }
    }

    public class ContentItem
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("category_name")]
        public string CategoryName { get; set; }

        [JsonProperty("stream_url")]
        public string StreamUrl { get; set; }

        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("rating")]
        public string Rating { get; set; }
    }
}