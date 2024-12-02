using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoChainSwap.DTO.MailerSend
{
    public class MailerErrorInfo
    {
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("errors")]
        public IDictionary<string, IList<string>> Errors { get; set; }
    }
}
