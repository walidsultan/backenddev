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
        
        public IEnumerable<int> GetUserFriends(int userId)
        {
            User user = FindById(userId);

            return  user.Friends ;
        }
        public IEnumerable<int> GetUserFriendsIds(int userId,Dictionary<int, int> visitedMap)
        {
            User user = FindById(userId);

            return user.Friends.Where(f =>
            {
                if (!visitedMap.ContainsKey(f))
                {
                    visitedMap.Add(f, 1);
                    return true;
                }
                else
                {
                    visitedMap[f]++;
                    return false;
                }
            });
        }

        public void AddFriend(int userId, int friendId)
        {
            if (!users.ContainsKey(userId) || !users.ContainsKey(friendId))
                throw new KeyNotFoundException();

            if (!users[userId].Friends.Contains(friendId) && userId != friendId)
            {
                users[userId].Friends.Add(friendId);
                users[friendId].Friends.Add(userId);
            }
        }

        public void RemoveFriend(int userId, int friendId)
        {
            if (!users.ContainsKey(userId) || !users.ContainsKey(friendId))
                throw new KeyNotFoundException();

                users[userId].Friends.Remove(friendId);
                users[friendId].Friends.Remove(userId); 
        }

        public IEnumerable<int> GetUserPotentialFriends(int userId, int level)
        {
            if (!users.ContainsKey(userId)) return new List<int>();

            Dictionary<int, int> visitedMap = new Dictionary<int, int>();
            visitedMap.Add(userId, 1);

            IEnumerable<int> currentFriends = GetUserFriendsIds(userId, visitedMap);
            return GetUserPotentialFriends(currentFriends, level, 0, visitedMap);
        }

        private IEnumerable<int> GetUserPotentialFriends(IEnumerable<int> users, int targetLevel, int currentLevel, Dictionary<int, int> visitedMap)
        {
            if(currentLevel==targetLevel) return users;

            List<int> potentialFriends = new List<int>();
            foreach (int userId in users) {
                IEnumerable<int> friends = GetUserFriendsIds(userId, visitedMap);
                potentialFriends.AddRange(friends);
            }

            return GetUserPotentialFriends(potentialFriends, targetLevel, ++currentLevel, visitedMap);
        }

    }
}