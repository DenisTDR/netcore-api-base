namespace API.Base.Emailing.Models
{
    public class SendGridCredentials
    {
        public string Key { get; set; }
        public bool Simulate { get; set; }
    }
}