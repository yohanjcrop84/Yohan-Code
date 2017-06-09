using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Sabio.Web.Models.Requests;
using Sabio.Web.Models.Requests.Images;
using Sabio.Web.Models.Responses;
using Sabio.Web.Controllers.Api;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.Runtime;
using Amazon.S3.Model;
using System.Data;
using Sabio.Web.Models;
using Sabio.Data;
using Sabio.Web.Domain;

namespace Sabio.Web.Services.AWS
{

    public class FileServices : IFileServices
    {

        private IConfigService _configService = null;
        private IImagesService _imagesService = null;
        private IUserService _userService = null;
        public FileServices(IConfigService configService, IImagesService imagesService, IUserService userService)
        {
            _configService = configService;
            _imagesService = imagesService;
            _userService = userService;
        }


        //작성폼


        public string Uploaded(Stream fileStream, string fileName, int mediaTypeId = 0, int pId = 0)
        {
            try
            {//여기서 클라이언트 만들고
                var g = Guid.NewGuid();
                string fileNameAtAws = "C32/" + g.ToString() + "_" + fileName.Replace("+", "0");
                AWSCredentials awsCredentials = new BasicAWSCredentials(_configService.AwsAccessKeyId, _configService.AwsSecret);
                AmazonS3Client amazonS3 = new AmazonS3Client(awsCredentials, Amazon.RegionEndpoint.USWest2);
                TransferUtility fileTransferUtility = new TransferUtility(amazonS3);
                //파일 트랜스퍼 유틸리티라는 그릇에 amazones3주워 담은다음에
                TransferUtilityUploadRequest uploadRequest = new TransferUtilityUploadRequest();
                //업로드 신청서에 버킷 이름 쉽게말해 폴더안에 파일을 스트림 즉 흐르게 하고 키에다가 파일이 들어가는 형식을 만들어 적는다
                uploadRequest.BucketName = _configService.AwsBucketName;
                uploadRequest.InputStream = fileStream;
                uploadRequest.Key = fileNameAtAws;
                // 그후에 파일트랜스퍼유틸리티에 업로드 속성을 주어서 업로드가 신청서의 속성을 가지게 한다.
                fileTransferUtility.Upload(uploadRequest);
                ///_imageSrv . insert
                ImagesAddRequest request = new ImagesAddRequest();
                request.ImageUrl = fileNameAtAws;
                request.Title = fileName;
                if (mediaTypeId > 0)
                {
                    request.MediaTypeId = mediaTypeId;
                }

                if (request.MediaTypeId == 1) // If the image to be added is a main image, change all other images to secondary.
                {
                    _imagesService.UpdateMainToProductImage(pId);
                }
                else if (request.MediaTypeId == 3) // If it's a secondary image, change the current secondary to a regular product image.
                {
                    _imagesService.UpdateSecondaryToProductImage(pId);
                }

                int imgId = _imagesService.Insert(request, _userService.GetCurrentUserId());

                if (imgId > 0 && pId > 0)
                {
                    _imagesService.InsertToProductImages(imgId, pId);
                }

                return fileNameAtAws;
            }
            catch (AmazonS3Exception)
            {
                throw;
            }
        }

        public string UploadBlogImage(Stream fileStream, string fileName, int bId = 0)
        {
            try
            {
                var g = Guid.NewGuid();
                string fileNameAtAws = "C32/" + g.ToString() + "_" + fileName.Replace("+", "0");
                AWSCredentials awsCredentials = new BasicAWSCredentials(_configService.AwsAccessKeyId, _configService.AwsSecret);
                AmazonS3Client amazonS3 = new AmazonS3Client(awsCredentials, Amazon.RegionEndpoint.USWest2);
                TransferUtility fileTransferUtility = new TransferUtility(amazonS3);
                TransferUtilityUploadRequest uploadRequest = new TransferUtilityUploadRequest();
              
                uploadRequest.BucketName = _configService.AwsBucketName;
                uploadRequest.InputStream = fileStream;
                uploadRequest.Key = fileNameAtAws;
            
                fileTransferUtility.Upload(uploadRequest);

                ImagesAddRequest request = new ImagesAddRequest();
                request.ImageUrl = fileNameAtAws;
                request.Title = fileName;

                int imgId = _imagesService.Insert(request, _userService.GetCurrentUserId());
                Image update = new Image();
                update.ImageUrl = fileNameAtAws;


                if (imgId > 0 && bId > 0)
                {
                    _imagesService.UpdateBlogImages(update, bId);
                }

                return fileNameAtAws;
            }
            catch (AmazonS3Exception)
            {
                throw;
            }
        }
        public bool deleteFilesInDirectory(int id)
        {
            DeleteObjectResponse response = null;
            bool ok = false;

            try
            {
                if (id > 0)
                {
                    Image image = _imagesService.Get(id);

                    if (image != null)
                    {
                        string fileNameAtAws = image.ImageUrl;

                        AWSCredentials awsCredentials = new BasicAWSCredentials(_configService.AwsAccessKeyId, _configService.AwsSecret);
                        AmazonS3Client client = new AmazonS3Client(awsCredentials, Amazon.RegionEndpoint.USWest2);

                        DeleteObjectRequest fileDeleteRequest = new DeleteObjectRequest();
                        fileDeleteRequest.BucketName = _configService.AwsBucketName;
                        fileDeleteRequest.Key = fileNameAtAws;

                        bool keyExists = ExistsKey(client, fileNameAtAws, fileDeleteRequest.BucketName);

                        if (keyExists)
                        {
                            response = client.DeleteObject(fileDeleteRequest);

                            ok = true;

                        }
                    }
                }
            }
            catch (AmazonS3Exception)
            {
                throw;
            }

            return ok;
        }

        private bool ExistsKey(AmazonS3Client client, string key, string bucketName)
        {
            GetObjectMetadataRequest request = new GetObjectMetadataRequest();
            request.BucketName = bucketName;
            request.Key = key;
            bool ok = false;

            try
            {
                GetObjectMetadataResponse response = client.GetObjectMetadata(request);

                if (response != null)
                {
                    ok = (response.HttpStatusCode == HttpStatusCode.OK);
                }
            }
            catch (AmazonS3Exception e)
            {
                if (e.Message == "The specified key does not exist")
                {
                    ok = false;

                }
                else
                {
                    throw;
                }
            }

            return ok;
        }



    }
}