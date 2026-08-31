using System.Security.Cryptography;
using System.Text;
using WebApi.Interface;

namespace WebApi.Implementation
{
    public class TotpService : ITotpService
    {
        private const int SecretSizeBytes = 20;
        private const int Digits = 6;
        private const int StepSeconds = 30;

        public string GenerateSecret()
        {
            var bytes = RandomNumberGenerator.GetBytes(SecretSizeBytes);
            return Base32Encode(bytes);
        }

        public string GenerateProvisioningUri(string secretBase32, string accountEmail, string issuer = "NexoVida")
        {
            var encodedIssuer = Uri.EscapeDataString(issuer);
            var encodedAccount = Uri.EscapeDataString(accountEmail);
            return $"otpauth://totp/{encodedIssuer}:{encodedAccount}?secret={secretBase32}&issuer={encodedIssuer}&digits={Digits}&period={StepSeconds}";
        }

        public bool VerifyCode(string secretBase32, string code)
        {
            if (string.IsNullOrWhiteSpace(code) || code.Length != Digits || !code.All(char.IsDigit))
            {
                return false;
            }

            var secretBytes = Base32Decode(secretBase32);
            var currentStep = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / StepSeconds;

            for (var window = -1; window <= 1; window++)
            {
                var candidate = ComputeCode(secretBytes, currentStep + window);
                if (CryptographicOperations.FixedTimeEquals(
                        Encoding.ASCII.GetBytes(candidate),
                        Encoding.ASCII.GetBytes(code)))
                {
                    return true;
                }
            }

            return false;
        }

        private static string ComputeCode(byte[] secretBytes, long timeStep)
        {
            var stepBytes = BitConverter.GetBytes(timeStep);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(stepBytes);
            }

            using var hmac = new HMACSHA1(secretBytes);
            var hash = hmac.ComputeHash(stepBytes);

            var offset = hash[^1] & 0x0F;
            var binaryCode =
                ((hash[offset] & 0x7F) << 24) |
                ((hash[offset + 1] & 0xFF) << 16) |
                ((hash[offset + 2] & 0xFF) << 8) |
                (hash[offset + 3] & 0xFF);

            var otp = binaryCode % (int)Math.Pow(10, Digits);
            return otp.ToString().PadLeft(Digits, '0');
        }

        private static string Base32Encode(byte[] data)
        {
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
            var sb = new StringBuilder();
            int bitBuffer = 0, bitsLeft = 0;
            foreach (var b in data)
            {
                bitBuffer = (bitBuffer << 8) | b;
                bitsLeft += 8;
                while (bitsLeft >= 5)
                {
                    sb.Append(alphabet[(bitBuffer >> (bitsLeft - 5)) & 0x1F]);
                    bitsLeft -= 5;
                }
            }
            if (bitsLeft > 0)
            {
                sb.Append(alphabet[(bitBuffer << (5 - bitsLeft)) & 0x1F]);
            }
            return sb.ToString();
        }

        private static byte[] Base32Decode(string base32)
        {
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
            base32 = base32.Trim().ToUpperInvariant();
            var bytes = new List<byte>();
            int bitBuffer = 0, bitsLeft = 0;
            foreach (var c in base32)
            {
                var index = alphabet.IndexOf(c);
                if (index < 0) continue;
                bitBuffer = (bitBuffer << 5) | index;
                bitsLeft += 5;
                if (bitsLeft >= 8)
                {
                    bytes.Add((byte)((bitBuffer >> (bitsLeft - 8)) & 0xFF));
                    bitsLeft -= 8;
                }
            }
            return bytes.ToArray();
        }
    }
}
