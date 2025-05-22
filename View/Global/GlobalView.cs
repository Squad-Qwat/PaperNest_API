using PaperNest_API.Controllers;
using Microsoft.AspNetCore.Mvc;
using View.Student;
//using View.Lecturer;
using API.Services;
using API.Models;
using API.Models.DataBinding;
using Microsoft.AspNetCore.Authentication;
using System.Threading;
using System.Text.RegularExpressions;
using View.Lecturer;
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

            do
            {
                DisplayLoginMenu();

                Console.WriteLine("\nTekan tombol apa saja untuk melanjutkan...");
                Console.ReadKey();
                Console.Clear();
            } while (_isRunning);
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

            if (result == null || result is UnauthorizedObjectResult)
            {
                Console.WriteLine("Username atau password salah. Silakan coba lagi.");
                _authState.ActivateTrigger(AuthStateMachine.Trigger.LOGOUT);
                return;
            }

            _authState.ActivateTrigger(AuthStateMachine.Trigger.LOGIN);
            Console.WriteLine("Login berhasil!");

            var user = result;

            if (user == null)
            {
                Console.WriteLine("Terjadi kesalahan saat mengambil data pengguna.");
                _authState.ActivateTrigger(AuthStateMachine.Trigger.LOGOUT);
                return;
            }

            _currentUser = user;
            Console.WriteLine($"Selamat datang, {user.Name}!");
            Console.WriteLine("Memasuki sistem...");
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
                Console.WriteLine("Role tidak dikenali. Silakan hubungi administrator.");
                _currentUser = null;
                _authState.ActivateTrigger(AuthStateMachine.Trigger.LOGOUT);
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
                if (!success)
                {
                    Console.WriteLine("Email tidak ditemukan dalam sistem.");
                }

                Console.WriteLine("Password berhasil direset! Silakan login dengan password baru.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private bool ValidateInput(string? input, string fieldName)
        {
            try
            {
                if (string.IsNullOrEmpty(input))
                {
                    throw new ArgumentException($"{fieldName} tidak boleh kosong!");
                }

                /* note: Regex for email validation is commented out to avoid dependency issues
                if (fieldName == "Email" && !Regex.IsMatch(input, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    throw new ArgumentException("Format email tidak valid!");
                }
                */

                if (fieldName == "Password" && input.Length < 8)
                {
                    throw new ArgumentException("Password harus minimal 8 karakter!");
                }

                if (fieldName == "Username" && input.Length > 15)
                {
                    throw new ArgumentException("Username tidak boleh lebih dari 15 karakter!");
                }

                if (fieldName == "Nama" && input.Length > 100)
                {
                    throw new ArgumentException("Nama tidak boleh lebih dari 100 karakter!");
                }

                if (fieldName == "Role" && (input != "1" && input != "2"))
                {
                    throw new ArgumentException("Pilihan role tidak valid! Pilih 1 untuk Mahasiswa atau 2 untuk Dosen.");
                }

                if (fieldName == "Password Baru" && input.Length < 8)
                {
                    throw new ArgumentException("Password baru harus minimal 8 karakter!");
                }

                return true;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error validasi input: {ex.Message}");
                return false;
            }
        }
    }
}