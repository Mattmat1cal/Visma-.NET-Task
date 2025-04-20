using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ShortageSystem.Model;
using ShortageSystem.Model.Enums;

namespace ShortageSystem
{
    internal static class ConsoleUI
    {
        private static ReadWriteOperations _readerWriter = new ReadWriteOperations();
        private static User _loggedInUser = new User();

        public static void StartMenu()
        {
            bool isValidInput = true;
            while (true)
            {
                string text = @"What do you want to do:
                1. Register
                2. Login
                3. Exit";
                Console.WriteLine(text);
                if (!isValidInput) Console.WriteLine("Invalid Input");

                string? userInput;
                int selectedOption;
                userInput = Console.ReadLine();
                if (userInput != null && int.TryParse(userInput, out selectedOption))
                {
                    switch (selectedOption)
                    {
                        case 1:
                            Registration();
                            isValidInput = true;
                            break;
                        case 2:
                            Login();
                            isValidInput = true;
                            break;
                        case 3:
                            System.Environment.Exit(0);
                            break;
                        default:
                            isValidInput = false;
                            break;
                    }
                } else isValidInput = false;
            }
        }

        private static void Registration()
        {
            string? userInput;
            int userType;
            User user = new User();
            bool isValidType = true;
            while (true)
            {
                Console.WriteLine("Select user type (1 - Regular user, 2 - Admin)");
                if (!isValidType) Console.WriteLine("Invalid user type");
                userInput = Console.ReadLine();
                if (userInput != null && int.TryParse(userInput, out userType))
                {
                    switch (userType)
                    {
                        case 1:
                            user.UserType = UserType.Regular;
                            break;
                        case 2:
                            user.UserType = UserType.Admin;
                            break;
                    }

                    if (user.UserType != null) break;
                }

                isValidType = false;
            }

            userInput = null;
            while (userInput == null || userInput.Trim() == "")
            {
                Console.WriteLine("Enter username: ");
                userInput = Console.ReadLine();
            }
            user.Username = userInput;

            userInput = null;
            while (userInput == null || userInput.Trim() == "")
            {
                Console.WriteLine("Enter password: ");
                userInput = Console.ReadLine();
            }

            user.Password = userInput;
            if (!_readerWriter.WriteUserToFile(user))
            {
                Console.WriteLine("User with the same username already exists");
                Console.ReadLine();
            }
            Console.WriteLine("Registration successful");
            Console.ReadLine();
        }

        private static void Login()
        {
            string? userInput = null;
            string username;
            string password;
            while (userInput == null || userInput.Trim() == "")
            {
                Console.WriteLine("Enter username: ");
                userInput = Console.ReadLine();
            }
            username = userInput;

            userInput = null;
            while (userInput == null || userInput.Trim() == "")
            {
                Console.WriteLine("Enter password: ");
                userInput = Console.ReadLine();
            }
            password = userInput;
            User? checkUser = _readerWriter.IsValidCredentials(username, password);
            if (checkUser != null)
            {
                _loggedInUser = checkUser;
                CommandList();
            }
            Console.WriteLine("Incorrect username or password");
            Console.ReadLine();
        }
        private static void CommandList()
        {
            bool isBadInput = false;
            while (true)
            {
                int option;
                string? userInput;
                string text = @"Select command:
                1. Register new shortage
                2. Delete shortage
                3. List all requests
                4. Exit program";
                Console.WriteLine(text);
                if (isBadInput) Console.WriteLine("Invalid command");
                userInput = Console.ReadLine();
                if (userInput != null && int.TryParse(userInput, out option))
                {
                    switch (option)
                    {
                        case 1:
                            CreateShortageRequest();
                            isBadInput = false;
                            break;
                        case 2:
                            DeleteRequest();
                            isBadInput = false;
                            break;
                        case 3:
                            ViewRequests();
                            isBadInput = false;
                            break;
                        case 4:
                            System.Environment.Exit(0);
                            break;
                        default:
                            isBadInput = true;
                            break;
                    }
                }
                else isBadInput = true;
            }
        }
        private static void CreateShortageRequest()
        {
            string? userInput = null;
            ShortageRequest request = new ShortageRequest();

            Console.WriteLine("Enter request title: ");
            while (userInput == null || userInput.Trim() == "") userInput = Console.ReadLine();
            request.Title = userInput;

            Console.WriteLine("Enter request name: ");
            userInput = null;
            while (userInput == null || userInput.Trim() == "") userInput = Console.ReadLine();
            request.Name = userInput;

            while (true)
            {
                string roomOptions = @"Select room:
                1. Meeting Room,
                2. Kitchen,
                3. Bathroom";
                Console.WriteLine(roomOptions);

                int selectedRoom;
                userInput = Console.ReadLine();
                if (userInput != null && int.TryParse(userInput, out selectedRoom))
                {
                    switch (selectedRoom)
                    {
                        case 1:
                            request.Room = Room.MeetingRoom;
                            break;
                        case 2:
                            request.Room = Room.Kitchen;
                            break;
                        case 3:
                            request.Room = Room.Bathroom;
                            break;
                    }
                    if (request.Room != null) break;
                }
            }

            while (true)
            {
                string categoryOptions = @"Select item category:
                1. Electronics
                2. Food
                3. Other";
                Console.WriteLine(categoryOptions);

                int selectedCategory;
                userInput = Console.ReadLine();
                if (userInput != null && int.TryParse(userInput, out selectedCategory))
                {
                    switch (selectedCategory)
                    {
                        case 1:
                            request.Category = Category.Electronics;
                            break;
                        case 2:
                            request.Category = Category.Food;
                            break;
                        case 3:
                            request.Category = Category.Other;
                            break;
                    }
                    if (request.Category != null) break;
                }
            }

            while (true)
            {
                Console.WriteLine("Enter priority (1 - 10)");
                userInput = Console.ReadLine();
                int selectedPriority;
                if (userInput != null && int.TryParse(userInput, out selectedPriority))
                {
                    if (selectedPriority >= 1 && selectedPriority <= 10)
                    {
                        request.Priority = selectedPriority;
                        break;
                    }
                }
            }
            request.CreatedOn = DateTime.Now;
            request.CreatedBy = _loggedInUser.Username;
            if (!_readerWriter.WriteRequestToFile(request))
            {
                Console.WriteLine("Request with the same title and room already exists");
                Console.ReadLine();
            }
            Console.WriteLine("Request successfully created");
            Console.ReadLine();
        }

        private static void DeleteRequest()
        {
            string? userInput = null;
            List<ShortageRequest>? requestList = _readerWriter.ReadAllRequests(_loggedInUser);
            if (requestList == null || requestList.Count == 0)
            {
                Console.WriteLine("No requests that can be deleted");
                Console.ReadLine();
                return;
            }
            for (int i = 0; i < requestList.Count; i++)
            {
                Console.WriteLine($"{i + 1}. Title: {requestList[i].Title}, Location: {requestList[i].Room}");
            }

            while (userInput == null)
            {
                int deleteIndex;
                Console.WriteLine("Enter number of request you want to delete (or q to quit)");
                userInput = Console.ReadLine();
                if (userInput != null && int.TryParse(userInput, out deleteIndex))
                {
                    deleteIndex--;
                    if (deleteIndex >= 0 && deleteIndex < requestList.Count)
                    {
                        if (!_readerWriter.DeleteRequest(_loggedInUser, deleteIndex))
                        {
                            Console.WriteLine("Couldn't delete item");
                            Console.ReadLine();
                        }
                        else
                        {
                            Console.WriteLine("Request successfully deleted");
                            Console.ReadLine();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid index");
                        Console.ReadLine();
                    }
                    return;
                }

                if (userInput.Equals("q")) return;
            }
        }

        private static void ViewRequests()
        {
            string? userInput = null;
            string? titleFiler = null;
            DateTime? dateStartFilter = null;
            DateTime? dateEndFilter = null;
            Category? categoryFilter = null;
            Room? roomFilter = null;
            Console.WriteLine("Filter by title: ");
            titleFiler = Console.ReadLine();
            while (true)
            {
                Console.WriteLine("Filter creation start date (leave empty to not filter): ");
                userInput = Console.ReadLine();
                if (userInput == null || userInput.Trim() == "") break;
                DateTime tempDate;
                if (DateTime.TryParse(userInput, out tempDate))
                {
                    dateStartFilter = tempDate;
                    break;
                }
            }

            while (true)
            {
                Console.WriteLine("Filter creation end date (leave empty to not filter): ");
                userInput = Console.ReadLine();
                if (userInput == null || userInput.Trim() == "") break;
                DateTime tempDate;
                if (DateTime.TryParse(userInput, out tempDate))
                {
                    dateEndFilter = tempDate;
                    break;
                }
            }
            

            int selectedCategory = -1;
            while (true)
            {
                string categoryText = @"Select category to filter by (or write 0 to not filter):
                1. Electronics
                2. Food
                3. Other";
                Console.WriteLine(categoryText);
                userInput = Console.ReadLine();
                if (userInput != null && int.TryParse(userInput, out selectedCategory))
                {
                    switch (selectedCategory)
                    {
                        case 0:
                            break;
                        case 1:
                            categoryFilter = Category.Electronics;
                            break;
                        case 2:
                            categoryFilter = Category.Food;
                            break;
                        case 3:
                            categoryFilter = Category.Other;
                            break;
                    }
                }
                if (categoryFilter != null || selectedCategory == 0) break;
            }

            while (true)
            {
                string roomText = @"Select room to filter by (or write 0 to not filter): 
                1. Meeting Room,
                2. Kitchen,
                3. Bathroom";
                Console.WriteLine(roomText);
                userInput = Console.ReadLine();
                if (userInput != null && int.TryParse(userInput, out selectedCategory))
                {
                    switch (selectedCategory)
                    {
                        case 0:
                            break;
                        case 1:
                            roomFilter = Room.MeetingRoom;
                            break;
                        case 2:
                            roomFilter = Room.Kitchen;
                            break;
                        case 3:
                            roomFilter = Room.Bathroom;
                            break;
                    }
                }
                if (roomFilter != null || selectedCategory == 0) break;
            }

            List<ShortageRequest>? filteredRequests = _readerWriter.ReadFilteredRequests(_loggedInUser, titleFiler,
                dateStartFilter, dateEndFilter, categoryFilter, roomFilter);
            if (filteredRequests == null || filteredRequests.Count == 0)
            {
                Console.WriteLine("No requests match filters");
                Console.ReadLine();
                return;
            }
            foreach (var request in filteredRequests)
            {
                Console.WriteLine($"Title: {request.Title}, Name: {request.Name}, Room: {request.Room}, Category: {request.Category}" +
                                  $", Priority: {request.Priority}, Created On: {request.CreatedOn}, Created By: {request.CreatedBy}");
            }

            while (true)
            {
                Console.WriteLine("Enter q to go back");
                userInput = Console.ReadLine();
                if (userInput != null && userInput.Equals("q")) break;
            }
        }
    }
}
