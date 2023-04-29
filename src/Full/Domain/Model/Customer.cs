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
    public partial class Customer : Entity
    {
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


        public long CountryId { get; set; }


        public virtual Country Country
        {
            get; set;
        }
        public virtual ICollection<Order> Orders { get; set; }


        public string BuildSearchTerms() => $"{Id} {FirstName} {LastName} {EmailAddress} {AddressLine1}".ToLower();


        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
        [NotMapped]
        public string Initials => string.Format("{0}{1}", $"{FirstName} "[0], $"{LastName} "[0]).Trim().ToUpper();
        //public string CountryName => Ioc.Default.GetRequiredService<LookupTableServiceFacade>().GetCountry(CountryCode);
        [NotMapped]
        public string CountryName => "Fake Country";
        [NotMapped]
        public string FullAddress => $"{AddressLine1} {AddressLine2}\n{City}, {Region} {PostalCode}"/*\n{CountryName}"*/;

        //public override void Merge(Customer source)
        //{
        //    if (source != null)
        //    {
        //        Id = source.Id;
        //        Title = source.Title;
        //        FirstName = source.FirstName;
        //        MiddleName = source.MiddleName;
        //        LastName = source.LastName;
        //        Suffix = source.Suffix;
        //        Gender = source.Gender;
        //        EmailAddress = source.EmailAddress;
        //        AddressLine1 = source.AddressLine1;
        //        AddressLine2 = source.AddressLine2;
        //        City = source.City;
        //        Region = source.Region;
        //        Country = source.Country;
        //        PostalCode = source.PostalCode;
        //        Phone = source.Phone;
        //        BirthDate = source.BirthDate;
        //        Education = source.Education;
        //        Occupation = source.Occupation;
        //        YearlyIncome = source.YearlyIncome;
        //        MaritalStatus = source.MaritalStatus;
        //        TotalChildren = source.TotalChildren;
        //        ChildrenAtHome = source.ChildrenAtHome;
        //        IsHouseOwner = source.IsHouseOwner;
        //        NumberCarsOwned = source.NumberCarsOwned;
        //        CreatedOn = source.CreatedOn;
        //        LastModifiedOn = source.LastModifiedOn;
        //        Thumbnail = source.Thumbnail;
        //        Picture = source.Picture;

        //        CountryId = source.CountryId;

        //    }
        //}
    }
}
