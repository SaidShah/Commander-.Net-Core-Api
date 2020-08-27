using System;
using System.Collections;
using System.Collections.Generic;
using AutoMapper;
using Commander.Data;
using Commander.Dtos;
using Commander.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Commander.Controllers
{
    [Route("api/commands")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommanderRepository _repository;
        private IMapper _mapper;

        public CommandsController(ICommanderRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        
        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> Get()
        {
            var result = _repository.GetAllCommands();
            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(result));
        }
        
        [HttpGet("{id}", Name="GetById")]
        public ActionResult<CommandReadDto> GetById(int id)
        {
            var result = _repository.GetCommandById(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<CommandReadDto>(result));
        }

        [HttpPost]
        public ActionResult<CommandReadDto> Create(CommandCreateDto commandCreateDto)
        {
            var commandModel = _mapper.Map<Command>(commandCreateDto);
            _repository.Create(commandModel);
            _repository.SaveChanges();
            var createResult = _mapper.Map<CommandReadDto>(commandModel);
            return CreatedAtRoute(nameof(GetById), new {createResult.Id}, createResult);
        }

        [HttpPut("{id}")]
        public ActionResult<CommandReadDto> Update(int id, CommandUpdateDto dto)
        {
            if (id == null || dto == null)
            {
                throw new ArgumentNullException();
            }

            var commandResult = _repository.GetCommandById(id);
            if (commandResult == null)
            {
                return NotFound();
            }

            _mapper.Map(dto, commandResult);
            _repository.Update(commandResult);
            _repository.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public ActionResult Patch(int id, JsonPatchDocument<CommandUpdateDto> patchDocument)
        {
            if (id == null || patchDocument == null)
            {
                throw new ArgumentNullException();
            }

            var model = _repository.GetCommandById(id);

            if (model == null)
            {
                return NotFound();
            }

            var updatedCommand = _mapper.Map<CommandUpdateDto>(model);
            patchDocument.ApplyTo(updatedCommand, ModelState);

            if (!TryValidateModel(updatedCommand))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(updatedCommand, model);
            _repository.Update(model);
            _repository.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var model = _repository.GetCommandById(id);
            if (model == null)
            {
                return NotFound();
            }
            
            _repository.Delete(model);
            _repository.SaveChanges();
            return NoContent();
        }
    }
}