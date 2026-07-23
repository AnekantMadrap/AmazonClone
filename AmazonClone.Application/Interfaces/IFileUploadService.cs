using AmazonClone.Application.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AmazonClone.Application.Interfaces
{
    public interface IFileUploadService
    {
        Task<FileUploadResponseDto> UploadFileAsync(
            Stream fileStream,
            string fileName,
            string contentType,
            int? productId = null,
            int? variantId = null,
            bool isPrimary = false,
            int sortOrder = 0);

        Task<bool> DeleteFileAsync(int fileId);
    }
}
