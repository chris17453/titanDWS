/******************************************************************** 
  ████████╗██╗████████╗ █████╗ ███╗   ██╗██████╗ ██╗    ██╗███████╗
  ╚══██╔══╝██║╚══██╔══╝██╔══██╗████╗  ██║██╔══██╗██║    ██║██╔════╝
     ██║   ██║   ██║   ███████║██╔██╗ ██║██║  ██║██║ █╗ ██║███████╗
     ██║   ██║   ██║   ██╔══██║██║╚██╗██║██║  ██║██║███╗██║╚════██║
     ██║   ██║   ██║   ██║  ██║██║ ╚████║██████╔╝╚███╔███╔╝███████║
     ╚═╝   ╚═╝   ╚═╝   ╚═╝  ╚═╝╚═╝  ╚═══╝╚═════╝  ╚══╝╚══╝ ╚══════╝
*********************************************************************
- Created: 01-23-2017
- Author : Charles Watkins
- Email  : chris17453@gmail.com
********************************************************************/
using System;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace titan {
    public class security {
        public class titan_token {
            public string r1 {get; set; }
            public string r2 {get; set; }
            public string r3 {get; set; }
            public string r4 {get; set; }
            public string r5 {get; set; }
            public string r6 {get; set; }
            public string r7 {get; set; }
            public string r8 {get; set; }
            public string r9 {get; set; }
            public string r10 {get; set; }
            public string r11 {get; set; }
            public string r12 {get; set; }
            public string r13 {get; set; }
            public string r14 {get; set; }
            public string r15 {get; set; }
            public int expiration {get; set; }
            public int issued_at {get; set; }
            public bool valid {get; set; }
            public titan_token() {
                valid=false;
            }
        }

        public class error {
            public string msg="";
             public error(string _msg) {
                this.msg=_msg;
            }
        }

         public static string ByteToHex(byte[] bytes, bool upperCase) {
            StringBuilder result = new StringBuilder(bytes.Length*2);
            for (int i = 0; i < bytes.Length; i++)
                result.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));
            return result.ToString();
        }
        
        public static string MD5(string values) {
            byte[] hash=System.Security.Cryptography.MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(values));
            return ByteToHex(hash,true);
        }

        public static byte[] get_certificate_private_key() {
            if(ConfigurationManager.AppSettings["local_store_certificate"]!=null) {
                string azure_cert_id=ConfigurationManager.AppSettings["local_store_certificate"];
                X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                    certStore.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection certCollection = certStore.Certificates.Find(
                                X509FindType.FindByThumbprint,
                                azure_cert_id,
                                false);
                if (certCollection.Count > 0){
                    X509Certificate2 certificate = certCollection[0];
                    certStore.Close();
                    byte[] privateKey = certificate.Export(X509ContentType.Cert);
                    return privateKey;
                }
                certStore.Close();
            }
            if(null!=ConfigurationManager.AppSettings["titan_CERTIFICATE_KEY"]) {                                          //if not use file path
                string titan_CERT_KEY    =ConfigurationManager.AppSettings["titan_CERTIFICATE_KEY"];
                string titan_CERTIFICATE =ConfigurationManager.AppSettings["titan_CERTIFICATE_PATH"];
                if(!string.IsNullOrWhiteSpace(titan_CERT_KEY) && 
                    !string.IsNullOrWhiteSpace(titan_CERTIFICATE) ) {
                    X509Certificate2  certificate = new X509Certificate2(titan_CERTIFICATE,titan_CERT_KEY);
                    return certificate.Export(X509ContentType.Cert);
                }
            } else {
                using (Stream CertStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("titan.certificates.cer.pfx")){
                    byte[] RawBytes = new byte[CertStream.Length];
                    for (int Index = 0; Index < CertStream.Length; Index++)                    {
                        RawBytes[Index] = (byte)CertStream.ReadByte();
                    }

                    X509Certificate2 certificate= new X509Certificate2();
                    certificate.Import(RawBytes);
                    return certificate.Export(X509ContentType.Cert);

                }
            }


            return null;
        }
        public static titan_token  Decode(string JWT) {
            byte[] privateKey=get_certificate_private_key();   
            if(null==privateKey) {
                titan_token t=new titan_token();
                t.valid=false;
            }
            titan_token token= JsonWebToken.Decode(JWT,privateKey ,true);
            return token;
        }

        public static models.json_results wrapper(string JWT,models.lambda i,Func<titan_token,object>func){
            models.json_results res=new models.json_results();
            if(JWT!=null)  {
                byte[] privateKey=get_certificate_private_key();                
                titan_token token= JsonWebToken.Decode(JWT,privateKey ,true);
                if(token.valid) {
                    res.results=func(token);
                }  else {
                    res.results=new error("Failed to validate token");
                    res.success=false;
                }
            } else {
                res.results=new error("Failed to validate token");
                res.success=false;
            }
            res.request_for=i.group+"/"+i.method+"/"+i.owner;
            return res;
        }

        
         public static string Encode(string reference1,string reference2,string reference3,string reference4,string reference5,
                 string reference6,string reference7,string reference8,string reference9,string reference10,
                 string reference11,string reference12,string reference13,string reference14,string reference15){
            var utc0 = new DateTime(1970,1,1,0,0,0,0, DateTimeKind.Utc);
            var issueTime = DateTime.Now;

            var iat = (int)issueTime.Subtract(utc0).TotalSeconds;
            var exp = (int)issueTime.AddMinutes(60*10).Subtract(utc0).TotalSeconds; // Expiration time is up to 10 hours

            string titan_AUDIENCE    =ConfigurationManager.AppSettings["titan_AUDIENCE"];
            string titan_SCOPE       =ConfigurationManager.AppSettings["titan_SCOPE"];

            titan_token token= new titan_token();
            token.r1  = reference1;
            token.r2  = reference2;
            token.r3  = reference3;
            token.r4  = reference4;
            token.r5  = reference5;
            token.r6  = reference6;
            token.r7  = reference7;
            token.r8  = reference8;
            token.r9  = reference9;
            token.r10  = reference10;
            token.r11  = reference11;
            token.r12  = reference12;
            token.r13  = reference13;
            token.r14  = reference14;
            token.r15  = reference15;
            token.expiration  = exp;
            token.issued_at   = iat;

            byte[] privateKey=get_certificate_private_key();                
            return JsonWebToken.Encode(token, privateKey, JwtHashAlgorithm.RS256);
        }


        public enum JwtHashAlgorithm{
            RS256,
            HS384,
            HS512
        }

        public class JsonWebToken{
            private static Dictionary<JwtHashAlgorithm, Func<byte[], byte[], byte[]>> HashAlgorithms;

            static JsonWebToken(){
                HashAlgorithms = new Dictionary<JwtHashAlgorithm, Func<byte[], byte[], byte[]>>{
                        { JwtHashAlgorithm.RS256, (key, value) => { using (var sha = new HMACSHA256(key)) { return sha.ComputeHash(value); } } },
                        { JwtHashAlgorithm.HS384, (key, value) => { using (var sha = new HMACSHA384(key)) { return sha.ComputeHash(value); } } },
                        { JwtHashAlgorithm.HS512, (key, value) => { using (var sha = new HMACSHA512(key)) { return sha.ComputeHash(value); } } }
                    };
            }

            public static string Encode(titan_token payload, string key, JwtHashAlgorithm algorithm){
                return Encode(payload, Encoding.UTF8.GetBytes(key), algorithm);
            }

            public class jwt_header{ 
                    public string alg {get; set; }
                    public string type {get; set; }
            }
            public static string Encode(titan_token payload, byte[] keyBytes, JwtHashAlgorithm algorithm){
                System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
                var segments = new List<string>();
                jwt_header header= new jwt_header();
                header.alg=algorithm.ToString();
                header.type="JWT";

                byte[] headerBytes = Encoding.UTF8.GetBytes(jss.Serialize(header));
                byte[] payloadBytes = Encoding.UTF8.GetBytes(jss.Serialize(payload));

                segments.Add(Base64UrlEncode(headerBytes));
                segments.Add(Base64UrlEncode(payloadBytes));

                var stringToSign = string.Join(".", segments.ToArray());

                var bytesToSign = Encoding.UTF8.GetBytes(stringToSign);

                byte[] signature = HashAlgorithms[algorithm](keyBytes, bytesToSign);
                segments.Add(Base64UrlEncode(signature));

                return string.Join(".", segments.ToArray());
            }
            //exmple
            //"eyJhbGciOiJSUzI1NiIsInR5cGUiOiJKV1QifQ.
            //eyJyZWZlcmVuY2UxIjoiNiIsInJlZmVyZW5jZTIiOiI0IiwicmVmZXJlbmNlMyI6bnVsbCwicmVmZXJlbmNlNCI6IjYiLCJzY29wZSI6Imh0dHA6Ly9wZXJjZW50LmNvbXBsZXRlL21hbmdvL2FwaS90aXRhbi8iLCJhdWRpZW5jZSI6Imh0dHA6Ly9wZXJjZW50LmNvbXBsZXRlLyIsImV4cGlyYXRpb24iOjE0OTQ4MDU5ODYsImlzc3VlZF9hdCI6MTQ5NDc2OTk4NiwidmFsaWQiOmZhbHNlfQ
            //._3aBo6Y2xZ4darI9CR9Eq07jhJrEnj-KjsJfYBiszM4"
            public static titan_token Decode(string token, byte[] keyBytes, bool verify){
                if(String.IsNullOrWhiteSpace(token) || null==keyBytes) {
                    titan_token t=new titan_token();
                    t.valid=false;
                    return t;
                }
                System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
                var parts = token.Split('.');
                var header = parts[0];
                var payload = parts[1];
                byte[] crypto = Base64UrlDecode(parts[2]);
                

                var headerJSON = Encoding.UTF8.GetString(Base64UrlDecode(header));
                var payloadJSON=Encoding.UTF8.GetString(Base64UrlDecode(payload));
                jwt_header headerData = jss.Deserialize<jwt_header>(headerJSON);
                titan_token t_token=jss.Deserialize<titan_token>(payloadJSON);
                
                if (verify){
                    var bytesToSign = Encoding.UTF8.GetBytes(string.Concat(header, ".", payload));
                    //var keyBytes    = Encoding.UTF8.GetBytes(key);
                    var algorithm   = headerData.alg;

                    var signature = HashAlgorithms[GetHashAlgorithm(algorithm)](keyBytes, bytesToSign);
                    var decodedCrypto = Convert.ToBase64String(crypto);
                    var decodedSignature = Convert.ToBase64String(signature);
                    var utc0 = new DateTime(1970,1,1,0,0,0,0, DateTimeKind.Utc);
                    var now = DateTime.Now;

                    var time = (int)now.Subtract(utc0).TotalSeconds;
            
                    if (decodedCrypto != decodedSignature || time<t_token.issued_at || time>t_token.expiration ){       //invalid signatures or expirations... blow up!
                        throw new ApplicationException(string.Format("Invalid signature. Expected {0} got {1}", decodedCrypto, decodedSignature));
                    }
                    t_token.valid=true;
                }
                

                return t_token;
            }

            private static JwtHashAlgorithm GetHashAlgorithm(string algorithm){
                switch (algorithm){
                    case "RS256": return JwtHashAlgorithm.RS256;
                    case "HS384": return JwtHashAlgorithm.HS384;
                    case "HS512": return JwtHashAlgorithm.HS512;
                    default: throw new InvalidOperationException("Algorithm not supported.");
                }
            }

            // from JWT spec
            private static string Base64UrlEncode(byte[] input){
                var output = Convert.ToBase64String(input);
                output = output.Split('=')[0]; // Remove any trailing '='s
                output = output.Replace('+', '-'); // 62nd char of encoding
                output = output.Replace('/', '_'); // 63rd char of encoding
                return output;
            }

            // from JWT spec
            private static byte[] Base64UrlDecode(string input){
                var output = input;
                output = output.Replace('-', '+'); // 62nd char of encoding
                output = output.Replace('_', '/'); // 63rd char of encoding
                switch (output.Length % 4) {  // Pad with trailing '='s
                    case 0: break; // No pad chars in this case
                    case 2: output += "=="; break; // Two pad chars
                    case 3: output += "="; break; // One pad char
                    default: throw new System.Exception("Illegal base64url string!");
                }
                var converted = Convert.FromBase64String(output); // Standard base64 decoder
                return converted;
            }
        }


    }//end class
}//end namespace