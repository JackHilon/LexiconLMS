using Microsoft.AspNetCore.Http;

namespace LexiconLMS.Models.Models.Document
{
    public class FileInputModel
    {
        public IFormFile FileToUpload { get; set; }
    }
}
