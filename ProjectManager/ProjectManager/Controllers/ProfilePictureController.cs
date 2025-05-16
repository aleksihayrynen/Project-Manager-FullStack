using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.Models.Services;
using Microsoft.AspNetCore.StaticFiles; 
using ProjectManager.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


namespace ProjectManager.Controllers
{
    [Authorize]
    public class ProfilePictureController : Controller
    {
        private readonly IProfilePictureService _profilePictureService;
        private readonly FileExtensionContentTypeProvider _contentTypeProvider;
        private readonly string _imageFolderPath = @"C:\UserImages\";

        public ProfilePictureController(IProfilePictureService profilePictureService )
        {
            _profilePictureService = profilePictureService;
            _contentTypeProvider = new FileExtensionContentTypeProvider();

        }


        [HttpGet]
        [Route("Images/GetProfilePicture")]
        public IActionResult GetProfilePicture(string userId)
        {
        if (string.IsNullOrEmpty(userId))
            return BadRequest("User ID is required.");

        var stream = _profilePictureService.GetProfilePictureStream(userId);
        var fileName = _profilePictureService.GetProfilePictureUrl(userId);

        if (stream == null || string.IsNullOrEmpty(fileName))
        {
        var defaultPath = Path.Combine(_imageFolderPath, "default.png");
        if (!System.IO.File.Exists(defaultPath))
            return NotFound("Default profile picture not found.");

        stream = new FileStream(defaultPath, FileMode.Open, FileAccess.Read);
        fileName = "default.png";
        }

        if (!_contentTypeProvider.TryGetContentType(fileName, out var contentType))
        {
        contentType = "application/octet-stream";
        }

        return File(stream, contentType);
        }






        [HttpPost]
        [RequestSizeLimit(5 * 1024 * 1024)] // 5MB limit
        public async Task<IActionResult> Upload(string userId)
        {
        if (string.IsNullOrEmpty(userId))
            return BadRequest("User ID is required.");

        var file = Request.Form.Files["profilePicture"];
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
            return BadRequest("Invalid file type. Allowed: .jpg, .jpeg, .png, .gif");

        Image image;
        try
        {
        image = await Image.LoadAsync(file.OpenReadStream());
        }
        catch (UnknownImageFormatException)
        {
        return BadRequest("Uploaded file is not a valid image.");
        }

        // Resize image to max 150x150
        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(256, 256),
            Mode = ResizeMode.Max
        }));

        // Create user folder if it doesn't exist
        var userFolderPath = Path.Combine(_imageFolderPath, userId);
        Directory.CreateDirectory(userFolderPath);

        // Unique file name
        var uniqueFileName = $"{userId}_{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(userFolderPath, uniqueFileName);

        // Save the image using correct encoder
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
        var encoder = GetEncoder(extension);
        await image.SaveAsync(stream, encoder);
        }

        // Update user in DB (example, adjust to your data access)
        var user = await MongoManipulator.GetObjectById<User>(userId);
        if (user == null)
            return NotFound("User not found.");

        user.ProfilePictureURL = Path.Combine(userId, uniqueFileName).Replace("\\", "/");
        await MongoManipulator.Save(user);

        return Ok(new { message = "Profile picture uploaded successfully." });
        }



        private IImageEncoder GetEncoder(string extension)
        {
        return extension switch
        {
            ".jpg" or ".jpeg" => new JpegEncoder(),
            ".png" => new PngEncoder(),
            ".gif" => new GifEncoder(),
            _ => throw new InvalidOperationException("Unsupported image format")
        };
        }

    }
}
