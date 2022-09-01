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
    public class ProductCollection : List<ProductModel>
    {
        public ProductCollection(ProductServiceUwp productService)
        {
        }

        internal Task LoadAsync(DataRequest<Product> request)
        {
            throw new NotImplementedException();
        }
    }
}
