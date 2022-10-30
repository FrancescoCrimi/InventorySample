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
using CommunityToolkit.Mvvm.DependencyInjection;
using Inventory.Infrastructure.Common;
using Inventory.Persistence;
using Microsoft.Extensions.Logging;

namespace CiccioSoft.Inventory.Uwp.ViewModels
{
    public class CreateDatabaseViewModel : ViewModelBase
    {
        private readonly ILogger<CreateDatabaseViewModel> logger;

        public CreateDatabaseViewModel(ILogger<CreateDatabaseViewModel> logger)
            : base()
        {
            this.logger = logger;
            Result = Result.Error("Operation cancelled");
        }

        public Result Result
        {
            get; private set;
        }

        private string _progressStatus = null;
        public string ProgressStatus
        {
            get => _progressStatus;
            set => SetProperty(ref _progressStatus, value);
        }

        private double _progressMaximum = 1;
        public double ProgressMaximum
        {
            get => _progressMaximum;
            set => SetProperty(ref _progressMaximum, value);
        }

        private double _progressValue = 0;
        public double ProgressValue
        {
            get => _progressValue;
            set => SetProperty(ref _progressValue, value);
        }

        private string _message = null;
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                if (SetProperty(ref _message, value)) OnPropertyChanged(nameof(HasMessage));
            }
        }

        public bool HasMessage => _message != null;

        private string _primaryButtonText;
        public string PrimaryButtonText
        {
            get => _primaryButtonText;
            set => SetProperty(ref _primaryButtonText, value);
        }

        private string _secondaryButtonText = "Cancel";
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

                using (var db = new DatabaseSettings(connectionString, DataProviderType.SQLServer, Ioc.Default))
                {
                    if (!await db.ExistsAsync())
                    {
                        ProgressValue = 1;
                        ProgressStatus = "Creating Database...";
                        await db.EnsureCreatedAsync();
                        ProgressValue = 2;
                        await db.CopyDataTables(SetValue, SetStatus);
                        ProgressValue = 14;
                        Message = "Database created successfully.";
                        Result = Result.Ok("Database created successfully.");
                    }
                    else
                    {
                        ProgressValue = 14;
                        Message = $"Database already exists. Please, delete database and try again.";
                        Result = Result.Error("Database already exist");
                    }
                }
            }
            catch (Exception ex)
            {
                Result = Result.Error("Error creating database. See details in Activity Log");
                Message = $"Error creating database: {ex.Message}";
                logger.LogError(ex, "Create Database");
            }
            PrimaryButtonText = "Ok";
            SecondaryButtonText = null;
        }

        private void SetValue(double value) => ProgressValue = value;
        private void SetStatus(string status) => ProgressStatus = status;
    }
}
