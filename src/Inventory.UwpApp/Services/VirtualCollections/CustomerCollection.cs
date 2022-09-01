using CiccioSoft.Inventory.Domain.Model;
using CiccioSoft.Inventory.Infrastructure.Common;
using Inventory.UwpApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.UwpApp.Services
{
    public class CustomerCollection : List<CustomerModel>
    {
        public CustomerCollection(CustomerServiceUwp customerService)
        {
        }

        internal Task LoadAsync(DataRequest<Customer> request)
        {
            throw new NotImplementedException();
        }
    }
}
