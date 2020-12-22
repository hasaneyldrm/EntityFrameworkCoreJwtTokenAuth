using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EntityFrameworkCoreJwtTokenAuth.Context;
using EntityFrameworkCoreJwtTokenAuth.Interfaces;
using EntityFrameworkCoreJwtTokenAuth.Models.Auth;
using EntityFrameworkCoreJwtTokenAuth.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using HashCode = EntityFrameworkCoreJwtTokenAuth.Utilities.HashCode;

namespace EntityFrameworkCoreJwtTokenAuth.Services
{
    public class UserService : IUserService
    {
        private DatabaseContext _context;
        private readonly AppSettings _appSettings;
        private readonly IMemoryCache _memCache;

        public UserService(
            DatabaseContext context,
            IOptions<AppSettings> appSettings, IMemoryCache memoryCache)
        {
            _context = context;
            _appSettings = appSettings.Value;
            _memCache = memoryCache;
        }

        public User GetModel(AuthenticateRequest model)
        {

            var user = new User();
            var company = _context.Company.SingleOrDefault(predicate: x =>
                x.PhoneNumber == model.PhoneNumber && x.Password == model.Password && x.Status);
            if (company != null)
            {
                user.Password = company.Password;
                user.Id = company.Id;
                user.Name = company.Name;
                return user;
            }
            try
            {
                var student = _context.Student.SingleOrDefault(predicate: x =>
                x.PhoneNumber == model.PhoneNumber && x.Password == Convert.ToInt32(model.Password) && x.Status);

                if (student != null)
                {
                    user.Password = student.Password.ToString();
                    user.Id = student.Id;
                    user.Name = student.Name;
                    return user;
                }

                var educator = _context.Educator.SingleOrDefault(predicate: x =>
                    x.PhoneNumber == model.PhoneNumber && x.Password == Convert.ToInt32(model.Password) &&
                    x.Status);

                if (educator != null)
                {
                    user.Password = educator.Password.ToString();
                    user.Id = educator.Id;
                    user.Name = educator.Name;
                    return user;
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(message: ex.Message);
            }

            return null;
        }
        public AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress)
        {
            var user = GetModel(model);

            if (user == null) return null;


            if (!CheckUserCondition(user.Id))
            {
                throw new ArgumentException("Logged in with more than one IP address. Please contact your administrator.");
            }


            RefreshToken refreshToken = generateRefreshToken(ipAddress, user.Id.ToString());

            var jwtToken = GenerateJwtToken(user);

            _memCache.Set(refreshToken.Token, user, TimeSpan.FromHours(10));

            return new AuthenticateResponse(user, jwtToken, refreshToken.Token);
        }

        private List<string> GetAllKeysList()
        {
            var field = typeof(MemoryCache).GetProperty(name: "EntriesCollection", bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance);
            if (field is { })
            {
                var collection = field.GetValue(obj: _memCache) as ICollection;
                var items = new List<string>();
                if (collection != null)
                    foreach (var item in collection)
                    {
                        var methodInfo = item.GetType().GetProperty(name: "Key");
                        if (methodInfo is { })
                        {
                            var val = methodInfo.GetValue(obj: item);
                            if (val != null) items.Add(item: val.ToString());
                        }
                    }
                return items;
            }

            return null;
        }

        private bool CheckUserCondition(int userId)
        {
            var allKeys = GetAllKeysList();
            var condition = true;
            var userIpList = new List<string>();
            foreach (var tokens in allKeys)
            {
                var decrytedToken = HashCode.Decrypt(decryptValue: tokens);
                var splittedToken = decrytedToken.Split(separator: '|');


                var ip = splittedToken.First();
                string id;
                id = splittedToken[1];

                if (id == userId.ToString())
                {
                    if (!userIpList.Contains(item: ip))
                    {
                        userIpList.Add(item: ip);
                    }

                    if (userIpList.Count == 3)
                    {
                        condition = false;
                        break;

                    }
                }
            }

            return condition;
        }

        public AuthenticateResponse RefreshToken(string token, string ipAddress)
        {
            bool exist = _memCache.TryGetValue(key: token, value: out User user);
            if (exist)
            {
                var newRefreshToken = generateRefreshToken(ipAddress: ipAddress, publicId: user.Id.ToString());

                // generate new jwt
                var jwtToken = GenerateJwtToken(user: user);

                _memCache.Remove(key: token);
                _memCache.Set(key: newRefreshToken.Token, value: user, absoluteExpirationRelativeToNow: TimeSpan.FromHours(value: 10));

                return new AuthenticateResponse(user, jwtToken, newRefreshToken.Token);
            }
            else
            {
                throw new ArgumentException(message: "Token is not valid");
            }

        }

        public IEnumerable<User> GetAll()
        {
            // return _context.Users;
            return null;
        }

        public User GetById(int id)
        {
            // return _context.Users.Find(id);
            return null;
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(s: _appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims: new[]
                {
                    new Claim(type: ClaimTypes.Name, value: user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(value: 24),
                SigningCredentials = new SigningCredentials(key: new SymmetricSecurityKey(key: key), algorithm: SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor: tokenDescriptor);
            return tokenHandler.WriteToken(token: token);
        }

        private RefreshToken generateRefreshToken(string ipAddress, string publicId)
        {
            using (new RNGCryptoServiceProvider())
            {
                string hashCode = HashCode.RandomString(length: 12);
                var encrypt = HashCode.Encrypt(password: ipAddress + "|" + publicId + "|" + hashCode);

                return new RefreshToken
                {
                    Token = encrypt,
                    Expires = DateTime.UtcNow.AddHours(value: 24),
                    Created = DateTime.UtcNow,
                    CreatedByIp = ipAddress
                };
            }
        }

        string IUserService.GetCompany(int companyId)
        {
            var company = _context.Company.SingleOrDefault(predicate: x => x.Id == companyId && x.Status);
            return company?.PublicId;
        }
    }
}
