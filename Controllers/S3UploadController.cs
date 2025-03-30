using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace be.Controllers
{
    [ApiController]
    [Route("api/upload")]
    public class S3UploadController : ControllerBase
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;
        private readonly string _region;

        public S3UploadController(IConfiguration configuration)
        {
            _bucketName = configuration["AWS:BucketName"];
            _region = configuration["AWS:Region"];

            _s3Client = new AmazonS3Client(
                configuration["AWS:AccessKeyId"],
                configuration["AWS:SecretAccessKey"],
                new AmazonS3Config
                {
                    RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(_region)
                });
        }


        [HttpPost]
        public async Task<IActionResult> UploadFile(
            [Required] IFormFile file,
            [FromQuery] string? prefix)
        {
            try
            {
                // Validate file
                if (file.Length == 0)
                    return BadRequest("Empty file");

                // Generate unique filename
                var fileName = $"{DateTime.UtcNow:yyyyMMddHHmmss}-{file.FileName}";
                var objectKey = string.IsNullOrEmpty(prefix)
                    ? fileName
                    : $"{prefix.TrimEnd('/')}/{fileName}";

                // Upload to S3
                using var stream = file.OpenReadStream();
                var request = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = objectKey,
                    InputStream = stream,
                    ContentType = file.ContentType
                };

                await _s3Client.PutObjectAsync(request);

                var imageUrl = $"https://{_bucketName}.s3.{_region}.amazonaws.com/{objectKey}";
                return Ok(new { Url = imageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Upload failed: {ex.Message}");
            }
        }
    }
}
