﻿using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Types
{
    /// <summary>
    /// This object describes the position on faces where a mask should be placed by default.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn,
                NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class MaskPosition
    {
        /// <summary>
        /// The part of the face relative to which the mask should be placed. One of “forehead”, “eyes”, “mouth”, or “chin”.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public MaskPositionPoint Point { get; set; }

        /// <summary>
        /// Shift by X-axis measured in widths of the mask scaled to the face size, from left to right. For example, choosing -1.0 will place mask just to the left of the default mask position.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public float XShift { get; set; } // ToDo: use nullable values. no need to send default values as well

        /// <summary>
        /// Shift by Y-axis measured in heights of the mask scaled to the face size, from top to bottom. For example, 1.0 will place the mask just below the default mask position.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public float YShift { get; set; }

        /// <summary>
        /// Mask scaling coefficient. For example, 2.0 means double size.
        /// </summary>
        [Obsolete]
        public float Zoom { get; set; } // ToDo: Remove. Doesn't exist on Bot API

        /// <summary>
        /// Mask scaling coefficient. For example, 2.0 means double size.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public float Scale { get; set; }
    }
}
