using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Business_Logic_Layer.Models;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services;
using Business_Logic_Layer.Services.CategoryService;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebAPI.Controllers
{
    [Route("api/v1/images")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;

        public ImageController(IImageService imageService, IMapper mapper)
        {
            _imageService = imageService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllImages()
        {
            var images = await _imageService.GetAllImagesAsync();
            var imageResponses = _mapper.Map<IEnumerable<ImageRespone>>(images);
            return Ok(new ApiResponse(HttpStatusCode.OK, true, "Thành công", imageResponses));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetImageById(Guid id)
        {
            var image = await _imageService.GetImageByIdAsync(id);
            var imageResponse = _mapper.Map<ImageRespone>(image);
            if (image == null)
            {
                return NotFound("Không Tìm Thấy Id");
            }
            return Ok(new ApiResponse(HttpStatusCode.OK, true, "Thành công", imageResponse));
        }

        //Create Image
        [HttpPost]
        [Authorize("ROLE_STAFF")]
        public async Task<ActionResult> AddImage([FromBody] ImageRequest imageRequest)
        {
            if (imageRequest == null)
            {
                return BadRequest(
                    new ApiResponse(HttpStatusCode.BadRequest, false, "Data không hợp lệ")
                );
            }

            var imageResponse = _mapper.Map<ImageRespone>(imageRequest);

            await _imageService.AddImageAsync(imageResponse);

            return CreatedAtAction(
                nameof(GetImageById),
                new { id = imageRequest.IngredientId },
                imageRequest
            );
        }

        //Update Image
        [HttpPut("{id}")]
        [Authorize("ROLE_STAFF")]
        public async Task<IActionResult> UpdateImage(Guid id, [FromBody] ImageRequest imageRequest)
        {
            if (imageRequest == null)
            {
                return BadRequest(
                    new ApiResponse(HttpStatusCode.BadRequest, false, "Dữ liệu không hợp lệ")
                );
            }

            // Kiểm tra hình ảnh tồn tại không
            var existingImage = await _imageService.GetImageByIdAsync(id);
            if (existingImage == null)
            {
                return NotFound(
                    new ApiResponse(HttpStatusCode.NotFound, false, "Không tìm thấy hình ảnh")
                );
            }

            // Cập nhật dữ liệu
            var imageResponse = _mapper.Map<ImageRespone>(imageRequest);

            // Gọi service cập nhật ảnh
            await _imageService.UpdateImageAsync(id, imageResponse);

            return Ok(new ApiResponse(HttpStatusCode.OK, true, "Cập nhật hình ảnh thành công"));
        }

        //Delete Image
        [HttpDelete("{id}")]
        [Authorize("ROLE_STAFF")]
        public async Task<ActionResult> DeleteImage(Guid id)
        {
            await _imageService.DeleteImageAsync(id);
            return NoContent();
        }
    }
}
