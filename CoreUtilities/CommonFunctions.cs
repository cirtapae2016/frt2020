using Newtonsoft.Json;
using OfficeOpenXml;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace CoreUtilities
{
    public static class CommonFunctions
    {
        public static long Random()
        {
            long _aleatorio = -1;
            long _minimo = 1;
            long _maximo = 1000;
            Random r = new Random();
            _aleatorio = (_minimo - _maximo) * r.Next() + _maximo;
            return _aleatorio;
        }

        public static string RemoveParents(this string Text)
        {
            return Regex.Replace(Text, @"\([^()]*\)", string.Empty).Trim();
        }

        public static void CopyProperties(this object source, object destination)
        {
            PropertyInfo[] destinationProperties = destination.GetType().GetProperties();
            foreach (PropertyInfo destinationPi in destinationProperties)
            {
                PropertyInfo sourcePi = source.GetType().GetProperty(destinationPi.Name);
                destinationPi.SetValue(destination, sourcePi.GetValue(source, null), null);
            }
        }

        public static double WeightedAverage<T>(this IEnumerable<T> records, Func<T, double> value, Func<T, double> weight)
        {
            double weightedValueSum = records.Sum(x => value(x) * weight(x));
            double weightSum = records.Sum(x => weight(x));

            if (weightSum != 0)
                return weightedValueSum / weightSum;
            else
                return 0.00;
        }

        public static double ParseMath(string formula)
        {
            double result = 0;

            MathParserTK.MathParser parser = new MathParserTK.MathParser();
            try { result = parser.Parse(formula, false); }
            catch { throw; }

            return result;
        }

        public static void LogFile(string FileName, string LogText)
        {
            try
            {
                StreamWriter log = !File.Exists(FileName) ? new StreamWriter(FileName) : File.AppendText(FileName);

                log.WriteLine(LogText);

                log.Close();
            }
            catch
            {
                throw;
            }
        }

        public static void DeleteOldLogFiles(string Path)
        {
            string[] files = Directory.GetFiles(Path);

            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                if (fi.LastAccessTime < DateTime.Now.AddDays(-5))
                    fi.Delete();
            }
        }

        public static int IndexOfNth(string str, string value, int nth = 1)
        {
            if (nth <= 0)
                throw new ArgumentException("El indice debe estar entre 1 y n");

            int offset = str.IndexOf(value);
            for (int i = 1; i < nth; i++)
            {
                if (offset == -1) return -1;
                offset = str.IndexOf(value, offset + 1);
            }
            return offset;
        }

        public static double GetDoubleFromString(this string doublestring, string DecimalSeparator)
        {
            double doublenumber = 0;

            if (doublestring.Length == 0)
                return doublenumber;

            if (DecimalSeparator == ",")
            {
                if (doublestring.Contains("."))
                {
                    doublenumber = double.Parse(doublestring, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                }
                else
                {
                    doublenumber = double.Parse(doublestring, System.Globalization.CultureInfo.CurrentCulture);
                }
            }
            else
            {
                doublenumber = double.Parse(doublestring);
            }

            return doublenumber;
        }

        public static string GetStringFromDouble(this double doublenumber, int decimales = 6)
        {
            string doublestring = string.Empty, aux = string.Empty;

            try
            {
                aux = Math.Round(doublenumber, decimales).ToString();
                doublestring = aux.Replace(",", ".");
            }
            catch (Exception)
            {
                doublestring = "0.00";
            }
            return doublestring;
        }

        public static string GetStringFromDoubleDecimal(double doublenumber, int decimalprecision)
        {
            string doublestring = string.Empty, aux1 = string.Empty, aux2 = string.Empty;

            try
            {
                aux1 = doublenumber.ToString();
                aux2 = aux1.Replace(",", ".");

                if (aux2.IndexOf(".") == -1)
                    doublestring = aux2;
                else
                    doublestring = aux2.Substring(0, aux2.IndexOf(".") + decimalprecision + 1);
            }
            catch (Exception)
            {
                doublestring = "0.00";
            }
            return doublestring;
        }

        public static string CleanString(string imputText)
        {
            string result = string.Empty;

            //string pattern = @"\b[A-Z]\w*\b";

            result = Regex.Replace(imputText, "[^A-Za-z0-9 _]", "");

            return result;
        }

        public static IEnumerable<FileInfo> GetAssemblies(string directoryPlugins)
        {
            DirectoryInfo DirInfo = new DirectoryInfo(directoryPlugins);

            return from f in DirInfo.EnumerateFiles()
                   where f.Extension == ".dll" &&
                   f.Name.Contains("plugin")
                   //orderby f.CreationTime
                   select f;
        }

        private static string RequestSL(string url, Method method, dynamic body, string sessionId, out HttpStatusCode httpStatus)
        {
            string jsonBody = "";
            string output = null;

            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            var client = new RestClient(url);
            var request = new RestRequest(method);
            request.AddHeader("content-type", "application/json");

            if (body != null)
            {
                jsonBody = JsonConvert.SerializeObject(body, Formatting.None,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                });

                request.AddParameter("application/json", jsonBody, ParameterType.RequestBody);
            }

            if (!string.IsNullOrEmpty(sessionId))
            {
                foreach (var cookieItem in sessionId.Split(';'))
                {
                    string[] parts = cookieItem.Split('=');
                    if (parts.Length == 2)
                    {
                        request.AddCookie(parts[0].Trim(), parts[1].Trim());
                    }
                }

                IRestResponse response = client.Execute(request);
                httpStatus = response.StatusCode;
                output = response.Content;

                while (!string.IsNullOrEmpty(output) && output.TrimStart().StartsWith("<"))
                {
                    response = client.Execute(request);
                    httpStatus = response.StatusCode;
                    output = response.Content;
                }

                return output;
            }
            else
            {
                IRestResponse response = client.Execute(request);
                httpStatus = response.StatusCode;

                foreach (var c in response.Cookies)
                {
                    sessionId += $"{c.Name}={c.Value};";
                }

                return sessionId;
            }
        }

        public static string GET(string objectSAP, string DocKey, string args, string sessionId, out HttpStatusCode statusCode)
        {
            string uri = null;
            bool isNumeric = int.TryParse(DocKey, out _);

            if (args != null)
            {
                if (args.Substring(0, 1) != "?")
                    args = ("?" + args);
            }

            if ((DocKey != null) && (args == null))
            {
                if (isNumeric)
                {
                    uri = ServiceLayer.Address + objectSAP + "(" + DocKey + ")";
                }
                else
                {
                    uri = ServiceLayer.Address + objectSAP + "('" + DocKey + "')";
                }
            }

            if ((DocKey != null) && (args != null))
            {
                if (isNumeric)
                {
                    uri = ServiceLayer.Address + objectSAP + "(" + DocKey + ")" + args;
                }
                else
                {
                    uri = ServiceLayer.Address + objectSAP + "('" + DocKey + "')" + args;
                }
            }

            if ((DocKey == null) && (args == null))
                uri = ServiceLayer.Address + objectSAP;

            if ((DocKey == null) && (args != null))
                uri = ServiceLayer.Address + objectSAP + args;

            return RequestSL(uri, Method.GET, null, sessionId, out statusCode);
        }

        public static string POST(string objectSAP, object body, string sessionId, out HttpStatusCode statusCode)
        {
            return RequestSL(ServiceLayer.Address + objectSAP, Method.POST, body, sessionId, out statusCode);
        }

        public static string DELETE(string objectSAP, string DocKey, string sessionId, out HttpStatusCode statusCode)
        {
            return RequestSL(ServiceLayer.Address + objectSAP + "(" + DocKey + ")", Method.DELETE, null, sessionId, out statusCode);
        }


        public static string PUT(string objectSAP, object body, string DocKey, string sessionId, out HttpStatusCode statusCode)
        {
            bool result = int.TryParse(DocKey, out _);

            if (result)
            {
                return RequestSL(ServiceLayer.Address + objectSAP + "(" + DocKey + ")", Method.PUT, body, sessionId, out statusCode);
            }
            else
            {
                return RequestSL(ServiceLayer.Address + objectSAP + "('" + DocKey + "')", Method.PUT, body, sessionId, out statusCode);
            }
        }

        public static string PATCH(string objectSAP, object body, string DocKey, string sessionId, out HttpStatusCode statusCode)
        {
            bool result = int.TryParse(DocKey, out _);

            if (result)
            {
                return RequestSL(ServiceLayer.Address + objectSAP + "(" + DocKey + ")", Method.PATCH, body, sessionId, out statusCode);
            }
            else
            {
                return RequestSL(ServiceLayer.Address + objectSAP + "('" + DocKey + "')", Method.PATCH, body, sessionId, out statusCode);
            }
        }

        public static string json2xml(this string jsonString, string DtUid)

        {
            string output = null;

            List<XMLGrid.Column> _columns = new List<XMLGrid.Column>();
            List<XMLGrid.Row> _rows = new List<XMLGrid.Row>();

            dynamic jsonObject = JsonConvert.DeserializeObject(jsonString);

            var value = jsonObject.value;

            if (value == null || value.Count == 0)
                throw new Exception("La consulta no ha devuelto registros");

            foreach (var item in jsonObject.value[0])
            {
                XMLGrid.Column column = new XMLGrid.Column { Uid = item.Name, MaxLength = "100", Type = "1" };
                _columns.Add(column);
            }

            foreach (var item in jsonObject.value)
            {
                XMLGrid.Row row = new XMLGrid.Row { Cells = new XMLGrid.Cells { Cell = new List<XMLGrid.Cell>() } };

                foreach (var subitem in item)
                {
                    XMLGrid.Cell cell = new XMLGrid.Cell { ColumnUid = subitem.Name, Value = subitem.Value };
                    row.Cells.Cell.Add(cell);
                }
                _rows.Add(row);
            }

            XMLGrid.DataTable xmlGrid = new XMLGrid.DataTable
            {
                Uid = DtUid,
                Columns = new XMLGrid.Columns
                {
                    Column = _columns
                },
                Rows = new XMLGrid.Rows
                {
                    Row = _rows
                }
            };

            StringWriter stringwriter;

            using (stringwriter = new StringWriter())
            {
                var serializer = new XmlSerializer(xmlGrid.GetType());
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");

                serializer.Serialize(stringwriter, xmlGrid, ns);
            };

            return output = stringwriter.ToString();
        }

        public static string SerializeJson(this object Objeto)
        {
            string jsonBody = JsonConvert.SerializeObject(Objeto, Formatting.None,
            new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            });

            return jsonBody;
        }

        public static dynamic DeserializeJsonToDynamic(this string json)
        {
            dynamic jsonObject = JsonConvert.DeserializeObject(json);

            if (jsonObject.value != null)
            {
                json = JsonConvert.SerializeObject(jsonObject.value);
                json = json.Trim().Substring(1, json.Length - 2);
            }

            return jsonObject;
        }

        public static bool Operator(string logic, double x, double y)
        {
            switch (logic)
            {
                case ">=": return x >= y;
                case "<=": return x <= y;
                case ">": return x > y;
                case "<": return x < y;
                case "=": return x == y;
                case "!=": return x != y;
                default: throw new Exception("invalid logic");
            }
        }

        public static List<T> DeserializeList<T>(this string json)
        {
            List<T> oLista = new List<T>();
            json = json.TrimStart('"').TrimEnd('"');
            //json = json.Replace(@"\""",string.Empty);
            dynamic jsonObject = JsonConvert.DeserializeObject(json);

            try
            {
                foreach (var item in jsonObject.value)
                {
                    var Attr = DeserializeJsonObject<T>(JsonConvert.SerializeObject(item));
                    oLista.Add(Attr);
                }
            }
            catch
            {
                foreach (var item in jsonObject)
                {
                    var Attr = DeserializeJsonObject<T>(JsonConvert.SerializeObject(item));
                    oLista.Add(Attr);
                }
            }

            return oLista;
        }

        public static T DeserializeJsonObject<T>(this string json)
        {
            dynamic jsonObject = JsonConvert.DeserializeObject(json);
            if (jsonObject.value != null)
            {
                json = JsonConvert.SerializeObject(jsonObject.value);
                json = json.Trim().Substring(1, json.Length - 2);
            }

            jsonObject = JsonConvert.DeserializeObject<T>(json);
            return jsonObject;
        }

        public static string GetPeso(string ip, int port, string PesoAnt)
        {
            try
            {
                Socket s = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream,
                    ProtocolType.Tcp);
                s.Connect(ip, port);
                int milliseconds = 50;
                System.Threading.Thread.Sleep(milliseconds);
                //byte[] msg = Encoding.UTF8.GetBytes("This is a test");
                byte[] bytes = new byte[512];

                string beginstr = "";
                string peso = "";
                bool ok = false;
                while (!ok)
                {
                    beginstr = "";
                    peso = "";
                    long i = s.Receive(bytes);
                    beginstr = (Encoding.ASCII.GetString(bytes));

                    peso = beginstr.Trim();
                    peso = peso.Replace("\0", "");
                    // peso = peso.Replace("\r\n", "");

                    //beginstr = beginstr.Substring(0, 5);

                    if (peso.Contains(".kg \r\n"))
                    {
                        if (peso.LastIndexOf(".kg \r\n") > 20)
                            ok = true;
                    }
                }
                int index = peso.LastIndexOf(".kg \r\n") - 6;
                int ended = peso.LastIndexOf(".kg \r\n") + 4;
                peso = peso.Substring(index, 6);
                peso = peso.Trim();

                s.Dispose();
                return peso;
            }
            catch { 
                return PesoAnt; }
        }

        public static string ConsumoLoteCalibrado(string Lote, string DocEntryOF, string LineNum, string sessionId)
        {
            var OF = GET(ServiceLayer.ProductionOrders, DocEntryOF, null, sessionId, out _).DeserializeJsonToDynamic();

            if (OF.AbsoluteEntry != null)
            {
                var batch = DeserializeJsonObject<BatchNumberDetails>(GET(ServiceLayer.BatchNumberDetails, null, $"?$filter=Batch eq '{Lote}'", sessionId, out _));
                var bin = DeserializeJsonObject<ListadoBinsMP>(GET(ServiceLayer.ListadoBinsMP, null, $"?$filter=LOTE eq '{Lote}' ", sessionId, out _)); //and ALMACEN eq 'FRUTEXSA'

                if (bin.CANTIDADBINS > 0)
                {
                    var ListDocBatch = new List<BatchNumbers>();
                    BatchNumbers DocBatch = new BatchNumbers
                    {
                        BatchNumber = Lote,
                        SystemNumber = batch.SystemNumber,
                        Quantity = double.Parse(bin.CANTIDADBINS.ToString()),
                    };
                    ListDocBatch.Add(DocBatch);

                    var ListDocLines = new List<IDocument_Lines>();
                    IDocument_Lines DocLines = new IDocument_Lines
                    {
                        DocEntry = null,
                        BaseEntry = OF.AbsoluteEntry,
                        BaseLine = int.Parse(LineNum),
                        BaseType = "202",
                        Quantity = double.Parse(bin.CANTIDADBINS.ToString()), //bin.CANTIDADBINS,
                        BatchNumbers = ListDocBatch,
                        WarehouseCode = bin.ALMACEN
                    };
                    ListDocLines.Add(DocLines);

                    IDocuments Documents = new IDocuments
                    {
                        DocEntry = null,
                        DocDate = DateTime.Now.ToString("yyyyMMdd"),
                        DocumentLines = ListDocLines,
                    };

                    var response = POST(ServiceLayer.InventoryGenExits, Documents, sessionId, out HttpStatusCode statusCode).DeserializeJsonToDynamic();

                    if (statusCode == HttpStatusCode.Created)
                    {
                        batch.U_FRU_CantBinsDis -= 1;
                        if (!batch.U_FRU_CantBinsVol.HasValue)
                            batch.U_FRU_CantBinsVol = 0;

                        batch.U_FRU_CantBinsVol += 1;
                        PATCH(ServiceLayer.BatchNumberDetails, batch, batch.DocEntry.ToString(), sessionId, out _);

                    }
                    else
                    {
                        var objresponse = DeserializeJsonToDynamic(response);
                        return objresponse.error.message.value.ToString();
                    }
                    return response.ToString();
                }
                else
                {
                    return "El lote no tiene cantidad disponible";
                }
            }
            else
            {
                return "OF no encontrada";
            }
        }

        public static void ActualizarTotalesPorLote(int DocEntry, string sessionId)
        {
            var titulosStdGrid = new List<string>
            {
                "#",
                "Muestra[Editable]",
                "Fecha",
                "Hora"
            };

            var RegCalidad = GET(ServiceLayer.RegistroCalidad, DocEntry.ToString(), null, sessionId, out _).DeserializeJsonObject<RegistroCalidad>();

            if (RegCalidad == null)
                throw new Exception("No se encontro el registro");

            if (RegCalidad.U_BaseType == "OTRUCK")
            {
                var Recepcion = GET(ServiceLayer.Recepcion, null, $"?$filter=DocEntry eq {RegCalidad.U_BaseEntry}", sessionId, out _).DeserializeJsonObject<Recepcion>();

                foreach (var lote in RegCalidad.DFO_RQLTY3Collection)
                {
                    lote.U_Kg = Recepcion.DFO_TRUCK2Collection.Where(i => i.U_Lote == lote.U_BatchNum).Select(i => i.U_PesoLote).FirstOrDefault();
                }

                RegCalidad.U_TotalKg = RegCalidad.DFO_RQLTY3Collection.Sum(i => i.U_Kg);
            }

            var _TotByLotes = new List<RegistroCalidad_Totales_Lote>();

            foreach (var lote in RegCalidad.DFO_RQLTY3Collection)
            {
                foreach (var title in RegCalidad.DFO_RQLTY1Collection.GroupBy(i => i.U_Title))
                {
                    foreach (var attr in RegCalidad.DFO_RQLTY1Collection.Where(i => i.U_Title == title.Key).GroupBy(i => i.U_Attr))
                    {
                        if (!titulosStdGrid.Any(word => attr.Key == word))
                        {
                            if (RegCalidad.DFO_RQLTY1Collection.Where(i => i.U_Title == title.Key && i.U_Attr == attr.Key && double.TryParse(i.U_Value, out _)).Any(value => GetDoubleFromString(value.U_Value, ",") > 0.00))
                            {
                                var _tot = new RegistroCalidad_Totales_Lote();

                                _tot.DocEntry = RegCalidad.DocEntry;
                                _tot.LineId = null;
                                _tot.U_Attr = attr.Key;
                                _tot.U_Title = title.Key;
                                _tot.U_BatchNum = lote.U_BatchNum;
                                _tot.U_Value = RegCalidad.DFO_RQLTY1Collection.Where(i => i.U_Title == title.Key && i.U_Attr == attr.Key && double.TryParse(i.U_Value, out _)).WeightedAverage(i => GetDoubleFromString(i.U_Value, ","), x => RegCalidad.U_TotalKg).GetStringFromDouble(2);

                                _TotByLotes.Add(_tot);
                            }
                            else if (!double.TryParse(RegCalidad.DFO_RQLTY1Collection.Where(i => i.U_Title == title.Key && i.U_Attr == attr.Key).Select(a => a.U_Value).FirstOrDefault(), out _))
                            {
                                var _tot = new RegistroCalidad_Totales_Lote();

                                _tot.DocEntry = RegCalidad.DocEntry;
                                _tot.LineId = null;
                                _tot.U_Attr = attr.Key;
                                _tot.U_Title = title.Key;
                                _tot.U_BatchNum = lote.U_BatchNum;
                                _tot.U_Value = RegCalidad.DFO_RQLTY1Collection.Where(i => i.U_Title == title.Key && i.U_Attr == attr.Key).Select(a => a.U_Value).FirstOrDefault();

                                _TotByLotes.Add(_tot);
                            }
                        }
                    }
                }
            }

            if (_TotByLotes.Count > 0)
            {
                RegCalidad.DFO_RQLTY4Collection = _TotByLotes;
                var response = PATCH(ServiceLayer.RegistroCalidad, RegCalidad, RegCalidad.DocEntry.ToString(), sessionId, out _);
                System.Threading.Tasks.Task.Run(() => ActualizarTarjasRegCalidad(DocEntry, sessionId));
            }
        }

        private static void ActualizarTarjasRegCalidad(int DocEntry, string sessionId)
        {
            var CalibreFinal = string.Empty;
            var RegCalidad = GET(ServiceLayer.RegistroCalidad, DocEntry.ToString(), null, sessionId, out _).DeserializeJsonObject<RegistroCalidad>();

            if (RegCalidad == null)
                throw new Exception("No se encontro el registro");

            foreach (var lote in RegCalidad.DFO_RQLTY3Collection)
            {
                try
                {
                    bool hasChanges = false;
                    var Batch = GET(ServiceLayer.BatchNumberDetails, null, $"?$filter=Batch eq '{lote.U_BatchNum}'", sessionId, out _).DeserializeJsonObject<BatchNumberDetails>();

                    if (Batch != null)
                    {
                        var AttrsToLote = GET(ServiceLayer.MaestroAtributosCalidad, null, "?$filter=U_AttrLote ne ''", sessionId, out _).DeserializeList<MaestroAtributosCalidad>();

                        foreach (var total in RegCalidad.DFO_RQLTY4Collection.Where(i => i.U_BatchNum == lote.U_BatchNum))
                        {
                            foreach (var attr in AttrsToLote.Where(i => string.Equals(total.U_Attr.RemoveParents().Trim(), i.U_AttrName.Trim(), StringComparison.OrdinalIgnoreCase)))
                            {
                                PropertyInfo prop = Batch.GetType().GetProperty(attr.U_AttrLote, BindingFlags.Public | BindingFlags.Instance);
                                if (null != prop && prop.CanWrite)
                                {
                                    var isNumber = double.TryParse(total.U_Value, out double d);
                                    if (isNumber)
                                    {
                                        var url = new ServiceLayer.RegQltyByLote(Batch.Batch).url;
                                        var ListaDeREgistros = GET(url, null, null, sessionId, out _).DeserializeList<dynamic>();

                                        if (ListaDeREgistros.Count > 1)
                                        {
                                            var oldValue = prop.GetValue(Batch);
                                            if (oldValue == null)
                                            {
                                                if (prop.PropertyType.FullName == "System.String")
                                                {
                                                    prop.SetValue(Batch, total.U_Value.GetDoubleFromString(",").GetStringFromDouble(2), null);
                                                }
                                                else
                                                {
                                                    prop.SetValue(Batch, total.U_Value.GetDoubleFromString(","), null);
                                                }

                                                hasChanges = true;
                                            }
                                            else
                                            {
                                                var ListAttrsToCalc = new List<ListAttrsToProm>();

                                                foreach (var result in ListaDeREgistros.Where(i => i.Control == RegCalidad.U_PuntoControl))
                                                {
                                                    string r = GET(ServiceLayer.RegistroCalidad, result.Registro.ToString(), null, sessionId, out HttpStatusCode http);
                                                    var resultObj = r.DeserializeJsonObject<RegistroCalidad>();

                                                    var _d = new ListAttrsToProm
                                                    {
                                                        Attr = total.U_Attr.RemoveParents().Trim(),
                                                        Value = resultObj.DFO_RQLTY2Collection.Where(i => i.U_Attr.RemoveParents().Trim() == total.U_Attr.RemoveParents().Trim()).Select(i => i.U_Value.GetDoubleFromString(",")).FirstOrDefault(),
                                                        Weigth = resultObj.DFO_RQLTY3Collection.Where(i => i.U_BatchNum == Batch.Batch).Select(i => i.U_Kg).FirstOrDefault()
                                                    };

                                                    ListAttrsToCalc.Add(_d);
                                                }

                                                var newVal = ListAttrsToCalc.WeightedAverage(i => i.Value, x => x.Weigth);

                                                if (prop.PropertyType.FullName == "System.String")
                                                {
                                                    prop.SetValue(Batch, newVal.GetStringFromDouble(2), null);
                                                }
                                                else
                                                {
                                                    prop.SetValue(Batch, newVal, null);
                                                }

                                                hasChanges = true;
                                            }
                                        }
                                        else
                                        {
                                            if (prop.PropertyType.FullName == "System.String")
                                            {
                                                prop.SetValue(Batch, total.U_Value.GetDoubleFromString(",").GetStringFromDouble(2), null);
                                            }
                                            else
                                            {
                                                prop.SetValue(Batch, total.U_Value.GetDoubleFromString(","), null);
                                            }

                                            hasChanges = true;
                                        }
                                    }
                                    else
                                    {
                                        prop.SetValue(Batch, total.U_Value, null);
                                        hasChanges = true;
                                    }
                                }
                            }
                        }
                    }
                    if (hasChanges)
                    {
                        var Cliente = string.Empty;

                        if (RegCalidad.U_BaseType == "4")
                        {
                            var OT = GET(ServiceLayer.ProductionOrders, RegCalidad.U_BaseEntry, null, sessionId, out _).DeserializeJsonObject<ProductionOrder>();

                            if (!string.IsNullOrEmpty(OT.CustomerCode))
                                Cliente = GET(ServiceLayer.BusinessPartners, OT.CustomerCode, "?$select=CardName", sessionId, out _).DeserializeJsonToDynamic().CardName;

                            if (!string.IsNullOrEmpty(OT.U_FRU_Variedad))
                                Batch.U_FRU_Variedad = OT.U_FRU_Variedad;

                            if (!string.IsNullOrEmpty(OT.U_FRU_Tipo))
                                Batch.U_FRU_Tipo = OT.U_FRU_Tipo;

                            if (!string.IsNullOrEmpty(OT.U_FRU_Calibre))
                                Batch.U_FRU_Calibre = OT.U_FRU_Calibre;

                            //if (OT.ItemNo.Contains("CIR"))
                            //{
                            //    Batch.BatchAttribute2 = "CIRUELA";

                            //    if (OT.U_FRU_Tipo.Contains("ASHLOCK"))
                            //    {
                            //        var res = GET(ServiceLayer.CalibresCiruela, null, $"?$select=U_CalSc&$filter=U_Conteo eq {Math.Round(Batch.U_FRU_Conteo.GetDoubleFromString(","),0)}", sessionId, out _);
                            //        var dyn = res.DeserializeList<dynamic>();
                            //        CalibreFinal = dyn[0].U_CalSc;
                            //    }
                            //    else if (OT.U_FRU_Tipo.Contains("ELLIOT"))
                            //    {
                            //        CalibreFinal = "N/A";
                            //    }
                            //    else if (OT.U_FRU_Tipo.Contains("CON CAROZO"))
                            //    {
                            //        var res = GET(ServiceLayer.CalibresCiruela, null, $"?$select=U_CalCc&$filter=U_Conteo eq {Batch.U_FRU_Conteo}", sessionId, out _);
                            //        var dyn = res.DeserializeList<dynamic>();
                            //        CalibreFinal = dyn[0].U_CalCc;
                            //    }
                            //    else
                            //    {
                            //        var res = GET(ServiceLayer.CalibresCiruela, null, $"?$select=U_CalCn&$filter=U_Conteo eq {Batch.U_FRU_Conteo}", sessionId, out _);
                            //        var dyn = res.DeserializeList<dynamic>();
                            //        CalibreFinal = dyn[0].U_CalCn;
                            //    }
                            //}
                        }

                        //if (!string.IsNullOrEmpty(CalibreFinal))
                        //    Batch.U_FRU_Calibre = CalibreFinal;

                        Batch.BatchAttribute1 = Cliente;
                        Batch.U_FRU_Cliente = "C79749070";
                        Batch.U_FRU_NomCliente = "FRUTAS DE EXPORTACION SPA.";

                        if (Batch.U_FRU_Clasificacion == "NO CONFORME")
                            Batch.Status = "bdsStatus_Locked";


                        if (Batch.U_FRU_Clasificacion == "CONFORME")
                            Batch.Status = "bdsStatus_Released";

                        var response = PATCH(ServiceLayer.BatchNumberDetails, Batch, Batch.DocEntry.ToString(), sessionId, out HttpStatusCode statusCode);
                        if (statusCode != HttpStatusCode.NoContent)
                        {
                            var _Error = response.DeserializeJsonToDynamic();
                            throw new Exception($"Error actualizando tarja id {Batch.Batch} : {_Error.error.message.value.ToString()}");
                        }
                    }
                }
                catch
                {
                    throw;
                }
            }
        }

        public static void ActualizarTarjasByMP(string batchNum, string sessionId)
        {
            /*
             *Productor
             *Variedad
             *Tipo
             *Tipo Secado
             *Cosecha
             */
            /*
            var sql = "" +
                "select top 1 distinct H.\"BatchNum\" " +
                    "from IGN1 A" +
                    "inner join OIGN B on A.\"DocEntry\"=B.\"DocEntry\" " +
                    "inner join IBT1 C on C.\"BaseEntry\"=B.\"DocEntry\" and C.\"BaseType\"=B.\"ObjType\" and C.\"BaseLinNum\"=A.\"LineNum\" " +
                    "inner join OWOR D on D.\"DocEntry\"=A.\"BaseEntry\" " +
                    "inner join IGE1 E on E.\"BaseEntry\"=A.\"BaseEntry\" and E.\"BaseType\"=A.\"BaseType\" " +
                    "inner join OIGE G on G.\"DocEntry\"=E.\"DocEntry\" " +
                    "inner join IBT1 H on H.\"BaseEntry\"=G.\"DocEntry\" and H.\"BaseType\"=G.\"ObjType\" and H.\"BaseLinNum\"=E.\"LineNum\" " +
                    $"where C.\"BatchNum\"='{batchNum}' "
                ;
            */

            var url = new ServiceLayer.LoteMPtoCA(batchNum).url;
            var response = GET(url, null, null, sessionId, out HttpStatusCode statusCode);

            if (statusCode == HttpStatusCode.OK)
            {
                var batchMP = response.DeserializeJsonToDynamic();
                var batchSE = GET(ServiceLayer.BatchNumberDetails, null, $"?$filter=Batch eq '{batchNum}'", sessionId, out _).DeserializeJsonObject<BatchNumberDetails>();

                batchSE.U_FRU_Variedad = batchMP.U_FRU_Variedad;
                batchSE.U_FRU_Productor = batchMP.U_FRU_Productor;
                batchSE.U_FRU_NomProveedor = batchMP.U_FRU_NomProveedor;
                batchSE.U_FRU_Tipo = batchMP.U_FRU_Tipo;
                batchSE.U_FRU_TipoSecado = batchMP.U_FRU_TipoSecado;
                batchSE.ManufacturingDate = batchMP.ManufacturingDate;

                response = PATCH(ServiceLayer.BatchNumberDetails, batchSE, batchSE.DocEntry.ToString(), sessionId, out _);
            }
        }

        public static string ConsumoProductoTerminado(string LoteID, string CodigoPT, string FolioInicio, string FolioFin, string DocEntryOF, string sessionId)
        {
            var OF = DeserializeJsonToDynamic(GET(ServiceLayer.ProductionOrders, DocEntryOF, null, sessionId, out _));

            if (OF.AbsoluteEntry != null)
            {
                ProductoTerminado OLOPT = new ProductoTerminado
                {
                    U_LoteID = LoteID,
                    U_CodigoPT = CodigoPT,
                    U_DocEntryOF = OF.AbsoluteEntry
                };

                var response = POST(ServiceLayer.ProductoTerminado, OLOPT, sessionId, out HttpStatusCode statusCode);

                if (statusCode != HttpStatusCode.Created)
                {
                    var m = response.DeserializeJsonToDynamic();
                    throw new Exception($"Error grabando el registro : {m.error.message.value.ToString()}");
                }

                return response;
            }
            else
            {
                return "OF no encontrada";
            }
        }

        public static string ReciboCalibrado(string WhsCode, string DocEntry, string DocEntryOF, string Tarja, string Peso, string Conteo, string reparo, string Variedad, string CodPro, string NomPro, string sessionId)
        {
            var OF = DeserializeJsonToDynamic(GET(ServiceLayer.ProductionOrders, DocEntryOF, null, sessionId, out _));

            if (OF.AbsoluteEntry != null)
            {
                //var batch = DeserializeJsonObject<BatchNumberDetails>(GET(ServiceLayer.BatchNumberDetails, null, $"?$filter=Batch eq '{Lote}'", sessionId));
                //var bin = DeserializeJsonObject<DFO_LOTESCALIBRADO>(GET(ServiceLayer.DFO_LOTESCALIBRADO, null, $"?$filter=LOTE eq '{Lote}'", sessionId));

                //Consultar cajas asociadas a OF sin lote asignado

                DateTime date = DateTime.Now;
                string fecha = date.ToString("yyyyMMddHHmmssfff");
                string Notes = "";
                string Status = "0";
                if (reparo == "Y")
                {
                    Notes = "Aprobado con reparos";
                    Status = "2";
                }

                var ListDocBatch = new List<BatchNumbers>();

                BatchNumbers DocBatch = new BatchNumbers
                {
                    BatchNumber = Tarja,
                    Quantity = double.Parse(Peso),
                    //Status = Status,
                    U_FRU_CantBins = 1,
                    U_FRU_CantBinsDis = 1,
                    U_FRU_Conteo = Conteo,
                    U_FRU_EstadoCalid = Notes
                };

                ListDocBatch.Add(DocBatch);

                var ListDocLines = new List<IDocument_Lines>();
                IDocument_Lines DocLines = new IDocument_Lines
                {
                    BaseEntry = OF.AbsoluteEntry,
                    //BaseLine = 0,
                    BaseType = "202",
                    Quantity = double.Parse(Peso),
                    BatchNumbers = ListDocBatch,
                    WarehouseCode = WhsCode//,
                };
                ListDocLines.Add(DocLines);

                IDocuments Documents = new IDocuments
                {
                    DocDate = DateTime.Now.ToString("yyyyMMdd"),
                    DocumentLines = ListDocLines,
                };

                var response = DeserializeJsonToDynamic(POST(ServiceLayer.InventoryGenEntries, Documents, sessionId, out HttpStatusCode statusCode));

                if (response.DocEntry != null)
                {
                    var batch = DeserializeJsonObject<BatchNumberDetails>(GET(ServiceLayer.BatchNumberDetails, null, $"?$filter=Batch eq '{Tarja}'", sessionId, out _));

                    batch.U_FRU_CantBinsDis = 1;
                    batch.U_FRU_CantBins = 1;
                    batch.U_FRU_Conteo = Conteo;
                    batch.U_FRU_EstadoCalid = Notes;
                    PATCH(ServiceLayer.BatchNumberDetails, batch, batch.DocEntry.ToString(), sessionId, out _);

                    // Tarja

                    var OCAOF = DeserializeList<Calibrado>(GET(ServiceLayer.Calibrado, null, $"?$filter=DocEntry eq {DocEntry} and Remark eq null", sessionId, out _));
                    foreach (var item in OCAOF)
                    {
                        item.Remark = response.DocEntry;
                        item.U_Estado = "A";
                        PATCH(ServiceLayer.Calibrado, item, item.DocEntry, sessionId, out _);
                    }
                }
                else
                {
                    var objresponse = DeserializeJsonToDynamic(response);
                    return objresponse.error.message.value.ToString();
                }

                return response.ToString();
            }
            else
            {
                return "OF no encontrada";
            }
        }

        public static bool validarRut(string rut)
        {
            bool validacion = false;

            rut = rut.ToUpper();
            rut = rut.Replace(".", "");
            rut = rut.Replace("-", "");
            int rutAux = int.Parse(rut.Substring(0, rut.Length - 1));

            char dv = char.Parse(rut.Substring(rut.Length - 1, 1));

            int m = 0, s = 1;
            for (; rutAux != 0; rutAux /= 10)
            {
                s = (s + rutAux % 10 * (9 - m++ % 6)) % 11;
            }
            if (dv == (char)(s != 0 ? s + 47 : 75))
            {
                validacion = true;
            }

            return validacion;
        }
    }
}