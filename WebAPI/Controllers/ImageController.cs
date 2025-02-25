using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Business_Logic_Layer.Models;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebAPI.Controllers
{
    [Route("api/v1/images")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;
        private readonly ILogger<ImageController> _logger;

        public ImageController(
            IImageService imageService,
            IMapper mapper,
            ILogger<ImageController> logger = null
        )
        {
            _imageService = imageService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllImages()
        {
            try
            {
                var images = await _imageService.GetAllImagesAsync();
                var imageResponses = _mapper.Map<IEnumerable<ImageRespone>>(images);
                return Ok(new ApiResponse(HttpStatusCode.OK, true, "Thành công", imageResponses));
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Lỗi truy xuất tất cả hình ảnh");
                return StatusCode(
                    500,
                    new ApiResponse(
                        HttpStatusCode.InternalServerError,
                        false,
                        "Đã xảy ra lỗi khi lấy danh sách hình ảnh"
                    )
                );
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetImageById(Guid id)
        {
            try
            {
                var image = await _imageService.GetImageByIdAsync(id);
                if (image == null)
                {
                    _logger?.LogWarning("Không tìm thấy hình ảnh có ID {ImageId}", id);
                    return NotFound(
                        new ApiResponse(
                            HttpStatusCode.NotFound,
                            false,
                            $"Không tìm thấy hình ảnh với ID {id}"
                        )
                    );
                }

                var imageResponse = _mapper.Map<ImageRespone>(image);
                return Ok(new ApiResponse(HttpStatusCode.OK, true, "Thành công", imageResponse));
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Lỗi truy xuất hình ảnh có ID {ImageId}", id);
                return StatusCode(
                    500,
                    new ApiResponse(
                        HttpStatusCode.InternalServerError,
                        false,
                        $"Đã xảy ra lỗi khi lấy hình ảnh với ID {id}"
                    )
                );
            }
        }

        //Add Image
        [HttpPost]
        [Authorize("ROLE_STAFF")]
        public async Task<ActionResult> AddImage([FromBody] ImageRequest imageRequest)
        {
            try
            {
                if (imageRequest == null)
                {
                    return BadRequest(
                        new ApiResponse(HttpStatusCode.BadRequest, false, "Data không hợp lệ")
                    );
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(
                        new ApiResponse(
                            HttpStatusCode.BadRequest,
                            false,
                            "Dữ liệu không đúng định dạng"
                        )
                    );
                }

                var imageResponse = _mapper.Map<ImageRespone>(imageRequest);
                await _imageService.AddImageAsync(imageResponse);

                return CreatedAtAction(
                    nameof(GetImageById),
                    new { id = imageResponse.Id },
                    new ApiResponse(
                        HttpStatusCode.Created,
                        true,
                        "Thêm hình ảnh thành công",
                        imageRequest
                    )
                );
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Lỗi thêm hình ảnh");
                return StatusCode(
                    500,
                    new ApiResponse(
                        HttpStatusCode.InternalServerError,
                        false,
                        "Đã xảy ra lỗi khi thêm hình ảnh: " + ex.Message
                    )
                );
            }
        }

        //Update Image
        [HttpPut("{id}")]
        [Authorize("ROLE_STAFF")]
        public async Task<IActionResult> UpdateImage(Guid id, [FromBody] ImageRequest imageRequest)
        {
            try
            {
                if (imageRequest == null)
                {
                    return BadRequest(
                        new ApiResponse(HttpStatusCode.BadRequest, false, "Dữ liệu không hợp lệ")
                    );
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(
                        new ApiResponse(
                            HttpStatusCode.BadRequest,
                            false,
                            "Dữ liệu không đúng định dạng"
                        )
                    );
                }

                var existingImage = await _imageService.GetImageByIdAsync(id);
                if (existingImage == null)
                {
                    _logger?.LogWarning(
                        "Cố gắng cập nhật hình ảnh không tồn tại với ID {ImageId}",
                        id
                    );
                    return NotFound(
                        new ApiResponse(
                            HttpStatusCode.NotFound,
                            false,
                            $"Không tìm thấy hình ảnh với ID {id}"
                        )
                    );
                }

                // Map and update
                var imageResponse = _mapper.Map<ImageRespone>(imageRequest);
                await _imageService.UpdateImageAsync(id, imageResponse);

                return Ok(new ApiResponse(HttpStatusCode.OK, true, "Cập nhật hình ảnh thành công"));
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Lỗi cập nhật hình ảnh với ID {ImageId}", id);
                return StatusCode(
                    500,
                    new ApiResponse(
                        HttpStatusCode.InternalServerError,
                        false,
                        $"Đã xảy ra lỗi khi cập nhật hình ảnh: {ex.Message}"
                    )
                );
            }
        }

        //Delete Image
        [HttpDelete("{id}")]
        [Authorize("ROLE_STAFF")]
        public async Task<ActionResult> DeleteImage(Guid id)
        {
            try
            {
                var existingImage = await _imageService.GetImageByIdAsync(id);
                if (existingImage == null)
                {
                    _logger?.LogWarning("Cố gắng xóa hình ảnh không tồn tại với ID {ImageId}", id);
                    return NotFound(
                        new ApiResponse(
                            HttpStatusCode.NotFound,
                            false,
                            $"Không tìm thấy hình ảnh với ID {id}"
                        )
                    );
                }

                await _imageService.DeleteImageAsync(id);
                return Ok(new ApiResponse(HttpStatusCode.OK, true, "Xóa hình ảnh thành công"));
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Lỗi xóa ảnh có ID {ImageId}", id);
                return StatusCode(
                    500,
                    new ApiResponse(
                        HttpStatusCode.InternalServerError,
                        false,
                        $"Đã xảy ra lỗi khi xóa hình ảnh: {ex.Message}"
                    )
                );
            }
        }
    }
}
