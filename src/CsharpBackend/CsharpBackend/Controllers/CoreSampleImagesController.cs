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

namespace CsharpBackend
{
    [EnableCors("MyPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class CoreSampleImagesController : ControllerBase
    {
        private readonly IImageRepository repository;
        private int MaxFileLength = 15 * 1024 * 1024;
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

        [HttpPost]
        [Route("uploadimagemask")]
        async public Task<ActionResult<CoreSampleImage>> UploadImageMask(IFormFile file, [FromForm] ImageInfoDTO imageInfoDTO)
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

            var thumbImage = DataConverter.ThumbImage(coreSampleImage.GetImageMat(), height);


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

            var maskImage = coreSampleImage.GetMaskImageMat();

            if (maskImage == null)
                return NotFound($"There is no item mask for this item");

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

            var imageWithMask = coreSampleImage.GetImageWithMaskMat();


            var file = TmpFiles.SaveMat(imageWithMask);
            return PhysicalFile(file, "image/png");
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

            _networkManager.Predict(coreSampleImage.PathToImage, coreSampleImage.PathToMask);

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
