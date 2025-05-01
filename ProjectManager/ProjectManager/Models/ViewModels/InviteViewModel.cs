using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjectManager.Models.ViewModels
{
    public class InviteViewModel
    {
        public string ProjectId { get; set; }
        public List<string> UserIds { get; set; } = new();

    }
}
