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
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Inventory.Uwp.Services
{
    public class DialogService
    {
        private static DialogService current;

        public static DialogService Current => current ?? (current = new DialogService());

        public async Task ShowAsync(string title,
                                    Exception ex,
                                    string ok = "Ok",
                                    XamlRoot xamlRoot = null)
        {
            await ShowAsync(title, ex.Message, ok, null, xamlRoot);
        }

        public async Task<bool> ShowAsync(string title,
                                          string content,
                                          string ok = "Ok",
                                          string cancel = null,
                                          XamlRoot xamlRoot = null)
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = content,
                PrimaryButtonText = ok,
                XamlRoot = xamlRoot
            };
            if (cancel != null)
            {
                dialog.SecondaryButtonText = cancel;
            }
            if (xamlRoot != null)
            {
                dialog.XamlRoot = xamlRoot;
            }
            var result = await dialog.ShowAsync();
            return result == ContentDialogResult.Primary;
        }
    }
}
