using Bluebeam.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bluebeam.Servers
{
    public class UserServer
    {
        private static readonly Lazy<UserServer> LazyInstance = new Lazy<UserServer>(() => new UserServer());

        public static UserServer Instance
        {
            get
            {
                return LazyInstance.Value;
            }
        } 

        public Dictionary<int, UserRepo> userRepos = new Dictionary<int, UserRepo>();
        public Dictionary<int, int> userIdToRepoMap = new Dictionary<int, int>();

        private readonly object addUserLock = new object();

        public void AddUser(User user) {
            lock (addUserLock)
            {
                int userRepoId = user.Id / UserRepo.MaxCapacity;
                if (userRepos.Count < userRepoId + 1)
                {
                    userRepos.Add(userRepoId, new UserRepo());
                }

                userIdToRepoMap.Add(user.Id, userRepoId);

                userRepos.Last().Value.AddUser(user);
            }
        }

        public List<User> GetAll()
        {
            List<User> allUsers = new List<User>();
            foreach (var repo in userRepos) {
                allUsers.AddRange(repo.Value.GetAll());
            }
            return allUsers;
        }

        public User FindById(int userId)
        {
            if (!userIdToRepoMap.ContainsKey(userId)) return null;

            int userRepoId = userIdToRepoMap[userId];

            return userRepos[userRepoId].FindById(userId);
        }

        public void AddFriend(int userId, int friendId)
        {
            if (!userIdToRepoMap.ContainsKey(userId) || !userIdToRepoMap.ContainsKey(friendId))
                throw new KeyNotFoundException();

            int userRepoId = userIdToRepoMap[userId];
            int friendRepoId = userIdToRepoMap[friendId];

            userRepos[userRepoId].AddFriend(userId, friendId);
            userRepos[friendRepoId].AddFriend(friendId, userId);
        }

        public void RemoveFriend(int userId, int friendId)
        {
            if (!userIdToRepoMap.ContainsKey(userId) || !userIdToRepoMap.ContainsKey(friendId))
                throw new KeyNotFoundException();

            int userRepoId = userIdToRepoMap[userId];
            int friendRepoId = userIdToRepoMap[friendId];

            userRepos[userRepoId].RemoveFriend(userId, friendId);
            userRepos[friendRepoId].RemoveFriend(friendId, userId);
        }

        public IEnumerable<int> GetUserFriends(int userId)
        {
            int userRepoId = userIdToRepoMap[userId];

          return   userRepos[userRepoId].GetUserFriends(userId);
        }

        public IEnumerable<int> GetUserPotentialFriends(int userId, int level)
        {
            if (!userIdToRepoMap.ContainsKey(userId)) return new List<int>();

            Dictionary<int, int> visitedMap = new Dictionary<int, int>();
            visitedMap.Add(userId, 1);

            int userRepoId = userIdToRepoMap[userId];

            IEnumerable<int> currentFriends = userRepos[userRepoId].GetUserFriendsIds(userId, visitedMap);
            return GetUserPotentialFriends(currentFriends, level, 0, visitedMap);
        }

        private IEnumerable<int> GetUserPotentialFriends(IEnumerable<int> users, int targetLevel, int currentLevel, Dictionary<int, int> visitedMap)
        {
            if (currentLevel == targetLevel) return users;

            List<int> potentialFriends = new List<int>();
            foreach (int userId in users)
            {
                int userRepoId = userIdToRepoMap[userId];
                IEnumerable<int> friends = userRepos[userRepoId].GetUserFriendsIds(userId, visitedMap);
                potentialFriends.AddRange(friends);
            }

            return GetUserPotentialFriends(potentialFriends, targetLevel, ++currentLevel, visitedMap);
        }
    }
}