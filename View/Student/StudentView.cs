//using API.Controllers;
//using API.Models;
//using API.StateMachines;
//using API.Services;
//using Microsoft.AspNetCore.Mvc;

//namespace View.Student
//{
//    public class StudentView
//    {
//        private readonly UserController _userController;
//        private readonly AuthController _authController;
//        private readonly WorkspaceController _workspaceController;
//        private readonly DocumentController _documentController;
//        private readonly AuthStateMachine _authState;
//        private User? _currentUser;
//        private Workspace? _currentWorkspace;

//        public StudentView()
//        {
//            _userController = new UserController();
//            _authController = new AuthController();
//            _workspaceController = new WorkspaceController();
//            _documentController = new DocumentController();
//            _authState = new AuthStateMachine();
//            _currentUser = null;
//            _currentWorkspace = null;
//        }

//        public void Start()
//        {
//            bool isRunning = true;
            
//            Console.WriteLine("=== PaperNest - Sistem Manajemen Karya Tulis Ilmiah ===");
//            Console.WriteLine("=== Panel Mahasiswa ===");
            
//            while (isRunning)
//            {
//                if (_authState.GetCurrentState() == AuthStateMachine.AuthState.BELUM_LOGIN)
//                {
//                    DisplayLoginMenu();
//                }
//                else
//                {
//                    DisplayMainMenu();
//                }
                
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
//                    Environment.Exit(0);
//                    break;
//                default:
//                    Console.WriteLine("Menu tidak valid. Silakan coba lagi.");
//                    break;
//            }
//        }

//        private void DisplayMainMenu()
//        {
//            Console.WriteLine($"\n=== Menu Utama - Selamat datang, {_currentUser?.Name} ===");
//            Console.WriteLine("1. Buat Workspace");
//            Console.WriteLine("2. Lihat Workspace");
//            Console.WriteLine("3. Kelola Workspace");
//            Console.WriteLine("8. Lihat Profil");
//            Console.WriteLine("9. Logout");
//            Console.Write("Pilih menu: ");
            
//            string? choice = Console.ReadLine();
            
//            switch (choice)
//            {
//                case "1":
//                    CreateNewWorkspace();
//                    break;
//                case "2":
//                    ViewWorkspaces();
//                    break;
//                case "3":
//                    ManageWorkspaces();
//                    break;
//                case "8":
//                    DisplayUserProfile();
//                    break;
//                case "9":
//                    Logout();
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
                
//                // Validasi bahwa user adalah mahasiswa
//                if (user != null && user.Role == "Mahasiswa")
//                {
//                    _currentUser = user;
//                }
//                else
//                {
//                    Console.WriteLine("Akun ini bukan milik mahasiswa. Silakan login dengan akun mahasiswa.");
//                    _authState.ActivateTrigger(AuthStateMachine.Trigger.LOGOUT);
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
            
//            // Tetapkan role sebagai Mahasiswa
//            string role = "Mahasiswa";
//            Console.WriteLine("Role: Mahasiswa (otomatis)");
            
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

//        private void Logout()
//        {
//            _currentUser = null;
//            _authState.ActivateTrigger(AuthStateMachine.Trigger.LOGOUT);
//            Console.WriteLine("Logout berhasil.");
//        }

//        private void DisplayUserProfile()
//        {
//            if (_currentUser == null)
//            {
//                Console.WriteLine("Tidak ada user yang sedang login.");
//                return;
//            }
            
//            Console.WriteLine("\n=== Profil User ===");
//            Console.WriteLine($"ID: {_currentUser.Id}");
//            Console.WriteLine($"Nama: {_currentUser.Name}");
//            Console.WriteLine($"Email: {_currentUser.Email}");
//            Console.WriteLine($"Username: {_currentUser.Username}");
//            Console.WriteLine($"Role: {_currentUser.Role}");
//            Console.WriteLine($"Dibuat pada: {_currentUser.Created_at}");
//        }
        
//        // Method untuk membuat workspace baru
//        private void CreateNewWorkspace()
//        {
//            if (_currentUser == null)
//            {
//                Console.WriteLine("Anda harus login terlebih dahulu.");
//                return;
//            }
            
//            Console.WriteLine("\n=== Buat Workspace Baru ===");
            
//            Console.Write("Nama Workspace: ");
//            string? name = Console.ReadLine();
            
//            Console.Write("Deskripsi (opsional): ");
//            string? description = Console.ReadLine();
            
//            if (string.IsNullOrEmpty(name))
//            {
//                Console.WriteLine("Nama workspace tidak boleh kosong!");
//                return;
//            }
            
//            var workspace = new Workspace
//            {
//                Name = name,
//                Description = description,
//                User_id = _currentUser.Id,
//                Updated_at = DateTime.Now
//            };
            
//            var result = _workspaceController.CreateWorkspace(workspace);
            
//            if (result is ObjectResult okResult && okResult.StatusCode >= 200 && okResult.StatusCode < 300)
//            {
//                Console.WriteLine("Workspace berhasil dibuat!");
//                Console.WriteLine($"ID Workspace: {workspace.Id}");
//                Console.WriteLine("Simpan ID ini untuk memberikan akses ke dosen Anda.");
//            }
//            else
//            {
//                Console.WriteLine("Gagal membuat workspace.");
//            }
//        }
        
//        // Method untuk melihat semua workspace
//        private void ViewWorkspaces()
//        {
//            if (_currentUser == null)
//            {
//                Console.WriteLine("Anda harus login terlebih dahulu.");
//                return;
//            }
            
//            Console.WriteLine("\n=== Daftar Workspace ===");
            
//            // Untuk mahasiswa, tampilkan workspace yang dimiliki
//            var result = _workspaceController.GetWorkspacesByUserId(_currentUser.Id);
            
//            if (result is OkObjectResult okResult)
//            {
//                dynamic? resultData = okResult.Value;
//                var workspaces = resultData?.data as IEnumerable<Workspace>;
                
//                if (workspaces == null || !workspaces.Any())
//                {
//                    Console.WriteLine("Belum ada workspace.");
//                    return;
//                }
                
//                int index = 1;
//                foreach (var workspace in workspaces)
//                {
//                    string description = workspace.Description ?? "Tidak ada deskripsi";
//                    string createdAt = workspace.Created_at.ToString("dd/MM/yyyy HH:mm:ss");
                    
//                    Console.WriteLine($"{index}. {workspace.Name} [ID: {workspace.Id}]");
//                    Console.WriteLine($"   Deskripsi: {description}");
//                    Console.WriteLine($"   Dibuat pada: {createdAt}");
//                    Console.WriteLine();
//                    index++;
//                }
                
//                Console.Write("Pilih workspace (nomor) atau 0 untuk kembali: ");
//                if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= workspaces.Count())
//                {
//                    _currentWorkspace = workspaces.ElementAt(choice - 1);
//                    WorkspaceMenu();
//                }
//            }
//            else
//            {
//                Console.WriteLine("Gagal mendapatkan daftar workspace.");
//            }
//        }
        
//        // Method untuk mengelola workspace
//        private void ManageWorkspaces()
//        {
//            if (_currentUser == null)
//            {
//                Console.WriteLine("Anda harus login terlebih dahulu.");
//                return;
//            }
            
//            ViewWorkspaces(); // Tampilkan daftar workspace saat user tidak null
//        }
        
//        // Menu untuk workspace yang dipilih
//        private void WorkspaceMenu()
//        {
//            if (_currentWorkspace == null)
//            {
//                Console.WriteLine("Tidak ada workspace yang dipilih.");
//                return;
//            }
            
//            bool backToMainMenu = false;
            
//            while (!backToMainMenu)
//            {
//                Console.WriteLine($"\n=== Workspace: {_currentWorkspace.Name} ===");
//                Console.WriteLine($"ID: {_currentWorkspace.Id} (Bagikan ID ini kepada dosen untuk bergabung)");
                
//                // Menu lengkap untuk mahasiswa
//                Console.WriteLine("1. Lihat Dokumen");
//                Console.WriteLine("2. Buat Dokumen Baru");
//                Console.WriteLine("3. Edit Info Workspace");
//                Console.WriteLine("4. Hapus Workspace");
//                Console.WriteLine("0. Kembali ke Menu Utama");
                
//                Console.Write("Pilih menu: ");
                
//                string? choice = Console.ReadLine();
                
//                switch (choice)
//                {
//                    case "1":
//                        ViewDocuments();
//                        break;
//                    case "2":
//                        CreateNewDocument();
//                        break;
//                    case "3":
//                        EditWorkspace();
//                        break;
//                    case "4":
//                        if (DeleteWorkspace())
//                        {
//                            backToMainMenu = true;
//                        }
//                        break;
//                    case "0":
//                        backToMainMenu = true;
//                        _currentWorkspace = null;
//                        break;
//                    default:
//                        Console.WriteLine("Menu tidak valid. Silakan coba lagi.");
//                        break;
//                }
                
//                if (!backToMainMenu)
//                {
//                    Console.WriteLine("\nTekan tombol apa saja untuk melanjutkan...");
//                    Console.ReadKey();
//                    Console.Clear();
//                }
//            }
//        }
        
//        // Method untuk melihat dokumen dalam workspace
//        private void ViewDocuments()
//        {
//            if (_currentWorkspace == null)
//            {
//                Console.WriteLine("Tidak ada workspace yang dipilih.");
//                return;
//            }
            
//            Console.WriteLine($"\n=== Dokumen di Workspace: {_currentWorkspace.Name} ===");
            
//            var result = _documentController.GetDocumentsByWorkspaceId(_currentWorkspace.Id);
            
//            if (result is OkObjectResult okResult)
//            {
//                dynamic? resultData = okResult.Value;
//                var documents = resultData?.data as IEnumerable<Document>;
                
//                if (documents == null || !documents.Any())
//                {
//                    Console.WriteLine("Belum ada dokumen di workspace ini.");
//                    return;
//                }
                
//                int index = 1;
//                foreach (var document in documents)
//                {
//                    // Cek status draft dari properti HasDraft
//                    string draftInfo = "";
                    
//                    if (document.HasDraft && document.LastEditedByUserId.HasValue)
//                    {
//                        var lastEditor = PaperNest_API.Repository.UserRepository.userRepository.FirstOrDefault(u => u.Id == document.LastEditedByUserId.Value);
//                        string lastEditorName = lastEditor?.Name ?? "Pengguna lain";
//                        draftInfo = $"[Draft tersedia - terakhir diedit oleh {lastEditorName}]";
//                    }
                    
//                    Console.WriteLine($"{index}. {document.Title} {draftInfo}");
//                    Console.WriteLine($"   Deskripsi: {document.Description ?? "Tidak ada deskripsi"}");
//                    Console.WriteLine($"   Dibuat pada: {document.Created_at.ToString("dd/MM/yyyy HH:mm:ss")}");
//                    Console.WriteLine();
//                    index++;
//                }
                
//                Console.Write("Pilih dokumen (nomor) atau 0 untuk kembali: ");
//                if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= documents.Count())
//                {
//                    var selectedDocument = documents.ElementAt(choice - 1);
//                    DocumentMenu(selectedDocument);
//                }
//            }
//            else
//            {
//                Console.WriteLine("Gagal mendapatkan daftar dokumen.");
//            }
//        }
        
//        // Method untuk membuat dokumen baru
//        private void CreateNewDocument()
//        {
//            if (_currentUser == null || _currentWorkspace == null)
//            {
//                Console.WriteLine("Anda harus login dan memilih workspace terlebih dahulu.");
//                return;
//            }
            
//            Console.WriteLine("\n=== Buat Dokumen Baru ===");
            
//            Console.Write("Judul: ");
//            string? title = Console.ReadLine();
            
//            Console.Write("Deskripsi (opsional): ");
//            string? description = Console.ReadLine();
            
//            Console.Write("Konten: ");
//            string? content = Console.ReadLine();            
            
//            if (string.IsNullOrEmpty(title))
//            {
//                Console.WriteLine("Judul tidak boleh kosong!");
//                return;
//            }
            
//            var document = new Document
//            {
//                Title = title,
//                Description = description,
//                Content = content,
//                User_id = _currentUser.Id,
//                Workspace_id = _currentWorkspace.Id,
//                Updated_at = DateTime.Now
//            };
            
//            var result = _documentController.CreateDocument(document);
            
//            if (result is ObjectResult okResult && okResult.StatusCode >= 200 && okResult.StatusCode < 300)
//            {
//                Console.WriteLine("Dokumen berhasil dibuat!");
//            }
//            else
//            {
//                Console.WriteLine("Gagal membuat dokumen.");
//            }
//        }
        
//        // Menu untuk dokumen yang dipilih
//        private void DocumentMenu(Document document)
//        {
//            if (document == null)
//            {
//                Console.WriteLine("Tidak ada dokumen yang dipilih.");
//                return;
//            }
            
//            bool backToWorkspaceMenu = false;
            
//            while (!backToWorkspaceMenu)
//            {
//                // Tampilkan informasi tentang draft jika ada
//                string draftInfo = "";
                
//                if (document.HasDraft && document.LastEditedByUserId.HasValue)
//                {
//                    var lastEditor = PaperNest_API.Repository.UserRepository.userRepository.FirstOrDefault(u => u.Id == document.LastEditedByUserId.Value);
//                    string lastEditorName = lastEditor?.Name ?? "Pengguna lain";
//                    draftInfo = $"\nAda draft tersedia (terakhir diedit oleh {lastEditorName} pada {document.LastEditedAt?.ToString("dd/MM/yyyy HH:mm:ss") ?? "waktu tidak diketahui"})";
//                }
                
//                // Tambahkan informasi tentang status review terbaru
//                string reviewInfo = "";
//                var versions = DocumentBodyService.GetVersions(document.Id);
//                if (versions != null && versions.Any())
//                {
//                    var currentVersion = versions.FirstOrDefault(v => v.IsCurrentVersion);
//                    if (currentVersion != null && currentVersion.IsReviewed)
//                    {
//                        string statusText = "";
//                        switch (currentVersion.ReviewResult)
//                        {
//                            case ReviewResult.Approved:
//                                statusText = "DISETUJUI";
//                                break;
//                            case ReviewResult.Rejected:
//                                statusText = "DITOLAK";
//                                break;
//                            case ReviewResult.NeedsRevision:
//                                statusText = "PERLU REVISI";
//                                break;
//                            default:
//                                statusText = currentVersion.ReviewResult.ToString();
//                                break;
//                        }
//                        reviewInfo = $"\nVersi saat ini: [{statusText}] (Klik menu 'Lihat Versi Dokumen' untuk detail)";
//                    }
//                    else if (currentVersion != null)
//                    {
//                        reviewInfo = "\nVersi saat ini belum direview";
//                    }
//                }
                
//                Console.WriteLine($"\n=== Dokumen: {document.Title} ===");
//                Console.WriteLine($"Deskripsi: {document.Description ?? "Tidak ada deskripsi"}");
//                Console.WriteLine($"Konten: {document.Content ?? "Tidak ada konten"}");
//                Console.WriteLine($"Dibuat pada: {document.Created_at.ToString("dd/MM/yyyy HH:mm:ss")}");
                
//                Console.WriteLine(draftInfo);
                
//                // Tampilkan info review untuk semua pengguna
//                Console.WriteLine(reviewInfo);
                
//                // Menu lengkap untuk mahasiswa
//                Console.WriteLine("\n1. Edit Dokumen (Judul, Deskripsi)");
//                Console.WriteLine("2. Edit Konten Dokumen");
//                Console.WriteLine("3. Hapus Dokumen");
//                Console.WriteLine("4. Manajemen Versi Dokumen");
//                Console.WriteLine("0. Kembali ke Menu Workspace");
                
//                Console.Write("Pilih menu: ");
                
//                string? choice = Console.ReadLine();
                
//                switch (choice)
//                {
//                    case "1":
//                        EditDocumentMetadata(document);
//                        // Refresh document data
//                        var refreshResult = _documentController.GetDocumentById(document.Id);
//                        if (refreshResult is OkObjectResult okResult)
//                        {
//                            dynamic? resultData = okResult.Value;
//                            document = resultData?.data as Document;
//                        }
//                        break;
//                    case "2":
//                        EditDocumentContent(document);
//                        // Refresh document data
//                        var refreshContentResult = _documentController.GetDocumentById(document.Id);
//                        if (refreshContentResult is OkObjectResult contentOkResult)
//                        {
//                            dynamic? resultData = contentOkResult.Value;
//                            document = resultData?.data as Document;
//                        }
//                        break;
//                    case "3":
//                        if (DeleteDocument(document.Id))
//                        {
//                            backToWorkspaceMenu = true;
//                        }
//                        break;
//                    case "4":
//                        ManageDocumentVersions(document);
//                        break;
//                    case "0":
//                        backToWorkspaceMenu = true;
//                        break;
//                    default:
//                        Console.WriteLine("Menu tidak valid. Silakan coba lagi.");
//                        break;
//                }
                
//                if (!backToWorkspaceMenu)
//                {
//                    Console.WriteLine("\nTekan tombol apa saja untuk melanjutkan...");
//                    Console.ReadKey();
//                    Console.Clear();
//                }
//            }
//        }
        
//        // Method untuk mengedit metadata dokumen (judul, deskripsi, status)
//        private void EditDocumentMetadata(Document document)
//        {
//            if (document == null)
//            {
//                Console.WriteLine("Tidak ada dokumen yang dipilih.");
//                return;
//            }
            
//            Console.WriteLine($"\n=== Edit Dokumen: {document.Title} ===");
            
//            Console.Write($"Judul Baru (kosongkan untuk tetap '{document.Title}'): ");
//            string? title = Console.ReadLine();
            
//            Console.Write($"Deskripsi Baru (kosongkan untuk tetap '{document.Description ?? "kosong"}'): ");
//            string? description = Console.ReadLine();
            
//            if (!string.IsNullOrEmpty(title))
//            {
//                document.Title = title;
//            }
            
//            if (!string.IsNullOrEmpty(description))
//            {
//                document.Description = description;
//            }
            
//            document.Updated_at = DateTime.Now;
            
//            var result = _documentController.UpdateDocument(document.Id, document);
            
//            if (result is OkObjectResult)
//            {
//                Console.WriteLine("Metadata dokumen berhasil diperbarui!");
//            }
//            else
//            {
//                Console.WriteLine("Gagal memperbarui metadata dokumen.");
//            }
//        }

//        // Method untuk mengedit konten dokumen
//        private void EditDocumentContent(Document document)
//        {
//            if (document == null)
//            {
//                Console.WriteLine("Tidak ada dokumen yang dipilih.");
//                return;
//            }
            
//            if (_currentUser == null)
//            {
//                Console.WriteLine("Anda harus login terlebih dahulu.");
//                return;
//            }
            
//            Console.WriteLine($"\n=== Edit Konten Dokumen: {document.Title} ===");
            
//            // Ambil konten dari dokumen saat ini
//            string initialContent = document.Content ?? string.Empty;
            
//            // Tampilkan info jika dokumen memiliki draft (konten yang belum diversion)
//            if (document.HasDraft && document.LastEditedByUserId.HasValue)
//            {
//                var lastEditor = PaperNest_API.Repository.UserRepository.userRepository.FirstOrDefault(u => u.Id == document.LastEditedByUserId);
//                string lastEditorName = lastEditor?.Name ?? "Pengguna lain";
                
//                Console.WriteLine($"Melanjutkan pengeditan draft dari {lastEditorName}");
//                if (document.LastEditedAt.HasValue)
//                {
//                    Console.WriteLine($"Terakhir diubah: {document.LastEditedAt.Value}");
//                }
//            }
//            else
//            {
//                Console.WriteLine($"Konten Sebelumnya:\n{initialContent}\n");
//            }
            
//            Console.WriteLine("Masukkan konten baru (bisa dimodifikasi dari konten sebelumnya):");
            
//            // Gunakan hanya metode interaktif
//            string newContent = ReadAndEditMultilineText(initialContent);
            
//            if (newContent == initialContent)
//            {
//                Console.WriteLine("Tidak ada perubahan pada konten.");
//                return;
//            }
            
//            // Simpan draft langsung ke document.Content
//            document.Content = newContent;
//            document.LastEditedByUserId = _currentUser.Id;
//            document.LastEditedAt = DateTime.Now;
//            document.HasDraft = true;  // Tandai bahwa ini adalah draft
//            document.Updated_at = DateTime.Now;
            
//            var result = _documentController.UpdateDocument(document.Id, document);
            
//            if (result is OkObjectResult)
//            {
//                Console.WriteLine("Konten dokumen berhasil diperbarui!");
//                Console.WriteLine("Catatan: Versi baru belum dibuat. Untuk membuat versi baru, silakan pilih menu 'Kirim Versi Baru' di menu Manajemen Versi Dokumen.");
//                Console.WriteLine("Draft tersimpan dan dapat dilihat oleh semua anggota workspace.");
//            }
//            else
//            {
//                Console.WriteLine("Gagal memperbarui konten dokumen.");
//            }
//        }

//        // Method untuk manajemen versi dokumen
//        private void ManageDocumentVersions(Document document)
//        {
//            if (document == null)
//            {
//                Console.WriteLine("Tidak ada dokumen yang dipilih.");
//                return;
//            }
            
//            bool backToDocumentMenu = false;
            
//            while (!backToDocumentMenu)
//            {
//                Console.WriteLine($"\n=== Manajemen Versi Dokumen: {document.Title} ===");
//                Console.WriteLine("1. Lihat Semua Versi");
//                Console.WriteLine("2. Kirim Versi Baru");
//                Console.WriteLine("3. Rollback ke Versi Sebelumnya");
//                Console.WriteLine("0. Kembali ke Menu Dokumen");
//                Console.Write("Pilih menu: ");
                
//                string? choice = Console.ReadLine();
                
//                switch (choice)
//                {
//                    case "1":
//                        ViewDocumentVersions(document.Id);
//                        break;
//                    case "2":
//                        CreateNewDocumentVersion(document);
//                        break;
//                    case "3":
//                        RollbackDocumentVersion(document);
//                        break;
//                    case "0":
//                        backToDocumentMenu = true;
//                        break;
//                    default:
//                        Console.WriteLine("Menu tidak valid. Silakan coba lagi.");
//                        break;
//                }
                
//                if (!backToDocumentMenu)
//                {
//                    Console.WriteLine("\nTekan tombol apa saja untuk melanjutkan...");
//                    Console.ReadKey();
//                    Console.Clear();
//                }
//            }
//        }

//        // Method untuk membuat versi baru dokumen tanpa mengedit konten
//        private void CreateNewDocumentVersion(Document document)
//        {
//            if (document == null)
//            {
//                Console.WriteLine("Tidak ada dokumen yang dipilih.");
//                return;
//            }
            
//            Console.WriteLine($"\n=== Kirim Versi Baru Dokumen: {document.Title} ===");
            
//            // Periksa apakah versi sebelumnya sudah direview
//            if (!DocumentBodyService.CanCreateNewVersion(document.Id))
//            {
//                Console.WriteLine("Tidak dapat membuat versi baru karena masih ada versi yang belum direview oleh dosen.");
//                Console.WriteLine("Harap tunggu hingga versi sebelumnya direview sebelum membuat versi baru.");
//                return;
//            }
            
//            // Gunakan konten dari dokumen saat ini (yang mungkin berisi draft)
//            string currentContent = document.Content ?? string.Empty;
//            bool hasDraft = document.HasDraft;
            
//            if (hasDraft && document.LastEditedByUserId.HasValue)
//            {
//                var lastEditor = PaperNest_API.Repository.UserRepository.userRepository.FirstOrDefault(u => u.Id == document.LastEditedByUserId.Value);
//                string lastEditorName = lastEditor?.Name ?? "Pengguna lain";
                
//                Console.WriteLine($"Menggunakan draft yang diedit oleh {lastEditorName} pada {document.LastEditedAt}:");
//            }
//            else
//            {
//                Console.WriteLine("Menggunakan konten saat ini (belum ada draft):");
//            }
            
//            Console.WriteLine(currentContent);
            
//            if (string.IsNullOrWhiteSpace(currentContent))
//            {
//                Console.WriteLine("Tidak dapat membuat versi baru karena konten kosong.");
//                return;
//            }
            
//            // Periksa apakah sudah ada versi sebelumnya dengan konten yang sama
//            var versions = DocumentBodyService.GetVersions(document.Id);
//            if (versions != null && versions.Any())
//            {
//                var latestVersion = versions.FirstOrDefault();
//                if (latestVersion != null && latestVersion.Content == currentContent)
//                {
//                    Console.WriteLine("Tidak dapat mengirim versi baru karena konten sama dengan versi sebelumnya.");
//                    Console.WriteLine("Silakan ubah konten dokumen terlebih dahulu.");
//                    return;
//                }
//            }
            
//            Console.Write("\nDeskripsi versi baru (seperti commit message): ");
//            string description = Console.ReadLine() ?? "New version";
            
//            var newVersion = DocumentBodyService.CreateVersion(document.Id, currentContent, description);
//            document.Updated_at = DateTime.Now;
            
//            // Reset draft status setelah konten dijadikan versi
//            if (hasDraft)
//            {
//                document.HasDraft = false;
//            }
            
//            var result = _documentController.UpdateDocument(document.Id, document);
            
//            if (result is OkObjectResult)
//            {
//                Console.WriteLine("Versi baru dokumen berhasil dikirim!");
//                Console.WriteLine("Versi ini perlu direview oleh dosen sebelum Anda dapat membuat versi baru lagi.");
                
//                // Informasikan bahwa draft telah menjadi versi baru
//                if (hasDraft)
//                {
//                    Console.WriteLine("Draft telah dikonversi menjadi versi baru.");
//                }
//            }
//            else
//            {
//                Console.WriteLine("Gagal mengirim versi baru dokumen.");
//            }
//        }
        
//        // Method untuk melihat semua versi dokumen
//        private void ViewDocumentVersions(Guid documentId)
//        {
//            Console.WriteLine("\n=== Daftar Versi Dokumen ===");
            
//            var versions = DocumentBodyService.GetVersions(documentId);
            
//            if (versions == null || !versions.Any())
//            {
//                Console.WriteLine("Belum ada versi dokumen.");
//                return;
//            }
            
//            int index = 1;
//            foreach (var version in versions)
//            {
//                string reviewStatus = "";
//                // Cek apakah sudah di-review
//                if (version.IsReviewed && version.ReviewId != Guid.Empty)
//                {
//                    // Tampilkan hasil review dengan jelas
//                    switch (version.ReviewResult)
//                    {
//                        case ReviewResult.Approved:
//                            reviewStatus = "[DISETUJUI]";
//                            break;
//                        case ReviewResult.Rejected:
//                            reviewStatus = "[DITOLAK]";
//                            break;
//                        case ReviewResult.NeedsRevision:
//                            reviewStatus = "[PERLU REVISI]";
//                            break;
//                        default:
//                            reviewStatus = $"[{version.ReviewResult}]";
//                            break;
//                    }
//                }
//                else if (version.ReviewId == Guid.Empty)
//                {
//                    reviewStatus = "[BELUM DIREVIEW]";
//                }
                
//                Console.WriteLine($"{index}. Versi dari {version.Created_at.ToString("dd/MM/yyyy HH:mm:ss")} {reviewStatus}");
//                Console.WriteLine($"   {(version.IsCurrentVersion ? "[AKTIF]" : "")}");
//                Console.WriteLine($"   Deskripsi: {version.VersionDescription}");
//                // Tampilkan preview konten (maksimal 50 karakter)
//                string contentPreview = version.Content.Length > 50 
//                    ? version.Content.Substring(0, 50) + "..." 
//                    : version.Content;
//                Console.WriteLine($"   Preview: {contentPreview}");
//                Console.WriteLine();
//                index++;
//            }
            
//            Console.Write("Pilih versi untuk melihat detail (nomor) atau 0 untuk kembali: ");
//            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= versions.Count())
//            {
//                var selectedVersion = versions.ElementAt(choice - 1);
//                ViewVersionDetailWithReview(selectedVersion);
//            }
//        }

//        // Method untuk melihat detail versi dengan review (jika ada)
//        private void ViewVersionDetailWithReview(DocumentBody version)
//        {
//            if (version == null)
//            {
//                Console.WriteLine("Versi tidak ditemukan.");
//                return;
//            }
            
//            Console.WriteLine($"\n=== Detail Versi {version.Id} ===");
//            Console.WriteLine($"Dibuat pada: {version.Created_at.ToString("dd/MM/yyyy HH:mm:ss")}");
//            Console.WriteLine($"Status: {(version.IsCurrentVersion ? "Aktif" : "Tidak Aktif")}");
//            Console.WriteLine($"Deskripsi: {version.VersionDescription}");
//            Console.WriteLine("\nKonten:");
//            Console.WriteLine(version.Content);
            
//            // Tampilkan informasi review jika ada
//            if (version.IsReviewed && version.ReviewId != Guid.Empty)
//            {
//                Console.WriteLine("\n=== Hasil Review ===");
                
//                // Tampilkan status review dengan format yang mudah dibaca
//                string statusReview = "";
//                switch (version.ReviewResult)
//                {
//                    case ReviewResult.Approved:
//                        statusReview = "DISETUJUI ✓";
//                        break;
//                    case ReviewResult.Rejected:
//                        statusReview = "DITOLAK ✗";
//                        break;
//                    case ReviewResult.NeedsRevision:
//                        statusReview = "PERLU REVISI !";
//                        break;
//                    default:
//                        statusReview = version.ReviewResult.ToString();
//                        break;
//                }
                
//                Console.WriteLine($"Status: {statusReview}");
                
//                // Ambil detail review dari ResearchRequest
//                var researchRequestController = new ResearchRequestController();
//                var requests = researchRequestController.GetAllRequests();
//                var request = requests.FirstOrDefault(r => r.Id == version.ReviewId);
                
//                if (request != null && request.Reviews.Count > 0)
//                {
//                    foreach (var review in request.Reviews)
//                    {
//                        Console.WriteLine($"\nReviewer: {review.ReviewerName}");
//                        Console.WriteLine($"Tanggal: {review.ReviewDate.ToString("dd/MM/yyyy HH:mm:ss")}");
//                        Console.WriteLine("Komentar:");
//                        Console.WriteLine("------------");
//                        Console.WriteLine(review.Comment);
//                        Console.WriteLine("------------");
//                    }
                    
//                    // Tampilkan panduan untuk mahasiswa berdasarkan status review
//                    Console.WriteLine("\nPanduan tindak lanjut:");
//                    switch (version.ReviewResult)
//                    {
//                        case ReviewResult.Approved:
//                            Console.WriteLine("Dokumen Anda telah disetujui. Anda dapat melanjutkan ke tahap berikutnya.");
//                            break;
//                        case ReviewResult.NeedsRevision:
//                            Console.WriteLine("Dokumen Anda memerlukan revisi. Silakan perbaiki sesuai komentar reviewer.");
//                            Console.WriteLine("Setelah selesai merevisi, buat versi baru untuk direview kembali.");
//                            break;
//                        case ReviewResult.Rejected:
//                            Console.WriteLine("Dokumen Anda ditolak. Harap lakukan perubahan signifikan sesuai komentar reviewer.");
//                            Console.WriteLine("Setelah selesai merevisi, buat versi baru untuk direview kembali.");
//                            break;
//                    }
//                }
//                else
//                {
//                    Console.WriteLine("Detail review tidak tersedia.");
//                }
//            }
//            else
//            {
//                Console.WriteLine("\nVersi ini belum direview.");
//                Console.WriteLine("Versi ini masih menunggu review dari dosen.");
//                Console.WriteLine("Silakan hubungi dosen Anda untuk mempercepat proses review.");
//            }
//        }

//        // Method untuk mengedit workspace
//        private void EditWorkspace()
//        {
//            if (_currentWorkspace == null)
//            {
//                Console.WriteLine("Tidak ada workspace yang dipilih.");
//                return;
//            }
            
//            Console.WriteLine($"\n=== Edit Workspace: {_currentWorkspace.Name} ===");
            
//            Console.Write($"Nama Baru (kosongkan untuk tetap '{_currentWorkspace.Name}'): ");
//            string? name = Console.ReadLine();
            
//            Console.Write($"Deskripsi Baru (kosongkan untuk tetap '{_currentWorkspace.Description ?? "kosong"}'): ");
//            string? description = Console.ReadLine();
            
//            if (!string.IsNullOrEmpty(name))
//            {
//                _currentWorkspace.Name = name;
//            }
            
//            if (!string.IsNullOrEmpty(description))
//            {
//                _currentWorkspace.Description = description;
//            }
            
//            _currentWorkspace.Updated_at = DateTime.Now;
            
//            var result = _workspaceController.UpdateWorkspace(_currentWorkspace.Id, _currentWorkspace);
            
//            if (result is OkObjectResult)
//            {
//                Console.WriteLine("Workspace berhasil diperbarui!");
//            }
//            else
//            {
//                Console.WriteLine("Gagal memperbarui workspace.");
//            }
//        }
        
//        // Method untuk menghapus workspace
//        private bool DeleteWorkspace()
//        {
//            if (_currentWorkspace == null)
//            {
//                Console.WriteLine("Tidak ada workspace yang dipilih.");
//                return false;
//            }
            
//            Console.WriteLine($"\n=== Hapus Workspace: {_currentWorkspace.Name} ===");
//            Console.Write("Anda yakin ingin menghapus workspace ini? (y/n): ");
//            string? confirmation = Console.ReadLine();
            
//            if (confirmation?.ToLower() != "y")
//            {
//                Console.WriteLine("Penghapusan workspace dibatalkan.");
//                return false;
//            }
            
//            var result = _workspaceController.DeleteWorkspace(_currentWorkspace.Id);
            
//            if (result is OkObjectResult)
//            {
//                Console.WriteLine("Workspace berhasil dihapus!");
//                _currentWorkspace = null;
//                return true;
//            }
//            else
//            {
//                Console.WriteLine("Gagal menghapus workspace.");
//                return false;
//            }
//        }
        
//        // Method untuk menghapus dokumen
//        private bool DeleteDocument(Guid documentId)
//        {
//            Console.WriteLine("\n=== Hapus Dokumen ===");
//            Console.Write("Anda yakin ingin menghapus dokumen ini? (y/n): ");
//            string? confirmation = Console.ReadLine();
            
//            if (confirmation?.ToLower() != "y")
//            {
//                Console.WriteLine("Penghapusan dokumen dibatalkan.");
//                return false;
//            }
            
//            var result = _documentController.DeleteDocument(documentId);
            
//            if (result is OkObjectResult)
//            {
//                Console.WriteLine("Dokumen berhasil dihapus!");
//                return true;
//            }
//            else
//            {
//                Console.WriteLine("Gagal menghapus dokumen.");
//                return false;
//            }
//        }

//        // Method untuk rollback ke versi sebelumnya
//        private void RollbackDocumentVersion(Document document)
//        {
//            if (document == null)
//            {
//                Console.WriteLine("Tidak ada dokumen yang dipilih.");
//                return;
//            }
            
//            Console.WriteLine($"\n=== Rollback Dokumen: {document.Title} ===");
            
//            var versions = DocumentBodyService.GetVersions(document.Id);
            
//            if (versions == null || !versions.Any())
//            {
//                Console.WriteLine("Belum ada versi dokumen untuk rollback.");
//                return;
//            }
            
//            int index = 1;
//            foreach (var version in versions)
//            {
//                if (!version.IsCurrentVersion) // Tampilkan hanya versi non-aktif
//                {
//                    Console.WriteLine($"{index}. Versi dari {version.Created_at.ToString("dd/MM/yyyy HH:mm:ss")}");
//                    Console.WriteLine($"   Deskripsi: {version.VersionDescription}");
//                    // Tampilkan preview konten (maksimal 50 karakter)
//                    string contentPreview = version.Content.Length > 50 
//                        ? version.Content.Substring(0, 50) + "..." 
//                        : version.Content;
//                    Console.WriteLine($"   Preview: {contentPreview}");
//                    Console.WriteLine();
//                }
//                index++;
//            }
            
//            Console.Write("Pilih versi untuk rollback (nomor) atau 0 untuk kembali: ");
//            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= versions.Count())
//            {
//                var selectedVersion = versions.ElementAt(choice - 1);
                
//                Console.Write($"Anda yakin ingin rollback ke versi dari {selectedVersion.Created_at}? (y/n): ");
//                string? confirmation = Console.ReadLine();
                
//                if (confirmation?.ToLower() == "y")
//                {
//                    var newVersion = DocumentBodyService.RollbackToVersion(document.Id, selectedVersion.Id);
//                    document.Content = newVersion.Content;
//                    document.Updated_at = DateTime.Now;
                    
//                    var result = _documentController.UpdateDocument(document.Id, document);
                    
//                    if (result is OkObjectResult)
//                    {
//                        Console.WriteLine("Rollback berhasil!");
//                    }
//                    else
//                    {
//                        Console.WriteLine("Gagal melakukan rollback.");
//                    }
//                }
//                else
//                {
//                    Console.WriteLine("Rollback dibatalkan.");
//                }
//            }
//        }
        
//        // Metode pengeditan teks interaktif untuk multiline text
//        private string ReadAndEditMultilineText(string initialText)
//        {
//            // Split text into lines for multiline support
//            List<string> lines = initialText.Split(Environment.NewLine).ToList();
//            if (lines.Count == 0) lines.Add(string.Empty);
            
//            int currentLine = 0;
//            bool editing = true;
            
//            Console.Clear();
//            Console.WriteLine("==== EDITOR INTERAKTIF ====");
//            Console.WriteLine("Navigasi: ↑↓ = pindah baris, ←→ = pindah karakter");
//            Console.WriteLine("Esc = keluar dan simpan, Ctrl+C = keluar tanpa simpan");
//            Console.WriteLine("===============================");
            
//            // Display all lines
//            Console.WriteLine();
//            for (int i = 0; i < lines.Count; i++)
//            {
//                Console.WriteLine($"{i+1}: {lines[i]}");
//            }
            
//            // Position cursor at the beginning of first line
//            Console.SetCursorPosition(3, 5); // Adjust for header and line numbers
//            int cursorX = 3;  // Line number + ": " = 3 characters
//            int cursorY = 5;  // Starting line after header
            
//            while (editing)
//            {
//                // Read a key press without displaying it
//                var key = Console.ReadKey(true);
                
//                switch (key.Key)
//                {
//                    case ConsoleKey.Escape:
//                        editing = false;  // Done editing
//                        break;
                        
//                    case ConsoleKey.UpArrow:
//                        if (currentLine > 0)
//                        {
//                            currentLine--;
//                            cursorY--;
//                            // Make sure cursor X position is valid for the new line
//                            cursorX = Math.Min(lines[currentLine].Length + 3, cursorX);
//                            Console.SetCursorPosition(cursorX, cursorY);
//                        }
//                        break;
                        
//                    case ConsoleKey.DownArrow:
//                        if (currentLine < lines.Count - 1)
//                        {
//                            currentLine++;
//                            cursorY++;
//                            // Make sure cursor X position is valid for the new line
//                            cursorX = Math.Min(lines[currentLine].Length + 3, cursorX);
//                            Console.SetCursorPosition(cursorX, cursorY);
//                        }
//                        break;
                        
//                    case ConsoleKey.LeftArrow:
//                        if (cursorX > 3)  // Don't move left of the line number prefix
//                        {
//                            cursorX--;
//                            Console.SetCursorPosition(cursorX, cursorY);
//                        }
//                        break;
                        
//                    case ConsoleKey.RightArrow:
//                        if (cursorX < lines[currentLine].Length + 3)
//                        {
//                            cursorX++;
//                            Console.SetCursorPosition(cursorX, cursorY);
//                        }
//                        break;
                        
//                    case ConsoleKey.Enter:
//                        // Insert a new line
//                        string currentLineText = lines[currentLine];
//                        int posInLine = cursorX - 3;
                        
//                        string beforeCursor = posInLine > 0 ? currentLineText.Substring(0, posInLine) : "";
//                        string afterCursor = posInLine < currentLineText.Length ? currentLineText.Substring(posInLine) : "";
                        
//                        lines[currentLine] = beforeCursor;
//                        lines.Insert(currentLine + 1, afterCursor);
                        
//                        // Redraw all lines after the current one
//                        for (int i = currentLine; i < lines.Count; i++)
//                        {
//                            Console.SetCursorPosition(0, i + 5);
//                            Console.Write(new string(' ', Console.WindowWidth));  // Clear the line
//                            Console.SetCursorPosition(0, i + 5);
//                            Console.Write($"{i+1}: {lines[i]}");
//                        }
                        
//                        currentLine++;
//                        cursorY++;
//                        cursorX = 3;  // Move to beginning of the new line
//                        Console.SetCursorPosition(cursorX, cursorY);
//                        break;
                        
//                    case ConsoleKey.Backspace:
//                        int positionInLine = cursorX - 3;
//                        if (positionInLine > 0)
//                        {
//                            // Remove character from current line
//                            string lineText = lines[currentLine];
//                            lines[currentLine] = lineText.Remove(positionInLine - 1, 1);
                            
//                            // Redraw the current line
//                            Console.SetCursorPosition(0, cursorY);
//                            Console.Write(new string(' ', Console.WindowWidth));  // Clear the line
//                            Console.SetCursorPosition(0, cursorY);
//                            Console.Write($"{currentLine+1}: {lines[currentLine]}");
                            
//                            // Move cursor back one position
//                            cursorX--;
//                            Console.SetCursorPosition(cursorX, cursorY);
//                        }
//                        else if (currentLine > 0)
//                        {
//                            // At beginning of line, join with previous line
//                            string previousLine = lines[currentLine - 1];
//                            string lineToJoin = lines[currentLine];
                            
//                            // Remove the current line and append its content to the previous line
//                            lines.RemoveAt(currentLine);
//                            int previousLineLength = previousLine.Length;
//                            lines[currentLine - 1] = previousLine + lineToJoin;
                            
//                            // Redraw all lines from previous line onwards
//                            for (int i = currentLine - 1; i < lines.Count; i++)
//                            {
//                                Console.SetCursorPosition(0, i + 5);
//                                Console.Write(new string(' ', Console.WindowWidth));  // Clear the line
//                                Console.SetCursorPosition(0, i + 5);
//                                Console.Write($"{i+1}: {lines[i]}");
//                            }
                            
//                            // Clear the last line that's now empty
//                            Console.SetCursorPosition(0, lines.Count + 5);
//                            Console.Write(new string(' ', Console.WindowWidth));
                            
//                            // Update cursor position
//                            currentLine--;
//                            cursorY--;
//                            cursorX = 3 + previousLineLength;
//                            Console.SetCursorPosition(cursorX, cursorY);
//                        }
//                        break;
                        
//                    default:
//                        // For regular key presses, insert the character at the cursor position
//                        if (char.IsLetterOrDigit(key.KeyChar) || char.IsPunctuation(key.KeyChar) || key.KeyChar == ' ')
//                        {
//                            int pos = cursorX - 3;  // Adjust for line number prefix
//                            string line = lines[currentLine];
                            
//                            // Insert the character
//                            if (pos >= 0 && pos <= line.Length)
//                            {
//                                lines[currentLine] = line.Insert(pos, key.KeyChar.ToString());
                                
//                                // Redraw the current line
//                                Console.SetCursorPosition(0, cursorY);
//                                Console.Write(new string(' ', Console.WindowWidth));  // Clear the line
//                                Console.SetCursorPosition(0, cursorY);
//                                Console.Write($"{currentLine+1}: {lines[currentLine]}");
                                
//                                // Move cursor forward one position
//                                cursorX++;
//                                Console.SetCursorPosition(cursorX, cursorY);
//                            }
//                        }
//                        break;
//                }
//            }
            
//            Console.Clear();  // Clear the screen after editing is done
//            return string.Join(Environment.NewLine, lines);
//        }
//    }
//}