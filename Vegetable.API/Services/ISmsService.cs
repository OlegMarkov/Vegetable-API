namespace Vegetable.API.Services
{
    public interface ISmsService
    {
        string SendVerificationCode(string phoneNumber, string code);
    }
}
