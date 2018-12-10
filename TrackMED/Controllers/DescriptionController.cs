using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TrackMED.Models;
using TrackMED.Services;
// using TrackMED.ViewModels;

namespace TrackMED.Controllers
{
    public class DescriptionController : MVCControllerWithHub<Description>
    {
        public DescriptionController(IEntityService<Description> entityService, IEntityService<Component> componentService) 
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
        public async Task<ActionResult> Index()
        {
            var allRecords = await _entityService.GetEntitiesAsync();
            Description desc = allRecords[0];

            var items = allRecords
                        .OrderBy(x => x.Desc);
            var model = new EntityViewModel<Description>(items, desc);

            ViewBag.EntityType = typeof(Description);
            // var model = new EntityViewModel<IEntityVM>();

            //.ToList();
            
            if (id != null)
            {
                model.Id = id;
                var compRecords = await _componentService.GetEntitiesAsync();
                model.linkedComponents = compRecords
                      .OrderBy(x => x.imte)
                      .Where(x => x.DescriptionID == id)
                      .ToList();
            }
            

            return View(model);
        }
             
        // POST: Entities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id, Desc, Tag")] Description Entity)
        {
            if (id != Entity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //var res = await _entityService.EditEntityAsync(Entity.Id, Entity);
                    var res = await _entityService.EditEntityAsync(Entity);
                }
                catch (Exception)
                {
                    if (!EntityExists(Entity.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(Entity);
        }
        */
    }
}