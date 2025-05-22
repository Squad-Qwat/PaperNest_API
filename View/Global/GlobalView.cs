using PaperNest_API.Controllers;
using Microsoft.AspNetCore.Mvc;
using View.Student;
//using View.Lecturer;
using API.Services;
using API.Models;
using API.Models.DataBinding;
using Microsoft.AspNetCore.Authentication;
using View.Lecturer;
using Microsoft.AspNetCore.Identity.Data;
using API.StateMachineAndUtils;

namespace View.Global
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
        }

        public void Start()
        {
            Console.WriteLine("=== PaperNest - Sistem Manajemen Karya Tulis Ilmiah ===");

            while (_isRunning)
            {
                DisplayLoginMenu();

                Console.WriteLine("\nTekan tombol apa saja untuk melanjutkan...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        private void DisplayLoginMenu()
        {
            Console.WriteLine("\n=== Menu Autentikasi ===");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Register");
            Console.WriteLine("3. Lupa Password");
            Console.WriteLine("0. Keluar");
            Console.Write("Pilih menu: ");

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
                    Console.WriteLine("Terima kasih telah menggunakan PaperNest. Keluar dari aplikasi...");
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Menu tidak valid. Silakan coba lagi.");
                    break;
            }
        }

        private void Login()
        {
            Console.WriteLine("\n=== Login ===");
            Console.Write("Email/Username: ");
            string? email = Console.ReadLine();

            Console.Write("Password: ");
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
                Console.WriteLine("Login berhasil!");

                var user = result;

                if (user != null)
                {
                    _currentUser = user;
                    Console.WriteLine($"Selamat datang, {user.Name}!");
                    Console.WriteLine("Memasuki sistem...");
                    System.Threading.Thread.Sleep(1000);

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
                        Console.WriteLine("Role tidak dikenali. Silakan hubungi administrator.");
                        _currentUser = null;
                        _authState.ActivateTrigger(AuthStateMachine.Trigger.LOGOUT);
                    }
                }
                else
                {
                    Console.WriteLine("Terjadi kesalahan saat mengambil data pengguna.");
                }
            }
            else if (result is UnauthorizedObjectResult)
            {
                Console.WriteLine("Username atau password salah. Silakan coba lagi.");
            }
        }

        private void Register()
        {
            Console.WriteLine("\n=== Register ===");

            Console.Write("Nama: ");
            string? name = Console.ReadLine();

            Console.Write("Email: ");
            string? email = Console.ReadLine();

            Console.Write("Username: ");
            string? username = Console.ReadLine();

            Console.Write("Password: ");
            string? password = Console.ReadLine();

            Console.WriteLine("Pilih Role:");
            Console.WriteLine("1. Mahasiswa");
            Console.WriteLine("2. Dosen");
            Console.Write("Pilihan Anda: ");
            string? roleChoice = Console.ReadLine();

            if (!ValidateInput(name, "Nama") || !ValidateInput(email, "Email") ||
                !ValidateInput(username, "Username") || !ValidateInput(password, "Password") ||
                !ValidateInput(roleChoice, "Role"))
            {
                return;
            }

            string role = roleChoice == "1" ? "Mahasiswa" : "Dosen";

            // For some reason, RegisterRequest doesn't recognized some of these properties
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
                Console.WriteLine("Email sudah terdaftar. Silakan gunakan email lain.");
                return;
            }

            if (checkUsername != null)
            {
                Console.WriteLine("Username sudah terdaftar. Silakan gunakan username lain.");
                return;
            }

            try
            {
                _userService.Register(user.Email, user.Password, user.Name, user.Username, user.Role);
                Console.WriteLine("Registrasi berhasil! Silakan login.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Registrasi gagal: {ex.Message}");
            }
        }

        private void ForgotPassword()
        {
            Console.WriteLine("\n=== Lupa Password ===");
            Console.Write("Email: ");
            string? email = Console.ReadLine();

            if (!ValidateInput(email, "Email"))
            {
                return;
            }

            Console.Write("Password Baru: ");
            string? newPassword = Console.ReadLine();

            if (!ValidateInput(newPassword, "Password Baru"))
            {
                return;
            }

            try
            {
                bool success = _userService.ResetPassword(email!, newPassword!);
                if (success)
                {
                    Console.WriteLine("Password berhasil direset! Silakan login dengan password baru.");
                }
                else
                {
                    Console.WriteLine("Email tidak ditemukan dalam sistem.");
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
                Console.WriteLine($"{fieldName} tidak boleh kosong!");
                return false;
            }
            return true;
        }
    }
}
