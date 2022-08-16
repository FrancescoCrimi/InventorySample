using CiccioSoft.Inventory.Application;
using CiccioSoft.Inventory.Domain.Model;
using CiccioSoft.Inventory.Infrastructure.Common;
using CiccioSoft.Inventory.Uwp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CiccioSoft.Inventory.Uwp.Services
{
    public class CustomerServiceUwp /*: ICustomerService*/
    {
        private readonly ICustomerService customerService;

        public CustomerServiceUwp(ICustomerService customerService)
        {
            this.customerService = customerService;
        }

        public Task<int> DeleteCustomerAsync(CustomerModel model)
        {
            var customer = new Customer { CustomerID = model.CustomerID };
            return customerService.DeleteCustomerAsync(customer);
        }

        public Task<int> DeleteCustomerRangeAsync(int index, int length, DataRequest<Customer> request)
        {
            return customerService.DeleteCustomerRangeAsync(index, length, request);
        }

        public async Task<CustomerModel> GetCustomerAsync(long id)
        {
            Customer customer = await customerService.GetCustomerAsync(id);
            CustomerModel model = await CreateCustomerModelAsync(customer, includeAllFields: true);
            return model;
        }

        public async Task<IList<CustomerModel>> GetCustomersAsync(int skip, int take, DataRequest<Customer> request)
        {
            var models = new List<CustomerModel>();
            var customers = await customerService.GetCustomersAsync(skip, take, request);
            foreach (var item in customers)
            {
                models.Add(await CreateCustomerModelAsync(item, includeAllFields: false));
            }
            return models;
        }

        public Task<int> GetCustomersCountAsync(DataRequest<Customer> request)
        {
            return customerService.GetCustomersCountAsync(request);
        }

        public async Task<int> UpdateCustomerAsync(CustomerModel model)
        {
            int rtn = 0;
            long id = model.CustomerID;
            Customer customer = id > 0 ? await customerService.GetCustomerAsync(model.CustomerID) : new Customer();
            if (customer != null)
            {
                UpdateCustomerFromModel(customer, model);
                rtn = await customerService.UpdateCustomerAsync(customer);
                //TODO: fix below
                var item = await customerService.GetCustomerAsync(id);
                var newmodel = await CreateCustomerModelAsync(item, includeAllFields: true);
                model.Merge(newmodel);
            }
            return rtn;
        }


        public static async Task<CustomerModel> CreateCustomerModelAsync(Customer source, bool includeAllFields)
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
                ThumbnailSource = await BitmapTools.LoadBitmapAsync(source.Thumbnail)
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
                model.PictureSource = await BitmapTools.LoadBitmapAsync(source.Picture);
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
