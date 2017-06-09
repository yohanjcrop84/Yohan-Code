using Sabio.Web.Controllers.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.Runtime;
using Amazon.S3.Model;
using Sabio.Web.Services.AWS;
using Sabio.Web.Models.Responses;
using Sabio.Web.Services;

namespace Sabio.Web.Controllers.Api
{
    [RoutePrefix("api/files")]

    public class FilesApiController : ApiController
    {
        IFileServices _fileServices = null;
        IImagesService _imagesService = null;
        public FilesApiController(IFileServices fileServices, IImagesService imagesService)
        {
            _fileServices = fileServices;
            _imagesService = imagesService;
        }


        [Route, HttpPost]
        public HttpResponseMessage Upload()
        {
            //

            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count < 1)
            {
                ErrorResponse err = new ErrorResponse("There is no file to be uploaded.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, err);
            }

            List<string> filepaths = null;
            foreach (string file in httpRequest.Files)
            {
                filepaths = new List<string>();
                var postedFile = httpRequest.Files[file];
                string newFilePath = _fileServices.Uploaded(postedFile.InputStream, postedFile.FileName);

                filepaths.Add(newFilePath);
            }


            return Request.CreateResponse(HttpStatusCode.OK, filepaths);
        }


        [Route("{mediatypeId:int}/products/{pId:int}"), HttpPost]
        public HttpResponseMessage UploadProductImages(int mediaTypeId, int pId)
        {
            //
            List<string> filepaths = null;

            if (!IsValidFileRequest())
            {
                return GetBadRequest();
            }

            filepaths = UploadProductMedia(mediaTypeId, pId);

            return Request.CreateResponse(HttpStatusCode.OK, filepaths);
        }

        [Route("blogs/{id:int}"), HttpPost]
        public HttpResponseMessage UploadBlogImage(int Id)
        {
            //
            List<string> filepaths = null;

            if (!IsValidFileRequest())
            {
                return GetBadRequest();
            }

            filepaths = UploadBlogMedia(Id);

            return Request.CreateResponse(HttpStatusCode.OK, filepaths);
        }


        [Route("delete/{id:int}"), HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            if (id <= 0)
            {
                return GetBadFileDeleteRequest();
            }

            /* Checks to see if the file on the server is actually deleted before deleting it from the database. */
            bool isDeleted = _fileServices.deleteFilesInDirectory(id);
            if (isDeleted == true)
            {
                _imagesService.DeleteFromProductImages(id);
                _imagesService.Delete(id);

                return Request.CreateResponse(HttpStatusCode.OK, id);
            }
            else
            {
                ErrorResponse err = new ErrorResponse("The file is not deleted.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, err);
            }
        }

        private HttpResponseMessage GetBadFileDeleteRequest()
        {
            ErrorResponse err = new ErrorResponse("There is no file to be deleted.");
            return Request.CreateResponse(HttpStatusCode.BadRequest, err);
        }

        private HttpResponseMessage GetBadRequest()
        {
            ErrorResponse err = new ErrorResponse("There is no file to be uploaded.");
            return Request.CreateResponse(HttpStatusCode.BadRequest, err);
        }

        private List<string> UploadProductMedia(int mediaTypeId, int pId = 0)
        {
            List<string> filepaths = new List<string>();

            foreach (string file in HttpContext.Current.Request.Files)
            {

                var postedFile = HttpContext.Current.Request.Files[file];
                string newFilePath = _fileServices.Uploaded(postedFile.InputStream, postedFile.FileName, mediaTypeId, pId);

                filepaths.Add(newFilePath);
            }

            return filepaths;
        }

        private List<string> UploadBlogMedia(int BlogId)
        {
            List<string> filepaths = new List<string>();

            foreach (string file in HttpContext.Current.Request.Files)
            {

                var postedFile = HttpContext.Current.Request.Files[file];
                string newFilePath = _fileServices.UploadBlogImage (postedFile.InputStream, postedFile.FileName, BlogId);

                filepaths.Add(newFilePath);
            }

            return filepaths;
        }
        private static bool IsValidFileRequest()
        {
            HttpRequest httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count < 1)
            {
                return false;
            }
            {
                return true;
            }
        }
    }
}

