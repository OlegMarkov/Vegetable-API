using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Vegetable.API.Services
{
    public class EmailService : IEmailService
    {
        public void SendConfirmationEmail()
        {
            throw new NotImplementedException();
        }

        public void SendVerificationCode(string email, string code)
        {
            var values = new Dictionary<string, string>();
            values.Add("apikey", "a0516c71-c9d2-4a2a-a4a0-d8026f2da419");
            values.Add("to", email);
            values.Add("from", "vegetable.proj@gmail.com");
           // values.Add("from", "germancishevskiy@gmail.com");
            values.Add("merge_code", code);
            values.Add("subject", "Verification Email Vegetable");
            values.Add("template", "Vegetable_Verification");            
            values.Add("isTransactional", "true");
            string address = "https://api.elasticemail.com/v2/email/send";
            Send(address, values);
        }

        private async Task<string> Send(string address, Dictionary<string, string> values)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    using (var postContent = new FormUrlEncodedContent(values))
                    using (HttpResponseMessage response = await client.PostAsync(address, postContent))
                    {
                        response.EnsureSuccessStatusCode();
                        using (HttpContent content = response.Content)
                        {
                            string result = await content.ReadAsStringAsync();
                            return result;
                        }
                    }
                }
                catch (Exception exc)
                {
                    return exc.ToString();                   
                }

            }
        }
    }
}
