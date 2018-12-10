using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TrackMED.Models;
using TrackMED.Services;

namespace TrackMED.Controllers
{
    public class SystemsDescriptionController: MVCControllerWithHub<SystemsDescription>
    {
        public SystemsDescriptionController(IEntityService<SystemsDescription> entityService,
                                            IEntityService<Component> componentService,
                                            IEntityService<SystemTab> systemtabService)
            : base(entityService, componentService, systemtabService)
        {

        }

        // GET: Entities
        public async Task<ActionResult> Index()
        {
            m = Regex.Match(entType, sPattern);
            ViewBag.EntityType = m.Groups[1].Value;
            var allRecords = await _entityService.GetEntitiesAsync();
            var items = allRecords
                        .OrderBy(x => x.Desc);
            return View(items);
        }

        [HttpGet]
        public async Task<IEnumerable<SystemTab>> LoadSystems(string descId)
        {
            List<SystemTab> systemRecords = await _systemtabService.GetEntitiesAsync();

            var items = systemRecords
                          .OrderBy(x => x.imte)
                          .Where(x => x.SystemsDescriptionID == descId)
                          .ToList();

            return items;
        }
    }
}