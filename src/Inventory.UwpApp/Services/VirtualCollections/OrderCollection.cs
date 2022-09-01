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
    public class OrderCollection : List<OrderModel>
    {
        public OrderCollection(OrderServiceUwp orderService)
        {
        }

        internal Task LoadAsync(DataRequest<Order> request)
        {
            throw new NotImplementedException();
        }
    }
}
