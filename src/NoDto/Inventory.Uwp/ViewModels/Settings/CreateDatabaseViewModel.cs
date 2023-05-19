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
    public class CreateDatabaseViewModel : ViewModelBase
    {
        private readonly ILogger _logger;
        private readonly IPersistenceService _persistenceService;
        private string _progressStatus = null;
        private double _progressMaximum = 1;
        private double _progressValue = 0;
        private string _message = null;
        private string _primaryButtonText;
        private string _secondaryButtonText = "Cancel";

        public CreateDatabaseViewModel(ILogger<CreateDatabaseViewModel> logger,
                                       IPersistenceService persistenceService)
            : base()
        {
            _logger = logger;
            _persistenceService = persistenceService;
            Result = Result.Error("Operation cancelled");
        }

        public Result Result { get; private set; }

        public string ProgressStatus
        {
            get => _progressStatus;
            set => SetProperty(ref _progressStatus, value);
        }

        public double ProgressMaximum
        {
            get => _progressMaximum;
            set => SetProperty(ref _progressMaximum, value);
        }

        public double ProgressValue
        {
            get => _progressValue;
            set => SetProperty(ref _progressValue, value);
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
                ProgressMaximum = 14;
                ProgressStatus = "Connecting to Database";

                if (!await _persistenceService.ExistsAsync(connectionString, DataProviderType.SQLServer))
                {
                    
                    await _persistenceService.CopyDatabase(connectionString,
                                                           DataProviderType.SQLServer,
                                                           (value) => ProgressValue = value,
                                                           (status) => ProgressStatus = status);
                    Result = Result.Ok("Database created successfully.");
                }
                else
                {
                    ProgressValue = 14;
                    Message = $"Database already exists. Please, delete database and try again.";
                    Result = Result.Error("Database already exist");
                }
            }
            catch (Exception ex)
            {
                Result = Result.Error("Error creating database. See details in Activity Log");
                Message = $"Error creating database: {ex.Message}";
                _logger.LogError(LogEvents.Settings, ex, "Create Database");
            }
            PrimaryButtonText = "Ok";
            SecondaryButtonText = null;
        }

        //private void SetValue(double value) => ProgressValue = value;
        //private void SetStatus(string status) => ProgressStatus = status;
    }
}
