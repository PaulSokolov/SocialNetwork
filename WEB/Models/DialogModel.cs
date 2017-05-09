using System;

namespace SocialNetwork.Models
{
    public class DialogModel
    {
        private string _body;
        public string Name { get; set; }
        public string Surname { get; set; }
        public string MyAvatar { get; set; }
        public string SenderAvatar { get; set; }
        public long PublicId { get; set; }
        public bool IsRead { get; set; }
        public string Body
        {
            get
            {
                if (_body.Length <= 65)
                    return _body;
                return _body.Substring(0, 65) + "...";
            }
            set => _body = value;
        }
        public DateTime LastMessageTime { get; set; }
        //public int UnReadMessages { get; set; }
    }
}