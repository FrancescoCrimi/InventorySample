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

using Microsoft.Extensions.Logging;
using System;

namespace Inventory.Infrastructure.Logging
{
    public class Log
    {
        public long Id { get; set; }
        public bool IsRead { get; set; }
        public string Name { get; set; }
        public DateTimeOffset DateTime { get; set; }
        public string User { get; set; }
        public LogLevel Level { get; set; }
        public string Source { get; set; }
        public string Action { get; set; }
        public string Message { get; set; }
        public string Description { get; set; }
    }
}
