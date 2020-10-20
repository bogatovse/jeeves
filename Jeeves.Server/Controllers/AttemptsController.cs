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
    [Route("/api/v1/challenges/{challengeId}/attempts")]
    public class AttemptsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAttemptsService _attemptsService;
        
        public AttemptsController(IAttemptsService attemptsService, IMapper mapper)
        {
            _mapper = mapper;
            _attemptsService = attemptsService;
        }
        
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<JeevesAttempt[]>> GetAll()
        {
            var attempts = (await _attemptsService.GetAttemptsAsync()).Select(a => _mapper.Map<JeevesAttempt>(a)).ToArray();

            if (attempts.Length == 0)
                return NoContent();

            return attempts;
        }
        
        [HttpPost]
        [Route("")]
        public async Task<ActionResult> Create([FromRoute] Guid challengeId, [FromBody] CreateAttempt request)
        {
            var attempt = await _attemptsService.CreateAttemptAsync(new ChallengeAttempt
            {
                Solution = request.Solution
            });

            return CreatedAtAction(nameof(GetById), new { challengeId = challengeId, id = attempt.Id }, attempt);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<JeevesAttempt>> GetById([FromRoute] Guid id)
        {
            var attempt = await _attemptsService.FindAttemptAsync(id);

            if (attempt == null)
                return NotFound();

            return _mapper.Map<JeevesAttempt>(attempt);
        }
        
        [HttpGet]
        [Route("{id}/stop")]
        public async Task<ActionResult> StopById([FromRoute] Guid id)
        {
            var attempt = await _attemptsService.FindAttemptAsync(id);

            if (attempt == null)
                return NotFound();

            //TODO: We shouldn't block client. Consider to use queue for that and Accepted status code
            await _attemptsService.StopAttemptAsync(id);
            return Ok();
        }
        
        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> DeleteById([FromRoute] Guid id)
        {
            var attempt = await _attemptsService.FindAttemptAsync(id);

            if (attempt == null)
                return NotFound();

            await _attemptsService.DeleteAttemptAsync(id);
            return NoContent();
        }
    }
}