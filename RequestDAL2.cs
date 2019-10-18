using BP.eCoW.RMS.BDO;
using BP.eCoW.RMS.DataAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BP.eCoW.RMS.DAL
{
    public static class RequestDAL2
    {
        public static Collection<RMSLog> FetchRMSLogDetails(int requestId)
        {
            Collection<RMSLog> objRMSLogCollectionDetails = new Collection<RMSLog>();
            IDataAccess dataAccess = null;
            dataAccess = default(IDataAccess);
            IDbDataParameter[] parameterCollection;
            DataTable FetchLogs;
            try
            {
                //actionConfigurationTable.Locale = CultureInfo.InvariantCulture;
                using (DataUtility dataUtility = new DataUtility())
                {
                    parameterCollection = new IDbDataParameter[1];
                    parameterCollection[0] = DataUtility.MakeInParameter("@RequestId", SqlDbType.Int, 50, requestId);
                    dataAccess = dataUtility.CreateConnection(BP.eCoW.RMS.Utility.CommonMethods.ConnectionString, false);
                    FetchLogs = dataAccess.ExecuteDataTable("FetchRMSLogs", parameterCollection);
                    foreach (DataRow row in FetchLogs.Rows)
                    {
                        RMSLog objRMSLog = new RMSLog();
                        objRMSLog.Comments = Convert.ToString(row["RMSComments"], CultureInfo.InvariantCulture);
                        objRMSLog.CreatedBy = Convert.ToString(row["CreatedBy"], CultureInfo.InvariantCulture);
                        DateTime? myDateTime = row.IsNull("CreatedDate") ? (DateTime?)null : (DateTime?)row["CreatedDate"];
                        objRMSLog.CreatedDate = myDateTime;
                        objRMSLogCollectionDetails.Add(objRMSLog);
                    }
                }

            }
            catch (Exception ex)
            {
                CommonDAL.LogExceptionInDatabase("RequestDAL2", ex.GetType().Name, ex.Message, "FetchRMSLogDetails", ex.StackTrace);
                throw;
            }
            finally
            {
                if (dataAccess != null)
                {
                    dataAccess.CloseConnection();
                    dataAccess = null;
                }
            }
            return objRMSLogCollectionDetails;
        }


        public static RequestCollectionDetails ViewHistoricAssessmentForCA(string userLogOnId, Int16 requestStatus, Int16 userRoleMasterId)
        {
            RequestCollectionDetails objRequestCollectionDetails = new RequestCollectionDetails();
            IDataAccess dataAccess = null;
            dataAccess = default(IDataAccess);
            IDbDataParameter[] parameterCollection;
            DataTable ViewAssessmentForCA = null;
            try
            {
                using (DataUtility dataUtility = new DataUtility())
                {
                    parameterCollection = new IDbDataParameter[3];
                    parameterCollection[0] = DataUtility.MakeInParameter("@NTID", SqlDbType.VarChar, 50, userLogOnId);
                    parameterCollection[1] = DataUtility.MakeInParameter("@Status", SqlDbType.Int, 20, requestStatus);
                    parameterCollection[2] = DataUtility.MakeInParameter("@UserRoleMasterId", SqlDbType.TinyInt, 20, userRoleMasterId);
                    dataAccess = dataUtility.CreateConnection(BP.eCoW.RMS.Utility.CommonMethods.ConnectionString, false);
                    ViewAssessmentForCA = dataAccess.ExecuteDataTable("FetchHistoricalRequests", parameterCollection);
                    foreach (DataRow row in ViewAssessmentForCA.Rows)
                    {
                        RequestModel objRequestModel = new RequestModel();
                        objRequestModel.RequestId = Convert.ToInt64(row["RequestId"], CultureInfo.InvariantCulture);
                        objRequestModel.DisplayName = Convert.ToString(row["DisplayName"], CultureInfo.InvariantCulture);
                        objRequestModel.RoleName = Convert.ToString(row["RoleName"], CultureInfo.InvariantCulture);
                        objRequestModel.SiteName = Convert.ToString(row["SiteName"], CultureInfo.InvariantCulture);
                        objRequestModel.DateRequested = Convert.ToDateTime(row["DateRequested"], CultureInfo.InvariantCulture);
                        objRequestModel.DateRequested = new DateTime(objRequestModel.DateRequested.Year, objRequestModel.DateRequested.Month, objRequestModel.DateRequested.Day, 0, 0, 0);
                        objRequestCollectionDetails.RequestDetailsValue.Add(objRequestModel);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonDAL.LogExceptionInDatabase("RequestDAL2", ex.GetType().Name, ex.Message, "ViewHistoricAssessmentForCA", ex.StackTrace);
                throw;
            }
            finally
            {
                if (dataAccess != null)
                {
                    dataAccess.CloseConnection();
                    dataAccess = null;
                }
            }
            return objRequestCollectionDetails;
        }

        /// <summary>
        /// Method to fetch Site Authority and Competency Assessors based on request Id
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public static RequestDetailsModel FetchPersonnelDetails(int requestId)
        {
            RequestDetailsModel objPersonnelCollectionDetails = new RequestDetailsModel();
            IDataAccess dataAccess = null;
            dataAccess = default(IDataAccess);
            IDbDataParameter[] parameterCollection;
            DataTable FetchSAPersonnelDetails;
            DataTable FetchCAPersonnelDetails;
            DataSet ds = new DataSet();
            try
            {
                using (DataUtility dataUtility = new DataUtility())
                {
                    parameterCollection = new IDbDataParameter[1];
                    parameterCollection[0] = DataUtility.MakeInParameter("@RequestId", SqlDbType.NVarChar, 50, requestId);
                    dataAccess = dataUtility.CreateConnection(BP.eCoW.RMS.Utility.CommonMethods.ConnectionString, false);
                    ds = dataAccess.ExecuteDataSet("FetchPersonnelDetails", parameterCollection);
                    FetchSAPersonnelDetails = ds.Tables[0];
                    FetchCAPersonnelDetails = ds.Tables[1];
                    foreach (DataRow row in FetchSAPersonnelDetails.Rows)
                    {
                        SiteAuthorities objSiteAuthoritiesModel = new SiteAuthorities();
                        objSiteAuthoritiesModel.DisplayName = Convert.ToString(row["DisplayName"], CultureInfo.InvariantCulture);
                        objPersonnelCollectionDetails.SiteAuthoritiesDetailsValue.Add(objSiteAuthoritiesModel);
                    }

                    foreach (DataRow row in FetchCAPersonnelDetails.Rows)
                    {
                        CompetencyAssessors objCompetencyAssessors = new CompetencyAssessors();
                        objCompetencyAssessors.DisplayName = Convert.ToString(row["DisplayName"], CultureInfo.InvariantCulture);
                        objPersonnelCollectionDetails.CompetencyAssessorsDetailsValue.Add(objCompetencyAssessors);
                    }


                }
            }
            catch (Exception ex)
            {
                CommonDAL.LogExceptionInDatabase("RequestDAL", ex.GetType().Name, ex.Message, "FetchPersonnelDetails", ex.StackTrace);
                throw;
            }
            finally
            {
                if (dataAccess != null)
                {
                    dataAccess.CloseConnection();
                    dataAccess = null;
                }
            }
            return objPersonnelCollectionDetails;
        }

        public static RequestCollectionDetails ViewAssessmentForCARejected(string userLogOnId, Int16 requestStatus, Int16 userRoleMasterId)
        {
            RequestCollectionDetails objRequestCollectionDetails = new RequestCollectionDetails();
            IDataAccess dataAccess = null;
            dataAccess = default(IDataAccess);
            IDbDataParameter[] parameterCollection;
            DataTable ViewAssessmentForCA = null;
            try
            {
                using (DataUtility dataUtility = new DataUtility())
                {
                    parameterCollection = new IDbDataParameter[3];
                    parameterCollection[0] = DataUtility.MakeInParameter("@NTID", SqlDbType.VarChar, 50, userLogOnId);
                    parameterCollection[1] = DataUtility.MakeInParameter("@Status", SqlDbType.Int, 20, requestStatus);
                    parameterCollection[2] = DataUtility.MakeInParameter("@UserRoleMasterId", SqlDbType.TinyInt, 20, userRoleMasterId);
                    dataAccess = dataUtility.CreateConnection(BP.eCoW.RMS.Utility.CommonMethods.ConnectionString, false);
                    ViewAssessmentForCA = dataAccess.ExecuteDataTable("FetchRejectedRequests", parameterCollection);
                    foreach (DataRow row in ViewAssessmentForCA.Rows)
                    {
                        RequestModel objRequestModel = new RequestModel();
                        objRequestModel.RequestId = Convert.ToInt64(row["RequestId"], CultureInfo.InvariantCulture);
                        objRequestModel.DisplayName = Convert.ToString(row["DisplayName"], CultureInfo.InvariantCulture);
                        objRequestModel.RoleName = Convert.ToString(row["RoleName"], CultureInfo.InvariantCulture);
                        objRequestModel.SiteName = Convert.ToString(row["SiteName"], CultureInfo.InvariantCulture);
                        objRequestModel.DateRequested = Convert.ToDateTime(row["DateRequested"], CultureInfo.InvariantCulture);
                        objRequestModel.DateRequested = new DateTime(objRequestModel.DateRequested.Year, objRequestModel.DateRequested.Month, objRequestModel.DateRequested.Day, 0, 0, 0);
                        objRequestCollectionDetails.RequestDetailsValue.Add(objRequestModel);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonDAL.LogExceptionInDatabase("RequestDAL", ex.GetType().Name, ex.Message, "ViewAssessmentForCA", ex.StackTrace);
                throw;
            }
            finally
            {
                if (dataAccess != null)
                {
                    dataAccess.CloseConnection();
                    dataAccess = null;
                }
            }
            return objRequestCollectionDetails;
        }

        public static CompetencyCollectionDetails FetchCompetencyCompletedDetails(string RoleName, string NTID)
        {
            CompetencyCollectionDetails objCompetencyCollectionDetails = new CompetencyCollectionDetails();
            IDataAccess dataAccess = null;
            dataAccess = default(IDataAccess);
            IDbDataParameter[] parameterCollection;
            DataTable FetchCompetencyCompletedDetails;
            DataSet ds = new DataSet();
            try
            {
                using (DataUtility dataUtility = new DataUtility())
                {
                    parameterCollection = new IDbDataParameter[2];
                    parameterCollection[0] = DataUtility.MakeInParameter("@RoleName", SqlDbType.NVarChar, 50, RoleName);
                    parameterCollection[1] = DataUtility.MakeInParameter("@NTID", SqlDbType.NVarChar, 50, NTID);
                    dataAccess = dataUtility.CreateConnection(BP.eCoW.RMS.Utility.CommonMethods.ConnectionString, false);
                    ds = dataAccess.ExecuteDataSet("FetchCompetencyCompletedDetails", parameterCollection);
                    FetchCompetencyCompletedDetails = ds.Tables[0];
                    foreach (DataRow row in FetchCompetencyCompletedDetails.Rows)
                    {
                        CompetencyModel objCompetencyModel = new CompetencyModel();
                        objCompetencyModel.Remarks = Convert.ToString(row["Remarks"], CultureInfo.InvariantCulture);
                        objCompetencyModel.AssessmentCleared = Convert.ToString(row["Result"], CultureInfo.InvariantCulture);
                        objCompetencyModel.AssessedBy = Convert.ToString(row["AssessedBy"], CultureInfo.InvariantCulture);
                        // objCompetencyModel.FileId = Convert.ToInt32(row["FileId"], CultureInfo.InvariantCulture);
                        objCompetencyModel.AssessmentId = Convert.ToInt16(row["AssessmentId"], CultureInfo.InvariantCulture);
                        objCompetencyCollectionDetails.CompetencyDetailsValue.Add(objCompetencyModel);
                    }

                }
            }
            catch (Exception ex)
            {
                CommonDAL.LogExceptionInDatabase("RequestDAL2", ex.GetType().Name, ex.Message, "FetchCompetencyCompletedDetails", ex.StackTrace);
                throw;
            }
            finally
            {
                if (dataAccess != null)
                {
                    dataAccess.CloseConnection();
                    dataAccess = null;
                }
            }
            return objCompetencyCollectionDetails;
        }

        /// <summary>
        /// Validating Prerequisites for a role
        /// </summary>
        /// <param name="ntid"></param>
        /// <param name="regionId"></param>
        /// <param name="siteId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static bool ValidateRolePrerequisites(int ntid, int regionId, int siteId, int roleId)
        {
            bool IsEligible = false;

            IDataAccess dataAccess = null;
            dataAccess = default(IDataAccess);
            IDbDataParameter[] parameterCollection;
            try
            {
                using (DataUtility dataUtility = new DataUtility())
                {
                    dataAccess = dataUtility.CreateConnection(BP.eCoW.RMS.Utility.CommonMethods.ConnectionString, false);
                    dataAccess.CommandTypeString = 0;
                    parameterCollection = new IDbDataParameter[4];
                    parameterCollection[0] = DataUtility.MakeInParameter("@NTID", SqlDbType.Int, 20, ntid);
                    parameterCollection[1] = DataUtility.MakeInParameter("@RegionId", SqlDbType.Int, 20, regionId);
                    parameterCollection[2] = DataUtility.MakeInParameter("@Site", SqlDbType.Int, 20, siteId);
                    parameterCollection[3] = DataUtility.MakeInParameter("@Role", SqlDbType.Int, 20, roleId);
                    IsEligible = Convert.ToBoolean(Convert.ToInt64(dataAccess.ExecuteScalarString("ValidateRolePrerequisites", parameterCollection), CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);


                }
            }
            catch (Exception ex)
            {
                CommonDAL.LogExceptionInDatabase("RequestDAL2", ex.GetType().Name, ex.Message, "ValidateRolePrerequisites", ex.StackTrace);
                throw;
            }
            finally
            {
                if (dataAccess != null)
                {
                    dataAccess.CloseConnection();
                    dataAccess = null;
                }
            }
            return IsEligible;
        }

        public static ErrorCollectionDetails FetchEvalErrorDetails(string pageName)
        {
            ErrorCollectionDetails objErrorCollectionDetails = new ErrorCollectionDetails();

            IDataAccess dataAccess = null;
            dataAccess = default(IDataAccess);
            IDbDataParameter[] parameterCollection;
            DataTable FetchEvalErrorDetails;
            DataSet ds = new DataSet();
            try
            {
                using (DataUtility dataUtility = new DataUtility())
                {
                    parameterCollection = new IDbDataParameter[1];
                    parameterCollection[0] = DataUtility.MakeInParameter("@PageId", SqlDbType.NVarChar, 50, pageName);

                    dataAccess = dataUtility.CreateConnection(BP.eCoW.RMS.Utility.CommonMethods.ConnectionString, false);
                    ds = dataAccess.ExecuteDataSet("FetchErrorDetails", parameterCollection);
                    FetchEvalErrorDetails = ds.Tables[0];
                    if (pageName == "AppErrorData")
                    {
                        foreach (DataRow row in FetchEvalErrorDetails.Rows)
                        {
                            ErrorDetailsModel objErrorDetailsModel = new ErrorDetailsModel();
                         
                            objErrorDetailsModel.ExID = Convert.ToInt32(row["pkExceptionLogId"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.ExceptionMessage = Convert.ToString(row["ExceptionMessage"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.DateOccured = Convert.ToDateTime(row["exceptionTimeStamp"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.ExceptionType = Convert.ToString(row["exceptionType"], CultureInfo.InvariantCulture);



                            objErrorCollectionDetails.ErrorDetailsValue.Add(objErrorDetailsModel);
                        }
                    }
                    else
                    {
                        foreach (DataRow row in FetchEvalErrorDetails.Rows)
                        {
                            ErrorDetailsModel objErrorDetailsModel = new ErrorDetailsModel();
                            objErrorDetailsModel.SLNo = Convert.ToInt32(row["Id"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.userId = Convert.ToString(row["userId"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.ExceptionMessage = Convert.ToString(row["ExceptionMessage"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.DateOccured = Convert.ToDateTime(row["exceptionTimeStamp"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.ExceptionType = Convert.ToString(row["exceptionType"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.customMessage = Convert.ToString(row["customMessage"], CultureInfo.InvariantCulture);
                           

                            objErrorCollectionDetails.ErrorDetailsValue.Add(objErrorDetailsModel);
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                CommonDAL.LogExceptionInDatabase("RequestDAL2", ex.GetType().Name, ex.Message, "EvalErrorDetails", ex.StackTrace);
                throw;
            }
            finally
            {
                if (dataAccess != null)
                {
                    dataAccess.CloseConnection();
                    dataAccess = null;
                }
            }
            return objErrorCollectionDetails;
        }
        public static ErrorCollectionDetails FetchAppDetails(string pageName)
        {
            ErrorCollectionDetails objErrorCollectionDetails = new ErrorCollectionDetails();

            IDataAccess dataAccess = null;
            dataAccess = default(IDataAccess);
            IDbDataParameter[] parameterCollection;
            DataTable FetchAppDetails;
            DataSet ds = new DataSet();
            try
            {
                using (DataUtility dataUtility = new DataUtility())
                {
                    parameterCollection = new IDbDataParameter[1];
                    parameterCollection[0] = DataUtility.MakeInParameter("@PageId", SqlDbType.NVarChar, 50, pageName);

                    dataAccess = dataUtility.CreateConnection(BP.eCoW.RMS.Utility.CommonMethods.ConnectionString, false);
                    ds = dataAccess.ExecuteDataSet("FetchAppDetails", parameterCollection);
                    FetchAppDetails = ds.Tables[0];
                    foreach (DataRow row in FetchAppDetails.Rows)
                    {
                        ErrorDetailsModel objErrorDetailsModel = new ErrorDetailsModel();


                        objErrorDetailsModel.ExceptionType = Convert.ToString(row["exceptionType"], CultureInfo.InvariantCulture);



                        objErrorCollectionDetails.ErrorDetailsValue.Add(objErrorDetailsModel);
                    }

                }
            }
            catch (Exception ex)
            {
                CommonDAL.LogExceptionInDatabase("RequestDAL2", ex.GetType().Name, ex.Message, "AppErrorDetails", ex.StackTrace);
                throw;
            }
            finally
            {
                if (dataAccess != null)
                {
                    dataAccess.CloseConnection();
                    dataAccess = null;
                }
            }
            return objErrorCollectionDetails;
        }

        public static ErrorCollectionDetails FetchAppDataByDate(DateTime FromDate, DateTime ToDate, string pageName)
        {
            ErrorCollectionDetails objErrorCollectionDetails = new ErrorCollectionDetails();

            IDataAccess dataAccess = null;
            dataAccess = default(IDataAccess);
            IDbDataParameter[] parameterCollection;
            DataTable FetchAppdata;
            try
            {
                using (DataUtility dataUtility = new DataUtility())
                {
                    parameterCollection = new IDbDataParameter[3]; //Added for Role specific training details display
                    parameterCollection[0] = DataUtility.MakeInParameter("@FromDate", SqlDbType.DateTime, 50, FromDate);
                    parameterCollection[1] = DataUtility.MakeInParameter("@ToDate", SqlDbType.DateTime, 50, ToDate);
                    parameterCollection[2] = DataUtility.MakeInParameter("@PageId", SqlDbType.NVarChar, 50, pageName);
                    dataAccess = dataUtility.CreateConnection(BP.eCoW.RMS.Utility.CommonMethods.ConnectionString, false);
                    FetchAppdata = dataAccess.ExecuteDataTable("FetchAppDataByDate", parameterCollection);
                    foreach (DataRow row in FetchAppdata.Rows)
                    {

                        if (pageName == "AppErrorData")
                        {
                            ErrorDetailsModel objErrorDetailsModel = new ErrorDetailsModel();
                            objErrorDetailsModel.ExID = Convert.ToInt32(row["pkExceptionLogId"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.ExceptionMessage = Convert.ToString(row["ExceptionMessage"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.DateOccured = Convert.ToDateTime(row["exceptionTimeStamp"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.ExceptionType = Convert.ToString(row["exceptionType"], CultureInfo.InvariantCulture);
                            objErrorCollectionDetails.ErrorDetailsValue.Add(objErrorDetailsModel);
                            objErrorCollectionDetails.ErrorDetailsValue.Add(objErrorDetailsModel);
                        }
                        else
                        {
                            ErrorDetailsModel objErrorDetailsModel = new ErrorDetailsModel();
                            objErrorDetailsModel.SLNo = Convert.ToInt32(row["Id"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.userId = Convert.ToString(row["userId"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.ExceptionMessage = Convert.ToString(row["ExceptionMessage"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.DateOccured = Convert.ToDateTime(row["exceptionTimeStamp"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.ExceptionType = Convert.ToString(row["exceptionType"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.customMessage = Convert.ToString(row["customMessage"], CultureInfo.InvariantCulture);

                            objErrorCollectionDetails.ErrorDetailsValue.Add(objErrorDetailsModel);

                        }


                        


                    }
                }
            }
            
            catch (Exception ex)
            {
                CommonDAL.LogExceptionInDatabase("RequestDAL2", ex.GetType().Name, ex.Message, "AppDataDetails", ex.StackTrace);
                throw;
            }
            finally
            {
                if (dataAccess != null)
                {
                    dataAccess.CloseConnection();
                    dataAccess = null;
                }
            }
            return objErrorCollectionDetails;
        }

        public static ErrorCollectionDetails FetchAllErrorDetails(string id, string pageName)
        {
            ErrorCollectionDetails objErrorCollectionDetails = new ErrorCollectionDetails();

            IDataAccess dataAccess = null;
            dataAccess = default(IDataAccess);
            IDbDataParameter[] parameterCollection;
            DataTable FetchEvalErrorDetails;
            DataSet ds = new DataSet();
            try
            {
                using (DataUtility dataUtility = new DataUtility())
                {
                    parameterCollection = new IDbDataParameter[2];
                    parameterCollection[0] = DataUtility.MakeInParameter("@PageId", SqlDbType.NVarChar, 50, pageName);
                    parameterCollection[1] = DataUtility.MakeInParameter("@Id", SqlDbType.NVarChar, 50, id);
                    dataAccess = dataUtility.CreateConnection(BP.eCoW.RMS.Utility.CommonMethods.ConnectionString, false);
                    ds = dataAccess.ExecuteDataSet("FetchAllErrorDetails", parameterCollection);
                    FetchEvalErrorDetails = ds.Tables[0];
                    
                        foreach (DataRow row in FetchEvalErrorDetails.Rows)
                        {
                            ErrorDetailsModel objErrorDetailsModel = new ErrorDetailsModel();
                           objErrorDetailsModel.SLNo = Convert.ToInt32(row["Id"], CultureInfo.InvariantCulture);
                        objErrorDetailsModel.userId = Convert.ToString(row["userId"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.ExceptionMessage = Convert.ToString(row["ExceptionMessage"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.DateOccured = Convert.ToDateTime(row["exceptionTimeStamp"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.ExceptionType = Convert.ToString(row["exceptionType"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.customMessage = Convert.ToString(row["customMessage"], CultureInfo.InvariantCulture);
                        objErrorDetailsModel.exceptionstack = Convert.ToString(row["exceptionstack"], CultureInfo.InvariantCulture);


                        objErrorCollectionDetails.ErrorDetailsValue.Add(objErrorDetailsModel);
                        }

                    
                }
            }
            catch (Exception ex)
            {
                CommonDAL.LogExceptionInDatabase("RequestDAL2", ex.GetType().Name, ex.Message, "EvalErrorDetails", ex.StackTrace);
                throw;
            }
            finally
            {
                if (dataAccess != null)
                {
                    dataAccess.CloseConnection();
                    dataAccess = null;
                }
            }
            return objErrorCollectionDetails;
        }

        public static ErrorDetailsModel FetchErrorDetailsById(string id, string pageName)
        {

            ErrorDetailsModel objErrorDetailsModel = new ErrorDetailsModel();
            IDataAccess dataAccess = null;
            dataAccess = default(IDataAccess);
            IDbDataParameter[] parameterCollection;
            DataTable FetchEvalErrorDetails;
            DataSet ds = new DataSet();
            try
            {
                using (DataUtility dataUtility = new DataUtility())
                {
                    parameterCollection = new IDbDataParameter[2];
                    parameterCollection[0] = DataUtility.MakeInParameter("@PageId", SqlDbType.NVarChar, 50, pageName);
                    parameterCollection[1] = DataUtility.MakeInParameter("@Id", SqlDbType.NVarChar, 50, id);
                    dataAccess = dataUtility.CreateConnection(BP.eCoW.RMS.Utility.CommonMethods.ConnectionString, false);
                    ds = dataAccess.ExecuteDataSet("FetchAllErrorDetailsById", parameterCollection);
                    FetchEvalErrorDetails = ds.Tables[0];
                    
                    foreach (DataRow row in FetchEvalErrorDetails.Rows)
                    {
                        if (pageName == "AppError")
                        {
                            objErrorDetailsModel.userId = Convert.ToString(row["UserNTId"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.ExID = Convert.ToInt32(row["pkExceptionLogId"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.ExceptionMessage = Convert.ToString(row["ExceptionMessage"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.DateOccured = Convert.ToDateTime(row["exceptionTimeStamp"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.ExceptionType = Convert.ToString(row["exceptionType"], CultureInfo.InvariantCulture);
                          objErrorDetailsModel.exceptionstack = Convert.ToString(row["ExceptionStack"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.userId = Convert.ToString(row["UserNTId"], CultureInfo.InvariantCulture);
                           objErrorDetailsModel.procedureName = Convert.ToString(row["CreatedBy"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.certificationName = Convert.ToString(row["ExceptionScreen"], CultureInfo.InvariantCulture); 
                        }
                        else
                        {
                            objErrorDetailsModel.SLNo = Convert.ToInt32(row["Id"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.userId = Convert.ToString(row["userId"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.ExceptionMessage = Convert.ToString(row["ExceptionMessage"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.DateOccured = Convert.ToDateTime(row["exceptionTimeStamp"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.ExceptionType = Convert.ToString(row["exceptionType"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.customMessage = Convert.ToString(row["customMessage"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.exceptionstack = Convert.ToString(row["exceptionstack"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.certificationName = Convert.ToString(row["certificationName"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.procedureName = Convert.ToString(row["procedureName"], CultureInfo.InvariantCulture);

                        }
                    }


                }
            }
            catch (Exception ex)
            {
                CommonDAL.LogExceptionInDatabase("RequestDAL2", ex.GetType().Name, ex.Message, "EvalErrorDetails", ex.StackTrace);
                throw;
            }
            finally
            {
                if (dataAccess != null)
                {
                    dataAccess.CloseConnection();
                    dataAccess = null;
                }
            }
            return objErrorDetailsModel;
        }
        public static ErrorDetailsModel FetchReportData(string pageName)
        {
            ErrorDetailsModel objErrorDetailsModel = new ErrorDetailsModel();

            IDataAccess dataAccess = null;
            dataAccess = default(IDataAccess);
            IDbDataParameter[] parameterCollection;
            DataTable FetchAppdata;
            try
            {
                using (DataUtility dataUtility = new DataUtility())
                {
                    parameterCollection = new IDbDataParameter[1]; //Added for Role specific training details display
                    parameterCollection[0] = DataUtility.MakeInParameter("@PageId", SqlDbType.NVarChar, 50, pageName);
                    dataAccess = dataUtility.CreateConnection(BP.eCoW.RMS.Utility.CommonMethods.ConnectionString, false);
                    FetchAppdata = dataAccess.ExecuteDataTable("FetchReportDetails", parameterCollection);
                    foreach (DataRow row in FetchAppdata.Rows)
                    {

                        if (pageName == "eCoWReportData")
                        {
                            
                            
                            objErrorDetailsModel.reportDate = Convert.ToDateTime(row["updateddate"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.reportDate = new DateTime(objErrorDetailsModel.reportDate.Year, objErrorDetailsModel.reportDate.Month, objErrorDetailsModel.reportDate.Day, 0, 0, 0);

                            if ( Convert.ToBoolean(row["reportStatus"], CultureInfo.InvariantCulture))
                            {
                                objErrorDetailsModel.ReportStatus = "1";
                            }
                            else
                            {

                                objErrorDetailsModel.ReportStatus = "0";
                            }
                                
                            
                            

                        }
                        else if (pageName == "MTLReportData")
                        {


                            objErrorDetailsModel.reportDate = Convert.ToDateTime(row["updateddate"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.reportDate = new DateTime(objErrorDetailsModel.reportDate.Year, objErrorDetailsModel.reportDate.Month, objErrorDetailsModel.reportDate.Day, 0, 0, 0);

                            if (Convert.ToBoolean(row["reportStatus"], CultureInfo.InvariantCulture))
                            {
                                objErrorDetailsModel.ReportStatus = "1";
                            }
                            else
                            {

                                objErrorDetailsModel.ReportStatus = "0";
                            }


                        }
                        else if (pageName == "PowerBIReportData")
                        {
                           
                           
                            objErrorDetailsModel.reportDate = Convert.ToDateTime(row["updateddate"], CultureInfo.InvariantCulture);


                        }


                    }
                }
            }

            catch (Exception ex)
            {
                CommonDAL.LogExceptionInDatabase("RequestDAL2", ex.GetType().Name, ex.Message, "AppDataDetails", ex.StackTrace);
                throw;
            }
            finally
            {
                if (dataAccess != null)
                {
                    dataAccess.CloseConnection();
                    dataAccess = null;
                }
            }
            return objErrorDetailsModel;
        }
        public static ErrorCollectionDetails FetchReportDatabydate(DateTime FromDate, DateTime ToDate, string pageName)
        {
            ErrorCollectionDetails objErrorCollectionDetails = new ErrorCollectionDetails();

            IDataAccess dataAccess = null;
            dataAccess = default(IDataAccess);
            IDbDataParameter[] parameterCollection;
            DataTable FetchAppdata;
            try
            {
                using (DataUtility dataUtility = new DataUtility())
                {
                    parameterCollection = new IDbDataParameter[3]; //Added for Role specific training details display
                    parameterCollection[0] = DataUtility.MakeInParameter("@FromDate", SqlDbType.DateTime, 50, FromDate);
                    parameterCollection[1] = DataUtility.MakeInParameter("@ToDate", SqlDbType.DateTime, 50, ToDate);
                    parameterCollection[2] = DataUtility.MakeInParameter("@PageId", SqlDbType.NVarChar, 50, pageName);
                    dataAccess = dataUtility.CreateConnection(BP.eCoW.RMS.Utility.CommonMethods.ConnectionString, false);
                    FetchAppdata = dataAccess.ExecuteDataTable("FetchAppDataByDate", parameterCollection);
                    foreach (DataRow row in FetchAppdata.Rows)
                    {

                        if (pageName == "eCowData" )
                        {
                            ErrorDetailsModel objErrorDetailsModel = new ErrorDetailsModel();
                            objErrorDetailsModel.ReportStatus1 = Convert.ToString(row["reportstatus"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.DateOccured = Convert.ToDateTime(row["updateddate"], CultureInfo.InvariantCulture);

                            objErrorCollectionDetails.ErrorDetailsValue.Add(objErrorDetailsModel);
                            objErrorCollectionDetails.ErrorDetailsValue.Add(objErrorDetailsModel);
                        }
                        else
                        {
                            ErrorDetailsModel objErrorDetailsModel = new ErrorDetailsModel();
                            objErrorDetailsModel.SLNo = Convert.ToInt32(row["Id"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.userId = Convert.ToString(row["userId"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.ExceptionMessage = Convert.ToString(row["ExceptionMessage"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.DateOccured = Convert.ToDateTime(row["exceptionTimeStamp"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.ExceptionType = Convert.ToString(row["exceptionType"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.customMessage = Convert.ToString(row["customMessage"], CultureInfo.InvariantCulture);

                            objErrorCollectionDetails.ErrorDetailsValue.Add(objErrorDetailsModel);

                        }





                    }
                }
            }

            catch (Exception ex)
            {
                CommonDAL.LogExceptionInDatabase("RequestDAL2", ex.GetType().Name, ex.Message, "AppDataDetails", ex.StackTrace);
                throw;
            }
            finally
            {
                if (dataAccess != null)
                {
                    dataAccess.CloseConnection();
                    dataAccess = null;
                }
            }
            return objErrorCollectionDetails;
        }

     
        public static ErrorCollectionDetails FetchReportDataDefault(string pageName)
        {
            ErrorCollectionDetails objErrorCollectionDetails = new ErrorCollectionDetails();

            IDataAccess dataAccess = null;
            dataAccess = default(IDataAccess);
            IDbDataParameter[] parameterCollection;
            DataTable FetchEvalErrorDetails;
            DataSet ds = new DataSet();
            try
            {
                using (DataUtility dataUtility = new DataUtility())
                {
                    parameterCollection = new IDbDataParameter[1];
                    parameterCollection[0] = DataUtility.MakeInParameter("@PageId", SqlDbType.NVarChar, 50, pageName);

                    dataAccess = dataUtility.CreateConnection(BP.eCoW.RMS.Utility.CommonMethods.ConnectionString, false);
                    ds = dataAccess.ExecuteDataSet("FetchErrorDetails", parameterCollection);
                    FetchEvalErrorDetails = ds.Tables[0];
                    if (pageName == "AppErrorData")
                    {
                        foreach (DataRow row in FetchEvalErrorDetails.Rows)
                        {
                            ErrorDetailsModel objErrorDetailsModel = new ErrorDetailsModel();

                            objErrorDetailsModel.ExID = Convert.ToInt32(row["pkExceptionLogId"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.ExceptionMessage = Convert.ToString(row["ExceptionMessage"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.DateOccured = Convert.ToDateTime(row["exceptionTimeStamp"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.ExceptionType = Convert.ToString(row["exceptionType"], CultureInfo.InvariantCulture);



                            objErrorCollectionDetails.ErrorDetailsValue.Add(objErrorDetailsModel);
                        }
                    }
                    else
                    {
                        foreach (DataRow row in FetchEvalErrorDetails.Rows)
                        {
                            ErrorDetailsModel objErrorDetailsModel = new ErrorDetailsModel();
                            objErrorDetailsModel.SLNo = Convert.ToInt32(row["Id"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.userId = Convert.ToString(row["userId"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.ExceptionMessage = Convert.ToString(row["ExceptionMessage"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.DateOccured = Convert.ToDateTime(row["exceptionTimeStamp"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.ExceptionType = Convert.ToString(row["exceptionType"], CultureInfo.InvariantCulture);
                            objErrorDetailsModel.customMessage = Convert.ToString(row["customMessage"], CultureInfo.InvariantCulture);


                            objErrorCollectionDetails.ErrorDetailsValue.Add(objErrorDetailsModel);
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                CommonDAL.LogExceptionInDatabase("RequestDAL2", ex.GetType().Name, ex.Message, "EvalErrorDetails", ex.StackTrace);
                throw;
            }
            finally
            {
                if (dataAccess != null)
                {
                    dataAccess.CloseConnection();
                    dataAccess = null;
                }
            }
            return objErrorCollectionDetails;
        }
    }
}
