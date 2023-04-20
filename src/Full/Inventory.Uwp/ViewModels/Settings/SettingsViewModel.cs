#region copyright
// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************
#endregion

using CommunityToolkit.Mvvm.Input;
using Inventory.Infrastructure.Common;
using Inventory.Uwp.Common;
using Inventory.Uwp.Helpers;
using Inventory.Uwp.Services;
using Inventory.Uwp.ViewModels.Common;
using Inventory.Uwp.Views.Settings;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Inventory.Uwp.ViewModels.Settings
{
    public class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel()
        {
        }


        private ElementTheme _elementTheme = ThemeSelectorService.Theme;
        public ElementTheme ElementTheme
        {
            get => _elementTheme;
            set => SetProperty(ref _elementTheme, value);
        }


        private ICommand _switchThemeCommand;
        public ICommand SwitchThemeCommand
        {
            get
            {
                if (_switchThemeCommand == null)
                {
                    _switchThemeCommand = new RelayCommand<ElementTheme>(
                        async (param) =>
                        {
                            ElementTheme = param;
                            await ThemeSelectorService.SetThemeAsync(param);
                        });
                }
                return _switchThemeCommand;
            }
        }



        public string Version => $"v{AppSettings.Current.Version}";

        private bool _isBusy = false;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private bool _isLocalProvider;
        public bool IsLocalProvider
        {
            get => _isLocalProvider;
            set { if (SetProperty(ref _isLocalProvider, value)) UpdateProvider(); }
        }

        private bool _isSqlProvider;
        public bool IsSqlProvider
        {
            get => _isSqlProvider;
            set => SetProperty(ref _isSqlProvider, value);
        }

        private string _sqlConnectionString = null;
        public string SqlConnectionString
        {
            get => _sqlConnectionString;
            set => SetProperty(ref _sqlConnectionString, value);
        }

        public bool IsRandomErrorsEnabled
        {
            get => AppSettings.Current.IsRandomErrorsEnabled;
            set => AppSettings.Current.IsRandomErrorsEnabled = value;
        }

        public ICommand ResetLocalDataCommand => new RelayCommand(OnResetLocalData);
        public ICommand ValidateSqlConnectionCommand => new RelayCommand(OnValidateSqlConnection);
        public ICommand CreateDatabaseCommand => new RelayCommand(OnCreateDatabase);
        public ICommand SaveChangesCommand => new RelayCommand(OnSaveChanges);

        public SettingsArgs ViewModelArgs { get; private set; }

        public Task LoadAsync(SettingsArgs args)
        {
            ViewModelArgs = args ?? SettingsArgs.CreateDefault();

            StatusReady();

            IsLocalProvider = AppSettings.Current.DataProvider == DataProviderType.SQLite;

            SqlConnectionString = AppSettings.Current.SQLServerConnectionString;
            IsSqlProvider = AppSettings.Current.DataProvider == DataProviderType.SQLServer;

            return Task.CompletedTask;
        }

        private void UpdateProvider()
        {
            if (IsLocalProvider && !IsSqlProvider)
            {
                AppSettings.Current.DataProvider = DataProviderType.SQLite;
            }
        }

        private async void OnResetLocalData()
        {
            IsBusy = true;
            StatusMessage("Waiting database reset...");
            var result = await ResetLocalDataProviderAsync();
            IsBusy = false;
            if (result.IsOk)
            {
                await ShowDialogAsync("Reset Local Data Provider", "Local Data Provider restore successfully.");
                StatusReady();
            }
            else
            {
                await ShowDialogAsync("Reset Local Data Provider", result.Message);
                StatusMessage(result.Message);
            }
        }

        private async void OnValidateSqlConnection()
        {
            await ValidateSqlConnectionAsync();
        }

        private async Task<bool> ValidateSqlConnectionAsync()
        {
            StatusReady();
            IsBusy = true;
            StatusMessage("Validating connection string...");

            var dialog = new ValidateConnectionDialog(SqlConnectionString);
            var res = await dialog.ShowAsync();
            Result result = res == ContentDialogResult.Secondary ? Result.Ok("Operation canceled by user") : dialog.Result;

            IsBusy = false;
            if (result.IsOk)
            {
                StatusMessage(result.Message);
                return true;
            }
            else
            {
                StatusMessage(result.Message);
                return false;
            }
        }

        private async void OnCreateDatabase()
        {
            StatusReady();
            DisableAllViews("Waiting for the database to be created...");

            var dialog = new CreateDatabaseDialog(SqlConnectionString);
            var res = await dialog.ShowAsync();
            Result result = res == ContentDialogResult.Secondary ? Result.Ok("Operation canceled by user") : dialog.Result;

            EnableOtherViews();
            EnableThisView("");
            await Task.Delay(100);
            if (result.IsOk)
            {
                StatusMessage(result.Message);
            }
            else
            {
                StatusError("Error creating database");
            }
        }

        private async void OnSaveChanges()
        {
            if (IsSqlProvider)
            {
                if (await ValidateSqlConnectionAsync())
                {
                    AppSettings.Current.SQLServerConnectionString = SqlConnectionString;
                    AppSettings.Current.DataProvider = DataProviderType.SQLServer;
                }
            }
            else
            {
                AppSettings.Current.DataProvider = DataProviderType.SQLite;
            }
        }

        private async Task<Result> ResetLocalDataProviderAsync()
        {
            Result result;
            try
            {
                var localFolder = ApplicationData.Current.LocalFolder;
                var databaseFolder = await localFolder.CreateFolderAsync(AppSettings.DatabasePath, CreationCollisionOption.OpenIfExists);
                var sourceFile = await databaseFolder.GetFileAsync(AppSettings.DatabasePattern);
                var targetFile = await databaseFolder.CreateFileAsync(AppSettings.DatabaseName, CreationCollisionOption.ReplaceExisting);
                await sourceFile.CopyAndReplaceAsync(targetFile);
                result = Result.Ok();
            }
            catch (Exception ex)
            {
                result = Result.Error(ex);
            }
            return result;
        }
    }
}
