using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTO;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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

        public UsersController(IUserRespository userRespository, IMapper mapper)
        {
            mapper = mapper;
            _userRespository = userRespository;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers()
        {
            return Ok(await _userRespository.GetMembersAsync());
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDTO>> GetUsers(string username)
        {
            var user = await _userRespository.GetMemberAsync(username);
            return user;
        }
        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDTO)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userRespository.GetUserByUsernameAsync(username);
            // conver data update to data to save into database
            user.City = memberUpdateDTO.City;
            user.Country = memberUpdateDTO.Country;
            user.LookingFor = memberUpdateDTO.LookingFor;
            user.Interests = memberUpdateDTO.Interests;
            user.KnowAs = memberUpdateDTO.KnowAs;
            user.Introduction = memberUpdateDTO.Introduction;
            _userRespository.Update(user);
          if(await _userRespository.SaveAllAsync()) return NoContent();
          return BadRequest("Failed to update user");
        }
    }
}