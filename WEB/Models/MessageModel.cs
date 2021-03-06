﻿using System;

namespace SocialNetwork.Models
{
    public class MessageModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Avatar { get; set; }
        public string Body { get; set; }
        public long PublicId { get; set; }
        public bool IsRead { get; set; }
        public bool IsMy { get; set; }          
        public DateTime PostedTime { get; set; }
    }
}