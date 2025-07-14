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

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Inventory.Infrastructure.Logging
{
    public class LogDbContext : DbContext
    {
        public LogDbContext(DbContextOptions<LogDbContext> options)
            : base(options)
        {
        }

        protected LogDbContext()
        {
        }

        public virtual DbSet<Log> Logs
        {
            get; set;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Log>(log =>
            {
                log.ToTable(@"logs");
                log.Property(x => x.Id)
                    .IsRequired()
                    .ValueGeneratedOnAdd();
                log.HasKey(x => x.Id);
                //log.Property(x => x.MachineName)
                //    .IsRequired();
                //log.Property(x => x.Logged)
                //   .IsRequired();
                log.Property(x => x.Level)
                    .IsRequired();
                log.Property(x => x.Message)
                    .IsRequired();
                log.Property(x => x.DateTime)
                    .HasConversion(new DateTimeOffsetToBinaryConverter());
            });
        }
    }
}
