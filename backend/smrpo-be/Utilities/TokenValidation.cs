namespace smrpo_be
{
    public class TokenValidation
    {
        public bool ValidateLifetime { get; set; }
        public bool ValidateAudience { get; set; }
        public bool ValidateIssuer { get; set; }
        public bool ValidateIssuerSigningKey { get; set; }
        public string ValidIssuer { get; set; }
        public string ValidAudience { get; set; }
        public string IssuerSigningKey { get; set; }
    }
}
