﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Telegram.Bot.Types.Payments
{
    /// <summary>
    /// This object contains basic information about an invoice.
    /// </summary>
    /// <seealso href="https://core.telegram.org/bots/api#invoice"/>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn,
                NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class Invoice
    {
        /// <summary>
        /// Product name
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string Title { get; set; }

        /// <summary>
        /// Product description
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string Description { get; set; }

        /// <summary>
        /// Unique bot deep-linking parameter that can be used to generate this invoice
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string StartParameter { get; set; }

        /// <summary>
        /// Three-letter ISO 4217 currency code
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string Currency { get; set; }

        /// <summary>
        /// Total price in the smallest units of the currency.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public int TotalAmount { get; set; }
    }
}
