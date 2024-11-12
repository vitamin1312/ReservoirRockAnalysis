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

namespace CsharpBackend
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoreSampleImagesController : ControllerBase
    {
        private readonly IImageRepository repository;
        private int MaxFileLength = 15 * 1024 * 1024;
        private INetworkManager _networkManager;

        public CoreSampleImagesController(IImageRepository context, INetworkManager networkManager)
        {
            repository = context;
            _networkManager = networkManager;
        }

        [HttpPost]
        [Route("upload")]
        async public Task<ActionResult<CoreSampleImage>> UploadImage(IFormFile file,
            [FromForm] ImageInfoDTO imageInfoDTO)
        {

            if (file.Length > MaxFileLength)
                return StatusCode(StatusCodes.Status413PayloadTooLarge,
                    new { message = "File is too large" });

            if (file.ContentType != "image/jpeg" && file.ContentType != "image/png")
                return StatusCode(StatusCodes.Status415UnsupportedMediaType,
                    new { message = "Unsupported media type" });

            if (file == null)
                return NoContent();

            var coreSampleImage = new CoreSampleImage();

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

            return PhysicalFile(coreSampleImage.PathToImage, "image/jpeg");



        }

        [HttpGet("getmaskfile/{id}")]
        public async Task<ActionResult<IEnumerable<CoreSampleImage>>> GetMaskFile(int id)
        {
            var coreSampleImage = await repository.GetImage(id);

            if (coreSampleImage == null || coreSampleImage.PathToMask == null)
                return NotFound();

            return PhysicalFile(coreSampleImage.PathToMask, "image/png");
        }


        [HttpGet("getmaskimagefile/{id}")]
        public async Task<ActionResult<IEnumerable<CoreSampleImage>>> GetMaskImageFile(int id)
        {
            var coreSampleImage = await repository.GetImage(id);

            if (coreSampleImage == null)
                return NotFound();

            var maskImage = coreSampleImage.GetMaskImageMat();

            if (maskImage == null)
                return NotFound();

            var tempFiles = new TempFileCollection();
            string file = tempFiles.AddExtension("jpg");
            maskImage.Save(file);
            return PhysicalFile(file, "image/jpeg");
        }

        [HttpGet("getimagewithmaskfile/{id}")]
        public async Task<ActionResult<IEnumerable<CoreSampleImage>>> GetImageWithMaskFile(int id)
        {
            var coreSampleImage = await repository.GetImage(id);

            if (coreSampleImage == null)
                return NotFound();

            var maskImage = coreSampleImage.GetImageWithMaskMat();

            if (maskImage == null)
                return NotFound();

            var tempFiles = new TempFileCollection();
            string file = tempFiles.AddExtension("jpg");
            maskImage.Save(file);
            return PhysicalFile(file, "image/jpeg");
        }

        [HttpGet("predict/{id}")]
        public async Task<ActionResult<CoreSampleImage>> PredictMaskForItem(int id)
        {
            var coreSampleImage = await repository.GetImage(id);

            if (coreSampleImage == null)
            {
                return NotFound();
            }
            
            var pathToMask = coreSampleImage.GenerateMaskPath();

            _networkManager.Predict(coreSampleImage.PathToImage, pathToMask);
            coreSampleImage.PathToMask = pathToMask;

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
        public async Task<IActionResult> PutImageInfo(int id, [FromForm] ImageInfoDTO imageInfoDTO)
        {
/*
            if (id != imageInfoDTO.Id)
            {
                return BadRequest();
            }
*/
            var imageInfo = new ImageInfo(imageInfoDTO);

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
