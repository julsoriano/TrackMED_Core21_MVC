using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TrackMED.Models;
using TrackMED.Services;

namespace TrackMED.Controllers
{
    public class Model_ManufacturerController : MVCControllerWithHub<Model_Manufacturer>
    {
        public Model_ManufacturerController(IEntityService<Model_Manufacturer> entityService, IEntityService<Component> componentService) 
            : base(entityService, componentService)
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
        
        /*
        public async Task<ActionResult> Index(String id = null)
        {
            var allRecords = await _entityService.GetEntitiesAsync();

            var model = new DescComponentModel<Model>();
            model.descRecords = allRecords
                        .OrderBy(x => x.Desc)
                        .ToList();

            if (id != null)
            {
                model.Id = id;
                var compRecords = await _componentService.GetEntitiesAsync();
                model.linkedComponents = compRecords
                      .OrderBy(x => x.imte)
                      .Where(x => x.ModelID == id)
                      .ToList();
            }
            return View(model);
        }
        */
    }
}