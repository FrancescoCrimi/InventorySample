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

using System;
using System.Threading.Tasks;
using System.Windows.Input;

using Inventory.Services;
using Microsoft.Toolkit.Mvvm.Input;

namespace Inventory.ViewModels
{
    #region SettingsArgs
    public class SettingsArgs
    {
        static public SettingsArgs CreateDefault() => new SettingsArgs();
    }
    #endregion

    public class SettingsViewModel : ViewModelBase
    {
        private readonly ISettingsService settingsService;

        public SettingsViewModel(ISettingsService settingsService)
            : base()
        {
            this.settingsService = settingsService;
        }

        public string Version => $"v{settingsService.Version}";

        private bool _isBusy = false;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private bool _isLocalProvider;
        public bool IsLocalProvider
        {
            get { return _isLocalProvider; }
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
            get { return settingsService.IsRandomErrorsEnabled; }
            set { settingsService.IsRandomErrorsEnabled = value; }
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

            IsLocalProvider = settingsService.DataProvider == DataProviderType.SQLite;

            SqlConnectionString = settingsService.SQLServerConnectionString;
            IsSqlProvider = settingsService.DataProvider == DataProviderType.SQLServer;

            return Task.CompletedTask;
        }

        private void UpdateProvider()
        {
            if (IsLocalProvider && !IsSqlProvider)
            {
                settingsService.DataProvider = DataProviderType.SQLite;
            }
        }

        private async void OnResetLocalData()
        {
            IsBusy = true;
            StatusMessage("Waiting database reset...");
            var result = await settingsService.ResetLocalDataProviderAsync();
            IsBusy = false;
            if (result.IsOk)
            {
                StatusReady();
            }
            else
            {
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
            var result = await settingsService.ValidateConnectionAsync(SqlConnectionString);
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
            var result = await settingsService.CreateDabaseAsync(SqlConnectionString);
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
                    settingsService.SQLServerConnectionString = SqlConnectionString;
                    settingsService.DataProvider = DataProviderType.SQLServer;
                }
            }
            else
            {
                settingsService.DataProvider = DataProviderType.SQLite;
            }
        }
    }
}
