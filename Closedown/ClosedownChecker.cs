using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Net;
using System.Json;
using Linkhub;

namespace Closedown
{
    public class ClosedownChecker
    {
        private const String ServiceID = "CLOSEDOWN";
        private const String ServiceURL = "https://closedown.linkhub.co.kr";
        private const String APIVersion = "1.0";
        private const String CRLF = "\r\n";

        private Token token;
        private Authority _LinkhubAuth;
        private List<String> _Scopes = new List<string>();


        public ClosedownChecker(String LinkID, String SecretKey)
        {
            _LinkhubAuth = new Authority(LinkID, SecretKey);
            _Scopes.Add("170");
        }

        public Double GetBalance()
        {
            try
            {
                return _LinkhubAuth.getPartnerBalance(getSession_Token(), ServiceID);
            }
            catch (LinkhubException le)
            {
                throw new ClosedownException(le);
            }
        }

        public Single GetUnitCost()
        {
            UnitCostResponse response = httpget<UnitCostResponse>("/UnitCost");

            return Single.Parse(response.unitCost);
        }

        public CorpState checkCorpNum(String CorpNum)
        {
            if (CorpNum == null || CorpNum =="")
            {
                throw new ClosedownException(-99999999, "조회할 사업자번호가 입력되지 않았습니다");
            }
            
            CorpState response = httpget<CorpState>("/Check?CN="+CorpNum);

            return response;
        }

        public List<CorpState> checkCorpNums(List<String> CorpNumList)
        {
            if(CorpNumList == null || CorpNumList.Count == 0) throw new ClosedownException(-99999999,"조회할 사업자번호 목록이 입력되지 않았습니다.");

            String PostData = toJsonString(CorpNumList);

            return httppost<List<CorpState>>("/Check",PostData);
        }




        #region protected

        protected String toJsonString(Object graph)
        {
            return _LinkhubAuth.stringify(graph);
        }
        protected T fromJson<T>(Stream jsonStream)
        {
            return _LinkhubAuth.fromJson<T>(jsonStream);
        }

        private JsonValue getJsonValue(Stream jsonStream)
        {
            return JsonValue.Load(jsonStream);
        }


        private String getSession_Token()
        {
            Token tmpToken = null;

            if (token != null)
            {
                tmpToken = token;
            }

            bool expired = true;
            if (tmpToken != null)
            {
                DateTime expiration = DateTime.Parse(tmpToken.expiration);

                expired = expiration < DateTime.Now;
            }

            if (expired)
            {
                try
                {
                    tmpToken = _LinkhubAuth.getToken(ServiceID, null, _Scopes);

                    token = tmpToken;
                }
                catch (LinkhubException le)
                {
                    throw new ClosedownException(le);
                }
            }

            return token.session_token;
        }

        protected T httpget<T>(String url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ServiceURL + url);

            String bearerToken = getSession_Token();
            request.Headers.Add("Authorization", "Bearer" + " " + bearerToken);
            request.Headers.Add("x-api-version", APIVersion);
            request.Method = "GET";

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stReadData = response.GetResponseStream();

                return fromJson<T>(stReadData);

            }
            catch (Exception we)
            {
                if (we is WebException && ((WebException)we).Response != null)
                {
                    Stream stReadData = ((WebException)we).Response.GetResponseStream();
                    JsonValue t = getJsonValue(stReadData);
                    throw new ClosedownException(t["code"], t["message"]);
                }
                throw new ClosedownException(-99999999, we.Message);
            }

        }

        protected T httppost<T>(String url, String PostData)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ServiceURL + url);

            request.ContentType = "application/json;";

            String bearerToken = getSession_Token();
            request.Headers.Add("Authorization", "Bearer" + " " + bearerToken);
            request.Headers.Add("x-api-version", APIVersion);
            request.Method = "POST";

            if (String.IsNullOrEmpty(PostData)) PostData = "";

            byte[] btPostDAta = Encoding.UTF8.GetBytes(PostData);

            request.ContentLength = btPostDAta.Length;

            request.GetRequestStream().Write(btPostDAta, 0, btPostDAta.Length);

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stReadData = response.GetResponseStream();

                return fromJson<T>(stReadData);

            }
            catch (Exception we)
            {
                if (we is WebException && ((WebException)we).Response != null)
                {
                    Stream stReadData = ((WebException)we).Response.GetResponseStream();
                    JsonValue t = getJsonValue(stReadData);
                    throw new ClosedownException(t["code"], t["message"]);
                }
                throw new ClosedownException(-99999999, we.Message);
            }
        }



        #endregion

        public class UnitCostResponse
        {
            public string unitCost;
        }
    }
}
