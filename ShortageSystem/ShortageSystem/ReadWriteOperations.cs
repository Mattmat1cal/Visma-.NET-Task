using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ShortageSystem.Model;
using ShortageSystem.Model.Enums;

namespace ShortageSystem
{
    internal class ReadWriteOperations
    {
        private readonly string _requestFile = "ShortageRequests.json";
        private readonly string _userFile = "Users.json";

        public bool WriteUserToFile(User user)
        {
            List<User>? userList = new List<User>();
            if (File.Exists(_userFile))
            {
                string fileContent = File.ReadAllText(_userFile);
                userList = JsonSerializer.Deserialize<List<User>>(fileContent);
            }

            if (userList != null && userList.Any(u => u.Username == user.Username)) return false;
            userList?.Add(user);
            string jsonString = JsonSerializer.Serialize(userList, new JsonSerializerOptions {WriteIndented = true});
            File.WriteAllText(_userFile,jsonString);
            return true;
        }

        public User? IsValidCredentials(string username, string password)
        {
            if (!File.Exists(_userFile)) return null;
            string fileContent = File.ReadAllText(_userFile);
            var userList = JsonSerializer.Deserialize<List<User>>(fileContent);
            return userList.FirstOrDefault(u => u.Username == username && u.Password == password);
        }
        public bool WriteRequestToFile(ShortageRequest request)
        {
            List<ShortageRequest>? shortageList = new List<ShortageRequest>();
            if (File.Exists(_requestFile))
            {
                string fileContent = File.ReadAllText(_requestFile);
                shortageList = JsonSerializer.Deserialize<List<ShortageRequest>>(fileContent);
                
            }

            if (shortageList != null && shortageList.Any(x => x.Title == request.Title && x.Room == request.Room))
            {
                var matches = shortageList.FirstOrDefault(x => x.Priority < request.Priority);
                if (matches == null) return false;

                int index = shortageList.IndexOf(matches);
                if (index != -1)
                {
                    shortageList[index] = request;
                }
            } else shortageList?.Add(request);

            string jsonString = JsonSerializer.Serialize(shortageList, new JsonSerializerOptions {WriteIndented = true});
            File.WriteAllText(_requestFile, jsonString);
            return true;
        }

        public List<ShortageRequest>? ReadAllRequests(User user)
        {
            if (!File.Exists(_requestFile)) return null;
            string fileContent = File.ReadAllText(_requestFile);
            var requestList = JsonSerializer.Deserialize<List<ShortageRequest>>(fileContent);
            if (user.UserType == UserType.Admin) return requestList;

            var filteredList = requestList?.Where(r => r.CreatedBy == user.Username).ToList();
            return filteredList;
        }

        public bool DeleteRequest(User user, int index)
        {
            var shortageList = ReadAllRequests(user);
            if (shortageList != null && index > 0 && index < shortageList.Count)
            {
                shortageList.RemoveAt(index);
                string jsonString =
                    JsonSerializer.Serialize(shortageList, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_requestFile,jsonString);
                return true;
            }

            return false;
        }

        public List<ShortageRequest>? ReadFilteredRequests(User user, string? title, DateTime? startDate, DateTime? endDate,
            Category? category, Room? room)
        {
            List<ShortageRequest>? shortageList = ReadAllRequests(user);
            if (shortageList == null) return null;
            if (title != null)
                shortageList = shortageList.Where(s => Regex.IsMatch(s.Title.ToLower(), title.ToLower())).ToList();
            if (startDate != null) shortageList = shortageList.Where(s => s.CreatedOn > startDate).ToList();
            if (endDate != null) shortageList = shortageList.Where(s => s.CreatedOn < endDate).ToList();
            if (category != null) shortageList = shortageList.Where(s => s.Category == category).ToList();
            if (room != null) shortageList = shortageList.Where(s => s.Room == room).ToList();
            shortageList = shortageList.OrderByDescending(s => s.Priority).ToList();
            return shortageList;
        }
    }
}
