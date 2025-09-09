using System;

namespace PlatinumDev.KnittingAIWebAPI.Dto
{
    public class ProjectCreated
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; }

        public ProjectCreated(Guid id, string imageUrl)
        {
            Id = id;
            ImageUrl = imageUrl;
        }
    }
}
