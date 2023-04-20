using Inventory.Domain.Model;
using System.Linq;

namespace Inventory.Uwp.Dto
{
    public static partial class DtoAssembler
    {
        public static CustomerDto DtoFromCustomer(Customer source)
        {
            var model = new CustomerDto
            {
                Id = source.Id,
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
                PostalCode = source.PostalCode,
                Phone = source.Phone,
                CreatedOn = source.CreatedOn,
                LastModifiedOn = source.LastModifiedOn,
                Thumbnail = source.Thumbnail,
                BirthDate = source.BirthDate,
                Education = source.Education,
                Occupation = source.Occupation,
                YearlyIncome = source.YearlyIncome,
                MaritalStatus = source.MaritalStatus,
                TotalChildren = source.TotalChildren,
                ChildrenAtHome = source.ChildrenAtHome,
                IsHouseOwner = source.IsHouseOwner,
                NumberCarsOwned = source.NumberCarsOwned,
                Picture = source.Picture,

                CountryId = source.CountryId,
                Country = _lookupTable.Countries.FirstOrDefault(c => c.Id == source.CountryId),
            };
            model.CountryName = model.Country.Name;
            return model;
        }

        public static void UpdateCustomerFromDto(Customer target, CustomerDto source)
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

            target.CountryId = source.CountryId;
        }

        public static CustomerDto CloneCustomerDto(CustomerDto source)
        {
            CustomerDto merged = new CustomerDto();
            if (source != null)
            {
                merged.Id = source.Id;
                merged.Title = source.Title;
                merged.FirstName = source.FirstName;
                merged.MiddleName = source.MiddleName;
                merged.LastName = source.LastName;
                merged.Suffix = source.Suffix;
                merged.Gender = source.Gender;
                merged.EmailAddress = source.EmailAddress;
                merged.AddressLine1 = source.AddressLine1;
                merged.AddressLine2 = source.AddressLine2;
                merged.City = source.City;
                merged.Region = source.Region;
                merged.CountryId = source.CountryId;
                merged.PostalCode = source.PostalCode;
                merged.Phone = source.Phone;
                merged.BirthDate = source.BirthDate;
                merged.Education = source.Education;
                merged.Occupation = source.Occupation;
                merged.YearlyIncome = source.YearlyIncome;
                merged.MaritalStatus = source.MaritalStatus;
                merged.TotalChildren = source.TotalChildren;
                merged.ChildrenAtHome = source.ChildrenAtHome;
                merged.IsHouseOwner = source.IsHouseOwner;
                merged.NumberCarsOwned = source.NumberCarsOwned;
                merged.CreatedOn = source.CreatedOn;
                merged.LastModifiedOn = source.LastModifiedOn;
                merged.Thumbnail = source.Thumbnail;
                merged.Picture = source.Picture;
            }
            return merged;
        }
    }
}
