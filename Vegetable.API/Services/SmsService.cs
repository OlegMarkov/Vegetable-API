using System;
using Telesign;

namespace Vegetable.API.Services
{
    public class SmsService : ISmsService
    {
        public string SendVerificationCode(string phoneNumber, string code)
        {
            string customerId = "36A5163F-6770-4C80-B92E-A53466709FA6";
            string apiKey = "A2eE4a00YDYGTcOuxq3nD8B9OHbRM/406/EU5FU1D0ofuxXiG3fvkIjDL91jzNdmfl1WTaNVxw7xwnOHJ8yCzg==";
            
            string message = $"Your verification code for Busy Carrot is {code}";
            string messageType = "OTP";

            try
            {
                MessagingClient messagingClient = new MessagingClient(customerId, apiKey);
                RestClient.TelesignResponse telesignResponse =
                    messagingClient.Message(phoneNumber, message, messageType);
                return telesignResponse.ToString();
            }
            catch (Exception e)
            {
                return "";
            }

        }
    }  
}
