using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Jeeves.Server.Domain;
using Jeeves.Server.Representations;
using Jeeves.Server.Representations.Requests;
using Jeeves.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Jeeves.Server.Controllers
{
    [ApiController]
    [Route("/api/v1/challenges")]
    public class ChallengesController : ControllerBase
    {
        private readonly IChallengesService _challengesService;
        private readonly IMapper _mapper;
        
        public ChallengesController(IChallengesService challengesService, IMapper mapper)
        {
            _mapper = mapper;
            _challengesService = challengesService;
        }
        
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<JeevesChallenge[]>> GetAll()
        {
            var challenges = (await _challengesService.GetChallengesAsync()).Select(c => _mapper.Map<JeevesChallenge>(c)).ToArray();

            if (challenges.Length == 0)
                return NoContent();

            return challenges;
        }
        
        [HttpPost]
        [Route("")]
        public async Task<ActionResult> Create([FromBody] CreateChallenge request)
        {
            var challenge = await _challengesService.CreateChallengeAsync(new Challenge
            {
                Name = request.Name
            });

            return CreatedAtAction(nameof(GetById), new { id = challenge.Id }, challenge);
        }
        
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<JeevesChallenge>> GetById([FromRoute] Guid id)
        {
            var challenge = await _challengesService.FindChallengeAsync(id);

            if (challenge == null)
                return NotFound();

            return _mapper.Map<JeevesChallenge>(challenge);
        }
        
        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> DeleteById([FromRoute] Guid id)
        {
            var challenge = await _challengesService.FindChallengeAsync(id);

            if (challenge == null)
                return NotFound();
            
            await _challengesService.DeleteChallengeAsync(id);
            return NoContent();
        }
    }
}