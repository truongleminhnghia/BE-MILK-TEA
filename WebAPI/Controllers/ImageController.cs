using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business_Logic_Layer.Models;
using Business_Logic_Layer.Services;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebAPI.Controllers
{
    [Route("api/v1/images")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly IImageRepository _imageRepository;

        public ImageController(IImageService imageService, IImageRepository imageRepository)
        {
            _imageService = imageService;
            _imageRepository = imageRepository;
        }

        [HttpGet]
        public async Task<
            ActionResult<IEnumerable<Business_Logic_Layer.Models.Image>>
        > GetAllImages()
        {
            var images = await _imageService.GetAllImagesAsync();
            return Ok(images);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Business_Logic_Layer.Models.Image>> GetImageById(Guid id)
        {
            var image = await _imageService.GetImageByIdAsync(id);
            if (image == null)
            {
                return NotFound();
            }
            return Ok(image);
        }

        [HttpPost]
        public async Task<ActionResult> AddImage([FromBody] Business_Logic_Layer.Models.Image image)
        {
            if (image == null)
            {
                return BadRequest(new { message = "Invalid image data" });
            }

            // Generate new Id for the image
            image.Id = Guid.NewGuid();

            await _imageService.AddImageAsync(image);
            return CreatedAtAction(nameof(GetImageById), new { id = image.Id }, image);
        }

        [HttpPut("{id}")]
        public async Task UpdateImageAsync(Guid id, Business_Logic_Layer.Models.Image image)
        {
            var existingImage = await _imageRepository.GetImageByIdAsync(id);
            if (existingImage == null)
            {
                throw new Exception("Image not found.");
            }

            // Update the existing image's properties
            existingImage.ImageUrl = image.ImageUrl;
            existingImage.IngredientId = image.IngredientId;

            await _imageRepository.UpdateImageAsync(existingImage);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteImage(Guid id)
        {
            await _imageService.DeleteImageAsync(id);
            return NoContent();
        }
    }
}
