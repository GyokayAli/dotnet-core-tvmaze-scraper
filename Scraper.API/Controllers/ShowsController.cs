﻿using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Scraper.API.Models;
using Scraper.Services.IServices;
using Scraper.Data.Entities;
using System;

namespace Scraper.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowsController : ControllerBase
    {
        private readonly IShowService _showService;
        private readonly IMapper _mapper;
        private readonly ILogger<ShowsController> _logger;

        public ShowsController(IShowService showService, IMapper mapper, ILogger<ShowsController> logger)
        {
            _showService = showService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Gets paginated shows
        /// </summary>
        /// <param name="page">The page number. First page = 0</param>
        /// <param name="pageSize">The number of items to return per page. Default = 25, Max = 100</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<ICollection<ShowDto>>> Get(int page = 0, int pageSize = 25, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                // Prevent some odd numbers and set default
                if (page < 0) page = 0;
                if (pageSize < 1 || pageSize > 100) pageSize = 25;

                var shows = await _showService.GetShows(page, pageSize, cancellationToken);
                var mappedShows = _mapper.Map<ICollection<Show>, List<ShowDto>>(shows);

                // Order shows by birthday descending
                mappedShows?.ForEach(s => s.Cast = s.Cast.OrderByDescending(cast => cast.Birthday).ToList());

                return Ok(mappedShows);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not read Shows from storage.");
                throw new Exception("Internal server error while trying to get the Shows.");
            }
        }
    }
}