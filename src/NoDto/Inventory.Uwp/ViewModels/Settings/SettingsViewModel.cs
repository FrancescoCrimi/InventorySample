// Copyright (c) Microsoft. All rights reserved.
// Copyright (c) 2023 Francesco Crimi francrim@gmail.com
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

using CommunityToolkit.Mvvm.Input;
using Inventory.Infrastructure;
using Inventory.Infrastructure.Common;
using Inventory.Uwp.Common;
using Inventory.Uwp.Services;
using Inventory.Uwp.ViewModels.Common;
using Inventory.Uwp.Views.Settings;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Inventory.Uwp.ViewModels.Settings
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly ILogger _logger;
        private readonly AppSettings _appSettings;
        private ElementTheme _elementTheme = ThemeSelectorService.Theme;
        private bool _isBusy = false;
        private bool _isLocalProvider;
        private bool _isSqlProvider;
        private string _sqlConnectionString = null;
        private AsyncRelayCommand<ElementTheme> _switchThemeCommand;
        private AsyncRelayCommand _resetLocalDataCommand;
        private AsyncRelayCommand _validateSqlConnectionCommand;
        private AsyncRelayCommand _createDatabaseCommand;
        private AsyncRelayCommand _saveChangesCommand;

        public SettingsViewModel(ILogger<SettingsViewModel> logger,
                                 AppSettings appSettings)
        {
            _logger = logger;
            _appSettings = appSettings;
        }

        public Task LoadAsync(SettingsArgs args)
        {
            ViewModelArgs = args ?? SettingsArgs.CreateDefault();

            StatusReady();

            IsLocalProvider = _appSettings.DataProvider == DataProviderType.SQLite;

            SqlConnectionString = _appSettings.SQLServerConnectionString;
            IsSqlProvider = _appSettings.DataProvider == DataProviderType.SQLServer;

            return Task.CompletedTask;
        }


        public ElementTheme ElementTheme
        {
            get => _elementTheme;
            set => SetProperty(ref _elementTheme, value);
        }

        public string Version => $"v{_appSettings.Version}";

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public bool IsLocalProvider
        {
            get => _isLocalProvider;
            set
            {
                if (SetProperty(ref _isLocalProvider, value)) UpdateProvider();
            }
        }

        public bool IsSqlProvider
        {
            get => _isSqlProvider;
            set => SetProperty(ref _isSqlProvider, value);
        }

        public string SqlConnectionString
        {
            get => _sqlConnectionString;
            set => SetProperty(ref _sqlConnectionString, value);
        }

        public bool IsRandomErrorsEnabled
        {
            get => _appSettings.IsRandomErrorsEnabled;
            set => _appSettings.IsRandomErrorsEnabled = value;
        }

        public SettingsArgs ViewModelArgs { get; private set; }


        #region command

        public ICommand SwitchThemeCommand => _switchThemeCommand
            ?? (_switchThemeCommand = new AsyncRelayCommand<ElementTheme>(async (param) => 
            {
                ElementTheme = param;
                await ThemeSelectorService.SetThemeAsync(param);
            }));

        public ICommand ResetLocalDataCommand => _resetLocalDataCommand
            ?? (_resetLocalDataCommand = new AsyncRelayCommand(OnResetLocalData));

        public ICommand ValidateSqlConnectionCommand => _validateSqlConnectionCommand
            ?? (_validateSqlConnectionCommand = new AsyncRelayCommand(OnValidateSqlConnection));

        public ICommand CreateDatabaseCommand => _createDatabaseCommand
            ?? (_createDatabaseCommand = new AsyncRelayCommand(OnCreateDatabase));

        public ICommand SaveChangesCommand => _saveChangesCommand
            ?? (_saveChangesCommand = new AsyncRelayCommand(OnSaveChanges));

        #endregion


        #region private method

        private async Task OnResetLocalData()
        {
            IsBusy = true;
            StatusMessage("Waiting database reset...");
            var result = await _appSettings.ResetLocalDatabaseAsync();
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

        private async Task OnValidateSqlConnection()
        {
            await ValidateSqlConnectionAsync();
        }

        private async Task OnCreateDatabase()
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

        private async Task OnSaveChanges()
        {
            if (IsSqlProvider)
            {
                if (await ValidateSqlConnectionAsync())
                {
                    _appSettings.SQLServerConnectionString = SqlConnectionString;
                    _appSettings.DataProvider = DataProviderType.SQLServer;
                }
            }
            else
            {
                _appSettings.DataProvider = DataProviderType.SQLite;
            }
        }

        private void UpdateProvider()
        {
            if (IsLocalProvider && !IsSqlProvider)
            {
                _appSettings.DataProvider = DataProviderType.SQLite;
            }
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

        #endregion
    }
}
