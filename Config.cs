﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot
{
    internal struct Config
    {
        [JsonProperty("token")]
        public string Token { get; private set; }
    }
}
