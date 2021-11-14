using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTO;
using API.Entities;
using API.Extensisons;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : BaseAPIController
    {
        public IUserRespository _userRespository;
        private readonly IMapper _mapper;
        public IPhotoService _photoService;

        public UsersController(IUserRespository userRespository, IMapper mapper, IPhotoService photoService)
        {
            _photoService = photoService;
            mapper = mapper;
            _userRespository = userRespository;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers()
        {
            return Ok(await _userRespository.GetMembersAsync());
        }

        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDTO>> GetUsers(string username)
        {
            var user = await _userRespository.GetMemberAsync(username);
            return user;
        }
        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDTO)
        {
            var username = User.GetUserName();
            var user = await _userRespository.GetUserByUsernameAsync(username);
            // conver data update to data to save into database
            user.City = memberUpdateDTO.City;
            user.Country = memberUpdateDTO.Country;
            user.LookingFor = memberUpdateDTO.LookingFor;
            user.Interests = memberUpdateDTO.Interests;
            user.KnowAs = memberUpdateDTO.KnowAs;
            user.Introduction = memberUpdateDTO.Introduction;
            _userRespository.Update(user);
            if (await _userRespository.SaveAllAsync()) return NoContent();
            return BadRequest("Failed to update user");
        }
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file)
        {
            var username = User.GetUserName();
            var user = await _userRespository.GetUserByUsernameAsync(username);
            var result = await _photoService.AddphotoAsync(file);
            if (result.Error != null) return BadRequest(result.Error.Message);
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };
            if (user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }
            user.Photos.Add(photo);
            if (await _userRespository.SaveAllAsync())
            {
                PhotoDTO obj = new PhotoDTO();
                obj.Id = photo.Id;
                obj.Url = photo.Url;
                obj.IsMain = photo.IsMain;
                // return CreatedAtRoute("GetUser", obj);
                return CreatedAtRoute("GetUser", new { username = user.UserName }, obj);
            }
            return BadRequest("Problem add image");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _userRespository.GetUserByUsernameAsync(User.GetUserName());
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo.IsMain) return BadRequest("This is already your main photo");

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if (currentMain != null) currentMain.IsMain = false;

            photo.IsMain = true;

            if (await _userRespository.SaveAllAsync()) return NoContent();
            return BadRequest("Failed to set main photo");
        }
        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> Deletephoto(int photoId)
        {
            var user = await _userRespository.GetUserByUsernameAsync(User.GetUserName());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null) return NotFound();

            if (photo.IsMain) return BadRequest("You can not delete main photo");

            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);

                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);

            if (await _userRespository.SaveAllAsync()) return Ok();

            return BadRequest("Failer when delete photo");

        }
    }
}