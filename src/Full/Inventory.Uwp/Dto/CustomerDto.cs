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
    public class CustomerDto : ObservableDto<CustomerDto>
    {
        private string _title;
        private string _firstName;
        private string _middleName;
        private string _lastName;
        private string _suffix;
        private string _gender;
        private string _emailAddress;
        private string _addressLine1;
        private string _addressLine2;
        private string _city;
        private string _region;
        private string _postalCode;
        private string _phone;
        private DateTimeOffset? _birthDate;
        private string _education;
        private string _occupation;
        private decimal? _yearlyIncome;
        private string _maritalStatus;
        private int? _totalChildren;
        private int? _childrenAtHome;
        private bool? _isHouseOwner;
        private int? _numberCarsOwned;
        private DateTimeOffset _createdOn;
        private DateTimeOffset? _lastModifiedOn;
        private byte[] _picture;
        private byte[] _thumbnail;
        private long _countryId;
        private CountryDto _country;


        #region property

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }
        public string MiddleName
        {
            get => _middleName;
            set => SetProperty(ref _middleName, value);
        }
        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }
        public string Suffix
        {
            get => _suffix;
            set => SetProperty(ref _suffix, value);
        }
        public string Gender
        {
            get => _gender;
            set => SetProperty(ref _gender, value);
        }
        public string EmailAddress
        {
            get => _emailAddress;
            set => SetProperty(ref _emailAddress, value);
        }
        public string AddressLine1
        {
            get => _addressLine1;
            set => SetProperty(ref _addressLine1, value);
        }
        public string AddressLine2
        {
            get => _addressLine2;
            set => SetProperty(ref _addressLine2, value);
        }
        public string City
        {
            get => _city;
            set => SetProperty(ref _city, value);
        }
        public string Region
        {
            get => _region;
            set => SetProperty(ref _region, value);
        }
        public string PostalCode
        {
            get => _postalCode;
            set => SetProperty(ref _postalCode, value);
        }
        public string Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
        }
        public DateTimeOffset? BirthDate
        {
            get => _birthDate;
            set => SetProperty(ref _birthDate, value);
        }
        public string Education
        {
            get => _education;
            set => SetProperty(ref _education, value);
        }
        public string Occupation
        {
            get => _occupation;
            set => SetProperty(ref _occupation, value);
        }
        public decimal? YearlyIncome
        {
            get => _yearlyIncome;
            set => SetProperty(ref _yearlyIncome, value);
        }
        public string MaritalStatus
        {
            get => _maritalStatus;
            set => SetProperty(ref _maritalStatus, value);
        }
        public int? TotalChildren
        {
            get => _totalChildren;
            set => SetProperty(ref _totalChildren, value);
        }
        public int? ChildrenAtHome
        {
            get => _childrenAtHome;
            set => SetProperty(ref _childrenAtHome, value);
        }
        public bool? IsHouseOwner
        {
            get => _isHouseOwner;
            set => SetProperty(ref _isHouseOwner, value);
        }
        public int? NumberCarsOwned
        {
            get => _numberCarsOwned;
            set => SetProperty(ref _numberCarsOwned, value);
        }
        public DateTimeOffset CreatedOn
        {
            get => _createdOn;
            set => SetProperty(ref _createdOn, value);
        }
        public DateTimeOffset? LastModifiedOn
        {
            get => _lastModifiedOn;
            set => SetProperty(ref _lastModifiedOn, value);
        }
        public byte[] Picture
        {
            get => _picture;
            set => SetProperty(ref _picture, value);
        }
        public byte[] Thumbnail
        {
            get => _thumbnail;
            set => SetProperty(ref _thumbnail, value);
        }

        #endregion


        #region relation

        public long CountryId
        {
            get => _countryId;
            set => SetProperty(ref _countryId, value);
        }
        public CountryDto Country
        {
            get => _country;
            set => SetProperty(ref _country, value);
        }

        public IList<OrderItemDto> Items { get; set; }

        #endregion


        public string CountryName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string Initials => string.Format("{0}{1}", $"{FirstName} "[0], $"{LastName} "[0]).Trim().ToUpper();
        public string FullAddress => $"{AddressLine1} {AddressLine2}\n{City}, {Region} {PostalCode}\n{CountryName}";


        public static CustomerDto CreateEmpty() => new CustomerDto { Id = -1, IsEmpty = true };

        public override void Merge(CustomerDto source)
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
