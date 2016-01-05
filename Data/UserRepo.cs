using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;

namespace Bluebeam.Data
{
    public class UserRepo
    {
        private static readonly Lazy<UserRepo> LazyInstance = new Lazy<UserRepo>(() => new UserRepo());
        public static UserRepo Instance
        {
            get
            {
                return LazyInstance.Value;
            }
        }

        public UserRepo()
        {
            this.users = new Dictionary<int, User>();
        }

        private readonly IDictionary<int, User> users;

        public void AddUser(User user)
        {
            if (null == user)
                throw new ArgumentNullException();

            users.Add(user.Id, user);
        }

        public User FindById(int id)
        {
            if (users.ContainsKey(id))
                return users[id];

            throw new KeyNotFoundException();
        }

        public IEnumerable<User> GetAll()
        {
            return this.users.Values.ToList(); 
        }

        public IEnumerable<User> GetUserFriends(int userId)
        {
            User user = FindById(userId);

            return user.Friends.Values;
        }

        public void AddFriend(int userId, int friendId)
        {
            if (!users.ContainsKey(userId) || !users.ContainsKey(friendId))
                throw new KeyNotFoundException();

            if (!users[userId].Friends.ContainsKey(friendId) && userId != friendId)
            {
                users[userId].Friends.Add(friendId, FindById(friendId));
                users[friendId].Friends.Add(userId, FindById(userId));
            }
        }

        public void RemoveFriend(int userId, int friendId)
        {
            if (!users.ContainsKey(userId) || !users.ContainsKey(friendId))
                throw new KeyNotFoundException();

         
                users[userId].Friends.Remove(friendId);
                users[friendId].Friends.Remove(userId); 
         
        }
    }
}