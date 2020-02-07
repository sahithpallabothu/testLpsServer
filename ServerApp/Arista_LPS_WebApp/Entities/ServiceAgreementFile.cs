using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arista_LPS_WebApp.Entities
{
    public class ServiceAgreementFile
    {
      //  public string FileName { set; get; }
      //  public string FileContent { set; get; }
        public IFormFile File { set; get; }
        /*
         fileName : string;
    fileExt : string;
    fileMimeType : string;
    fileContent : File;
    fileContentBlob : Blob;
         */
    }
}
