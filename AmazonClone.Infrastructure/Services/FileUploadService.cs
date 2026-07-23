using AmazonClone.Application.DTOs;
using AmazonClone.Application.Interfaces;
using AmazonClone.Infrastructure.Data;
using Class_Library__.NET_.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AmazonClone.Infrastructure.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FileUploadService> _logger;
        private readonly string _uploadDirectory;

        public FileUploadService(ApplicationDbContext context, ILogger<FileUploadService> logger)
        {
            _context = context;
            _logger = logger;
            _uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "products");
            if (!Directory.Exists(_uploadDirectory))
            {
                Directory.CreateDirectory(_uploadDirectory);
            }
        }

        public async Task<bool> DeleteFileAsync(int fileId)
        {
            var fileRecord = await _context.UploadedFiles.FindAsync(fileId);
            if (fileRecord == null)
                return false;

            try
            {
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileRecord.FileUrl.TrimStart('/'));
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete physical file for fileId {FileId}", fileId);
            }

            _context.UploadedFiles.Remove(fileRecord);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<FileUploadResponseDto> UploadFileAsync(
            Stream fileStream,
            string fileName,
            string contentType,
            int? productId = null,
            int? variantId = null,
            bool isPrimary = false,
            int sortOrder = 0)
        {
            var fileExtension = Path.GetExtension(fileName);
            var uniqueFileName = $"{Guid.NewGuid()}_{DateTime.UtcNow.Ticks}{fileExtension}";
            var filePath = Path.Combine(_uploadDirectory, uniqueFileName);

            using (var destinationStream = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(destinationStream);
            }

            var relativeUrl = $"/uploads/products/{uniqueFileName}";
            var fileType = contentType.StartsWith("video", StringComparison.OrdinalIgnoreCase) ? "Video" : "Image";

            // If this is set to primary, un-primary existing records for this product/variant
            if (isPrimary && (productId.HasValue || variantId.HasValue))
            {
                var existingPrimaries = _context.UploadedFiles
                    .Where(f => (f.ProductId == productId && productId.HasValue) || (f.VariantId == variantId && variantId.HasValue));
                foreach (var record in existingPrimaries)
                {
                    record.IsPrimary = false;
                }
            }

            var uploadedRecord = new UploadedFileRecord
            {
                ProductId = productId,
                VariantId = variantId,
                FileUrl = relativeUrl,
                FileType = fileType,
                IsPrimary = isPrimary,
                SortOrder = sortOrder,
                UploadedAt = DateTime.UtcNow
            };

            _context.UploadedFiles.Add(uploadedRecord);
            await _context.SaveChangesAsync();

            return new FileUploadResponseDto
            {
                FileId = uploadedRecord.FileId,
                ProductId = uploadedRecord.ProductId,
                VariantId = uploadedRecord.VariantId,
                FileUrl = uploadedRecord.FileUrl,
                FileType = uploadedRecord.FileType,
                IsPrimary = uploadedRecord.IsPrimary,
                SortOrder = uploadedRecord.SortOrder
            };
        }
    }
}
