﻿using MongoDB.Driver;
using OneOf;
using OneOf.Types;
using Mapster;
using UserMicroservice.Entities;
using UserMicroservice.Services.Users.Models;
using UserMicroservice.Services.Users.Requests;
using System.Text;
using System.Security.Cryptography;

namespace UserMicroservice.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IUserEventPublisher _userEventPublisher;

        public UserService(IMongoDatabase database, IUserEventPublisher userEventPublisher)
        {
            _users = database.GetCollection<User>("Users");
            _userEventPublisher = userEventPublisher;
        }

        public async Task<OneOf<UserDetailsDto, NotFound>> LoginAsync(
            LoginRequest loginRequest,
            CancellationToken cancellationToken = default)
        {
            var hashedPassword = HashPassword(loginRequest.Password);

            var user = await _users
                .Find(u => u.Username == loginRequest.Username && u.PasswordHash == hashedPassword)
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
                return new NotFound();

            return user.Adapt<UserDetailsDto>();
        }

        public async Task<OneOf<string, Error>> RegisterAsync(
            RegisterRequest registerRequest,
            CancellationToken cancellationToken = default)
        {
            var existingUser = await _users
                .Find(u => u.Username == registerRequest.Username)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingUser != null)
                return new Error();

            var hashedPassword = HashPassword(registerRequest.Password);

            var newUser = new User
            {
                Username = registerRequest.Username,
                PasswordHash = hashedPassword
            };

            await _users
                .InsertOneAsync(newUser, cancellationToken: cancellationToken);

            return newUser.Id;
        }

        public async Task<OneOf<string, NotFound>> DeleteUserAsync(
            string userId,
            CancellationToken cancellationToken = default)
        {
            var deleteResult = await _users
                .DeleteOneAsync(u => u.Id == userId, cancellationToken);

            if (deleteResult.DeletedCount == 0)
                return new NotFound();

            await _userEventPublisher.UserDeleted(userId);

            return userId;
        }

        private static string HashPassword(string password)
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = SHA256.HashData(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
