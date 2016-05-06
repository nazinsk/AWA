using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace API.Models
{
    public class MongoBase<T> : IDisposable
    {
        #region Propriedades

        [NonSerialized]
        [JsonIgnore]
        MongoClient Client;

        [BsonIgnore]
        [JsonIgnore]
        public IMongoDatabase DB;

        [BsonIgnore]
        [JsonIgnore]
        IMongoCollection<BsonDocument> CollectionInternal;

        [BsonIgnore]
        [JsonIgnore]
        String ServersMongoDB
        { get { return System.Configuration.ConfigurationManager.AppSettings["ServersMongoDB"].ToString(); } }


        [BsonId]
        [BsonElement("_id")]
        [JsonIgnore]
        public ObjectId _ID
        { get; set; }

        [BsonIgnore]
        [JsonProperty("ID")]
        public String ID
        {
            get
            {
                return _ID.ToString();
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    _ID = ObjectId.Empty;
                    return;
                }

                try
                {
                    _ID = new ObjectId(value);
                }
                catch
                { }
            }
        }


        [BsonIgnore]
        [JsonIgnore]
        public bool HasData
        { get { return _ID != ObjectId.Empty; } }


        #endregion


        #region Construtores

        public MongoBase()
        { }

        public MongoBase(String ID)
        {
            this.ID = ID;
            Get();
        }

        #endregion


        #region Uteis

        bool DBStarted = false;

        void StartDB()
        {
            if (!DBStarted)
            {
                Client = new MongoClient("mongodb://" + ServersMongoDB);
                DB = Client.GetDatabase(System.Configuration.ConfigurationManager.AppSettings["DBMongo"].ToString(), null);
                DBStarted = true;
            }
        }

        void StartInternal()
        {
            StartDB();
            CollectionInternal = DB.GetCollection<BsonDocument>(this.GetType().Name, null);
            VerificaIndex();
        }

        protected IMongoCollection<T> Start()
        {
            StartDB();

            CollectionInternal = DB.GetCollection<BsonDocument>(this.GetType().Name, null);
            IMongoCollection<T> RET = DB.GetCollection<T>(this.GetType().Name, null);

            VerificaIndex();

            return RET;
        }

        protected IMongoQueryable<T> StartLINQ()
        {
            return Start().AsQueryable<T>();
        }

        public void EndDB()
        {
            if (DB != null)
                DB = null;

            if (Client != null)
                Client = null;

            DBStarted = false;
        }

        public void CleanID()
        {
            ID = "";
        }

        protected void IncorporateData(BsonDocument dado)
        {
            //BsonSerializer.Deserialize<T>(dado);

            MethodInfo[] Metodos = typeof(BsonSerializer).GetMethods();
            MethodInfo method = Metodos.First(mm => mm.Name == "Deserialize" && mm.GetParameters().Count() == 2);
            MethodInfo generic = method.MakeGenericMethod(this.GetType());

            var NewOBJ = generic.Invoke(null, new object[] { dado, null });


            foreach (var pS in NewOBJ.GetType().GetProperties())
            {
                foreach (var pT in this.GetType().GetProperties())
                {
                    if (pT.Name != pS.Name) continue;
                    if (!pT.CanWrite) continue;
                    //if (pT.GetCustomAttribute(typeof(JsonIgnoreAttribute)) != null) continue;

                    try
                    {
                        (pT.GetSetMethod()).Invoke(this, new object[] { pS.GetGetMethod().Invoke(NewOBJ, null) });
                    }
                    catch { }
                }
            };

            foreach (var P in NewOBJ.GetType().GetProperties())
            {
                try
                {
                    if (P.PropertyType.ToString().Contains("List"))
                    {
                        var typeArgs = P.PropertyType.GetGenericArguments();
                        Type d1 = typeof(List<>);
                        Type makeme = d1.MakeGenericType(typeArgs);

                        //P.SetValue(this, (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(makeme)), null);

                        //((IList)P.GetValue(this)).Add(

                        //object obj = Activator.CreateInstance(makeme);

                        //makeme.GetProperty(P.Name).SetValue(obj, "abc", null);

                        //list.Add(obj);


                        P.SetValue(this, P.GetValue(NewOBJ, null), null);

                    }
                    else
                        P.SetValue(this, P.GetValue(NewOBJ, null), null);
                }
                catch (Exception EX)
                {
                    String TXT = EX.Message;

                }
            }
        }

        protected T GetDataFromBsonDocument(BsonDocument dado)
        {
            //BsonSerializer.Deserialize<T>(dado);

            MethodInfo[] Metodos = typeof(BsonSerializer).GetMethods();
            MethodInfo method = Metodos.First(mm => mm.Name == "Deserialize" && mm.GetParameters().Count() == 2);
            MethodInfo generic = method.MakeGenericMethod(this.GetType());

            return (T)generic.Invoke(null, new object[] { dado, null });

        }

        protected void IncorporateData(object dado)
        {
            foreach (var pS in dado.GetType().GetProperties())
            {
                foreach (var pT in this.GetType().GetProperties())
                {
                    if (pT.Name != pS.Name) continue;
                    if (!pT.CanWrite) continue;
                    //if (pT.GetCustomAttribute(typeof(JsonIgnoreAttribute)) != null) continue;

                    try
                    {
                        (pT.GetSetMethod()).Invoke(this, new object[] { pS.GetGetMethod().Invoke(dado, null) });
                    }
                    catch { }
                }
            };
        }

        #endregion


        #region Indices

        struct IndiceColetado
        {
            public String Nome { get; set; }

            public List<String> Asc { get; set; }
            public List<String> Desc { get; set; }
        }

        List<IndiceColetado> GetIndices(Type obj)
        {
            List<IndiceColetado> Indices = new List<IndiceColetado>();

            foreach (var P in obj.GetProperties())
            {
                if (!P.CanWrite)
                    continue;

                DBIndex[] Att = new DBIndex[] { };

                try
                {
                    Att = (DBIndex[])P.GetCustomAttributes(typeof(DBIndex), false);
                }
                catch
                {
                    continue;
                }

                //Faz parte de algum indice
                if (Att.Length > 0)
                {
                    foreach (var I in Att)
                    {
                        IndiceColetado III;

                        if (Indices.Count(II => II.Nome == I.Nome) == 0)
                        {
                            III = new IndiceColetado();
                            III.Asc = new List<string>();
                            III.Desc = new List<string>();
                            III.Nome = I.Nome;

                            Indices.Add(III);
                        }
                        else
                            III = Indices.First(II => II.Nome == I.Nome);

                        if (I.Desc)
                            III.Desc.Add(P.Name);
                        else
                            III.Asc.Add(P.Name);
                    }
                }
                else
                {
                    try
                    {
                        if (P.PropertyType.IsClass &&
                            !P.PropertyType.IsEnum &&
                            !P.PropertyType.Name.ToLower().StartsWith("string") &&
                            !P.PropertyType.Name.ToLower().StartsWith("int") &&
                            !P.PropertyType.Name.ToLower().StartsWith("byte") &&
                            !P.PropertyType.Name.ToLower().StartsWith("object") &&
                            !P.PropertyType.Name.ToLower().StartsWith("mongo") &&
                            !P.PropertyType.Name.ToLower().StartsWith("decimal") &&
                            !P.PropertyType.Name.ToLower().StartsWith("float") &&
                            !P.PropertyType.Name.ToLower().StartsWith("datetime") &&
                            !P.PropertyType.Name.ToLower().StartsWith("date") &&
                            !P.PropertyType.Name.ToLower().StartsWith("timaspan")
                            )
                        {
                            try
                            {
                                BsonIgnoreAttribute[] AttBI = (BsonIgnoreAttribute[])P.GetCustomAttributes(typeof(BsonIgnoreAttribute), false);

                                if (AttBI.Length == 0)
                                {
                                    List<IndiceColetado> SubIndices = GetIndices(P.PropertyType);

                                    for (int i = 0; i < SubIndices.Count; i++)
                                    {
                                        for (int ii = 0; ii < SubIndices[i].Asc.Count; ii++)
                                            SubIndices[i].Asc[ii] = P.Name + "." + SubIndices[i].Asc[ii];

                                        for (int iii = 0; iii < SubIndices[i].Desc.Count; iii++)
                                            SubIndices[i].Desc[iii] = P.Name + "." + SubIndices[i].Desc[iii];
                                    }
                                    Indices.AddRange(SubIndices);
                                }
                            }
                            catch { }
                        }
                    }
                    catch
                    { }
                }
            }

            return Indices;
        }

        IndiceColetado GetIndiceChaves(Type obj)
        {
            IndiceColetado RET = new IndiceColetado();
            RET.Nome = "KEY";
            RET.Asc = new List<string>();
            RET.Desc = new List<string>();

            foreach (var P in obj.GetProperties())
            {
                DBKey[] Att = new DBKey[] { };

                try
                {
                    Att = (DBKey[])P.GetCustomAttributes(typeof(DBKey), false);
                }
                catch
                {
                    continue;
                }

                if (Att.Length > 0)
                    foreach (var I in Att)
                        RET.Asc.Add(P.Name);

            }

            return RET;
        }

        void VerificaIndex()
        {
            List<IndiceColetado> Indices = GetIndices(this.GetType());
            IndiceColetado CHAVE = GetIndiceChaves(this.GetType());

            if (CHAVE.Asc.Count > 0)
                Indices.Add(CHAVE);

            foreach (var I in Indices)
            {
                String INDEX = "{";

                foreach (var II in I.Asc)
                    INDEX += (INDEX.Length > 1 ? ", " : "") + II + ": 1";

                foreach (var II in I.Desc)
                    INDEX += (INDEX.Length > 1 ? ", " : "") + II + ": -1";

                INDEX += "}";

                CollectionInternal.Indexes.CreateOneAsync(INDEX);

                //if (!String.IsNullOrEmpty(I.Nome) && I.Nome == "ILocalizacao")
                //{
                //    String NomeCampo = I.Asc.First();
                //    Collection.Indexes.CreateOneAsync(INDEX);
                //}
                //else
                //    Collection.Indexes.CreateOneAsync(INDEX);
                //    //Collection.CreateIndex(IndexKeys.GeoSpatialSpherical(NomeCampo), IndexOptions.SetName(I.Nome));
                //    }
                //    else
                //        Collection.CreateIndex(IDX, IndexOptions.SetName(I.Nome));
                //}
                //else
                //    Collection.Indexes.CreateOneAsync(IDX.);

            }
        }

        #endregion


        #region Chaves

        async Task CheckKeys(bool Chanding)
        {
            List<String> FieldsKey = new List<string>();

            foreach (var P in this.GetType().GetProperties())
            {
                DBKey[] Att = (DBKey[])P.GetCustomAttributes(typeof(DBKey), false);

                //Faz parte da chave
                if (Att.Length > 0)
                    FieldsKey.Add(P.Name);
            }

            if (FieldsKey.Count == 0)
                return;

            FilterDefinition<BsonDocument> FILTER = null;

            foreach (var C in FieldsKey)
            {
                String dado = "";

                if (this.GetType().GetProperty(C).GetValue(this) != null)
                    dado = this.GetType().GetProperty(C).GetValue(this).ToString();

                if (!String.IsNullOrEmpty(dado))
                {
                    if (FILTER == null)
                        FILTER = Builders<BsonDocument>.Filter.Eq(C, BsonRegularExpression.Create(new Regex(dado, RegexOptions.IgnoreCase)));
                    else
                        FILTER &= Builders<BsonDocument>.Filter.Eq(C, BsonRegularExpression.Create(new Regex(dado, RegexOptions.IgnoreCase)));
                }
            }

            //Se for alteração, já vem com o ID, então tem no DB
            if (Chanding)
            {
                if (FILTER == null)
                    FILTER = Builders<BsonDocument>.Filter.Ne("_id", _ID);
                else
                    FILTER &= Builders<BsonDocument>.Filter.Ne("_id", _ID);
            }

            long NumberItens = await CollectionInternal.CountAsync(FILTER);

            if (NumberItens > 0)
            {
                List<String> CamposChaveLabel = new List<string>();

                foreach (var CC in FieldsKey)
                    CamposChaveLabel.Add("\"" + CC + "\"");

                throw new DBKeyExists(CamposChaveLabel);
            }

        }

        #endregion


        #region Funcoes PRONTAS 

        /// <summary>
        /// Get Item - ID from parameter
        /// </summary>
        /// <returns></returns>
        public bool Get(String id)
        {
            ID = id;
            return Get();
        }

        /// <summary>
        /// Get Item - Need ID
        /// </summary>
        /// <returns></returns>
        public bool Get()
        {
            Start();

            FilterDefinition<BsonDocument> FILTRO = Builders<BsonDocument>.Filter.Eq("_id", _ID);

            var Query = CollectionInternal.Find(FILTRO);
            var Dado = Query.ToListAsync();
            Dado.Wait();
            EndDB();

            if (Dado.Result.Count > 0)
            {
                IncorporateData(Dado.Result.First());
                return true;
            }
            else
                CleanID();

            return false;
        }


        /// <summary>
        /// Update Data Not in Object - Need ID
        /// </summary>
        /// <returns></returns>
        public void UpdateData()
        {
            Start();

            FilterDefinition<BsonDocument> FILTRO = Builders<BsonDocument>.Filter.Eq("_id", _ID);

            var Query = CollectionInternal.Find(FILTRO);
            var Dado = Query.ToListAsync();
            Dado.Wait();
            EndDB();

            if (Dado.Result.Count > 0)
            {
                T DataDB = GetDataFromBsonDocument(Dado.Result[0]);

                foreach (var P in typeof(T).GetProperties())
                {
                    if (!P.CanWrite)
                        continue;

                    var OldData = P.GetValue(DataDB);
                    var NewData = P.GetValue(this);

                    if (NewData == null || NewData.ToString() == "" || NewData.ToString() == "null")
                        P.SetValue(this, OldData);
                }
            }
            else
                CleanID();
        }




        void DefaultData(Type T, Object DATA)
        {
            foreach (var P in T.GetProperties())
            {
                if (!P.CanWrite)
                    continue;

                var NewData = P.GetValue(DATA);

                if (P.PropertyType.Name.ToLower().StartsWith("string") &&
                    (NewData == null || NewData.ToString() == "" || NewData.ToString() == "null"))
                { P.SetValue(DATA, ""); }
                //else if (P.PropertyType.Name.ToLower().StartsWith("int") || P.PropertyType.Name.ToLower().StartsWith("long") &&
                //    (NewData == null || NewData.ToString() == "" || NewData.ToString() == "null"))
                //    { P.SetValue(DATA, 0); }
                //else if (P.PropertyType.Name.ToLower().StartsWith("decimal") || P.PropertyType.Name.ToLower().StartsWith("float") &&
                //    (NewData == null || NewData.ToString() == "" || NewData.ToString() == "null"))
                //    { P.SetValue(DATA, 0); }
                //else if (P.PropertyType.Name.ToLower().StartsWith("datetime") &&
                //    (NewData == null || NewData.ToString() == "" || NewData.ToString() == "null" ||
                //        ((DateTime)NewData).ToString("dd/mm/yyyy") == "01/01/0001"))
                //    { P.SetValue(DATA, null); }
                else if (NewData == null || NewData.ToString() == "" || NewData.ToString() == "null")
                {
                    if (P.PropertyType.IsValueType || P.PropertyType.IsClass)
                    {
                        object DATA_DEFAULT = Activator.CreateInstance(P.PropertyType);
                        if (!P.PropertyType.IsGenericType)
                            DefaultData(P.PropertyType, DATA_DEFAULT);
                        P.SetValue(DATA, DATA_DEFAULT);
                    }
                }


            }
        }


        /// <summary>
        /// Save - All Data - Insert ou Update all fields
        /// </summary>
        /// <returns></returns>
        public virtual async Task Save()
        {
            Start();

            if (!HasData)
            {
                await CheckKeys(false);

                DefaultData(typeof(T), this);

                //Insere
                BsonDocument A_INSERIR = this.ToBsonDocument();
                await CollectionInternal.InsertOneAsync(A_INSERIR);
                _ID = A_INSERIR["_id"].AsObjectId;
            }
            else
            {
                await CheckKeys(true);
                UpdateData();
                //Atualiza
                await CollectionInternal.FindOneAndReplaceAsync(Builders<BsonDocument>.Filter.Eq("_id", _ID), this.ToBsonDocument());
            }

            EndDB();
        }

        /// <summary>
        /// Update - Update only fields in 'FieldsName' parameter
        /// </summary>
        /// <returns></returns>
        public virtual async Task Update(params string[] FieldsName)
        {
            Start();

            if (!HasData)
                throw new NotFountException();

            await CheckKeys(true);

            UpdateData();

            BsonDocument ToChange = new BsonDocument();

            foreach (var item in FieldsName)
                ToChange.Add(item, (BsonValue)this.GetType().GetProperty(item).GetValue(this));

            await CollectionInternal.FindOneAndReplaceAsync(Builders<BsonDocument>.Filter.Eq("_id", _ID), ToChange);

            EndDB();
        }


        /// <summary>
        /// Delete - Remove from Database
        /// </summary>
        /// <returns></returns>
        public virtual async Task Delete()
        {
            if (HasData)
            {
                Start();
                await CollectionInternal.DeleteOneAsync(Builders<BsonDocument>.Filter.Eq("_id", _ID));
                EndDB();
            }
            else
            {
                throw new Exception("Não existe!");
            }
        }

        #endregion


        public void Dispose()
        {
            EndDB();
        }
    }

    

    #region Atributos DB

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DBKey : System.Attribute
    { }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class DBIndex : System.Attribute
    {
        //public DBIndex()
        //{
        //}

        //public DBIndex(String nome)
        //{
        //    this.Nome = nome;
        //}

        //public DBIndex(String nome, bool desc)
        //{
        //    this.Nome = nome;
        //    this.Desc = desc;
        //}
        public String Nome
        { get; set; }

        public bool Desc
        { get; set; }
    }

    public class DBIgnoreMongo : BsonIgnoreAttribute
    { }

    #endregion


    #region Exceptions

    public class NotFountException : Exception
    { }

    public class DBKeyExists : Exception
    {
        public List<String> NameFields { get; set; }

        public DBKeyExists(List<String> NameFields)
        {
            this.NameFields = NameFields;
        }

        public override string ToString()
        {
            StringBuilder SB = new StringBuilder();

            foreach (var NC in NameFields)
            {
                if (SB.Length > 0)
                    SB.Append(" ");

                SB.Append(NC);
            }

            return SB.ToString();
        }
    }

    #endregion

}
