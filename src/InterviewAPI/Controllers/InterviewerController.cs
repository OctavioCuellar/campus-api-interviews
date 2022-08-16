using System;
using System.Threading.Tasks;
using InterviewAPI.DTOs;
using InterviewAPI.Repositories.Abstractions;
using InterviewAPI.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace InterviewAPI.Controllers
{
    [ApiController]
    [Route("api/Interviewer")]
    public class InterviewerController : ControllerBase
    {
        private readonly IInterviewerService _interviewerService;

        public InterviewerController(IInterviewerService interviewerService)
        {
            _interviewerService = interviewerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetInterviewers()
        {
            var interviewers = await _interviewerService.GetInterviewers();
            return Ok(interviewers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInterviewersById(int id)
        {
            var interviewer = await _interviewerService.GetInterviewersById(id);
            if (interviewer is null)
                return NotFound();
            return Ok(interviewer);
        }

        [HttpPost]
        public async Task<IActionResult> CreateInterviewer(InterviewerWriteDto interviewerWriteDto)
        {
            var interviewer = await _interviewerService.CreateInterviewer(interviewerWriteDto);

            return Created(Request.Path, interviewer);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInterviewer(int id, InterviewerUpdateDto interviewerUpdateDto)
        {
            var interviewer = await _interviewerService.UpdateInterviewer(id, interviewerUpdateDto);
            if (interviewer is null)
                return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInterviewer(int id)
        {
            try
            {
                await _interviewerService.DeleteInterviewer(id);
                return NoContent();
            }
            catch (NullReferenceException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}