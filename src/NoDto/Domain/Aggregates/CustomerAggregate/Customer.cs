// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

using Inventory.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Domain.Aggregates.CustomerAggregate
{
    public class Customer : ObservableObject<Customer>, IEquatable<Customer>
    {
        #region fields

        private string title;
        private string firstName;
        private string middleName;
        private string lastName;
        private string suffix;
        private string gender;
        private string emailAddress;
        private string addressLine1;
        private string addressLine2;
        private string city;
        private string region;
        private string postalCode;
        private string phone;
        private DateTimeOffset? birthDate;
        private string education;
        private string occupation;
        private decimal? yearlyIncome;
        private string maritalStatus;
        private int? totalChildren;
        private int? childrenAtHome;
        private bool? isHouseOwner;
        private int? numberCarsOwned;
        private DateTimeOffset createdOn;
        private DateTimeOffset? lastModifiedOn;
        private string searchTerms;
        private byte[] picture;
        private byte[] thumbnail;
        private long countryId;

        #endregion


        #region Property

        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        public string FirstName
        {
            get => firstName;
            set => SetProperty(ref firstName, value);
        }

        public string MiddleName
        {
            get => middleName;
            set => SetProperty(ref middleName, value);
        }

        public string LastName
        {
            get => lastName;
            set => SetProperty(ref lastName, value);
        }

        public string Suffix
        {
            get => suffix;
            set => SetProperty(ref suffix, value);
        }

        public string Gender
        {
            get => gender;
            set => SetProperty(ref gender, value);
        }

        public string EmailAddress
        {
            get => emailAddress;
            set => SetProperty(ref emailAddress, value);
        }

        public string AddressLine1
        {
            get => addressLine1;
            set => SetProperty(ref addressLine1, value);
        }

        public string AddressLine2
        {
            get => addressLine2;
            set => SetProperty(ref addressLine2, value);
        }

        public string City
        {
            get => city;
            set => SetProperty(ref city, value);
        }

        public string Region
        {
            get => region;
            set => SetProperty(ref region, value);
        }

        public string PostalCode
        {
            get => postalCode;
            set => SetProperty(ref postalCode, value);
        }

        public string Phone
        {
            get => phone;
            set => SetProperty(ref phone, value);
        }

        public DateTimeOffset? BirthDate
        {
            get => birthDate;
            set => SetProperty(ref birthDate, value);
        }

        public string Education
        {
            get => education;
            set => SetProperty(ref education, value);
        }

        public string Occupation
        {
            get => occupation;
            set => SetProperty(ref occupation, value);
        }

        public decimal? YearlyIncome
        {
            get => yearlyIncome;
            set => SetProperty(ref yearlyIncome, value);
        }
        public string MaritalStatus
        {
            get => maritalStatus;
            set => SetProperty(ref maritalStatus, value);
        }

        public int? TotalChildren
        {
            get => totalChildren;
            set => SetProperty(ref totalChildren, value);
        }

        public int? ChildrenAtHome
        {
            get => childrenAtHome;
            set => SetProperty(ref childrenAtHome, value);
        }

        public bool? IsHouseOwner
        {
            get => isHouseOwner;
            set => SetProperty(ref isHouseOwner, value);
        }

        public int? NumberCarsOwned
        {
            get => numberCarsOwned;
            set => SetProperty(ref numberCarsOwned, value);
        }

        public DateTimeOffset CreatedOn
        {
            get => createdOn;
            set => SetProperty(ref createdOn, value);
        }

        public DateTimeOffset? LastModifiedOn
        {
            get => lastModifiedOn;
            set => SetProperty(ref lastModifiedOn, value);
        }

        public string SearchTerms
        {
            get => searchTerms;
            set => SetProperty(ref searchTerms, value);
        }

        public byte[] Picture
        {
            get => picture;
            set => SetProperty(ref picture, value);
        }

        public byte[] Thumbnail
        {
            get => thumbnail;
            set => SetProperty(ref thumbnail, value);
        }

        #endregion


        #region relation

        public long CountryId
        {
            get => countryId;
            set => SetProperty(ref countryId, value);
        }

        public virtual Country Country { get; set; }

        #endregion


        #region not mapped

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        [NotMapped]
        public string Initials => string.Format("{0}{1}", $"{FirstName} "[0], $"{LastName} "[0]).Trim().ToUpper();

        [NotMapped]
        public string CountryName => Country == null ? string.Empty : Country.Name;

        [NotMapped]
        public string FullAddress => $"{AddressLine1} {AddressLine2}\n{City}, {Region} {PostalCode}\n{CountryName}";

        #endregion


        #region method

        public string BuildSearchTerms() => $"{Id} {FirstName} {LastName} {EmailAddress} {AddressLine1}".ToLower();

        #endregion


        #region Equals

        public override bool Equals(object obj)
        {
            return Equals(obj as Customer);
        }

        public bool Equals(Customer other)
        {
            return !(other is null) &&
                   Id == other.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public static bool operator ==(Customer left, Customer right)
        {
            return EqualityComparer<Customer>.Default.Equals(left, right);
        }

        public static bool operator !=(Customer left, Customer right)
        {
            return !(left == right);
        }

        #endregion
    }
}
