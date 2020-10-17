using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Jeeves.Server.Representations;
using Jeeves.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Jeeves.Server.Controllers
{
    [ApiController]
    [Route("/api/v1/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;
        private readonly IMapper _mapper;
        
        public UsersController(IUsersService usersService, IMapper mapper)
        {
            _usersService = usersService;
            _mapper = mapper;
        }
        
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<JeevesUser[]>> GetAll()
        {
            var users = (await _usersService.GetUsersAsync()).Select(u => _mapper.Map<JeevesUser>(u)).ToArray();

            if (users.Length == 0)
                return NoContent();

            return users;
        }
        
        // [HttpPost]
        // [Route("")]
        // public ActionResult Create([FromBody] CreateUser request)
        // {
        //     return NoContent();
        // }
        
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<JeevesUser>> GetById([FromRoute] Guid id)
        {
            var user = await _usersService.FindUserAsync(id);

            if (user == null)
                return NotFound();

            return _mapper.Map<JeevesUser>(user);
        }
        
        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> DeleteById([FromRoute] Guid id)
        {
            var user = await _usersService.FindUserAsync(id);

            if (user == null)
                return NotFound();
            
            await _usersService.DeleteUserAsync(id);
            return NoContent();
        }
    }
}