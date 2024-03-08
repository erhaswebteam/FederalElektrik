using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Grand.Web.Services
{
    public static class JwtParserService
    {
        public static TokenObject ParseToken(string token, string key)
        {
            
            //var tt = JsonWebToken.Decode(token, "6cqTFHebH0sMynRc6TX3hsxy1oqTJtQDt7kYEpXbdgLAtrCn8ogeGCC");
            var valid = ValidateJwtToken(token, key);
            if (valid)
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
                //var bayiId = tokenS.Claims.First(claim => claim.Type == "BayiId").Value;
                //var unvan = tokenS.Claims.First(claim => claim.Type == "unvan").Value;
                var email = jwtToken.Claims.First(claim => claim.Type == "id").Value;
                string[] exp = jwtToken.Claims.First(claim => claim.Type == "exp").Value.Split('.');
                //var expdate = DateTimeOffset.FromUnixTimeSeconds(exp).ToLocalTime().DateTime;
                var expdate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp[0])).ToLocalTime().DateTime;
                //var role = tokenS.Claims.First(claim => claim.Type == "Rol").Value;
                //var point = tokenS.Claims.First(claim => claim.Type == "Point").Value;

                if (expdate < DateTime.Now)
                {
                    return null;
                }
                else
                {
                    var obj = new TokenObject
                    {
                        //BayiId = bayiId,
                        Email = email,
                        exp = expdate,
                        //Point = point,
                        //Rol = role,
                        //unvan = unvan
                    };
                    return obj;
                }
            }
            else
            {
                return null;
            }
        }

        public static string GetUsername(string token, string key)
        {
            var valid = ValidateJwtToken(token, key);
            if (valid)
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
                return jwtToken.Claims.First(claim => claim.Type == "id").Value;
            }
            else
            {
                return null;
            }
        }

        public static bool ValidateJwtToken(string token, string key)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = GetValidationJwtTokenParameters(key);
                SecurityToken validatedToken;
                IPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        private static TokenValidationParameters GetValidationJwtTokenParameters(string key)
        {
            var keyB = Encoding.ASCII.GetBytes(key);
            return new TokenValidationParameters()
            {
                ValidateLifetime = false,
                ValidateAudience = false, // Because there is no audiance in the generated token
                ValidateIssuer = false,   // Because there is no issuer in the generated token                           
                IssuerSigningKey = new SymmetricSecurityKey(keyB),
            };
        }


        public static string Encrypt(string plainText, string password)
        {
            if (plainText == null)
            {
                return null;
            }

            if (password == null)
            {
                password = String.Empty;
            }

            // Get the bytes of the string
            var bytesToBeEncrypted = Encoding.UTF8.GetBytes(plainText);
            var passwordBytes = Encoding.UTF8.GetBytes(password);

            // Hash the password with SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            var bytesEncrypted = JwtParserService.Encrypt(bytesToBeEncrypted, passwordBytes);

            return Convert.ToBase64String(bytesEncrypted);
        }


        public static string Decrypt(string encryptedText, string password)
        {
            if (encryptedText == null)
            {
                return null;
            }

            if (password == null)
            {
                password = String.Empty;
            }

            // Get the bytes of the string
            var bytesToBeDecrypted = Convert.FromBase64String(encryptedText);
            var passwordBytes = Encoding.UTF8.GetBytes(password);

            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            var bytesDecrypted = JwtParserService.Decrypt(bytesToBeDecrypted, passwordBytes);

            return Encoding.UTF8.GetString(bytesDecrypted);
        }

        private static byte[] Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            var saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);

                    AES.KeySize = 256;
                    AES.BlockSize = 128;
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }

                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        private static byte[] Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            var saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);

                    AES.KeySize = 256;
                    AES.BlockSize = 128;
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);
                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }

                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }

        public static void SetCookie(HttpContext context, string key, string value, int? expireTime)
        {

            CookieOptions option = new CookieOptions();

            if (expireTime.HasValue)
                option.Expires = DateTime.Now.AddDays(expireTime.Value);
            else
                option.Expires = DateTime.Now.AddMilliseconds(10);

            context.Response.Cookies.Append(key, value, option);
        }
        public static string ReadCookie(string key, HttpContext context)
        {
            return context.Request.Cookies[key]?.ToString();
        }
    }


    public class TokenObject
    {
        public string BayiId { get; set; }
        public string unvan { get; set; }
        public string Email { get; set; }
        public DateTime exp { get; set; }
        public string Rol { get; set; }
        public string Point { get; set; }
    }
}
