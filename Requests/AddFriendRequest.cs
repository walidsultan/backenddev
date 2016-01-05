using Bluebeam.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bluebeam.Requests
{
    public class AddFriendRequest
    {
        public int UserId { get; set; }
        public int FriendId { get; set; }
    }
}