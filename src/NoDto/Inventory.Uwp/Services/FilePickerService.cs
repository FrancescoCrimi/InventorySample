// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace Inventory.Uwp.Services
{
    public class ImagePickerResult
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] ImageBytes { get; set; }
        public object ImageSource { get; set; }
    }

    public class FilePickerService
    {
        public async Task<ImagePickerResult> OpenImagePickerAsync()
        {
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".bmp");
            picker.FileTypeFilter.Add(".gif");

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                var bytes = await GetImageBytesAsync(file);
                return new ImagePickerResult
                {
                    FileName = file.Name,
                    ContentType = file.ContentType,
                    ImageBytes = bytes,
                    ImageSource = await LoadBitmapAsync(bytes)
                };
            }
            return null;
        }

        private static async Task<byte[]> GetImageBytesAsync(StorageFile file)
        {
            using (var randomStream = await file.OpenReadAsync())
            {
                using (var stream = randomStream.AsStream())
                {
                    byte[] buffer = new byte[randomStream.Size];
                    await stream.ReadAsync(buffer, 0, buffer.Length);
                    return buffer;
                }
            }
        }

        private static async Task<BitmapImage> LoadBitmapAsync(byte[] bytes)
        {
            if (bytes != null && bytes.Length > 0)
            {
                using (var stream = new InMemoryRandomAccessStream())
                {
                    var bitmap = new BitmapImage();
                    await stream.WriteAsync(bytes.AsBuffer());
                    stream.Seek(0);
                    await bitmap.SetSourceAsync(stream);
                    return bitmap;
                }
            }
            return null;
        }

        private static async Task<BitmapImage> LoadBitmapAsync(IRandomAccessStreamReference randomStreamReference)
        {
            var bitmap = new BitmapImage();
            try
            {
                using (var stream = await randomStreamReference.OpenReadAsync())
                {
                    await bitmap.SetSourceAsync(stream);
                }
            }
            catch { }
            return bitmap;
        }
    }
}
