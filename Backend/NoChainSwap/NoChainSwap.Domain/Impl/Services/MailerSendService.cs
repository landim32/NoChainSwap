using Newtonsoft.Json;
using NoChainSwap.Domain.Interfaces.Services;
using NoChainSwap.DTO.MailerSend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace NoChainSwap.Domain.Impl.Services
{
    public class MailerSendService : IMailerSendService
    {
        private const string MAIL_SENDER = "contact@trial-3zxk54vxer6gjy6v.mlsender.net";
        private const string API_URL = "https://api.mailersend.com/v1/email";
        private const string API_TOKEN = "mlsn.30332ed20b31409638f22ad3c259905860bbf5e2ec79e3e3542f3d5776c6dabd";


        public async Task<bool> Sendmail(MailerInfo email)
        {
            using (var client = new HttpClient())
            {
                email.From.Email = MAIL_SENDER;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", API_TOKEN);
                var jsonContent = new StringContent(JsonConvert.SerializeObject(email), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(API_URL, jsonContent);
                if (!response.IsSuccessStatusCode)
                {
                    var errorStr = await response.Content.ReadAsStringAsync();
                    var msgErro = JsonConvert.DeserializeObject<MailerErrorInfo>(errorStr);
                    if (msgErro != null && !string.IsNullOrEmpty(msgErro.Message))
                    {
                        throw new Exception(msgErro.Message);
                    }
                    throw new Exception("Unknown error");
                }
            }
            return await Task.FromResult(true);
        }
    }
}
