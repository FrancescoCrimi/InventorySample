// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

using Inventory.Infrastructure.Logging;
using System;
using System.Linq.Expressions;

namespace Inventory.Uwp.ViewModels.Logs
{
    public class LogListArgs
    {
        public LogListArgs()
        {
            OrderByDesc = r => r.DateTime;
        }

        public string Query { get; set; }

        public bool IsMainView { get; set; }

        public Expression<Func<Log, object>> OrderBy { get; set; }

        public Expression<Func<Log, object>> OrderByDesc { get; set; }
    }
}
