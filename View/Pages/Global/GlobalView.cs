using PaperNest_API.Controllers;
using Microsoft.AspNetCore.Mvc;
using View.Pages.Student;
//using View.Lecturer;
using API.StateMachines;
using API.Services;
using API.Models;
using API.Models.DataBinding;
using Microsoft.AspNetCore.Authentication;
using View.Pages.Lecturer;
using View.Utils;

namespace View.Pages.Global
{
    public class GlobalView
    {
        private readonly AuthStateMachine _authState;
        private readonly UserService _userService;

        private User? _currentUser;
        private bool _isRunning;

        public GlobalView()
        {
            _authState = new AuthStateMachine();
            _userService = new UserService();
            _currentUser = null;
            _isRunning = true;

            Localization.Load("global_view_localization.json", "en");
        }

        public void Start()
        {
            Console.WriteLine(Localization.GetLangKey("app.title"));

            while (_isRunning)
            {
                DisplayLoginMenu();

                Console.WriteLine(Localization.GetLangKey("app.pressAnyKeyToContinue"));
                Console.ReadKey();
                Console.Clear();
            }
        }

        private void DisplayLoginMenu()
        {
            Console.WriteLine(Localization.GetLangKey("authMenu.title"));
            Console.WriteLine(Localization.GetLangKey("authMenu.login"));
            Console.WriteLine(Localization.GetLangKey("authMenu.register"));
            Console.WriteLine(Localization.GetLangKey("authMenu.forgotPassword"));
            Console.WriteLine(Localization.GetLangKey("authMenu.exit"));
            Console.Write(Localization.GetLangKey("mainMenu.selectOption"));

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Login();
                    break;
                case "2":
                    Register();
                    break;
                case "3":
                    ForgotPassword();
                    break;
                case "0":
                    _isRunning = false;
                    Console.WriteLine(Localization.GetLangKey("authMenu.exitMessage"));
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine(Localization.GetLangKey("app.invalidMenu"));
                    break;
            }
        }

        private void Login()
        {
            Console.WriteLine(Localization.GetLangKey("login.title"));
            Console.Write(Localization.GetLangKey("login.emailUsername"));
            string? email = Console.ReadLine();

            Console.Write(Localization.GetLangKey("login.password"));
            string? password = Console.ReadLine();

            if (!ValidateInput(email, "Email") || !ValidateInput(password, "Password"))
            {
                return;
            }

            var loginRequest = new LoginRequest
            {
                Email = email!,
                Password = password!
            };

            var result = _userService.Login(loginRequest.Email, loginRequest.Password);

            if (result != null)
            {
                _authState.ActivateTrigger(AuthStateMachine.Trigger.LOGIN);
                Console.WriteLine(Localization.GetLangKey("login.success"));

                var user = result;

                if (user != null)
                {
                    _currentUser = user;
                    Console.WriteLine($"{Localization.GetLangKey("login.welcome")} {user.Name}!");
                    Console.WriteLine(Localization.GetLangKey("login.enteringSystem"));
                    Thread.Sleep(1000);

                    if (user.Role == "Mahasiswa")
                    {
                        Console.Clear();
                        var studentView = new StudentView(user, _authState);
                        studentView.Start();
                        _currentUser = null;
                        _authState.ActivateTrigger(AuthStateMachine.Trigger.LOGOUT);
                    }
                    else if (user.Role == "Dosen")
                    {
                        Console.Clear();
                        var lecturerView = new LecturerView(user, _authState);
                        lecturerView.Start();
                        _currentUser = null;
                        _authState.ActivateTrigger(AuthStateMachine.Trigger.LOGOUT);
                    }
                    else
                    {
                        Console.WriteLine(Localization.GetLangKey("login.roleNotRecognized"));
                        _currentUser = null;
                        _authState.ActivateTrigger(AuthStateMachine.Trigger.LOGOUT);
                    }
                }
                else
                {
                    Console.WriteLine(Localization.GetLangKey("login.errorFetchingUserData"));
                }
            }
            else if (result is UnauthorizedObjectResult)
            {
                Console.WriteLine(Localization.GetLangKey("login.invalidCredentials"));
            }
        }

        private void Register()
        {
            Console.WriteLine(Localization.GetLangKey("register.title"));

            Console.Write(Localization.GetLangKey("register.name"));
            string? name = Console.ReadLine();

            Console.Write(Localization.GetLangKey("register.email"));
            string? email = Console.ReadLine();

            Console.Write(Localization.GetLangKey("register.username"));
            string? username = Console.ReadLine();

            Console.Write(Localization.GetLangKey("register.password"));
            string? password = Console.ReadLine();

            if (!ValidateInput(name, "Nama") || !ValidateInput(email, "Email") ||
                !ValidateInput(username, "Username") || !ValidateInput(password, "Password"))
            {
                return;
            }

            // Validasi role dengan angka
            string role = "";

            Console.WriteLine(Localization.GetLangKey("register.selectRole"));
            Console.WriteLine(Localization.GetLangKey("register.roleStudent"));
            Console.WriteLine(Localization.GetLangKey("register.roleLecturer"));
            Console.Write(Localization.GetLangKey("register.yourChoice"));
            string? roleChoice = Console.ReadLine();

            if (roleChoice == "1")
            {
                role = "Mahasiswa";
            }
            else if (roleChoice == "2")
            {
                role = "Dosen";
            }
            else
            {
                Console.WriteLine("Tidak ada pilihan tersebut. Registrasi dibatalkan.");
                return; // Keluar dari metode Register jika role tidak valid
            }

            var user = new RegisterRequest
            {
                Name = name!,
                Email = email!,
                Username = username!,
                Password = password!,
                Role = role
            };

            var checkEmail = _userService.GetByEmail(user.Email);
            var checkUsername = _userService.GetByUsername(user.Username);

            if (checkEmail != null)
            {
                Console.WriteLine(Localization.GetLangKey("register.emailAlreadyRegistered"));
                return;
            }

            if (checkUsername != null)
            {
                Console.WriteLine(Localization.GetLangKey("register.usernameAlreadyRegistered"));
                return;
            }

            try
            {
                _userService.Register(user.Email, user.Password, user.Name, user.Username, user.Role);
                Console.WriteLine(Localization.GetLangKey("register.success"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{Localization.GetLangKey("register.failed")} {ex.Message}");
            }
        }

        private void ForgotPassword()
        {
            Console.WriteLine(Localization.GetLangKey("password.title"));
            Console.Write(Localization.GetLangKey("password.email"));
            string? email = Console.ReadLine();

            if (!ValidateInput(email, Localization.GetLangKey("password.email")))
            {
                return;
            }

            Console.Write(Localization.GetLangKey("password.pasword"));
            string? newPassword = Console.ReadLine();

            if (!ValidateInput(newPassword, Localization.GetLangKey("password.password")))
            {
                return;
            }

            try
            {
                bool success = _userService.ResetPassword(email!, newPassword!);
                if (success)
                {
                    Console.WriteLine(Localization.GetLangKey("password.resetSuccess"));
                }
                else
                {
                    Console.WriteLine(Localization.GetLangKey("password.emailNotFound"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private bool ValidateInput(string? input, string fieldName)
        {
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine($"{fieldName} {Localization.GetLangKey("validateInput.error")}");
                return false;
            }
            return true;
        }
    }
}
