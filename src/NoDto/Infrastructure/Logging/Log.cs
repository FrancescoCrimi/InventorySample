using Microsoft.Extensions.Logging;
using System;

namespace Inventory.Infrastructure.Logging
{
    public static class LogEvents
    {
        public static EventId UnhandledException = new EventId(1000, "UnhandledException");
        public static EventId Suspending = new EventId(1001, "Suspending");
        public static EventId Configuration = new EventId(1002, "Configuration");
        public static EventId Fetch = new EventId(1003, "Fetch");
        public static EventId LoadTaxTypes = new EventId(1004, "Load TaxTypes");
        public static EventId LoadShippers = new EventId(1005, "Load Shippers");
        public static EventId LoadPaymentTypes = new EventId(1006, "Load PaymentTypes");
        public static EventId LoadOrderStatus = new EventId(1007, "Load OrderStatus");
        public static EventId LoadCountryCodes = new EventId(1008, "Load CountryCodes");
        public static EventId LoadCategories = new EventId(1009, "Load Load Categories");
        public static EventId Save = new EventId(1010, "Save");
        public static EventId Delete = new EventId(1010, "Delete");
        public static EventId Load = new EventId(1011, "Load");
        public static EventId Refresh = new EventId(1012, "Refresh");
        public static EventId LoadDetails = new EventId(1013, "Load Details");
        public static EventId HandleChanges = new EventId(1014, "Handle Changes");
        public static EventId HandleRangesDeleted = new EventId(1015, "Handle Ranges Deleted");
        public static EventId LoadOrders = new EventId(1016, "Load Orders");
        public static EventId LoadCustomers = new EventId(1017, "Load Customers");
        public static EventId LoadProducts = new EventId(1018, "Load Products");
        public static EventId LoadOrderItems = new EventId(1019, "Load OrderItems");
        public static EventId Settings = new EventId(1020, "Settings");
    }

    public class Log
    {
        public long Id { get; set; }
        public bool IsRead { get; set; }
        public string Name { get; set; }
        public DateTimeOffset DateTime { get; set; }
        public string User { get; set; }
        public LogLevel Level { get; set; }
        public string Source { get; set; }
        public string Action { get; set; }
        public string Message { get; set; }
        public string Description { get; set; }
    }
}
