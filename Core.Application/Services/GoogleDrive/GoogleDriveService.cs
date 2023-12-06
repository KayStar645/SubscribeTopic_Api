using Core.Application.DTOs.Common;
using Core.Application.Interfaces.Services;
using Core.Application.Models.GoogleDrive;
using Core.Application.Responses;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System.Net;

namespace Core.Application.Services.GoogleDrive
{
    public class GoogleDriveService : IGoogleDriveService
    {
        private const string _credentialsPath = "../Core.Application/Services/GoogleDrive/client_secret.json";
        private const string _folderId = "1T12wTE6cGjqOBJ2pTGo3vxGfXT3mELdq";
        private const string _path = "https://drive.google.com/uc?id=";


        public async Task<Result<UploadResponse>> UploadFilesToGoogleDrive(UploadRequest pRequest)
        {
            GoogleCredential credential;

            using (var stream1 = new FileStream(_credentialsPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream1)
                     .CreateScoped(DriveService.ScopeConstants.DriveFile);

                var service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Google Drive Upload SubscribeTopic"
                });

                // Tạo thư mục trước khi lưu tệp tin
                var folderId = _folderId;

                string[] folders = pRequest.FileName.Split("/");
                var fileName = folders[folders.Length - 1];
                for (int i = 0; i < folders.Length - 1; i++)
                {
                    var existingFolderQuery = service.Files.List();
                    existingFolderQuery.Q = $"name='{folders[i]}' and '{folderId}' in parents";
                    existingFolderQuery.Fields = "files(id, name)";
                    var existingFolders = existingFolderQuery.Execute().Files;

                    if (existingFolders != null && existingFolders.Count > 0)
                    {
                        folderId = existingFolders.First().Id;
                    }
                    else
                    {
                        var folderMetadata = new Google.Apis.Drive.v3.Data.File()
                        {
                            Name = folders[i],
                            MimeType = "application/vnd.google-apps.folder",
                            Parents = new List<string> { folderId },
                        };

                        var folderRequest = service.Files.Create(folderMetadata);
                        folderRequest.Fields = "id";
                        var folder = folderRequest.Execute();
                        folderId = folder.Id;
                    }
                }

                // Kiểm tra xem tệp tin đã tồn tại chưa
                string fileExtension = Path.GetExtension(pRequest.File.FileName);
                var existingFileQuery = service.Files.List();
                existingFileQuery.Q = $"name='{fileName + fileExtension}' and '{folderId}' in parents";
                existingFileQuery.Fields = "files(id, name)";
                var existingFiles = existingFileQuery.Execute().Files;


                if (existingFiles != null && existingFiles.Count > 0)
                {
                    return Result<UploadResponse>.Failure($"File có tên '{pRequest.FileName + fileExtension}' đã tồn tại!",
                        (int)HttpStatusCode.BadRequest);
                }
                else
                {
                    var fileMetaData = new Google.Apis.Drive.v3.Data.File()
                    {
                        Name = fileName + fileExtension,
                        Parents = new List<string> { folderId },
                    };

                    using (var stream2 = new MemoryStream())
                    {
                        pRequest.File.OpenReadStream().CopyTo(stream2);
                        stream2.Position = 0;

                        FilesResource.CreateMediaUpload request = service.Files.Create(fileMetaData, stream2, "");
                        request.Fields = "id,size,mimeType";
                        request.Upload();
                        var uploadFile = request.ResponseBody;

                        var result = new UploadResponse
                        {
                            Name = fileName,
                            Path = $"{_path}{uploadFile.Id}",
                            Type = uploadFile.MimeType,
                            SizeInBytes = uploadFile.Size
                        };

                        return Result<UploadResponse>.Success(result, (int)HttpStatusCode.Created);
                    }

                }
            }
        }

        public async Task<Result<FileDto>> GetFileInfoFromGoogleDrive(string filePath)
        {
            // Lấy fileId từ đường dẫn Google Drive
            var fileId = GetFileIdFromDrivePath(filePath);

            if (string.IsNullOrEmpty(fileId) == true)
            {
                return Result<FileDto>.Failure("Không thể xác định fileId từ đường dẫn.", (int)HttpStatusCode.BadRequest);
            }

            GoogleCredential credential;

            using (var stream1 = new FileStream(_credentialsPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream1)
                    .CreateScoped(DriveService.ScopeConstants.DriveFile);

                var service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Google Drive Upload SubscribeTopic"
                });

                try
                {
                    var file = service.Files.Get(fileId).Execute();

                    // Lấy thông tin kích thước của tệp tin sau khi đã được tải lên
                    var fileRequest = service.Files.Get(fileId);
                    fileRequest.Fields = "size";
                    var fileInfo = fileRequest.Execute();

                    var fileDto = new FileDto
                    {
                        Name = file.Name,
                        Path = $"{_path}{file.Id}",
                        SizeInBytes = fileInfo.Size ?? 0,
                        Type = file.MimeType
                    };


                    return Result<FileDto>.Success(fileDto, (int)HttpStatusCode.OK);
                }
                catch (Exception ex)
                {
                    return Result<FileDto>.Failure($"Không thể lấy thông tin về tệp: {ex.Message}", (int)HttpStatusCode.InternalServerError);
                }
            }
        }

        private string GetFileIdFromDrivePath(string drivePath)
        {
            var uri = new Uri(drivePath);

            // Kiểm tra xem đường dẫn có chứa tham số "id" không
            var fileId = System.Web.HttpUtility.ParseQueryString(uri.Query).Get("id");

            return fileId;
        }

    }
}
