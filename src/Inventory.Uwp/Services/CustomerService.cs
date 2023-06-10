﻿using Inventory.Domain.Model;
using Inventory.Domain.Repository;
using Inventory.Infrastructure.Common;
using Inventory.Uwp.Dto;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.Uwp.Services
{
    public class CustomerService
    {
        private readonly ILogger<CustomerService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public CustomerService(ILogger<CustomerService> logger,
                                     IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task<int> DeleteCustomerAsync(CustomerDto model)
        {
            using (var customerRepository = _serviceProvider.GetService<ICustomerRepository>())
            {
                var customer = await customerRepository.GetCustomerAsync(model.Id);
                return await customerRepository.DeleteCustomersAsync(customer);
            }
        }

        public async Task<int> DeleteCustomerRangeAsync(int index,
                                                        int length,
                                                        DataRequest<Customer> request)
        {
            using (var customerRepository = _serviceProvider.GetService<ICustomerRepository>())
            {
                var items = await customerRepository.GetCustomerKeysAsync(index, length, request);
                return await customerRepository.DeleteCustomersAsync(items.ToArray());
            }
        }

        public async Task<CustomerDto> GetCustomerAsync(long id)
        {
            using (var customerRepository = _serviceProvider.GetService<ICustomerRepository>())
            {
                var customer = await customerRepository.GetCustomerAsync(id);
                CustomerDto model = DtoAssembler.DtoFromCustomer(customer);
                return model;
            }
        }

        public async Task<List<CustomerDto>> GetCustomersAsync(int skip,
                                                               int take,
                                                               DataRequest<Customer> request,
                                                               Windows.UI.Core.CoreDispatcher dispatcher = null)
        {
            using (var customerRepository = _serviceProvider.GetService<ICustomerRepository>())
            {
                var models = new List<CustomerDto>();
                var items = await customerRepository.GetCustomersAsync(skip, take, request);
                foreach (var item in items)
                {
                    var dto = DtoAssembler.DtoFromCustomer(item);
                    models.Add(dto);
                }
                return models;
            }
        }

        public async Task<int> GetCustomersCountAsync(DataRequest<Customer> request)
        {
            using (var customerRepository = _serviceProvider.GetService<ICustomerRepository>())
            {
                return await customerRepository.GetCustomersCountAsync(request);
            }
        }

        public async Task<int> UpdateCustomerAsync(CustomerDto model)
        {
            using (var customerRepository = _serviceProvider.GetService<ICustomerRepository>())
            {
                int rtn = 0;
                long id = model.Id;
                Customer customer = id > 0 ? await customerRepository.GetCustomerAsync(model.Id) : new Customer();
                if (customer != null)
                {
                    DtoAssembler.UpdateCustomerFromDto(customer, model);
                    rtn = await customerRepository.UpdateCustomerAsync(customer);
                    //TODO: fix below
                    var item = await customerRepository.GetCustomerAsync(id);
                    var newmodel = DtoAssembler.DtoFromCustomer(item);
                    model.Merge(newmodel);
                }
                return rtn;
            }
        }
    }
}
