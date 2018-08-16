﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarGuddy.Api.Constants;
using StarGuddy.Api.Models.Profile;
using StarGuddy.Business.Interface.Account;
using StarGuddy.Business.Interface.Profile;
using StarGuddy.Core.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace StarGuddy.Api.Controllers.Profile
{
    [Produces("application/json")]
    [Route("api/Profile/Operations")]
    public class ProfileEditController : BaseApiController
    {
        /// <summary>
        /// The account manager
        /// </summary>
        private readonly IAccountManager accountManager;
        private readonly IProfileManager profileManager;
        //private read only IHttpContextAccessor httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileEditController"/> class.
        /// </summary>
        /// <param name="accountManager">The account manager.</param>
        /// <param name="profileManager">The profile manager.</param>
        public ProfileEditController(IAccountManager accountManager, IProfileManager profileManager)
        {
            this.accountManager = accountManager;
            this.profileManager = profileManager;
        }

        #region Physical appearance
        [HttpPost]
        [Route("SavePhysicalApperance")]
        public async Task<IActionResult> SavePhysicalApperance([FromBody]PhysicalAppearanceModal physicalAppearance)
        {
            if (physicalAppearance.IsNull())
            {
                return BadRequest(HttpStatusText.InvalidRequest);
            }

            physicalAppearance.UserId = UserContext.Current.UserId;
            return Ok(await profileManager.PerformSave(physicalAppearance));
        }


        [HttpGet]
        [Route("PhysicalApperance")]
        public async Task<IActionResult> GetPhysicalApperance()
        {
            var result = await profileManager.GetPhysicalAppreance(UserContext.Current.UserId);

            if (result.IsNull())
            {
                return NotFound();
            }

            return Ok(result);

        }
        #endregion

        #region Credits
        [HttpGet]
        [Route("Credit")]
        public async Task<IActionResult> GetUserCredits()
        {
            var creditResult = await profileManager.GetUserCredits(UserContext.Current.UserId);

            if (!creditResult.IsNull() && creditResult.Any())
            {
                return Ok(creditResult);
            }

            if (!creditResult.IsNull() && !creditResult.Any())
            {
                return NoContent();
            }

            return NotFound(creditResult);
        }

        [HttpPost]
        [Route("Credit")]
        public async Task<IActionResult> SaveUserCredits([FromBody]List<UserCreditModel> credits)
        {
            if (credits.IsNull())
            {
                return BadRequest(HttpStatusText.InvalidRequest);
            }

            var isSuccess = await profileManager.SaveUserCredits(UserContext.Current.UserId, credits);
            if (isSuccess)
            {
                return Ok(isSuccess);
            }

            return StatusCode(Convert.ToInt32(HttpStatusCode.NotModified), this);
        }

        [HttpDelete]
        [Route("Credit")]
        public async Task<IActionResult> DeleteUserCredits(Guid Id)
        {
            if (Id == Guid.Empty)
            {
                return BadRequest(HttpStatusText.InvalidRequest);
            }

            var isDeleted = await profileManager.DeleteUserCredits(Id);

            if (isDeleted)
            {
                return Ok(isDeleted);
            }

            return StatusCode(Convert.ToInt32(HttpStatusCode.NotModified), this);
        }
        #endregion

        #region Dancing
        [HttpGet]
        [Route("Dancing")]
        public async Task<IActionResult> GetUserDancing()
        {
            var dancingResult = await profileManager.GetUserDancingAsync(UserContext.Current.UserId);

            if (dancingResult.IsNull())
            {
                return NotFound(dancingResult);
            }

            return Ok(dancingResult);
        }

        [HttpPost]
        [Route("Dancing")]
        public async Task<IActionResult> SaveUserDancing([FromBody]DancingModel dancingModel)
        {
            if (dancingModel.IsNull())
            {
                return BadRequest(HttpStatusText.InvalidRequest);
            }

            var isSuccess = await profileManager.SaveUserDancingAsync(dancingModel);
            if (isSuccess)
            {
                return Ok(isSuccess);
            }

            return StatusCode(HttpStatusCode.NotModified.GetHashCode(), this);
        }
        #endregion

        #region Acting
        [HttpGet]
        [Route("Acting")]
        public async Task<IActionResult> GetUserActingDetails()
        {
            var actingResult = await profileManager.GetUserActingDetailAsync(UserContext.Current.UserId);

            if (actingResult.IsNull())
            {
                return NotFound(actingResult);
            }

            return Ok(actingResult);
        }

        [HttpPost]
        [Route("Acting")]
        public async Task<IActionResult> SaveUserActingDetails([FromBody]UserActingModel userActingModel)
        {
            if (userActingModel.IsNull())
            {
                return BadRequest(HttpStatusText.InvalidRequest);
            }

            var isSuccess = await profileManager.SaveUserActingDetailsAsync(userActingModel);
            if (isSuccess)
            {
                return Ok(isSuccess);
            }

            return StatusCode(HttpStatusCode.NotModified.GetHashCode(), this);
        }
        #endregion
    }
}