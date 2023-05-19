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

using Inventory.Infrastructure;
using Inventory.Infrastructure.Common;
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.ViewModels.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Inventory.Uwp.ViewModels.Settings
{
    public class ValidateConnectionViewModel : ViewModelBase
    {
        private readonly ILogger _logger;
        private readonly AppSettings _appSettings;
        private readonly IPersistenceService _persistenceService;
        private string _progressStatus = null;
        private string _message = null;
        private string _primaryButtonText;
        private string _secondaryButtonText = "Cancel";

        public ValidateConnectionViewModel(ILogger<ValidateConnectionViewModel> logger,
                                           AppSettings appSettings,
                                           IPersistenceService persistenceService)
            : base()
        {
            _logger = logger;
            _appSettings = appSettings;
            _persistenceService = persistenceService;
            Result = Result.Error("Operation cancelled");
        }

        public Result Result { get; private set; }

        public string ProgressStatus
        {
            get => _progressStatus;
            set => SetProperty(ref _progressStatus, value);
        }

        public string Message
        {
            get => _message;
            set
            {
                if (SetProperty(ref _message, value)) OnPropertyChanged(nameof(HasMessage));
            }
        }

        public bool HasMessage => _message != null;

        public string PrimaryButtonText
        {
            get => _primaryButtonText;
            set => SetProperty(ref _primaryButtonText, value);
        }

        public string SecondaryButtonText
        {
            get => _secondaryButtonText;
            set => SetProperty(ref _secondaryButtonText, value);
        }

        public async Task ExecuteAsync(string connectionString)
        {
            try
            {
                if (await _persistenceService.ExistsAsync(connectionString, DataProviderType.SQLServer))
                {
                    var version = _persistenceService.GetDbVersion(connectionString, DataProviderType.SQLServer);
                    if (version != null)
                    {
                        if (version == _appSettings.DbVersion)
                        {
                            Message = $"Database connection succeeded and version is up to date.";
                            Result = Result.Ok("Database connection succeeded");
                        }
                        else
                        {
                            Message = $"Database version mismatch. Current version is {version}, expected version is {_appSettings.DbVersion}. Please, recreate the database.";
                            Result = Result.Error("Database version mismatch");
                        }
                    }
                    else
                    {
                        Message = $"Database schema mismatch.";
                        Result = Result.Error("Database schema mismatch");
                    }
                }
                else
                {
                    Message = $"Database does not exists. Please, create the database.";
                    Result = Result.Error("Database does not exist");
                }
            }
            catch (Exception ex)
            {
                Result = Result.Error("Error creating database. See details in Activity Log");
                Message = $"Error validating connection: {ex.Message}";
                _logger.LogError(LogEvents.Settings, ex, "Validate Connection");
            }
            PrimaryButtonText = "Ok";
            SecondaryButtonText = null;
        }
    }
}
