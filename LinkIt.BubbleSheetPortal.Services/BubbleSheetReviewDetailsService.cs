using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.BubbleSheetReview;
using LinkIt.BubbleSheetPortal.Models.Interfaces;
using LinkIt.BubbleSheetPortal.Services.CommonServices;
using S3Library;


namespace LinkIt.BubbleSheetPortal.Services
{
    public class BubbleSheetReviewDetailsService
    {
        private readonly IReadOnlyRepository<BubbleSheetReviewDetails> repository;
        private readonly IBubbleSheetReviewDetailsRepository detailRepository;
        private readonly IBubbleSheetClassViewAutoSaveRepository autoSaverepository;
        private readonly IBubbleSheetFileRepository bubbleSheetFileRepository;
        private readonly IBubbleSheetFileSubRepository bubbleSheetFileSubRepository;
        private readonly IS3Service _s3Service;
        private readonly BubbleSheetCommonService bubbleSheetCommonService;

        private static readonly string[] SupportedImageExtensions = { ".png", ".jpg", ".jpeg", ".jpe", ".jfif", ".gif", ".tiff", ".tif" };

        public BubbleSheetReviewDetailsService(
            IReadOnlyRepository<BubbleSheetReviewDetails> repository,
            IBubbleSheetReviewDetailsRepository detailRepository,
            IBubbleSheetClassViewAutoSaveRepository autoSaverepository,
            IBubbleSheetFileRepository bubbleSheetFileRepository,
            IBubbleSheetFileSubRepository bubbleSheetFileSubRepository,
            BubbleSheetCommonService bubbleSheetCommonService,
            IS3Service s3Service)
        {
            this.repository = repository;
            this.detailRepository = detailRepository;
            this.autoSaverepository = autoSaverepository;
            this.bubbleSheetFileRepository = bubbleSheetFileRepository;
            this.bubbleSheetFileSubRepository = bubbleSheetFileSubRepository;
            this._s3Service = s3Service;
            this.bubbleSheetCommonService = bubbleSheetCommonService;
        }

        public BubbleSheetReviewDetails GetBubbleSheetReviewDetailsByTicketStudentIdAndClassId(string ticket, int studentId, int classId)
        {
            return GetAllBubbleSheetReviewDetailsForStudentByTicketAndClassId(studentId, ticket, classId).FirstOrDefault();
        }

        public IQueryable<BubbleSheetReviewDetails> GetAllBubbleSheetReviewDetailsForStudentByTicketAndClassId(int studentId, string ticket, int classId)
        {
            return repository.Select().Where(x => x.Ticket.Equals(ticket) && x.StudentId.Equals(studentId) && x.ClassId.Equals(classId)).OrderByDescending(x => x.BubbleSheetFileId);
        }

        public IQueryable<BubbleSheetReviewDetails> GetLatestBubbleSheetReviewDetailsForStudentListByTicketAndClassId(
            List<int> studentIdList, string ticket, int classId)
        {
            return repository.Select()
                .Where(x =>
                    x.Ticket.Equals(ticket)
                    && x.ClassId.Equals(classId)
                    && studentIdList.Contains(x.StudentId))
                .GroupBy(x => x.StudentId, (key, g) => g.OrderByDescending(y => y.BubbleSheetFileId).First());
        }

        public IQueryable<BubbleSheetReviewDetails> GetAllBubbleSheetReviewDetailsForStudentListByTicketAndClassId(
            List<int> studentIdList, string ticket, int classId)
        {
            return repository.Select()
                .Where(x =>
                    x.Ticket.Equals(ticket) 
                    && studentIdList.Contains(x.StudentId) 
                    && x.ClassId.Equals(classId)).OrderByDescending(x => x.BubbleSheetFileId);
        }

        public BubbleSheetReviewUploadArtifactFileResponse UploadArtifactFile(BubbleSheetReviewUploadArtifactFileModel request, Func<MemoryStream, int?, bool, List<byte[]>> convertPdfToImage)
        {
            var response = new BubbleSheetReviewUploadArtifactFileResponse();

            if (request.PostedFile == null)
            {
                response.Message = "The file is required";
                return response;
            }

            var extension = Path.GetExtension(request.PostedFile.FileName).ToLowerInvariant();
            if (!SupportedImageExtensions.Contains(extension) && extension != ".pdf")
            {
                response.Message = "The uploaded file format is not supported. Only PDF, PNG, JPG, JPEG, GIF, and TIFF formats are allowed.";
                return response;
            }

            var inputFileName = bubbleSheetCommonService.BuildInputFileName(request.PostedFile.FileName);
            var outputFileName = string.Format("{0}_artifactImage.png", Guid.NewGuid().ToByteArray().HexEncode());
            var outputFilePNG = ConvertToSinglePng(request.PostedFile, extension, convertPdfToImage);

            var result = _s3Service.UploadRubricFile(request.BucketName, $"{request.BubbleSheetFolder}/originalfile/{inputFileName}", request.PostedFile.InputStream);
            if (result == null && !result.IsSuccess)
            {
                return response;
            }

            using (Stream fileStream = new MemoryStream(outputFilePNG))
            {
                result = _s3Service.UploadRubricFile(request.BucketName, $"{request.BubbleSheetFolder}/testimage/{outputFileName}", fileStream);
                if (result == null && !result.IsSuccess)
                {
                    return response;
                }
            }

            var artifactBubbleSheetFile = bubbleSheetFileRepository.Select().FirstOrDefault(x => x.BubbleSheetId == request.BubbleSheetID && x.PageNumber == -1);
            if (artifactBubbleSheetFile != null)
            {
                var artifactBubbleSheetFileSub = bubbleSheetFileSubRepository.Select().FirstOrDefault(x => x.BubbleSheetFileId == artifactBubbleSheetFile.BubbleSheetFileId && x.PageNumber == -1);
                if (artifactBubbleSheetFileSub != null)
                {
                    bubbleSheetFileSubRepository.Delete(artifactBubbleSheetFileSub);
                }
                bubbleSheetFileRepository.Delete(artifactBubbleSheetFile);
            }

            var newArtifactBubbleSheetFile = new BubbleSheetFile
            {
                BubbleSheetId = request.BubbleSheetID,
                Barcode1 = string.Empty,
                Barcode2 = string.Empty,
                InputFilePath = string.Empty,
                Resolution = "0",
                PageNumber = -1,
                FileDisposition = "Readable",
                OutputFileName = outputFileName,
                ProcessingTime = 0,
                RosterPosition = 0,
                ResultCount = 0,
                ResultString = string.Empty,
                DistrictId = request.DistrictID,
                IsConfirmed = false,
                IsUnmappedGeneric = false,
                Date = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow,
                InputFileName = inputFileName,
                UserId = request.CurrentUserID
            };
            bubbleSheetFileRepository.Save(newArtifactBubbleSheetFile);

            var newArtifactBubbleSheetFileSub = new BubbleSheetFileSub
            {
                BubbleSheetFileId = newArtifactBubbleSheetFile.BubbleSheetFileId,
                InputFilePath = inputFileName,
                InputFileName = string.Empty,
                Resolution = "0",
                PageNumber = -1,
                OutputFileName = outputFileName,
                CreateDate = DateTime.UtcNow
            };
            bubbleSheetFileSubRepository.Save(newArtifactBubbleSheetFileSub);

            response.IsSucceed = true;
            response.SubFileName = outputFileName;
            return response;
        }

        public BubbleSheetReviewDeleteArtifactFileResponse DeleteArtifactFile(BubbleSheetReviewDeleteArtifactFileRequest request)
        {
            var response = new BubbleSheetReviewDeleteArtifactFileResponse();
            response.IsSucceed = true;

            var artifactBubbleSheetFile = bubbleSheetFileRepository.Select()
                .FirstOrDefault(x => x.BubbleSheetId == request.BubbleSheetID && x.PageNumber == -1);
            if (artifactBubbleSheetFile == null)
            {
                return response;
            }

            var artifactBubbleSheetFileSub = bubbleSheetFileSubRepository.Select().FirstOrDefault(x => x.BubbleSheetFileId == artifactBubbleSheetFile.BubbleSheetFileId && x.PageNumber == -1);
            if (artifactBubbleSheetFileSub != null)
            {
                bubbleSheetFileSubRepository.Delete(artifactBubbleSheetFileSub);
            }

            bubbleSheetFileRepository.Delete(artifactBubbleSheetFile);

            return response;
        }

        

        private byte[] ConvertToSinglePng(HttpPostedFileBase postedFile, string extension, Func<MemoryStream, int?, bool, List<byte[]>> convertPdfToImage)
        {
            using (var inputStream = new MemoryStream())
            {
                postedFile.InputStream.CopyTo(inputStream);
                inputStream.Position = 0;

                using (var outputStream = new MemoryStream())
                {
                    if (extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                    {
                        var byteArrays = convertPdfToImage(inputStream, null, true);
                        var pageImages = ImageHelper.ConvertByteArraysToImages(byteArrays);
                        using (var combinedImage = ImageHelper.CombineImagesVertically(pageImages))
                        {
                            combinedImage.Save(outputStream, ImageFormat.Png);
                        }
                    }
                    else
                    {
                        using (var img = Image.FromStream(inputStream))
                        {
                            img.Save(outputStream, ImageFormat.Png);
                        }
                    }

                    postedFile.InputStream.Position = 0;
                    return outputStream.ToArray();
                }
            }
        }

        public List<BubbleSheetClassViewAnswer> GetBubbleSheetClassViewAnswerData(string studentIdList, string ticket, int classId)
        {
            return detailRepository.GetBubbleSheetClassViewAnswerData(studentIdList, ticket, classId);
        }

        public void AutoSavedClassView(BubbleSheetClassViewAutoSave item)
        {
            autoSaverepository.Save(item);
        }
        public void DeleteAutoSavedData(BubbleSheetClassViewAutoSave item)
        {
            autoSaverepository.Delete(item);
        }
        public void DeleteAllAutoSavedData(string ticket, int? classId)
        {
            autoSaverepository.BubbleSheetDeleteAllAutoSaveData(ticket, classId.GetValueOrDefault());
        }
        public BubbleSheetClassViewAutoSave GetAutoSavedData (string ticket, int classId, int userId)
        {
            return
                autoSaverepository.Select()
                    .Where(x => x.Ticket == ticket && x.ClassId == classId && x.UserId == userId)
                    .FirstOrDefault();
        }
    }
}
