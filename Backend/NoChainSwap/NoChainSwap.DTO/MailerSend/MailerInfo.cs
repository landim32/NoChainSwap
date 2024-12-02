using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoChainSwap.DTO.MailerSend
{
    public class MailerInfo
    {
        [JsonProperty("from")]
        public MailerRecipientInfo From {  get; set; }
        [JsonProperty("to")]
        public IList<MailerRecipientInfo> To {  get; set; }
        [JsonProperty("subject")]
        public string Subject { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("html")]
        public string Html { get; set; }
    }
}
