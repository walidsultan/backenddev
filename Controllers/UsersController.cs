using Bluebeam.Data;
using Bluebeam.Requests;
using Bluebeam.Responses;
using Bluebeam.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Bluebeam.Controllers
{
    public class UsersController : ApiController
    {
        [HttpPost]
        [Route("users")]
        public void Post(UserRequest request)
        {
            UserServer.Instance.AddUser(request.ToUser(UniqueId.Next()));
        }

        [HttpPost]
        [Route("addFriend")]
        public void AddFriend(FriendRequest request)
        {
            UserServer.Instance.AddFriend(request.UserId, request.FriendId);
        }

        [HttpPost]
        [Route("removeFriend")]
        public void RemoveFriend(FriendRequest request)
        {
            UserServer.Instance.RemoveFriend(request.UserId, request.FriendId);
        }

        [HttpGet]
        [Route("getFriends/{userId}")]
        public List<UserResponse> GetFriends(int userId)
        {
            var query = from u in UserServer.Instance.GetUserFriends(userId)
                        select new UserResponse(UserServer.Instance.FindById(u));

            return query.ToList();
        }

        [HttpGet]
        [Route("potentialFriends")]
        public List<UserResponse> PotentialFriends(int userId,int targetLevel)
        {
            var query = from u in UserServer.Instance.GetUserPotentialFriends(userId, targetLevel)
                        select new UserResponse(UserServer.Instance.FindById(u));

            return query.ToList();
        }

        [HttpGet]
        [Route("users")]
        public List<UserResponse> Get()
        {
            var query = from u in UserServer.Instance.GetAll()
                        select new UserResponse(u);

            return query.ToList();
        }

        [HttpGet]
        [Route("users/{userId}")]
        public UserResponse Get(int userId)
        {
            var user = UserServer.Instance.FindById(userId);
            return new UserResponse(user);
        }
    }
}