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

using System;
using System.Collections.Generic;

namespace Inventory.Uwp.Dto
{
    public class CustomerDto : ObservableDto
    {
        public static CustomerDto CreateEmpty() => new CustomerDto { Id = -1, IsEmpty = true };

        //public long Id { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string Gender { get; set; }
        public string EmailAddress { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public DateTimeOffset? BirthDate { get; set; }
        public string Education { get; set; }
        public string Occupation { get; set; }
        public decimal? YearlyIncome { get; set; }
        public string MaritalStatus { get; set; }
        public int? TotalChildren { get; set; }
        public int? ChildrenAtHome { get; set; }
        public bool? IsHouseOwner { get; set; }
        public int? NumberCarsOwned { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset? LastModifiedOn { get; set; }
        public byte[] Picture { get; set; }
        public byte[] Thumbnail { get; set; }

        public long CountryId { get; set; }

        public CountryDto Country { get; set; }
        public IList<OrderItemDto> Items { get; set; }

        public string CountryName { get; set; }
        public bool IsNew => Id <= 0;
        public string FullName => $"{FirstName} {LastName}";
        public string Initials => string.Format("{0}{1}", $"{FirstName} "[0], $"{LastName} "[0]).Trim().ToUpper();
        public string FullAddress => $"{AddressLine1} {AddressLine2}\n{City}, {Region} {PostalCode}\n{CountryName}";


        public override void Merge(ObservableDto source)
        {
            if (source is CustomerDto model)
            {
                Merge(model);
            }
        }

        public void Merge(CustomerDto source)
        {
            if (source != null)
            {
                AddressLine1 = source.AddressLine1;
                AddressLine2 = source.AddressLine2;
                BirthDate = source.BirthDate;
                ChildrenAtHome = source.ChildrenAtHome;
                City = source.City;
                CreatedOn = source.CreatedOn;
                Id = source.Id;
                Education = source.Education;
                EmailAddress = source.EmailAddress;
                FirstName = source.FirstName;
                Gender = source.Gender;
                IsEmpty = source.IsEmpty;
                IsHouseOwner = source.IsHouseOwner;
                LastModifiedOn = source.LastModifiedOn;
                LastName = source.LastName;
                MaritalStatus = source.MaritalStatus;
                MiddleName = source.MiddleName;
                NumberCarsOwned = source.NumberCarsOwned;
                Occupation = source.Occupation;
                Phone = source.Phone;
                Picture = source.Picture;
                PostalCode = source.PostalCode;
                Region = source.Region;
                Suffix = source.Suffix;
                Thumbnail = source.Thumbnail;
                Title = source.Title;
                TotalChildren = source.TotalChildren;
                YearlyIncome = source.YearlyIncome;

                CountryId = source.CountryId;

                Country = source.Country;

                CountryName = source.CountryName;
            }
        }
    }
}
