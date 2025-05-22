//using PaperNest_API.Controllers;
//using PaperNest_API.Models;
//using PaperNest_API.Utils;
//using Microsoft.AspNetCore.Mvc;
//using View.Lecturer;
//using View.Student;

//namespace View.Global
//{
//    public class GlobalView
//    {
//        private readonly AuthController _authController;
//        private readonly AuthStateMachine _authState;
//        private User? _currentUser;
//        private bool _isRunning;

//        public GlobalView()
//        {
//            _authController = new AuthController();
//            _authState = new AuthStateMachine();
//            _currentUser = null;
//            _isRunning = true;
//        }

//        public void Start()
//        {
//            Console.WriteLine("=== PaperNest - Sistem Manajemen Karya Tulis Ilmiah ===");

//            while (_isRunning)
//            {
//                DisplayLoginMenu();
                
//                Console.WriteLine("\nTekan tombol apa saja untuk melanjutkan...");
//                Console.ReadKey();
//                Console.Clear();
//            }
//        }

//        private void DisplayLoginMenu()
//        {
//            Console.WriteLine("\n=== Menu Autentikasi ===");
//            Console.WriteLine("1. Login");
//            Console.WriteLine("2. Register");
//            Console.WriteLine("0. Keluar");
//            Console.Write("Pilih menu: ");
            
//            string? choice = Console.ReadLine();
            
//            switch (choice)
//            {
//                case "1":
//                    Login();
//                    break;
//                case "2":
//                    Register();
//                    break;
//                case "0":
//                    _isRunning = false;
//                    break;
//                default:
//                    Console.WriteLine("Menu tidak valid. Silakan coba lagi.");
//                    break;
//            }
//        }

//        private void Login()
//        {
//            Console.WriteLine("\n=== Login ===");
//            Console.Write("Username: ");
//            string? username = Console.ReadLine();
            
//            Console.Write("Password: ");
//            string? password = Console.ReadLine();
            
//            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
//            {
//                Console.WriteLine("Username dan password tidak boleh kosong!");
//                return;
//            }
            
//            var result = _authController.Login(username, password);
            
//            if (result is OkObjectResult okResult)
//            {
//                _authState.ActivateTrigger(AuthStateMachine.Trigger.LOGIN);
//                Console.WriteLine("Login berhasil!");
                
//                // Ambil data user
//                var user = PaperNest_API.Repository.UserRepository.userRepository.FirstOrDefault(u => u.Username.Equals(username) && u.Password.Equals(password));
                
//                if (user != null)
//                {
//                    _currentUser = user;
//                    Console.WriteLine($"Selamat datang, {user.Name}!");
//                    Console.WriteLine("Memasuki sistem...");
//                    System.Threading.Thread.Sleep(1000); // Delay sedikit untuk UX
                    
//                    // Redirect berdasarkan role
//                    if (user.Role == "Mahasiswa")
//                    {
//                        Console.Clear();
//                        var studentView = new StudentView();
//                        studentView.Start();
//                        // Setelah keluar dari StudentView, reset state 
//                        _currentUser = null;
//                        _authState.ActivateTrigger(AuthStateMachine.Trigger.LOGOUT);
//                    }
//                    else if (user.Role == "Dosen")
//                    {
//                        Console.Clear();
//                        var lecturerView = new LecturerView();
//                        lecturerView.Start();
//                        // Setelah keluar dari LecturerView, reset state
//                        _currentUser = null;
//                        _authState.ActivateTrigger(AuthStateMachine.Trigger.LOGOUT);
//                    }
//                    else
//                    {
//                        Console.WriteLine("Role tidak dikenali. Silakan hubungi administrator.");
//                        _currentUser = null;
//                        _authState.ActivateTrigger(AuthStateMachine.Trigger.LOGOUT);
//                    }
//                }
//                else
//                {
//                    Console.WriteLine("Terjadi kesalahan saat mengambil data pengguna.");
//                }
//            }
//            else if (result is UnauthorizedObjectResult)
//            {
//                Console.WriteLine("Username atau password salah. Silakan coba lagi.");
//            }
//        }

//        private void Register()
//        {
//            Console.WriteLine("\n=== Register ===");
            
//            Console.Write("Nama: ");
//            string? name = Console.ReadLine();
            
//            Console.Write("Email: ");
//            string? email = Console.ReadLine();
            
//            Console.Write("Username: ");
//            string? username = Console.ReadLine();
            
//            Console.Write("Password: ");
//            string? password = Console.ReadLine();
            
//            Console.WriteLine("Pilih Role:");
//            Console.WriteLine("1. Mahasiswa");
//            Console.WriteLine("2. Dosen");
//            Console.Write("Pilihan Anda: ");
//            string? roleChoice = Console.ReadLine();
            
//            string role;
//            switch (roleChoice)
//            {
//                case "1":
//                    role = "Mahasiswa";
//                    break;
//                case "2":
//                    role = "Dosen";
//                    break;
//                default:
//                    Console.WriteLine("Pilihan tidak valid. Pendaftaran dibatalkan.");
//                    return;
//            }
            
//            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || 
//                string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
//            {
//                Console.WriteLine("Semua field harus diisi!");
//                return;
//            }
            
//            var user = new User
//            {
//                Name = name,
//                Email = email,
//                Username = username,
//                Password = password,
//                Role = role
//            };
            
//            var result = _authController.Register(user);
            
//            if (result is OkObjectResult)
//            {
//                Console.WriteLine("Registrasi berhasil! Silakan login.");
//            }
//            else
//            {
//                Console.WriteLine("Registrasi gagal. Username mungkin sudah digunakan.");
//            }
//        }
//    }
//}
