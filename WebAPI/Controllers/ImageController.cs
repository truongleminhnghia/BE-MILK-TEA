using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business_Logic_Layer.Models;
using Business_Logic_Layer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebAPI.Controllers
{
    [Route("api/v1/images")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Image>>> GetAllImages()
        {
            var images = await _imageService.GetAllImagesAsync();
            return Ok(images);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Image>> GetImageById(Guid id)
        {
            var image = await _imageService.GetImageByIdAsync(id);
            if (image == null)
            {
                return NotFound();
            }
            return Ok(image);
        }

        [HttpPost]
        public async Task<ActionResult> AddImage([FromBody] Image imageModel)
        {
            await _imageService.AddImageAsync(imageModel);
            return CreatedAtAction(nameof(GetImageById), new { id = imageModel.Id }, imageModel);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateImage(Guid id, [FromBody] Image imageModel)
        {
            if (id != imageModel.Id)
            {
                return BadRequest();
            }

            await _imageService.UpdateImageAsync(imageModel);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteImage(Guid id)
        {
            await _imageService.DeleteImageAsync(id);
            return NoContent();
        }
    }
}
