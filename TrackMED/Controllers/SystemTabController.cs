using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TrackMED.Models;
using TrackMED.Services;
//using WilderBlog.Services;
//using Excel = Microsoft.Office.Interop.Excel;
//using Serilog;
using System.Globalization;
// using FluentAssertions;

namespace TrackMED.Controllers
{
    //[Route("systems")]
    public class SystemTabController : Controller
    {
        private readonly IEntityService<SystemTab> _entityService;
        private readonly IEntityService<ActivityType> _activitytypeService;
        private readonly IEntityService<Category> _categoryService;
        private readonly IEntityService<Classification> _classificationService;
        private readonly IEntityService<Component> _componentService;
        private readonly IEntityService<Deployment> _deploymentService;
        private readonly IEntityService<Description> _descriptionService;
        private readonly IEntityService<EquipmentActivity> _equipmentactivityService;
        private readonly IEntityService<Event> _eventService;
        private readonly IEntityService<Location> _locationService;
        private readonly IEntityService<Manufacturer> _manufacturerService;
        private readonly IEntityService<Model> _modelService;
        private readonly IEntityService<Model_Manufacturer> _modelmanufacturerService;
        private readonly IEntityService<Owner> _ownerService;
        private readonly IEntityService<ProviderOfService> _serviceproviderService;
        private readonly IEntityService<Status> _statusService;
        private readonly IEntityService<SystemsDescription> _systemsdescriptionService;
        private readonly IEntityService<SystemTab> _systemtabService;

        private readonly ILogger<SystemTabController> _logger;
        private IEmailSender _mailService;
        //private IMailService _mailService;
        private readonly Settings _settings;

        public SystemTabController(IEntityService<SystemTab> entityService,
                                 IEntityService<ActivityType> activitytypeService,
                                 IEntityService<Category> categoryService,
                                 IEntityService<Classification> classificationService,
                                 IEntityService<Component> componentService,
                                 IEntityService<Deployment> deploymentService,
                                 IEntityService<Description> descriptionService,
                                 IEntityService<EquipmentActivity> equipmentactivityService,
                                 IEntityService<Event> eventService,
                                 IEntityService<Location> locationService,
                                 IEntityService<Manufacturer> manufacturerService,
                                 IEntityService<Model> modelService,
                                 IEntityService<Model_Manufacturer> modelmanufacturerService,
                                 IEntityService<Owner> ownerService,
                                 IEntityService<ProviderOfService> serviceproviderService,
                                 IEntityService<Status> statusService,
                                 IEntityService<SystemsDescription> systemsdescriptionService,
                                 IEntityService<SystemTab> systemtabService,
                                 IOptions<Settings> optionsAccessor,
                                 ILogger<SystemTabController> logger,
                                 IEmailSender mailService)
        {
            _entityService = entityService;
            _activitytypeService = activitytypeService;
            _categoryService = categoryService;
            _classificationService = classificationService;
            _componentService = componentService;
            _deploymentService = deploymentService;
            _descriptionService = descriptionService;
            _equipmentactivityService = equipmentactivityService;
            _eventService = eventService;
            _locationService = locationService;
            _manufacturerService = manufacturerService;
            _modelService = modelService;
            _modelmanufacturerService = modelmanufacturerService;
            _ownerService = ownerService;
            _serviceproviderService = serviceproviderService;
            _statusService = statusService;
            _systemsdescriptionService = systemsdescriptionService;
            _systemtabService = systemtabService;

            _settings = optionsAccessor.Value; // reads appsettings.json
            _logger = logger;
            _mailService = mailService;
        }

        
        // GET: Entities
        public async Task<ActionResult> Index(string id = null)
        {
            if (id != null) CleanUp(id);
           
            //await _mailService.SendEmailAsync("jul_soriano@yahoo.com", "Error Creating Components Table", "Please look into this");
            return View(await _entityService.GetEntitiesAsync());
        }

        // GET: Systems/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            SystemTab systemTab = await _entityService.GetEntityAsync(id);
            if (systemTab == null)
            {
                return NotFound();
            }

            List<Component> leftComponents = new List<Component>();
            if (systemTab.LeftComponents != null) {
                leftComponents = PopulateLeftRightComponents(systemTab.LeftComponents).Result;
            }

            List<Component> rightComponents = new List<Component>();
            if (systemTab.RightComponents != null)
            {
                rightComponents = PopulateLeftRightComponents(systemTab.RightComponents).Result;
            }

            var countLeft = systemTab.LeftComponents != null ? systemTab.LeftComponents.Count : 0;
            var countRight = systemTab.RightComponents != null ? systemTab.RightComponents.Count : 0;

            if (countLeft > countRight)
            {
                int diff = countLeft - countRight;
                for (int i = 0; i < diff; i++)
                {
                    Component cRec = new Component();
                    cRec.Description = new Description();
                    cRec.Description.Desc = null;
                    rightComponents.Add(cRec);
                };
            }
            else
            if (countLeft < countRight)
            {
                int diff = countRight - countLeft;
                for (int i = 0; i < diff; i++)
                {
                    Component cRec = new Component();
                    cRec.Description = new Description();
                    cRec.Description.Desc = null;
                    leftComponents.Add(cRec);
                };
            };

            ViewBag.leftComponents = leftComponents;
            ViewBag.rightComponents = rightComponents;

            return View(systemTab);
        }
        
        // GET: Systems/Create
        public ActionResult Create()
        {
            CreateCommon();
      
            ViewBag.leftComponents = null;
            ViewBag.rightComponents = null;

            return View();
        }
        
        // POST: Systems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("imte, serialnumber, SystemsDescriptionID, OwnerID, LocationID, Notes")]
                                        SystemTab systemTab, string[] selectedIMTEs, string typeSubmit)
        {
            // check if imte valid
            int resValidateIMTE = ValidateIMTE(systemTab.imte);

            // valid
            if(resValidateIMTE == (int)ErrorCodes.Valid) 
            {
                List<string> leftComponents = new List<string>();
                List<string> rightComponents = new List<string>();

                Status statusRecord = (typeSubmit == "Deploy") ? _statusService.GetEntityAsyncByDescription("Active_PROD").Result : _statusService.GetEntityAsyncByDescription("Pending").Result;
                // Status statusRecord = (typeSubmit == "Deploy") ? _statusService.GetEntityAsyncByFieldID("Active_PROD").Result : _statusService.GetEntityAsyncByFieldID("Pending").Result;
                string statusID = statusRecord != null ? statusRecord.Id : null;

                foreach (var imte in selectedIMTEs)
                {
                    Component recComponent = await _componentService.GetEntityAsync(imte.Substring(1));
                    recComponent.imteModule = systemTab.imte;
                    recComponent.Status = statusRecord;
                    recComponent.StatusID = statusID;

                    if (imte.Substring(0, 1) == "0")
                    {
                        leftComponents.Add(imte.Substring(1));
                    }
                    else
                    {
                        rightComponents.Add(imte.Substring(1));
                    }
                    await _componentService.EditEntityAsync(recComponent);
                }

                if (ModelState.IsValid)
                {
                    systemTab.serialnumber = !String.IsNullOrEmpty(systemTab.serialnumber) ? systemTab.serialnumber : String.Empty;
                    systemTab.SystemsDescription = !String.IsNullOrEmpty(systemTab.SystemsDescriptionID) ? await _systemsdescriptionService.GetEntityAsync(systemTab.SystemsDescriptionID): null;
                    systemTab.Owner = !String.IsNullOrEmpty(systemTab.OwnerID) ? await _ownerService.GetEntityAsync(systemTab.OwnerID):null;
                    systemTab.Location = !String.IsNullOrEmpty(systemTab.LocationID) ? await _locationService.GetEntityAsync(systemTab.LocationID) : null;
                    systemTab.ReferenceNo = !String.IsNullOrEmpty(systemTab.ReferenceNo) ? systemTab.ReferenceNo : String.Empty;
                    systemTab.Status = statusRecord;
                    systemTab.StatusID = statusID;
                    systemTab.LeftComponents = leftComponents;
                    systemTab.RightComponents = rightComponents;
                    systemTab.DeploymentDate = (typeSubmit == "Deploy") ? DateTime.Now : (DateTime?)null;
                    systemTab.CreatedAtUtc = DateTime.Now;
                    await _entityService.PostEntityAsync(systemTab);
                    return RedirectToAction("Index");
                }
            }

            // invalid
            switch (resValidateIMTE)
            {
                case (int)ErrorCodes.SpecialCharacters:
                    ModelState.AddModelError("imte", "No special characters allowed");
                    break;

                case (int)ErrorCodes.InvalidFormat:
                    ModelState.AddModelError("imte", "IMTE formatted incorrectly");
                    break;

                case (int)ErrorCodes.DuplicateIMTESystem:
                    ModelState.AddModelError("imte", "Duplicate imte found in System Table");
                    break;

                case (int)ErrorCodes.DuplicateIMTEComponent:
                    ModelState.AddModelError("imte", "Duplicate imte found in Component Table");
                    break;

                default:
                    break;
            }

            CreateCommon();

            if (selectedIMTEs != null)
            {
                List<Component> leftComponents = new List<Component>();
                List<Component> rightComponents = new List<Component>();

                foreach (var imte in selectedIMTEs)
                {
                    var imteToAdd = await _componentService.GetEntityAsync(imte.Substring(1));
                    if (imte.Substring(0, 1) == "0")
                        leftComponents.Add(imteToAdd);
                    else
                        rightComponents.Add(imteToAdd);

                    // remove component already selected from the dropdownlist
                    //items = items.Where(x => x.Id != imteToAdd.Id);

                    //items.Remove(items.Where(c => c.Value == Convert.ToString(imteToAdd.ID)).Single());

                }
                ViewBag.leftComponents = leftComponents;
                ViewBag.rightComponents = rightComponents;
            };

            return View(systemTab);                    //return RedirectToAction("Create", systemTab); 
        }

        // GET: Systems/Edit/5
        public async Task<ActionResult> Edit(string Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            SystemTab systemTab = await _entityService.GetEntityAsync(Id);
            if (systemTab == null)
            {
                return NotFound();
            }

            // http://stackoverflow.com/questions/668589/how-can-i-add-an-item-to-a-selectlist-in-asp-net-mvc
            List<SystemsDescription> descRecords = await _systemsdescriptionService.GetEntitiesAsync();
            List<SelectListItem> sl = new SelectList(descRecords.OrderBy(x => x.Desc), "Id", "Desc", systemTab.SystemsDescriptionID).ToList();
            if (systemTab.SystemsDescriptionID == null)
            {
                sl.Insert(0, new SelectListItem { Value = "0", Text = "Please Select", Selected = true });
            }
            ViewBag.SystemsDescriptionID = sl;

            List<Owner> ownerRecords = await _ownerService.GetEntitiesAsync();
            sl = new SelectList(ownerRecords.OrderBy(x => x.Desc), "Id", "Desc", systemTab.OwnerID).ToList();
            if (systemTab.OwnerID == null)
            {
                sl.Insert(0, new SelectListItem { Value = "0", Text = "Please Select", Selected = true });
            }
            ViewBag.OwnerID = sl;

            List<Location> locationRecords = await _locationService.GetEntitiesAsync();
            sl = new SelectList(locationRecords.OrderBy(x => x.Desc), "Id", "Desc", systemTab.LocationID).ToList();
            if (systemTab.LocationID == null)
            {
                sl.Insert(0, new SelectListItem { Value = "0", Text = "Please Select", Selected = true });
            }
            ViewBag.LocationID = sl;

            //ViewBag.LocationID = new SelectList(locationRecords.OrderBy(x => x.Desc), "Id", "Desc", systemTab.LocationID);

            // select only components that are not yet commissioned
            List<Component> compRecords = await _componentService.GetEntitiesAsync();
            var items = compRecords
                        .OrderBy(x => x.Description.Desc)
                        .Where(x => x.imteModule == null);
                        // .Where(x => String.IsNullOrEmpty(x.imteModule));

            //ViewBag.ComponentID = new SelectList(compRecords.OrderBy(x => x.Description.Desc).Where(x => x.imteModule == systemTab.ModuleCode || String.IsNullOrEmpty(x.imteModule) ), "Id", "DescTag");

            // create lookup table with key = Description and value = IEnumerable<Component>
            ILookup<string, Component> lookup = items
                    .ToLookup(p => p.Description.Desc.Trim() + (!String.IsNullOrEmpty(p.Description.Tag) ? " (" + p.Description.Tag + ")" : null),
                              p => p);                   // will be used to create a select list of p.IMTE + " " + p.MaintenanceDateTime);
            ViewBag.LookUp = lookup;

            // from the lookup table: create a select list of description + tag only 
            var listDesc = new List<SelectListItem>();           
            foreach (IGrouping<string, Component> g in lookup)
            {
                listDesc.Add
                (
                    new SelectListItem { Value = g.Key, Text = g.Key }
                );
            }
            ViewBag.ComponentDescID = new SelectList(listDesc, "Value", "Text");

            ViewBag.leftComponents = null;
            ViewBag.rightComponents = null;

            List<string> leftIMTEs = systemTab.LeftComponents;
            List<string> rightIMTEs = systemTab.RightComponents;
            
            List<Component> leftComponents = new List<Component>();
            List<Component> rightComponents = new List<Component>();

            foreach(String c in leftIMTEs)
            {
                var imteToAdd = await _componentService.GetEntityAsync(c);
                leftComponents.Add(imteToAdd);
            }

            foreach (String c in rightIMTEs)
            {
                var imteToAdd = await _componentService.GetEntityAsync(c);
                rightComponents.Add(imteToAdd);
            }

            /*
            foreach (var lr in leftIMTEs.Zip(rightIMTEs, Tuple.Create))
            {
                var imteToAdd = await _componentService.GetEntityAsync(lr.Item1);
                leftComponents.Add(imteToAdd);

                imteToAdd = await _componentService.GetEntityAsync(lr.Item2);
                rightComponents.Add(imteToAdd);
            }
            */

            ViewBag.leftComponents = leftComponents;
            ViewBag.rightComponents = rightComponents;

            return View(systemTab);
        }

        [HttpGet]
        public async Task<IEnumerable<SelectListItem>> LoadComponents(string descId)
        // http://localhost:5000/Systems/LoadComponents?descId=Flow+Analyser
        /*  From: Model Binding JSON POSTs in ASP.NET Core
            By default, when you call AddMvc() in Startup.cs, a JSON formatter, JsonInputFormatter, is automatically configured, 
            but you can add additional formatters if you need to, for example to bind XML to an object.

            So these are superfluous (used in ASP.NET MVC): 
               public JsonResult LoadComponents(string descId) 
               return Json(compData);

        */
        {
            // select only components that are not yet commissioned and equal to the passed description
            List<Component> compRecords = await _componentService.GetEntitiesAsync();

            var items = compRecords
                          .OrderBy(x => x.imte)
                          .Where(x => (String.IsNullOrEmpty(x.imteModule) || (x.imteModule).Contains("PENDING")) && x.Description.Desc == descId)
                          .ToList();  // this step is necessary in order to use Convert.ToString() or String.Format() in the following LINQ to Entities

            var compData = items.Select(m => new SelectListItem()
            {
                Value = m.Id,
                Text = m.imte + (m.CalibrationDate != null? " {" + String.Format("{0:dd/MMM/yy}", m.CalibrationDate) + "}":" {}")
                              + (m.MaintenanceDate != null ? "{" + String.Format("{0:dd/MMM/yy}", m.MaintenanceDate) + "}" : " ")
            });

            return compData;
        }

        // See http://www.codeproject.com/Articles/795483/Do-GET-POST-PUT-DELETE-in-asp-net-MVC-with-Jquery
        [HttpPost]
        //[Route("{id}")]  // WARNING! Do NOT resurrect this line. jQuery POST is laid astray by this
        public async Task<JsonResult> RemoveComponent(string id)
        {
            Component comptoupdate = await _componentService.GetEntityAsync(id);
            if (comptoupdate == null) { return Json("Bad Response"); }
            try
            {
                comptoupdate.imteModule = String.Concat(comptoupdate.imteModule, " PENDING");
                await _componentService.EditEntityAsync(comptoupdate);
            }
            catch (Exception)
            {
                return Json("Bad Response");
            }
            
            return Json("Response is good");
        }

        [HttpPost]
        //[Route("{id}")]  // WARNING! Do NOT resurrect this line. jQuery POST is laid astray by this
        public async void CleanUp(string id)
        {
            if (id == null) return;           

            var recordToUpdate = await _entityService.GetEntityAsync(id);
            if (id == null) return;

            // Remove "PENDING" from imteModule
            if (recordToUpdate.LeftComponents != null) CleanUpComponents(recordToUpdate.LeftComponents);
            if (recordToUpdate.RightComponents != null) CleanUpComponents(recordToUpdate.RightComponents);

            return;
        }

        // POST: Systems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, 
                                        [Bind("Id, imte, serialnumber, SystemsDescriptionID, OwnerID, FixtureCode, ModuleCode, Notes, LeftComponents, RightComponents")]
                                        SystemTab systemTab, string[] selectedIMTEs, string typeSubmit)
        {
            string[] fieldsToBind = new string[] { "Id", "imte", "serialnumber", "SystemsDescriptionID", "OwnerID", "FixtureCode", "ModuleCode", "Notes", "LeftComponents", "RightComponents" };

            if (id == null)
            {
                return BadRequest();
            }

            var recordToUpdate = await _entityService.GetEntityAsync(id);
            if (recordToUpdate == null)
            {
                return NotFound();
            }

            // Nullify all previous components
            List<string> selectedIMTEsLeft = new List<string>();
            List<string> selectedIMTEsRight = new List<string>();
            foreach(String s in selectedIMTEs)
            {
                if(s.Substring(0, 1) == "0")
                {
                    selectedIMTEsLeft.Add(s);
                }
                else
                {
                    selectedIMTEsRight.Add(s);
                }
            }
                 
            if (recordToUpdate.LeftComponents != null) {
                foreach (String s in recordToUpdate.LeftComponents)
                {
                    if(FindImte(s, selectedIMTEsLeft)) continue;
                    Component comp = await _componentService.GetEntityAsync(s);
                    comp.imteModule = null;
                    await _componentService.EditEntityAsync(comp);
                };
            }

            if (recordToUpdate.RightComponents != null)
            {
                foreach (String s in recordToUpdate.RightComponents)
                {
                    if (FindImte(s, selectedIMTEsRight)) continue;
                    Component comp = await _componentService.GetEntityAsync(s);
                    comp.imteModule = null;
                    await _componentService.EditEntityAsync(comp);
                };
            }

            if (ModelState.IsValid)
            {
                systemTab.Id = id;
                systemTab.SystemsDescription = await _systemsdescriptionService.GetEntityAsync(systemTab.SystemsDescriptionID);
                systemTab.Owner = await _ownerService.GetEntityAsync(systemTab.OwnerID);

                Status objStatus = (typeSubmit == "Deploy") ? _statusService.GetEntityAsyncByDescription("Active_PROD").Result : _statusService.GetEntityAsyncByDescription("Pending").Result;
                // Status objStatus = (typeSubmit == "Deploy") ? _statusService.GetEntityAsyncByFieldID("Desc", "Active_PROD").Result : _statusService.GetEntityAsyncByFieldID("Desc", "Pending").Result;
                systemTab.StatusID = objStatus != null ? (objStatus).Id : null;

                List<string> leftComponents = new List<string>();
                List<string> rightComponents = new List<string>();

                foreach (var imte in selectedIMTEsLeft)
                {
                    Component recComponent = await _componentService.GetEntityAsync(imte.Substring(1));
                    recComponent.imteModule = systemTab.imte;
                    recComponent.StatusID = systemTab.StatusID;
                    leftComponents.Add(imte.Substring(1));

                    await _componentService.EditEntityAsync(recComponent);
                }

                foreach (var imte in selectedIMTEsRight)
                {
                    Component recComponent = await _componentService.GetEntityAsync(imte.Substring(1));
                    recComponent.imteModule = systemTab.imte;
                    recComponent.StatusID = systemTab.StatusID;
                    rightComponents.Add(imte.Substring(1));

                    await _componentService.EditEntityAsync(recComponent);
                }

                systemTab.LeftComponents = leftComponents;
                systemTab.RightComponents = rightComponents;
                systemTab.CreatedAtUtc = DateTime.Now;

                await _entityService.EditEntityAsync(systemTab);

                return RedirectToAction("Index");
            }

            return View(recordToUpdate);
        }

        // GET: Systems/Delete/5
        public async Task<ActionResult> Delete(string id, bool? concurrencyError)
        {
            if (id == null)
            {
                return BadRequest();
            }

            SystemTab systemTab = await _entityService.GetEntityAsync(id);
            if (systemTab == null)
            {
                return NotFound();
            }

            List<Component> leftComponents = new List<Component>();
            if (systemTab.LeftComponents != null)
            {
                leftComponents = PopulateLeftRightComponents(systemTab.LeftComponents).Result;
            }

            List<Component> rightComponents = new List<Component>();
            if (systemTab.RightComponents != null)
            {
                rightComponents = PopulateLeftRightComponents(systemTab.RightComponents).Result;
            }

            var countLeft = systemTab.LeftComponents != null ? systemTab.LeftComponents.Count : 0;
            var countRight = systemTab.RightComponents != null ? systemTab.RightComponents.Count : 0;

            if (countLeft > countRight)
            {
                int diff = countLeft - countRight;
                for (int i = 0; i < diff; i++)
                {
                    Component cRec = new Component();
                    cRec.Description = new Description();
                    cRec.Description.Desc = null;
                    rightComponents.Add(cRec);
                };
            }
            else
            if (countLeft < countRight)
            {
                int diff = countRight - countLeft;
                for (int i = 0; i < diff; i++)
                {
                    Component cRec = new Component();
                    cRec.Description = new Description();
                    cRec.Description.Desc = null;
                    leftComponents.Add(cRec);
                };
            };

            ViewBag.leftComponents = leftComponents;
            ViewBag.rightComponents = rightComponents;

            return View(systemTab);
        }

        //  Use this block if IT IS NECESSARY to list out system component before deleting record
        // POST: Systems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id)
        {
            SystemTab systemTab = await _entityService.GetEntityAsync(id);
            if (systemTab == null)
            {
                return NotFound();
            }

            Status objStatus = null;

            try
            {
                foreach (var lr in systemTab.LeftComponents)
                {
                    var imteToUpdate = await _componentService.GetEntityAsync(lr);
                    imteToUpdate.imteModule = null;
                    objStatus = _statusService.GetEntityAsyncByDescription("Avail_PTS_Shelf").Result;
                    // objStatus = _statusService.GetEntityAsyncByFieldID("Desc", "Avail_PTS_Shelf").Result;
                    imteToUpdate.StatusID = objStatus != null ? (objStatus).Id : null;
                    await _componentService.EditEntityAsync(imteToUpdate);
                }

                foreach (var lr in systemTab.RightComponents)
                {
                    var imteToUpdate = await _componentService.GetEntityAsync(lr);
                    imteToUpdate.imteModule = null;
                    objStatus = _statusService.GetEntityAsyncByDescription("Avail_PTS_Shelf").Result;
                    // objStatus = _statusService.GetEntityAsyncByFieldID("Desc", "Avail_PTS_Shelf").Result;/
                    imteToUpdate.StatusID = objStatus != null ? (objStatus).Id : null;
                    await _componentService.EditEntityAsync(imteToUpdate);
                }
                await _entityService.DeleteEntityAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Error in deleting record", ex.Message);
                _logger.LogWarning("Error in deleting {0} record with id = {1}: " + ex.TargetSite.ToString(), "SystemTab", id);

                return RedirectToAction("Index");
            }

            await _entityService.DeleteEntityAsync(id);

            return RedirectToAction("Index");
        }

        /* Use this block if IT IS NOT NECESSARY to list out system component before deleting record
        // POST: Entities/Delete/5
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<JsonResult> Remove(string id)
        {
            var rectodelete = await _entityService.GetEntityAsync(id);
            if (rectodelete == null) { return Json(new { Success = false, Status = "Record non-existent" }); }

            Status objStatus = null;

            try
            {
                foreach (var lr in rectodelete.LeftComponents)
                {
                    var imteToUpdate = await _componentService.GetEntityAsync(lr);
                    imteToUpdate.imteModule = null;
                    objStatus = _statusService.GetEntityAsyncByDescription("Avail_PTS_Shelf").Result;
                    // objStatus = _statusService.GetEntityAsyncByFieldID("Desc", "Avail_PTS_Shelf").Result;
                    imteToUpdate.StatusID = objStatus != null ? (objStatus).Id : null;
                    await _componentService.EditEntityAsync(imteToUpdate);
                }

                foreach (var lr in rectodelete.RightComponents)
                {
                    var imteToUpdate = await _componentService.GetEntityAsync(lr);
                    imteToUpdate.imteModule = null;
                    objStatus = _statusService.GetEntityAsyncByDescription("Avail_PTS_Shelf").Result;
                    // objStatus = _statusService.GetEntityAsyncByFieldID("Desc", "Avail_PTS_Shelf").Result;
                    imteToUpdate.StatusID = objStatus != null ? (objStatus).Id : null;
                    await _componentService.EditEntityAsync(imteToUpdate);
                }
                await _entityService.DeleteEntityAsync(id);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Status = ex.Message });
            }

            return Json(new { Success = true, Status = "Completed Successfully" });
        }
        */

        public async Task<IEnumerable<Deployment>> LoadDeployments(string descId)
        {
            List<Deployment> selectedRecords = await _deploymentService.GetSelectedEntitiesAsync("Deployment", descId);

            var items = selectedRecords
                          .OrderBy(x => x.DeploymentID)
                          .ToList();

            return items;
        }


        #region Private Helper Methods
        private async Task<List<Component>> PopulateLeftRightComponents(List<string> lrComponents)
        {
            List<Component> lrEntities = new List<Component>();

            foreach (var imte in lrComponents)
            {
                var imteToAdd = await _componentService.GetEntityAsync(imte);

                if (imteToAdd != null)
                {
                    imteToAdd.Description = !String.IsNullOrEmpty(imteToAdd.DescriptionID)?_descriptionService.GetEntityAsync(imteToAdd.DescriptionID).Result:null;
                    imteToAdd.Owner = !String.IsNullOrEmpty(imteToAdd.OwnerID)?_ownerService.GetEntityAsync(imteToAdd.OwnerID).Result:null;
                    //imteToAdd.Model = !String.IsNullOrEmpty(imteToAdd.ModelID)?_modelService.GetEntityAsync(imteToAdd.ModelID).Result:null;
                    //imteToAdd.Classification = !String.IsNullOrEmpty(imteToAdd.ClassificationID)?_classificationService.GetEntityAsync(imteToAdd.ClassificationID).Result:null;
                    //imteToAdd.Manufacturer = !String.IsNullOrEmpty(imteToAdd.ManufacturerID)?_manufacturerService.GetEntityAsync(imteToAdd.ManufacturerID).Result:null;            
                    imteToAdd.Model_Manufacturer = !String.IsNullOrEmpty(imteToAdd.Model_ManufacturerID) ? _modelmanufacturerService.GetEntityAsync(imteToAdd.Model_ManufacturerID).Result : null;

                    lrEntities.Add(imteToAdd);
                }
            }
            return lrEntities;
        }
        
        private async void CleanUpComponents(List<string> lrComponents)
        {
            foreach (String s in lrComponents)
            {
                Component comp = await _componentService.GetEntityAsync(s);
                if (comp.imteModule != null)
                {
                    if ((comp.imteModule).Contains("PENDING"))
                    {
                        int pendingStart = (comp.imteModule).IndexOf("PENDING");
                        comp.imteModule = comp.imteModule.Substring(0, pendingStart - 1).Trim();
                        await _componentService.EditEntityAsync(comp);
                    }
                }
            }
        }

        private bool FindImte(String sToFind, List<string> sToFindFrom)
        {
            int cnt = sToFindFrom.Count();
            for(int i=0; i <= cnt; i++)
            {
                if(sToFind == sToFindFrom[i].Substring(1))
                {
                    return true;
                }
            }
            return false;
        }

        private static void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                Console.WriteLine("Unable to release the Object " + ex.ToString());
            }
            finally
            {
                // http://forums.asp.net/t/1719856.aspx?Three+Methods+to+Read+Excel+with+ASP+NET
                Process[] procs = Process.GetProcessesByName("excel");
                foreach (Process pro in procs)
                {
                    pro.Kill();//Kill process.
                }
                GC.Collect();
            }
        }

        private static T CreateRecord<T>(dynamic myvalues, int nColumns)
        {

            Type t = typeof(T);
            //var properties = t.GetProperties();

            PropertyInfo[] pi = t.GetProperties();
            int pCnt = pi.Count();

            // in lieu of the non-generic: T classInstance = new Description(); see https://msdn.microsoft.com/en-us/library/a89hcwhh(v=vs.110).aspx
            // ConstructorInfo magicConstructor = t.GetConstructor(Type.EmptyTypes);
            // T classInstance = (T) magicConstructor.Invoke(new object[] { });

            T classInstance = (T)Activator.CreateInstance(typeof(T), new object[] { }); // http://stackoverflow.com/questions/731452/create-instance-of-generic-type

            for (int i = 1; i < pCnt; i++) // leaving out Id which will be filled up my MongoDB
            {
                if (pi[i].PropertyType != null && pi[i].CanWrite)
                {
                    //if (pi[i].Name == "CreatedAtUtc") pi[i].SetValue(classInstance, DateTime.Now);
                    if (pi[i].PropertyType == typeof(DateTime))
                    {
                        pi[i].SetValue(classInstance, DateTime.Now);
                    }
                    else
                        if (nColumns > 1)
                        {
                            pi[i].SetValue(classInstance, Convert.ChangeType(myvalues[1, i], pi[i].PropertyType), null); // http://stackoverflow.com/questions/1089123/setting-a-property-by-reflection-with-a-string-value
                            //pi[i].SetValue(classInstance, myvalues[1, i+1]);     Error: Object of type 'System.Double' cannot be converted to type 'System.Int32'.
                        }
                        else
                        {
                            pi[i].SetValue(classInstance, Convert.ChangeType(myvalues, pi[i].PropertyType), null);
                        }                               
                }
            }

            return classInstance;
        }

        private int ValidateIMTE(String imte)
        {
            // no special characters allowed
            char[] chars = { '+', '&', '|', '!', '(', ')', '{', '}', '[', ']', '^', '~', '*', '?', ':', '\\', ';', '/', '%', '$' };
            int indexSpecial = imte.IndexOfAny(chars);
            if (indexSpecial >= 0)
            {
                return (int)ErrorCodes.SpecialCharacters;
            }

            // get the portion before the _
            Regex r = new Regex(@"^(?<imtenew>[A-Za-z0-9]+)_?.*$", RegexOptions.None, TimeSpan.FromMilliseconds(200));
            Match m = r.Match(imte);

            if (!m.Success)
            {
                return (int)ErrorCodes.InvalidFormat;
            }

            string[] suffixesIMTE = { String.Empty, "_Yellow", "_Blue", "_Left", "_Right", "_L", "_R" };
            int cnt = suffixesIMTE.Count();
            for (int i = 0; i < cnt; i++)
            {
                string imteToCreate = imte + suffixesIMTE[i];
                SystemTab systabToCreate = _entityService.GetEntityAsyncByDescription(imteToCreate).Result;
                if (systabToCreate != null)
                {
                    return (int)ErrorCodes.DuplicateIMTESystem;
                }
                else
                {
                    Component compToCreate = _componentService.GetEntityAsyncByDescription(imteToCreate).Result;
                    if (compToCreate != null)
                    {
                        return (int)ErrorCodes.DuplicateIMTEComponent;
                    }
                }
            }

            return (int)ErrorCodes.Valid;
        }

        private void CreateCommon()
        {
            List<SystemsDescription> descRecords =  _systemsdescriptionService.GetEntitiesAsync().Result;
            ViewBag.SystemsDescriptionID = new SelectList(descRecords.OrderBy(x => x.Desc), "Id", "Desc");

            List<Owner> ownerRecords =  _ownerService.GetEntitiesAsync().Result;
            ViewBag.OwnerID = new SelectList(ownerRecords.OrderBy(x => x.Desc), "Id", "Desc");

            List<Location> locationRecords =_locationService.GetEntitiesAsync().Result;
            ViewBag.LocationID = new SelectList(locationRecords.OrderBy(x => x.Desc), "Id", "Desc");

            // select only components that are not yet commissioned
            List<Component> compRecords = _componentService.GetEntitiesAsync().Result;
            var items = compRecords
                        .OrderBy(x => x.Description.Desc)
                        .Where(x => String.IsNullOrEmpty(x.imteModule));

            // create lookup table with key = Description and value = IEnumerable<Component>
            ILookup<string, Component> lookup = items
                    .ToLookup(p => p.Description.Desc.Trim(), // + (!String.IsNullOrEmpty(p.Description.Tag) ? " (" + p.Description.Tag + ")" : null),
                              p => p);                        // will be used to create a select list of p.IMTE + " " + p.MaintenanceDateTime);

            ViewBag.LookUp = lookup;

            // from the lookup table: create a select list of description + tag only 
            var listDesc = new List<SelectListItem>();
            foreach (IGrouping<string, Component> g in lookup)
            {
                listDesc.Add
                (
                    new SelectListItem { Value = g.Key, Text = g.Key }
                );
            }
            ViewBag.ComponentDescID = new SelectList(listDesc, "Value", "Text");
        }

        enum ErrorCodes { Valid, SpecialCharacters, InvalidFormat, DuplicateIMTESystem, DuplicateIMTEComponent };

        #endregion
    }

}