﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Telegram.Bot.Types.Payments
{
    /// <summary>
    /// This object represents one shipping option.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn,
                NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class ShippingOption
    {
        /// <summary>
        /// Shipping option identifier
        /// </summary>
        [JsonProperty]
        public string Id { get; set; }

        /// <summary>
        /// Option title
        /// </summary>
        [JsonProperty]
        public string Title { get; set; }

        /// <summary>
        /// List of price portions
        /// </summary>
        [JsonProperty]
        public LabeledPrice[] Prices { get; set; }
    }
}
