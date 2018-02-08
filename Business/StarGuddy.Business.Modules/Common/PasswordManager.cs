﻿// -------------------------------------------------------------------------------
// <copyright file="ImageManager.cs" company="StarGuddy India">
// Copyright (c) 2017. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------
namespace StarGuddy.Business.Modules.Common
{    
    using System.Linq;
    using System.Threading.Tasks;
    using StarGuddy.Api.Models.Interface.Account;
    using StarGuddy.Business.Interface.Common;
    using StarGuddy.Repository.Interfaces;

    /// <summary>
    /// Password Manager
    /// </summary>
    public class PasswordManager : IPasswordManager
    {
        #region /// Properties
        /// <summary>
        /// The user repository
        /// </summary>
        private IUserRepository UserRepository { get; set; }

        /// <summary>
        /// The security manager
        /// </summary>
        private ISecurityManager SecurityManager { get; set; }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SignupManager" /> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="securityManager">The security manager.</param>
        public PasswordManager(IUserRepository userRepository, ISecurityManager securityManager)
        {
            this.UserRepository = userRepository;
            this.SecurityManager = securityManager;
        }

        /// <summary>
        /// Gets the hashed password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns>string values</returns>
        public async Task<string> GetHashedPassword(string password)
        {
            return await this.SecurityManager.GetHashPassword(password);
        }

        /// <summary>
        /// Determines whether [is valid password] [the specified user identifier].
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// boolean value
        /// </returns>
        public async Task<bool> IsValidPassword(string userId, string password)
        {
            var user = this.UserRepository.FindById(userId);
            if (user == null)
            {
                return false;
            }

            return await this.SecurityManager.VerifyHashedPassword(password, user.PasswordHash);
        }

        /// <summary>
        /// Changes the password.
        /// </summary>
        /// <param name="pwdModel">The password model.</param>
        /// <returns></returns>
        public bool ChangePassword(IPasswordModel pwdModel)
        {
            if (pwdModel.NewPassword != pwdModel.ConfirmPassword)
            {
                return false;
            }

            var isValidPwd = this.UserRepository.GetVerifiedUser(pwdModel.Email, pwdModel.OldPassword).Where(x => x.LockoutEnabled == false).Any();
            if (isValidPwd)
            {
                var result = this.UserRepository.UpdatePassword(pwdModel.UserName, pwdModel.NewPassword);
                return result > decimal.Zero;
            }

            return false;
        }
    }
}
