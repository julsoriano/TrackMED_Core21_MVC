namespace TrackMED
{
    public class Settings_Email
    {
        public string DeliveryMethod { get; set; }
        public bool EnableSsl { get; set; }
        public string UserName { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool DefaultCredentials { get; set; }
        public string Password { get; set; }
    }
}