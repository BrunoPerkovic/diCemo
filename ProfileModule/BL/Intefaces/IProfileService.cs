using ProfileModule.BL.DataModels;
using ProfileModule.BL.Models;

namespace ProfileModule.BL.Intefaces;

public interface IProfileService
{
    Task<Profile> PostProfile(PostProfileRequest request);
}