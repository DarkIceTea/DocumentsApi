using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;

namespace DocumentsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesManagerController : ControllerBase
    {
        private const string ContainerName = "myfiles";
        public const string SuccessMessageKey = "SuccessMessage";
        public const string ErrorMessageKey = "ErrorMessage";
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _containerClient;
        public FilesManagerController(BlobServiceClient blobClient)
        {
            _blobServiceClient = blobClient;
            _containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);
            _containerClient.CreateIfNotExists();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                var blobClient = _containerClient.GetBlobClient(file.FileName);
                await blobClient.UploadAsync(file.OpenReadStream(), true);
            }
            catch (Exception)
            {
                throw;
            }
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> Download(string fileName)
        {
            try
            {
                var blobClient = _containerClient.GetBlobClient(fileName);
                var memoryStream = new MemoryStream();
                await blobClient.DownloadToAsync(memoryStream);
                memoryStream.Position = 0;
                var contentType = blobClient.GetProperties().Value.ContentType;
                return File(memoryStream, contentType, fileName);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpDelete("[action]")]
        public async Task<IActionResult> Delete(string fileName)
        {
            try
            {
                var blobClient = _containerClient.GetBlobClient(fileName);
                await blobClient.DeleteAsync();
            }
            catch (Exception)
            {
                throw;
            }
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> List()
        {
            var blobs = _containerClient.GetBlobsAsync();
            var blobNames = new List<string>();

            await foreach (var blob in blobs)
            {
                blobNames.Add(blob.Name);
            }

            return Ok(blobNames);
        }

    }
}
