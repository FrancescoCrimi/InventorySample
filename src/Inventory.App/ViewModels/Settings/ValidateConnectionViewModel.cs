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
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

using CiccioSoft.Inventory.Uwp.Services;
using CiccioSoft.Inventory.Data.Services;
using Microsoft.Extensions.Logging;

namespace CiccioSoft.Inventory.Uwp.ViewModels
{
    public class ValidateConnectionViewModel : ViewModelBase
    {
        private readonly ILogger<ValidateConnectionViewModel> logger;
        private readonly ISettingsService settingsService;

        public ValidateConnectionViewModel(ILogger<ValidateConnectionViewModel> logger,
                                           ISettingsService settingsService)
            : base()
        {
            this.logger = logger;
            this.settingsService = settingsService;
            Result = Result.Error("Operation cancelled");
        }

        public Result Result { get; private set; }

        private string _progressStatus = null;
        public string ProgressStatus
        {
            get => _progressStatus;
            set => SetProperty(ref _progressStatus, value);
        }

        private string _message = null;
        public string Message
        {
            get { return _message; }
            set { if (SetProperty(ref _message, value)) OnPropertyChanged(nameof(HasMessage)); }
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
                using (var db = new SQLServerDb(connectionString))
                {
                    var dbCreator = db.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
                    if (await dbCreator.ExistsAsync())
                    {
                        var version = db.DbVersion.FirstOrDefault();
                        if (version != null)
                        {
                            if (version.Version == settingsService.DbVersion)
                            {
                                Message = $"Database connection succeeded and version is up to date.";
                                Result = Result.Ok("Database connection succeeded");
                            }
                            else
                            {
                                Message = $"Database version mismatch. Current version is {version.Version}, expected version is {settingsService.DbVersion}. Please, recreate the database.";
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
            }
            catch (Exception ex)
            {
                Result = Result.Error("Error creating database. See details in Activity Log");
                Message = $"Error validating connection: {ex.Message}";
                logger.LogError(ex, "Validate Connection");
            }
            PrimaryButtonText = "Ok";
            SecondaryButtonText = null;
        }
    }
}
