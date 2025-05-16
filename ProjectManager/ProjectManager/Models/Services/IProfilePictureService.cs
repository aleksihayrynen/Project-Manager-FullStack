namespace ProjectManager.Models.Services
{
    public interface IProfilePictureService
    {
        Stream? GetProfilePictureStream(string userId);
        string? GetProfilePictureUrl(string userId); // If using URLs or relative paths
    }
}
