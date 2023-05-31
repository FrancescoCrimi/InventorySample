using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Infrastructure
{
    public interface IUnitOfWork<out TContext> where TContext : DbContext
    {
        //The following Property is going to hold the context object
        TContext Context { get; }

        //Start the database Transaction
        void CreateTransaction();

        //Commit the database Transaction
        void Commit();

        //Rollback the database Transaction
        void Rollback();

        //DbContext Class SaveChanges method
        void Save();
    }
}
