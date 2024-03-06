using Bigon.Business.Modules.SizesModule.Commands.SizeAddCommand;
using Bigon.Business.Modules.SizesModule.Commands.SizeEditCommand;
using Bigon.Business.Modules.SizesModule.Commands.SizeRemoveCommand;
using Bigon.Business.Modules.SizesModule.Queries.SizeGetAllQuery;
using Bigon.Business.Modules.SizesModule.Queries.SizeGetByIdQuery;
using Bigon.Infrastructure.Commons.Concrates;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Bigon.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SizesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IValidator<SizeAddRequest> _validator;

        public SizesController(IMediator mediator,
            IValidator<SizeAddRequest> validator)
        {
            _mediator = mediator;
            _validator = validator;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromRoute] SizeGetAllRequest request)
        {
            var response = await _mediator.Send(request);
            var data = ApiResponse.Succes(response, "OK");
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] SizeGetByIdRequest request)
        {
            var response = await _mediator.Send(request);
            var data = ApiResponse.Succes(response, "OK");
            return Ok(data);
        }


        [HttpPost]
        [Authorize("admin.sizes.create")]
        public async Task<IActionResult> Add([FromBody] SizeAddRequest request)
        {
            var response = await _mediator.Send(request);
            var data = ApiResponse.Succes(response, "CREATED", HttpStatusCode.Created);
            return CreatedAtAction(nameof(GetById), routeValues: new { id = response.Id }, data);
        }

        [HttpPut("{id}")]
        [Authorize("admin.sizes.edit")]
        public async Task<IActionResult> Edit(int id, [FromBody] SizeEditRequest request)
        {
            request.Id = id;
            var response = await _mediator.Send(request);
            var data = ApiResponse.Succes(response, "EDITED", HttpStatusCode.OK);
            return Ok(data);
        }

        [HttpDelete("{id}")]
        [Authorize("admin.sizes.delete")]
        public async Task<IActionResult> Remove([FromRoute] SizeRemoveRequest request)
        {
            await _mediator.Send(request);
            var data = ApiResponse.Succes("REMOVED", HttpStatusCode.OK);
            return Ok(data);
        }

    }
}
