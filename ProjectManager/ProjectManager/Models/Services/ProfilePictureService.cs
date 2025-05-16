using System.IO;
using System.Threading.Tasks;




namespace ProjectManager.Models.Services

{
    public class ProfilePictureService : IProfilePictureService
    {
        private readonly string _imageFolderPath = @"C:\UserImages\";

        public Stream? GetProfilePictureStream(string userId)
        {
        if (string.IsNullOrEmpty(userId))
            return null;

        var userInfo = MongoManipulator.GetObjectById<User>(userId).GetAwaiter().GetResult();
        if (userInfo == null)
            return null;

        var fileName = string.IsNullOrEmpty(userInfo.ProfilePictureURL)
            ? "default.png"
            : userInfo.ProfilePictureURL;

        var filePath = Path.Combine(_imageFolderPath, fileName);
        if (!System.IO.File.Exists(filePath))
            return null;

        return System.IO.File.OpenRead(filePath);
        }


        public string? GetProfilePictureUrl(string userId)
        {
        // If you prefer to serve pictures from a controller action or URL
        // return a URL like "/Images/GetProfilePicture?userId=xxx"
        // or null if no picture found
        var userInfo = MongoManipulator.GetObjectById<User>(userId);
        var userPictureUrl = userInfo.Result.ProfilePictureURL;
        if (string.IsNullOrEmpty(userPictureUrl)) return null;

        return $"/Images/GetProfilePicture?userId={userId}";
        }

        public async Task<string?> GetProfilePictureFileName(string userId)
        {
        var user = await  MongoManipulator.GetObjectById<User>(userId);

        if (user == null)
            return null;

        return user.ProfilePictureURL;
        }
    }
}
