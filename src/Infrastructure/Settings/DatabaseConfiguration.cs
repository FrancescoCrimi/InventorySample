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

namespace Inventory.Infrastructure.Settings
{
    public enum DatabaseProviderType
    {
        SQLite,
        SQLServer,
        MySql,
        WebAPI
    }


    public class DatabaseConfiguration
    {
        public string Key { get; }
        public DatabaseProviderType Provider { get; }
        public string ConnectionString { get; }
        public bool IsReadOnly { get; }

        public DatabaseConfiguration(string key,
                                     DatabaseProviderType provider,
                                     string cs,
                                     bool isReadOnly)
        {
            Key = key;
            Provider = provider;
            ConnectionString = cs;
            IsReadOnly = isReadOnly;
        }
    }
}