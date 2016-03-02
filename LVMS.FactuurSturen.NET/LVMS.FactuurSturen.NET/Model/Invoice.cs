﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LVMS.FactuurSturen.Model
{
    public class Invoice
    {
        public string Id { get; set; }
        [JsonProperty("invoicenr_full")]
        public string InvoiceNrFull { get; set; }
        /// <summary>
        /// Invoice number including layout
        /// </summary>
        public string InvoiceNr { get; set; }
        /// <summary>
        /// Contains reference lines on the invoice. 'line1', 'line2', 'line3'. All are strings
        /// </summary>
        public Reference Reference { get; set; }
        /// <summary>
        /// All invoice lines on the invoice
        /// </summary>
        [JsonProperty("lines")]
        public Dictionary<string, InvoiceLine> Lines { get; set; }
        /// <summary>
        /// The ID of the used profile. Default is default profile
        /// </summary>
        public int Profile { get; set; }
        /// <summary>
        /// The type of discount. 'amount' or 'percentage'
        /// </summary>
        public string DiscountType { get; set; }
        /// <summary>
        /// If 'DiscountType' is amount, then this is the amount of discount set on the invoice. 
        /// If 'DiscountType' is set to 'percentage', this is the discount percentage set.
        /// </summary>
        public double Discount { get; set; }
        /// <summary>
        /// The payment condition set on the invoice. Default is the payment condition set in the application.
        /// </summary>
        public string PaymentCondition { get; set; }
        /// <summary>
        /// Term of payment in days.Default is the payment period set with the client.
        /// </summary>
        public int PaymentPeriod { get; set; }
        /// <summary>
        /// If invoice is an automatic collection
        /// </summary>
        [JsonConverter(typeof(BoolConverter))]
        public bool Collection { get; set; }
        /// <summary>
        /// Total of all taxes on this invoice
        /// </summary>
        public double Tax { get; set; }
        /// <summary>
        /// Invoice total including taxes
        /// </summary>
        public double Totalintax { get; set; }
        /// <summary>
        /// Client number
        /// </summary>
        public int ClientNr { get; set; }
        public string Company { get; set; }
        public string Contact { get; set; }
        public string Address { get; set; }
        public string Zipcode { get; set; }
        public string City { get; set; }
        /// <summary>
        /// Country id. You can get a list of country id's with the function api/v1/countrylist.
        /// When creating or updating a client, you can supply a country id or a country name. 
        /// We'll then try to find the id of the country you supplied
        /// </summary>
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string TaxNumber { get; set; }
        /// <summary>
        /// A note that is saved with the invoice
        /// </summary>
        public string InvoiceNote { get; set; }
        /// <summary>
        /// The date the invoice is sent
        /// </summary>
        public DateTime? Sent { get; set; }
        /// <summary>
        /// The date when the invoice is marked as uncollectible
        /// </summary>
        public DateTime? Uncollectible { get; set; }
        /// <summary>
        /// The date when the last reminder was sent
        /// </summary>
        public DateTime? LastReminder { get; set; }
        /// <summary>
        /// The amount that is still open on the invoice. If this amount is 0, then the invoice is paid.If it is negative, 
        /// there is paid more than the invoice amount.
        /// </summary>
        public double Open { get; set; }
        /// <summary>
        /// The date of the last received payment
        /// </summary>
        public DateTime? PaidDate { get; set; }
        /// <summary>
        /// All taxes defined in the invoice
        /// </summary>
        [JsonProperty("taxes")]
        public Dictionary<string, TaxItem> Taxes { get; set; }
        /// <summary>
        /// The complete URL where the invoice can be paid
        /// </summary>
        [JsonProperty("payment_url")]
        public string PaymentUrl { get; set; }
        /// <summary>
        /// The duedate of the invoice. This is the sent-date + the payment period in days
        /// </summary>
        public DateTime? Duedate { get; set; }
        [JsonProperty("history")]
        public Dictionary<string, HistoryItem> History { get; set; }

        #region Needed for new invoice
        /// <summary>
        /// Define the action what to do when this is a new invoice request
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public InvoiceAction Action { get; set; }

        /// <summary>
        /// How to send the invoice to the receiver. Required when you use the action 'send'
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public SendMethod SendMethod { get; set; }

        /// <summary>
        /// When the action is 'save' or 'repeat' you must supply a savename.We'll save the invoice under that name.
        /// </summary>
        public string SaveName { get; set; }

        /// <summary>
        /// If a savename already exists, it will not be overwritten unless this attribute is set to 'true'. Default is 'false'.
        /// </summary>
        [JsonProperty("overwrite_if_exist")]
        public bool OverwriteIfExist { get; set; }

        /// <summary>
        /// When this option is set to 'true' we will convert all the given prices on the invoices to euro, 
        /// based on the currency set in the selected client and the invoice date (to retrieve the current exchange rate)  
        /// </summary>
        [JsonProperty("convert_prices_to_euro")]
        public bool ConvertPricesToEuro { get; set; }

        #region Needed when Action is Repeat
        /// <summary>
        /// Date when the first invoice must be sent. Please use YYYY-MM-DD
        /// </summary>
        public DateTime? InitialDate { get; set; }

        /// <summary>
        /// Date when the last invoice must be sent. After this date the recurring invoice entry is deleted.
        /// </summary>
        public DateTime? FinalSendDate { get; set; }

        /// <summary>
        /// The frequency when the invoice must be sent. Based on the initialdate.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public Frequency Frequency { get; set; }

        /// <summary>
        /// Set if the recurring invoice is automatically sent by our system
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public RepeatType RepeatType { get; set; }

        #endregion
        #endregion
    }

    public enum InvoiceAction
    {
        None,
        /// <summary>
        /// Send the invoice
        /// </summary>
        Send,
        /// <summary>
        /// Save the invoice as a draft
        /// </summary>
        Save,
        /// <summary>
        /// Plan a recurring invoice
        /// </summary>
        Repeat
    }

    public enum SendMethod
    {
        None,
        /// <summary>
        /// Print the invoices yourself. We'll send you the invoice number so you can execute a command to retrieve the PDF if you need so 
        ///  </summary>
        Mail,
        /// <summary>
        ///  Send invoices through e-mail. It will be sent immediately
        /// </summary>
        Email,
        /// <summary>
        /// Send invoice through the printcenter.
        /// </summary>
        Printcenter
    }

    
    public enum Frequency
    {
        None,
        Weekly,
        Monthly,
        Quarterly,
        HalfYearly,
        Yearly,
        BiWeekly,
        BiMonthly,
        FourWeekly
    }

    public enum RepeatType
    {
        None,
        /// <summary>
        /// Send automatically when due
        /// </summary>
        Auto,
        /// <summary>
        /// Do not send automatically
        /// </summary>
        Manual
    }
}