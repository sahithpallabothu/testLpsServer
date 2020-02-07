using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Arista_LPS_WebApp.Entities;
using Arista_LPS_WebApp.Helpers;
using Dapper;
using LPS;
using Microsoft.Extensions.Options;

namespace Arista_LPS_WebApp.DataProvider
{

    public class InsertDataProvider : IInsertDataProvider
    {
        #region private variables
            private readonly AppSettings _appSettings;
            private readonly string connectionString;
            private ILoggerManager _logger;
        #endregion

        public InsertDataProvider(IOptions<AppSettings> appSettings, ILoggerManager logger)
        {
            _appSettings = appSettings.Value;
            connectionString = _appSettings.ConnectionString;
            _logger = logger;
        }
    
        // to get inserts
        public async Task<IEnumerable<Inserts>> GetAllInsertsOrByID(int id=0,string custName=null,string custCode=null,string startDate=null,string endDate=null, string screenCPLName=null,bool active=true)
        {
           using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                  await sqlConnection.OpenAsync();
                   var dynamicParameters =new DynamicParameters();
                    dynamicParameters.Add("@applicationID", id);
                    dynamicParameters.Add("@custName",custName);
                    dynamicParameters.Add("@custCode",custCode);
                    dynamicParameters.Add("@startDate",startDate);
                    dynamicParameters.Add("@endDate", endDate);
                    dynamicParameters.Add("@screenCPLName", screenCPLName=="insert"?null: screenCPLName);
                    dynamicParameters.Add("@active",active);
                    return await sqlConnection.QueryAsync<Inserts>(
                        "spGetAllInserts",
                            dynamicParameters,
                    commandType: CommandType.StoredProcedure);
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }
        }
        //get all insert types
        public async Task<IEnumerable<InsertType>> GetAllInsertTypes()
        {
           using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    return await sqlConnection.QueryAsync<InsertType>(
                        "spGetInsertType",
                    commandType: CommandType.StoredProcedure);
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }
        }

        public async Task<IEnumerable<Application>> GetApplicationsForInserts()
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    return await sqlConnection.QueryAsync<Application>(
                        "spGetActiveApplications",
                    commandType: CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        //to check if insert type and insert number exist for given application code
        public async Task<int> CheckInsertType(string applicationCode, int insertType, int insertNumber, int active,string startDate,string endDate)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@applicationcode", applicationCode);
                    dynamicParameters.Add("@InsertType", insertType);
                    dynamicParameters.Add("@InsertNumber", insertNumber);
                    dynamicParameters.Add("@active", active);
                    dynamicParameters.Add("@startDate", startDate);
                    dynamicParameters.Add("@endDate", endDate);
                    dynamicParameters.Add("@output",dbType: DbType.Int32, direction: ParameterDirection.Output);
                    await sqlConnection.QueryAsync<int>(
                        "spCheckInsertTypeOfAppl",
                         dynamicParameters,
                    commandType: CommandType.StoredProcedure);
                    int count = dynamicParameters.Get<int>("@output");
                    return count;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        //To Add Or Update Insert based on isupdate.
        public async Task<string> AddOrUpdateInsert(Inserts[] inserts, bool isupdate = false)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    DynamicParameters dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@action",  isupdate ? "U" : "I");
                    dynamicParameters.Add("@insert_XML", await GetInserts(inserts));
                     await sqlConnection.OpenAsync();             
                     await sqlConnection.ExecuteAsync(
                        "spInsert_Update",
                         dynamicParameters,
                        commandType: CommandType.StoredProcedure
                    );
                    return "Ok";
                }
                catch(SqlException ex)
                {
                    String DisplayErrorMessage = "";
                    _logger.LogDebug(" Application Data Provider - AddOrUpdateApplication - SQL Exception:" + ex.Message);
                    DisplayErrorMessage = MessageConstants.BadRequest;
                    return DisplayErrorMessage;
                }
                catch(Exception)
                {
                    throw;
                }
            }        
        }


        private async Task<string> GetInserts(Inserts[] inserts)
        {
            StringBuilder childXml = new StringBuilder();
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                   
                    childXml.Append("<root>");
                    foreach(var data in inserts){
                        childXml.AppendFormat
                         ("<row RecordID='{0}' Inserts2Customers='{1}' StartDate='{2}' EndDate='{3}' Type='{4}' InsertNum='{5}' Return='{6}' Active='{7}' Weight='{8}'  Description='{9}'" +
                         " Location='{10}' ReceivedQuantity='{11}' ReceivedDate='{12}' ReceivedBy='{13}'" +
                         " BinLocation='{14}' ReturnedQuantity='{15}' ReorderQuantity='{16}' UsedQuantity='{17}' IsDelete='{18}'" +
                         " UserAdded='{19}' DateAdded='{20}' UserLast='{21}' DateLast='{22}'/>",
                        data.RecordID,data.ApplicationID, data.StartDate, data.EndDate,
                        data.InsertType,data.Number,
                        data.ReturnInserts, data.Active, data.Weight,data.Description,
                        data.LocationInserts,data.ReceivedQuantity,data.ReceivedDate,
                        data.ReceivedBy,data.BinLocation,data.ReturnedQuantity,data.ReorderQuantity,
                        data.UsedQuantity,data.IsDelete,data.UserAdded,data.DateAdded,
                        data.UserLast,data.DateLast);
                     }
                    childXml.Append("</root>");
                }
                catch
                {
                    throw;
                }
            }
            return childXml.ToString();
        }


        public async Task<string> DeleteInsertByID(int recID)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@recId", recID);
                    dynamicParameters.Add("@result", dbType: DbType.String, direction: ParameterDirection.Output, size: 30);
                    await sqlConnection.ExecuteAsync(
                        "spInserts_Delete",
                        dynamicParameters,
                        commandType: CommandType.StoredProcedure);

                    return "ok";
                }
                catch
                {
                    throw;
                }
            }
        }

    }
}
