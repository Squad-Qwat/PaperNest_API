using API.Services;
using API.Models;
using API.Helpers.Enums;
using View.Global;
using API.Helpers.ExtraClass;
using API.StateMachineAndUtils;

namespace View.Lecturer
{
    public class LecturerView(User currentUser, AuthStateMachine authState)
    {
        private readonly UserService _userService = new(); // Setara dengan 'new UserService()'
        private readonly WorkspaceService _workspaceService = new(); // Setara dengan 'new WorkspaceService()'
        private readonly DocumentService _documentService = new(); // Setara dengan 'new DocumentService()'
        private readonly DocumentBodyService _documentBodyService = new(); // Setara dengan 'new DocumentBodyService()'
        private readonly ReviewService _reviewService = new(); // Setara dengan 'new ReviewService()'
        private readonly AuthStateMachine _authState = authState;
        private User? _currentUser = currentUser;
        private Workspace? _currentWorkspace = null;
        private readonly GlobalView _globalView = new(); // Setara dengan 'new GlobalView()'
        private readonly UserWorkspaceService _userWorkspaceService = new(); // Setara dengan 'new UserWorkspaceService()'
        // Add ReviewUtil
        private readonly ReviewUtil _reviewUtil = new(); // Setara dengan 'new ReviewUtil()'

        /*
         * Setara dengan:
         * public LecturerView(User currentUser, AuthStateMachine authState)
         * {
         *    _userService = new UserService();
         *    _workspaceService = new WorkspaceService();
         *    _documentService = new DocumentService();
         *    _documentBodyService = new DocumentBodyService();
         *    _reviewService = new ReviewService();
         *    _authState = authState;
         *    _currentUser = currentUser;
         *    _currentWorkspace = null;
         *    _globalView = new GlobalView();
         *    _userWorkspaceService = new UserWorkspaceService();
         *    _reviewUtil = new ReviewUtil(); <- Initialize ReviewUtil
         * }
         */

        public void Start()
        {
            bool isRunning = true;

            Console.WriteLine("=== PaperNest - Sistem Manajemen Karya Tulis Ilmiah ===");
            Console.WriteLine("=== Panel Dosen ===");

            do
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
            } while (isRunning);
        }

        private void DisplayMainMenu()
        {
            Console.WriteLine($"\n=== Menu Utama - Selamat datang, {_currentUser?.Name} ===");
            Console.WriteLine("1. Lihat Workspace");
            Console.WriteLine("2. Bergabung dengan Workspace");
            Console.WriteLine("8. Lihat Profil");
            Console.WriteLine("9. Logout");
            Console.Write("Pilih menu: ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ViewWorkspaces();
                    break;
                case "2":
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

        private void ViewWorkspaces()
        {
            if (_currentUser == null)
            {
                Console.WriteLine("Anda harus login terlebih dahulu.");
                return;
            }

            Console.WriteLine("\n=== Daftar Workspace ===");

            // tampilkan workspace yang diikuti
            var results = _workspaceService.GetByUserId(_currentUser.Id);

            if (results == null)
            {
                Console.WriteLine("Gagal mendapatkan daftar workspace.");
                return;
            }

            var workspaces = results;

            if (workspaces == null || !workspaces.Any())
            {
                Console.WriteLine("Anda belum bergabung dengan workspace manapun.");
                Console.WriteLine("Gunakan menu 'Bergabung dengan Workspace' untuk bergabung.");
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

        private void JoinWorkspace()
        {
            if (_currentUser == null)
            {
                Console.WriteLine("Anda harus login terlebih dahulu.");
                return;
            }

            Console.WriteLine("\n=== Bergabung dengan Workspace ===");
            Console.WriteLine("Masukkan ID Workspace untuk bergabung.");
            Console.WriteLine("Anda dapat meminta ID Workspace dari mahasiswa pengelola workspace.");

            Console.Write("\nID Workspace: ");
            string? workspaceIdStr = Console.ReadLine();

            if (string.IsNullOrEmpty(workspaceIdStr) || !Guid.TryParse(workspaceIdStr, out Guid workspaceId))
            {
                Console.WriteLine("ID Workspace tidak valid. Pastikan format ID benar.");
                return;
            }

            // Cek apakah workspace ada
            var workspace = _workspaceService.GetById(workspaceId);

            if (workspace == null)
            {
                Console.WriteLine("Workspace tidak ditemukan. Silakan periksa ID workspace.");
                return;
            }

            Console.WriteLine($"Workspace ditemukan: {workspace.Title}");
            Console.WriteLine($"Deskripsi: {workspace.Description ?? "Tidak ada deskripsi"}");

            Console.Write("\nApakah Anda yakin ingin bergabung dengan workspace ini? (y/n): ");
            string? confirmation = Console.ReadLine()?.ToLower();

            if (confirmation == "y")
            {
                // Bergabung dengan workspace sebagai dosen (Lecturer)
                _userWorkspaceService.AddUserWorkspaceAsLecturer(_currentUser.Id, workspaceId);
                Console.WriteLine("\nBerhasil bergabung dengan workspace!");
                Console.WriteLine("Anda sekarang dapat melihat dokumen dan memberikan review pada workspace ini.");
            }
            else
            {
                Console.WriteLine("\nBergabung dengan workspace dibatalkan.");
            }
        }

        private void WorkspaceMenu()
        {
            if (_currentWorkspace == null)
            {
                Console.WriteLine("Tidak ada workspace yang dipilih.");
                return;
            }

            if(_currentUser == null)
            {
                Console.WriteLine("Anda harus login terlebih dahulu.");
                return;
            }

            bool backToMainMenu = false;

            // Cek role pengguna saat ini di workspace ini
            WorkspaceRole userRole = _workspaceService.GetUserRoleInWorkspace(_currentUser.Id, _currentWorkspace.Id);
            

            do
            {
                Console.WriteLine($"\n=== Workspace: {_currentWorkspace?.Title} ===");
                Console.WriteLine($"ID: {_currentWorkspace?.Id}");
                Console.WriteLine($"Role Anda: {userRole}");

                // Menu untuk dosen - melihat dan mereview dokumen
                Console.WriteLine("1. Lihat Dokumen");

                Console.WriteLine("0. Kembali ke Menu Utama");

                Console.Write("Pilih menu: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ViewDocuments();
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
            } while (!backToMainMenu);
        }

        private void ViewDocuments()
        {
            if (_currentWorkspace == null)
            {
                Console.WriteLine("Tidak ada workspace yang dipilih.");
                return;
            }

            Console.WriteLine($"\n=== Dokumen di Workspace: {_currentWorkspace.Title} ===");

            var result = _documentService.GetByWorkspaceId(_currentWorkspace.Id);

            if (result == null)
            {
                Console.WriteLine("Gagal mendapatkan daftar dokumen.");
                return;
            }

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
                Console.WriteLine($"   Dibuat pada: {document.CreatedAt:dd/MM/yyyy HH:mm:ss}"); // Setara dengan $"{document.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss")}"
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

        private void DocumentMenu(Document document)
        {
            if (document == null)
            {
                Console.WriteLine("Tidak ada dokumen yang dipilih.");
                return;
            }

            bool backToWorkspaceMenu = false;

            do
            {
                string reviewInfo;
                var versions = _documentBodyService.GetDocumentBodiesByDocumentId(document.Id);
                if(versions == null)
                {
                    Console.WriteLine("Gagal mendapatkan versi dokumen.");
                    return;
                }

                if (!versions.Any())
                {
                    Console.WriteLine("Belum ada versi dokumen yang dibuat.");
                    Console.WriteLine("Silakan buat versi baru untuk dokumen ini.");
                    backToWorkspaceMenu = true; // Kembali ke menu workspace jika tidak ada versi
                    continue;
                }

                var currentVersion = _documentBodyService.GetCurrentVersion(document.Id);
                if (currentVersion == null)
                {
                    Console.WriteLine("Tidak ada versi dokumen yang aktif saat ini.");
                    backToWorkspaceMenu = true; // Kembali ke menu workspace jika tidak ada versi
                    continue;
                }

                if (currentVersion.IsReviewed && currentVersion.IsCurrentVersion)
                {
                    reviewInfo = "\nVersi saat ini sudah direview";
                }
                else if (!currentVersion.IsReviewed)
                {
                    reviewInfo = "\nVersi saat ini belum direview";
                }

                var review = _reviewService.GetReviewByDocumentBodyId(currentVersion.Id);
                string statusText;
                if (review == null)
                {
                    Console.WriteLine("Tidak ada review untuk versi dokumen ini.");
                    return;
                }

                // Using the Review's State instead of direct ReviewStatus
                if (review?.State is ApprovedState)
                {
                    statusText = "DISETUJUI";
                }
                else if (review?.State is NeedsRevisionState)
                {
                    statusText = "PERLU REVISI";
                }
                else if (review?.State is DoneState)
                {
                    statusText = "SELESAI";
                }
                else
                {
                    statusText = review?.State?.GetType().Name.Replace("State", "").ToUpper() ?? "TIDAK DIKETAHUI";
                }
                reviewInfo = $"\nVersi saat ini: [{statusText}] (Klik menu 'Lihat Versi Dokumen' untuk detail)";

                Console.WriteLine($"\n=== Dokumen: {document.Title} ===");
                Console.WriteLine($"Konten: {document.SavedContent ?? "Tidak ada konten"}");
                Console.WriteLine($"Dibuat pada: {document.CreatedAt:dd/MM/yyyy HH:mm:ss}"); // Setara dengan $"{document.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss")}"

                Console.WriteLine(reviewInfo);

                Console.WriteLine("\n1. Lihat Versi Dokumen");
                Console.WriteLine("2. Review Dokumen");
                Console.WriteLine("0. Kembali ke Menu Workspace");

                Console.Write("Pilih menu: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ViewDocumentVersions(document.Id);
                        break;
                    case "2":
                        ReviewDocumentVersions(document.Id);
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
            } while (!backToWorkspaceMenu);
        }

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
                string reviewStatus;
                var review = _reviewService.GetReviewByDocumentBodyId(version.Id); // Attempt to get review

                if (!version.IsReviewed)
                {
                    reviewStatus = "[BELUM DIREVIEW]";
                }

                if (review == null) 
                {
                    reviewStatus = "[PERLU REVIEW]"; // If no review exists, set status to "PERLU REVIEW"
                }


                // Check the state of the review using the Review object's State property
                if (review?.State is ApprovedState)
                {
                    reviewStatus = "[DISETUJUI]";
                }
                else if (review?.State is NeedsRevisionState)
                {
                    reviewStatus = "[PERLU REVISI]";
                }
                else if (review?.State is DoneState)
                {
                    reviewStatus = "[SELESAI]";
                }
                else
                {
                    reviewStatus = $"[{review?.State?.GetType().Name.Replace("State", "").ToUpper() ?? "TIDAK DIKETAHUI"}]";
                }

                var creator = _userService.GetById(version.FK_UserCreatorId);

                Console.WriteLine($"{index}. Versi dari {version.CreatedAt:dd/MM/yyyy HH:mm:ss} {reviewStatus}"); // Setara dengan $"{index}. Versi dari {version.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss")} [{reviewStatus}]"
                if (version.IsCurrentVersion)
                {
                    Console.WriteLine($"   {("[AKTIF]")}");
                }
                Console.WriteLine($"   Nama pengirim: {creator?.Name}");
                Console.WriteLine($"   Deskripsi: {version.Comment}");

                string? contentPreview = version.Content?.Length > 50
                    ? string.Concat(version.Content.AsSpan(0, 50), "...") // Setara dengan version.Content[..50] + "..." dan version.Content.Substring(0, 50) + "..."
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

        private void ViewVersionDetailWithReview(DocumentBody version)
        {
            if (version == null)
            {
                Console.WriteLine("Versi tidak ditemukan.");
                return;
            }

            var creator = _userService.GetById(version.FK_UserCreatorId);

            Console.WriteLine($"\n=== Detail Versi {version.Id} ===");
            Console.WriteLine($"Nama pembuat: {creator?.Name}");
            Console.WriteLine($"Dibuat pada: {version.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss")}");
            Console.WriteLine($"Status: {(version.IsCurrentVersion ? "Aktif" : "Tidak Aktif")}");
            Console.WriteLine($"Deskripsi: {version.Comment}");
            Console.WriteLine("\nKonten:");
            Console.WriteLine(version.Content);

            var review = _reviewService.GetReviewByDocumentBodyId(version.Id);


            if (!version.IsReviewed)
            {
                Console.WriteLine("\nVersi ini belum direview.");

                Console.Write("\nApakah Anda ingin mereview versi ini? (y/n): ");
                string? choice = Console.ReadLine()?.ToLower();
                if (choice == "y")
                {
                    ReviewVersion(version);
                }
            }

            Console.WriteLine("\n=== Hasil Review ===");

            string statusReview;
            // Use the Review's State for display
            if (review?.State is ApprovedState)
            {
                statusReview = "DISETUJUI";
            }
            else if (review?.State is NeedsRevisionState)
            {
                statusReview = "PERLU REVISI";
            }
            else if (review?.State is DoneState)
            {
                statusReview = "SELESAI";
            }
            else
            {
                statusReview = review?.State?.GetType().Name.Replace("State", "").ToUpper() ?? "TIDAK DIKETAHUI";
            }

            Console.WriteLine($"Status: {statusReview}");

            if (review == null)
            {
                Console.WriteLine("Tidak ada review untuk versi ini.");
                return;
            }

            var lecturer = _userService.GetById(review.FK_UserLecturerId);
            if (lecturer != null)
            {
                Console.WriteLine($"\nReviewer: {lecturer.Name}");
            }
            else
            {
                Console.WriteLine("Reviewer: Tidak Diketahui");
            }
            Console.WriteLine($"Tanggal: {review.CreatedAt:dd/MM/yyyy HH:mm:ss}"); // Setara dengan $"{review.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss")}"
            Console.WriteLine("Komentar:");
            Console.WriteLine("------------");
            Console.WriteLine(review.Comment);
            Console.WriteLine("------------");
        }

        private void ReviewDocumentVersions(Guid documentId)
        {
            Console.WriteLine("\n=== Review Versi Dokumen ===");

            var versions = _documentBodyService.GetDocumentBodiesByDocumentId(documentId);

            if (versions == null || !versions.Any())
            {
                Console.WriteLine("Belum ada versi dokumen yang perlu di-review.");
                return;
            }

            int index = 1;
            var versionsList = versions.ToList();
            foreach (var version in versionsList)
            {
                string reviewStatus = "";
                Review? review = null;
                try
                {
                    review = _reviewService.GetReviewByDocumentBodyId(version.Id);
                }
                catch (InvalidOperationException)
                {
                    reviewStatus = "[PERLU REVIEW]";
                }

                if (!version.IsReviewed && review == null)
                {
                    reviewStatus = "[PERLU REVIEW]";
                }

                // Use the Review's State for display
                if (review?.State is ApprovedState)
                {
                    reviewStatus = "[DISETUJUI]";
                }
                else if (review?.State is NeedsRevisionState)
                {
                    reviewStatus = "[PERLU REVISI]";
                }
                else if (review?.State is DoneState)
                {
                    reviewStatus = "[SELESAI]";
                }
                else
                {
                    reviewStatus = $"[{review?.State?.GetType().Name.Replace("State", "").ToUpper() ?? "TIDAK DIKETAHUI"}]";
                }

                Console.WriteLine($"{index}. Versi dari {version.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss")} {reviewStatus}");
                Console.WriteLine($"   {(version.IsCurrentVersion ? "[AKTIF]" : "")}");
                Console.WriteLine($"   Deskripsi: {version.Comment}");
                string? contentPreview = version.Content?.Length > 50
                    ? version.Content.Substring(0, 50) + "..."
                    : version.Content;
                Console.WriteLine($"   Preview: {contentPreview}");
                Console.WriteLine();
                index++;
            }

            Console.Write("Pilih versi untuk review (nomor) atau 0 untuk kembali: ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= versionsList.Count)
            {
                var selectedVersion = versionsList.ElementAt(choice - 1);
                ReviewVersion(selectedVersion);
            }
        }

        private void ReviewVersion(DocumentBody version)
        {
            string? choice;

            if (version == null)
            {
                Console.WriteLine("Versi tidak ditemukan.");
                return;
            }

            Console.WriteLine($"\n=== Review Versi {version.Id} ===");
            Console.WriteLine($"Dibuat pada: {version.CreatedAt:dd/MM/yyyy HH:mm:ss}"); // Setara dengan $"{version.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss")}"
            Console.WriteLine($"Status: {(version.IsCurrentVersion ? "Aktif" : "Tidak Aktif")}");
            Console.WriteLine($"Deskripsi: {version.Comment}");
            Console.WriteLine("\nKonten:");
            Console.WriteLine(version.Content);

            Review? existingReview = null; // Renamed to avoid conflict
            bool hasReview;

            try
            {
                existingReview = _reviewService.GetReviewByDocumentBodyId(version.Id);
                hasReview = (existingReview != null);
            }
            catch (InvalidOperationException)
            {
                hasReview = false;
            }

            if (!version.IsReviewed || !hasReview)
            {
                Console.WriteLine("\nVersi ini belum direview. Anda dapat membuat review baru.");
                return;
            }

            Console.WriteLine("\nDokumen ini sudah direview sebelumnya.");
            // Display the current state of the existing review
            string currentReviewStatus;
            if (existingReview == null)
            {
                Console.WriteLine("Tidak ada review untuk saat ini!");
                return;
            }

            if (existingReview.State is ApprovedState)
            {
                currentReviewStatus = "DISETUJUI";
            }
            else if (existingReview.State is NeedsRevisionState)
            {
                currentReviewStatus = "PERLU REVISI";
            }
            else if (existingReview.State is DoneState)
            {
                currentReviewStatus = "SELESAI";
            }
            else
            {
                currentReviewStatus = existingReview.State?.GetType().Name.Replace("State", "").ToUpper() ?? "TIDAK DIKETAHUI";
            }
            Console.WriteLine($"Status review: {currentReviewStatus}");

            Console.Write("\nApakah Anda ingin membuat review baru? (y/n): ");
            choice = Console.ReadLine()?.ToLower();
            if (choice != "y")
            {
                return;
            }

            Console.WriteLine("\n=== Buat Review ===");
            Console.WriteLine("1. Setujui (Approve)");
            Console.WriteLine("2. Perlu Revisi (Needs Revision)");
            Console.WriteLine("3. Selesai (Done)");
            Console.WriteLine("0. Kembali tanpa Review");

            Console.Write("Pilihan: ");
            string? reviewChoice = Console.ReadLine();

            if (reviewChoice == "0")
            {
                return;
            }

            // Determine status based on choice
            ReviewStatus status;
            ReviewState newState; // Declare ReviewState variable
            switch (reviewChoice)
            {
                case "1":
                    status = ReviewStatus.Approved;
                    newState = new ApprovedState(); // Instantiate ApprovedState
                    break;
                case "2":
                    status = ReviewStatus.NeedsRevision;
                    newState = new NeedsRevisionState(); // Instantiate NeedsRevisionState
                    break;
                case "3":
                    status = ReviewStatus.Done;
                    newState = new DoneState(); // Instantiate DoneState
                    break;
                default:
                    Console.WriteLine("Pilihan tidak valid.");
                    return;
            }

            Console.Write("Masukkan komentar untuk review: ");
            string comment = Console.ReadLine() ?? "";

            try
            {
                if(_currentUser == null)
                {
                    Console.WriteLine("User tidak ditemukan.");
                    return;
                }
                // Create a new Review object
                var newReview = new Review
                {
                    Id = Guid.NewGuid(),
                    FK_DocumentBodyId = version.Id,
                    FK_UserLecturerId = _currentUser.Id,
                    Comment = comment,
                    Status = status, // Still need ReviewStatus for the Review model
                    State = newState, // Set the initial state
                    CreatedAt = DateTime.Now
                };

                // Add the review to the ReviewService (which would typically save it to a database)
                // _reviewService.AddReview(newReview); <- Will create AddReview now takes a Review object, currently not implemented

                // Add the review to ReviewUtil, due to non-existant Add review in the review service
                _reviewUtil.AddReviewRequest(newReview); // Add the review to ReviewUtil's internal list

                // Process the review using ReviewUtil, which will handle state transitions and document updates
                _reviewUtil.ProcessReview(newReview, status, comment);

                // Update IsReviewed in DocumentBody
                version.IsReviewed = true;
                // _documentBodyService.UpdateDocumentBody(version); <- Assuming an UpdateDocumentBody method

                Console.WriteLine("\nReview berhasil disimpan!");
                Console.WriteLine($"Status review: {status}");

                switch (status)
                {
                    case ReviewStatus.Approved:
                        Console.WriteLine("Dokumen telah disetujui. Mahasiswa dapat melanjutkan versi berikutnya.");
                        break;
                    case ReviewStatus.NeedsRevision:
                        Console.WriteLine("Dokumen perlu revisi. Mahasiswa akan diminta untuk membuat perubahan.");
                        break;
                    case ReviewStatus.Done:
                        Console.WriteLine("Dokumen telah dinyatakan selesai. Proses review telah selesai untuk dokumen ini.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nGagal menyimpan review: {ex.Message}");
            }
        }
    }
}