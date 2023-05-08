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
using System.ComponentModel.DataAnnotations.Schema;
using Inventory.Infrastructure.DomainBase;

namespace Inventory.Domain.Model
{
    public class Customer : Entity<Customer>
    {
        #region property

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
        public string SearchTerms { get; set; }
        public byte[] Picture { get; set; }
        public byte[] Thumbnail { get; set; }

        #endregion


        #region relation

        public long CountryId { get; set; }
        public virtual Country Country { get; set; }
        public virtual ICollection<Order> Orders { get; set; }

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
    }
}
