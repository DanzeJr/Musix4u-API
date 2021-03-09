using System;
using Microsoft.AspNetCore.Http;

namespace Musix4u_API.Models
{
    public class FormFileAbstraction : TagLib.File.IFileAbstraction
    {
        private readonly IFormFile _file;

        public FormFileAbstraction(IFormFile file)
        {
            _file = file;
        }

        public string Name => _file.FileName;

        public System.IO.Stream ReadStream => _file.OpenReadStream();

        public System.IO.Stream WriteStream => throw new Exception("Cannot write to IFormFile");

        public void CloseStream(System.IO.Stream stream) { }
    }
}