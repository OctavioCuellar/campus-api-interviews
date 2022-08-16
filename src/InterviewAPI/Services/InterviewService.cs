using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using InterviewAPI.DTOs;
using InterviewAPI.Models;
using InterviewAPI.Repositories.Abstractions;
using InterviewAPI.Services.Abstractions;

namespace InterviewAPI.Services
{
    public class InterviewService : IInterviewService
    {
        private readonly IRepositoryWrapper _repoWrapper;
        private readonly IMapper _mapper;

        public InterviewService(IRepositoryWrapper wrapper, IMapper mapper)
        {
            _repoWrapper = wrapper;
            _mapper = mapper;
        }

        public async Task<IEnumerable<InterviewReadDto>> GetInterviews()
        {
            var interviews = await _repoWrapper.Interview.GetAll();
            var allInterviews = _mapper.Map<List<InterviewReadDto>>(interviews);

            return allInterviews;
        }

        public async Task<InterviewReadDto> GetInterviewById(int id)
        {
            var interview = await
                _repoWrapper
                    .Interview
                    .GetByCondition(i => i.Id == id);

            var mapInterview = _mapper.Map<List<InterviewReadDto>>(interview);

            return mapInterview.Count > 0 ? mapInterview.FirstOrDefault() : null;
        }

        public async Task<InterviewReadDto> CreateInterview(InterviewWriteDto interviewWriteDto)
        {
            int intervieweeId = interviewWriteDto.IntervieweeId;
            var interviewerIds = interviewWriteDto.InterviewerIds;

            var interviewee = await _repoWrapper
                .Interviewee
                .GetByCondition(i => i.Id.Equals(intervieweeId));
            
            var interviewers = await _repoWrapper
                .Interviewer
                .GetByCondition(i => interviewerIds.Contains(i.Id));
            
            Interview interview = new Interview()
            {
                Appointment = interviewWriteDto.Appointment,
                Name = interviewWriteDto.Name,
                Interviewee = interviewee.FirstOrDefault(),
                Interviewers = interviewers
            };
            
            _repoWrapper.Interview.Create(interview);
            await _repoWrapper.Save();
            
            var interviewReadDto = _mapper.Map<InterviewReadDto>(interview);
            
            return interviewReadDto;
        }

        public async Task<InterviewReadDto> UpdateInterview(int id, InterviewUpdateDto interviewUpdateDto)
        {
            int intervieweeId = interviewUpdateDto.IntervieweeId;
            var interviewerIds = interviewUpdateDto.InterviewerIds;

            var interviewees = await _repoWrapper
                .Interviewee
                .GetByCondition(i => i.Id.Equals(intervieweeId));
            
            var interviewers = await _repoWrapper
                .Interviewer
                .GetByCondition(i => interviewerIds.Any(ii => i.Id.Equals(ii)));
            
            var interviews = await _repoWrapper.Interview
                .GetByCondition(i => i.Id.Equals(id));

            var interviewee = interviewees.FirstOrDefault();
            var interview = interviews.FirstOrDefault();

            if (interview is null)
                return null;
            
            interview.Interviewee = interviewee;
            interview.Interviewers = interviewers;
            interview.Appointment = interviewUpdateDto.Appointment;
            interview.Name = interviewUpdateDto.Name;
            
            // var interviewToUpdate = _mapper.Map<InterviewUpdateDto>(interview);

            _repoWrapper.Interview.Update(interview);
            await _repoWrapper.Save();
            var interviewReadDto = _mapper.Map<InterviewReadDto>(interview);

            return interviewReadDto;
        }

        public async Task DeleteInterview(int id)
        {
            var interviews= await _repoWrapper.Interview
                .GetByCondition(i => i.Id.Equals(id));
            var interviewToDelete = interviews.FirstOrDefault();
            if (interviewToDelete is null)
                throw new NullReferenceException("No existe la entrevista");
            _repoWrapper.Interview.Delete(interviewToDelete);
            await _repoWrapper.Save();
        }
    }
}