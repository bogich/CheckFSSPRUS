using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace CheckFSSPRUS.Helpers.HelperWebRequest
{
    /// <summary>
    /// Структура ответа на запрос поиска юридического лица
    /// </summary>
    [DataContract]
    public class FsspResponseLegal
    {
        [DataMember(Name = "status")]
        public string Status { get; set; }
        [DataMember(Name = "code")]
        public int Code { get; set; }
        [DataMember(Name = "exception")]
        public string Exception { get; set; }
        [DataMember(Name = "response")]
        public ResponseLegal Response { get; set; }
    }
    [DataContract]
    public class ResponseLegal
    {
        [DataMember(Name = "task")]
        public string Task { get; set; }
    }

    /// <summary>
    /// Структура ответа на запрос статуса выполнения задачи
    /// </summary>
    [DataContract]
    public class FsspResponseStatus
    {
        [DataMember(Name = "status")]
        public string Status { get; set; }
        [DataMember(Name = "code")]
        public int Code { get; set; }
        [DataMember(Name = "exception")]
        public string Exception { get; set; }
        [DataMember(Name = "response")]
        public ResponseStatus Response { get; set; }
    }
    [DataContract]
    public class ResponseStatus
    {
        [DataMember(Name = "status")]
        public int Status { get; set; }
        [DataMember(Name = "progress")]
        public string Progress { get; set; }
    }

    /// <summary>
    /// Структура ответа на запрос получения результата выполнения задачи
    /// </summary>
    [DataContract]
    public class FsspResponseResult
    {
        [DataMember(Name = "status")]
        public string Status { get; set; }
        [DataMember(Name = "code")]
        public int Code { get; set; }
        [DataMember(Name = "exception")]
        public string Exception { get; set; }
        [DataMember(Name = "response")]
        public ResponseResult Response { get; set; }
    }
    [DataContract]
    public class ResponseResult
    {
        [DataMember(Name = "status")]
        public int Status { get; set; }
        [DataMember(Name = "task_start")]
        public string Task_start { get; set; }
        [DataMember(Name = "task_end")]
        public string Task_end { get; set; }
        [DataMember(Name = "result")]
        public List<ResponseRes> Result { get; set; }
    }
    [DataContract]
    public class ResponseRes
    {
        [DataMember(Name = "status")]
        public int Status { get; set; }
        [DataMember(Name = "query")]
        public ResponseQuery Query { get; set; }
        [DataMember(Name = "result")]
        public List<ResponseResResult> Result { get; set; }
    }
    [DataContract]
    public class ResponseQuery
    {
        [DataMember(Name = "type")]
        public int Type { get; set; }
        [DataMember(Name = "params")]
        public ResponseParams Params { get; set; }
    }
    [DataContract]
    public class ResponseParams
    {
        [DataMember(Name = "region")]
        public string Region { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "address")]
        public string Address { get; set; }
    }
    [DataContract]
    public class ResponseResResult
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "exe_production")]
        public string Exe_production { get; set; }
        [DataMember(Name = "details")]
        public string Details { get; set; }
        [DataMember(Name = "subject")]
        public string Subject { get; set; }
        [DataMember(Name = "department")]
        public string Department { get; set; }
        [DataMember(Name = "bailiff")]
        public string Bailiff { get; set; }
        [DataMember(Name = "ip_end")]
        public string Ip_end { get; set; }
    }

    /// <summary>
    /// Структура ответа на запрос поиска по номеру исполнительного производства
    /// </summary>
    [DataContract]
    public class FsspResponseIP
    {
        [DataMember(Name = "status")]
        public string Status { get; set; }
        [DataMember(Name = "code")]
        public int Code { get; set; }
        [DataMember(Name = "exception")]
        public string Exception { get; set; }
        [DataMember(Name = "response")]
        public ResponseIP Response { get; set; }
    }
    [DataContract]
    public class ResponseIP
    {
        [DataMember(Name = "task")]
        public string Task { get; set; }
    }

    class HelperWebRequest
    {
        /// <summary>
        /// Отправить запрос на поиск юридического лица
        /// </summary>
        /// <param name="_token">Ключ доступа к API</param>
        /// <param name="_region">Номер региона</param>
        /// <param name="_name">Имя юридического лица</param>
        /// <returns></returns>
        public FsspResponseLegal WebRequestLegal(string _token, string _region, string _name)
        {
            WebRequest _webRequest = (HttpWebRequest)WebRequest.Create("https://api-ip.fssprus.ru/api/v1.0/search/legal?token=" + _token + "&region=" + _region + "&name=" + _name);
            _webRequest.ContentType = "application/json";
            _webRequest.Method = "GET";

            WebResponse _webResponse = _webRequest.GetResponse();
            Stream streamResponse = _webResponse.GetResponseStream();
            StreamReader streamReader = new StreamReader(streamResponse);
            string Response = streamReader.ReadToEnd();
            
            DataContractJsonSerializer _dataContractJsonSerializer = new DataContractJsonSerializer(typeof(FsspResponseLegal));
            FsspResponseLegal _result = (FsspResponseLegal)_dataContractJsonSerializer.ReadObject(new MemoryStream(Encoding.Unicode.GetBytes(Response)));

            return _result;
        }

        /// <summary>
        /// Отправить запрос на получение статуса выполнения задачи
        /// </summary>
        /// <param name="_token">Ключ доступа к API</param>
        /// <param name="_task">Task UUID, выданный при успешном выполнении запроса</param>
        /// <returns></returns>
        public FsspResponseStatus WebRequestStatus(string _token, string _task)
        {
            WebRequest _webRequest = (HttpWebRequest)WebRequest.Create("https://api-ip.fssprus.ru/api/v1.0/status?token=" + _token + "&task=" + _task);
            _webRequest.ContentType = "application/json";
            _webRequest.Method = "GET";

            WebResponse _webResponse = _webRequest.GetResponse();
            Stream streamResponse = _webResponse.GetResponseStream();
            StreamReader streamReader = new StreamReader(streamResponse);
            string Response = streamReader.ReadToEnd();

            DataContractJsonSerializer _dataContractJsonSerializer = new DataContractJsonSerializer(typeof(FsspResponseStatus));
            FsspResponseStatus _result = (FsspResponseStatus)_dataContractJsonSerializer.ReadObject(new MemoryStream(Encoding.Unicode.GetBytes(Response)));

            return _result;
        }

        /// <summary>
        /// Отправить запрос на получение результата выполнения задачи
        /// </summary>
        /// <param name="_token">Ключ доступа к API</param>
        /// <param name="_task">Task UUID, выданный при успешном выполнении запроса</param>
        /// <returns></returns>
        public FsspResponseResult WebRequestResult(string _token, string _task)
        {
            WebRequest _webRequest = (HttpWebRequest)WebRequest.Create("https://api-ip.fssprus.ru/api/v1.0/result?token=" + _token + "&task=" + _task);
            _webRequest.ContentType = "application/json";
            _webRequest.Method = "GET";

            WebResponse _webResponse = _webRequest.GetResponse();
            Stream streamResponse = _webResponse.GetResponseStream();
            StreamReader streamReader = new StreamReader(streamResponse);
            string Response = streamReader.ReadToEnd();

            DataContractJsonSerializer _dataContractJsonSerializer = new DataContractJsonSerializer(typeof(FsspResponseResult));
            FsspResponseResult _result = (FsspResponseResult)_dataContractJsonSerializer.ReadObject(new MemoryStream(Encoding.Unicode.GetBytes(Response)));

            return _result;
        }

        /// <summary>
        /// Отправить запрос на поиск по номеру исполнительного производства
        /// </summary>
        /// <param name="_token">Ключ доступа к API</param>
        /// <param name="_number">Номер исполнительного производства в формате n…n/yy/dd/rr или n…n/yy/ddddd-ИП</param>
        /// <returns></returns>
        public FsspResponseIP WebRequestIP(string _token, string _number)
        {
            WebRequest _webRequest = (HttpWebRequest)WebRequest.Create("https://api-ip.fssprus.ru/api/v1.0/search/ip?token=" + _token + "&number=" + _number);
            _webRequest.ContentType = "application/json";
            _webRequest.Method = "GET";

            WebResponse _webResponse = _webRequest.GetResponse();
            Stream streamResponse = _webResponse.GetResponseStream();
            StreamReader streamReader = new StreamReader(streamResponse);
            string Response = streamReader.ReadToEnd();

            DataContractJsonSerializer _dataContractJsonSerializer = new DataContractJsonSerializer(typeof(FsspResponseIP));
            FsspResponseIP _result = (FsspResponseIP)_dataContractJsonSerializer.ReadObject(new MemoryStream(Encoding.Unicode.GetBytes(Response)));

            return _result;
        }
    }
}
