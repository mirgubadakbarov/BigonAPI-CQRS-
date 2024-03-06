using AutoMapper;
using Bigon.Business.Modules.BlogPostModule.Commands.BlogPostAddCommand;
using Bigon.Business.Modules.BlogPostModule.Commands.BlogPostAddCommentCommand;
using Bigon.Business.Modules.BlogPostModule.Commands.BlogPostEditCommand;
using Bigon.Business.Modules.BlogPostModule.Commands.BlogPostPublishCommand;
using Bigon.Business.Modules.BlogPostModule.Commands.BlogPostRemoveCommand;
using Bigon.Business.Modules.BlogPostModule.Queries.BlogPostCommentsQuery;
using Bigon.Business.Modules.BlogPostModule.Queries.BlogPostGetAllQuery;
using Bigon.Business.Modules.BlogPostModule.Queries.BlogPostGetByIdQuery;
using Bigon.Business.Modules.BlogPostModule.Queries.BlogPostGetBySlugQuery;
using Bigon.Infrastructure.Commons.Concrates;
using Bigon.Infrastructure.Extensions;
using Bigon.WebApi.Models.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace Bigon.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public BlogsController(IMediator mediator,
            IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromRoute] BlogPostGetAllRequest request)
        {
            var response = await _mediator.Send(request);
            var dateFormat = "";

            if (Request.Headers.TryGetValue("dateFormat", out StringValues dateFormats))
            {
                dateFormat = dateFormats.First();
            }
            else
            {
                dateFormat = "dd.MM.yyyy HH:mm:ss:fffff";
            }

            var dto = _mapper.Map<PagedResponse<BlogPostDto>>(response, cfg =>
            {
                cfg.Items["host"] = Request.GetHost();
                Request.AppendHeaderTo(cfg.Items, "dateFormat");
            });

            return Ok(dto);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] BlogPostGetByIdRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }


        [HttpGet("{slug}")]
        public async Task<IActionResult> GetBySlug([FromRoute] BlogPostGetBySlugRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }




        [HttpPost]
        [Authorize("admin.blogs.create")]
        public async Task<IActionResult> Add([FromForm] BlogPostAddRequest request)
        {
            var response = await _mediator.Send(request);
            return CreatedAtAction(nameof(GetById), routeValues: new { id = response.Id }, response);
        }

        [HttpPut("{id}")]
        [Authorize("admin.blogs.edit")]

        public async Task<IActionResult> Edit(int id, [FromForm] BlogPostEditRequest request)
        {
            request.Id = id;
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPost("{postId}/publish")]
        [Authorize("admin.blogs.publish")]

        public async Task<IActionResult> Publish([FromRoute] BlogPostPublishRequest request)
        {
            await _mediator.Send(request);
            return NoContent();
        }

        [HttpPost("addcomment")]
        //[Authorize("admin.blogs.publish")]

        public async Task<IActionResult> AddComment([FromBody] BlogPostAddCommentRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPost("{postId}/comments")]

        public async Task<IActionResult> Comments([FromRoute] BlogPostCommentsRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }


        [HttpDelete("{id}")]
        [Authorize("admin.blogs.delete")]

        public async Task<IActionResult> Remove([FromRoute] BlogPostRemoveRequest request)
        {
            await _mediator.Send(request);
            return NoContent();
        }
    }
}
