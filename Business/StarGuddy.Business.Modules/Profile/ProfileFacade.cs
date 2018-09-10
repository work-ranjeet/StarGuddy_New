﻿using AutoMapper;
using StarGuddy.Api.Models.Dto;
using StarGuddy.Api.Models.Interface.Profile;
using StarGuddy.Api.Models.Profile;
using StarGuddy.Api.Models.UserJobs;
using StarGuddy.Business.Interface.Profile;
using StarGuddy.Core.Constants;
using StarGuddy.Core.Enums;
using StarGuddy.Repository.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarGuddy.Business.Modules.Profile
{
    public class ProfileFacade: IProfileFacade
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserDetailRepository _userDetailRepository;
        private readonly IUserSettingsRepository _userSettingsRepository;
        private readonly IUserCreditsRepository _userCreditsRepository;
        private readonly IPhysicalAppearanceRepository _physicalAppearanceRepository;
        private readonly IUserDancingRepository _userDancingRepository;
        private readonly IUserActingRepository _userActingRepository;
        private readonly IUserModelingRepository _userModelingRepository;
        private readonly IMapper _mapper;

        public ProfileFacade(
            IUserRepository userRepository,
            IUserDetailRepository userDetailRepository,
            IUserSettingsRepository userSettingsRepository,
            IPhysicalAppearanceRepository physicalAppearanceRepository,
            IUserCreditsRepository userCreditsRepository,
            IUserDancingRepository userDancingRepository,
            IUserActingRepository userActingRepository,
            IUserModelingRepository userModelingRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _userDetailRepository = userDetailRepository;
            _userSettingsRepository = userSettingsRepository;
            _userDancingRepository = userDancingRepository;
            _userCreditsRepository = userCreditsRepository;
            _physicalAppearanceRepository = physicalAppearanceRepository;
            _userActingRepository = userActingRepository;
            _userModelingRepository = userModelingRepository;
            _mapper = mapper;
        }

        #region /// Physical Appearance
        public async Task<IPhysicalAppearanceModal> FetchPhysicalAppreance(Guid userId)
        {
            var result = await _physicalAppearanceRepository.GetPhysicalAppreanceById(userId);
            return new PhysicalAppearanceModal
            {
                BodyType = result.BodyType,
                Chest = result.Chest,
                Ethnicity = result.Ethnicity,
                EyeColor = result.EyeColor,
                HairColor = result.HairColor,
                HairLength = result.HairLength,
                HairType = result.HairType,
                Height = result.Height,
                SkinColor = result.SkinColor,
                UserId = result.UserId,
                Weight = result.Weight,
                West = result.West
            };
        }
        #endregion

        #region /// User credits
        public async Task<IEnumerable<IUserCreditModel>> FetchUserCredits(Guid userId)
        {
            var result = await this._userCreditsRepository.GetUserCreditsById(userId);
            if (result != null && result.Any())
            {
                var creditBag = new ConcurrentBag<IUserCreditModel>();
                result.ToList().ForEach(credit =>
                {
                    creditBag.Add(new UserCreditModel
                    {
                        Id = credit.Id,
                        Action = DbOperation.NoAction,
                        WorkYear = credit.Year,
                        WorkPlace = credit.WorkPlace,
                        WorkDetail = credit.WorkDetail
                    });
                });

                return creditBag.AsEnumerable();
            }

            return null;
        }
        #endregion

        #region /// User dancing
        public async Task<DancingModel> FetchUserDancingAsync(Guid userId)
        {
            var dancingModel = new DancingModel()
            {
                AgentNeed = 0,
                DanceAbilities = 0,
                ChoreographyAbilities = 0,
                DanceAbilitiesText = ExpertLavel.Beginner.ToString(),
                ChoreographyAbilitiesText = ExpertLavel.Beginner.ToString(),
                HasDanceStyle = false,
                DnacingStyles = new List<DancingStyleDto>()
            };

            var userDancing = await _userDancingRepository.GetUserDancingAsync(userId);

            var dancingStyle = await _userDancingRepository.GetDancingStyleSelectedAsync(userId);

            //await Task.WhenAll(userDancing, dancingStyle);

            if (userDancing != null)//&& userDancing.IsCompletedSuccessfully)
            {
                dancingModel = new DancingModel
                {
                    Id = userDancing.Id,
                    AgentNeed = userDancing.AgentNeedCode ?? 0,
                    DanceAbilities = userDancing.DanceAbilitiesCode,
                    ChoreographyAbilities = userDancing.ChoreographyAbilitiesCode,
                    DanceAbilitiesText = ((ExpertLavel)userDancing.DanceAbilitiesCode).ToString(),
                    ChoreographyAbilitiesText = ((ExpertLavel)userDancing.ChoreographyAbilitiesCode).ToString(),
                    IsAgent = userDancing.IsAgent,
                    IsAttendedSchool = userDancing.IsAttendedSchool,
                    Experience = userDancing.Experiance,
                    UserId = userDancing.UserId,
                    DnacingStyles = new List<DancingStyleDto>()
                };
            }

            if (dancingStyle != null)// && dancingStyle.IsCompletedSuccessfully)
            {

                var danceStyle = dancingStyle.Select(x =>
                {
                    return new DancingStyleDto
                    {
                        Id = x.Id,
                        Name = x.Style,
                        SelectedValue = x.SelectedValue,
                        Value = x.Value
                    };
                });

                danceStyle.ToList().ForEach(x => dancingModel.DnacingStyles.Add(x));

                dancingModel.HasDanceStyle = dancingModel.DnacingStyles.Any(x => (x.SelectedValue ?? 0) > 0);
            }

            return dancingModel;
        }
        #endregion

        #region /// User Acting
        public async Task<UserActingModel> FetchUserActingDetailAsync(Guid userId)
        {
            var result = await _userActingRepository.GetUserActingDetailAsync(userId);
            if (result.IsNotNull())
            {
                return new UserActingModel
                {
                    Id = result.UserActing?.Id ?? Guid.Empty,
                    UserId = userId,
                    ActingExperianceCode = result.UserActing?.ActingExperianceCode ?? 0,
                    ActingExperiance = result.UserActing?.ActingExperiance ?? string.Empty,
                    AgentNeedCode = result.UserActing?.AgentNeedCode ?? 0,
                    Experiance = result.UserActing?.Experiance ?? string.Empty,
                    Accents = _mapper.Map<List<AccentsDto>>(result.Accents),
                    Languages = _mapper.Map<List<LanguageDto>>(result.Languages),
                    AuditionsAndJobsGroup = _mapper.Map<List<AuditionsAndJobsGroupDto>>(result.ActingRoles)
                };
            }

            return new UserActingModel
            {
                Accents = new List<AccentsDto>(),
                Languages = new List<LanguageDto>(),
                AuditionsAndJobsGroup = new List<AuditionsAndJobsGroupDto>()
            };
        }

        #endregion

        #region /// User Modeling
        public async Task<UserModelingModel> FetchUserModelingDetailAsync(Guid userId)
        {
            var result = await _userModelingRepository.GetUserModelingDetailAsync(userId);
            if (result.IsNotNull())
            {
                var userModelingModel = _mapper.Map<UserModelingModel>(result.UserModeling);
                if (userModelingModel.IsNull())
                {
                    userModelingModel = new UserModelingModel { ModelingRoles = new List<AuditionsAndJobsGroupDto>() };
                }

                userModelingModel.ModelingRoles = _mapper.Map<List<AuditionsAndJobsGroupDto>>(result.ModelingRoles);

                return userModelingModel;
            }

            return new UserModelingModel { ModelingRoles = new List<AuditionsAndJobsGroupDto>() };
        }
        #endregion

        #region /// Name
        public async Task<UserNameModel> FetchNameDetailsByUserId(Guid userId)
        {
            var result = await _userRepository.FindByIdAsync(userId);

            if (result.IsNotNull())
            {
                return new UserNameModel
                {
                    Id = result.Id,
                    FirstName = result.FirstName,
                    LastName = result.LastName,
                    DisplayName = result.DisplayName,
                    OrgName = result.OrgName
                };
            }

            return null;
        }
        #endregion

        #region /// profile header
        public async Task<ProfileHeader> FetchProfileHeaderByUserId(Guid userId)
        {
            var result = await _userRepository.GetUserProfileHeaderById(userId);

            if (result.IsNotNull())
            {
                var profileHeader = _mapper.Map<ProfileHeader>(result);
                profileHeader.JobGroups = _mapper.Map<List<JobGroupModel>>(result.JobGroups).Where(x => x.Code == x.SelectedCode);

                return profileHeader;
            }

            return null;
        }
        #endregion

        #region /// Profile Intro
        public async Task<UserDetailModel> FetchProfileDetail(Guid userId)
        {
            var details = _userDetailRepository.GetUserDetailByUserId(userId);
            var settings = _userSettingsRepository.GetUsetSettingByUserIdAsync(userId);

            var taskResult = Task.WhenAll(details, settings);

            UserDetailModel detailModel = null;
            try
            {
                await taskResult.ConfigureAwait(false);
                if (taskResult.IsCompletedSuccessfully)
                {
                    detailModel = _mapper.Map<UserDetailModel>(await details);
                    if (detailModel != null)
                    {
                        detailModel.ProfileAddress = (await settings).ProfileUrl;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return detailModel;
        }
        #endregion
    }
}
