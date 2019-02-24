﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bifrost.Extensions;
using ClientCommon.Contract;
using ClientCommon.Resources;
using Microsoft.PowerBI.Api.V2;
using Microsoft.PowerBI.Api.V2.Models;
using Microsoft.Rest;
using Newtonsoft.Json;
using PowerBIService.Common;
using PowerBIService.Services.Base;
using PowerBIService.Services.Interfaces;
using CloneReportRequest = ClientCommon.Contract.CloneReportRequest;

namespace PowerBIService.Services.Implementation
{
    public class PowerService:PowerServiceBase,IPowerService
    {
        
        protected EmbedConfig         MEmbedConfig;
        protected TileEmbedConfig     MTileEmbedConfig;
        protected TokenCredentials    MTokenCredentials;

        public PowerService()
        {
            MTokenCredentials = null;
            MEmbedConfig = new EmbedConfig();
            MTileEmbedConfig = new TileEmbedConfig();
        }

        public async Task<CloneReportResponse[]> CloneReports(CloneReportRequest cloneReportRequest)
        {
            if (string.IsNullOrWhiteSpace(cloneReportRequest.Credential.TenantId))
            {
                throw new ValidationException(PowerResource.ValidationError_TenantIdMissing);
            }
            if (string.IsNullOrWhiteSpace(cloneReportRequest.Credential.SecretId))
            {
                throw new ValidationException(PowerResource.ValidationError_SecretIdMissing);
            }
            if (string.IsNullOrWhiteSpace(cloneReportRequest.Credential.ApplicationId))
            {
                throw new ValidationException(PowerResource.ValidationError_ApplicationIdMissing);
            }
            if (!cloneReportRequest.CloneReports.Any())
            {
                throw new ValidationException(PowerResource.ValidationError_ReportsMissingForClone);
            }
            if (string.IsNullOrWhiteSpace(cloneReportRequest.ParentWorkSpaceId))
            {
                throw new ValidationException(PowerResource.ValidationError_ParentWorkSpaceMissingForClone);
            }
            if (string.IsNullOrWhiteSpace(cloneReportRequest.ClientWorkSpaceId))
            {
                throw new ValidationException(PowerResource.ValidationError_ClientWorkSpaceMissingForClone);
            }
            if (cloneReportRequest.CloneReports.FirstOrDefault(s => string.IsNullOrWhiteSpace(s.ParentReportId))!=null)
            {
                throw new ValidationException(PowerResource.ValidationError_ParentReportsNameMissingForClone);
            }
            UserCredential = cloneReportRequest.Credential;

            if (!await AuthenticateAsync())
            {
                throw new ValidationException(PowerResource.ValidationError_AuthenticationError);   
            }
            return await Clone(cloneReportRequest);
          
        }
        private async Task<CloneReportResponse[]> Clone(CloneReportRequest cloneReportRequest)
        {
            var responseList = new List<CloneReportResponse>();
            try
            {
                using (var pClient = new PowerBIClient(new Uri(POWER_BI_API_URL), PTokenCredentials))
                {
                    var groups = await pClient.Groups.GetGroupsWithHttpMessagesAsync();
                    var group = groups.Body.Value.FirstOrDefault(s => s.Id == cloneReportRequest.ParentWorkSpaceId);
                    if (group == null)
                    {
                        throw new ValidationException(PowerResource.ValidationErrorParentGroupNotFoundError);
                    }

                    var clientGroup = groups.Body.Value.FirstOrDefault(s => s.Id == cloneReportRequest.ClientWorkSpaceId);
                    if (clientGroup == null)
                    {
                        throw new ValidationException(PowerResource.ValidationErrorClientGroupNotFoundError);
                    }

                    var reports = await pClient.Reports.GetReportsInGroupAsync(group.Id);
                    if (reports.Value.Any())
                    {
                        foreach (var cloneReport in cloneReportRequest.CloneReports)
                        {
                           var parentReport= reports.Value.FirstOrDefault(s => s.Id == cloneReport.ParentReportId);
                           if (parentReport == null) continue;

                               var export= await pClient.Reports.ExportReportInGroupAsync(@group.Id, parentReport.Id);
                               var import = await TryUploadAsync(pClient, clientGroup.Id, export, cloneReport.CloneReportName);
                               var reportDatasetId = import.Datasets.First().Id;
                               try
                               {
                                   var parameter = new Dictionary<string, string> {{"ConnectionUrl", cloneReport.WebApiEndPoint}};

                                   var reportParameters = await pClient.Datasets.GetParametersInGroupAsync(clientGroup.Id, reportDatasetId);
                                   if (reportParameters!=null && reportParameters.Value.Any())
                                   {
                                       await SetReportParameters(pClient, clientGroup.Id, reportDatasetId, reportParameters.Value,parameter );
                                   }
                                   foreach (var impDataset in import.Datasets)
                                   {
                                       var refresh=await pClient.Datasets.RefreshDatasetInGroupWithHttpMessagesAsync(clientGroup.Id, impDataset.Id);
                                   }
                                   var clientGroupReports= await pClient.Reports.GetReportsInGroupAsync(clientGroup.Id);
                                   
                                   foreach (var impReport in import.Reports)
                                   {
                                       var cloneResultReport= clientGroupReports.Value.FirstOrDefault(s =>s.Id == impReport.Id);
                                       await pClient.Reports.RebindReportInGroupWithHttpMessagesAsync(clientGroup.Id,impReport.Id, new RebindReportRequest{DatasetId= cloneResultReport.DatasetId});
                                   }
                                   responseList.Add(new CloneReportResponse
                                   {
                                       CloneReportName = cloneReport.CloneReportName,
                                       CloneReportId = import.Reports.FirstOrDefault()?.Id,
                                       ParentReportId = cloneReport.ParentReportId,
                                       ParentReportName = parentReport.Name,
                                       Success = true
                                   });
                               }
                               catch (Exception e){throw e;}
                        }
                    }
                }
            }
            catch (Exception exp)
            {
               throw new ApplicationException(exp.Message);
            }
            return responseList.ToArray();
        }
        
        private async Task<bool> UpdateAndRefreshDataSet(PowerBIClient pClient,EmbedReportRequest embedReportRequest, string groupId,string reportId,string dataSetId)
        {
            try
            {
                var dataset= await pClient.Datasets.GetDatasetByIdInGroupWithHttpMessagesAsync(groupId, dataSetId);
                var data= await pClient.Datasets.GetParametersInGroupWithHttpMessagesAsync(groupId,  dataSetId);


                var ParamList = new List<UpdateDatasetParameterDetails>();
                ParamList.Add(new UpdateDatasetParameterDetails
                {
                    Name = "ConnectionUrl",
                    NewValue = embedReportRequest.EmbedReportUrl
                });
            
                await pClient.Datasets.UpdateParametersInGroupWithHttpMessagesAsync(groupId, dataSetId,new UpdateDatasetParametersRequest{UpdateDetails =ParamList});
                await pClient.Datasets.RefreshDatasetInGroupWithHttpMessagesAsync(groupId, dataSetId);
                await pClient.Reports.RebindReportInGroupWithHttpMessagesAsync(groupId,reportId, new RebindReportRequest{DatasetId =dataset.Body.Id});
                return true;
            }
            catch (Exception exp)
            {
                return false;
            }
        }
        
        public async Task<EmbedConfig> ClientEmbedReport(EmbedReportRequest embedReportRequest)
        {
            if (string.IsNullOrWhiteSpace(embedReportRequest.EmbedReportUrl))
            {
                throw new ValidationException(PowerResource.EmbedReportUrlIsMissingError);
            }

            if (string.IsNullOrWhiteSpace(embedReportRequest.WorkSpaceId))
            {
                throw new ValidationException(PowerResource.ValidationError_EmbedWorkSpaceMissingForClone);
            }
            
            if (string.IsNullOrWhiteSpace(embedReportRequest.ReportId))
            {
                throw new ValidationException(PowerResource.ValidationError_EmbedReportsMissingError);
            }
            
            if (embedReportRequest.Credential==null)
            {
                throw new ValidationException(PowerResource.ValidationErrorCredentialMissingError);
            }
            
            return await EmbedReport(embedReportRequest);
        }
        private async Task<EmbedConfig> EmbedReport(EmbedReportRequest embedReportRequest)
        {
            var config = new EmbedConfig();
            UserCredential = embedReportRequest.Credential;
            try
            {
                var data =  await AuthenticateAsync();
                using (var pClient = new PowerBIClient(new Uri(POWER_BI_API_URL), PTokenCredentials))
                {

                    var groups = await pClient.Groups.GetGroupsWithHttpMessagesAsync();
                    var group = groups.Body.Value.FirstOrDefault(s => s.Id == embedReportRequest.WorkSpaceId);
                    if (group == null)
                    {
                        throw new ValidationException(PowerResource.ValidationErrorParentGroupNotFoundError);
                    }
                    var reports = await pClient.Reports.GetReportsInGroupAsync(group.Id);
                    if (!reports.Value.Any())
                    {
                        config.ErrorMessage = PowerResource.EmbedReportNotFoundError;
                        return config;
                    }

                    var embeddingReport=reports.Value.FirstOrDefault(s => s.Id == embedReportRequest.ReportId);
                    if (embeddingReport == null)
                    {
                        config.ErrorMessage = PowerResource.EmbedReportNotFoundError;
                    }

                    var dataSet=await pClient.Datasets.GetDatasetByIdInGroupAsync(group.Id, embeddingReport.DatasetId);
                    
                    
                    config.IsEffectiveIdentityRequired = dataSet.IsEffectiveIdentityRequired;
                    config.IsEffectiveIdentityRolesRequired = dataSet.IsEffectiveIdentityRolesRequired;

                   var upDateResponse= await UpdateAndRefreshDataSet(pClient,embedReportRequest, group.Id, embeddingReport.Id, dataSet.Id);
                   if (upDateResponse)
                   {
                       GenerateTokenRequest generateTokenRequestParameters;
                       if (!string.IsNullOrWhiteSpace(embedReportRequest.EmbedUserName))
                       {
                           var rls = new EffectiveIdentity(embedReportRequest.EmbedUserName, new List<string> { embeddingReport.DatasetId });
                           if (!string.IsNullOrWhiteSpace(embedReportRequest.EmbedRoles))
                           {
                               var rolesList = new List<string>();
                               rolesList.AddRange(embedReportRequest.EmbedRoles.Split(','));
                               rls.Roles = rolesList;
                           }
                           generateTokenRequestParameters = new GenerateTokenRequest(accessLevel: "view", identities: new List<EffectiveIdentity> { rls });
                       }
                       else
                       {
                           generateTokenRequestParameters = new GenerateTokenRequest(accessLevel: "view");
                       }
                       var tokenResponse = await pClient.Reports.GenerateTokenInGroupAsync(group.Id, embeddingReport.Id, generateTokenRequestParameters);
                       if (tokenResponse == null)
                       {
                           config.ErrorMessage = "Failed to generate embed token.";
                           return config;
                       }
                       
                       config.EmbedToken = tokenResponse;
                       config.EmbedUrl = embeddingReport.EmbedUrl;
                       config.Id = embeddingReport.Id;
                       return config;
                   }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return null;
        }
        public async Task<bool> CreateGroup(GroupCreateRequest groupCreateRequest)
        {
            UserCredential = groupCreateRequest.Credential;
            await AuthenticateAsync();
            using (var pClient = new PowerBIClient(new Uri(POWER_BI_API_URL), PTokenCredentials))
            {

              var ss=await pClient.Groups.GetGroupsWithHttpMessagesAsync();
                
               var group= await pClient.Groups.CreateGroupWithHttpMessagesAsync(new GroupCreationRequest{Name =groupCreateRequest.GroupName},true);
               if (groupCreateRequest.Members.Any())
               {
                   groupCreateRequest.Members.ForEach(async s =>
                   {
                       try
                       {
                           await pClient.Groups.AddGroupUserWithHttpMessagesAsync(group.Body.Id,new GroupUserAccessRight{EmailAddress = s.MemberEmail, GroupUserAccessRightProperty = s.GroupUserAccessRight.GetRight()});
                       }
                       catch (Exception e)
                       {
                           throw new ApplicationException(PowerResource.ProcessError_UserAssignment);
                       }
                   });
               }
            }
            return true;
        }

        public async Task<bool> AssignUsersToGroup(GroupMemberAssignRequest memberAssignRequest)
        {
            UserCredential = memberAssignRequest.Credential;
            await AuthenticateAsync();
            
            using (var pClient = new PowerBIClient(new Uri(POWER_BI_API_URL), PTokenCredentials))
            {
                var groupSearch= await pClient.Groups.GetGroupsWithHttpMessagesAsync($"id eq '{memberAssignRequest.GroupId}'");
                var group=  groupSearch.Body.Value.FirstOrDefault();
                if (group == null)
                {
                  throw new ValidationException(PowerResource.ValidationErrorParentGroupNotFoundError);   
                }
                if (memberAssignRequest.Members.Any())
                {
                    memberAssignRequest.Members.ForEach(async s =>
                    {
                        try
                        {
                            await pClient.Groups.AddGroupUserWithHttpMessagesAsync(group.Id,new GroupUserAccessRight{EmailAddress = s.MemberEmail, GroupUserAccessRightProperty = s.GroupUserAccessRight.GetRight()});
                        }
                        catch (Exception e)
                        {
                            throw new ApplicationException(PowerResource.ProcessError_UserAssignment);
                        }
                    });
                }
            }
            return true;
        }
        
        #region Helper Methods
        public async Task<NameValueContract[]> GetAllGroups(UserData credential)
        {
            UserCredential = credential;
            await AuthenticateAsync();
            var groupList = new List<NameValueContract>();
            using (var pClient = new PowerBIClient(new Uri(POWER_BI_API_URL), PTokenCredentials))
            {
                var groups = await pClient.Groups.GetGroupsWithHttpMessagesAsync();
                groupList.AddRange(groups.Body.Value.Select(@group => new NameValueContract {Id = @group.Id, Name = @group.Name}));
            }
            return groupList.ToArray();
        }

        public async Task<NameValueContract[]> GetAllReportInWorkSpace(GetReportRequest getReportRequest)
        {
            UserCredential = getReportRequest.Credential;
            await AuthenticateAsync();
            var reportList = new List<NameValueContract>();
            using (var pClient = new PowerBIClient(new Uri(POWER_BI_API_URL), PTokenCredentials))
            {
                var reportsInGroup= await pClient.Reports.GetReportsInGroupWithHttpMessagesAsync(getReportRequest.WorkSpaceId);
                reportList.AddRange(reportsInGroup.Body.Value.Select(@report => new NameValueContract {Id = @report.Id, Name = @report.Name}));
              
            }
            return reportList.ToArray();
            
        }

        public async Task<bool> AddUsersToClonedReport(UserDataSetRequest userDataSetRequest)
        {
            UserCredential = userDataSetRequest.Credential;
            await AuthenticateAsync();
            using (var pClient = new PowerBIClient(new Uri(POWER_BI_API_URL), PTokenCredentials))
            {
                var groupSearch =
                    await pClient.Groups.GetGroupsWithHttpMessagesAsync($"id eq '{userDataSetRequest.GroupId}'");
                if (!groupSearch.Body.Value.Any())
                    return false;

                var group = groupSearch.Body.Value.FirstOrDefault();
                if (group == null) return false;

                var report =await pClient.Reports.GetReportInGroupWithHttpMessagesAsync(group.Id, userDataSetRequest.ReportId);
                if (report.Body == null)
                    return false;

             
            }
            return false;
        }
 
        #endregion
    }
}