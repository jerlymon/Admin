﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataLayer;
using GeneralLayer;
using AdminAPI.Models.Log;
using Admin.Base;
using System.Diagnostics;

namespace AdminAPI.Base
{
    public class BaseAdminController : ApiController
    {
        #region public Variables
        public HttpResponseMessage msg = new HttpResponseMessage();
        public string IP = "";
        public string currentMethodName = "";
        public string currentControllerName = "";
        public int UserId = 0;
        public string ParamsPath = "";
        #endregion
        public BaseAdminController()
        {
            IP = GetIPDetails();
            ParamsPath = @System.Configuration.ConfigurationManager.AppSettings["params"];
        }
        public string LogRequest(string className, string methodName, string request, string workflow, string tui)
        {
            paramFile PF = new paramFile(ParamsPath);
            try
            {
                string dbCon = PF.getDatabaseConnectionString(DBConstants.MainDBLog);
                DataOperation DO = new DataOperation(dbCon);
                RequestLog log = new RequestLog();
                log.ClassName = className; // this.GetType().Name;
                log.MethodName = methodName;
                log.Request = request;
                log.Response = "";
                log.TUI = tui;
                log.IP = IP;
                log.WorkFlow = workflow;
                DO.BeginTRansaction();
                int result = DO.iteratePropertyObjectsAndInsert(log, "requestLog", false);
                DO.EndTRansaction();
                return "success";
            }
            catch (Exception)
            {
                return "fail";
            }
        }
        public string LogResponse(string className, string methodName, string response, string workflow, string tui)
        {
            paramFile PF = new paramFile(ParamsPath);
            try
            {
                string dbCon = PF.getDatabaseConnectionString(DBConstants.MainDBLog);
                DataOperation DO = new DataOperation(dbCon);
                RequestLog log = new RequestLog();
                log.ClassName = className; // this.GetType().Name;
                log.MethodName = methodName;
                log.Request = "";
                log.Response = response;
                log.TUI = tui;
                log.IP = IP;
                log.WorkFlow = workflow;
                DO.BeginTRansaction();
                int result = DO.iteratePropertyObjectsAndInsert(log, "requestLog", false);
                DO.EndTRansaction();
                return "success";
            }
            catch (Exception)
            {
                return "fail";
            }
        }
        public string LogError(string className, string methodName, string remarks, string tui)
        {
            paramFile PF = new paramFile(ParamsPath);
            try
            {
                string dbCon = PF.getDatabaseConnectionString(DBConstants.MainDBLog);
                DataOperation DO = new DataOperation(dbCon);
                ErrorLog log = new ErrorLog();
                log.ClassName = className; // this.GetType().Name;
                log.MethodName = methodName;
                log.Remarks = remarks;
                log.TUI = tui;
                log.IP = IP;
                DO.BeginTRansaction();
                int result = DO.iteratePropertyObjectsAndInsert(log, "errorlog", false);
                DO.EndTRansaction();
                return "success";
            }
            catch (Exception)
            {
                return "fail";
            }
        }
        public string GetIPDetails()
        {
            string functionReturnValue = "";
            try
            {
                functionReturnValue = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                if (functionReturnValue == null)
                {
                    functionReturnValue = "BLANK";
                }
            }
            catch (Exception)
            {

                throw;
            }
            return functionReturnValue;
        }
        public string getEncryptData(string decString, string decType)
        {
            ManagedAesSample MAS = new ManagedAesSample();
            paramFile PF = new paramFile(ParamsPath);
            string encData = "";
            try
            {
                encData = MAS.EncryptData(decString, PF.getKey(decType));
            }
            catch (Exception ex)
            {
                var st = new StackTrace();
                var sf = st.GetFrame(0);
                string currentMethodName = sf.GetMethod().Name;
                Exception customex = new Exception(currentMethodName + " | " + this.GetType().Name + " | " + ex.Message + " : " + ex.StackTrace);
                throw customex;
            }

            return encData;
        }
    }
}
