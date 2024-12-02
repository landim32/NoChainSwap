using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoChainSwap.DTO.MailerSend
{
    public class MailerRecipientInfo
    {
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
