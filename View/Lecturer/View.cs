//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using API.Controllers;
//using API.Models;
//using API.StateMachines;
//using API.Services;
//using Microsoft.AspNetCore.Mvc;

//namespace View.Lecturer
//{
//    public class LecturerView
//    {
//        private readonly UserController _userController;
//        private readonly AuthController _authController;
//        private readonly WorkspaceController _workspaceController;
//        private readonly DocumentController _documentController;
//        private readonly AuthStateMachine _authState;
//        private User? _currentUser;
//        private Workspace? _currentWorkspace;

//        public LecturerView()
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
//            Console.WriteLine("=== Panel Dosen ===");
            
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

//        private void DisplayMainMenu()
//        {
//            Console.WriteLine($"\n=== Menu Utama - Selamat datang, {_currentUser?.Name} ===");
//            Console.WriteLine("1. Lihat Workspace");
//            Console.WriteLine("2. Bergabung dengan Workspace");
//            Console.WriteLine("8. Lihat Profil");
//            Console.WriteLine("9. Logout");
//            Console.Write("Pilih menu: ");
            
//            string? choice = Console.ReadLine();
            
//            switch (choice)
//            {
//                case "1":
//                    ViewWorkspaces();
//                    break;
//                case "2":
//                    JoinWorkspace();
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
                
//                // Validasi bahwa user adalah dosen
//                if (user != null && user.Role == "Dosen")
//                {
//                    _currentUser = user;
//                }
//                else
//                {
//                    Console.WriteLine("Akun ini bukan milik dosen. Silakan login dengan akun dosen.");
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
            
//            // Tetapkan role sebagai Dosen
//            string role = "Dosen";
//            Console.WriteLine("Role: Dosen (otomatis)");
            
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
        
//        // Method untuk melihat semua workspace
//        private void ViewWorkspaces()
//        {
//            if (_currentUser == null)
//            {
//                Console.WriteLine("Anda harus login terlebih dahulu.");
//                return;
//            }
            
//            Console.WriteLine("\n=== Daftar Workspace ===");
            
//            // Untuk dosen, tampilkan workspace yang diikuti
//            var result = _workspaceController.GetJoinedWorkspaces(_currentUser.Id);
            
//            if (result is OkObjectResult okResult)
//            {
//                dynamic? resultData = okResult.Value;
//                var workspaces = resultData?.data as IEnumerable<Workspace>;
                
//                if (workspaces == null || !workspaces.Any())
//                {
//                    Console.WriteLine("Anda belum bergabung dengan workspace manapun.");
//                    Console.WriteLine("Gunakan menu 'Bergabung dengan Workspace' untuk bergabung.");
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
        
//        // Method khusus dosen untuk bergabung dengan workspace
//        private void JoinWorkspace()
//        {
//            if (_currentUser == null)
//            {
//                Console.WriteLine("Anda harus login terlebih dahulu.");
//                return;
//            }
            
//            Console.WriteLine("\n=== Bergabung dengan Workspace ===");
//            Console.WriteLine("Masukkan ID Workspace untuk bergabung.");
//            Console.WriteLine("Anda dapat meminta ID Workspace dari mahasiswa pengelola workspace.");
            
//            Console.Write("\nID Workspace: ");
//            string? workspaceIdStr = Console.ReadLine();
            
//            if (string.IsNullOrEmpty(workspaceIdStr))
//            {
//                Console.WriteLine("ID Workspace tidak boleh kosong!");
//                return;
//            }
            
//            // Coba parse ID workspace
//            if (!Guid.TryParse(workspaceIdStr, out Guid workspaceId))
//            {
//                Console.WriteLine("ID Workspace tidak valid. Pastikan format ID benar.");
//                return;
//            }
            
//            // Cek apakah workspace ada
//            var result = _workspaceController.GetWorkspaceById(workspaceId);
            
//            if (result is OkObjectResult okResult)
//            {
//                dynamic? resultData = okResult.Value;
//                var workspace = resultData?.data as Workspace;
                
//                if (workspace != null)
//                {
//                    Console.WriteLine($"Workspace ditemukan: {workspace.Name}");
//                    Console.WriteLine($"Deskripsi: {workspace.Description ?? "Tidak ada deskripsi"}");
//                    Console.WriteLine($"Pemilik: {workspace.User?.Name ?? "Unknown"}");
                    
//                    Console.Write("\nApakah Anda yakin ingin bergabung dengan workspace ini? (y/n): ");
//                    string? confirmation = Console.ReadLine()?.ToLower();
                    
//                    if (confirmation == "y")
//                    {
//                        // Bergabung dengan workspace
//                        var joinResult = _workspaceController.JoinWorkspace(workspaceId, _currentUser.Id);
                        
//                        if (joinResult is OkObjectResult joinOkResult)
//                        {
//                            Console.WriteLine("\nBerhasil bergabung dengan workspace!");
//                            Console.WriteLine("Anda sekarang dapat melihat dokumen dan memberikan review pada workspace ini.");
//                        }
//                        else
//                        {
//                            Console.WriteLine("\nGagal bergabung dengan workspace. Silakan coba lagi nanti.");
//                        }
//                    }
//                    else
//                    {
//                        Console.WriteLine("\nBergabung dengan workspace dibatalkan.");
//                    }
//                }
//                else
//                {
//                    Console.WriteLine("Workspace tidak ditemukan. Silakan periksa ID workspace.");
//                }
//            }
//            else
//            {
//                Console.WriteLine("Workspace tidak ditemukan. Silakan periksa ID workspace.");
//            }
//        }
        
//        // Menu untuk workspace yang dipilih (dosen)
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
                
//                // Menu terbatas untuk dosen - hanya lihat dokumen
//                Console.WriteLine("1. Lihat Dokumen");
//                Console.WriteLine("0. Kembali ke Menu Utama");
                
//                Console.Write("Pilih menu: ");
                
//                string? choice = Console.ReadLine();
                
//                switch (choice)
//                {
//                    case "1":
//                        ViewDocuments();
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
        
//        // Menu untuk dokumen yang dipilih (dosen)
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
                
//                // Tampilkan info review
//                Console.WriteLine(reviewInfo);
                
//                // Menu khusus dosen
//                Console.WriteLine("\n1. Lihat Versi Dokumen");
//                Console.WriteLine("2. Review Dokumen");
//                Console.WriteLine("0. Kembali ke Menu Workspace");
                
//                Console.Write("Pilih menu: ");
                
//                string? choice = Console.ReadLine();
                
//                switch (choice)
//                {
//                    case "1":
//                        ViewDocumentVersions(document.Id);
//                        break;
//                    case "2":
//                        ReviewDocumentVersions(document.Id);
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
//                }
//                else
//                {
//                    Console.WriteLine("Detail review tidak tersedia.");
//                }
//            }
//            else
//            {
//                Console.WriteLine("\nVersi ini belum direview.");
                
//                // Berikan opsi untuk review
//                Console.Write("\nApakah Anda ingin mereview versi ini? (y/n): ");
//                string? choice = Console.ReadLine()?.ToLower();
//                if (choice == "y")
//                {
//                    ReviewVersion(version);
//                }
//            }
//        }

//        // Method untuk mereview versi dokumen
//        private void ReviewDocumentVersions(Guid documentId)
//        {
//            Console.WriteLine("\n=== Review Versi Dokumen ===");
            
//            // Dapatkan versi dokumen yang sudah di-commit (bukan draft)
//            var versions = DocumentBodyService.GetVersions(documentId)
//                   .Where(v => v.IsCurrentVersion || (!v.IsCurrentVersion && v.ReviewId == Guid.Empty))
//                   .ToList();
            
//            if (versions == null || !versions.Any())
//            {
//                Console.WriteLine("Belum ada versi dokumen yang perlu di-review.");
//                return;
//            }
            
//            int index = 1;
//            foreach (var version in versions)
//            {
//                string reviewStatus = "";
//                // Jika reviewId adalah empty, berarti belum direview
//                if (version.ReviewId == Guid.Empty)
//                {
//                    reviewStatus = "[PERLU REVIEW]";
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
            
//            Console.Write("Pilih versi untuk review (nomor) atau 0 untuk kembali: ");
//            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= versions.Count())
//            {
//                var selectedVersion = versions.ElementAt(choice - 1);
//                ReviewVersion(selectedVersion);
//            }
//        }

//        // Method untuk mereview satu versi dokumen
//        private void ReviewVersion(DocumentBody version)
//        {
//            if (version == null)
//            {
//                Console.WriteLine("Versi tidak ditemukan.");
//                return;
//            }
            
//            Console.WriteLine($"\n=== Review Versi {version.Id} ===");
//            Console.WriteLine($"Dibuat pada: {version.Created_at.ToString("dd/MM/yyyy HH:mm:ss")}");
//            Console.WriteLine($"Status: {(version.IsCurrentVersion ? "Aktif" : "Tidak Aktif")}");
//            Console.WriteLine($"Deskripsi: {version.VersionDescription}");
//            Console.WriteLine("\nKonten:");
//            Console.WriteLine(version.Content);
            
//            // Jika sudah direview sebelumnya, tampilkan informasi review
//            if (version.IsReviewed && version.ReviewId != Guid.Empty)
//            {
//                Console.WriteLine("\nDokumen ini sudah direview sebelumnya.");
//                Console.WriteLine($"Hasil review: {version.ReviewResult}");
                
//                Console.WriteLine("\nApakah Anda ingin membuat review baru? (y/n): ");
//                string? choice = Console.ReadLine()?.ToLower();
//                if (choice != "y")
//                {
//                    return;
//                }
//            }
            
//            Console.WriteLine("\n=== Buat Review ===");
//            Console.WriteLine("1. Setujui (Approve)");
//            Console.WriteLine("2. Perlu Revisi (Needs Revision)");
//            Console.WriteLine("3. Tolak (Reject)");
//            Console.WriteLine("0. Kembali tanpa Review");
            
//            Console.Write("Pilihan: ");
//            string? reviewChoice = Console.ReadLine();
            
//            if (reviewChoice == "0")
//            {
//                return;
//            }
            
//            // Tentukan result berdasarkan pilihan
//            ReviewResult result;
//            switch (reviewChoice)
//            {
//                case "1":
//                    result = ReviewResult.Approved;
//                    break;
//                case "2":
//                    result = ReviewResult.NeedsRevision;
//                    break;
//                case "3":
//                    result = ReviewResult.Rejected;
//                    break;
//                default:
//                    Console.WriteLine("Pilihan tidak valid.");
//                    return;
//            }
            
//            Console.Write("Masukkan komentar untuk review: ");
//            string comment = Console.ReadLine() ?? "";
            
//            // Buat objek ResearchRequest jika belum ada
//            // Kita perlu membuat permintaan penelitian untuk versi ini
//            var document = DocumentBodyService.GetDocumentById(version.DocumentId);
//            if (document == null)
//            {
//                Console.WriteLine("Dokumen tidak ditemukan.");
//                return;
//            }
            
//            // Cek apakah sudah ada research request untuk dokumen ini
//            var researchRequestController = new ResearchRequestController();
//            var allRequests = researchRequestController.GetAllRequests();
//            var existingRequest = allRequests.FirstOrDefault(r => r.DocumentBodyId == version.Id);
            
//            Guid requestId;
            
//            if (existingRequest == null)
//            {
//                // Buat research request baru
//                string title = $"Review untuk {document.Title} - {version.VersionDescription}";
//                string abstractText = $"Review dokumen versi dari {version.Created_at}";
//                string researcherName = document.User?.Name ?? "Unknown";
                
//                // Tambahkan request
//                researchRequestController.AddRequest(title, abstractText, researcherName, document.User_id, version.Id);
                
//                // Dapatkan ID request yang baru dibuat
//                var newRequests = researchRequestController.GetAllRequests();
//                var newRequest = newRequests.LastOrDefault();
//                if (newRequest == null)
//                {
//                    Console.WriteLine("Gagal membuat permintaan review.");
//                    return;
//                }
                
//                requestId = newRequest.Id;
//            }
//            else
//            {
//                requestId = existingRequest.Id;
//            }
            
//            // Mulai review
//            researchRequestController.StartReview(requestId);
            
//            // Proses review
//            researchRequestController.ProcessReview(requestId, result, _currentUser.Id, comment);
            
//            // Update ReviewId di DocumentBody
//            var success = DocumentBodyService.MarkVersionAsReviewed(version.Id, requestId, result);
            
//            if (success)
//            {
//                Console.WriteLine("\nReview berhasil disimpan!");
//                Console.WriteLine($"Status review: {result}");
//                if (result == ReviewResult.Approved)
//                {
//                    Console.WriteLine("Dokumen telah disetujui. Mahasiswa dapat melanjutkan versi berikutnya.");
//                }
//                else if (result == ReviewResult.NeedsRevision)
//                {
//                    Console.WriteLine("Dokumen perlu revisi. Mahasiswa akan diminta untuk membuat perubahan.");
//                }
//                else
//                {
//                    Console.WriteLine("Dokumen ditolak. Mahasiswa perlu membuat perubahan besar sebelum mengajukan ulang.");
//                }
//            }
//            else
//            {
//                Console.WriteLine("\nGagal memperbarui status review pada dokumen.");
//            }
//        }
//    }
//}
