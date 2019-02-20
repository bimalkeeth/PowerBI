using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bifrost.Extensions;
using ClientCommon.Contract;
using Microsoft.AspNetCore.Mvc;
using PowerBIService.Common;
using PowerBIService.Services.Interfaces;
using WebClientDemo.Models;

namespace PowerBIWebApi.Controllers
{
    public class EmbedReportApiController : ControllerBase
    {
        protected readonly IPowerService _powerService;
        private UserData Credential;
        public EmbedReportApiController(IPowerService powerService)
        {
            _powerService = powerService;
            Credential = new UserData
            {
                TenantId = "470cec91-5a0e-47c7-87a9-2fcaf82d5d90",
                SecretId = "82(t[}]Ee+y&+GvT8[tjh+;U9[|x;",
                ApplicationId = "66bec1b2-4684-4a08-9f2b-b67216d4695a",
            };
        }
        /// <summary>
        /// Get All Groups within Service principles
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/reports/GetWorkSpaceReports/{groupId}", Name = "WorkspaceAllReports")]
        public async Task<IEnumerable<PowerReport>> GetWorkSpaceReports(WorkSpaceRequest workSpaceRequest)
        {
            var list = new List<PowerReport>();

            var request = new GetReportRequest {Credential =new UserData
            {
                TenantId = workSpaceRequest.Credential.TenantId,
                SecretId = workSpaceRequest.Credential.SecretId,
                ApplicationId = workSpaceRequest.Credential.ApplicationId
                
            }, WorkSpaceId = workSpaceRequest.GroupId};
            var Result =  await _powerService.GetAllReportInWorkSpace(request);
            Result.ForEach(s =>
            {
                list.Add(new PowerReport { Id = s.Id, Name = s.Name });
            });
            return list.ToArray();
        }
        
        
        /// <summary>
        /// Clone Report from report repository
        /// </summary>
        /// <param name="cloneReport"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/reports/CloneReport", Name = "CloneReport")]
        public async Task<IEnumerable<CloneReportResponseVM>> CloneReport(CloneReportRequestVM cloneReport)
        {

            var cloneReportRequest = new CloneReportRequest
            {
                
                ClientWorkSpaceId = cloneReport.ClientWorkSpaceId,
                ParentWorkSpaceId = cloneReport.ParentWorkSpaceId,
                CloneReports = cloneReport.CloneReports.Select(w => new CloneReport { CloneReportName = w.CloneReportName, ParentReportId = w.ParentReportId, WebApiEndPoint = w.WebApiEndPoint }).ToArray()

            };
            cloneReportRequest.Credential.SecretId = cloneReport.Credential.SecretId;
            cloneReportRequest.Credential.TenantId = cloneReport.Credential.TenantId;
            cloneReportRequest.Credential.ApplicationId = cloneReport.Credential.ApplicationId;
            
            var result =  await _powerService.CloneReports(cloneReportRequest);
            var responseData = result;

            var responseList = new List<CloneReportResponseVM>();

            responseData.ForEach(s =>
            {
                responseList.Add(new CloneReportResponseVM
                {
                    CloneReportName = s.CloneReportName,
                    ParentReportName = s.ParentReportName,
                    Success = s.Success

                });
            });
            return responseList.ToArray();
        }
        /// <summary>
        /// Report Embedding token provider
        /// </summary>
        /// <param name="embedReportRequest"></param>
        /// <returns></returns>
        
        [HttpPost]
        [Route("api/values/EmbedReport", Name = "EmbedReport")]
        public async Task<EmbedConfig> EmbedReport(EmbedReportRequestVM embedReportRequest)
        {
            var embedRequest=new EmbedReportRequest
            {
                Credential = new UserData
                {
                    SecretId = embedReportRequest.Credential.SecretId,
                    TenantId = embedReportRequest.Credential.TenantId,
                    ApplicationId = embedReportRequest.Credential.ApplicationId
                },
                EmbedRoles = embedReportRequest.EmbedRoles,
                ReportId = embedReportRequest.ReportId,
                EmbedUserName = embedReportRequest.EmbedUserName,
                WorkSpaceId = embedReportRequest.WorkSpaceId,
                EmbedReportUrl = embedReportRequest.EmbedReportUrl,
                ParaMeters = embedReportRequest.ParaMeters.Select(s=> new EmbededReportDataSetParam{ParaType = s.ParaType,ParamName = s.ParamName,ParamValue = s.ParamValue}).ToArray()
            };
            var result =  await _powerService.ClientEmbedReport(embedRequest);
            var responseData = result;
            
            return responseData;
        }
    }
}