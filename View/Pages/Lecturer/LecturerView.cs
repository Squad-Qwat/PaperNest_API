using API.Services;
using API.Models;
using API.StateMachines;
using API.Helpers.Enums;
using View.Pages.Global;

namespace View.Pages.Lecturer
{
   public class LecturerView
   {
       private readonly UserService _userService;
       private readonly WorkspaceService _workspaceService;
       private readonly DocumentService _documentService;
       private readonly DocumentBodyService _documentBodyService;
       private readonly ReviewService _reviewService;
       private readonly CitationService _citationService;
       private readonly AuthStateMachine _authState;
       private User? _currentUser;
       private Workspace? _currentWorkspace;
        private readonly GlobalView _globalView;
        private readonly UserWorkspaceService _userWorkspaceService;

        public LecturerView(User currentUser, AuthStateMachine authState)
       {
           _userService = new UserService();
           _workspaceService = new WorkspaceService();
           _documentService = new DocumentService();
           _documentBodyService = new DocumentBodyService();
           _reviewService = new ReviewService();
           _citationService = new CitationService();
           _authState = authState;
           _currentUser = currentUser;
           _currentWorkspace = null;
            _globalView = new GlobalView();
            _userWorkspaceService = new UserWorkspaceService();
        }

       public void Start()
       {
           bool isRunning = true;

           Console.WriteLine("=== PaperNest - Sistem Manajemen Karya Tulis Ilmiah ===");
           Console.WriteLine("=== Panel Dosen ===");

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
    
           if (results != null)
           {
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
           else
           {
               Console.WriteLine("Gagal mendapatkan daftar workspace.");
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
    
           if (workspace != null)
           {
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
           else
           {
               Console.WriteLine("Workspace tidak ditemukan. Silakan periksa ID workspace.");
           }
       }

       private void WorkspaceMenu()
       {
           if (_currentWorkspace == null)
           {
               Console.WriteLine("Tidak ada workspace yang dipilih.");
               return;
           }
    
           bool backToMainMenu = false;
    
           // Cek role pengguna saat ini di workspace ini
           WorkspaceRole userRole = _workspaceService.GetUserRoleInWorkspace(_currentUser.Id, _currentWorkspace.Id);
    
           while (!backToMainMenu)
           {
               Console.WriteLine($"\n=== Workspace: {_currentWorkspace.Title} ===");
               Console.WriteLine($"ID: {_currentWorkspace.Id}");
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
           }
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
                            switch (review.Status)
                            {
                                case ReviewStatus.Approved:
                                    statusText = "DISETUJUI";
                                    break;
                                case ReviewStatus.NeedsRevision:
                                    statusText = "PERLU REVISI";
                                    break;
                                case ReviewStatus.Done:
                                    statusText = "SELESAI";
                                    break;
                                default:
                                    statusText = review.Status.ToString();
                                    break;
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

                Console.WriteLine(reviewInfo);

                Console.WriteLine("\n1. Lihat Versi Dokumen");
                Console.WriteLine("2. Review Dokumen");
                Console.WriteLine("3. Lihat Sitasi Dokumen");
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
                    case "3":
                        ViewDocumentCitations(document);
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
           var versionsList = versions.ToList(); // Konversi ke List untuk akses indeks
           foreach (var version in versionsList)
           {
               string reviewStatus = "";
               // Cek apakah sudah di-review
               var review = _reviewService.GetReviewByDocumentBodyId(version.Id);
               if (version.IsReviewed && review != null)
               {
                   switch (review.Status)
                   {
                       case ReviewStatus.Approved:
                           reviewStatus = "[DISETUJUI]";
                           break;
                       case ReviewStatus.NeedsRevision:
                           reviewStatus = "[PERLU REVISI]";
                           break;
                       case ReviewStatus.Done:
                           reviewStatus = "[SELESAI]";
                           break;
                       default:
                           reviewStatus = $"[{review.Status}]";
                           break;
                   }
               }
               else
               {
                   reviewStatus = "[BELUM DIREVIEW]";
               }

               var creator = _userService.GetById(version.FK_UserCreaotorId);

                Console.WriteLine($"{index}. Versi dari {version.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss")} {reviewStatus}");
               if(version.IsCurrentVersion){
                   Console.WriteLine($"   {"[AKTIF]"}");
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

       private void ViewVersionDetailWithReview(DocumentBody version)
       {
           if (version == null)
           {
               Console.WriteLine("Versi tidak ditemukan.");
               return;
           }

            var creator = _userService.GetById(version.FK_UserCreaotorId);
    
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
               
               // Tampilkan status review dengan format yang mudah dibaca
               string statusReview = "";
               switch (review.Status)
               {
                   case ReviewStatus.Approved:
                       statusReview = "DISETUJUI";
                       break;
                   case ReviewStatus.NeedsRevision:
                       statusReview = "PERLU REVISI";
                       break;
                   case ReviewStatus.Done:
                       statusReview = "SELESAI";
                       break;
                   default:
                       statusReview = review.Status.ToString();
                       break;
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
               
               // Berikan opsi untuk mereview dokumen
               Console.Write("\nApakah Anda ingin mereview versi ini? (y/n): ");
               string? choice = Console.ReadLine()?.ToLower();
               if (choice == "y")
               {
                   ReviewVersion(version);
               }
           }
       }

       // Method untuk mereview versi dokumen
       private void ReviewDocumentVersions(Guid documentId)
       {
           Console.WriteLine("\n=== Review Versi Dokumen ===");
    
           // Dapatkan versi dokumen dari DocumentBodyService
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
               // Cek apakah sudah di-review
               Review? review = null;
               try {
                   review = _reviewService.GetReviewByDocumentBodyId(version.Id);
               } catch (InvalidOperationException) {
                   reviewStatus = "[PERLU REVIEW]";
               }
               
               if (version.IsReviewed && review != null)
               {
                   switch (review.Status)
                   {
                       case ReviewStatus.Approved:
                           reviewStatus = "[DISETUJUI]";
                           break;
                       case ReviewStatus.NeedsRevision:
                           reviewStatus = "[PERLU REVISI]";
                           break;
                       case ReviewStatus.Done:
                           reviewStatus = "[SELESAI]";
                           break;
                       default:
                           reviewStatus = $"[{review.Status}]";
                           break;
                   }
               }
               else
               {
                   reviewStatus = "[PERLU REVIEW]";
               }
               
               Console.WriteLine($"{index}. Versi dari {version.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss")} {reviewStatus}");
               Console.WriteLine($"   {(version.IsCurrentVersion ? "[AKTIF]" : "")}");
               Console.WriteLine($"   Deskripsi: {version.Comment}");
               // Tampilkan preview konten (maksimal 50 karakter)
               string contentPreview = version.Content.Length > 50 
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
           if (version == null)
           {
               Console.WriteLine("Versi tidak ditemukan.");
               return;
           }

           Console.WriteLine($"\n=== Review Versi {version.Id} ===");
           Console.WriteLine($"Dibuat pada: {version.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss")}");
           Console.WriteLine($"Status: {(version.IsCurrentVersion ? "Aktif" : "Tidak Aktif")}");
           Console.WriteLine($"Deskripsi: {version.Comment}");
           Console.WriteLine("\nKonten:");
           Console.WriteLine(version.Content);

           Review? review = null;
           bool hasReview = false;
    
           try {
               review = _reviewService.GetReviewByDocumentBodyId(version.Id);
               hasReview = review != null;
           } catch (InvalidOperationException) {
               // Review tidak ditemukan
               hasReview = false;
           }

           if (version.IsReviewed && hasReview)
           {
               Console.WriteLine("\n=== Review Sudah Diberikan ===");
               
               // Tampilkan status review dengan format yang mudah dibaca
               string statusReview = "";
               switch (review.Status)
               {
                   case ReviewStatus.Approved:
                       statusReview = "DISETUJUI";
                       break;
                   case ReviewStatus.NeedsRevision:
                       statusReview = "PERLU REVISI";
                       break;
                   case ReviewStatus.Done:
                       statusReview = "SELESAI";
                       break;
                   default:
                       statusReview = review.Status.ToString();
                       break;
               }
               
               Console.WriteLine($"Status: {statusReview}");
               
               var lecturer = _userService.GetById(review.FK_UserLecturerId);
               if (lecturer != null)
               {
                   Console.WriteLine($"Reviewer: {lecturer.Name}");
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
               
               Console.WriteLine("\nDokumen ini sudah direview dan tidak dapat diubah lagi.");
               Console.WriteLine("Review yang sudah diberikan bersifat final.");
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

           // Tentukan status berdasarkan pilihan
           ReviewStatus status;
           switch (reviewChoice)
           {
               case "1":
                   status = ReviewStatus.Approved;
                   break;
               case "2":
                   status = ReviewStatus.NeedsRevision;
                   break;
               case "3":
                   status = ReviewStatus.Done;
                   break;
               default:
                   Console.WriteLine("Pilihan tidak valid.");
                   return;
           }

           Console.Write("Masukkan komentar untuk review: ");
           string comment = Console.ReadLine() ?? "";

           // Simpan review
           try {
               _reviewService.AddReview(version.Id, _currentUser.Id, comment, status);
               
               // Update IsReviewed di DocumentBody
               version.IsReviewed = true;
               
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
           catch (Exception ex) {
               Console.WriteLine($"\nGagal menyimpan review: {ex.Message}");
           }
       }

       // Method untuk melihat sitasi dokumen (khusus untuk dosen - read-only)
       private void ViewDocumentCitations(Document document)
       {
           if (document == null)
           {
               Console.WriteLine("Tidak ada dokumen yang dipilih.");
               return;
           }

           bool backToDocumentMenu = false;

           while (!backToDocumentMenu)
           {
               Console.WriteLine($"\n=== Sitasi dalam Dokumen: {document.Title} ===");
               Console.WriteLine("1. Lihat Semua Sitasi");
               Console.WriteLine("2. Lihat Detail Sitasi Tertentu");
               Console.WriteLine("3. Lihat Format APA Sitasi");
               Console.WriteLine("0. Kembali ke Menu Dokumen");
               Console.Write("Pilih menu: ");

               string? choice = Console.ReadLine();

               switch (choice)
               {
                   case "1":
                       ViewAllCitations(document.Id);
                       break;
                   case "2":
                       ViewCitationDetail(document.Id);
                       break;
                   case "3":
                       ViewCitationAPAFormat(document.Id);
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

       // Method untuk melihat semua sitasi dalam dokumen
       private void ViewAllCitations(Guid documentId)
       {
           Console.WriteLine("\n=== Daftar Semua Sitasi dalam Dokumen ===");

           var citations = _citationService.GetCitationsByDocumentId(documentId);

           if (citations == null || !citations.Any())
           {
               Console.WriteLine("Belum ada sitasi untuk dokumen ini.");
               Console.WriteLine("Mahasiswa belum menambahkan referensi apapun ke dokumen ini.");
               return;
           }

           int index = 1;
           foreach (var citation in citations)
           {
               Console.WriteLine($"{index}. {citation.Title} ({GetCitationTypeDisplay(citation.Type)})");
               Console.WriteLine($"   Penulis: {citation.Author}");
               Console.WriteLine($"   Informasi Publikasi: {citation.PublicationInfo}");
               
               if (citation.PublicationDate.HasValue)
               {
                   Console.WriteLine($"   Tanggal Publikasi: {citation.PublicationDate.Value.ToString("dd/MM/yyyy")}");
               }

               if (!string.IsNullOrEmpty(citation.AccessDate))
               {
                   Console.WriteLine($"   Tanggal Akses: {citation.AccessDate}");
               }

               if (!string.IsNullOrEmpty(citation.DOI))
               {
                   Console.WriteLine($"   DOI: {citation.DOI}");
               }

               Console.WriteLine($"   Ditambahkan pada: {citation.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss")}");
               Console.WriteLine();
               index++;
           }

           Console.WriteLine($"\nTotal sitasi: {citations.Count()}");
           Console.WriteLine("\nCatatan: Sebagai dosen, Anda hanya dapat melihat sitasi yang telah dibuat oleh mahasiswa.");
           Console.WriteLine("Untuk memberikan feedback tentang sitasi, gunakan fitur review dokumen.");
       }

       // Method untuk melihat detail sitasi tertentu
       private void ViewCitationDetail(Guid documentId)
       {
           Console.WriteLine("\n=== Detail Sitasi ===");

           var citations = _citationService.GetCitationsByDocumentId(documentId).ToList();

           if (citations == null || !citations.Any())
           {
               Console.WriteLine("Belum ada sitasi untuk dokumen ini.");
               return;
           }

           int index = 1;
           foreach (var citation in citations)
           {
               Console.WriteLine($"{index}. {citation.Title} ({GetCitationTypeDisplay(citation.Type)})");
               Console.WriteLine($"   Penulis: {citation.Author}");
               index++;
           }

           Console.Write("Pilih sitasi untuk melihat detail (nomor) atau 0 untuk kembali: ");
           if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > citations.Count)
           {
               if (choice != 0) Console.WriteLine("Pilihan tidak valid.");
               return;
           }

           var selectedCitation = citations[choice - 1];

           Console.WriteLine($"\n=== Detail Sitasi: {selectedCitation.Title} ===");
           Console.WriteLine($"ID: {selectedCitation.Id}");
           Console.WriteLine($"Tipe: {GetCitationTypeDisplay(selectedCitation.Type)}");
           Console.WriteLine($"Judul: {selectedCitation.Title}");
           Console.WriteLine($"Penulis: {selectedCitation.Author}");
           Console.WriteLine($"Informasi Publikasi: {selectedCitation.PublicationInfo}");

           if (selectedCitation.PublicationDate.HasValue)
           {
               Console.WriteLine($"Tanggal Publikasi: {selectedCitation.PublicationDate.Value.ToString("dd/MM/yyyy")}");
           }
           else
           {
               Console.WriteLine("Tanggal Publikasi: Tidak dicantumkan");
           }

           if (!string.IsNullOrEmpty(selectedCitation.AccessDate))
           {
               Console.WriteLine($"Tanggal Akses: {selectedCitation.AccessDate}");
           }

           if (!string.IsNullOrEmpty(selectedCitation.DOI))
           {
               Console.WriteLine($"DOI: {selectedCitation.DOI}");
           }

           Console.WriteLine($"Ditambahkan pada: {selectedCitation.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss")}");
           Console.WriteLine($"Terakhir diperbarui: {selectedCitation.UpdatedAt.ToString("dd/MM/yyyy HH:mm:ss")}");

           // Tampilkan format APA untuk sitasi ini
           string? apaFormat = _citationService.GetFormattedCitationAPA(selectedCitation.Id);
           if (!string.IsNullOrEmpty(apaFormat))
           {
               Console.WriteLine($"\nFormat APA:");
               Console.WriteLine($"{apaFormat}");
           }
       }

       // Method untuk melihat format APA sitasi
       private void ViewCitationAPAFormat(Guid documentId)
       {
           Console.WriteLine("\n=== Format APA Sitasi ===");

           var citations = _citationService.GetCitationsByDocumentId(documentId).ToList();

           if (citations == null || !citations.Any())
           {
               Console.WriteLine("Belum ada sitasi untuk dokumen ini.");
               return;
           }

           Console.WriteLine("Pilih opsi untuk melihat format APA:");
           Console.WriteLine("1. Lihat semua sitasi dalam format APA");
           Console.WriteLine("2. Lihat format APA sitasi tertentu");
           Console.WriteLine("0. Kembali");
           Console.Write("Pilihan: ");

           string? choice = Console.ReadLine();

           switch (choice)
           {
               case "1":
                   ViewAllCitationsInAPAFormat(citations);
                   break;
               case "2":
                   ViewSpecificCitationAPAFormat(citations);
                   break;
               case "0":
                   return;
               default:
                   Console.WriteLine("Pilihan tidak valid.");
                   break;
           }
       }

       // Method untuk melihat semua sitasi dalam format APA
       private void ViewAllCitationsInAPAFormat(List<Citation> citations)
       {
           Console.WriteLine("\n=== Daftar Referensi (Format APA) ===");
           Console.WriteLine("Berikut adalah semua sitasi dalam dokumen ini menggunakan format APA:\n");

           int index = 1;
           foreach (var citation in citations.OrderBy(c => c.Author))
           {
               string? apaFormat = _citationService.GetFormattedCitationAPA(citation.Id);
               if (!string.IsNullOrEmpty(apaFormat))
               {
                   Console.WriteLine($"{index}. {apaFormat}");
                   Console.WriteLine();
                   index++;
               }
           }

           Console.WriteLine("---");
           Console.WriteLine("Catatan: Daftar di atas sudah diurutkan berdasarkan nama penulis sesuai standar APA.");
           Console.WriteLine("Sebagai dosen, Anda dapat menggunakan ini untuk mengevaluasi kualitas referensi mahasiswa.");
       }

       // Method untuk melihat format APA sitasi tertentu
       private void ViewSpecificCitationAPAFormat(List<Citation> citations)
       {
           Console.WriteLine("\n=== Pilih Sitasi untuk Format APA ===");

           int index = 1;
           foreach (var citation in citations)
           {
               Console.WriteLine($"{index}. {citation.Title} ({GetCitationTypeDisplay(citation.Type)})");
               Console.WriteLine($"   Penulis: {citation.Author}");
               index++;
           }

           Console.Write("Pilih sitasi untuk melihat format APA (nomor) atau 0 untuk kembali: ");
           if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > citations.Count)
           {
               if (choice != 0) Console.WriteLine("Pilihan tidak valid.");
               return;
           }

           var selectedCitation = citations[choice - 1];
           string? apaFormat = _citationService.GetFormattedCitationAPA(selectedCitation.Id);

           Console.WriteLine($"\n=== Format APA untuk: {selectedCitation.Title} ===");
           if (!string.IsNullOrEmpty(apaFormat))
           {
               Console.WriteLine(apaFormat);
               Console.WriteLine("\nCatatan: Format di atas mengikuti standar American Psychological Association (APA) Style 7th Edition.");
           }
           else
           {
               Console.WriteLine("Tidak dapat membuat format APA untuk sitasi ini.");
               Console.WriteLine("Kemungkinan ada informasi yang kurang lengkap dalam sitasi.");
           }
       }

       // Helper method untuk mendapatkan nama tipe sitasi dalam bahasa Indonesia
       private string GetCitationTypeDisplay(CitationType type)
       {
           return type switch
           {
               CitationType.Book => "Buku",
               CitationType.JournalArticle => "Artikel Jurnal",
               CitationType.Website => "Website",
               CitationType.ConferencePaper => "Makalah Konferensi",
               CitationType.Thesis => "Tesis/Disertasi",
               _ => type.ToString()
           };
       }
   }
}
