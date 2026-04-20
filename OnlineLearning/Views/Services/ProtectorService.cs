using Microsoft.AspNetCore.DataProtection;

namespace OnlineLearning.Views.Services
{
    public class ProtectorService
    {
        private readonly IDataProtector _protector;

        public ProtectorService(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("CourseIdProtector");
        }

        public string Encrypt(int id)
        {
            return _protector.Protect(id.ToString());
        }

        
        public int Decrypt(string encryptedId)
        {
            var decrypted = _protector.Unprotect(encryptedId);
            return int.Parse(decrypted);
        }

        
    }
}
