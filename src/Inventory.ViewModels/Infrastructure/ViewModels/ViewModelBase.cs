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

using Inventory.Data;
using Inventory.Models;
using Inventory.Services;
using Microsoft.Toolkit.Mvvm.DependencyInjection;

namespace Inventory.ViewModels
{
    public class ViewModelBase : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableRecipient
    {
        private Stopwatch _stopwatch = new Stopwatch();

        public ViewModelBase()
        {
            ContextService = Ioc.Default.GetService<IContextService>();
            //NavigationService = Ioc.Default.GetService<INavigationService>();
            MessageService = Ioc.Default.GetService<IMessageService>();
            //DialogService = Ioc.Default.GetService<IDialogService>();
            //LogService = Ioc.Default.GetService<ILogService>();
        }

        public IContextService ContextService { get; }
        //public INavigationService NavigationService { get; }
        public IMessageService MessageService { get; }
        //public IDialogService DialogService { get; }
        //public ILogService LogService { get; }

        public bool IsMainView => ContextService.IsMainView;

        virtual public string Title => String.Empty;

        public void StartStatusMessage(string message)
        {
            StatusMessage(message);
            _stopwatch.Reset();
            _stopwatch.Start();
        }
        public void EndStatusMessage(string message)
        {
            _stopwatch.Stop();
            StatusMessage($"{message} ({_stopwatch.Elapsed.TotalSeconds:#0.000} seconds)");            
        }

        public void StatusReady()
        {
            MessageService.Send(this, "StatusMessage", "Ready");
        }
        public void StatusMessage(string message)
        {
            //Microsoft.AppCenter.Analytics.Analytics.TrackEvent(message);
            MessageService.Send(this, "StatusMessage", message);
        }
        public void StatusError(string message)
        {
            //Microsoft.AppCenter.Analytics.Analytics.TrackEvent(message);
            MessageService.Send(this, "StatusError", message);
        }

        public void EnableThisView(string message = null)
        {
            message = message ?? "Ready";
            MessageService.Send(this, "EnableThisView", message);
        }
        public void DisableThisView(string message)
        {
            MessageService.Send(this, "DisableThisView", message);
        }

        public void EnableOtherViews(string message = null)
        {
            message = message ?? "Ready";
            MessageService.Send(this, "EnableOtherViews", message);
        }
        public void DisableOtherViews(string message)
        {
            MessageService.Send(this, "DisableOtherViews", message);
        }

        public void EnableAllViews(string message = null)
        {
            message = message ?? "Ready";
            MessageService.Send(this, "EnableAllViews", message);
        }
        public void DisableAllViews(string message)
        {
            MessageService.Send(this, "DisableAllViews", message);
        }
    }
}
