using API.Controllers;
using API.Models;
using API.Services;
using View.Global;
using Microsoft.AspNetCore.Mvc;
using PaperNest_API.Controllers;
using Microsoft.AspNetCore.Http;
using API.Helpers.Enums;
using API.Helpers.ExtraClass;
using API.StateMachineAndUtils;

namespace View.Student
{
    public class StudentView
    {
        private readonly UserService _userService;
        private readonly WorkspaceService _workspaceService;
        private readonly DocumentService _documentService;
        private readonly AuthStateMachine _authState;
        private readonly DocumentBodyService _documentBodyService;
        private readonly ReviewService _reviewService;
        private readonly UserWorkspaceService _userWorkspaceService;
        private User? _currentUser;
        private Workspace? _currentWorkspace;
        private readonly GlobalView _globalView;
        private readonly ReviewUtil _reviewUtil; // Add ReviewUtil

        public StudentView(User currentUser, AuthStateMachine authState)
        {
            _userService = new UserService();
            _workspaceService = new WorkspaceService();
            _documentService = new DocumentService();
            _authState = authState;
            _currentUser = currentUser;
            _currentWorkspace = null;
            _globalView = new GlobalView();
            _documentBodyService = new DocumentBodyService();
            _reviewService = new ReviewService();
            _userWorkspaceService = new UserWorkspaceService();
            _reviewUtil = new ReviewUtil(); // Initialize ReviewUtil
        }

        public void Start()
        {
            bool isRunning = true;

            Console.WriteLine("=== PaperNest - Sistem Manajemen Karya Tulis Ilmiah ===");
            Console.WriteLine("=== Panel Mahasiswa ===");

            while (isRunning)
            {
                if (_currentUser == null || _authState.GetCurrentState() == AuthStateMachine.AuthState.BELUM_LOGIN)
                {
                    _globalView.Start();
                }
                else
                {
                    DisplayMainMenu();
                }

                Console.WriteLine("\nTekan tombol apa saja untuk melanjutkan...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        private void DisplayMainMenu()
        {
            Console.WriteLine($"\n=== Menu Utama - Selamat datang, {_currentUser?.Name} ===");
            Console.WriteLine("1. Buat Workspace");
            Console.WriteLine("2. Lihat Workspace");
            Console.WriteLine("3. Kelola Workspace");
            Console.WriteLine("4. Join Workspace");
            Console.WriteLine("8. Lihat Profil");
            Console.WriteLine("9. Logout");
            Console.Write("Pilih menu: ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    CreateNewWorkspace();
                    break;
                case "2":
                    ViewWorkspaces();
                    break;
                case "3":
                    ManageWorkspaces();
                    break;
                case "4":
                    JoinWorkspace();
                    break;
                case "8":
                    DisplayUserProfile();
                    break;
                case "9":
                    Logout();
                    break;
                default:
                    Console.WriteLine("Menu tidak valid. Silakan coba lagi.");
                    break;
            }
        }

        private void Logout()
        {
            _currentUser = null;
            _authState.ActivateTrigger(AuthStateMachine.Trigger.LOGOUT);
            Console.WriteLine("Logout berhasil.");
        }

        private void JoinWorkspace()
        {
            if (_currentUser == null)
            {
                Console.WriteLine("Anda harus login terlebih dahulu.");
                return;
            }
            Console.WriteLine("\n=== Bergabung ke Workspace ===");
            Console.Write("Masukkan ID Workspace: ");
            string? workspaceIdInput = Console.ReadLine();
            if (string.IsNullOrEmpty(workspaceIdInput) || !Guid.TryParse(workspaceIdInput, out Guid workspaceId))
            {
                Console.WriteLine("ID Workspace tidak valid.");
                return;
            }

            var workspace = _workspaceService.GetById(workspaceId);
            if (workspace == null)
            {
                Console.WriteLine("Workspace tidak ditemukan.");
                return;
            }
            Console.WriteLine("Berhasil masuk ke workspace.");

            _userWorkspaceService.AddUserWorkspaceAsMember(_currentUser.Id, workspaceId);
        }

        private void DisplayUserProfile()
        {
            if (_currentUser == null)
            {
                Console.WriteLine("Tidak ada user yang sedang login.");
                return;
            }

            Console.WriteLine("\n=== Profil User ===");
            Console.WriteLine($"ID: {_currentUser.Id}");
            Console.WriteLine($"Nama: {_currentUser.Name}");
            Console.WriteLine($"Email: {_currentUser.Email}");
            Console.WriteLine($"Username: {_currentUser.Username}");
            Console.WriteLine($"Role: {_currentUser.Role}");
            Console.WriteLine($"Dibuat pada: {_currentUser.CreatedAt}");
        }

        private void CreateNewWorkspace()
        {
            if (_currentUser == null)
            {
                Console.WriteLine("Anda harus login terlebih dahulu.");
                return;
            }

            Console.WriteLine("\n=== Buat Workspace Baru ===");

            Console.Write("Nama Workspace: ");
            string? title = Console.ReadLine();

            Console.Write("Deskripsi (opsional): ");
            string? description = Console.ReadLine();

            if (string.IsNullOrEmpty(title))
            {
                Console.WriteLine("Nama workspace tidak boleh kosong!");
                return;
            }

            var workspace = new Workspace
            {
                Title = title,
                Description = description,
                UpdateAt = DateTime.Now,
            };

            _workspaceService.Create(workspace);

            _userWorkspaceService.AddUserWorkspaceAsOwner(_currentUser.Id, workspace.Id);

            Console.WriteLine("Workspace berhasil dibuat!");
            Console.WriteLine($"ID Workspace: {workspace.Id}");
            Console.WriteLine("Simpan ID ini untuk memberikan akses ke dosen Anda.");
        }

        // Method untuk melihat semua workspace
        private void ViewWorkspaces()
        {
            if (_currentUser == null)
            {
                Console.WriteLine("Anda harus login terlebih dahulu.");
                return;
            }

            Console.WriteLine("\n=== Daftar Workspace ===");

            // Untuk mahasiswa, tampilkan workspace yang dimiliki
            var results = _workspaceService.GetByUserId(_currentUser.Id);


            if (results != null)
            {
                var workspaces = results;

                if (workspaces == null || !workspaces.Any())
                {
                    Console.WriteLine("Belum ada workspace.");
                    return;
                }

                int index = 1;
                foreach (var workspace in workspaces)
                {
                    string description = workspace.Description ?? "Tidak ada deskripsi";
                    string createdAt = workspace.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss");

                    Console.WriteLine($"{index}. {workspace.Title} [ID: {workspace.Id}]");
                    Console.WriteLine($"   Deskripsi: {description}");
                    Console.WriteLine($"   Dibuat pada: {createdAt}");
                    Console.WriteLine();
                    index++;
                }

                Console.Write("Pilih workspace (nomor) atau 0 untuk kembali: ");
                if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= workspaces.Count())
                {
                    _currentWorkspace = workspaces.ElementAt(choice - 1);
                    WorkspaceMenu();
                }
            }
            else
            {
                Console.WriteLine("Gagal mendapatkan daftar workspace.");
            }
        }

        // Method untuk mengelola workspace
        private void ManageWorkspaces()
        {
            if (_currentUser == null)
            {
                Console.WriteLine("Anda harus login terlebih dahulu.");
                return;
            }

            ViewWorkspaces();
        }

        private void WorkspaceMenu()
        {
            if (_currentWorkspace == null)
            {
                Console.WriteLine("Tidak ada workspace yang dipilih.");
                return;
            }

            bool backToMainMenu = false;

            WorkspaceRole userRole = _workspaceService.GetUserRoleInWorkspace(_currentUser.Id, _currentWorkspace.Id);

            while (!backToMainMenu)
            {
                Console.WriteLine($"\n=== Workspace: {_currentWorkspace.Title} ===");
                Console.WriteLine($"ID: {_currentWorkspace.Id} (Bagikan ID ini kepada dosen atau teman untuk bergabung)");
                Console.WriteLine($"Role Anda: {userRole}");

                // Menu untuk semua pengguna
                Console.WriteLine("1. Lihat Dokumen");
                Console.WriteLine("2. Buat Dokumen Baru");

                // Menu hanya untuk Owner dan bukan Member
                if (userRole != WorkspaceRole.Member)
                {
                    Console.WriteLine("3. Edit Info Workspace");
                    Console.WriteLine("4. Hapus Workspace");
                }

                Console.WriteLine("0. Kembali ke Menu Utama");

                Console.Write("Pilih menu: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ViewDocuments();
                        break;
                    case "2":
                        CreateNewDocument();
                        break;
                    case "3":
                        if (userRole != WorkspaceRole.Member)
                        {
                            EditWorkspace();
                        }
                        else
                        {
                            Console.WriteLine("Anda tidak memiliki izin untuk mengedit workspace ini.");
                        }
                        break;
                    case "4":
                        if (userRole != WorkspaceRole.Member)
                        {
                            if (DeleteWorkspace())
                            {
                                backToMainMenu = true;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Anda tidak memiliki izin untuk menghapus workspace ini.");
                        }
                        break;
                    case "0":
                        backToMainMenu = true;
                        _currentWorkspace = null;
                        break;
                    default:
                        Console.WriteLine("Menu tidak valid. Silakan coba lagi.");
                        break;
                }

                if (!backToMainMenu)
                {
                    Console.WriteLine("\nTekan tombol apa saja untuk melanjutkan...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }

        // Method untuk melihat dokumen dalam workspace
        private void ViewDocuments()
        {
            if (_currentWorkspace == null)
            {
                Console.WriteLine("Tidak ada workspace yang dipilih.");
                return;
            }

            Console.WriteLine($"\n=== Dokumen di Workspace: {_currentWorkspace.Title} ===");

            var result = _documentService.GetByWorkspaceId(_currentWorkspace.Id);

            if (result != null)
            {
                var documents = result;

                if (documents == null || !documents.Any())
                {
                    Console.WriteLine("Belum ada dokumen di workspace ini.");
                    return;
                }

                int index = 1;
                foreach (var document in documents)
                {
                    Console.WriteLine($"{index}. {document.Title}");
                    Console.WriteLine($"   Dibuat pada: {document.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss")}");
                    Console.WriteLine();
                    index++;
                }

                Console.Write("Pilih dokumen (nomor) atau 0 untuk kembali: ");
                if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= documents.Count())
                {
                    var selectedDocument = documents.ElementAt(choice - 1);
                    DocumentMenu(selectedDocument);
                }
            }
            else
            {
                Console.WriteLine("Gagal mendapatkan daftar dokumen.");
            }
        }

        // Method untuk membuat dokumen baru
        private void CreateNewDocument()
        {
            if (_currentUser == null || _currentWorkspace == null)
            {
                Console.WriteLine("Anda harus login dan memilih workspace terlebih dahulu.");
                return;
            }

            Console.WriteLine("\n=== Buat Dokumen Baru ===");

            Console.Write("Judul: ");
            string? title = Console.ReadLine();

            Console.Write("Konten: ");
            string? content = Console.ReadLine();

            if (string.IsNullOrEmpty(title))
            {
                Console.WriteLine("Judul tidak boleh kosong!");
                return;
            }

            var document = new Document
            {
                Title = title,
                SavedContent = content,
                FK_WorkspaceId = _currentWorkspace.Id,
                UpdateAt = DateTime.Now
            };

            _documentService.Create(document);

            Console.WriteLine("Dokumen berhasil dibuat!");
        }

        // Menu untuk dokumen yang dipilih
        private void DocumentMenu(Document document)
        {
            if (document == null)
            {
                Console.WriteLine("Tidak ada dokumen yang dipilih.");
                return;
            }

            bool backToWorkspaceMenu = false;

            while (!backToWorkspaceMenu)
            {

                // Tambahkan informasi tentang status review terbaru
                string reviewInfo = "";
                var versions = _documentBodyService.GetDocumentBodiesByDocumentId(document.Id);
                if (versions != null && versions.Any())
                {
                    var currentVersion = _documentBodyService.GetCurrentVersion(document.Id);
                    if (currentVersion != null && currentVersion.IsReviewed)
                    {
                        var review = _reviewService.GetReviewByDocumentBodyId(currentVersion.Id);
                        string statusText = "";
                        if (review != null)
                        {
                            // Using the Review's State instead of direct ReviewStatus
                            if (review.State is ApprovedState)
                            {
                                statusText = "DISETUJUI";
                            }
                            else if (review.State is NeedsRevisionState)
                            {
                                statusText = "PERLU REVISI";
                            }
                            else if (review.State is DoneState)
                            {
                                statusText = "SELESAI";
                            }
                            else if (review.State is SubmittedState) // Added SubmittedState
                            {
                                statusText = "DIAJUKAN";
                            }
                            else if (review.State is UnderReviewState) // Added UnderReviewState
                            {
                                statusText = "DALAM REVIEW";
                            }
                            else
                            {
                                statusText = review.State?.GetType().Name.Replace("State", "").ToUpper() ?? "TIDAK DIKETAHUI";
                            }
                            reviewInfo = $"\nVersi saat ini: [{statusText}] (Klik menu 'Lihat Versi Dokumen' untuk detail)";
                        }
                    }
                    else if (currentVersion != null)
                    {
                        reviewInfo = "\nVersi saat ini belum direview";
                    }
                }

                Console.WriteLine($"\n=== Dokumen: {document.Title} ===");
                Console.WriteLine($"Konten: {document.SavedContent ?? "Tidak ada konten"}");
                Console.WriteLine($"Dibuat pada: {document.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss")}");

                // Tampilkan info review untuk semua pengguna
                Console.WriteLine(reviewInfo);

                // Menu lengkap untuk mahasiswa
                Console.WriteLine("\n1. Edit Judul Dokumen");
                Console.WriteLine("2. Edit Konten Dokumen");
                Console.WriteLine("3. Hapus Dokumen");
                Console.WriteLine("4. Manajemen Versi Dokumen");
                Console.WriteLine("0. Kembali ke Menu Workspace");

                Console.Write("Pilih menu: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        EditDocumentMetadata(document);
                        // Refresh document data
                        var refreshResult = _documentService.GetById(document.Id);
                        if (refreshResult != null)
                        {
                            document = refreshResult;
                        }
                        break;
                    case "2":
                        EditDocumentContent(document);
                        // Refresh document data
                        var refreshContentResult = _documentService.GetById(document.Id);
                        if (refreshContentResult != null)
                        {

                            document = refreshContentResult;
                        }
                        break;
                    case "3":
                        if (DeleteDocument(document.Id))
                        {
                            backToWorkspaceMenu = true;
                        }
                        break;
                    case "4":
                        ManageDocumentVersions(document);
                        break;
                    case "0":
                        backToWorkspaceMenu = true;
                        break;
                    default:
                        Console.WriteLine("Menu tidak valid. Silakan coba lagi.");
                        break;
                }

                if (!backToWorkspaceMenu)
                {
                    Console.WriteLine("\nTekan tombol apa saja untuk melanjutkan...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }

        // Method untuk mengedit metadata dokumen (judul, deskripsi, status)
        private void EditDocumentMetadata(Document document)
        {
            if (document == null)
            {
                Console.WriteLine("Tidak ada dokumen yang dipilih.");
                return;
            }

            Console.WriteLine($"\n=== Edit Dokumen: {document.Title} ===");

            Console.Write($"Judul Baru (kosongkan untuk tetap '{document.Title}'): ");
            string? title = Console.ReadLine();

            if (!string.IsNullOrEmpty(title))
            {
                document.Title = title;
            }

            document.UpdateAt = DateTime.Now;

            _documentService.Update(document.Id, document);
            Console.WriteLine("Dokumen berhasil diperbarui!");
        }

        // Method untuk mengedit konten dokumen
        private void EditDocumentContent(Document document)
        {
            if (document == null)
            {
                Console.WriteLine("Tidak ada dokumen yang dipilih.");
                return;
            }
            if (_currentUser == null)
            {
                Console.WriteLine("Anda harus login terlebih dahulu.");
                return;
            }
            Console.WriteLine($"\n=== Edit Konten Dokumen: {document.Title} ===");
            // Ambil konten dari dokumen saat ini
            string initialContent = document.SavedContent ?? string.Empty;
            Console.WriteLine("Masukkan konten baru (bisa dimodifikasi dari konten sebelumnya):");
            // Gunakan hanya metode interaktif
            string newContent = ReadAndEditMultilineText(initialContent);

            if (newContent == initialContent)
            {
                Console.WriteLine("Tidak ada perubahan pada konten.");
                return;
            }

            // Simpan draft langsung ke document.Content
            document.SavedContent = newContent;
            document.UpdateAt = DateTime.Now;
            _documentService.Update(document.Id, document);
            Console.WriteLine("Konten dokumen berhasil diperbarui!");
            Console.WriteLine("Catatan: Versi baru belum dibuat. Untuk membuat versi baru, silakan pilih menu 'Kirim Versi Baru' di menu Manajemen Versi Dokumen.");
            Console.WriteLine("Draft tersimpan dan dapat dilihat oleh semua anggota workspace.");
        }

        private bool DeleteDocument(Guid documentId)
        {
            Console.WriteLine("\n=== Hapus Dokumen ===");
            Console.Write("Apakah Anda yakin ingin menghapus dokumen ini? (y/n): ");
            string? confirmation = Console.ReadLine()?.ToLower();

            if (confirmation == "y")
            {
                _documentService.Delete(documentId);
                Console.WriteLine("Dokumen berhasil dihapus.");
                return true;
            }
            else
            {
                Console.WriteLine("Penghapusan dokumen dibatalkan.");
                return false;
            }
        }

        private void EditWorkspace()
        {
            if (_currentWorkspace == null)
            {
                Console.WriteLine("Tidak ada workspace yang dipilih.");
                return;
            }

            Console.WriteLine($"\n=== Edit Workspace: {_currentWorkspace.Title} ===");

            Console.Write($"Judul Baru (kosongkan untuk tetap '{_currentWorkspace.Title}'): ");
            string? title = Console.ReadLine();

            Console.Write($"Deskripsi Baru (kosongkan untuk tetap '{(string.IsNullOrEmpty(_currentWorkspace.Description) ? "Tidak ada deskripsi" : _currentWorkspace.Description)}'): ");
            string? description = Console.ReadLine();

            if (!string.IsNullOrEmpty(title))
            {
                _currentWorkspace.Title = title;
            }

            if (!string.IsNullOrEmpty(description))
            {
                _currentWorkspace.Description = description;
            }

            _currentWorkspace.UpdateAt = DateTime.Now;

            _workspaceService.Update(_currentWorkspace.Id, _currentWorkspace);
            Console.WriteLine("Workspace berhasil diperbarui!");
        }

        private bool DeleteWorkspace()
        {
            if (_currentWorkspace == null)
            {
                Console.WriteLine("Tidak ada workspace yang dipilih.");
                return false;
            }

            Console.WriteLine("\n=== Hapus Workspace ===");
            Console.Write("Apakah Anda yakin ingin menghapus workspace ini dan semua dokumen di dalamnya? (y/n): ");
            string? confirmation = Console.ReadLine()?.ToLower();

            if (confirmation == "y")
            {
                _workspaceService.Delete(_currentWorkspace.Id);
                Console.WriteLine("Workspace berhasil dihapus.");
                _currentWorkspace = null; // Clear current workspace after deletion
                return true;
            }
            else
            {
                Console.WriteLine("Penghapusan workspace dibatalkan.");
                return false;
            }
        }

        private void ManageDocumentVersions(Document document)
        {
            if (document == null)
            {
                Console.WriteLine("Tidak ada dokumen yang dipilih.");
                return;
            }

            bool backToDocumentMenu = false;

            while (!backToDocumentMenu)
            {
                Console.WriteLine($"\n=== Manajemen Versi Dokumen: {document.Title} ===");
                Console.WriteLine("1. Lihat Semua Versi");
                Console.WriteLine("2. Kirim Versi Baru");
                Console.WriteLine("3. Rollback ke Versi Sebelumnya");
                Console.WriteLine("0. Kembali ke Menu Dokumen");

                Console.Write("Pilih menu: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ViewDocumentVersions(document.Id);
                        break;
                    case "2":
                        CreateNewDocumentVersion(document);
                        break;
                    case "3":
                        RollbackDocumentVersion(document);
                        break;
                    case "0":
                        backToDocumentMenu = true;
                        break;
                    default:
                        Console.WriteLine("Menu tidak valid. Silakan coba lagi.");
                        break;
                }

                if (!backToDocumentMenu)
                {
                    Console.WriteLine("\nTekan tombol apa saja untuk melanjutkan...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }

        // Method untuk membuat versi baru dokumen tanpa mengedit konten
        private void CreateNewDocumentVersion(Document document)
        {
            if (document == null)
            {
                Console.WriteLine("Tidak ada dokumen yang dipilih.");
                return;
            }

            Console.WriteLine($"\n=== Kirim Versi Baru Dokumen: {document.Title} ===");

            // Periksa apakah versi sebelumnya sudah direview
            if (!_documentBodyService.CanCreateNewVersion(document.Id))
            {
                Console.WriteLine("Tidak dapat membuat versi baru karena masih ada versi yang belum direview oleh dosen.");
                Console.WriteLine("Harap tunggu hingga versi sebelumnya direview sebelum membuat versi baru.");
                return;
            }

            // Gunakan konten dari dokumen saat ini (yang mungkin berisi draft)
            string currentContent = document.SavedContent ?? string.Empty;
            Console.WriteLine(currentContent);

            if (string.IsNullOrWhiteSpace(currentContent))
            {
                Console.WriteLine("Tidak dapat membuat versi baru karena konten kosong.");
                return;
            }

            // Periksa apakah sudah ada versi sebelumnya dengan konten yang sama
            var versions = _documentBodyService.GetDocumentBodiesByDocumentId(document.Id);
            if (versions != null && versions.Any())
            {
                var latestVersion = versions.FirstOrDefault();
                if (latestVersion != null && latestVersion.Content == currentContent)
                {
                    Console.WriteLine("Tidak dapat mengirim versi baru karena konten sama dengan versi sebelumnya.");
                    Console.WriteLine("Silakan ubah konten dokumen terlebih dahulu.");
                    return;
                }
            }

            Console.Write("\nDeskripsi versi baru (seperti commit message): ");
            string comment = Console.ReadLine() ?? "New version";

            var newVersion = _documentBodyService.CreateDocumentBody(document.Id, _currentUser.Id, comment, currentContent);
            document.UpdateAt = DateTime.Now;
            _documentService.Update(document.Id, document);

            // Create an initial Pending review state for the new version
            if (_currentUser == null)
            {
                Console.WriteLine("User tidak ditemukan.");
                return;
            }
            var initialReview = new Review
            {
                Id = Guid.NewGuid(),
                FK_DocumentBodyId = newVersion.Id,
                FK_UserLecturerId = Guid.Empty, // No lecturer assigned yet, or you can assign a default/null
                Comment = "Dokumen baru diajukan untuk review.",
                Status = ReviewStatus.Pending, // Initial status
                State = new SubmittedState(), // Set the initial state
                CreatedAt = DateTime.Now
            };

            _reviewUtil.AddReviewRequest(initialReview); // Add to ReviewUtil's list

            Console.WriteLine("Versi baru dokumen berhasil dikirim!");
            Console.WriteLine("Versi ini perlu direview oleh dosen sebelum Anda dapat membuat versi baru lagi.");
        }

        // Method untuk melihat semua versi dokumen
        private void ViewDocumentVersions(Guid documentId)
        {
            Console.WriteLine("\n=== Daftar Versi Dokumen ===");

            var versions = _documentBodyService.GetDocumentBodiesByDocumentId(documentId);

            if (versions == null || !versions.Any())
            {
                Console.WriteLine("Belum ada versi dokumen.");
                return;
            }

            int index = 1;
            var versionsList = versions.ToList();
            foreach (var version in versionsList)
            {
                string reviewStatus = "";
                var review = _reviewService.GetReviewByDocumentBodyId(version.Id); // Attempt to get review

                if (version.IsReviewed && review != null)
                {
                    // Check the state of the review using the Review object's State property
                    if (review.State is ApprovedState)
                    {
                        reviewStatus = "[DISETUJUI]";
                    }
                    else if (review.State is NeedsRevisionState)
                    {
                        reviewStatus = "[PERLU REVISI]";
                    }
                    else if (review.State is DoneState)
                    {
                        reviewStatus = "[SELESAI]";
                    }
                    else if (review.State is SubmittedState)
                    {
                        reviewStatus = "[DIAJUKAN]";
                    }
                    else if (review.State is UnderReviewState)
                    {
                        reviewStatus = "[DALAM REVIEW]";
                    }
                    else
                    {
                        reviewStatus = $"[{review.State?.GetType().Name.Replace("State", "").ToUpper() ?? "TIDAK DIKETAHUI"}]";
                    }
                }
                else
                {
                    reviewStatus = "[BELUM DIREVIEW]";
                }

                var creator = _userService.GetById(version.FK_UserCreatorId);

                Console.WriteLine($"{index}. Versi dari {version.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss")} {reviewStatus}");
                if (version.IsCurrentVersion)
                {
                    Console.WriteLine($"   {("[AKTIF]")}");
                }
                Console.WriteLine($"   Nama pengirim: {creator.Name}");
                Console.WriteLine($"   Deskripsi: {version.Comment}");

                // Tampilkan preview konten (maksimal 50 karakter)
                string contentPreview = version.Content.Length > 50
                    ? version.Content.Substring(0, 50) + "..."
                    : version.Content;
                Console.WriteLine($"   Preview: {contentPreview}");
                Console.WriteLine();
                index++;
            }

            Console.Write("Pilih versi untuk melihat detail (nomor) atau 0 untuk kembali: ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= versionsList.Count)
            {
                var selectedVersion = versionsList[choice - 1];
                ViewVersionDetailWithReview(selectedVersion);
            }
        }

        // Method untuk melihat detail versi dengan review (jika ada)
        private void ViewVersionDetailWithReview(DocumentBody version)
        {
            if (version == null)
            {
                Console.WriteLine("Versi tidak ditemukan.");
                return;
            }

            var creator = _userService.GetById(version.FK_UserCreatorId);

            Console.WriteLine($"\n=== Detail Versi {version.Id} ===");
            Console.WriteLine($"Nama pembuat: {creator.Name}");
            Console.WriteLine($"Dibuat pada: {version.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss")}");
            Console.WriteLine($"Status: {(version.IsCurrentVersion ? "Aktif" : "Tidak Aktif")}");
            Console.WriteLine($"Deskripsi: {version.Comment}");
            Console.WriteLine("\nKonten:");
            Console.WriteLine(version.Content);

            var review = _reviewService.GetReviewByDocumentBodyId(version.Id);


            if (version.IsReviewed && review != null)
            {
                Console.WriteLine("\n=== Hasil Review ===");

                string statusReview = "";
                // Use the Review's State for display
                if (review.State is ApprovedState)
                {
                    statusReview = "DISETUJUI";
                }
                else if (review.State is NeedsRevisionState)
                {
                    statusReview = "PERLU REVISI";
                }
                else if (review.State is DoneState)
                {
                    statusReview = "SELESAI";
                }
                else if (review.State is SubmittedState)
                {
                    statusReview = "DIAJUKAN";
                }
                else if (review.State is UnderReviewState)
                {
                    statusReview = "DALAM REVIEW";
                }
                else
                {
                    statusReview = review.State?.GetType().Name.Replace("State", "").ToUpper() ?? "TIDAK DIKETAHUI";
                }

                Console.WriteLine($"Status: {statusReview}");

                var lecturer = _userService.GetById(review.FK_UserLecturerId);
                if (lecturer != null)
                {
                    Console.WriteLine($"\nReviewer: {lecturer.Name}");
                }
                else
                {
                    Console.WriteLine("Reviewer: Tidak Diketahui");
                }
                Console.WriteLine($"Tanggal: {review.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss")}");
                Console.WriteLine("Komentar:");
                Console.WriteLine("------------");
                Console.WriteLine(review.Comment);
                Console.WriteLine("------------");
            }
            else
            {
                Console.WriteLine("\nVersi ini belum direview.");
            }
        }

        private void RollbackDocumentVersion(Document document)
        {
            if (document == null)
            {
                Console.WriteLine("Tidak ada dokumen yang dipilih.");
                return;
            }

            Console.WriteLine($"\n=== Rollback Versi Dokumen: {document.Title} ===");

            var versions = _documentBodyService.GetDocumentBodiesByDocumentId(document.Id);

            if (versions == null || !versions.Any())
            {
                Console.WriteLine("Belum ada versi dokumen untuk di-rollback.");
                return;
            }

            // Exclude the current version from the list for rollback
            var rollbackableVersions = versions.Where(v => !v.IsCurrentVersion).ToList();

            if (!rollbackableVersions.Any())
            {
                Console.WriteLine("Tidak ada versi sebelumnya untuk di-rollback.");
                return;
            }

            int index = 1;
            foreach (var version in rollbackableVersions)
            {
                Console.WriteLine($"{index}. Versi dari {version.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss")} (Deskripsi: {version.Comment})");
                index++;
            }

            Console.Write("Pilih versi untuk di-rollback (nomor) atau 0 untuk kembali: ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= rollbackableVersions.Count)
            {
                var selectedVersion = rollbackableVersions[choice - 1];

                // _documentBodyService.SetCurrentVersion(document.Id, selectedVersion.Id); <- doesn't exist in the current context
                document.SavedContent = selectedVersion.Content; // Update document's content to the rolled-back version
                document.UpdateAt = DateTime.Now;
                _documentService.Update(document.Id, document);

                Console.WriteLine($"Dokumen berhasil di-rollback ke versi dari {selectedVersion.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss")}.");
            }
        }

        // Helper untuk input multi-line
        private string ReadAndEditMultilineText(string initialText)
        {
            List<string> lines = new List<string>(initialText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None));
            if (!lines.Any())
            {
                lines.Add("");
            }

            int currentLine = 0;
            int cursorX = 0; // Relative to the start of the current line's text content (after "X: ")
            int startY = Console.CursorTop; // Initial cursor position before drawing editor

            Console.WriteLine("Mode Edit (tekan Esc untuk selesai):");
            Console.WriteLine("-----------------------------------");

            // Initial draw
            for (int i = 0; i < lines.Count; i++)
            {
                Console.WriteLine($"{i + 1}: {lines[i]}");
            }

            // Set initial cursor position
            Console.SetCursorPosition(3 + lines[currentLine].Length, startY + currentLine + 2); // 3 for "X: " prefix, +2 for header lines

            bool editing = true;
            while (editing)
            {
                ConsoleKeyInfo key = Console.ReadKey(intercept: true); // intercept: true hides the key press

                switch (key.Key)
                {
                    case ConsoleKey.Escape:
                        editing = false;
                        break;
                    case ConsoleKey.Enter:
                        // If at the end of the current line, add a new line
                        if (cursorX - 3 == lines[currentLine].Length)
                        {
                            lines.Insert(currentLine + 1, "");
                        }
                        currentLine++;
                        if (currentLine >= lines.Count)
                        {
                            currentLine = lines.Count - 1; // Stay on the last line if Enter pressed at end
                        }
                        // Redraw all lines from current down to show new line or moved cursor
                        Console.SetCursorPosition(0, startY + 2);
                        for (int i = 0; i < lines.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}: {lines[i]}{new string(' ', Console.WindowWidth - (lines[i].Length + (i + 1).ToString().Length + 2))}"); // Clear rest of line
                        }
                        cursorX = 3 + lines[currentLine].Length; // Move cursor to end of new line
                        Console.SetCursorPosition(cursorX, startY + currentLine + 2);
                        break;
                    case ConsoleKey.UpArrow:
                        if (currentLine > 0)
                        {
                            currentLine--;
                            cursorX = Math.Min(3 + lines[currentLine].Length, cursorX);
                            Console.SetCursorPosition(cursorX, startY + currentLine + 2);
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        if (currentLine < lines.Count - 1)
                        {
                            currentLine++;
                            cursorX = Math.Min(3 + lines[currentLine].Length, cursorX);
                            Console.SetCursorPosition(cursorX, startY + currentLine + 2);
                        }
                        break;
                    case ConsoleKey.LeftArrow:
                        if (cursorX > 3) // Don't go past the "X: " prefix
                        {
                            cursorX--;
                            Console.SetCursorPosition(cursorX, Console.CursorTop);
                        }
                        else if (currentLine > 0) // Move to end of previous line
                        {
                            currentLine--;
                            cursorX = 3 + lines[currentLine].Length;
                            Console.SetCursorPosition(cursorX, startY + currentLine + 2);
                        }
                        break;
                    case ConsoleKey.RightArrow:
                        if (cursorX < 3 + lines[currentLine].Length)
                        {
                            cursorX++;
                            Console.SetCursorPosition(cursorX, Console.CursorTop);
                        }
                        else if (currentLine < lines.Count - 1) // Move to beginning of next line
                        {
                            currentLine++;
                            cursorX = 3;
                            Console.SetCursorPosition(cursorX, startY + currentLine + 2);
                        }
                        break;
                    case ConsoleKey.Backspace:
                        if (cursorX > 3)
                        {
                            int pos = cursorX - 3;
                            string line = lines[currentLine];
                            lines[currentLine] = line.Remove(pos - 1, 1);
                            cursorX--;

                            Console.SetCursorPosition(0, startY + currentLine + 2);
                            Console.Write(new string(' ', Console.WindowWidth)); // Clear the line
                            Console.SetCursorPosition(0, startY + currentLine + 2);
                            Console.Write($"{currentLine + 1}: {lines[currentLine]}");
                            Console.SetCursorPosition(cursorX, startY + currentLine + 2);
                        }
                        else if (currentLine > 0)
                        {
                            // Merge with previous line
                            string currentLineContent = lines[currentLine];
                            lines.RemoveAt(currentLine);
                            currentLine--;
                            lines[currentLine] += currentLineContent;

                            // Redraw from the merged line onwards
                            Console.SetCursorPosition(0, startY + currentLine + 2);
                            for (int i = currentLine; i < lines.Count; i++)
                            {
                                Console.WriteLine($"{i + 1}: {lines[i]}{new string(' ', Console.WindowWidth - (lines[i].Length + (i + 1).ToString().Length + 2))}");
                            }
                            Console.WriteLine(new string(' ', Console.WindowWidth)); // Clear the last line if necessary

                            cursorX = 3 + lines[currentLine].Length - currentLineContent.Length; // Position cursor at the start of merged content
                            Console.SetCursorPosition(cursorX, startY + currentLine + 2);
                        }
                        break;
                    case ConsoleKey.Delete:
                        if (cursorX < 3 + lines[currentLine].Length)
                        {
                            int pos = cursorX - 3;
                            string line = lines[currentLine];
                            lines[currentLine] = line.Remove(pos, 1);

                            Console.SetCursorPosition(0, startY + currentLine + 2);
                            Console.Write(new string(' ', Console.WindowWidth)); // Clear the line
                            Console.SetCursorPosition(0, startY + currentLine + 2);
                            Console.Write($"{currentLine + 1}: {lines[currentLine]}");
                            Console.SetCursorPosition(cursorX, startY + currentLine + 2);
                        }
                        else if (currentLine < lines.Count - 1)
                        {
                            // Merge with next line
                            string nextLineContent = lines[currentLine + 1];
                            lines.RemoveAt(currentLine + 1);
                            lines[currentLine] += nextLineContent;

                            // Redraw from the current line onwards
                            Console.SetCursorPosition(0, startY + currentLine + 2);
                            for (int i = currentLine; i < lines.Count; i++)
                            {
                                Console.WriteLine($"{i + 1}: {lines[i]}{new string(' ', Console.WindowWidth - (lines[i].Length + (i + 1).ToString().Length + 2))}");
                            }
                            Console.WriteLine(new string(' ', Console.WindowWidth)); // Clear the last line if necessary

                            Console.SetCursorPosition(cursorX, startY + currentLine + 2);
                        }
                        break;
                    case ConsoleKey.Home:
                        cursorX = 3;
                        Console.SetCursorPosition(cursorX, Console.CursorTop);
                        break;
                    case ConsoleKey.End:
                        cursorX = 3 + lines[currentLine].Length;
                        Console.SetCursorPosition(cursorX, Console.CursorTop);
                        break;
                    default:
                        // For regular key presses, insert the character at the cursor position
                        if (char.IsLetterOrDigit(key.KeyChar) || char.IsPunctuation(key.KeyChar) || key.KeyChar == ' ')
                        {
                            int pos = cursorX - 3;  // Adjust for line number prefix
                            string line = lines[currentLine];

                            // Insert the character
                            if (pos >= 0 && pos <= line.Length)
                            {
                                lines[currentLine] = line.Insert(pos, key.KeyChar.ToString());

                                // Redraw the current line
                                Console.SetCursorPosition(0, Console.CursorTop);
                                Console.Write(new string(' ', Console.WindowWidth));  // Clear the line
                                Console.SetCursorPosition(0, Console.CursorTop);
                                Console.Write($"{currentLine + 1}: {lines[currentLine]}");

                                // Move cursor forward one position
                                cursorX++;
                                Console.SetCursorPosition(cursorX, Console.CursorTop);
                            }
                        }
                        break;
                }
            }
            Console.Clear();  // Clear the screen after editing is done
            return string.Join(Environment.NewLine, lines);
        }
    }
}