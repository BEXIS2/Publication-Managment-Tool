﻿using BExIS.Dlm.Entities.Data;
using BExIS.Dlm.Entities.DataStructure;
using BExIS.Dlm.Services.Data;
using BExIS.Security.Entities.Authorization;
using BExIS.Security.Services.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Vaiona.Web.Extensions;
using Vaiona.Web.Mvc;
using Vaiona.Web.Mvc.Models;

namespace BExIS.Modules.Sam.UI.Controllers
{
    /// <summary>
    /// Manages all funactions an authorized user can do with datasets and their versions
    /// </summary>
    public class DatasetsController : BaseController
    {
        public ActionResult Checkin(int id)
        {
            return View();
        }

        public ActionResult Checkout(int id)
        {
            return View();
        }

        public ActionResult Sync(long id)
        {
            var datasetManager = new DatasetManager();

            try
            {
                datasetManager.SyncView(id, ViewCreationBehavior.Create | ViewCreationBehavior.Refresh);
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("", $@"Dataset {id} could not be synced.");
            }
            //return View();
            return RedirectToAction("Index", new { area = "Sam" });
        }

        /// <summary>
        /// Deletes a dataset, which means the dataset is marked as deleted, but is not physically removed from the database.
        /// </summary>
        /// <param name="id">the identifier of the dataset to be purged.</param>
        /// <remarks>When a dataset is deleted, it is consodered as non-exisiting, but for the sake or provenance, citation, history, etc, it is not removed froom the database.
        /// The function to recover a deleted dataset, will not be provided.</remarks>
        /// <returns></returns>
        public ActionResult Delete(long id)
        {
            var datasetManager = new DatasetManager();
            var entityPermissionManager = new EntityPermissionManager();

            try
            {
                if (datasetManager.DeleteDataset(id, ControllerContext.HttpContext.User.Identity.Name, true))
                {
                    entityPermissionManager.Delete(typeof(Dataset), id);

                    // ToDo: refactor the indexing after "delete dataset" process
                    //ISearchProvider provider = IoCFactory.Container.ResolveForSession<ISearchProvider>() as ISearchProvider;
                    //provider?.UpdateSingleDatasetIndex(id, IndexingAction.DELETE);
                }
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("", $@"Dataset {id} could not be deleted.");
            }
            return View();
            //return RedirectToAction("List");
        }

        /// <summary>
        /// Shows a berif intro about the functions available as well as some warnings that inofrom the user about non recoverability of some of the operations
        /// such as purge.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.Title = PresentationModel.GetViewTitleForTenant("Maintain Datasets", Session.GetTenant());

            DatasetManager dm = new DatasetManager();
            var entityPermissionManager = new EntityPermissionManager();

            List<Dataset> datasets = dm.DatasetRepo.Query().OrderBy(p => p.Id).ToList();

            List<long> datasetIds = new List<long>();
            if (HttpContext.User.Identity.Name != null)
            {
                datasetIds.AddRange(entityPermissionManager.GetKeys(HttpContext.User.Identity.Name, "Dataset", typeof(Dataset), RightType.Delete));
            }

            // dataset id, dataset status, number of data tuples of the latest version, number of variables in the dataset's structure
            List<Tuple<long, DatasetStatus, long, long>> datasetStat = new List<Tuple<long, DatasetStatus, long, long>>();
            foreach(Dataset ds in datasets)
            {
                long noColumns = ds.DataStructure.Self is StructuredDataStructure ? (ds.DataStructure.Self as StructuredDataStructure).Variables.Count() : 0L;
                long noRows = ds.DataStructure.Self is StructuredDataStructure ? dm.GetDatasetLatestVersionEffectiveTupleCount(ds.Id) : 0;
                datasetStat.Add(new Tuple<long, DatasetStatus, long, long>(ds.Id, ds.Status, noRows, noColumns));
            }
            ViewData["DatasetIds"] = datasetIds;
            return View(datasetStat);
        }

        /// <summary>
        /// Purges a dataset, which means the dataset and all its versions will be physically removed from the database.
        /// </summary>
        /// <param name="id">the identifier of the dataset to be purged.</param>
        /// <remarks>This operation is not revocerable.</remarks>
        /// <returns></returns>
        public ActionResult Purge(long id)
        {
            ViewBag.Title = PresentationModel.GetViewTitleForTenant("Purge", Session.GetTenant());

            DatasetManager dm = new DatasetManager();
            var entityPermissionManager = new EntityPermissionManager();

            try
            {
                if (dm.PurgeDataset(id))
                {
                    entityPermissionManager.Delete(typeof(Dataset), id);

                    // ToDo: refactor the indexing after "delete dataset" process
                    //ISearchProvider provider = IoCFactory.Container.ResolveForSession<ISearchProvider>() as ISearchProvider;
                    //provider?.UpdateSingleDatasetIndex(id, IndexingAction.DELETE);
                }
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("", string.Format("Dataset {0} could not be purged.", id));
            }
            return View();
        }

        public ActionResult Rollback(int id)
        {
            return View();
        }

        /// <summary>
        /// Shows the content of a specific dataset version
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Version(int id)
        {
            ViewBag.Title = PresentationModel.GetViewTitleForTenant("Version", Session.GetTenant());

            DatasetManager dm = new DatasetManager();
            DatasetVersion version = dm.DatasetVersionRepo.Get(p => p.Id == id).First();
            var tuples = dm.GetDatasetVersionEffectiveTuples(version);
            ViewBag.VersionId = id;
            ViewBag.DatasetId = version.Dataset.Id;

            if (version.Dataset.DataStructure is StructuredDataStructure)
            {
                ViewBag.Variables = ((StructuredDataStructure)version.Dataset.DataStructure.Self).Variables.ToList();
            }
            else
            {
                ViewBag.Variables = new List<Variable>();
            }

            return View(tuples);
        }

        /// <summary>
        /// Having the identifier of a dataset, lists all its versions.
        /// </summary>
        /// <param name="id">the identifier of the dataset.</param>
        /// <returns></returns>
        public ActionResult Versions(int id)
        {
            ViewBag.Title = PresentationModel.GetViewTitleForTenant("Versions", Session.GetTenant());
            DatasetManager dm = new DatasetManager();
            List<DatasetVersion> versions = dm.DatasetVersionRepo.Query(p => p.Dataset.Id == id).OrderBy(p => p.Id).ToList();
            ViewBag.VersionId = id;
            return View(versions);
        }
    }
}