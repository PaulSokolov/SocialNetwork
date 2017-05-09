namespace SocialNetwork.Models
{
    public class FriendModel
    {
        public long PublicId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Address { get; set; }
        public string Avatar { get; set; }
        public bool IsFriend { get; set; }
        public bool IsFollower { get; set; }
        public bool IsRequested { get; set; }
    }
}