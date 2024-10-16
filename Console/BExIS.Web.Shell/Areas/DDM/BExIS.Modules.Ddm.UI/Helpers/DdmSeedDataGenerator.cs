﻿using BExIS.Security.Entities.Authorization;
using BExIS.Security.Entities.Objects;
using BExIS.Security.Services.Authorization;
using BExIS.Security.Services.Objects;
using System.Collections.Generic;
using System.Linq;
using Vaiona.Web.Mvc.Modularity;

namespace BExIS.Modules.Ddm.UI.Helpers
{
    public class DdmSeedDataGenerator : IModuleSeedDataGenerator
    {
        public void GenerateSeedData()
        {
            using (FeatureManager featureManager = new FeatureManager())
            using (OperationManager operationManager = new OperationManager())
            using (var featurePermissionManager = new FeaturePermissionManager())
            {
                #region SECURITY

                //workflows = größere sachen, vielen operation
                //operations = einzelne actions

                //1.controller -> 1.Operation
                List<Feature> features = featureManager.FeatureRepository.Get().ToList();

                Feature DataDiscovery = features.FirstOrDefault(f => f.Name.Equals("Data Discovery"));
                if (DataDiscovery == null) DataDiscovery = featureManager.Create("Data Discovery", "Data Discovery");

                Feature SearchFeature = features.FirstOrDefault(f =>
                    f.Name.Equals("Search") &&
                    f.Parent != null &&
                    f.Parent.Id.Equals(DataDiscovery.Id));

                if (SearchFeature == null) SearchFeature = featureManager.Create("Search", "Search", DataDiscovery);

                Feature SearchManagementFeature = features.FirstOrDefault(f =>
                    f.Name.Equals("Search Managment") &&
                    f.Parent != null &&
                    f.Parent.Id.Equals(DataDiscovery.Id));

                if (SearchManagementFeature == null) SearchManagementFeature = featureManager.Create("Search Management", "Search Management", DataDiscovery);

                Feature Dashboard = features.FirstOrDefault(f =>
                    f.Name.Equals("Dashboard") &&
                    f.Parent != null &&
                    f.Parent.Id.Equals(DataDiscovery.Id));

                if (Dashboard == null) Dashboard = featureManager.Create("Dashboard", "Dashboard", DataDiscovery);

                Feature RequestsManage = features.FirstOrDefault(f =>
                    f.Name.Equals("Requests Manage") &&
                    f.Parent != null &&
                    f.Parent.Id.Equals(DataDiscovery.Id));

                if (RequestsManage == null) RequestsManage = featureManager.Create("Requests Manage", "Manange requests by user", DataDiscovery);

                Feature RequestsSend = features.FirstOrDefault(f =>
                   f.Name.Equals("Requests Send") &&
                   f.Parent != null &&
                   f.Parent.Id.Equals(DataDiscovery.Id));

                if (RequestsSend == null) RequestsSend = featureManager.Create("Requests Send", "Allow to send requests", DataDiscovery);
                //worklfows -> create dataset ->
                //WorkflowManager workflowManager = new WorkflowManager();

                //var operation = new Operation();
                //Workflow workflow = new Workflow();

                //List<Workflow> workflows = workflowManager.WorkflowRepository.Get().ToList();

                #region Help Workflow

                //workflow =
                //    workflows.FirstOrDefault(w =>
                //    w.Name.Equals("Search Help") &&
                //    w.Feature != null &&
                //    w.Feature.Id.Equals(DataDiscovery.Id));

                //if (workflow == null) workflow = workflowManager.Create("Search Help", "", DataDiscovery);

                //operationManager.Create("DDM", "Help", "*", null, workflow);
                operationManager.Create("DDM", "Help", "*");

                #endregion Help Workflow

                #region Search Workflow

                // ToDo -> David, Sven
                // [Sven / 2017-08-21]
                // I had to remove the feature to get dashboard running without DDM feature permissions.
                // We have to think about how we can fix it in a long run. Maybe "DDM/Home" is not the proper
                // place for dashboard!?
                operationManager.Create("DDM", "PublicSearch", "*");
                operationManager.Create("DDM", "Home", "*", SearchFeature);
                operationManager.Create("DDM", "Search", "*", SearchFeature);
                operationManager.Create("DDM", "Data", "*");

                if (!featurePermissionManager.ExistsAsync(null, SearchFeature.Id, PermissionType.Grant).Result)
                {
                    var result_create = featurePermissionManager.CreateAsync(null, SearchFeature.Id, PermissionType.Grant).Result;
                }

                #endregion Search Workflow

                #region Search Admin Workflow

                operationManager.Create("DDM", "Admin", "*", SearchManagementFeature);

                #endregion Search Admin Workflow

                #region Dashboard

                operationManager.Create("DDM", "Dashboard", "*", Dashboard);

                #endregion Dashboard

                #region Requests

                operationManager.Create("DDM", "RequestsManage", "*", RequestsManage);
                operationManager.Create("DDM", "RequestsSend", "*", RequestsSend);

                #endregion Requests


                Feature SearchApi = featureManager.FeatureRepository.Get().FirstOrDefault(f => f.Name.Equals("Search Api") && f.Parent.Equals(DataDiscovery));
                if (SearchApi == null) SearchApi = featureManager.Create("Search Api", "Search Api", DataDiscovery);
                if (!operationManager.Exists("api", "SearchApi", "*")) operationManager.Create("api", "SearchApi", "*", SearchApi);

                Feature DataTable = features.FirstOrDefault(f =>
                   f.Name.Equals("Api") &&
                   f.Parent != null &&
                   f.Parent.Id.Equals(DataDiscovery.Id));

                if (RequestsManage == null) RequestsManage = featureManager.Create("Api", "Api", DataDiscovery);

                operationManager.Create("Api", "DataTable", "*", DataTable);


                #region Tags

                Feature tagFeature = featureManager.FeatureRepository.Get().FirstOrDefault(f => f.Name.Equals("Tag") && f.Parent.Equals(DataDiscovery));
                if (tagFeature == null) tagFeature = featureManager.Create("Tag", "Tag", DataDiscovery);
                Feature tagView = featureManager.FeatureRepository.Get().FirstOrDefault(f => f.Name.Equals("View") && f.Parent.Equals(tagFeature));
                if (tagView == null) tagView = featureManager.Create("View", "View of the tags", tagFeature);

                Feature tagEdit = featureManager.FeatureRepository.Get().FirstOrDefault(f => f.Name.Equals("Edit") && f.Parent.Equals(tagFeature));
                if (tagEdit == null) tagEdit = featureManager.Create("Edit", "Edit of the tags", tagFeature);

                operationManager.Create("Api", "TagInfoView", "*", tagView);

                operationManager.Create("Api", "TagInfoEdit", "*", tagEdit);
                operationManager.Create("DDM", "TagInfo", "*", tagEdit);


                #endregion Requests


                #endregion SECURITY
            }
        }

        public void Dispose()
        {
        }
    }
}