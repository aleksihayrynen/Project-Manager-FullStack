using MongoDB.Bson;
using ProjectManager.Models.Utils;

namespace ProjectManager.Models.ViewModels
{
    public class ProfileViewModel
    {
        public User User { get; set; } = null!;
        public string? MostCommonProject_Title { get; set; }
        public int MostCommonProject_Done { get; set; }
        public int Total_Projects { get; set; }
        public int Total_Tasks { get; set; }

        public string FullName => $"{UtilityFunctions.CapitalizeFirstLetter(User.FirstName)} {UtilityFunctions.CapitalizeFirstLetter(User.LastName)}";

    }
}
