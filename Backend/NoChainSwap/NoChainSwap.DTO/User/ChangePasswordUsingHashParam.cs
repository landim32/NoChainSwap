using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NoChainSwap.DTO.User
{
    public class ChangePasswordUsingHashParam
    {
        [JsonPropertyName("recoveryHash")]
        public string RecoveryHash {  get; set; }
        [JsonPropertyName("newPassword")]
        public string NewPassword { get; set; }
    }
}
