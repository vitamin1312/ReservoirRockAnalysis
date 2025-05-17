using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CsharpBackend.Data;
using CsharpBackend.Models;
using CsharpBackend.NeuralNetwork;
using Emgu.CV.Structure;
using Emgu.CV;
using System.CodeDom.Compiler;
using Microsoft.AspNetCore.Authorization;
using CsharpBackend.Repository;
using CsharpBackend.Models.DTO;
using Microsoft.Extensions.Options;
using CsharpBackend.Utils;
using Microsoft.AspNetCore.Cors;
using CsharpBackend.Config;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using ClosedXML.Excel;

namespace CsharpBackend
{
    [EnableCors("MyPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class CoreSampleImagesController : ControllerBase
    {
        private readonly IImageRepository repository;
        private int MaxFileLength = 60 * 1024 * 1024;
        private INetworkManager _networkManager;
        private AppConfig _config;

        public CoreSampleImagesController(IImageRepository context, INetworkManager networkManager, IOptions<AppConfig> config)
        {
            repository = context;
            _networkManager = networkManager;
            _config = config.Value;
        }

        public ActionResult? ChechImageFile(IFormFile file)
        {
            if (file.Length > MaxFileLength)
                return StatusCode(StatusCodes.Status413PayloadTooLarge,
                    new { message = "File is too large" });

            if (file.ContentType != "image/png")
                return StatusCode(StatusCodes.Status415UnsupportedMediaType,
                    new { message = "Unsupported media type" });

            if (file == null)
                return NoContent();

            return null;
        }

        [HttpPost]
        [Route("upload")]
        async public Task<ActionResult<CoreSampleImage>> UploadImage(IFormFile file,
            [FromForm] ImageInfoDTO imageInfoDTO)
        {

            var validation_result = ChechImageFile(file);
            if (validation_result != null)
                return validation_result;

            var coreSampleImage = CoreSampleImage.WithImage(_config.PathToWWWROOT, Path.GetExtension(file.FileName));

            using (var stream = new FileStream(coreSampleImage.PathToImage, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var imageInfo = new ImageInfo(imageInfoDTO);

            checkFieldInInfo(imageInfo);

            coreSampleImage.ImageInfo = imageInfo;

            await repository.CreateImage(coreSampleImage);
            await repository.Save();
            return CreatedAtAction("GetCoreSampleImage", new { id = coreSampleImage.Id }, coreSampleImage);
            
        }

        [HttpPost]
        [Route("uploadmask")]
        async public Task<ActionResult<CoreSampleImage>> UploadMask(IFormFile file, [FromForm] ImageInfoDTO imageInfoDTO)
        {
            var validation_result = ChechImageFile(file);
            if (validation_result != null)
                return validation_result;

            var coreSampleImage = CoreSampleImage.WithMask(_config.PathToWWWROOT, Path.GetExtension(file.FileName));

            using (var stream = new FileStream(coreSampleImage.PathToMask, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var imageInfo = new ImageInfo(imageInfoDTO);

            checkFieldInInfo(imageInfo);

            coreSampleImage.ImageInfo = imageInfo;
            await repository.CreateImage(coreSampleImage);
            await repository.Save();
            return CreatedAtAction("GetCoreSampleImage", new { id = coreSampleImage.Id }, coreSampleImage);
        }

        [RequestSizeLimit(104857600)]
        [HttpPost]
        [Route("uploadimagemask")]
        async public Task<ActionResult<CoreSampleImage>> UploadImageMask(IFormFile file, [FromForm] ImageInfoDTO imageInfoDTO)
        {
            var validation_result = ChechImageFile(file);
            if (validation_result != null)
                return validation_result;

            var coreSampleImage = CoreSampleImage.WithMask(_config.PathToWWWROOT, Path.GetExtension(file.FileName));

            using var memstream = new MemoryStream();
            file.CopyTo(memstream);
            byte[] imageData = memstream.ToArray();

            Mat ImageMask = new();
            CvInvoke.Imdecode(imageData, ImreadModes.Unchanged, ImageMask);

            if (ImageMask == null || ImageMask.IsEmpty)
                throw new Exception("Can't decode image");

            var Mask = DataConverter.ImageMaskToMask(ImageMask);
            using var buf = new VectorOfByte();
            CvInvoke.Imencode(".png", Mask, buf);

            await System.IO.File.WriteAllBytesAsync(coreSampleImage.PathToMask, buf.ToArray());

            var imageInfo = new ImageInfo(imageInfoDTO);

            checkFieldInInfo(imageInfo);

            coreSampleImage.ImageInfo = imageInfo;
            await repository.CreateImage(coreSampleImage);
            await repository.Save();
            return CreatedAtAction("GetCoreSampleImage", new { id = coreSampleImage.Id }, coreSampleImage);
        }

        [HttpGet]
        [Route("get")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<CoreSampleImage>>> GetCoreSampleImage()
        {
            return await repository.GetImagesList();
        }

        
        [HttpGet("getitem/{id}")]
        public async Task<ActionResult<CoreSampleImage>> GetCoreSampleImage(int id)
        {
            var coreSampleImage = await repository.GetImage(id);

            if (coreSampleImage == null)
            {
                return NotFound();
            }

            return coreSampleImage;
        }

        [HttpGet("getfromfield/{fieldId}")]
        public async Task<ActionResult<IEnumerable<CoreSampleImage>>> GetCoreSampleImageFromField(int fieldId)
        {
            return await repository.GetCoreSampleImageFromField(fieldId);
        }

        [HttpGet("getwithmask")]
        public async Task<ActionResult<IEnumerable<CoreSampleImage>>> GetCoreSampleImageWithMask()
        {
            return await repository.GetCoreSampleImageWithMask();
        }

        [HttpGet("getwithoutmask")]
        public async Task<ActionResult<IEnumerable<CoreSampleImage>>> GetCoreSampleImageWithoutMask()
        {
            return await repository.GetCoreSampleImageWithoutMask();
        }


        [HttpGet("getimagefile/{id}")]
        public async Task<ActionResult<IEnumerable<CoreSampleImage>>> GetCoreSampleImageFile(int id)
        {
            var coreSampleImage = await repository.GetImage(id);

            if (coreSampleImage == null)
                return NotFound();

            if (coreSampleImage.PathToImage == null)
                return NotFound("There is no image for this item");

            return PhysicalFile(coreSampleImage.PathToImage, "image/png");
        }

        [HttpGet("getimagefilethumb/{id}")]
        public async Task<ActionResult<IEnumerable<CoreSampleImage>>> GetCoreSampleImageFileThumb(int id, int height)
        {
            var coreSampleImage = await repository.GetImage(id);

            if (coreSampleImage == null)
                return NotFound($"There is no item with id: {id}");

            if (coreSampleImage.PathToImage == null)
                return NotFound("There is no image for this item");
            var imageMat = DataConverter.GetImageMat(coreSampleImage.PathToImage);

            if (imageMat == null)
                return NotFound("Can't read image for this item");

            var thumbImage = DataConverter.ThumbImage(imageMat, height);


            var file = TmpFiles.SaveMat(thumbImage);
            return PhysicalFile(file, "image/png");
        }

        [HttpGet("getmaskfile/{id}")]
        public async Task<ActionResult<IEnumerable<CoreSampleImage>>> GetMaskFile(int id)
        {
            var coreSampleImage = await repository.GetImage(id);

            if (coreSampleImage == null)
                return NotFound($"There is no item with id: {id}");

            if (coreSampleImage.PathToMask == null)
                return NotFound("There is no mask for this item");

            return PhysicalFile(coreSampleImage.PathToMask, "image/png");
        }


        [HttpGet("getmaskimagefile/{id}")]
        public async Task<ActionResult<IEnumerable<CoreSampleImage>>> GetMaskImageFile(int id)
        {
            var coreSampleImage = await repository.GetImage(id);

            if (coreSampleImage == null)
                return NotFound($"There is no item with id: {id}");

            if (coreSampleImage.PathToMask == null || coreSampleImage.PathToMask == "")
                return NotFound($"There is no mask for this item");

            var maskImage = DataConverter.GetMaskImageMat(coreSampleImage.PathToMask);

            if (maskImage == null)
                return NotFound($"Can't load mask for this item");

            var file = TmpFiles.SaveMat(maskImage);
            return PhysicalFile(file, "image/png");
        }

        [HttpGet("getimagewithmaskfile/{id}")]
        public async Task<ActionResult<IEnumerable<CoreSampleImage>>> GetImageWithMaskFile(int id)
        {
            var coreSampleImage = await repository.GetImage(id);

            if (coreSampleImage == null)
                return NotFound($"There is no item with id: {id}");

            if (coreSampleImage.PathToImage == null)
                return NotFound($"There is image for this item");

            if (coreSampleImage.PathToMask == null)
                return NotFound($"There is no mask for this item");

            var imageWithMask = DataConverter.GetImageWithMaskMat(coreSampleImage.PathToImage, coreSampleImage.PathToMask);


            var file = TmpFiles.SaveMat(imageWithMask);
            return PhysicalFile(file, "image/png");
        }


        [HttpPost]
        [Route("poreinfo/{id}")]
        public async Task<ActionResult> GetPoresInfo(int id, [FromBody] double pixelLengthRatio)
        {
            var coreSampleImage = await repository.GetImage(id);

            if (coreSampleImage == null)
                return NotFound($"No item with id: {id}");

            if (string.IsNullOrEmpty(coreSampleImage.PathToMask))
                return NotFound("No mask available for this item");

            var mask = DataConverter.GetMaskMat(coreSampleImage.PathToMask);

            if (mask == null)
                return NotFound("Unable to read the mask for this item");

            var poresInfo = (await repository.GetImagePoresInfo(id)).ToList();

            if (poresInfo.Count == 0)
            {
                poresInfo = PorosityAnalyzer.CalculatePorosityInfo(mask, pixelLengthRatio, coreSampleImage.Id).ToList();
                await repository.AddPorosityInfo(poresInfo);
                await repository.Save();
            }

            using var workbook = new XLWorkbook();
            PorosityAnalyzer.ExportPoreInfoToWorksheet(workbook, poresInfo);
            var filePath = TmpFiles.SaveExcel(workbook);

            return PhysicalFile(
                filePath,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            );
        }


        [HttpGet("predict/{id}")]
        public async Task<ActionResult<CoreSampleImage>> PredictMaskForItem(int id)
        {
            var coreSampleImage = await repository.GetImage(id);

            if (coreSampleImage == null)
            {
                return NotFound($"There is no item with id: {id}");
            }

            if (coreSampleImage.PathToImage == null)
            {
                return NotFound($"There is no image for this item");
            }

            coreSampleImage.PathToMask = coreSampleImage.GenerateMaskPath(_config.PathToWWWROOT);

            await _networkManager.Predict(coreSampleImage.PathToImage, coreSampleImage.PathToMask);

            repository.UpdateImage(coreSampleImage);

            try
            {
                await repository.Save();
            }
            catch (DbUpdateConcurrencyException)
            {

                    throw;
            }

            return coreSampleImage;
        }



        // PUT: api/CoreSampleImages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("putitem/{id}")]
        [Authorize]
        public async Task<IActionResult> PutImageInfo(int id, [FromBody] ImageInfoDTO imageInfoDTO)
        {
/*
            if (id != imageInfoDTO.Id)
            {
                return BadRequest();
            }
*/
            var imageInfo = new ImageInfo(imageInfoDTO);
            imageInfo.Id = id;

            checkFieldInInfo(imageInfo);

            repository.UpdateInfo(imageInfo);

            try
            {
                await repository.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CoreSampleImageExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/CoreSampleImages/5
        [HttpDelete]
        [Route("deleteitem/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteCoreSampleImage(int id)
        {
            var coreSampleImage = await repository.GetImage(id);

            if (coreSampleImage == null)
            {
                return NotFound();
            }

            await repository.DeleteImage(id);
            await repository.Save();

            return NoContent();
        }

        private bool CoreSampleImageExists(int id)
        {
            return repository.CoreSampleImageExists(id);
        }

        private bool FieldExists(int id)
        {
            return repository.FieldExists(id);
        }

        private void checkFieldInInfo(ImageInfo imageInfo)
        {
            int fieldId = Convert.ToInt32(imageInfo.FieldId);
            if (imageInfo.FieldId != null && !FieldExists(fieldId))
                imageInfo.FieldId = null;
        }
    }
}
