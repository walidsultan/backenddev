using Bluebeam.Data;
using System;
using System.Collections.Generic;

namespace Bluebeam.Interfaces
{
    public interface IUserRepo
    {
        void AddFriend(int userId, int friendId);
        void AddUser(User user);
        User FindById(int id);
        IEnumerable<User> GetAll();
        IEnumerable<int> GetUserFriends(int userId);
        IEnumerable<int> GetUserFriendsIds(int userId, System.Collections.Generic.Dictionary<int, int> visitedMap);
        void RemoveFriend(int userId, int friendId);
    }
}
