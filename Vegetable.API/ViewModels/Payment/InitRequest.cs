using System.Collections.Generic;

namespace Vegetable.API.ViewModels.Payment
{
    public class InitRequest
    {
        /// <summary>
        /// Mandatory
        /// </summary>
        public string TerminalKey { get; set; }
        /// <summary>
        /// Mandatory in Kopeyka (not RUB)
        /// </summary>
        public int Amount { get; set; }
        /// <summary>
        /// Mandatory
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// Optional
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// Optional
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Optional
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// Optional
        /// </summary>
        public string Language { get; set; }
        /// <summary>
        /// Optional
        /// </summary>
        public char? Recurrent { get; set; }
        /// <summary>
        /// Optional
        /// </summary>
        public string CustomerKey { get; set; }
        /// <summary>
        /// Optional if default value set in control panel
        /// </summary>
        public string NotificationURL { get; set; }
        /// <summary>
        /// Optional if default value set in control panel
        /// </summary>
        public string SuccessURL { get; set; }
        /// <summary>
        /// Optional if default value set in control panel
        /// </summary>
        public string FailURL { get; set; }
        /// <summary>
        /// Optional
        /// </summary>
        public Receipt Receipt { get; set; }
        /// <summary>
        /// Optional 
        /// </summary>
        public Dictionary<string,string> DATA { get; set; }

    }

    public class Receipt
    {
        /// <summary>
        /// Customer email
        /// Requried Email OR Phone 
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Customer Phone
        /// Requried Email OR Phone 
        /// </summary>
        public string Phone { get; set; }
        public string EmailCompany { get; set; }

        /// <summary>
        /// Mandatory
        /// Система налогообложения:
        ///    osn — общая
        ///    usn_income — упрощенная(доходы)
        ///    usn_income_outcome — упрощенная(доходы минус расходы)
        ///    patent — патентная
        ///    envd — единый налог на вмененный доход
        ///    esn — единый сельскохозяйственный налог
        /// </summary>
        public string Taxation { get; set; }

        /// <summary>
        /// Mandatory
        /// </summary>
        public List<Item> Items { get; set; }
    }

    public class Item
    {
        /// <summary>
        /// Mandatory
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Mandatory
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// Mandatory
        /// </summary>
        public int Amount { get; set; }
        /// <summary>
        /// Mandatory
        /// </summary>
        public int Price { get; set; }
        /// <summary>
        /// Mandatory
        /// </summary>
        public string PaymentObject { get; set; } = "service";
        /// <summary>
        /// Mandatory
        ///   none — без НДС
        ///   vat0 — 0%
        ///   vat10 — 10%
        ///   vat20 — 20%
        ///   vat110 — 10/110
        ///   vat120 — 20/120
        /// </summary>
        public string Tax { get; set; } = "none";
    }
}
