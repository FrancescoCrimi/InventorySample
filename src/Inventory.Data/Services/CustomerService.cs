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

using CiccioSoft.Inventory.Data.Models;
using CiccioSoft.Inventory.Domain.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CiccioSoft.Inventory.Data.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ILogger<CustomerService> logger;
        private readonly IServiceProvider serviceProvider;

        public CustomerService(ILogger<CustomerService> logger,
                               IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }

        public async Task<CustomerModel> GetCustomerAsync(long id)
        {
            using (var dataService = serviceProvider.GetService<IDataService>())
            {
                return await GetCustomerAsync(dataService, id);
            }
        }

        public async Task<IList<CustomerModel>> GetCustomersAsync(int skip, int take, DataRequest<Customer> request)
        {
            var models = new List<CustomerModel>();
            using (var dataService = serviceProvider.GetService<IDataService>())
            {
                var items = await dataService.GetCustomersAsync(skip, take, request);
                foreach (var item in items)
                {
                    models.Add(CreateCustomerModelAsync(item, includeAllFields: false));
                }
                return models;
            }
        }

        public async Task<int> GetCustomersCountAsync(DataRequest<Customer> request)
        {
            using (var dataService = serviceProvider.GetService<IDataService>())
            {
                return await dataService.GetCustomersCountAsync(request);
            }
        }

        public async Task<int> UpdateCustomerAsync(CustomerModel model)
        {
            long id = model.CustomerID;
            using (var dataService = serviceProvider.GetService<IDataService>())
            {
                var customer = id > 0 ? await dataService.GetCustomerAsync(model.CustomerID) : new Customer();
                if (customer != null)
                {
                    UpdateCustomerFromModel(customer, model);
                    await dataService.UpdateCustomerAsync(customer);
                    model.Merge(await GetCustomerAsync(dataService, customer.CustomerID));
                }
                return 0;
            }
        }

        public async Task<int> DeleteCustomerAsync(CustomerModel model)
        {
            var customer = new Customer { CustomerID = model.CustomerID };
            using (var dataService = serviceProvider.GetService<IDataService>())
            {
                return await dataService.DeleteCustomersAsync(customer);
            }
        }

        public async Task<int> DeleteCustomerRangeAsync(int index, int length, DataRequest<Customer> request)
        {
            using (var dataService = serviceProvider.GetService<IDataService>())
            {
                var items = await dataService.GetCustomerKeysAsync(index, length, request);
                return await dataService.DeleteCustomersAsync(items.ToArray());
            }
        }


        private static async Task<CustomerModel> GetCustomerAsync(IDataService dataService, long id)
        {
            var item = await dataService.GetCustomerAsync(id);
            if (item != null)
            {
                return CreateCustomerModelAsync(item, includeAllFields: true);
            }
            return null;
        }

        public static CustomerModel CreateCustomerModelAsync(Customer source, bool includeAllFields)
        {
            var model = new CustomerModel()
            {
                CustomerID = source.CustomerID,
                Title = source.Title,
                FirstName = source.FirstName,
                MiddleName = source.MiddleName,
                LastName = source.LastName,
                Suffix = source.Suffix,
                Gender = source.Gender,
                EmailAddress = source.EmailAddress,
                AddressLine1 = source.AddressLine1,
                AddressLine2 = source.AddressLine2,
                City = source.City,
                Region = source.Region,
                CountryCode = source.CountryCode,
                PostalCode = source.PostalCode,
                Phone = source.Phone,
                CreatedOn = source.CreatedOn,
                LastModifiedOn = source.LastModifiedOn,
                Thumbnail = source.Thumbnail,
            };
            if (includeAllFields)
            {
                model.BirthDate = source.BirthDate;
                model.Education = source.Education;
                model.Occupation = source.Occupation;
                model.YearlyIncome = source.YearlyIncome;
                model.MaritalStatus = source.MaritalStatus;
                model.TotalChildren = source.TotalChildren;
                model.ChildrenAtHome = source.ChildrenAtHome;
                model.IsHouseOwner = source.IsHouseOwner;
                model.NumberCarsOwned = source.NumberCarsOwned;
                model.Picture = source.Picture;
            }
            return model;
        }

        private void UpdateCustomerFromModel(Customer target, CustomerModel source)
        {
            target.Title = source.Title;
            target.FirstName = source.FirstName;
            target.MiddleName = source.MiddleName;
            target.LastName = source.LastName;
            target.Suffix = source.Suffix;
            target.Gender = source.Gender;
            target.EmailAddress = source.EmailAddress;
            target.AddressLine1 = source.AddressLine1;
            target.AddressLine2 = source.AddressLine2;
            target.City = source.City;
            target.Region = source.Region;
            target.CountryCode = source.CountryCode;
            target.PostalCode = source.PostalCode;
            target.Phone = source.Phone;
            target.BirthDate = source.BirthDate;
            target.Education = source.Education;
            target.Occupation = source.Occupation;
            target.YearlyIncome = source.YearlyIncome;
            target.MaritalStatus = source.MaritalStatus;
            target.TotalChildren = source.TotalChildren;
            target.ChildrenAtHome = source.ChildrenAtHome;
            target.IsHouseOwner = source.IsHouseOwner;
            target.NumberCarsOwned = source.NumberCarsOwned;
            target.CreatedOn = source.CreatedOn;
            target.LastModifiedOn = source.LastModifiedOn;
            target.Picture = source.Picture;
            target.Thumbnail = source.Thumbnail;
        }
    }
}
