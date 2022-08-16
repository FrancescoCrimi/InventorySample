using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace CiccioSoft.Inventory.Uwp.Services.Infrastructure
{
    public class SettingsService
    {
        public SettingsService()
        {
        }

        public static async Task<Result> ResetLocalDataProviderAsync()
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
