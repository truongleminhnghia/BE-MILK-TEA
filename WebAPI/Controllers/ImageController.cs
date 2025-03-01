// using System;
// using System.Collections.Generic;
// using System.Net;
// using System.Threading.Tasks;
// using AutoMapper;
// using Business_Logic_Layer.Models;
// using Business_Logic_Layer.Models.Requests;
// using Business_Logic_Layer.Models.Responses;
// using Business_Logic_Layer.Services;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Logging;

// namespace WebAPI.Controllers
// {
//     [Route("api/v1/images")]
//     [ApiController]
//     public class ImageController : ControllerBase
//     {
//         private readonly IImageService _imageService;
//         private readonly IMapper _mapper;
//         private readonly ILogger<ImageController> _logger;

//         public ImageController(
//             IImageService imageService,
//             IMapper mapper,
//             ILogger<ImageController> logger = null
//         )
//         {
//             _imageService = imageService;
//             _mapper = mapper;
//             _logger = logger;
//         }

//         [HttpGet]
//         public async Task<ActionResult> GetAllImages()
//         {
//             var images = await _imageService.GetAllImagesAsync();
//             var imageResponses = _mapper.Map<IEnumerable<ImageRespone>>(images);
//             return Ok(new ApiResponse((int)HttpStatusCode.OK, true, "Thành công", imageResponses));
//         }

//         [HttpGet("{id}")]
//         public async Task<ActionResult> GetImageById(Guid id)
//         {
//             var image = await _imageService.GetImageByIdAsync(id);
//             if (image == null)
//             {
//                 _logger?.LogWarning("Không tìm thấy hình ảnh có ID {ImageId}", id);
//                 return NotFound(
//                     new ApiResponse(
//                         (int)HttpStatusCode.NotFound,
//                         false,
//                         $"Không tìm thấy hình ảnh với ID {id}"
//                     )
//                 );
//             }
//             var imageResponse = _mapper.Map<ImageRespone>(image);
//             return Ok(new ApiResponse((int)HttpStatusCode.OK, true, "Thành công", imageResponse));
//         }

//         //Add Image
//         [HttpPost]
//         [Authorize("ROLE_STAFF")]
//         public async Task<ActionResult> AddImage([FromBody] ImageRequest imageRequest)
//         {
//             if (imageRequest == null)
//             {
//                 return BadRequest(
//                     new ApiResponse((int)HttpStatusCode.BadRequest, false, "Data không hợp lệ")
//                 );
//             }
//             if (!ModelState.IsValid)
//             {
//                 return BadRequest(
//                     new ApiResponse(
//                         (int)HttpStatusCode.BadRequest,
//                         false,
//                         "Dữ liệu không đúng định dạng"
//                     )
//                 );
//             }
//             var imageResponse = _mapper.Map<ImageRespone>(imageRequest);
//             await _imageService.AddImageAsync(imageResponse);
//             return CreatedAtAction(
//                 nameof(GetImageById),
//                 new { id = imageResponse.Id },
//                 new ApiResponse(
//                     (int)HttpStatusCode.Created,
//                     true,
//                     "Thêm hình ảnh thành công",
//                     imageRequest
//                 )
//             );
//         }

//         //Update Image
//         [HttpPut("{id}")]
//         [Authorize("ROLE_STAFF")]
//         public async Task<IActionResult> UpdateImage(Guid id, [FromBody] ImageRequest imageRequest)
//         {
//             if (imageRequest == null)
//             {
//                 return BadRequest(
//                     new ApiResponse((int)HttpStatusCode.BadRequest, false, "Dữ liệu không hợp lệ")
//                 );
//             }
//             if (!ModelState.IsValid)
//             {
//                 return BadRequest(
//                     new ApiResponse(
//                         (int)HttpStatusCode.BadRequest,
//                         false,
//                         "Dữ liệu không đúng định dạng"
//                     )
//                 );
//             }
//             var existingImage = await _imageService.GetImageByIdAsync(id);
//             if (existingImage == null)
//             {
//                 _logger?.LogWarning("Cố gắng cập nhật hình ảnh không tồn tại với ID {ImageId}", id);
//                 return NotFound(
//                     new ApiResponse(
//                         (int)HttpStatusCode.NotFound,
//                         false,
//                         $"Không tìm thấy hình ảnh với ID {id}"
//                     )
//                 );
//             }
//             // Map and update
//             var imageResponse = _mapper.Map<ImageRespone>(imageRequest);
//             await _imageService.UpdateImageAsync(id, imageResponse);
//             return Ok(
//                 new ApiResponse((int)HttpStatusCode.OK, true, "Cập nhật hình ảnh thành công")
//             );
//         }

//         //Delete Image
//         [HttpDelete("{id}")]
//         [Authorize("ROLE_STAFF")]
//         public async Task<ActionResult> DeleteImage(Guid id)
//         {
//             var existingImage = await _imageService.GetImageByIdAsync(id);
//             if (existingImage == null)
//             {
//                 _logger?.LogWarning("Cố gắng xóa hình ảnh không tồn tại với ID {ImageId}", id);
//                 return NotFound(
//                     new ApiResponse(
//                         (int)HttpStatusCode.NotFound,
//                         false,
//                         $"Không tìm thấy hình ảnh với ID {id}"
//                     )
//                 );
//             }
//             await _imageService.DeleteImageAsync(id);
//             return Ok(new ApiResponse((int)HttpStatusCode.OK, true, "Xóa hình ảnh thành công"));
//         }
//     }
// }
