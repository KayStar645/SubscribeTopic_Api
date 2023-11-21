﻿using Core.Application.Interfaces.Services;
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

        public async Task<Result<string>> UploadFilesToGoogleDrive(UploadRequest pRequest)
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

                string[] folders = pRequest.FolderName.Split("/");
                foreach(string name in folders)
                {
                    var existingFolderQuery = service.Files.List();
                    existingFolderQuery.Q = $"name='{name}' and '{folderId}' in parents";
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
                            Name = name,
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
                string fileExtension = Path.GetExtension(new Uri(pRequest.FilePath).AbsolutePath);
                var existingFileQuery = service.Files.List();
                existingFileQuery.Q = $"name='{pRequest.FileName + fileExtension}' and '{folderId}' in parents";
                existingFileQuery.Fields = "files(id, name)";
                var existingFiles = existingFileQuery.Execute().Files;

                if (existingFiles != null && existingFiles.Count > 0)
                {
                    return Result<string>.Failure($"File có tên '{pRequest.FileName + fileExtension}' đã tồn tại!",
                        (int)HttpStatusCode.BadRequest);
                }
                else
                {
                    var fileMetaData = new Google.Apis.Drive.v3.Data.File()
                    {
                        Name = pRequest.FileName + fileExtension,
                        Parents = new List<string> { folderId },
                    };

                    using (var stream2 = new MemoryStream(new WebClient().DownloadData(pRequest.FilePath)))
                    {
                        FilesResource.CreateMediaUpload request = service.Files.Create(fileMetaData, stream2, "");
                        request.Fields = "id";
                        request.Upload();
                        var uploadFile = request.ResponseBody;

                        // Xây dựng đường dẫn đầy đủ
                        string fileId = uploadFile.Id;
                        string downloadUrl = $"{_path}{fileId}";
                        string result = _path + pRequest.FolderName + pRequest.FileName + fileExtension;

                        return Result<string>.Success(downloadUrl, (int)HttpStatusCode.Created);
                    }
                }
            }
        }
    }
}
