using AmazonClone.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AmazonClone.API.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IFileUploadService _fileUploadService;

        public FilesController(IFileUploadService fileUploadService)
        {
            _fileUploadService = fileUploadService;
        }

        [HttpPost("upload")]
        [Authorize]
        public async Task<IActionResult> UploadFiles(
            [FromForm] List<IFormFile> files,
            [FromForm] int? productId = null,
            [FromForm] int? variantId = null,
            [FromForm] bool isPrimary = false,
            [FromForm] int sortOrder = 0)
        {
            if (files == null || files.Count == 0)
                return BadRequest(new { message = "No files provided." });

            var results = new List<object>();

            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                using var stream = file.OpenReadStream();
                // If uploading multiple files at once, only set primary for the first file if requested
                bool setPrimary = (i == 0) && isPrimary;
                int currentSortOrder = sortOrder + i;

                var res = await _fileUploadService.UploadFileAsync(
                    stream,
                    file.FileName,
                    file.ContentType,
                    productId,
                    variantId,
                    setPrimary,
                    currentSortOrder);

                results.Add(res);
            }

            return Ok(new { message = "File(s) uploaded successfully.", data = results });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteFile(int id)
        {
            var deleted = await _fileUploadService.DeleteFileAsync(id);
            if (!deleted)
                return NotFound(new { message = $"File with ID {id} not found." });

            return Ok(new { message = "File deleted successfully." });
        }
    }
}
