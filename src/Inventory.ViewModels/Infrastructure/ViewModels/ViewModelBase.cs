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
using System.Diagnostics;
using System.Threading.Tasks;
using Inventory.Data;
using Inventory.Models;
using Inventory.Services;

namespace Inventory.ViewModels
{
    public class ViewModelBase : ObservableObject
    {
        //private Stopwatch _stopwatch = new Stopwatch();

        public ViewModelBase(/*ICommonServices commonServices*/)
        {
            //ContextService = commonServices.ContextService;
            //NavigationService = commonServices.NavigationService;
            //messageService = commonServices.MessageService;
            //DialogService = commonServices.DialogService;
            //LogService = commonServices.LogService;
        }

        public IContextService ContextService { get; }
        public INavigationService NavigationService { get; }
        public IMessageService messageService { get; }
        public IDialogService DialogService { get; }
        //public ILogService LogService { get; }

        public bool IsMainView => ContextService.IsMainView;

        virtual public string Title => String.Empty;

        public  void LogInformation(string source, string action, string message, string description)
        {
            //await LogService.WriteAsync(LogType.Information, source, action, message, description);
        }

        public  void LogWarning(string source, string action, string message, string description)
        {
            //await LogService.WriteAsync(LogType.Warning, source, action, message, description);
        }

        public void LogException(string source, string action, Exception exception)
        {
            LogError(source, action, exception.Message, exception.ToString());
        }

        public async void LogError(string source, string action, string message, string description)
        {
            //await LogService.WriteAsync(LogType.Error, source, action, message, description);
            await Task.CompletedTask;
        }

        //public void StartStatusMessage(string message)
        //{
        //    StatusMessage(message);
        //    //_stopwatch.Reset();
        //    //_stopwatch.Start();
        //}
        //public void EndStatusMessage(string message)
        //{
        //    //_stopwatch.Stop();
        //    StatusMessage($"{message} ({_stopwatch.Elapsed.TotalSeconds:#0.000} seconds)");            
        //}

        public void StatusReady()
        {
            messageService.Send(this, "StatusMessage", "Ready");
        }
        public void StatusMessage(string message)
        {
            //Microsoft.AppCenter.Analytics.Analytics.TrackEvent(message);
            messageService.Send(this, "StatusMessage", message);
        }
        public void StatusError(string message)
        {
            //Microsoft.AppCenter.Analytics.Analytics.TrackEvent(message);
            messageService.Send(this, "StatusError", message);
        }

        public void EnableThisView(string message = null)
        {
            message = message ?? "Ready";
            messageService.Send(this, "EnableThisView", message);
        }
        public void DisableThisView(string message)
        {
            messageService.Send(this, "DisableThisView", message);
        }

        public void EnableOtherViews(string message = null)
        {
            message = message ?? "Ready";
            messageService.Send(this, "EnableOtherViews", message);
        }
        public void DisableOtherViews(string message)
        {
            messageService.Send(this, "DisableOtherViews", message);
        }

        public void EnableAllViews(string message = null)
        {
            message = message ?? "Ready";
            messageService.Send(this, "EnableAllViews", message);
        }
        public void DisableAllViews(string message)
        {
            messageService.Send(this, "DisableAllViews", message);
        }
    }
}
