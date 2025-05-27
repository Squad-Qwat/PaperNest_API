using API.Controllers;
using API.Models;
using API.StateMachines;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using PaperNest_API.Controllers;
using Microsoft.AspNetCore.Http;
using API.Helpers.Enums;
using View.Pages.Global;

namespace View.Pages.Student
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
        private readonly CitationService _citationService;
        private User? _currentUser;
       private Workspace? _currentWorkspace;
        private readonly GlobalView _globalView;

        public StudentView(User currentUser,  AuthStateMachine authState)
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
            _citationService = new CitationService();
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
               Console.WriteLine("5. Manajemen Sitasi");
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
                   case "5":
                       ManageCitations(document);
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
            
           var newVersion = _documentBodyService.CreateDocumentBody(document.Id, _currentUser.Id ,comment, currentContent);
           document.UpdateAt = DateTime.Now;
            
           _documentService.Update(document.Id, document);
            
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
        if (version.IsCurrentVersion)
        {
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
        
       // Method untuk melihat detail versi dengan review (jika ada)
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
                        statusReview = "DONE";
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
               
               Console.WriteLine("\nPanduan tindak lanjut:");
               switch (review.Status)
               {
                   case ReviewStatus.Approved:
                       Console.WriteLine("Dokumen Anda telah disetujui. Anda dapat melanjutkan ke tahap berikutnya.");
                       break;
                   case ReviewStatus.NeedsRevision:
                       Console.WriteLine("Dokumen Anda memerlukan revisi. Silakan perbaiki sesuai komentar reviewer.");
                       Console.WriteLine("Setelah selesai merevisi, buat versi baru untuk direview kembali.");
                       break;
                   case ReviewStatus.Done:
                       Console.WriteLine("Dokumen Anda telah selesai dan disetujui !.");
                       break;
               }
           }
           else
           {
               Console.WriteLine("\nVersi ini belum direview.");
               Console.WriteLine("Versi ini masih menunggu review dari dosen.");
               Console.WriteLine("Silakan hubungi dosen Anda untuk mempercepat proses review.");
           }
       }

       // Method untuk mengedit workspace
       private void EditWorkspace()
       {
           if (_currentWorkspace == null)
           {
               Console.WriteLine("Tidak ada workspace yang dipilih.");
               return;
           }
            
           Console.WriteLine($"\n=== Edit Workspace: {_currentWorkspace.Title} ===");
            
           Console.Write($"Nama Baru (kosongkan untuk tetap '{_currentWorkspace.Title}'): ");
           string? title = Console.ReadLine();
            
           Console.Write($"Deskripsi Baru (kosongkan untuk tetap '{_currentWorkspace.Description ?? "kosong"}'): ");
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
        
       // Method untuk menghapus workspace
       private bool DeleteWorkspace()
       {
           if (_currentWorkspace == null)
           {
               Console.WriteLine("Tidak ada workspace yang dipilih.");
               return false;
           }
            
           Console.WriteLine($"\n=== Hapus Workspace: {_currentWorkspace.Title} ===");
           Console.Write("Anda yakin ingin menghapus workspace ini? (y/n): ");
           string? confirmation = Console.ReadLine();
            
           if (confirmation?.ToLower() != "y")
           {
               Console.WriteLine("Penghapusan workspace dibatalkan.");
               return false;
           }
            
           _workspaceService.Delete(_currentWorkspace.Id);
            Console.WriteLine("Workspace berhasil dihapus!");
            _currentWorkspace = null;
            return true;
       }
        
       // Method untuk menghapus dokumen
       private bool DeleteDocument(Guid documentId)
       {
           Console.WriteLine("\n=== Hapus Dokumen ===");
           Console.Write("Anda yakin ingin menghapus dokumen ini? (y/n): ");
           string? confirmation = Console.ReadLine();
            
           if (confirmation?.ToLower() != "y")
           {
               Console.WriteLine("Penghapusan dokumen dibatalkan.");
               return false;
           }
            
           _documentService.Delete(documentId);
            
            Console.WriteLine("Dokumen berhasil dihapus!");
            return true;
       }

       // Method untuk rollback ke versi sebelumnya
       private void RollbackDocumentVersion(Document document)
       {
           if (document == null)
           {
               Console.WriteLine("Tidak ada dokumen yang dipilih.");
               return;
           }
            
           Console.WriteLine($"\n=== Rollback Dokumen: {document.Title} ===");
            
           var versions = _documentBodyService.GetDocumentBodiesByDocumentId(document.Id);
            
           if (versions == null || !versions.Any())
           {
               Console.WriteLine("Belum ada versi dokumen untuk rollback.");
               return;
           }
            
            // Konversi ke List untuk akses yang konsisten
            var versionsList = versions.ToList();
            int index = 1;
            List<DocumentBody> nonCurrentVersions = new List<DocumentBody>();
            
            foreach (var version in versionsList)
            {
                if (!version.IsCurrentVersion)
                {
                    nonCurrentVersions.Add(version);
                    Console.WriteLine($"{nonCurrentVersions.Count}. Versi dari {version.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss")}");
                    Console.WriteLine($"   Deskripsi: {version.Comment}");
                    // Tampilkan preview konten (maksimal 50 karakter)
                    string contentPreview = version.Content.Length > 50 
                        ? version.Content.Substring(0, 50) + "..." 
                        : version.Content;
                    Console.WriteLine($"   Preview: {contentPreview}");
                    Console.WriteLine();
                }
                index++;
            }
            
            if (nonCurrentVersions.Count == 0)
            {
                Console.WriteLine("Tidak ada versi sebelumnya untuk rollback.");
                return;
            }
            
            Console.Write("Pilih versi untuk rollback (nomor) atau 0 untuk kembali: ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= nonCurrentVersions.Count)
            {
                var selectedVersion = nonCurrentVersions[choice - 1];
                
                Console.Write($"Anda yakin ingin rollback ke versi dari {selectedVersion.CreatedAt}? (y/n): ");
                string? confirmation = Console.ReadLine();
                
                if (confirmation?.ToLower() == "y")
                {
                    var newVersion = _documentBodyService.RollbackToPreviousDocumentBody(document.Id, selectedVersion.Id);
                    document.SavedContent = newVersion.Content;
                    document.UpdateAt = DateTime.Now;
                    
                    _documentService.Update(document.Id, document);
                    
                    Console.WriteLine("Rollback berhasil!");
                }
                else
                {
                    Console.WriteLine("Rollback dibatalkan.");
                }
            }
       }
        
       // Metode pengeditan teks interaktif untuk multiline text
       private string ReadAndEditMultilineText(string initialText)
       {
           // Split text into lines for multiline support
           List<string> lines = initialText.Split(Environment.NewLine).ToList();
           if (lines.Count == 0) lines.Add(string.Empty);
            
           int currentLine = 0;
           bool editing = true;
            
           Console.Clear();
           Console.WriteLine("==== EDITOR INTERAKTIF ====");
           Console.WriteLine("Navigasi: ↑↓ = pindah baris, ←→ = pindah karakter");
           Console.WriteLine("Esc = keluar dan simpan, Ctrl+C = keluar tanpa simpan");
           Console.WriteLine("===============================");
            
           // Display all lines
           Console.WriteLine();
           for (int i = 0; i < lines.Count; i++)
           {
               Console.WriteLine($"{i+1}: {lines[i]}");
           }
            
           // Position cursor at the beginning of first line
           Console.SetCursorPosition(3, 5); // Adjust for header and line numbers
           int cursorX = 3;  // Line number + ": " = 3 characters
           int cursorY = 5;  // Starting line after header
            
           while (editing)
           {
               // Read a key press without displaying it
               var key = Console.ReadKey(true);
                
               switch (key.Key)
               {
                   case ConsoleKey.Escape:
                       editing = false;  // Done editing
                       break;
                        
                   case ConsoleKey.UpArrow:
                       if (currentLine > 0)
                       {
                           currentLine--;
                           cursorY--;
                           // Make sure cursor X position is valid for the new line
                           cursorX = Math.Min(lines[currentLine].Length + 3, cursorX);
                           Console.SetCursorPosition(cursorX, cursorY);
                       }
                       break;
                        
                   case ConsoleKey.DownArrow:
                       if (currentLine < lines.Count - 1)
                       {
                           currentLine++;
                           cursorY++;
                           // Make sure cursor X position is valid for the new line
                           cursorX = Math.Min(lines[currentLine].Length + 3, cursorX);
                           Console.SetCursorPosition(cursorX, cursorY);
                       }
                       break;
                        
                   case ConsoleKey.LeftArrow:
                       if (cursorX > 3)  // Don't move left of the line number prefix
                       {
                           cursorX--;
                           Console.SetCursorPosition(cursorX, cursorY);
                       }
                       break;
                        
                   case ConsoleKey.RightArrow:
                       if (cursorX < lines[currentLine].Length + 3)
                       {
                           cursorX++;
                           Console.SetCursorPosition(cursorX, cursorY);
                       }
                       break;
                        
                   case ConsoleKey.Enter:
                       // Insert a new line
                       string currentLineText = lines[currentLine];
                       int posInLine = cursorX - 3;
                        
                       string beforeCursor = posInLine > 0 ? currentLineText.Substring(0, posInLine) : "";
                       string afterCursor = posInLine < currentLineText.Length ? currentLineText.Substring(posInLine) : "";
                        
                       lines[currentLine] = beforeCursor;
                       lines.Insert(currentLine + 1, afterCursor);
                        
                       // Redraw all lines after the current one
                       for (int i = currentLine; i < lines.Count; i++)
                       {
                           Console.SetCursorPosition(0, i + 5);
                           Console.Write(new string(' ', Console.WindowWidth));  // Clear the line
                           Console.SetCursorPosition(0, i + 5);
                           Console.Write($"{i+1}: {lines[i]}");
                       }
                        
                       currentLine++;
                       cursorY++;
                       cursorX = 3;  // Move to beginning of the new line
                       Console.SetCursorPosition(cursorX, cursorY);
                       break;
                        
                   case ConsoleKey.Backspace:
                       int positionInLine = cursorX - 3;
                       if (positionInLine > 0)
                       {
                           // Remove character from current line
                           string lineText = lines[currentLine];
                           lines[currentLine] = lineText.Remove(positionInLine - 1, 1);
                            
                           // Redraw the current line
                           Console.SetCursorPosition(0, cursorY);
                           Console.Write(new string(' ', Console.WindowWidth));  // Clear the line
                           Console.SetCursorPosition(0, cursorY);
                           Console.Write($"{currentLine+1}: {lines[currentLine]}");
                            
                           // Move cursor back one position
                           cursorX--;
                           Console.SetCursorPosition(cursorX, cursorY);
                       }
                       else if (currentLine > 0)
                       {
                           // At beginning of line, join with previous line
                           string previousLine = lines[currentLine - 1];
                           string lineToJoin = lines[currentLine];
                            
                           // Remove the current line and append its content to the previous line
                           lines.RemoveAt(currentLine);
                           int previousLineLength = previousLine.Length;
                           lines[currentLine - 1] = previousLine + lineToJoin;
                            
                           // Redraw all lines from previous line onwards
                           for (int i = currentLine - 1; i < lines.Count; i++)
                           {
                               Console.SetCursorPosition(0, i + 5);
                               Console.Write(new string(' ', Console.WindowWidth));  // Clear the line
                               Console.SetCursorPosition(0, i + 5);
                               Console.Write($"{i+1}: {lines[i]}");
                           }
                            
                           // Clear the last line that's now empty
                           Console.SetCursorPosition(0, lines.Count + 5);
                           Console.Write(new string(' ', Console.WindowWidth));
                            
                           // Update cursor position
                           currentLine--;
                           cursorY--;
                           cursorX = 3 + previousLineLength;
                           Console.SetCursorPosition(cursorX, cursorY);
                       }
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
                               Console.SetCursorPosition(0, cursorY);
                               Console.Write(new string(' ', Console.WindowWidth));  // Clear the line
                               Console.SetCursorPosition(0, cursorY);
                               Console.Write($"{currentLine+1}: {lines[currentLine]}");
                                
                               // Move cursor forward one position
                               cursorX++;
                               Console.SetCursorPosition(cursorX, cursorY);
                           }
                       }
                       break;
               }
           }
            
           Console.Clear();  // Clear the screen after editing is done
           return string.Join(Environment.NewLine, lines);
       }

       // Method for managing citations
       private void ManageCitations(Document document)
       {
           if (document == null)
           {
               Console.WriteLine("Tidak ada dokumen yang dipilih.");
               return;
           }

           bool backToDocumentMenu = false;

           while (!backToDocumentMenu)
           {
               Console.WriteLine($"\n=== Manajemen Sitasi untuk Dokumen: {document.Title} ===");
               Console.WriteLine("1. Lihat Semua Sitasi");
               Console.WriteLine("2. Tambah Sitasi Baru");
               Console.WriteLine("3. Edit Sitasi");
               Console.WriteLine("4. Hapus Sitasi");
               Console.WriteLine("5. Lihat Format APA Sitasi");
               Console.WriteLine("0. Kembali ke Menu Dokumen");
               Console.Write("Pilih menu: ");

               string? choice = Console.ReadLine();

               switch (choice)
               {
                   case "1":
                       ViewCitations(document.Id);
                       break;
                   case "2":
                       CreateCitation(document.Id);
                       break;
                   case "3":
                       EditCitation(document.Id);
                       break;
                   case "4":
                       DeleteCitation(document.Id);
                       break;
                   case "5":
                       ViewAPAFormat(document.Id);
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
       private void ViewCitations(Guid documentId)
       {
           Console.WriteLine("\n=== Daftar Sitasi dalam Dokumen ===");

           var citations = _citationService.GetCitationsByDocumentId(documentId);

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
               Console.WriteLine($"   Informasi Publikasi: {citation.PublicationInfo}");
               
               if (citation.PublicationDate.HasValue)
               {
                   Console.WriteLine($"   Tanggal Publikasi: {citation.PublicationDate.Value.ToString("dd/MM/yyyy")}");
               }
               
               Console.WriteLine();
               index++;
           }
       }

       // Method untuk membuat sitasi baru
       private void CreateCitation(Guid documentId)
       {
           Console.WriteLine("\n=== Tambah Sitasi Baru ===");

           Console.WriteLine("Pilih tipe sitasi:");
           Console.WriteLine("1. Buku");
           Console.WriteLine("2. Artikel Jurnal");
           Console.WriteLine("3. Website");
           Console.WriteLine("4. Makalah Konferensi");
           Console.WriteLine("5. Tesis/Disertasi");
           Console.Write("Pilih tipe: ");

           if (!int.TryParse(Console.ReadLine(), out int typeChoice) || typeChoice < 1 || typeChoice > 5)
           {
               Console.WriteLine("Pilihan tipe tidak valid.");
               return;
           }

           CitationType citationType = (CitationType)(typeChoice - 1);

           Console.Write("Judul: ");
           string? title = Console.ReadLine();

           Console.Write("Penulis: ");
           string? author = Console.ReadLine();

           Console.Write("Informasi Publikasi (seperti penerbit, URL, nama jurnal): ");
           string? publicationInfo = Console.ReadLine();

           if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(author) || string.IsNullOrWhiteSpace(publicationInfo))
           {
               Console.WriteLine("Judul, penulis, dan informasi publikasi harus diisi.");
               return;
           }

           DateTime? publicationDate = null;
           Console.Write("Tanggal Publikasi (DD/MM/YYYY) [opsional, tekan Enter untuk lewati]: ");
           string? dateInput = Console.ReadLine();
           if (!string.IsNullOrWhiteSpace(dateInput) && DateTime.TryParse(dateInput, out DateTime parsedDate))
           {
               publicationDate = parsedDate;
           }

           string? accessDate = null;
           if (citationType == CitationType.Website)
           {
               Console.Write("Tanggal Akses [opsional, tekan Enter untuk lewati]: ");
               accessDate = Console.ReadLine();
           }

           string? doi = null;
           if (citationType == CitationType.JournalArticle || citationType == CitationType.ConferencePaper)
           {
               Console.Write("DOI [opsional, tekan Enter untuk lewati]: ");
               doi = Console.ReadLine();
           }

           try
           {
               var citation = _citationService.CreateCitation(
                   citationType,
                   title,
                   author,
                   publicationInfo,
                   documentId,
                   publicationDate,
                   accessDate,
                   doi
               );

               Console.WriteLine("Sitasi berhasil ditambahkan!");
               Console.WriteLine($"ID Sitasi: {citation.Id}");
           }
           catch (Exception ex)
           {
               Console.WriteLine($"Gagal menambahkan sitasi: {ex.Message}");
           }
       }

       // Method untuk mengedit sitasi
       private void EditCitation(Guid documentId)
       {
           Console.WriteLine("\n=== Edit Sitasi ===");

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

           Console.Write("Pilih sitasi yang akan diedit (nomor) atau 0 untuk kembali: ");
           if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > citations.Count)
           {
               if (choice != 0) Console.WriteLine("Pilihan tidak valid.");
               return;
           }

           var selectedCitation = citations[choice - 1];

           Console.WriteLine("\nEdit informasi sitasi (kosongkan field untuk mempertahankan nilai saat ini):");

           Console.WriteLine("Pilih tipe sitasi:");
           Console.WriteLine("1. Buku");
           Console.WriteLine("2. Artikel Jurnal");
           Console.WriteLine("3. Website");
           Console.WriteLine("4. Makalah Konferensi");
           Console.WriteLine("5. Tesis/Disertasi");
           Console.Write($"Pilih tipe [{GetCitationTypeDisplay(selectedCitation.Type)}]: ");

           string? typeInput = Console.ReadLine();
           CitationType citationType = selectedCitation.Type;
           if (!string.IsNullOrWhiteSpace(typeInput) && int.TryParse(typeInput, out int typeChoice) && typeChoice >= 1 && typeChoice <= 5)
           {
               citationType = (CitationType)(typeChoice - 1);
           }

           Console.Write($"Judul [{selectedCitation.Title}]: ");
           string? title = Console.ReadLine();
           if (string.IsNullOrWhiteSpace(title))
           {
               title = selectedCitation.Title;
           }

           Console.Write($"Penulis [{selectedCitation.Author}]: ");
           string? author = Console.ReadLine();
           if (string.IsNullOrWhiteSpace(author))
           {
               author = selectedCitation.Author;
           }

           Console.Write($"Informasi Publikasi [{selectedCitation.PublicationInfo}]: ");
           string? publicationInfo = Console.ReadLine();
           if (string.IsNullOrWhiteSpace(publicationInfo))
           {
               publicationInfo = selectedCitation.PublicationInfo;
           }

           DateTime? publicationDate = selectedCitation.PublicationDate;
           Console.Write($"Tanggal Publikasi [{(selectedCitation.PublicationDate.HasValue ? selectedCitation.PublicationDate.Value.ToString("dd/MM/yyyy") : "Tidak ada")}] (DD/MM/YYYY) [opsional, tekan Enter untuk pertahankan]: ");
           string? dateInput = Console.ReadLine();
           if (!string.IsNullOrWhiteSpace(dateInput))
           {
               if (DateTime.TryParse(dateInput, out DateTime parsedDate))
               {
                   publicationDate = parsedDate;
               }
               else if (dateInput.ToLower() == "hapus")
               {
                   publicationDate = null;
               }
           }

           string? accessDate = selectedCitation.AccessDate;
           if (citationType == CitationType.Website)
           {
               Console.Write($"Tanggal Akses [{selectedCitation.AccessDate ?? "Tidak ada"}] [opsional, tekan Enter untuk pertahankan]: ");
               string? accessDateInput = Console.ReadLine();
               if (!string.IsNullOrWhiteSpace(accessDateInput))
               {
                   if (accessDateInput.ToLower() == "hapus")
                   {
                       accessDate = null;
                   }
                   else
                   {
                       accessDate = accessDateInput;
                   }
               }
           }

           string? doi = selectedCitation.DOI;
           if (citationType == CitationType.JournalArticle || citationType == CitationType.ConferencePaper)
           {
               Console.Write($"DOI [{selectedCitation.DOI ?? "Tidak ada"}] [opsional, tekan Enter untuk pertahankan]: ");
               string? doiInput = Console.ReadLine();
               if (!string.IsNullOrWhiteSpace(doiInput))
               {
                   if (doiInput.ToLower() == "hapus")
                   {
                       doi = null;
                   }
                   else
                   {
                       doi = doiInput;
                   }
               }
           }

           try
           {
               var updatedCitation = _citationService.UpdateCitation(
                   selectedCitation.Id,
                   citationType,
                   title,
                   author,
                   publicationInfo,
                   publicationDate,
                   accessDate,
                   doi
               );

               if (updatedCitation != null)
               {
                   Console.WriteLine("Sitasi berhasil diperbarui!");
               }
               else
               {
                   Console.WriteLine("Gagal memperbarui sitasi.");
               }
           }
           catch (Exception ex)
           {
               Console.WriteLine($"Error: {ex.Message}");
           }
       }

       // Method untuk menghapus sitasi
       private void DeleteCitation(Guid documentId)
       {
           Console.WriteLine("\n=== Hapus Sitasi ===");

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

           Console.Write("Pilih sitasi yang akan dihapus (nomor) atau 0 untuk kembali: ");
           if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > citations.Count)
           {
               if (choice != 0) Console.WriteLine("Pilihan tidak valid.");
               return;
           }

           var selectedCitation = citations[choice - 1];

           Console.Write($"Anda yakin ingin menghapus sitasi '{selectedCitation.Title}'? (y/n): ");
           string? confirmation = Console.ReadLine();

           if (confirmation?.ToLower() == "y")
           {
               bool result = _citationService.DeleteCitation(selectedCitation.Id);
               if (result)
               {
                   Console.WriteLine("Sitasi berhasil dihapus.");
               }
               else
               {
                   Console.WriteLine("Gagal menghapus sitasi.");
               }
           }
           else
           {
               Console.WriteLine("Penghapusan sitasi dibatalkan.");
           }
       }

       // Method untuk melihat format APA sitasi
       private void ViewAPAFormat(Guid documentId)
       {
           Console.WriteLine("\n=== Format APA Sitasi ===");

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

           Console.Write("Pilih sitasi untuk melihat format APA (nomor) atau 0 untuk kembali: ");
           if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > citations.Count)
           {
               if (choice != 0) Console.WriteLine("Pilihan tidak valid.");
               return;
           }

           var selectedCitation = citations[choice - 1];
           string? apaFormat = _citationService.GetFormattedCitationAPA(selectedCitation.Id);

           Console.WriteLine("\n=== Format APA ===");
           Console.WriteLine(apaFormat ?? "Tidak dapat membuat format APA untuk sitasi ini.");
       }

       // Helper untuk mendapatkan nama tipe sitasi dalam bahasa Indonesia
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