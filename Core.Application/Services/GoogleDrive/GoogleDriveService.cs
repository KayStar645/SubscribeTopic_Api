using Core.Application.Interfaces.Services;
using Core.Application.Responses;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Core.Application.Services.GoogleDrive
{
    public class GoogleDriveService : IGoogleDriveService
    {
        private const string CLIENT_ID = "766566499840-hb43bmbgvup0aacmt7a0bp4m19hboiga.apps.googleusercontent.com";
        private const string CLIENT_SECRET = "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";
        private const string REDIRECT_URI = "https://developers.google.com/oauthplayground";
        private const string REFRESH_TOKEN = "1//04yBWS43ntVnQCgYIARAAGAQSNwF-L9Ir7mwXuJaYz8sPTOeZ7Ek1iyJY61TrU_m8sVp7yDSGcjqC8IZtnbcB6snl68PZ4OK9RSQ";

        public async Task<Result<string>> UploadImage(IFormFile file)
        {
            try
            {
                UserCredential credential;

                using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
                {
                    string credPath = "token.json";

                    credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        new[] { DriveService.Scope.DriveFile },
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true));
                }

                var driveService = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "SubscribeTopic",
                });

                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = file.FileName,
                };

                FilesResource.CreateMediaUpload request;

                using (var stream = file.OpenReadStream())
                {
                    request = driveService.Files.Create(fileMetadata, stream, file.ContentType);
                    request.Fields = "id";
                    await request.UploadAsync();
                }

                var fileDrive = request.ResponseBody;
                var fileId = fileDrive.Id;

                return Result<string>.Success(fileId, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<string>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
