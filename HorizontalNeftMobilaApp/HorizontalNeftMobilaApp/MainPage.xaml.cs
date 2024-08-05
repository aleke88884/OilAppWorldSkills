using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace HorizontalNeftMobilaApp
{
    public partial class MainPage : ContentPage
    {
        public static string baseUrl = "http://10.0.2.2:8074/api";
        private readonly HttpClient httpClient = new HttpClient();
        private int allAssetsCount = 0;
        private List<Asset> allAssets;
        private double width = 0;
        private double height = 0;
        public MainPage()
        {
            InitializeComponent();
            width = this.Width;
            height = this.Height;
            LoadComboboxesAsync();
            LoadAssetsAsync();
            
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if(this.width != width || this.height != height)
            {
                this.width = width;
                this.height = height;

                UpdateLayout();
            }
        }

        private void UpdateLayout()
        {
            if(width>height)
            {
                VerticalLayout.IsVisible = false;
                HorizontalLayout.IsVisible = true;
            }
            else
            {
                HorizontalLayout.IsVisible = false;
                VerticalLayout.IsVisible = true;
            }
        }

        private async Task LoadComboboxesAsync()
        {
            try
            {
                var departmentResponse = await httpClient.GetStringAsync("http://10.0.2.2:8074/api/Departments");
                var departmentList = JsonConvert.DeserializeObject<List<Department>>(departmentResponse);
                var listd = departmentList.Select(s=>s.Name).ToList();
                listd.Insert(0,"All");
                ComboboxDepartment.ItemsSource = listd ;

                var assetGroupsResponse = await httpClient.GetStringAsync("http://10.0.2.2:8074/api/AssetGroups");
                var assetList = JsonConvert.DeserializeObject<List<AssetGroup>>(assetGroupsResponse);
                var lista = assetList.Select(s => s.Name).ToList();
                lista.Insert(0, "All");
                ComboboxAssetGroup.ItemsSource = lista;
            }catch (Exception ex)
            {
                await DisplayAlert("Error!", "Could not load departments and groups: " + ex.Message, "Ok");
            }
        }

        private async Task LoadAssetsAsync()
        {
            try
            {
                // Проверка базового URL
                var testResponse = await httpClient.GetAsync("http://10.0.2.2:8074/api/assets");
                if (!testResponse.IsSuccessStatusCode)
                {
                    await DisplayAlert("Error", "Base URL is not correct: " + testResponse.StatusCode, "Ok");
                    return;
                }

                // Получаем список активов
                var response = await httpClient.GetStringAsync("http://10.0.2.2:8074/api/assets");
                var assets = JsonConvert.DeserializeObject<List<Asset>>(response);
                allAssets = assets;
                allAssetsCount = assets.Count();
                // Получаем DepartmentLocation для каждого актива
                foreach (var asset in assets)
                {
                    var departmentLocationResponse = await httpClient.GetAsync("http://10.0.2.2:8074/api/DepartmentLocations/" + asset.DepartmentLocationId);
                    if (!departmentLocationResponse.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Error", "DepartmentLocation URL is not correct: " + departmentLocationResponse.StatusCode, "Ok");
                        return;
                    }
                    var departmentLocationJson = await departmentLocationResponse.Content.ReadAsStringAsync();
                    var departmentLocation = JsonConvert.DeserializeObject<DepartmentLocation>(departmentLocationJson);
                    var dep = await httpClient.GetStringAsync("http://10.0.2.2:8074/api/Departments/" + departmentLocation.DepartmentId);
                    var depObject = JsonConvert.DeserializeObject<Department>(dep);
                    departmentLocation.Department = depObject;
                    asset.DepartmentLocation = departmentLocation;

                    // Получаем Location для каждого DepartmentLocation
                    var locationResponse = await httpClient.GetAsync("http://10.0.2.2:8074/api/Locations/" + departmentLocation.LocationId);
                    if (!locationResponse.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Error", "Location URL is not correct: " + locationResponse.StatusCode, "Ok");
                        return;
                    }
                    var locationJson = await locationResponse.Content.ReadAsStringAsync();
                    var location = JsonConvert.DeserializeObject<Location>(locationJson);
                    asset.DepartmentLocation.Location = location;
                }

                // Устанавливаем источник данных для ListView
                ListViewAssets.ItemsSource = assets;
                ListViewAssetsHorizontal.ItemsSource = assets;
                UpdateItemCount();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Could not load assets: " + ex.Message, "Ok");
            }
        }

        private async void BtnAddNewAsset_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddNewAssetPage());
        }

        private void ComboboxDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            ApplyFilter();
        }

        private void ComboboxAssetGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void DatePickerFrom_DateSelected(object sender, DateChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void DatePickerTo_DateSelected(object sender, DateChangedEventArgs e)
        {
            ApplyFilter();
        }
        private void ApplyFilter()
        {
            var filteredAssets = allAssets;
            if (DatePickerFrom.Date != DateTime.MinValue && DatePickerTo.Date!=DateTime.MinValue) { 

                var startDate = DatePickerFrom.Date;
                var endDate = DatePickerTo.Date;    
                filteredAssets = filteredAssets.Where(a=>a.DepartmentLocation.StartDate<= startDate && a.DepartmentLocation.EndDate<= endDate).ToList();
            }

            if (ComboboxAssetGroup.SelectedItem != null && ComboboxAssetGroup.SelectedItem.ToString() != "All")
            {
                var selectedAssetGroup = ComboboxAssetGroup.SelectedItem.ToString();
                filteredAssets = filteredAssets.Where(asset => asset.AssetGroup != null && asset.AssetGroup.Name == selectedAssetGroup).ToList();
            }

            // Фильтрация по Department
            if (ComboboxDepartment.SelectedItem != null && ComboboxDepartment.SelectedItem.ToString() != "All")
            {
                var selectedDepartment = ComboboxDepartment.SelectedItem.ToString();
                filteredAssets = filteredAssets.Where(a => a.DepartmentLocation != null && a.DepartmentLocation.Department != null && a.DepartmentLocation.Department.Name == selectedDepartment).ToList();
            }
            if (!string.IsNullOrEmpty(SearchBar.Text) && SearchBar.Text.Length>2)
            {
                var searchText = SearchBar.Text;
                filteredAssets = filteredAssets.Where(s=>s.AssetName.ToLower().Contains(searchText)).ToList();
            }
            ListViewAssets.ItemsSource = filteredAssets;
            ListViewAssetsHorizontal.ItemsSource = filteredAssets;
            UpdateItemCount();
        }
        private void UpdateItemCount()
        {
            var count = (ListViewAssets.ItemsSource as List<Asset>)?.Count ?? 0;
            ItemAmount.Text = $"{count} assets from {allAssetsCount}";
        }
        private void ListViewAssets_SizeChanged(object sender, EventArgs e)
        {
            
        }
    }
    public class AssetsResponse
    {
        public List<Asset> assets { get; set; }
    }
}
