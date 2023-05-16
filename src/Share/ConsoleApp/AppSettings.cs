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
using System.Threading.Tasks;

namespace ConsoleApp
{
    internal class AppSettings : IAppSettings
    {
        public string DbVersion { get; set; }

        public bool IsRandomErrorsEnabled { get; set; }

        public string Version { get; set; }

        public DataProviderType DataProvider { get; set; }

        public string SQLiteConnectionString { get; set; }

        public string SQLServerConnectionString { get; set; }

        public string AppLogConnectionString { get; set; }

        public Task EnsureLocalDatabaseAsync()
        {
            return Task.CompletedTask;
        }

        public Task EnsureLogDatabaseAsync()
        {
            return Task.CompletedTask;
        }

        public Task<Result> ResetLocalDatabaseAsync()
        {
            return (Task<Result>)Task.CompletedTask;
        }
    }
}