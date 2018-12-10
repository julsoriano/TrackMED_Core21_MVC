using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNet.SignalR.Hubs;
//using Microsoft.AspNet.SignalR;
using System;
using System.Linq;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TrackMED.Models;
using TrackMED.Services;
using System.Diagnostics;
using System.Collections.Generic;

namespace TrackMED.Controllers
{
    public abstract class MVCControllerWithHub<T> : Controller where T : IEntity
    {
        //Lazy<IHubContext> hub = new Lazy<IHubContext>(
        //    () => GlobalHost.ConnectionManager.GetHubContext<THub>()
        //);

        //protected IHubContext Hub
        //{
        //    get { return hub.Value; }
        //}
        internal readonly IEntityService<T> _entityService;
        internal readonly IEntityService<Component> _componentService;
        internal readonly IEntityService<SystemTab> _systemtabService;
        internal readonly string entType = typeof(T).ToString().ToUpper();
        internal readonly string sPattern = "^TRACKMED.MODELS.([A-Za-z0-9_]+)$";
        internal Match m;

        public MVCControllerWithHub(IEntityService<T> entityService,
                                    IEntityService<Component> componentService)
        {
            _entityService = entityService;
            _componentService = componentService;
        }

        public MVCControllerWithHub(IEntityService<T> entityService,
                                    IEntityService<Component> componentService,
                                    IEntityService<SystemTab> systemtabService)
        {
            _entityService = entityService;
            _componentService = componentService;
            _systemtabService = systemtabService;
        }

        /*
        // GET: Entities
        public async Task<ActionResult> Index(String id = null)
        {
            return await _entityService.GetEntitiesAsync();
        }
        */

        // GET: Entities/Details/5
        public async Task<ActionResult> Details(string id)
        {
            return View(await _entityService.GetEntityAsync(id));
        }

        // GET: Entities/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Entities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(T collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _entityService.PostEntityAsync(collection);
                    return RedirectToAction("Index");
                }
            }
            catch
            // catch (DataException dex)
            {
                ModelState.AddModelError("", "Unable to create record. Try again, and if the problem persists see your system administrator.");
            }

            return View(collection);
        }

        // GET: Entities/Edit/5
        /*
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Entity = await _entityService.GetEntityAsync(id);

            if (Entity == null)
            {
                return NotFound();
            }
            return View(Entity);
        }
        */
        
        // POST: Entities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]  A "Bad Request" error status is generated if this is uncommented
        //public async Task<JsonResult> Edit(string id)  // Edit(string id, T Entity)
        public async Task<IActionResult> Edit(T entity)  // Edit(string id, T Entity)
        {
            Debug.Assert(entity != null);
            var findRecord = await _entityService.GetEntityAsync(entity.Id);
 
            if (findRecord == null) return NotFound();
            
            if (ModelState.IsValid)
            {
                try
                {
                    var res = await _entityService.EditEntityAsync(entity);
                }
                catch (Exception)
                {
                   throw;
                }
                return RedirectToAction("Index");
            }
            return View(findRecord);
        }

        /* http://gunnarpeipman.com/2010/07/how-to-make-ajax-requests-to-asp-net-mvc-application-using-jquery/#ampshare=http://gunnarpeipman.com/2010/07/how-to-make-ajax-requests-to-asp-net-mvc-application-using-jquery/
        public JsonResult ListCompanyNames(int id)
        {
            var party = _partyRepository.GetCompanyById(id);
            var names = from n in party.Names
                        select new
                        {
                            nameId = n.Id.ToString(),
                            name = n.Name.ToString(),
                            validTo = n.ValidTo.ToShortDateString(),
                            userdata = ""
                        };
            var rows = names.ToArray();
            var data = new JqGridData
            {
                total = rows.Count(),
                page = 1,
                records = rows.Count(),
                rows = rows
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }        */

        // GET: Entities/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            return View(await _entityService.GetEntityAsync(id));
        }

        // POST: Entities/Delete/5
        [HttpPost]
        //[ValidateAntiForgeryToken]  A "Bad Request" error status is generated if this is uncommented
        /*
            This attribute helps defend against cross-site request forgery. It won’t prevent other forgery or tampering attacks. 
              See https://msdn.microsoft.com/en-us/library/system.web.mvc.validateantiforgerytokenattribute(v=vs.118).aspx
              See also: http://stackoverflow.com/questions/13621934/validateantiforgerytoken-purpose-explanation-and-example

            From: http://www.bipinjoshi.net/articles/20e546b4-3ae9-416b-878e-5b12434fe7a6.aspx
              If you are using jQuery $.ajax() to make Ajax calls to the controller action methods that are marked with [ValidateAntiForgeryToken] attribute
              it can create problems because $.ajax() won't pass the hidden form field and the cookie automatically as the classic form submission technique does.
        */

        public async Task<JsonResult> Remove(string id)
        {
            var rectodelete = await _entityService.GetEntityAsync(id);

            // See http://stackoverflow.com/questions/2378023/how-to-return-error-from-asp-net-mvc-action to format Json response
            if (rectodelete == null) { return Json(new { Success = false, Status = "Record non-existent" }); }

            try
            {
                await _entityService.DeleteEntityAsync(id);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Status = ex.Message });
            }

            return Json(new { Success = true, Status = "Completed Successfully" });
        }

        public async Task<IEnumerable<Component>> LoadComponents(string tableName, string descId)
        {
            List<Component> compRecords = await _componentService.GetSelectedEntitiesAsync(tableName, descId);

            var items = compRecords
                          .OrderBy(x => x.imte)
                          .ToList();

            return items;
        }
    }
}