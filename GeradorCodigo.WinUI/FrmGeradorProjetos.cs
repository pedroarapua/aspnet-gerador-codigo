using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.DirectoryServices;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data.Common;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;

namespace GeradorCodigo.WinUI
{
    public partial class FrmGeradorProjetos : Form
    {
        #region     .....:::::     ATRIBUTOS     :::::.....

        private DbConnection connection;
        private static String connStrSqlWindows = "Data Source = {0}; {1} Integrated Security=True;";
        private static String connStrSql = "Data Source = {0}; {1} User Id={2}; Password={3}";
        private static String connStrOracle = "Data Source={0};User Id={1};Password={2};";
        private static String connStrOracleWindows = "Data Source={0};Integrated Security=SSPI;";
        private static String connStrOleDb = "Provider=MSDAORA.1;Data Source={0};Persist Security Info=False; User ID={1};password={2};";
        private static String connStrOleDbWindows = "Provider = OraOLEDB.Oracle; DataSource={0}; OSAuthent=1;";
        private const String prefixoProcedure = "MAG_SP_MM_";
        private List<TabelaVO> tabelas;
        
        #endregion

        #region     .....:::::     PROPRIEDADES     :::::.....

        /// <summary>
        /// Pega o driver do banco, convertendo para o tipo EDriverBanco
        /// </summary>
        public EDriverBanco DriverBanco
        {
            get
            {
                Int32 codigo = Convert.ToInt32(cboDriver.SelectedValue);
                return (EDriverBanco)codigo;
            }
        }

        /// <summary>
        /// Busca a conexão do driver especificado
        /// </summary>
        public DbConnection Connection
        { 
            get 
            {
                if (connection == null)
                {
                    switch(DriverBanco)
                    {
                        case EDriverBanco.SqlServer:
                            connection = new SqlConnection(GetConnectionStringSql());
                            break;
                        case EDriverBanco.Oracle:
                            connection = new OleDbConnection(GetConnectionStringOracle());
                            break;
                    }
                }

                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                return connection;
            }
            set
            {
                connection = value;
            }
        }

        /// <summary>
        /// Propriedade que armazena as tabelas do banco selecionado
        /// </summary>
        public List<TabelaVO> Tabelas
        {
            get
            {
                if (this.tabelas == null)
                    this.tabelas = new List<TabelaVO>();
                return tabelas;
            }
            set
            {
                tabelas = value;
            }
        }

        /// <summary>
        /// Proprieade que armazena informações da solução
        /// </summary>
        public SolucaoVO Solucao { get; set; }

        #endregion
        
        #region     .....:::::     CONSTRUTORES     :::::.....

        public FrmGeradorProjetos()
        {
            InitializeComponent();
            LoadDrivers();
        }

        #endregion

        #region     .....:::::     MÉTODOS     :::::.....

        /// <summary>
        /// Carrega os drivers possíveis para conexão com a base de dados
        /// </summary>
        private void LoadDrivers()
        {
            cboDriver.DisplayMember = "Nome";
            cboDriver.ValueMember = "Codigo";
            cboDriver.BindingContext = new BindingContext();

            List<DriverVO> lstDrivers = new List<DriverVO>();
            lstDrivers.Add(new DriverVO() { Nome = "Selecione...", Codigo = 0 });
            lstDrivers.Add(new DriverVO() { Nome = "Oracle", Codigo = 1 });
            lstDrivers.Add(new DriverVO() { Nome = "Sql Server", Codigo = 2 });

            cboDriver.DataSource = lstDrivers;
        }

        /// <summary>
        /// Carrega os bancos de dados existentes no servidor com o driver específico
        /// </summary>
        private void LoadBancoDeDados()
        {
            EDriverBanco driver = (EDriverBanco)Convert.ToInt32(cboDriver.SelectedValue);
            List<BancoVO> lstBancos = new List<BancoVO>();
            try
            {
                switch (driver)
                {
                    case EDriverBanco.SqlServer:
                        lstBancos = GetBancoDeDadosSqlServer();
                        break;
                    case EDriverBanco.Oracle:
                        lstBancos = GetBancoDeDadosOracle();
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro");
            }
            
            cboBanco.DisplayMember = "Nome";
            cboBanco.ValueMember = "Nome";
            cboBanco.BindingContext = new BindingContext();
            cboBanco.DataSource = lstBancos;
        }

        /// <summary>
        /// Limpa os campos de conectar
        /// </summary>
        private void LimparCamposConnectar()
        {
            cboDriver.SelectedValue = 0;
            txtServidor.Text = String.Empty;
            txtLogin.Text = String.Empty;
            txtSenha.Text = String.Empty;
            lblStatus.Text = "Offiline";
            lblStatus.ForeColor = Color.Red;
            chkTipoAutenticacao.Checked = false;
            Tabelas = new List<TabelaVO>();
        }

        /// <summary>
        /// Limpa os campos de conectar
        /// </summary>
        private void LimparCamposGerarCodigo()
        {
            cboBanco.SelectedIndex = 0;
            txtPath.Text = String.Empty;
            chkGerarInterfaces.Checked = false;
            txtPrefixoTabela.Text = String.Empty;
            txtPrefixoColuna.Text = String.Empty;
            txtSeparadorTabela.Text = String.Empty;
            txtSeparadorColuna.Text = String.Empty;
            txtSolucao.Text = String.Empty;
        }

        /// <summary>
        /// Habilita ou Desabilita os campos para conectar na base de dados
        /// </summary>
        /// <param name="enabled"></param>
        private void EnableCamposConnectar(Boolean enabled)
        {
            cboDriver.Enabled = enabled;
            txtServidor.Enabled = enabled;
            chkTipoAutenticacao.Enabled = enabled;
            txtLogin.Enabled = enabled;
            txtSenha.Enabled = enabled;
            
            txtLogin.Enabled = enabled;
            txtLogin.Enabled = enabled;
            txtLogin.Enabled = enabled;
            txtLogin.Enabled = enabled;

            btnConnectar.Text = enabled ? "Conectar" : "Desconectar";
        }

        /// <summary>
        /// Habilita ou Desabilita os campos de especificação de geração de código
        /// </summary>
        /// <param name="enabled"></param>
        private void EnableCamposGerarCodigo(Boolean enabled)
        {
            cboBanco.Enabled = enabled && DriverBanco != EDriverBanco.Oracle;
            txtPath.Enabled = enabled;
            btnSelecionar.Enabled = enabled;
            btnGerarCodigo.Enabled = enabled;
            txtPrefixoColuna.Enabled = enabled;
            txtPrefixoTabela.Enabled = enabled;
            txtSeparadorTabela.Enabled = enabled;
            txtSeparadorColuna.Enabled = enabled;
            txtSolucao.Enabled = enabled;
        }

        /// <summary>
        /// Retorna o tipo de dados do driver especificado
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private ETipoColuna GetTypeDataBase(object obj)
        {
            
            ETipoColuna retorno = ETipoColuna.Nenhum;
            switch(DriverBanco)
            {
                case EDriverBanco.Oracle:
                    retorno = GetTypeDataBaseOracle(Convert.ToInt32(obj));
                    break;
                case EDriverBanco.SqlServer:
                    retorno = GetTypeDataBaseSqlServer(obj.ToString());
                    break;
            }
            
            return retorno;
        }

        /// <summary>
        /// Retorna o tipo de dado do driver oracle
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private ETipoColuna GetTypeDataBaseOracle(Int32 obj)
        {
            ETipoColuna retorno = ETipoColuna.Nenhum;
            switch (Convert.ToInt32(obj))
            {
                case 131:
                    retorno = ETipoColuna.Numeric;
                    break;
                case 129:
                    retorno = ETipoColuna.Varchar;
                    break;
                case 139:
                    retorno = ETipoColuna.Number;
                    break;
                case 128:
                    retorno = ETipoColuna.Blob;
                    break;
                case 135:
                    retorno = ETipoColuna.DateTime;
                    break;
                case 110:
                    retorno = ETipoColuna.Int;
                    break;
                case 109:
                    retorno = ETipoColuna.BigInt;
                    break;
                case 112:
                    retorno = ETipoColuna.Decimal;
                    break;
                case 130:
                    retorno = ETipoColuna.NClob;
                    break;
                case 13:
                    retorno = ETipoColuna.Decimal;
                    break;
                case 113:
                    retorno = ETipoColuna.Char;
                    break;
            }
            return retorno;
        }

        /// <summary>
        /// Retorna o tipo de dado do driver Sql Server
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private ETipoColuna GetTypeDataBaseSqlServer(String obj)
        {
            ETipoColuna retorno = ETipoColuna.Nenhum;
            switch (obj.ToString())
            {
                case "numeric":
                    retorno = ETipoColuna.Numeric;
                    break;
                case "varchar":
                    retorno = ETipoColuna.Varchar;
                    break;
                case "datetime":
                    retorno = ETipoColuna.DateTime;
                    break;
                case "int":
                    retorno = ETipoColuna.Int;
                    break;
                case "bigint":
                    retorno = ETipoColuna.BigInt;
                    break;
                case "decimal":
                    retorno = ETipoColuna.Decimal;
                    break;
                case "money":
                    retorno = ETipoColuna.Money;
                    break;
                case "char":
                    retorno = ETipoColuna.Char;
                    break;
                case "ntext":
	                retorno = ETipoColuna.NText;
	                break;
                case "nchar":
                    retorno = ETipoColuna.NChar;
                    break;
                case "nvarchar":
                    retorno = ETipoColuna.Varchar;
                    break;
                case "real":
                    retorno = ETipoColuna.Real;
                    break;
                case "smalldatetime":
                	retorno = ETipoColuna.SmallDateTime;
                    break;
                case "smallint":
                	retorno = ETipoColuna.SmallInt;
                    break;
                case "smallmoney":
                	retorno = ETipoColuna.SmallMoney;
                    break;
                case "sql_variant":
                	retorno = ETipoColuna.SqlVariant;
                    break;
                case "text":
                	retorno = ETipoColuna.Text;
                    break;
                case "timestamp":
                    retorno = ETipoColuna.TimeStamp;
                    break;
                case "tinyint":
                	retorno = ETipoColuna.TinyInt;
                    break;
                case "uniqueidentifier":
                	retorno = ETipoColuna.UniqueIdentifier;
                    break;
                case "varbinary":
                	retorno = ETipoColuna.VarBinary;
                    break;
                case "xml":
                	retorno = ETipoColuna.Xml;
                    break;
                case "binary":
                	retorno = ETipoColuna.Binary;
                    break;
                case "bit":
                	retorno = ETipoColuna.Bit;
                    break;
                case "float":
                	retorno = ETipoColuna.Float;
                    break;
                case "image":
                	retorno = ETipoColuna.Image;
                    break;
            }
            return retorno;
        }

        /// <summary>
        /// Carrega as tabelas do banco de dados com o driver específico
        /// </summary>
        private void LoadTabelas()
        {
            try
            {
                DataTable dtTables = GetSchemaDataBase();
                
                // iterate through the rows and write the info to the console window
                foreach (DataRow dr in dtTables.Rows)
                {
                    TabelaVO tabela = new TabelaVO() { Nome = dr["TABLE_NAME"].ToString() };
                    DataTable dtColumns = Connection.GetSchema("Columns", new[] { null, null, tabela.Nome });
                    List<String> lstPrimaryKey = GetSchemaPrimaryKey(tabela.Nome);
                    
                    foreach (DataRow row in dtColumns.Rows)
                    {
                        ColunaVO coluna = new ColunaVO() { Nome = row["COLUMN_NAME"].ToString(), TipoColuna = GetTypeDataBase(row["DATA_TYPE"])};
                        coluna.IsChavePrimaria = lstPrimaryKey.Contains(coluna.Nome);
                        coluna.Tamanho = GetTamanhoColuna(row, coluna.TipoColuna);
                        coluna.Scala = GetScalaColuna(row, coluna.TipoColuna);
                        coluna.IsNullable = GetIsNullable(row);
                        tabela.Colunas.Add(coluna);
                    }

                    Tabelas.Add(tabela);
                }

                foreach (TabelaVO tabela in Tabelas)
                {
                    List<ForeignKeyVO> lstForeignKey = GetSchemaForeignKey(tabela.Nome);
                    foreach (ColunaVO coluna in tabela.Colunas)
                    {
                        ForeignKeyVO foreignKey = lstForeignKey.Find(delegate(ForeignKeyVO foreignKey1) { return foreignKey1.NomeColuna == coluna.Nome; });
                        coluna.IsChaveEstrangeira = foreignKey != null;
                        coluna.ChaveEstrangeira = foreignKey;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro ao carregas tabelas");
            }
            finally
            {
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();
            }

            foreach (TabelaVO tabela in Tabelas)
            {
                TreeNode nodeTabela = new TreeNode(tabela.Nome);
                nodeTabela.Name = tabela.Nome;
                foreach (ColunaVO coluna in tabela.Colunas)
                {
                    String texto = coluna.ToText();
                    TreeNode nodeColuna = new TreeNode(texto);
                    nodeColuna.Name = coluna.Nome;
                    nodeTabela.Nodes.Add(nodeColuna);
                }
                treeTabelasCampos.Nodes.Add(nodeTabela);
            }
        }

        /// <summary>
        /// Remove todos os nós do treeview para ser adicionado novos.
        /// </summary>
        private void RemoveAllNodes()
        {
            while (treeTabelasCampos.Nodes.Count > 0)
            {
                treeTabelasCampos.Nodes.RemoveAt(0);
            }
        }

        /// <summary>
        /// Retorna o tamanho da coluna do driver especificado
        /// </summary>
        /// <param name="row"></param>
        private Int64? GetTamanhoColuna(DataRow row, ETipoColuna tipo)
        {
            Int64? retorno = null;
            switch (DriverBanco)
            {
                case EDriverBanco.Oracle:
                    retorno = GetTamanhoColunaOracle(row, tipo);
                    break;
                case EDriverBanco.SqlServer:
                    retorno = GetTamanhoColunaSqlServer(row, tipo);
                    break;
            }
            return retorno;
        }

        /// <summary>
        /// Retorna o tamanho da coluna do Sql Server
        /// </summary>
        /// <param name="row"></param>
        private Int64? GetTamanhoColunaSqlServer(DataRow row, ETipoColuna tipo)
        {
            Int64? retorno = null;
            switch (tipo)
            {
                case ETipoColuna.Numeric:
                    retorno = Convert.ToInt64(row[11]);
                    break;
                case ETipoColuna.Int:
                    retorno = Convert.ToInt64(row[11]);
                    break;
                case ETipoColuna.BigInt:
                    retorno = Convert.ToInt64(row[11]);
                    break;
                case ETipoColuna.Decimal:
                    retorno = Convert.ToInt64(row[11]);
                    break;
                case ETipoColuna.Money:
                    retorno = Convert.ToInt64(row[11]);
                    break;
                case ETipoColuna.DateTime:
                    retorno = null;
                    break;
                case ETipoColuna.Varchar:
                    retorno = Convert.ToInt64(row[9]);
                    break;
                case ETipoColuna.Char:
                    retorno = Convert.ToInt64(row[9]);
                    break;
            }
            return retorno;
        }

        /// <summary>
        /// Retorna o tamanho da coluna do Oracle
        /// </summary>
        /// <param name="row"></param>
        private Int64? GetTamanhoColunaOracle(DataRow row, ETipoColuna tipo)
        {
            Int64? retorno = null;
            switch (tipo)
            {
                case ETipoColuna.Numeric:
                    if (row[15] != DBNull.Value)
                        retorno = Convert.ToInt64(row[15]);
                    break;
                case ETipoColuna.Int:
                    if (row[15] != DBNull.Value)
                        retorno = Convert.ToInt64(row[15]);
                    break;
                case ETipoColuna.BigInt:
                    if (row[15] != DBNull.Value)
                        retorno = Convert.ToInt64(row[15]);
                    break;
                case ETipoColuna.Decimal:
                    if(row[15] != DBNull.Value)
                        retorno = Convert.ToInt64(row[15]);
                    break;
                case ETipoColuna.Money:
                    if (row[15] != DBNull.Value)
                        retorno = Convert.ToInt64(row[15]);
                    break;
                case ETipoColuna.DateTime:
                    retorno = null;
                    break;
                case ETipoColuna.Varchar:
                    if (row[13] != DBNull.Value)
                        retorno = Convert.ToInt64(row[13]);
                    break;
                case ETipoColuna.Char:
                    if (row[13] != DBNull.Value)
                        retorno = Convert.ToInt64(row[13]);
                    break;
            }
            return retorno;
        }

        /// <summary>
        /// Retorna o se a coluna pode ser nula dependendo do driver especificado
        /// </summary>
        /// <param name="row"></param>
        private Boolean GetIsNullable(DataRow row)
        {
            Boolean retorno = false;
            switch (DriverBanco)
            {
                case EDriverBanco.Oracle:
                    retorno = GetIsNullableOracle(row);
                    break;
                case EDriverBanco.SqlServer:
                    retorno = GetIsNullableSqlServer(row);
                    break;
            }
            return retorno;
        }

        /// <summary>
        /// Retorna se a coluna pode ser nula no driver Oracle
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private Boolean GetIsNullableOracle(DataRow row)
        {
            Boolean retorno = false;
            if (row[10] != DBNull.Value)
                retorno = Convert.ToBoolean(row[10]);
            
            return retorno;
        }

        /// <summary>
        /// Retorna se a coluna pode ser nula no driver Sql Server
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private Boolean GetIsNullableSqlServer(DataRow row)
        {
            Boolean retorno = false;
            if (row[6] != DBNull.Value)
                retorno = row[6].ToString() == "YES";

            return retorno;
        }

        /// <summary>
        /// Retorna o tamanho da scala do driver especificado
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private Int32? GetScalaColuna(DataRow row, ETipoColuna tipo)
        {
            Int32? retorno = null;

            switch (DriverBanco)
            {
                case EDriverBanco.Oracle:
                    retorno = GetScalaColunaOracle(row, tipo);
                    break;
                case EDriverBanco.SqlServer:
                    retorno = GetScalaColunaSqlServer(row, tipo);
                    break;
            }
            return retorno;
        }

       
        /// <summary>
        /// Retorna o tamanho da scala do Sql Server
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private Int32? GetScalaColunaSqlServer(DataRow row, ETipoColuna tipo)
        {
            Int32? retorno = null; 
            switch (tipo)
            {
                case ETipoColuna.Numeric:
                    retorno = Convert.ToInt32(row[12]);
                    break;
                case ETipoColuna.Int:
                    retorno = null;
                    break;
                case ETipoColuna.BigInt:
                    retorno = null;
                    break;
                case ETipoColuna.Decimal:
                    retorno = Convert.ToInt32(row[12]);
                    break;
                case ETipoColuna.Money:
                    if (row[12] != DBNull.Value)
                        retorno = Convert.ToInt32(row[12]);
                    break;
                case ETipoColuna.DateTime:
                    retorno = null;
                    break;
                case ETipoColuna.Varchar:
                    retorno = null;
                    break;
                case ETipoColuna.Char:
                    retorno = null;
                    break;
            }
            return retorno;
        }

        /// <summary>
        /// Retorna o tamanho da scala do Oracle
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private Int32? GetScalaColunaOracle(DataRow row, ETipoColuna tipo)
        {
            Int32? retorno = null;
            if (row[16] != DBNull.Value)
                retorno = Convert.ToInt32(row[16]);
            //switch (tipo)
            //{
            //    case ETipoColuna.Numeric:
            //        if (row[16] != DBNull.Value)
            //            retorno = Convert.ToInt32(row[16]);
            //        break;
            //    case ETipoColuna.Int:
            //        retorno = null;
            //        break;
            //    case ETipoColuna.BigInt:
            //        retorno = null;
            //        break;
            //    case ETipoColuna.Decimal:
            //        if (row[16] != DBNull.Value)
            //            retorno = Convert.ToInt32(row[16]);
            //        break;
            //    case ETipoColuna.Money:
            //        if (row[16] != DBNull.Value)
            //            retorno = Convert.ToInt32(row[16]);
            //        break;
            //    case ETipoColuna.DateTime:
            //        retorno = null;
            //        break;
            //    case ETipoColuna.Varchar:
            //        retorno = null;
            //        break;
            //}
            return retorno;
        }


        /// <summary>
        /// Busca as tabelas do schema do driver especificado
        /// </summary>
        private DataTable GetSchemaDataBase()
        {
            DataTable dt = new DataTable();
            switch (DriverBanco)
            {
                case EDriverBanco.Oracle:
                    dt = GetSchemaDataBaseOracle();
                    break;
                case EDriverBanco.SqlServer:
                    dt = GetSchemaDataBaseSqlServer();
                    break;
            }
            return dt;
        }

        /// <summary>
        /// Busca as chaves primárias do schema do driver especificado
        /// </summary>
        private List<String> GetSchemaPrimaryKey(String tabela)
        {
            List<String> lstRetorno = new List<String>();
            switch (DriverBanco)
            {
                case EDriverBanco.Oracle:
                    lstRetorno = GetSchemaPrimaryKeyOracle(tabela);
                    break;
                case EDriverBanco.SqlServer:
                    lstRetorno = GetSchemaPrimaryKeySqlServer(tabela);
                    break;
            }
            return lstRetorno;
        }

        /// <summary>
        /// Busca as chaves estrangeiras do schema do driver especificado
        /// </summary>
        private List<ForeignKeyVO> GetSchemaForeignKey(String tabela)
        {
            List<ForeignKeyVO> lstRetorno = new List<ForeignKeyVO>();
            switch (DriverBanco)
            {
                case EDriverBanco.Oracle:
                    lstRetorno = GetSchemaForeignKeyOracle(tabela);
                    break;
                case EDriverBanco.SqlServer:
                    lstRetorno = GetSchemaForeignKeySqlServer(tabela);
                    break;
            }
            return lstRetorno;
        }

        /// <summary>
        /// Busca as tabelas do schema do driver Oracle
        /// </summary>
        /// <returns></returns>
        private DataTable GetSchemaDataBaseOracle()
        {
            string[] restrictions = new string[4];
            restrictions[1] = txtLogin.Text;
            restrictions[3] = "Table";

            DataTable dtTables = new DataTable();
            try
            {
                dtTables = Connection.GetSchema("Tables", restrictions);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();
            }
            return dtTables;
        }

        /// <summary>
        /// Busca as tabelas do schema do driver Sql Server
        /// </summary>
        /// <returns></returns>
        private DataTable GetSchemaDataBaseSqlServer()
        {
            DataTable dtTables = new DataTable();
            try
            {
                dtTables = Connection.GetSchema("Tables");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();
            }
            return dtTables;
        }

        /// <summary>
        /// Busca as chaves primárias do schema do driver SqlServer
        /// </summary>
        /// <returns></returns>
        private List<String> GetSchemaPrimaryKeySqlServer(String tabela)
        {
            List<String> lstRetorno = new List<String>();
            try
            {
                DbCommand command = Connection.CreateCommand();
                command.CommandText = "sp_pkeys";
                command.CommandType = CommandType.StoredProcedure;
                DbParameter parameter = command.CreateParameter();
                parameter.DbType = DbType.String;
                parameter.Direction = ParameterDirection.Input;
                parameter.ParameterName = "@table_name";
                parameter.Value = tabela;
                command.Parameters.Add(parameter);
                DbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    lstRetorno.Add(reader[3].ToString());
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();
            }
            return lstRetorno;
        }

        /// <summary>
        /// Busca as chaves primárias do schema do driver SqlServer
        /// </summary>
        /// <returns></returns>
        private List<ForeignKeyVO> GetSchemaForeignKeySqlServer(String tabela)
        {
            List<ForeignKeyVO> lstRetorno = new List<ForeignKeyVO>();
            try
            {
                DbCommand command = Connection.CreateCommand();
                command.CommandText = "sp_fkeys";
                command.CommandType = CommandType.StoredProcedure;
                DbParameter parameter = command.CreateParameter();
                parameter.DbType = DbType.String;
                parameter.Direction = ParameterDirection.Input;
                parameter.ParameterName = "@fktable_name";
                parameter.Value = tabela;
                command.Parameters.Add(parameter);
                DbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    TabelaVO tabelaAux = Tabelas.Find(delegate(TabelaVO t) { return t.Nome == reader["pktable_name"].ToString(); });
                    if (tabelaAux != null)
                    {
                        ForeignKeyVO foreignKey = new ForeignKeyVO();
                        foreignKey.Nome = reader["fk_name"].ToString();
                        foreignKey.NomeColuna = reader["fkcolumn_name"].ToString();
                        foreignKey.Tabela = tabelaAux;
                        lstRetorno.Add(foreignKey);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();
            }
            return lstRetorno;
        }

        /// <summary>
        /// Busca as chaves primárias do schema do driver Oracle
        /// </summary>
        /// <returns></returns>
        private List<String> GetSchemaPrimaryKeyOracle(String tabela)
        {
            List<String> lstRetorno = new List<String>();
            try
            {
                DataTable mySchema = (Connection as OleDbConnection).GetOleDbSchemaTable(OleDbSchemaGuid.Primary_Keys, new Object[] { null, txtLogin.Text, tabela });
                int columnOrdinalForName = mySchema.Columns["COLUMN_NAME"].Ordinal;

                foreach (DataRow r in mySchema.Rows)
                {
                    lstRetorno.Add(r.ItemArray[columnOrdinalForName].ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();
            }
            return lstRetorno;
        }

        /// <summary>
        /// Busca as chaves estrangeiras do schema do driver Oracle
        /// </summary>
        /// <returns></returns>
        private List<ForeignKeyVO> GetSchemaForeignKeyOracle(String tabela)
        {
            List<ForeignKeyVO> lstRetorno = new List<ForeignKeyVO>();
            try
            {
                DataTable mySchema = (Connection as OleDbConnection).GetOleDbSchemaTable(OleDbSchemaGuid.Foreign_Keys, new Object[] { null, txtLogin.Text, tabela });
                
                foreach (DataRow r in mySchema.Rows)
                {
                    TabelaVO tabelaAux = Tabelas.Find(delegate(TabelaVO t) { return t.Nome == r["FK_TABLE_NAME"].ToString(); });
                    if (tabelaAux != null)
                    {
                        ForeignKeyVO foreignKey = new ForeignKeyVO();
                        foreignKey.NomeColuna = r["FK_COLUMN_NAME"].ToString();
                        foreignKey.Nome = r["FK_NAME"].ToString();
                        foreignKey.Tabela = tabelaAux;
                        lstRetorno.Add(foreignKey);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();
            }
            return lstRetorno;
        }

        /// <summary>
        /// Carrega os bancos de dados Sql Server do servidor
        /// </summary>
        private List<BancoVO> GetBancoDeDadosSqlServer()
        {
            String connectionString = GetConnectionString();
            List<BancoVO> lstBancos = new List<BancoVO>();
            try
            {
                DataTable dtBancos = Connection.GetSchema("Databases");
                Connection.Close();

                lstBancos.Add(new BancoVO() { Nome = "Selecione..." });
                foreach (DataRow row in dtBancos.Rows)
                {
                    lstBancos.Add(new BancoVO() { Nome = row["database_name"].ToString() });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();
            }

            if (lstBancos.Count == 0)
                lstBancos.Add(new BancoVO() { Nome = "[Nenhum]" });

            return lstBancos;
        }

        /// <summary>
        /// Carrega os bancos de dados Oracle do servidor
        /// </summary>
        private List<BancoVO> GetBancoDeDadosOracle()
        {
            List<BancoVO> lstBancos = new List<BancoVO>();
            lstBancos.Add(new BancoVO() { Nome = "[Nenhum]" });
            return lstBancos;
        }

        /// <summary>
        /// Método que retorna a string de conexão dependendo do driver utilizado
        /// </summary>
        /// <returns></returns>
        private String GetConnectionString()
        {
            String connectionString = String.Empty;
            switch(DriverBanco)
            {
                case EDriverBanco.Oracle:
                    connectionString = GetConnectionStringOracleConfig();
                    break;
                case EDriverBanco.SqlServer:
                    connectionString = GetConnectionStringSql();
                    break;
            }
            return connectionString;
        }


        /// <summary>
        /// Método que retorna a string de conexão do banco SqlServer
        /// </summary>
        /// <returns></returns>
        private String GetConnectionStringSql()
        {
            String connectionString = String.Empty;
            if (chkTipoAutenticacao.Checked)
                connectionString = String.Format(FrmGeradorProjetos.connStrSqlWindows, txtServidor.Text, cboBanco.SelectedValue.ToString() == "Selecione..." ? String.Empty : String.Format("Database={0};", cboBanco.SelectedValue.ToString()));
            else
                connectionString = String.Format(FrmGeradorProjetos.connStrSql, txtServidor.Text, cboBanco.SelectedValue == null || cboBanco.SelectedValue.ToString() == "Selecione..." || cboBanco.SelectedValue.ToString() == "[Nenhum]" ? String.Empty : String.Format("Database={0};", cboBanco.SelectedValue.ToString()), txtLogin.Text, txtSenha.Text);
            
            return connectionString;
        }

        /// <summary>
        /// Método que retorna a string de conexão do banco Oracle
        /// </summary>
        /// <returns></returns>
        private String GetConnectionStringOracle()
        {
            String connectionString = String.Empty;
            if (chkTipoAutenticacao.Checked)
                connectionString = String.Format(FrmGeradorProjetos.connStrOleDbWindows, txtServidor.Text);
            else
                connectionString = String.Format(FrmGeradorProjetos.connStrOleDb, txtServidor.Text, txtLogin.Text, txtSenha.Text);

            return connectionString;
        }

        /// <summary>
        /// Método que retorna a string de conexão do banco Oracle
        /// </summary>
        /// <returns></returns>
        private String GetConnectionStringOracleConfig()
        {
            String connectionString = String.Empty;
            if (chkTipoAutenticacao.Checked)
                connectionString = String.Format(FrmGeradorProjetos.connStrOracleWindows, txtServidor.Text);
            else
                connectionString = String.Format(FrmGeradorProjetos.connStrOracle, txtServidor.Text, txtLogin.Text, txtSenha.Text);

            return connectionString;
        }

        /// <summary>
        /// Testa a conexão dependendo do driver especificado
        /// </summary>
        private void TestarConexao()
        {
            EDriverBanco driver = (EDriverBanco)Convert.ToInt32(cboDriver.SelectedValue);
            switch (driver)
            {
                case EDriverBanco.SqlServer:
                    TestarConexaoSql();
                    break;
                case EDriverBanco.Oracle:
                    TestarConexaoOracle();
                    break;
            }
        }

        /// <summary>
        /// Testa a conexão do banco Sql Server
        /// </summary>
        private void TestarConexaoSql()
        {
            try
            {
                using (SqlConnection conexao = new SqlConnection(GetConnectionStringSql()))
                {
                    conexao.Open();
                    conexao.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Testa a conexão do banco Oracle
        /// </summary>
        private void TestarConexaoOracle()
        {
            try
            {
                using (OleDbConnection connection = new OleDbConnection(GetConnectionStringOracle()))
                {
                    connection.Open();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Marca os checkbox dos nós filhos
        /// </summary>
        /// <param name="listaNos"></param>
        /// <param name="valor"></param>
        private void MarcarNosPosteriores(TreeNodeCollection listaNos, bool valor)
        {
            foreach (TreeNode n in listaNos)
            {
                n.Checked = valor;

                if (n.Nodes.Count > 0)
                    this.MarcarNosPosteriores(n.Nodes, valor);
            }
        }

        /// <summary>
        /// Marca o checkbox do nó pai
        /// </summary>
        /// <param name="listaNos"></param>
        /// <param name="valor"></param>
        private void MarcarNodesAcima(TreeNode node, bool valor)
        {
            if(!valor)
            {
                foreach (TreeNode n in node.Parent.Nodes)
                {
                    if (n.Checked)
                    {
                        valor = true;
                        break;
                    }
                    else
                        valor = false;
                }
            }
            node.Parent.Checked = valor;
        }

        /// <summary>
        /// Gera as entidades checadas no treeview com seus atributos e propriedades
        /// </summary>
        private void GerarProjetoEntidades()
        {
            StreamWriter stream = null;
            try
            {
                ProjetoVO projeto = new ProjetoVO() { Nome = ETipoProjeto.Entidades.ToString(), Tipo = ETipoProjeto.Entidades, Namespace = "Entidades" };
                String pathEntidades = String.Concat(txtPath.Text, String.Format(@"\{0}\Entidades", Solucao.Nome));
                Directory.CreateDirectory(@pathEntidades);
                foreach (TreeNode nodeTabela in treeTabelasCampos.Nodes)
                {
                    if (nodeTabela.Checked)
                    {
                        TabelaVO tabela = Tabelas.Find(delegate(TabelaVO t) { return t.Nome == nodeTabela.Name; });
                        
                        if (tabela != null)
                        {
                            #region      .....:::::     PREENCHIMENTO DE INFORMAÇÕES DA CLASSE     :::::.....

                            ClasseVO classe = new ClasseVO() { Tabela = tabela };
                            if (!String.IsNullOrEmpty(txtPrefixoTabela.Text))
                                classe.Nome = tabela.Nome.Substring(Convert.ToInt32(txtPrefixoTabela.Text)).ToLower();
                            else
                                classe.Nome = tabela.Nome.ToLower();

                            String separador = txtSeparadorTabela.Text.ToLower();

                            if (!String.IsNullOrEmpty(separador))
                            {
                                int index = classe.Nome.IndexOf(separador);
                                if (index < 0 && classe.Nome.Length > 1)
                                    classe.Nome = String.Concat(classe.Nome.Substring(0, 1).ToUpper(), classe.Nome.Substring(1));

                                while (index >= 0)
                                {
                                    if (index < classe.Nome.Length)
                                    {
                                        String strAntes = classe.Nome.Substring(0, index + 1);
                                        strAntes = strAntes.Replace(separador, String.Empty);
                                        String strApos = classe.Nome.Substring(index + 1);
                                        if (strAntes.Length > 1)
                                        {
                                            strAntes = String.Concat(strAntes.Substring(0, 1).ToUpper(), strAntes.Substring(1));
                                        }
                                        if (strApos.Length > 1)
                                        {
                                            strApos = String.Concat(strApos.Substring(0, 1).ToUpper(), strApos.Substring(1));
                                        }
                                        classe.Nome = String.Concat(strAntes, strApos);
                                    }
                                    index = classe.Nome.ToLower().IndexOf(separador);
                                }
                            }
                            else
                            {
                                if (classe.Nome.Length > 1)
                                    classe.Nome = String.Concat(classe.Nome.Substring(0, 1).ToUpper(), classe.Nome.Substring(1));
                            }
                           
                            #endregion

                            #region     .....:::::     PREENCHIMENTO DE INFORMAÇÕES DAS PROPRIEDADES     :::::.....

                            foreach (TreeNode nodeColuna in nodeTabela.Nodes)
                            {
                                if (nodeColuna.Checked)
                                {
                                    ColunaVO coluna = tabela.Colunas.Find(delegate(ColunaVO c) { return c.Nome == nodeColuna.Name; });
                                    if (coluna != null)
                                    {
                                        PropriedadeVO propriedade = new PropriedadeVO() { Nullable = coluna.GetVariavelNullable(), Tipo = coluna.GetTipoVariavel(), Coluna = coluna };

                                        String separadorColuna = txtSeparadorColuna.Text.ToLower();
                                        if (!String.IsNullOrEmpty(txtPrefixoColuna.Text))
                                            propriedade.Nome = coluna.Nome.Substring(Convert.ToInt32(txtPrefixoColuna.Text)).ToLower();
                                        else
                                            propriedade.Nome = coluna.Nome.ToLower();

                                        if (!String.IsNullOrEmpty(separadorColuna) && coluna.ChaveEstrangeira == null)
                                        {
                                            int index = propriedade.Nome.IndexOf(separadorColuna);
                                            if (index < 0 && propriedade.Nome.Length > 1)
                                                propriedade.Nome = String.Concat(propriedade.Nome.Substring(0, 1).ToUpper(), propriedade.Nome.Substring(1));

                                            while (index >= 0)
                                            {
                                                if (index < propriedade.Nome.Length)
                                                {
                                                    String strAntes = propriedade.Nome.Substring(0, index + 1);
                                                    strAntes = strAntes.Replace(separadorColuna, String.Empty);
                                                    String strApos = propriedade.Nome.Substring(index + 1);

                                                    if (strAntes.Length > 1)
                                                    {
                                                        strAntes = String.Concat(strAntes.Substring(0, 1).ToUpper(), strAntes.Substring(1));
                                                    }
                                                    if (strApos.Length > 1)
                                                    {
                                                        strApos = String.Concat(strApos.Substring(0, 1).ToUpper(), strApos.Substring(1));
                                                    }

                                                    propriedade.Nome = String.Concat(strAntes.ToUpper(), strApos);
                                                }
                                                index = propriedade.Nome.ToLower().IndexOf(separadorColuna);
                                            }
                                        }
                                        else
                                        {
                                            if(propriedade.Nome.Length > 1)
                                                propriedade.Nome = String.Concat(propriedade.Nome.Substring(0, 1).ToUpper(), propriedade.Nome.Substring(1));
                                        }

                                        classe.Propriedades.Add(propriedade);
                                    }
                                }
                            }

                            #endregion

                            projeto.Classes.Add(classe);
                        }
                    }
                }

                foreach (ClasseVO classe in projeto.Classes)
                {
                    foreach (PropriedadeVO prop in classe.Propriedades)
                    {
                        if (prop.Coluna.ChaveEstrangeira != null)
                        {
                            ClasseVO classeAux = projeto.Classes.Find(delegate(ClasseVO c) { return c.Tabela.Nome == prop.Coluna.ChaveEstrangeira.Tabela.Nome; });
                            prop.Coluna.ChaveEstrangeira.Classe = classeAux;
                            int count = classe.Propriedades.Count(x=> x.Nome == prop.Nome);
                            prop.Nome = String.Format("{0}{1}", classeAux.Nome, count == 1 ? String.Empty : count.ToString());
                        }
                    }
                }

                #region     .....:::::     CRIA AS ENTIDADES NO REPOSITÓRIO     :::::.....

                foreach (ClasseVO classe in projeto.Classes)
                {
                    String nomeClasse = String.Concat(classe.Nome, "VO.cs");
                    String pathClasse = String.Format(@"{0}\{1}", pathEntidades, nomeClasse);
                    stream = new StreamWriter(pathClasse, true);

                    #region     .....:::::     ESCREVE AS REFERENCIAS UTILIZADAS     :::::.....

                    stream.WriteLine("using System;");
                    stream.WriteLine("using System.Collections.Generic;");
                    stream.WriteLine("using System.Text;");
                    stream.WriteLine("using System.Data;");
                    stream.WriteLine("using Db.Persistence.Utils;");
                    
                    #endregion

                    stream.WriteLine(String.Format("namespace {0}{1}", String.IsNullOrEmpty(Solucao.Nome) ? String.Empty : String.Concat(Solucao.Nome, "."), projeto.Nome));
                    stream.WriteLine("{");
                    stream.WriteLine(String.Format("\t[ClassAnnotation(TableName = \"{0}\")]", classe.Tabela.Nome));
                    stream.WriteLine(String.Format("\tpublic class {0}VO", classe.Nome));
                    stream.WriteLine("\t{");

                    if (classe.Propriedades.Count > 0)
                    {
                        #region     .....:::::     ESCREVE OS ATRIBUTOS DA CLASSE     :::::.....

                        stream.WriteLine("");
                        stream.WriteLine("\t\t#region     .....:::::     ATRIBUTOS     :::::.....");
                        stream.WriteLine("");

                        foreach (PropriedadeVO propriedade in classe.Propriedades)
                        {
                            String nomeAtributo = String.Concat(propriedade.Nome.Substring(0,1).ToLower(), propriedade.Nome.Substring(1));
                            stream.WriteLine(String.Format("\t\tprivate {0} _{1};", propriedade.ToTextTipoVariavel(), nomeAtributo));
                        }

                        stream.WriteLine("");
                        stream.WriteLine("\t\t#endregion");
                        stream.WriteLine("");

                        #endregion

                        #region     .....:::::     ESCREVE AS PROPRIEDADES DA CLASSE     :::::.....

                        stream.WriteLine("\t\t#region     .....:::::     PROPRIEDADES     :::::.....");
                        stream.WriteLine("");

                        foreach (PropriedadeVO propriedade in classe.Propriedades)
                        {
                            String nomeAtributo = String.Concat(propriedade.Nome.Substring(0, 1).ToLower(), propriedade.Nome.Substring(1));
                            String get = String.Concat("get { return this._", nomeAtributo, "; } ");
                            String set = String.Concat("set { this._", nomeAtributo, " = value; }");

                            stream.WriteLine(String.Format("\t\t[FieldAnnotation( ColumName = \"{0}\"{1} {2} )]", propriedade.Coluna.Nome, propriedade.Coluna.IsChavePrimaria ? String.Format(", IsPrimaryKey = {0}", propriedade.Coluna.IsChavePrimaria.ToString().ToLower()) : String.Empty, propriedade.Coluna.IsChavePrimaria ? String.Format(", IsForeignKey = {0}", propriedade.Coluna.IsChaveEstrangeira.ToString().ToLower()) : String.Empty));
                            stream.WriteLine(String.Concat("\t\tpublic ", propriedade.ToTextTipoVariavel(), " ", propriedade.Nome));
                            stream.WriteLine("\t\t{");
                            stream.WriteLine(String.Concat("\t\t\t", get));
                            stream.WriteLine(String.Concat("\t\t\t", set));
                            stream.WriteLine("\t\t}");
                            stream.WriteLine("");
                        }

                        stream.WriteLine("\t\t#endregion");
                        stream.WriteLine("");

                        #endregion

                        #region     .....:::::     ESCREVE OS ENUMERADORES DA CLASSE     :::::.....

                        //stream.WriteLine("\t\t#region     .....:::::     ENUMERADORES     :::::.....");
                        //stream.WriteLine("");
                        //stream.WriteLine("\t\tpublic enum Atributos");
                        //stream.WriteLine("\t\t{");

                        //Int32 index = 1;
                        //foreach (PropriedadeVO prop in classe.Propriedades)
                        //{
                        //    stream.WriteLine(String.Format("\t\t\t{0}{1}{2}", classe.Nome, prop.Nome, index == classe.Propriedades.Count ? String.Empty : ","));
                        //    index++;

                        //}

                        //stream.WriteLine("\t\t}");
                        //stream.WriteLine("");
                        //stream.WriteLine("\t\t#endregion");

                        #endregion

                        #region     .....:::::     ESCREVE CONSTRUTORES DA CLASSE     :::::.....

                        stream.WriteLine("");
                        stream.WriteLine("\t\t#region     .....:::::     CONSTRUTORES     :::::.....");
                        stream.WriteLine("");

                        stream.WriteLine(String.Concat(String.Format("\t\tpublic {0}VO()", classe.Nome), "{}")); // Construtor padrão

                        //stream.WriteLine("");
                        //stream.WriteLine(String.Format("\t\tpublic {0}VO(DataRow row)", classe.Nome));
                        //stream.WriteLine("\t\t{");
                        //foreach (PropriedadeVO prop in classe.Propriedades)
                        //{
                        //    stream.WriteLine(String.Format("\t\t\tthis._{0} = Utils.GetValue<{1}>(row, Atributos.{2}{3}.ToString());", prop.Nome.ToAtributo(), prop.ToTextTipoVariavel(), classe.Nome, prop.Nome));
                        //}

                        //stream.WriteLine("\t\t}");
                        stream.WriteLine("");
                        stream.WriteLine("\t\t#endregion");

                        #endregion
                    }

                    stream.WriteLine("\t}");
                    stream.WriteLine("}");

                    stream.Close();
                }

                if (projeto.Classes.Count > 0)
                {
                    #region     .....:::::     CRIA ARQUIVO ASSEMBLYINFO     :::::.....

                    String pathProperties = String.Concat(pathEntidades, @"\Properties");
                    Directory.CreateDirectory(pathProperties);
                    stream = new StreamWriter(String.Concat(pathProperties, @"\AssemblyInfo.cs"), true);
                    
                    stream.WriteLine("using System.Reflection;");
                    stream.WriteLine("using System.Runtime.CompilerServices;");
                    stream.WriteLine("using System.Runtime.InteropServices;");
                    stream.WriteLine("");
                    stream.WriteLine("");
                    stream.WriteLine("");
                    stream.WriteLine(String.Format("[assembly: AssemblyTitle(\"{0}\")]", projeto.Nome));
                    stream.WriteLine("[assembly: AssemblyDescription(\"\")]");
                    stream.WriteLine("[assembly: AssemblyConfiguration(\"\")]");
                    stream.WriteLine("[assembly: AssemblyCompany(\"P.H.F. Dias Software\")]");
                    stream.WriteLine(String.Format("[assembly: AssemblyProduct(\"{0}\")]", projeto.Nome));
                    stream.WriteLine("[assembly: AssemblyCopyright(\"Copyright © P.H.F. Dias Software 2012\")]");
                    stream.WriteLine("[assembly: AssemblyTrademark(\"\")]");
                    stream.WriteLine("[assembly: AssemblyCulture(\"\")]");
                    stream.WriteLine("");
                    stream.WriteLine("[assembly: ComVisible(false)]");
                    stream.WriteLine("");
                    stream.WriteLine(String.Format("[assembly: Guid(\"{0}\")]",projeto.Guid.ToString()));
                    stream.WriteLine("");
                    stream.WriteLine("");
                    stream.WriteLine("[assembly: AssemblyVersion(\"1.0\")]");
                    stream.WriteLine("[assembly: AssemblyFileVersion(\"1.0\")]");
                    
                    stream.Close();

                    #endregion

                    #region     .....:::::     CRIA ARQUIVO DE PROJETO     :::::.....

                    String nomeProjeto = String.Format("{0}.csproj", ETipoProjeto.Entidades.ToString());
                    String pathProjeto = String.Format(@"{0}\{1}", pathEntidades, nomeProjeto);
                    
                    stream = new StreamWriter(pathProjeto, true);
                    stream.WriteLine("<Project DefaultTargets=\"Build\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">");
                    
                    stream.WriteLine("\t<PropertyGroup>");
                    stream.WriteLine("\t\t<SchemaVersion>2.0</SchemaVersion>");
                    stream.WriteLine(String.Format("\t\t<ProjectGuid>{0}</ProjectGuid>", String.Concat("{", projeto.Guid.ToString(), "}")));
                    stream.WriteLine("\t\t<OutputType>Library</OutputType>");
                    stream.WriteLine("\t\t<AppDesignerFolder>Properties</AppDesignerFolder>");
                    stream.WriteLine(String.Format("\t\t<RootNamespace>{0}{1}</RootNamespace>", String.IsNullOrEmpty(Solucao.Nome) ? String.Empty : String.Concat(Solucao.Nome, "."), projeto.Namespace));
                    stream.WriteLine(String.Format("\t\t<AssemblyName>{0}{1}</AssemblyName>", String.IsNullOrEmpty(Solucao.Nome) ? String.Empty : String.Concat(Solucao.Nome, "."), projeto.Namespace));
                    stream.WriteLine("\t\t<SccProjectName></SccProjectName>");
                    stream.WriteLine("\t\t<SccLocalPath></SccLocalPath>");
                    stream.WriteLine("\t\t<SccAuxPath></SccAuxPath>");
                    stream.WriteLine("\t\t<SccProvider></SccProvider>");
                    stream.WriteLine("\t</PropertyGroup>");
                    
                    stream.WriteLine("\t<PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' \">");
                    stream.WriteLine("\t\t<DebugSymbols>true</DebugSymbols>");
                    stream.WriteLine("\t\t<DebugType>full</DebugType>");
                    stream.WriteLine("\t\t<Optimize>false</Optimize>");
                    stream.WriteLine("\t\t<OutputPath>bin\\Debug\\</OutputPath>");
                    stream.WriteLine("\t\t<DefineConstants>DEBUG;TRACE</DefineConstants>");
                    stream.WriteLine("\t\t<ErrorReport>prompt</ErrorReport>");
                    stream.WriteLine("\t\t<WarningLevel>4</WarningLevel>");
                    stream.WriteLine("\t</PropertyGroup>");
                    
                    stream.WriteLine("\t<PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' \">");
                    stream.WriteLine("\t\t<DebugType>pdbonly</DebugType>");
                    stream.WriteLine("\t\t<Optimize>true</Optimize>");
                    stream.WriteLine("\t\t<OutputPath>bin\\Release\\</OutputPath>");
                    stream.WriteLine("\t\t<DefineConstants>TRACE</DefineConstants>");
                    stream.WriteLine("\t\t<ErrorReport>prompt</ErrorReport>");
                    stream.WriteLine("\t\t<WarningLevel>4</WarningLevel>");
                    stream.WriteLine("\t</PropertyGroup>");

                    if (projeto.Classes.Count > 0)
                    {
                        projeto.Classes.Add(new ClasseVO(){ Nome = "AssemblyInfo"});
                        stream.WriteLine("\t<ItemGroup>");
                        
                        foreach(ClasseVO classe in projeto.Classes)
                            stream.WriteLine(String.Format("\t\t<Compile Include=\"{0}{1}.cs\" />", classe.Nome == "AssemblyInfo" ? @"Properties\" : String.Empty, String.Concat(classe.Nome, classe.Nome == "AssemblyInfo" ? String.Empty : "VO")));
                        
                        stream.WriteLine("\t</ItemGroup>");
                    }
                    
                    stream.WriteLine("\t<ItemGroup>");
                    stream.WriteLine("\t\t<Reference Include=\"Db.Persistence.Utils, Version=1.0.0.0, Culture=neutral, processorArchitecture=MSIL\">");
                    stream.WriteLine("\t\t\t<SpecificVersion>False</SpecificVersion>");
                    stream.WriteLine("\t\t\t<HintPath>..\\AcessoDados\\dlls\\Db.Persistence.Utils.dll</HintPath>");
                    stream.WriteLine("\t\t</Reference>");
                    stream.WriteLine("\t\t<Reference Include=\"System\" />");
                    stream.WriteLine("\t\t<Reference Include=\"System.Data\" />");
                    stream.WriteLine("\t\t<Reference Include=\"System.Xml\" />");
                    stream.WriteLine("\t</ItemGroup>");
                    
                    stream.WriteLine("\t<Import Project=\"$(MSBuildBinPath)\\Microsoft.CSharp.targets\" />");
                    stream.WriteLine("</Project>");

                    stream.Close();
                    #endregion
                }

                #endregion

                Solucao.Projetos.Add(projeto);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (stream != null)
                    stream.Dispose();
            }
        }

        /// <summary>
        /// Gera as métodos e classes utilizadas em vários projetos
        /// </summary>
        private void GerarProjetoUtilitarios()
        {
            StreamWriter stream = null;
            try
            {
                ProjetoVO projeto = new ProjetoVO() { Nome = ETipoProjeto.Utilitarios.ToString(), Tipo = ETipoProjeto.Utilitarios, Namespace = "Utilitarios" };
                String pathUtilitarios = String.Concat(txtPath.Text, String.Format(@"\{0}\Utilitarios", Solucao.Nome));
                Directory.CreateDirectory(@pathUtilitarios);
                ClasseVO classe = null;
                String pathClasse = String.Empty;

                #region     .....:::::     CRIA AS ENTIDADES NO REPOSITÓRIO     :::::.....

                #region     .....:::::     CRIA CLASSE UTILS     :::::.....

                classe = new ClasseVO() { Nome = "Utils" };

                pathClasse = String.Format(@"{0}\{1}.cs", pathUtilitarios, classe.Nome);
                stream = new StreamWriter(pathClasse, true);

                #region     .....:::::     ESCREVE AS REFERENCIAS UTILIZADAS     :::::.....

                stream.WriteLine("using System;");
                stream.WriteLine("using System.Collections.Generic;");
                stream.WriteLine("using System.Text;");
                stream.WriteLine("using System.Data;");

                #endregion

                stream.WriteLine(String.Format("namespace {0}{1}", String.IsNullOrEmpty(Solucao.Nome) ? String.Empty : String.Concat(Solucao.Nome, "."), projeto.Nome));
                stream.WriteLine("{");
                stream.WriteLine(String.Format("\tpublic static class {0}", classe.Nome));
                stream.WriteLine("\t{");
                stream.WriteLine("\t\t#region     .....:::::     MÉTODOS     :::::.....");
                stream.WriteLine("");
                stream.WriteLine("\t\tpublic static T GetValue<T>(DataRow row, string columnName)");
                stream.WriteLine("\t\t{");

                stream.WriteLine("\t\t\tobject result = null;");
                stream.WriteLine("\t\t\tobject value = null;");

                stream.WriteLine("\t\t\ttry");
                stream.WriteLine("\t\t\t{");
                stream.WriteLine("\t\t\t\tif(row.Table.Columns.Contains(columnName))");
                stream.WriteLine("\t\t\t\t\tvalue = row[columnName];");
                stream.WriteLine("\t\t\t}");
                stream.WriteLine("\t\t\tcatch (IndexOutOfRangeException ex)");
                stream.WriteLine("\t\t\t{");
                stream.WriteLine("\t\t\t\tvalue = null;");
                stream.WriteLine("\t\t\t}");
                stream.WriteLine("\t\t\tcatch (ArgumentException ex)");
                stream.WriteLine("\t\t\t{");
                stream.WriteLine("\t\t\t\tvalue = null;");
                stream.WriteLine("\t\t\t}");
                
			    stream.WriteLine("\t\t\tif (value == DBNull.Value || value == null)");
                stream.WriteLine("\t\t\t{");
                stream.WriteLine("\t\t\t\tresult = default(T);");
                stream.WriteLine("\t\t\t}");
                stream.WriteLine("\t\t\telse");
                stream.WriteLine("\t\t\t{");
                stream.WriteLine("\t\t\t\tresult = value;");
                stream.WriteLine("\t\t\t}");

                stream.WriteLine("");
                stream.WriteLine("\t\t\treturn (T)result;");

                stream.WriteLine("\t\t}");
                stream.WriteLine("");
                stream.WriteLine("\t\t#endregion");
                stream.WriteLine("\t}");
                stream.WriteLine("}");
                stream.Close();

                projeto.Classes.Add(classe);

                #endregion

                #region     .....:::::     CRIA CLASSE CLASSANNOTATION     :::::.....

                classe = new ClasseVO() { Nome = "ClassAnnotation" };

                pathClasse = String.Format(@"{0}\{1}.cs", pathUtilitarios, classe.Nome);
                stream = new StreamWriter(pathClasse, true);

                #region     .....:::::     ESCREVE AS REFERENCIAS UTILIZADAS     :::::.....

                stream.WriteLine("using System;");
                stream.WriteLine("using System.Collections.Generic;");
                stream.WriteLine("using System.Text;");
                stream.WriteLine("using System.Data;");

                #endregion

                stream.WriteLine(String.Format("namespace {0}{1}", String.IsNullOrEmpty(Solucao.Nome) ? String.Empty : String.Concat(Solucao.Nome, "."), projeto.Nome));
                stream.WriteLine("{");
                stream.WriteLine(String.Format("\tpublic class {0} : System.Attribute", classe.Nome));
                stream.WriteLine("\t{");
                
                stream.WriteLine("\t\t#region     .....:::::     ATRIBUTOS     :::::.....");
                stream.WriteLine("");
                stream.WriteLine("\t\tprivate String _tableName;");
                stream.WriteLine("");
                stream.WriteLine("\t\t#endregion");
                
                stream.WriteLine("\t\t#region     .....:::::     PROPRIEDADES     :::::.....");
                stream.WriteLine("");
                stream.WriteLine("\t\tpublic String TableName");
                stream.WriteLine("\t\t{");
                stream.WriteLine("\t\t\tget { return this._tableName; }");
                stream.WriteLine("\t\t\tset { this._tableName = value; }");
                stream.WriteLine("\t\t}");
                stream.WriteLine("");
                stream.WriteLine("\t\t#endregion");

                
                stream.WriteLine("\t}");
                stream.WriteLine("}");
                stream.Close();

                projeto.Classes.Add(classe);

                #endregion

                #region     .....:::::     CRIA CLASSE FIELDANNOTATION     :::::.....

                classe = new ClasseVO() { Nome = "FieldAnnotation" };

                pathClasse = String.Format(@"{0}\{1}.cs", pathUtilitarios, classe.Nome);
                stream = new StreamWriter(pathClasse, true);

                #region     .....:::::     ESCREVE AS REFERENCIAS UTILIZADAS     :::::.....

                stream.WriteLine("using System;");
                stream.WriteLine("using System.Collections.Generic;");
                stream.WriteLine("using System.Text;");
                stream.WriteLine("using System.Data;");

                #endregion

                stream.WriteLine(String.Format("namespace {0}{1}", String.IsNullOrEmpty(Solucao.Nome) ? String.Empty : String.Concat(Solucao.Nome, "."), projeto.Nome));
                stream.WriteLine("{");
                stream.WriteLine(String.Format("\tpublic class {0} : System.Attribute", classe.Nome));
                stream.WriteLine("\t{");

                stream.WriteLine("\t\t#region     .....:::::     ATRIBUTOS     :::::.....");
                stream.WriteLine("");
                stream.WriteLine("\t\tprivate String _columName;");
                stream.WriteLine("\t\tprivate String _enumName;");
                stream.WriteLine("\t\tprivate Boolean _isPrimaryKey;");
                stream.WriteLine("");
                stream.WriteLine("\t\t#endregion");

                stream.WriteLine("\t\t#region     .....:::::     PROPRIEDADES     :::::.....");
                stream.WriteLine("");
                stream.WriteLine("\t\tpublic String ColumName");
                stream.WriteLine("\t\t{");
                stream.WriteLine("\t\t\tget { return this._columName; }");
                stream.WriteLine("\t\t\tset { this._columName = value; }");
                stream.WriteLine("\t\t}");
                stream.WriteLine("");
                
                stream.WriteLine("\t\tpublic String EnumName");
                stream.WriteLine("\t\t{");
                stream.WriteLine("\t\t\tget { return this._enumName; }");
                stream.WriteLine("\t\t\tset { this._enumName = value; }");
                stream.WriteLine("\t\t}");
                stream.WriteLine("");

                stream.WriteLine("\t\tpublic Boolean IsPrimaryKey");
                stream.WriteLine("\t\t{");
                stream.WriteLine("\t\t\tget { return this._isPrimaryKey; }");
                stream.WriteLine("\t\t\tset { this._isPrimaryKey = value; }");
                stream.WriteLine("\t\t}");
                stream.WriteLine("");
                stream.WriteLine("\t\t#endregion");
                
                stream.WriteLine("\t}");
                stream.WriteLine("}");
                stream.Close();

                projeto.Classes.Add(classe);

                #region     .....:::::     CRIA CLASSE CONDITION     :::::.....

                classe = new ClasseVO() { Nome = "Condition" };

                pathClasse = String.Format(@"{0}\{1}.cs", pathUtilitarios, classe.Nome);
                stream = new StreamWriter(pathClasse, true);

                #region     .....:::::     ESCREVE AS REFERENCIAS UTILIZADAS     :::::.....

                stream.WriteLine("using System;");
                stream.WriteLine("using System.Collections.Generic;");
                stream.WriteLine("using System.Text;");
                stream.WriteLine("using System.Data;");

                #endregion

                stream.WriteLine(String.Format("namespace {0}{1}", String.IsNullOrEmpty(Solucao.Nome) ? String.Empty : String.Concat(Solucao.Nome, "."), projeto.Nome));
                stream.WriteLine("{");
                stream.WriteLine(String.Format("\tpublic class {0}", classe.Nome));
                stream.WriteLine("\t{");

                stream.WriteLine("\t\t#region     .....:::::     ATRIBUTOS     :::::.....");
                stream.WriteLine("");
                stream.WriteLine("\t\tprivate List<KeyValuePair<String, KeyValuePair<object, ETipoWhere>>> condicoes = new List<KeyValuePair<String,KeyValuePair<object, ETipoWhere>>>();");
                stream.WriteLine("");
                stream.WriteLine("\t\t#endregion");
                stream.WriteLine("");

                stream.WriteLine("\t\t#region     .....:::::     METODOS     :::::.....");
                stream.WriteLine("");
                stream.WriteLine("\t\tpublic void Where(KeyValuePair<String, object> where, ETipoWhere tipo)");
                stream.WriteLine("\t\t{");
                stream.WriteLine("\t\t\tif (condicoes.Count == 0 && tipo == ETipoWhere.Or)");
                stream.WriteLine("\t\t\t{");
                stream.WriteLine("\t\t\t\tthrow (new Exception(\"Não foi adicionada nenhuma condição anteriormente para se fazer \\\"OR\\\".\"));");
                stream.WriteLine("\t\t\t}");
                stream.WriteLine("");
                stream.WriteLine("\t\t\tKeyValuePair<String, KeyValuePair<object, ETipoWhere>> keyValue = new KeyValuePair<string,KeyValuePair<object,ETipoWhere>>(where.Key, new KeyValuePair<object,ETipoWhere>(where.Value, tipo));");
                stream.WriteLine("\t\t\tcondicoes.Add(keyValue);");
                stream.WriteLine("\t\t}");
                stream.WriteLine("");
                stream.WriteLine("\t\tpublic void Where(String property, object value, ETipoWhere tipo)");
                stream.WriteLine("\t\t{");
                stream.WriteLine("\t\t\tif (condicoes.Count == 0 && tipo == ETipoWhere.Or)");
                stream.WriteLine("\t\t\t{");
                stream.WriteLine("\t\t\t\tthrow (new Exception(\"Não foi adicionada nenhuma condição anteriormente para se fazer \\\"OR\\\".\"));");
                stream.WriteLine("\t\t\t}");
                stream.WriteLine("");
                stream.WriteLine("\t\t\tKeyValuePair<String, KeyValuePair<object, ETipoWhere>> keyValue = new KeyValuePair<string, KeyValuePair<object, ETipoWhere>>(property, new KeyValuePair<object, ETipoWhere>(value, tipo));");
                stream.WriteLine("\t\t\tcondicoes.Add(keyValue);");
                stream.WriteLine("\t\t}");
                stream.WriteLine("");
                stream.WriteLine("\t\tpublic void WhereAnd(KeyValuePair<String, object> where)");
                stream.WriteLine("\t\t{");
                stream.WriteLine("\t\t\tKeyValuePair<String, KeyValuePair<object, ETipoWhere>> keyValue = new KeyValuePair<string, KeyValuePair<object, ETipoWhere>>(where.Key, new KeyValuePair<object, ETipoWhere>(where.Value, ETipoWhere.And));");
                stream.WriteLine("\t\t\tcondicoes.Add(keyValue);");
                stream.WriteLine("\t\t}");
                stream.WriteLine("");
                stream.WriteLine("\t\tpublic void WhereAnd(String property, object value)");
                stream.WriteLine("\t\t{");
                stream.WriteLine("\t\t\tKeyValuePair<String, KeyValuePair<object, ETipoWhere>> keyValue = new KeyValuePair<string, KeyValuePair<object, ETipoWhere>>(property, new KeyValuePair<object, ETipoWhere>(value, ETipoWhere.And));");
                stream.WriteLine("\t\t\tcondicoes.Add(keyValue);");
                stream.WriteLine("\t\t}");
                stream.WriteLine("");
                stream.WriteLine("\t\tpublic void WhereOr(KeyValuePair<String, object> where)");
                stream.WriteLine("\t\t{");
                stream.WriteLine("\t\t\tif (condicoes.Count == 0)");
                stream.WriteLine("\t\t\t{");
                stream.WriteLine("\t\t\t\tthrow (new Exception(\"Não foi adicionada nenhuma condição anteriormente para se fazer \\\"OR\\\".\"));");
                stream.WriteLine("\t\t\t}");
                stream.WriteLine("");
                stream.WriteLine("\t\t\tKeyValuePair<String, KeyValuePair<object, ETipoWhere>> keyValue = new KeyValuePair<string, KeyValuePair<object, ETipoWhere>>(where.Key, new KeyValuePair<object, ETipoWhere>(where.Value, ETipoWhere.Or));");
                stream.WriteLine("\t\t\tcondicoes.Add(keyValue);");
                stream.WriteLine("\t\t}");
                stream.WriteLine("");
                stream.WriteLine("\t\tpublic void WhereOr(String property, object value)");
                stream.WriteLine("\t\t{");
                stream.WriteLine("\t\t\tif (condicoes.Count == 0)");
                stream.WriteLine("\t\t\t{");
                stream.WriteLine("\t\t\t\tthrow (new Exception(\"Não foi adicionada nenhuma condição anteriormente para se fazer \\\"OR\\\".\"));");
                stream.WriteLine("\t\t\t}");
                stream.WriteLine("");
                stream.WriteLine("\t\t\tKeyValuePair<String, KeyValuePair<object, ETipoWhere>> keyValue = new KeyValuePair<string, KeyValuePair<object, ETipoWhere>>(property, new KeyValuePair<object, ETipoWhere>(value, ETipoWhere.Or));");
                stream.WriteLine("\t\t\tcondicoes.Add(keyValue);");
                stream.WriteLine("\t\t}");
                stream.WriteLine("");
                stream.WriteLine("\t\tpublic List<KeyValuePair<String, KeyValuePair<object, ETipoWhere>>> GetCondicoes()");
                stream.WriteLine("\t\t{");
                stream.WriteLine("\t\t\treturn condicoes;");
                stream.WriteLine("\t\t}");
                stream.WriteLine("");
                stream.WriteLine("\t\tpublic static String ToTextTipoWhere(ETipoWhere tipo)");
                stream.WriteLine("\t\t{");
                stream.WriteLine("\t\t\tswitch (tipo)");
                stream.WriteLine("\t\t\t{");
                stream.WriteLine("\t\t\t\tcase ETipoWhere.And:");
                stream.WriteLine("\t\t\t\t\treturn \"and \";");
                stream.WriteLine("\t\t\t\t\tbreak;");
                stream.WriteLine("\t\t\t\tcase ETipoWhere.Or:");
                stream.WriteLine("\t\t\t\t\treturn \"or \";");
                stream.WriteLine("\t\t\t\t\tbreak;");
                stream.WriteLine("\t\t\t}");
                stream.WriteLine("\t\t\treturn String.Empty;");
                stream.WriteLine("\t\t}");
                stream.WriteLine("");
                stream.WriteLine("\t\t#endregion");

                stream.WriteLine("\t}");
                stream.WriteLine("}");
                stream.Close();

                projeto.Classes.Add(classe);

                #region     .....:::::     CRIA ENUM ETIPOWHERE     :::::.....

                classe = new ClasseVO() { Nome = "ETipoWhere" };

                pathClasse = String.Format(@"{0}\{1}.cs", pathUtilitarios, classe.Nome);
                stream = new StreamWriter(pathClasse, true);

                #region     .....:::::     ESCREVE AS REFERENCIAS UTILIZADAS     :::::.....

                stream.WriteLine("using System;");
                stream.WriteLine("using System.Collections.Generic;");
                stream.WriteLine("using System.Text;");
                stream.WriteLine("using System.Data;");

                #endregion

                stream.WriteLine(String.Format("namespace {0}{1}", String.IsNullOrEmpty(Solucao.Nome) ? String.Empty : String.Concat(Solucao.Nome, "."), projeto.Nome));
                stream.WriteLine("{");
                stream.WriteLine(String.Format("\tpublic enum {0}", classe.Nome));
                stream.WriteLine("\t{");
                stream.WriteLine("\t\tAnd = 1,");
                stream.WriteLine("\t\tOr = 2");
                stream.WriteLine("\t}");
                stream.WriteLine("}");
                stream.Close();

                projeto.Classes.Add(classe);

                #endregion

                #endregion
                
                #region     .....:::::     CRIA ARQUIVO ASSEMBLYINFO     :::::.....

                String pathProperties = String.Concat(pathUtilitarios, @"\Properties");
                Directory.CreateDirectory(pathProperties);
                stream = new StreamWriter(String.Concat(pathProperties, @"\AssemblyInfo.cs"), true);

                stream.WriteLine("using System.Reflection;");
                stream.WriteLine("using System.Runtime.CompilerServices;");
                stream.WriteLine("using System.Runtime.InteropServices;");
                stream.WriteLine("");
                stream.WriteLine("");
                stream.WriteLine("");
                stream.WriteLine(String.Format("[assembly: AssemblyTitle(\"{0}\")]", projeto.Nome));
                stream.WriteLine("[assembly: AssemblyDescription(\"\")]");
                stream.WriteLine("[assembly: AssemblyConfiguration(\"\")]");
                stream.WriteLine("[assembly: AssemblyCompany(\"P.H.F. Dias Software\")]");
                stream.WriteLine(String.Format("[assembly: AssemblyProduct(\"{0}\")]", projeto.Nome));
                stream.WriteLine("[assembly: AssemblyCopyright(\"Copyright © P.H.F. Dias Software 2012\")]");
                stream.WriteLine("[assembly: AssemblyTrademark(\"\")]");
                stream.WriteLine("[assembly: AssemblyCulture(\"\")]");
                stream.WriteLine("");
                stream.WriteLine("[assembly: ComVisible(false)]");
                stream.WriteLine("");
                stream.WriteLine(String.Format("[assembly: Guid(\"{0}\")]", projeto.Guid.ToString()));
                stream.WriteLine("");
                stream.WriteLine("");
                stream.WriteLine("[assembly: AssemblyVersion(\"1.0\")]");
                stream.WriteLine("[assembly: AssemblyFileVersion(\"1.0\")]");

                stream.Close();

                #endregion

                #endregion

                #region     .....:::::     CRIA ARQUIVO DE PROJETO     :::::.....

                String nomeProjeto = String.Format("{0}.csproj", projeto.Tipo.ToString());
                String pathProjeto = String.Format(@"{0}\{1}", pathUtilitarios, nomeProjeto);

                stream = new StreamWriter(pathProjeto, true);
                stream.WriteLine("<Project DefaultTargets=\"Build\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">");

                stream.WriteLine("\t<PropertyGroup>");
                stream.WriteLine("\t\t<SchemaVersion>2.0</SchemaVersion>");
                stream.WriteLine(String.Format("\t\t<ProjectGuid>{0}</ProjectGuid>", String.Concat("{", projeto.Guid.ToString(), "}")));
                stream.WriteLine("\t\t<OutputType>Library</OutputType>");
                stream.WriteLine("\t\t<AppDesignerFolder>Properties</AppDesignerFolder>");
                stream.WriteLine(String.Format("\t\t<RootNamespace>{0}{1}</RootNamespace>", String.IsNullOrEmpty(Solucao.Nome) ? String.Empty : String.Concat(Solucao.Nome, "."), projeto.Namespace));
                stream.WriteLine(String.Format("\t\t<AssemblyName>{0}{1}</AssemblyName>", String.IsNullOrEmpty(Solucao.Nome) ? String.Empty : String.Concat(Solucao.Nome, "."), projeto.Namespace));
                stream.WriteLine("\t\t<SccProjectName></SccProjectName>");
                stream.WriteLine("\t\t<SccLocalPath></SccLocalPath>");
                stream.WriteLine("\t\t<SccAuxPath></SccAuxPath>");
                stream.WriteLine("\t\t<SccProvider></SccProvider>");
                stream.WriteLine("\t</PropertyGroup>");

                stream.WriteLine("\t<PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' \">");
                stream.WriteLine("\t\t<DebugSymbols>true</DebugSymbols>");
                stream.WriteLine("\t\t<DebugType>full</DebugType>");
                stream.WriteLine("\t\t<Optimize>false</Optimize>");
                stream.WriteLine("\t\t<OutputPath>bin\\Debug\\</OutputPath>");
                stream.WriteLine("\t\t<DefineConstants>DEBUG;TRACE</DefineConstants>");
                stream.WriteLine("\t\t<ErrorReport>prompt</ErrorReport>");
                stream.WriteLine("\t\t<WarningLevel>4</WarningLevel>");
                stream.WriteLine("\t</PropertyGroup>");

                stream.WriteLine("\t<PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' \">");
                stream.WriteLine("\t\t<DebugType>pdbonly</DebugType>");
                stream.WriteLine("\t\t<Optimize>true</Optimize>");
                stream.WriteLine("\t\t<OutputPath>bin\\Release\\</OutputPath>");
                stream.WriteLine("\t\t<DefineConstants>TRACE</DefineConstants>");
                stream.WriteLine("\t\t<ErrorReport>prompt</ErrorReport>");
                stream.WriteLine("\t\t<WarningLevel>4</WarningLevel>");
                stream.WriteLine("\t</PropertyGroup>");

                if (projeto.Classes.Count > 0)
                {
                    projeto.Classes.Add(new ClasseVO() { Nome = "AssemblyInfo" });
                    stream.WriteLine("\t<ItemGroup>");

                    foreach (ClasseVO classe1 in projeto.Classes)
                        stream.WriteLine(String.Format("\t\t<Compile Include=\"{0}{1}.cs\" />", classe1.Nome == "AssemblyInfo" ? @"Properties\" : String.Empty, String.Concat(classe1.Nome, classe1.Nome == "AssemblyInfo" ? String.Empty : "")));

                    stream.WriteLine("\t</ItemGroup>");
                }

                stream.WriteLine("\t<ItemGroup>");
                stream.WriteLine("\t\t<Reference Include=\"System\" />");
                stream.WriteLine("\t\t<Reference Include=\"System.Data\" />");
                stream.WriteLine("\t\t<Reference Include=\"System.Xml\" />");
                stream.WriteLine("\t</ItemGroup>");

                stream.WriteLine("\t<Import Project=\"$(MSBuildBinPath)\\Microsoft.CSharp.targets\" />");
                stream.WriteLine("</Project>");

                stream.Close();
                #endregion

                #endregion

                Solucao.Projetos.Add(projeto);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (stream != null)
                    stream.Dispose();
            }
        }

        /// <summary>
        /// Gera as classes de negócio de acordo com as entidades
        /// </summary>
        private void GerarProjetoNegocios()
        {
            StreamWriter stream = null;
            try
            {
                ProjetoVO projeto = new ProjetoVO() { Nome = ETipoProjeto.Negocios.ToString(), Tipo = ETipoProjeto.Negocios, Namespace = "Negocios", Classes = Solucao.Projetos.Count > 0 ? Solucao.Projetos[0].Classes : new List<ClasseVO>() };
                String pathNegocios = String.Concat(txtPath.Text, String.Format(@"\{0}\Negocios", Solucao.Nome));
                Directory.CreateDirectory(@pathNegocios);
                
                ClasseVO classeAssemblyInfo = projeto.Classes.Find(delegate(ClasseVO classe) { return classe.Nome == "AssemblyInfo"; });
                if(classeAssemblyInfo != null)
                    projeto.Classes.Remove(classeAssemblyInfo);
                
                #region     .....:::::     CRIA AS CLASSES DE NEGÓCIO NO REPOSITÓRIO     :::::.....

                foreach (ClasseVO classe in projeto.Classes)
                {
                    String nomeClasse = String.Concat(classe.Nome, "BO.cs");
                    String pathClasse = String.Format(@"{0}\{1}", pathNegocios, nomeClasse);
                    stream = new StreamWriter(pathClasse, true);

                    #region     .....:::::     ESCREVE AS REFERENCIAS UTILIZADAS     :::::.....

                    stream.WriteLine("using System;");
                    stream.WriteLine("using System.Collections.Generic;");
                    stream.WriteLine("using System.Text;");
                    stream.WriteLine("using System.Data;");
                    stream.WriteLine(String.Format("using {0}.{1};", Solucao.Nome, ETipoProjeto.AcessoDados.ToString()));
                    stream.WriteLine(String.Format("using {0}.{1};", Solucao.Nome, ETipoProjeto.Entidades.ToString()));

                    #endregion

                    stream.WriteLine(String.Format("namespace {0}{1}", String.IsNullOrEmpty(Solucao.Nome) ? String.Empty : String.Concat(Solucao.Nome, "."), projeto.Nome));
                    stream.WriteLine("{");
                    stream.WriteLine(String.Format("\tpublic class {0}BO:BaseBO<{0}VO>", classe.Nome));
                    stream.WriteLine("\t{");

                    stream.WriteLine("");
                    stream.WriteLine("\t\t#region     .....:::::     CONSTRUTORES     :::::.....");
                    stream.WriteLine("");
                    stream.WriteLine(String.Format("\t\tpublic {0}BO() : base()", classe.Nome));
                    stream.WriteLine("\t\t{");
                    stream.WriteLine("\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t#endregion");
                    

                    #region     .....:::::     ESCREVE OS MÉTODOS     :::::.....
                    
                    stream.WriteLine("");
                    stream.WriteLine("\t\t#region     .....:::::     MÉTODOS     :::::.....");
                    stream.WriteLine("");

                    //Busca todos as informações do objeto
                    //stream.WriteLine(String.Format("\t\tpublic List<{0}VO> Buscar()", classe.Nome));
                    //stream.WriteLine("\t\t{");
                    //stream.WriteLine(String.Format("\t\t\tDataTable dt = new {0}DAO().Buscar();", classe.Nome));
                    //stream.WriteLine(String.Format("\t\t\tList<{0}VO> lst = new List<{0}VO>();", classe.Nome));
                    //stream.WriteLine("\t\t\tforeach(DataRow row in dt.Rows)");
                    //stream.WriteLine("\t\t\t{");
                    //stream.WriteLine(String.Format("\t\t\t\tlst.Add(new {0}VO(row));", classe.Nome));
                    //stream.WriteLine("\t\t\t}");
                    //stream.WriteLine("\t\t\treturn lst;");
                    //stream.WriteLine("\t\t}");
                    //stream.WriteLine("");

                    //Busca um objeto específico através de sua chave primária
                    //List<PropriedadeVO> lstPropriedadesChave = classe.Propriedades.FindAll(delegate(PropriedadeVO property) { return property.Coluna.IsChavePrimaria; });
                    //String chavesPrimaria = String.Empty;
                    //foreach (PropriedadeVO property in lstPropriedadesChave)
                    //{
                    //    chavesPrimaria += String.Format("{0} {1},", property.ToTextTipoVariavel(), property.Nome.ToAtributo());
                    //}
                    //chavesPrimaria = String.Join(", ", chavesPrimaria.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));

                    //stream.WriteLine(String.Format("\t\tpublic {0}VO BuscarPorId({1})", classe.Nome, chavesPrimaria));
                    //stream.WriteLine("\t\t{");
                    //stream.WriteLine(String.Format("\t\t\tDataTable dt = new {0}DAO().BuscarPorId({1});", classe.Nome, String.Join(", ", lstPropriedadesChave.Select(x=> x.Nome.ToAtributo()).ToArray())));
                    //stream.WriteLine(String.Format("\t\t\t{0}VO obj = null;", classe.Nome));
                    //stream.WriteLine("\t\t\tif(dt.Rows.Count > 0)");
                    //stream.WriteLine("\t\t\t{");
                    //stream.WriteLine("\t\t\t\tDataRow row = dt.Rows[0];");
                    //stream.WriteLine(String.Format("\t\t\t\tobj = new {0}VO(row);", classe.Nome));
                    //stream.WriteLine("\t\t\t}");
                    //stream.WriteLine("\t\t\treturn obj;");
                    //stream.WriteLine("\t\t}");
                    //stream.WriteLine("");
                    
                    //Insere informações do objeto
                    //stream.WriteLine(String.Format("\t\tpublic void Inserir({0}VO obj)", classe.Nome));
                    //stream.WriteLine("\t\t{");
                    //stream.WriteLine("\t\t\ttry");
                    //stream.WriteLine("\t\t\t{");
                    //stream.WriteLine(String.Format("\t\t\t\tnew {0}DAO().Inserir(obj);", classe.Nome));
                    //stream.WriteLine("\t\t\t}");
                    //stream.WriteLine("\t\t\tcatch(Exception ex)");
                    //stream.WriteLine("\t\t\t{");
                    //stream.WriteLine("\t\t\t\tthrow ex;");
                    //stream.WriteLine("\t\t\t}");
                    //stream.WriteLine("\t\t}");
                    //stream.WriteLine("");

                    //Atualizar informações do objeto
                    //stream.WriteLine(String.Format("\t\tpublic void Atualizar({0}VO obj)", classe.Nome));
                    //stream.WriteLine("\t\t{");
                    //stream.WriteLine("\t\t\ttry");
                    //stream.WriteLine("\t\t\t{");
                    //stream.WriteLine(String.Format("\t\t\t\tnew {0}DAO().Atualizar(obj);", classe.Nome));
                    //stream.WriteLine("\t\t\t}");
                    //stream.WriteLine("\t\t\tcatch(Exception ex)");
                    //stream.WriteLine("\t\t\t{");
                    //stream.WriteLine("\t\t\t\tthrow ex;");
                    //stream.WriteLine("\t\t\t}");
                    //stream.WriteLine("\t\t}");
                    //stream.WriteLine("");

                    //Exclui informações do objeto
                    //stream.WriteLine(String.Format("\t\tpublic void Excluir({0})", chavesPrimaria));
                    //stream.WriteLine("\t\t{");
                    //stream.WriteLine("\t\t\ttry");
                    //stream.WriteLine("\t\t\t{");
                    //stream.WriteLine(String.Format("\t\t\t\tnew {0}DAO().Excluir({1});", classe.Nome, String.Join(", ", lstPropriedadesChave.Select(x => x.Nome.ToAtributo()).ToArray())));
                    //stream.WriteLine("\t\t\t}");
                    //stream.WriteLine("\t\t\tcatch(Exception ex)");
                    //stream.WriteLine("\t\t\t{");
                    //stream.WriteLine("\t\t\t\tthrow ex;");
                    //stream.WriteLine("\t\t\t}");
                    //stream.WriteLine("\t\t}");
                    //stream.WriteLine("");

                    #endregion

                    stream.WriteLine("\t\t#endregion");
                    stream.WriteLine("\t}");
                    stream.WriteLine("}");

                    stream.Close();
                }

                if (projeto.Classes.Count > 0)
                {
                    #region     .....:::::     CRIA ARQUIVO ASSEMBLYINFO     :::::.....

                    String pathProperties = String.Concat(pathNegocios, @"\Properties");
                    Directory.CreateDirectory(pathProperties);
                    stream = new StreamWriter(String.Concat(pathProperties, @"\AssemblyInfo.cs"), true);

                    stream.WriteLine("using System.Reflection;");
                    stream.WriteLine("using System.Runtime.CompilerServices;");
                    stream.WriteLine("using System.Runtime.InteropServices;");
                    stream.WriteLine("");
                    stream.WriteLine("");
                    stream.WriteLine("");
                    stream.WriteLine(String.Format("[assembly: AssemblyTitle(\"{0}\")]", projeto.Nome));
                    stream.WriteLine("[assembly: AssemblyDescription(\"\")]");
                    stream.WriteLine("[assembly: AssemblyConfiguration(\"\")]");
                    stream.WriteLine("[assembly: AssemblyCompany(\"P.H.F. Dias Software\")]");
                    stream.WriteLine(String.Format("[assembly: AssemblyProduct(\"{0}\")]", projeto.Nome));
                    stream.WriteLine("[assembly: AssemblyCopyright(\"Copyright © P.H.F. Dias Software 2012\")]");
                    stream.WriteLine("[assembly: AssemblyTrademark(\"\")]");
                    stream.WriteLine("[assembly: AssemblyCulture(\"\")]");
                    stream.WriteLine("");
                    stream.WriteLine("[assembly: ComVisible(false)]");
                    stream.WriteLine("");
                    stream.WriteLine(String.Format("[assembly: Guid(\"{0}\")]", projeto.Guid.ToString()));
                    stream.WriteLine("");
                    stream.WriteLine("");
                    stream.WriteLine("[assembly: AssemblyVersion(\"1.0\")]");
                    stream.WriteLine("[assembly: AssemblyFileVersion(\"1.0\")]");

                    stream.Close();

                    #region     .....:::::     CRIA CLASSE BASE DE NEGÓCIDO     :::::.....

                    stream = new StreamWriter(String.Concat(pathNegocios, @"\BaseBO.cs"));

                    #region     .....:::::     ESCREVE REFERENCIAS UTILIZADAS     :::::.....

                    stream.WriteLine("using System;");
                    stream.WriteLine("using System.Collections.Generic;");
                    stream.WriteLine("using System.Text;");
                    stream.WriteLine("using System.Data;");
                    stream.WriteLine("using Db.Persistence.Utils;");
                    stream.WriteLine("using Db.Persistence.DataAccess;");
                    stream.WriteLine(String.Format("using {0}.{1};", Solucao.Nome, ETipoProjeto.AcessoDados.ToString()));

                    #endregion

                    stream.WriteLine("");
                    stream.WriteLine(String.Format("namespace {0}.{1}", Solucao.Nome, projeto.Tipo.ToString()));
                    stream.WriteLine("{");
                    stream.WriteLine("\tpublic class BaseBO<T> where T : new()");
                    stream.WriteLine("\t{");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t#region     .....:::::     CONSTRUTORES     :::::.....");
                    stream.WriteLine("");
                    stream.WriteLine("\t\tpublic BaseBO()");
                    stream.WriteLine("\t\t{");
                    stream.WriteLine("\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t#endregion");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t#region     .....:::::     MÉTODOS     :::::.....");
                    stream.WriteLine("");
                    stream.WriteLine("\t\tpublic void Inserir(T obj, params String[] props)");
                    stream.WriteLine("\t\t{");
                    stream.WriteLine(String.Format("\t\t\tnew {0}().Inserir<T>(obj, props);", DriverBanco.ToTextBaseDAO()));
                    stream.WriteLine("\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine("\t\tpublic void Atualizar(T obj, params String[] props)");
                    stream.WriteLine("\t\t{");
                    stream.WriteLine(String.Format("\t\t\tnew {0}().Atualizar<T>(obj, props);", DriverBanco.ToTextBaseDAO()));
                    stream.WriteLine("\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine("\t\tpublic void Excluir(T obj, params String[] props)");
                    stream.WriteLine("\t\t{");
                    stream.WriteLine(String.Format("\t\t\tnew {0}().Excluir<T>(obj, props);", DriverBanco.ToTextBaseDAO()));
                    stream.WriteLine("\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine("\t\tpublic List<T> Buscar(params String[] props)");
                    stream.WriteLine("\t\t{");
                    stream.WriteLine(String.Format("\t\t\t{0} baseDao = new {0}();", DriverBanco.ToTextBaseDAO()));
                    stream.WriteLine("\t\t\tbaseDao.AddProperties<T>(props);");
                    stream.WriteLine("\t\t\tList<T> lstRetorno = baseDao.Buscar<T>(props);");
                    stream.WriteLine("\t\t\treturn lstRetorno;");
                    stream.WriteLine("\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine("\t\tpublic List<T> BuscarOrdenado(String[] orders, ETipoOrder tipo, params String[] props)");
                    stream.WriteLine("\t\t{");
                    stream.WriteLine(String.Format("\t\t\t{0} baseDao = new {0}();", DriverBanco.ToTextBaseDAO()));
                    stream.WriteLine("\t\t\tbaseDao.AddProperties<T>(props);");
                    stream.WriteLine("\t\t\tbaseDao.AddOrderBy<T>(tipo, orders);");
                    stream.WriteLine("\t\t\tList<T> lstRetorno = baseDao.Buscar<T>(props);");
                    stream.WriteLine("\t\t\treturn lstRetorno;");
                    stream.WriteLine("\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine("\t\tpublic T BuscarPorId(T obj, params String[] props)");
                    stream.WriteLine("\t\t{");
                    stream.WriteLine(String.Format("\t\t\treturn new {0}().BuscarPorId<T>(obj, props);", DriverBanco.ToTextBaseDAO()));
                    stream.WriteLine("\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine("\t\tpublic Paginacao<T> BuscarComPaginacao(Int32 indexPagina, Int32 totalRegistros, params String[] props)");
                    stream.WriteLine("\t\t{");
                    stream.WriteLine(String.Format("\t\t\tPaginacao<T> paginacao = new {0}().BuscarComPaginacao<T>(indexPagina, totalRegistros, props);", DriverBanco.ToTextBaseDAO()));
                    stream.WriteLine("\t\t\treturn paginacao;");
                    stream.WriteLine("\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine("\t\tpublic Paginacao<T> BuscarComPaginacao(String[] orders, ETipoOrder tipo, Int32 indexPagina, Int32 totalRegistros, params String[] props)");
                    stream.WriteLine("\t\t{");
                    stream.WriteLine(String.Format("\t\t\t{0} baseDao = new {0}();", DriverBanco.ToTextBaseDAO()));
                    stream.WriteLine("\t\t\tbaseDao.AddProperties<T>(props);");
                    stream.WriteLine("\t\t\tbaseDao.AddOrderBy<T>(tipo, orders);");
                    stream.WriteLine("\t\t\tPaginacao<T> paginacao = baseDao.BuscarComPaginacao<T>(indexPagina, totalRegistros, props);");
                    stream.WriteLine("\t\t\treturn paginacao;");
                    stream.WriteLine("\t\t}");

                    stream.WriteLine("");
                    stream.WriteLine("\t\t#endregion");
                    stream.WriteLine("");
                    stream.WriteLine("\t}");
                    stream.WriteLine("}");
                    stream.Close();

                    #endregion
                    

                    #endregion

                    #region     .....:::::     CRIA ARQUIVO DE PROJETO     :::::.....

                    String nomeProjeto = String.Format("{0}.csproj", ETipoProjeto.Negocios.ToString());
                    String pathProjeto = String.Format(@"{0}\{1}", pathNegocios, nomeProjeto);

                    stream = new StreamWriter(pathProjeto, true);
                    stream.WriteLine("<Project DefaultTargets=\"Build\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">");

                    stream.WriteLine("\t<PropertyGroup>");
                    stream.WriteLine("\t\t<SchemaVersion>2.0</SchemaVersion>");
                    stream.WriteLine(String.Format("\t\t<ProjectGuid>{0}</ProjectGuid>", String.Concat("{", projeto.Guid.ToString(), "}")));
                    stream.WriteLine("\t\t<OutputType>Library</OutputType>");
                    stream.WriteLine("\t\t<AppDesignerFolder>Properties</AppDesignerFolder>");
                    stream.WriteLine(String.Format("\t\t<RootNamespace>{0}{1}</RootNamespace>", String.IsNullOrEmpty(Solucao.Nome) ? String.Empty : String.Concat(Solucao.Nome, "."), projeto.Namespace));
                    stream.WriteLine(String.Format("\t\t<AssemblyName>{0}{1}</AssemblyName>", String.IsNullOrEmpty(Solucao.Nome) ? String.Empty : String.Concat(Solucao.Nome, "."), projeto.Namespace));
                    stream.WriteLine("\t\t<SccProjectName></SccProjectName>");
                    stream.WriteLine("\t\t<SccLocalPath></SccLocalPath>");
                    stream.WriteLine("\t\t<SccAuxPath></SccAuxPath>");
                    stream.WriteLine("\t\t<SccProvider></SccProvider>");
                    stream.WriteLine("\t</PropertyGroup>");

                    stream.WriteLine("\t<PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' \">");
                    stream.WriteLine("\t\t<DebugSymbols>true</DebugSymbols>");
                    stream.WriteLine("\t\t<DebugType>full</DebugType>");
                    stream.WriteLine("\t\t<Optimize>false</Optimize>");
                    stream.WriteLine("\t\t<OutputPath>bin\\Debug\\</OutputPath>");
                    stream.WriteLine("\t\t<DefineConstants>DEBUG;TRACE</DefineConstants>");
                    stream.WriteLine("\t\t<ErrorReport>prompt</ErrorReport>");
                    stream.WriteLine("\t\t<WarningLevel>4</WarningLevel>");
                    stream.WriteLine("\t</PropertyGroup>");

                    stream.WriteLine("\t<PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' \">");
                    stream.WriteLine("\t\t<DebugType>pdbonly</DebugType>");
                    stream.WriteLine("\t\t<Optimize>true</Optimize>");
                    stream.WriteLine("\t\t<OutputPath>bin\\Release\\</OutputPath>");
                    stream.WriteLine("\t\t<DefineConstants>TRACE</DefineConstants>");
                    stream.WriteLine("\t\t<ErrorReport>prompt</ErrorReport>");
                    stream.WriteLine("\t\t<WarningLevel>4</WarningLevel>");
                    stream.WriteLine("\t</PropertyGroup>");

                    if (projeto.Classes.Count > 0)
                    {
                        projeto.Classes.Add(new ClasseVO() { Nome = "AssemblyInfo" });
                        projeto.Classes.Add(new ClasseVO() { Nome = "Base" });
                        stream.WriteLine("\t<ItemGroup>");

                        foreach (ClasseVO classe in projeto.Classes)
                            stream.WriteLine(String.Format("\t\t<Compile Include=\"{0}{1}.cs\" />", classe.Nome == "AssemblyInfo" ? @"Properties\" : String.Empty, String.Concat(classe.Nome, classe.Nome == "AssemblyInfo" ? String.Empty : "BO")));

                        stream.WriteLine("\t</ItemGroup>");
                    }

                    stream.WriteLine("\t<ItemGroup>");
                    //stream.WriteLine(String.Format("\t\t<ProjectReference Include=\"..\\{0}\\{0}.csproj\">", Solucao.Projetos[0].Nome));
                    //stream.WriteLine(String.Concat("\t\t\t<Project>{", Solucao.Projetos[0].Guid.ToString(), "}</Project>"));
                    //stream.WriteLine(String.Format("\t\t\t<Name>{0}</Name>", Solucao.Projetos[0].Nome));
                    //stream.WriteLine("\t\t</ProjectReference>");
                    stream.WriteLine(String.Format("\t\t<ProjectReference Include=\"..\\{0}\\{0}.csproj\">", Solucao.Projetos[0].Nome));
                    stream.WriteLine(String.Concat("\t\t\t<Project>{", Solucao.Projetos[0].Guid.ToString(), "}</Project>"));
                    stream.WriteLine(String.Format("\t\t\t<Name>{0}</Name>", Solucao.Projetos[0].Nome));
                    stream.WriteLine("\t\t</ProjectReference>");
                    stream.WriteLine(String.Format("\t\t<ProjectReference Include=\"..\\{0}\\{0}.csproj\">", Solucao.Projetos[1].Nome));
                    stream.WriteLine(String.Concat("\t\t\t<Project>{", Solucao.Projetos[1].Guid.ToString(), "}</Project>"));
                    stream.WriteLine(String.Format("\t\t\t<Name>{0}</Name>", Solucao.Projetos[1].Nome));
                    stream.WriteLine("\t\t</ProjectReference>");
                    stream.WriteLine("\t</ItemGroup>");

                    stream.WriteLine("\t<ItemGroup>");

                    stream.WriteLine("\t\t<Reference Include=\"Db.Persistence.Utils, Version=1.0.0.0, Culture=neutral, processorArchitecture=MSIL\">");
                    stream.WriteLine("\t\t\t<SpecificVersion>False</SpecificVersion>");
                    stream.WriteLine("\t\t\t<HintPath>..\\AcessoDados\\dlls\\Db.Persistence.Utils.dll</HintPath>");
                    stream.WriteLine("\t\t</Reference>");
                    stream.WriteLine("\t\t<Reference Include=\"Db.Persistence.DataAccess, Version=1.0.0.0, Culture=neutral, processorArchitecture=MSIL\">");
                    stream.WriteLine("\t\t\t<SpecificVersion>False</SpecificVersion>");
                    stream.WriteLine("\t\t\t<HintPath>..\\AcessoDados\\dlls\\Db.Persistence.DataAccess.dll</HintPath>");
                    stream.WriteLine("\t\t</Reference>");
                    stream.WriteLine("\t\t<Reference Include=\"System\" />");
                    stream.WriteLine("\t\t<Reference Include=\"System.Data\" />");
                    stream.WriteLine("\t\t<Reference Include=\"System.Xml\" />");
                    stream.WriteLine("\t</ItemGroup>");

                    stream.WriteLine("\t<Import Project=\"$(MSBuildBinPath)\\Microsoft.CSharp.targets\" />");
                    stream.WriteLine("</Project>");

                    stream.Close();
                    #endregion
                }

                #endregion

                Solucao.Projetos.Add(projeto);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (stream != null)
                    stream.Dispose();
            }
        }

        /// <summary>
        /// Gera as classes de acesso a dados de acordo com as entidades
        /// </summary>
        private void GerarProjetoAcessoDados()
        {
            StreamWriter stream = null;
            try
            {
                ProjetoVO projeto = new ProjetoVO() { Nome = ETipoProjeto.AcessoDados.ToString(), Tipo = ETipoProjeto.AcessoDados, Namespace = "AcessoDados", Classes = Solucao.Projetos.Count > 0 ? Solucao.Projetos[0].Classes : new List<ClasseVO>() };
                String pathAcessoDados = String.Concat(txtPath.Text, String.Format(@"\{0}\AcessoDados", Solucao.Nome));
                String pathProcedures = String.Concat(pathAcessoDados, "\\Procedures");
                String prefixoParametro = DriverBanco.GetPrefixoParametro();
                Directory.CreateDirectory(@pathAcessoDados);
                Directory.CreateDirectory(pathProcedures);

                ClasseVO classeAssemblyInfo = projeto.Classes.Find(delegate(ClasseVO classe) { return classe.Nome == "AssemblyInfo"; });
                if (classeAssemblyInfo != null)
                    projeto.Classes.Remove(classeAssemblyInfo);

                CriaDllsProjeto();
                
                #region     .....:::::     CRIA AS CLASSES DE ACESSO A DADOS NO REPOSITÓRIO     :::::.....

                foreach (ClasseVO classe in projeto.Classes)
                {
                    String nomeClasse = String.Concat(classe.Nome, "DAO.cs");
                    String pathClasse = String.Format(@"{0}\{1}", pathAcessoDados, nomeClasse);
                    stream = new StreamWriter(pathClasse, true);

                    #region     .....:::::     ESCREVE AS REFERENCIAS UTILIZADAS     :::::.....

                    stream.WriteLine("using System;");
                    stream.WriteLine("using System.Collections.Generic;");
                    stream.WriteLine("using System.Text;");
                    stream.WriteLine("using System.Data;");
                    stream.WriteLine("using System.Data.Common;");
                    stream.WriteLine("using Db.Persistence.DataAccess;");
                    EscreveReferenciaBanco(stream);
                    stream.WriteLine(String.Format("using {0}.{1};", Solucao.Nome, ETipoProjeto.Entidades.ToString()));
                    
                    #endregion

                    stream.WriteLine(String.Format("namespace {0}{1}", String.IsNullOrEmpty(Solucao.Nome) ? String.Empty : String.Concat(Solucao.Nome, "."), projeto.Nome));
                    stream.WriteLine("{");
                    stream.WriteLine(String.Format("\tpublic class {0}DAO : {1}", classe.Nome, DriverBanco.ToTextBaseDAO()));
                    stream.WriteLine("\t{");

                    #region     .....:::::     ESCREVE CONSTANTES     :::::.....

                    //stream.WriteLine("");
                    //stream.WriteLine("\t\t#region     .....:::::     CONSTANTES     :::::.....");
                    //stream.WriteLine("");
                    //stream.WriteLine(String.Format("\t\tpublic const String {0} = \"{0}\";", String.Format("{0}BUSCAR_{1}", FrmGeradorProjetos.prefixoProcedure, classe.Nome.ToUpper()).GetNomeProcedure(DriverBanco)));
                    //stream.WriteLine(String.Format("\t\tpublic const String {0} = \"{0}\";", String.Format("{0}BUSCAR_POR_ID_{1}", FrmGeradorProjetos.prefixoProcedure, classe.Nome.ToUpper()).GetNomeProcedure(DriverBanco)));
                    //stream.WriteLine(String.Format("\t\tpublic const String {0} = \"{0}\";", String.Format("{0}INSERIR_{1}", FrmGeradorProjetos.prefixoProcedure, classe.Nome.ToUpper()).GetNomeProcedure(DriverBanco)));
                    //stream.WriteLine(String.Format("\t\tpublic const String {0} = \"{0}\";", String.Format("{0}ATUALIZAR_{1}", FrmGeradorProjetos.prefixoProcedure, classe.Nome.ToUpper()).GetNomeProcedure(DriverBanco)));
                    //stream.WriteLine(String.Format("\t\tpublic const String {0} = \"{0}\";", String.Format("{0}EXCLUIR_{1}", FrmGeradorProjetos.prefixoProcedure, classe.Nome.ToUpper()).GetNomeProcedure(DriverBanco)));

                    //stream.WriteLine("");
                    //stream.WriteLine("\t\t#endregion");

                    #endregion
                    
                    #region     .....:::::     ESCREVE OS MÉTODOS     :::::.....

                    stream.WriteLine("");
                    stream.WriteLine("\t\t#region     .....:::::     MÉTODOS     :::::.....");
                    stream.WriteLine("");

                    //#region     .....:::::     ESCREVE MÉTODO QUE BUSCA TODAS INFORMAÇÕES     :::::.....

                    ////Busca todos as informações do objeto
                    //stream.WriteLine("\t\tpublic DataTable Buscar()");
                    //stream.WriteLine("\t\t{");
                    //stream.WriteLine(String.Format("\t\t\t{0} conexao = null;", DriverBanco.ToTextConnection()));
                    //stream.WriteLine("\t\t\tDataTable tabela = new DataTable();");
                    //stream.WriteLine("\t\t\ttry");
                    //stream.WriteLine("\t\t\t{");
                    //EscreveDataAdapter(stream);
                    //stream.WriteLine("\t\t\t\tconexao = base.GetConnection();");
                    //stream.WriteLine(String.Format("\t\t\t\t{0} command = conexao.CreateCommand();", DriverBanco.ToTextCommand()));
                    //stream.WriteLine(String.Format("\t\t\t\tcommand.CommandText = {0}DAO.{1};", classe.Nome, String.Format("{0}BUSCAR_{1}", FrmGeradorProjetos.prefixoProcedure, classe.Nome.ToUpper()).GetNomeProcedure(DriverBanco)));
                    //stream.WriteLine("\t\t\t\tcommand.CommandType = CommandType.StoredProcedure;");
                    
                    //EscreveOutPutParameter(stream);

                    //stream.WriteLine("\t\t\t\tconexao.Open();");

                    //EscreveExecuteReader(stream);
                    //stream.WriteLine("\t\t\t}");
                    //stream.WriteLine("\t\t\tcatch(Exception ex)");
                    //stream.WriteLine("\t\t\t{");
                    //stream.WriteLine("\t\t\t\tthrow ex;");
                    //stream.WriteLine("\t\t\t}");
                    //stream.WriteLine("\t\t\tfinally");
                    //stream.WriteLine("\t\t\t{");
                    //stream.WriteLine("\t\t\t\tif(conexao != null)");
                    //stream.WriteLine("\t\t\t\t{");
                    //stream.WriteLine("\t\t\t\t\tconexao.Close();");
                    //stream.WriteLine("\t\t\t\t}");
                    //stream.WriteLine("\t\t\t}");
                    //stream.WriteLine("\t\t\treturn tabela;");
                    //stream.WriteLine("\t\t}");
                    //stream.WriteLine("");

                    //#endregion

                    //#region     .....:::::     ESCREVE MÉTODO QUE BUSCA UM ÚNICO ITEM     :::::.....

                    ////Busca um objeto específico através de sua chave primária
                    //List<PropriedadeVO> lstPropriedadesChave = classe.Propriedades.FindAll(delegate(PropriedadeVO property) { return property.Coluna.IsChavePrimaria; });
                    //String chavesPrimaria = String.Empty;
                    //foreach (PropriedadeVO property in lstPropriedadesChave)
                    //{
                    //    chavesPrimaria += String.Format("{0} {1},", property.ToTextTipoVariavel(), property.Nome.ToAtributo());
                    //}
                    //chavesPrimaria = String.Join(", ", chavesPrimaria.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));

                    //stream.WriteLine(String.Format("\t\tpublic DataTable BuscarPorId({0})", chavesPrimaria));
                    //stream.WriteLine("\t\t{");
                    //stream.WriteLine(String.Format("\t\t\t{0} conexao = null;", DriverBanco.ToTextConnection()));
                    //stream.WriteLine("\t\t\tDataTable tabela = new DataTable();");
                    //stream.WriteLine("\t\t\ttry");
                    //stream.WriteLine("\t\t\t{");
                    //EscreveDataAdapter(stream);
                    //stream.WriteLine("\t\t\t\tconexao = base.GetConnection();");
                    //stream.WriteLine(String.Format("\t\t\t\t{0} command = conexao.CreateCommand();", DriverBanco.ToTextCommand()));
                    //stream.WriteLine(String.Format("\t\t\t\tcommand.CommandText = {0}DAO.{1};", classe.Nome, String.Format("{0}BUSCAR_POR_ID_{1}", FrmGeradorProjetos.prefixoProcedure, classe.Nome.ToUpper()).GetNomeProcedure(DriverBanco)));
                    //stream.WriteLine("\t\t\t\tcommand.CommandType = CommandType.StoredProcedure;");
                    
                    //if(lstPropriedadesChave.Count > 0)
                    //{
                    //    stream.WriteLine("");
                    //    stream.WriteLine("\t\t\t\t#region     .....:::::     ADICIONA PARAMETROS     :::::.....");
                    //    stream.WriteLine("");
                    //    stream.WriteLine(String.Format("\t\t\t\t{0} parameter = null;", DriverBanco.ToTextParameter()));
                    //}

                    //foreach (PropriedadeVO property in lstPropriedadesChave)
                    //{
                    //    stream.WriteLine("\t\t\t\tparameter = command.CreateParameter();");
                    //    stream.WriteLine("\t\t\t\tparameter.Direction = ParameterDirection.Input;");
                    //    stream.WriteLine(String.Format("\t\t\t\tparameter.ParameterName = \"{0}\";", property.GetNomeParametro(classe, prefixoParametro, DriverBanco)));
                    //    stream.WriteLine(String.Format("\t\t\t\tparameter.Size = {0};", property.Coluna.Tamanho.GetValueOrDefault()));
                    //    stream.WriteLine(String.Format("\t\t\t\tparameter.{0} = {0}.{1};", DriverBanco.ToTextDbType(), property.ToTextTypeDriver(DriverBanco)));
                    //    stream.WriteLine(String.Format("\t\t\t\tparameter.Value = {0};", property.Nome.ToAtributo()));
                    //    stream.WriteLine("\t\t\t\tcommand.Parameters.Add(parameter);");
                    //    stream.WriteLine("");
                    //}

                    //EscreveOutPutParameter(stream);

                    //if(lstPropriedadesChave.Count > 0)
                    //{
                    //    stream.WriteLine("\t\t\t\t#endregion");
                    //    stream.WriteLine("");
                    //}
                    
                    //stream.WriteLine("\t\t\t\tconexao.Open();");

                    //EscreveExecuteReader(stream);

                    //stream.WriteLine("\t\t\t}");
                    //stream.WriteLine("\t\t\tcatch(Exception ex)");
                    //stream.WriteLine("\t\t\t{");
                    //stream.WriteLine("\t\t\t\tthrow ex;");
                    //stream.WriteLine("\t\t\t}");
                    //stream.WriteLine("\t\t\tfinally");
                    //stream.WriteLine("\t\t\t{");
                    //stream.WriteLine("\t\t\t\tif(conexao != null)");
                    //stream.WriteLine("\t\t\t\t{");
                    //stream.WriteLine("\t\t\t\t\tconexao.Close();");
                    //stream.WriteLine("\t\t\t\t}");
                    //stream.WriteLine("\t\t\t}");
                    //stream.WriteLine("\t\t\treturn tabela;");

                    //stream.WriteLine("\t\t}");
                    //stream.WriteLine("");

                    //#endregion

                    //#region     .....:::::     ESCREVE MÉTODO QUE INSERE UM ITEM     :::::.....

                    ////Insere informações do objeto
                    //stream.WriteLine(String.Format("\t\tpublic void Inserir({0}VO obj)", classe.Nome));
                    //stream.WriteLine("\t\t{");

                    //stream.WriteLine(String.Format("\t\t\t{0} conexao = null;", DriverBanco.ToTextConnection()));
                    //stream.WriteLine("\t\t\ttry");
                    //stream.WriteLine("\t\t\t{");
                    //stream.WriteLine("\t\t\t\tconexao = base.GetConnection();");
                    //stream.WriteLine(String.Format("\t\t\t\t{0} command = conexao.CreateCommand();", DriverBanco.ToTextCommand()));
                    //stream.WriteLine(String.Format("\t\t\t\tcommand.CommandText = {0}DAO.{1};", classe.Nome, String.Format("{0}INSERIR_{1}", FrmGeradorProjetos.prefixoProcedure, classe.Nome.ToUpper()).GetNomeProcedure(DriverBanco)));
                    //stream.WriteLine("\t\t\t\tcommand.CommandType = CommandType.StoredProcedure;");
                    //stream.WriteLine("");
                    //stream.WriteLine("\t\t\t\t#region     .....:::::     ADICIONA PARAMETROS     :::::.....");
                    //stream.WriteLine("");

                    //if (classe.Propriedades.Count > 0)
                    //{
                    //    stream.WriteLine(String.Format("\t\t\t\t{0} parameter = null;", DriverBanco.ToTextParameter()));
                    //}
                    
                    //foreach (PropriedadeVO property in classe.Propriedades)
                    //{
                    //    stream.WriteLine("\t\t\t\tparameter = command.CreateParameter();");
                    //    stream.WriteLine("\t\t\t\tparameter.Direction = ParameterDirection.Input;");
                    //    stream.WriteLine(String.Format("\t\t\t\tparameter.ParameterName = \"{0}\";", property.GetNomeParametro(classe, prefixoParametro, DriverBanco)));
                    //    stream.WriteLine(String.Format("\t\t\t\tparameter.Size = {0};", property.Coluna.Tamanho.GetValueOrDefault()));
                    //    stream.WriteLine(String.Format("\t\t\t\tparameter.{0} = {0}.{1};", DriverBanco.ToTextDbType(), property.ToTextTypeDriver(DriverBanco)));
                    //    stream.WriteLine(String.Format("\t\t\t\tparameter.Value = obj.{0};", property.Nome));
                    //    stream.WriteLine("\t\t\t\tcommand.Parameters.Add(parameter);");
                    //    stream.WriteLine("");
                    //}

                    
                    //stream.WriteLine("\t\t\t\t#endregion");
                    //stream.WriteLine("");
                    //stream.WriteLine("\t\t\t\tconexao.Open();");
                    //stream.WriteLine("\t\t\t\tcommand.ExecuteNonQuery();");
                    //stream.WriteLine("\t\t\t}");
                    //stream.WriteLine("\t\t\tcatch(Exception ex)");
                    //stream.WriteLine("\t\t\t{");
                    //stream.WriteLine("\t\t\t\tthrow ex;");
                    //stream.WriteLine("\t\t\t}");
                    //stream.WriteLine("\t\t\tfinally");
                    //stream.WriteLine("\t\t\t{");
                    //stream.WriteLine("\t\t\t\tif(conexao != null)");
                    //stream.WriteLine("\t\t\t\t{");
                    //stream.WriteLine("\t\t\t\t\tconexao.Close();");
                    //stream.WriteLine("\t\t\t\t}");
                    //stream.WriteLine("\t\t\t}");
                    
                    //stream.WriteLine("\t\t}");
                    //stream.WriteLine("");

                    //#endregion

                    //#region    .....:::::     ESCREVE MÉTODO QUE ATUALIZA INFORMAÇÕES     :::::.....

                    ////Atualizar informações do objeto
                    //stream.WriteLine(String.Format("\t\tpublic void Atualizar({0}VO obj)", classe.Nome));
                    //stream.WriteLine("\t\t{");
                    //stream.WriteLine(String.Format("\t\t\t{0} conexao = null;", DriverBanco.ToTextConnection()));
                    //stream.WriteLine("\t\t\ttry");
                    //stream.WriteLine("\t\t\t{");
                    //stream.WriteLine("\t\t\t\tconexao = base.GetConnection();");
                    //stream.WriteLine(String.Format("\t\t\t\t{0} command = conexao.CreateCommand();", DriverBanco.ToTextCommand()));
                    //stream.WriteLine(String.Format("\t\t\t\tcommand.CommandText = {0}DAO.{1};", classe.Nome, String.Format("{0}ATUALIZAR_{1}", FrmGeradorProjetos.prefixoProcedure, classe.Nome.ToUpper()).GetNomeProcedure(DriverBanco)));
                    //stream.WriteLine("\t\t\t\tcommand.CommandType = CommandType.StoredProcedure;");
                    //stream.WriteLine("");
                    //stream.WriteLine("\t\t\t\t#region     .....:::::     ADICIONA PARAMETROS     :::::.....");
                    //stream.WriteLine("");

                    //if (classe.Propriedades.Count > 0)
                    //{
                    //    stream.WriteLine(String.Format("\t\t\t\t{0} parameter = null;", DriverBanco.ToTextParameter()));
                    //}

                    //foreach (PropriedadeVO property in classe.Propriedades)
                    //{
                    //    stream.WriteLine("\t\t\t\tparameter = command.CreateParameter();");
                    //    stream.WriteLine("\t\t\t\tparameter.Direction = ParameterDirection.Input;");
                    //    stream.WriteLine(String.Format("\t\t\t\tparameter.ParameterName = \"{0}\";", property.GetNomeParametro(classe, prefixoParametro, DriverBanco)));
                    //    stream.WriteLine(String.Format("\t\t\t\tparameter.Size = {0};", property.Coluna.Tamanho.GetValueOrDefault()));
                    //    stream.WriteLine(String.Format("\t\t\t\tparameter.{0} = {0}.{1};", DriverBanco.ToTextDbType(), property.ToTextTypeDriver(DriverBanco)));
                    //    stream.WriteLine(String.Format("\t\t\t\tparameter.Value = obj.{0};", property.Nome));
                    //    stream.WriteLine("\t\t\t\tcommand.Parameters.Add(parameter);");
                    //    stream.WriteLine("");
                    //}

                    //stream.WriteLine("\t\t\t\t#endregion");
                    //stream.WriteLine("");
                    //stream.WriteLine("\t\t\t\tconexao.Open();");
                    //stream.WriteLine("\t\t\t\tcommand.ExecuteNonQuery();");
                    //stream.WriteLine("\t\t\t}");
                    //stream.WriteLine("\t\t\tcatch(Exception ex)");
                    //stream.WriteLine("\t\t\t{");
                    //stream.WriteLine("\t\t\t\tthrow ex;");
                    //stream.WriteLine("\t\t\t}");
                    //stream.WriteLine("\t\t\tfinally");
                    //stream.WriteLine("\t\t\t{");
                    //stream.WriteLine("\t\t\t\tif(conexao != null)");
                    //stream.WriteLine("\t\t\t\t{");
                    //stream.WriteLine("\t\t\t\t\tconexao.Close();");
                    //stream.WriteLine("\t\t\t\t}");
                    //stream.WriteLine("\t\t\t}");
                    //stream.WriteLine("\t\t}");
                    //stream.WriteLine("");

                    //#endregion

                    //#region     .....:::::     ESCREVE MÉTODO QUE EXCLUI INFORMAÇÕES     :::::.....

                    ////Exclui informações do objeto
                    //stream.WriteLine(String.Format("\t\tpublic void Excluir({0})", chavesPrimaria));
                    //stream.WriteLine("\t\t{");

                    //stream.WriteLine(String.Format("\t\t\t{0} conexao = null;", DriverBanco.ToTextConnection()));
                    //stream.WriteLine("\t\t\ttry");
                    //stream.WriteLine("\t\t\t{");
                    //stream.WriteLine("\t\t\t\tconexao = base.GetConnection();");
                    //stream.WriteLine(String.Format("\t\t\t\t{0} command = conexao.CreateCommand();", DriverBanco.ToTextCommand()));
                    //stream.WriteLine(String.Format("\t\t\t\tcommand.CommandText = {0}DAO.{1};", classe.Nome, String.Format("{0}EXCLUIR_{1}", FrmGeradorProjetos.prefixoProcedure, classe.Nome.ToUpper()).GetNomeProcedure(DriverBanco)));
                    //stream.WriteLine("\t\t\t\tcommand.CommandType = CommandType.StoredProcedure;");
                    //stream.WriteLine("");
                    //stream.WriteLine("\t\t\t\t#region     .....:::::     ADICIONA PARAMETROS     :::::.....");
                    //stream.WriteLine("");

                    //if (lstPropriedadesChave.Count > 0)
                    //{
                    //    stream.WriteLine(String.Format("\t\t\t\t{0} parameter = null;", DriverBanco.ToTextParameter()));
                    //}

                    //foreach (PropriedadeVO property in lstPropriedadesChave)
                    //{
                    //    stream.WriteLine("\t\t\t\tparameter = command.CreateParameter();");
                    //    stream.WriteLine("\t\t\t\tparameter.Direction = ParameterDirection.Input;");
                    //    stream.WriteLine(String.Format("\t\t\t\tparameter.ParameterName = \"{0}\";", property.GetNomeParametro(classe, prefixoParametro, DriverBanco)));
                    //    stream.WriteLine(String.Format("\t\t\t\tparameter.Size = {0};", property.Coluna.Tamanho.GetValueOrDefault()));
                    //    stream.WriteLine(String.Format("\t\t\t\tparameter.{0} = {0}.{1};", DriverBanco.ToTextDbType(), property.ToTextTypeDriver(DriverBanco)));
                    //    stream.WriteLine(String.Format("\t\t\t\tparameter.Value = {0};", property.Nome.ToAtributo()));
                    //    stream.WriteLine("\t\t\t\tcommand.Parameters.Add(parameter);");
                    //    stream.WriteLine("");
                    //}

                    //stream.WriteLine("\t\t\t\t#endregion");
                    //stream.WriteLine("");
                    //stream.WriteLine("\t\t\t\tconexao.Open();");
                    //stream.WriteLine("\t\t\t\tcommand.ExecuteNonQuery();");
                    //stream.WriteLine("\t\t\t}");
                    //stream.WriteLine("\t\t\tcatch(Exception ex)");
                    //stream.WriteLine("\t\t\t{");
                    //stream.WriteLine("\t\t\t\tthrow ex;");
                    //stream.WriteLine("\t\t\t}");
                    //stream.WriteLine("\t\t\tfinally");
                    //stream.WriteLine("\t\t\t{");
                    //stream.WriteLine("\t\t\t\tif(conexao != null)");
                    //stream.WriteLine("\t\t\t\t{");
                    //stream.WriteLine("\t\t\t\t\tconexao.Close();");
                    //stream.WriteLine("\t\t\t\t}");
                    //stream.WriteLine("\t\t\t}");

                    //stream.WriteLine("\t\t}");
                    //stream.WriteLine("");

                    //#endregion

                    stream.WriteLine("\t\t#endregion");
                    
                    #endregion

                    stream.WriteLine("\t}");
                    stream.WriteLine("}");

                    stream.Close();
                }

                //#region     .....:::::     CRIA AS PROCEDURES     :::::.....

                //Boolean possuiParamOutPut = VerificaPossuiParametroOutPut();
                //foreach (ClasseVO classe in projeto.Classes)
                //{
                //    List<PropriedadeVO> lstPropriedadesChave = classe.Propriedades.FindAll(delegate(PropriedadeVO property) { return property.Coluna.IsChavePrimaria; });
                    
                //    #region     .....:::::     CRIA PROCEDURE QUE BUSCA TODAS INFORMAÇÕES     :::::.....

                //    String procBuscar = String.Format("{0}BUSCAR_{1}", FrmGeradorProjetos.prefixoProcedure, classe.Nome.ToUpper()).GetNomeProcedure(DriverBanco);

                //    stream = new StreamWriter(String.Format(String.Concat(pathProcedures, "\\{0}.sql"), procBuscar));
                //    EscreveCreateProcedure(stream);
                //    stream.Write(String.Format("PROCEDURE {0}", procBuscar));
                //    stream.WriteLine("");

                //    if (possuiParamOutPut)
                //    {
                //        stream.WriteLine("(");
                //        EscreveSaidaTabelaProcedure(stream);
                //        stream.WriteLine(")");
                //    }
                //    EscreveIniciarProcedure(stream);
                //    stream.WriteLine("");
                //    stream.WriteLine("BEGIN");
                //    EscreveInicioSelectProcedure(stream);
                //    stream.WriteLine("\t\tselect");

                //    foreach (PropriedadeVO prop in classe.Propriedades)
                //    {
                //        stream.WriteLine(String.Format("\t\t\t{0} as {1}{2}{3}", prop.Coluna.Nome, classe.Nome, prop.Nome, prop.Nome != classe.Propriedades.Last().Nome ? "," : String.Empty));
                //    }

                //    stream.WriteLine("\t\tfrom");
                //    stream.WriteLine(String.Format("\t\t\t{0};", classe.Tabela.Nome));
                //    EscreveTerminoProcedure(stream, procBuscar);

                //    stream.Close();
                    
                //    #endregion

                //    #region     .....:::::     CRIA PROCEDURE QUE BUSCA TODAS INFORMAÇÕES POR ID     :::::.....

                //    String procBuscarPorId = String.Format("{0}BUSCAR_POR_ID_{1}", FrmGeradorProjetos.prefixoProcedure, classe.Nome.ToUpper()).GetNomeProcedure(DriverBanco);

                //    stream = new StreamWriter(String.Format(String.Concat(pathProcedures, "\\{0}.sql"), procBuscarPorId));
                //    EscreveCreateProcedure(stream);

                //    if (lstPropriedadesChave.Count > 0 || possuiParamOutPut)
                //    {
                //        stream.WriteLine(String.Format("PROCEDURE {0}", procBuscarPorId));
                //        stream.WriteLine("(");

                //        foreach (PropriedadeVO prop in lstPropriedadesChave)
                //        {
                //            stream.Write(String.Format("\t{0}", prop.GetNomeParametro(classe, prefixoParametro, DriverBanco)));
                //            EscreveSeparadorParametroTipo(stream);
                //            EscreveTipoParametro(stream, classe, prop, prop.Nome == lstPropriedadesChave.Last().Nome && !possuiParamOutPut, true);
                //        }

                //        EscreveSaidaTabelaProcedure(stream);
                //        stream.WriteLine(")");
                //    }

                //    EscreveIniciarProcedure(stream);
                //    stream.WriteLine("");
                //    stream.WriteLine("BEGIN");
                //    EscreveInicioSelectProcedure(stream);
                //    stream.WriteLine("\t\tselect");
                    
                //    foreach (PropriedadeVO prop in classe.Propriedades)
                //    {
                //        stream.WriteLine(String.Format("\t\t\t{0} as {1}{2}{3}", prop.Coluna.Nome, classe.Nome, prop.Nome, prop.Nome != classe.Propriedades.Last().Nome ? "," : String.Empty));
                //    }

                //    stream.WriteLine("\t\tfrom");
                //    stream.WriteLine(String.Format("\t\t\t{0}", classe.Tabela.Nome));
                //    if (lstPropriedadesChave.Count > 0)
                //    {
                //        stream.WriteLine("\t\twhere");
                //        foreach (PropriedadeVO prop in lstPropriedadesChave)
                //        {
                //            if (prop.Nome == lstPropriedadesChave.Last().Nome)
                //                stream.Write(String.Format("\t\t\t{0} = {1}", prop.Coluna.Nome, prop.GetNomeParametro(classe, prefixoParametro, DriverBanco)));
                //            else
                //                stream.WriteLine(String.Format("\t\t\t{0} = {1}", prop.Coluna.Nome, prop.GetNomeParametro(classe, prefixoParametro, DriverBanco)));
                            
                //            if (prop.Nome != lstPropriedadesChave.Last().Nome)
                //                stream.WriteLine("\t\t\tand");
                //            else
                //                stream.WriteLine(";");
                //        }
                //    }
                //    else
                //    {
                //        stream.Write(";");
                //    }
                //    EscreveTerminoProcedure(stream, procBuscarPorId);

                //    stream.Close();

                //    #endregion

                //    #region     .....:::::     CRIA PROCEDURE QUE INSERE INFORMAÇÕES     :::::.....

                //    String procInserir = String.Format("{0}INSERIR_{1}", FrmGeradorProjetos.prefixoProcedure, classe.Nome.ToUpper()).GetNomeProcedure(DriverBanco);

                //    stream = new StreamWriter(String.Format(String.Concat(pathProcedures, "\\{0}.sql"), procInserir));
                //    EscreveCreateProcedure(stream);
                //    stream.Write(String.Format("PROCEDURE {0}", procInserir));
                //    stream.WriteLine("");
                //    stream.WriteLine("(");
                    
                //    foreach (PropriedadeVO prop in classe.Propriedades)
                //    {
                //        stream.Write(String.Format("\t{0}", prop.GetNomeParametro(classe, prefixoParametro, DriverBanco)));
                //        EscreveSeparadorParametroTipo(stream);
                //        EscreveTipoParametro(stream, classe, prop, prop.Nome == classe.Propriedades.Last().Nome, false);
                //    }

                //    stream.WriteLine(")");
                //    EscreveIniciarProcedure(stream);
                //    stream.WriteLine("");
                //    stream.WriteLine("BEGIN");

                //    stream.WriteLine("\t\tinsert into");
                //    stream.WriteLine(String.Format("\t\t\t{0}", classe.Tabela.Nome));
                //    stream.WriteLine("\t\t(");

                //    foreach (PropriedadeVO prop in classe.Propriedades)
                //    {
                //        stream.WriteLine(String.Format("\t\t\t{0}{1}", prop.Coluna.Nome, prop.Coluna.Nome != classe.Propriedades.Last().Coluna.Nome ? "," : String.Empty));
                //    }

                //    stream.WriteLine("\t\t)");
                //    stream.WriteLine("\t\tvalues");
                //    stream.WriteLine("\t\t(");

                //    foreach (PropriedadeVO prop in classe.Propriedades)
                //    {
                //        stream.WriteLine(String.Format("\t\t\t{0}{1}", prop.GetNomeParametro(classe, prefixoParametro, DriverBanco), prop.Coluna.Nome != classe.Propriedades.Last().Coluna.Nome ? "," : String.Empty));
                //    }

                //    stream.WriteLine("\t\t);");
                    
                //    EscreveTerminoProcedure(stream, procInserir);

                //    stream.Close();

                //    #endregion

                //    #region     .....:::::     CRIA PROCEDURE QUE ATUALIZA TODAS INFORMAÇÕES     :::::.....

                //    String procAtualizar = String.Format("{0}ATUALIZAR_{1}", FrmGeradorProjetos.prefixoProcedure, classe.Nome.ToUpper()).GetNomeProcedure(DriverBanco);

                //    stream = new StreamWriter(String.Format(String.Concat(pathProcedures, "\\{0}.sql"), procAtualizar));
                //    EscreveCreateProcedure(stream);
                //    stream.Write(String.Format("PROCEDURE {0}", procAtualizar));
                //    stream.WriteLine("");
                //    stream.WriteLine("(");

                //    foreach (PropriedadeVO prop in classe.Propriedades)
                //    {
                //        stream.Write(String.Format("\t{0}", prop.GetNomeParametro(classe, prefixoParametro, DriverBanco)));
                //        EscreveSeparadorParametroTipo(stream);
                //        EscreveTipoParametro(stream, classe, prop, prop.Nome == classe.Propriedades.Last().Nome, false);
                //    }

                //    stream.WriteLine(")");
                //    EscreveIniciarProcedure(stream);
                //    stream.WriteLine("");
                //    stream.WriteLine("BEGIN");

                //    stream.WriteLine("\t\tupdate");
                //    stream.WriteLine(String.Format("\t\t\t{0}", classe.Tabela.Nome));
                //    stream.WriteLine("\t\tset");
                    
                //    List<PropriedadeVO> lstProperties = classe.Propriedades.Where(x=> !lstPropriedadesChave.Any(x1=> x1.Nome == x.Nome)).ToList();
                //    foreach (PropriedadeVO prop in lstProperties)
                //    {
                //        stream.WriteLine(String.Format("\t\t\t{0} = {1}{2}", prop.Coluna.Nome, prop.GetNomeParametro(classe, prefixoParametro, DriverBanco), prop.Coluna.Nome != lstProperties.Last().Coluna.Nome ? "," : String.Empty));
                //    }

                //    if (lstPropriedadesChave.Count > 0)
                //    {
                //        stream.WriteLine("\t\twhere");
                //        foreach (PropriedadeVO prop in lstPropriedadesChave)
                //        {
                //            if (prop.Nome == lstPropriedadesChave.Last().Nome)
                //                stream.Write(String.Format("\t\t\t{0} = {1}", prop.Coluna.Nome, prop.GetNomeParametro(classe, prefixoParametro, DriverBanco)));
                //            else
                //                stream.WriteLine(String.Format("\t\t\t{0} = {1}", prop.Coluna.Nome, prop.GetNomeParametro(classe, prefixoParametro, DriverBanco)));

                //            if (prop.Nome != lstPropriedadesChave.Last().Nome)
                //                stream.WriteLine("\t\t\tand");
                //            else
                //                stream.WriteLine(";");
                //        }
                //    }

                //    EscreveTerminoProcedure(stream, procAtualizar);

                //    stream.Close();

                //    #endregion

                //    #region     .....:::::     CRIA PROCEDURE QUE EXCLUI INFORMAÇÕES     :::::.....

                //    String procExcluir = String.Format("{0}EXCLUIR_{1}", FrmGeradorProjetos.prefixoProcedure, classe.Nome.ToUpper()).GetNomeProcedure(DriverBanco);

                //    stream = new StreamWriter(String.Format(String.Concat(pathProcedures, "\\{0}.sql"), procExcluir));
                //    EscreveCreateProcedure(stream);
                //    stream.Write(String.Format("PROCEDURE {0}", procExcluir));
                //    stream.WriteLine("");
                //    stream.WriteLine("(");

                //    foreach (PropriedadeVO prop in lstPropriedadesChave)
                //    {
                //        stream.Write(String.Format("\t{0}", prop.GetNomeParametro(classe, prefixoParametro, DriverBanco)));
                //        EscreveSeparadorParametroTipo(stream);
                //        EscreveTipoParametro(stream, classe, prop, prop.Nome == lstPropriedadesChave.Last().Nome, false);
                //    }

                //    stream.WriteLine(")");
                //    EscreveIniciarProcedure(stream);
                //    stream.WriteLine("");
                //    stream.WriteLine("BEGIN");

                //    stream.WriteLine("\t\tdelete from");
                //    stream.WriteLine(String.Format("\t\t\t{0}", classe.Tabela.Nome));
                    
                //    if (lstPropriedadesChave.Count > 0)
                //    {
                //        stream.WriteLine("\t\twhere");
                //        foreach (PropriedadeVO prop in lstPropriedadesChave)
                //        {
                //            if(prop.Nome == lstPropriedadesChave.Last().Nome)
                //                stream.Write(String.Format("\t\t\t{0} = {1}", prop.Coluna.Nome, prop.GetNomeParametro(classe, prefixoParametro, DriverBanco)));
                //            else
                //                stream.WriteLine(String.Format("\t\t\t{0} = {1}", prop.Coluna.Nome, prop.GetNomeParametro(classe, prefixoParametro, DriverBanco)));
                            
                //            if (prop.Nome != lstPropriedadesChave.Last().Nome)
                //                stream.WriteLine("\t\t\tand");
                //            else
                //                stream.WriteLine(";");
                //        }
                //    }

                //    EscreveTerminoProcedure(stream, procExcluir);

                //    stream.Close();

                //    #endregion
                //}

                //#endregion

                if (projeto.Classes.Count > 0)
                {
                    /*

                    #region     .....:::::     CRIA CLASSE BASE DE ACESSO A DADOS NO REPOSITÓRIO     :::::.....

                    ClasseVO classe = new ClasseVO() { Nome = "Base" };

                    String nomeClasse = String.Format("{0}DAO.cs", classe.Nome);
                    String pathClasse = String.Format(@"{0}\{1}", pathAcessoDados, nomeClasse);
                    stream = new StreamWriter(pathClasse, true);
                    
                    #region     .....:::::     ESCREVE AS REFERENCIAS UTILIZADAS     :::::.....

                    stream.WriteLine("using System;");
                    stream.WriteLine("using System.Collections.Generic;");
                    stream.WriteLine("using System.Text;");
                    stream.WriteLine("using System.Data;");
                    stream.WriteLine("using System.Data.Common;");
                    stream.WriteLine("using System.Configuration;");
                    stream.WriteLine("using System.Reflection;");
                    stream.WriteLine("using System.Diagnostics;");
                    stream.WriteLine(String.Format("using {0}.{1};", Solucao.Nome, Solucao.Projetos[0].Tipo.ToString()));
                    EscreveReferenciaBanco(stream);
                                        
                    #endregion

                    stream.WriteLine(String.Format("namespace {0}{1}", String.IsNullOrEmpty(Solucao.Nome) ? String.Empty : String.Concat(Solucao.Nome, "."), projeto.Nome));
                    stream.WriteLine("{");
                    stream.WriteLine(String.Format("\tpublic class {0}DAO", classe.Nome));
                    stream.WriteLine("\t{");

                    stream.WriteLine("");
                    stream.WriteLine("\t\t#region     .....:::::     ATRIBUTOS     :::::.....");
                    stream.WriteLine("");

                    stream.WriteLine("\t\tpublic static List<String> connectionString = new List<String>();");
                    stream.WriteLine("\t\tpublic static Boolean showSql = false;");
                    
                    stream.WriteLine("");
                    stream.WriteLine("\t\t#endregion");

                    #region     .....:::::     CONSTRUTORES     :::::.....

                    stream.WriteLine("");
                    stream.WriteLine("\t\t#region     .....:::::     CONSTRUTORES     :::::.....");
                    stream.WriteLine("");
                    stream.WriteLine("\t\tpublic BaseDAO()");
                    stream.WriteLine("\t\t{");
                    stream.WriteLine("\t\t\tfor(int i = 0; i < ConfigurationManager.ConnectionStrings.Count; i++)");
                    stream.WriteLine("\t\t\t{");
                    stream.WriteLine("\t\t\t\tif (i != 0)");
                    stream.WriteLine("\t\t\t\t\tconnectionString.Add(ConfigurationManager.ConnectionStrings[i].ConnectionString);");
                    stream.WriteLine("\t\t\t}");
                    stream.WriteLine("\t\t\tif (!String.IsNullOrEmpty(ConfigurationManager.AppSettings[\"ShowSql\"]))");
                    stream.WriteLine("\t\t\t{");
                    stream.WriteLine("\t\t\t\tshowSql = Convert.ToBoolean(ConfigurationManager.AppSettings[\"ShowSql\"]);");
                    stream.WriteLine("\t\t\t}");

                    stream.WriteLine("\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t#endregion");
                    
                    #endregion

                    stream.WriteLine("");
                    stream.WriteLine("\t\t#region     .....:::::     MÉTODOS     :::::.....");
                    stream.WriteLine("");

                    stream.WriteLine(String.Format("\t\tpublic {0} GetConnection()", DriverBanco.ToTextConnection()));
                    stream.WriteLine("\t\t{");
                    stream.WriteLine(String.Format("\t\t\t{0} connection = new {0}(connectionString.Count > 0 ? connectionString[0] : String.Empty);", DriverBanco.ToTextConnection()));
                    stream.WriteLine("\t\t\treturn connection;");
                    stream.WriteLine("\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine(String.Format("\t\tpublic {0} GetConnection(Int32 index)", DriverBanco.ToTextConnection()));
                    stream.WriteLine("\t\t{");
                    stream.WriteLine(String.Format("\t\t\t{0} connection = new {0}(connectionString.Count >= (index + 1) ? ConfigurationManager.ConnectionStrings[index].ConnectionString : String.Empty);", DriverBanco.ToTextConnection()));
                    stream.WriteLine("\t\t\treturn connection;");
                    stream.WriteLine("\t\t}");
                    stream.WriteLine("");

                    stream.WriteLine("\t\tpublic void Atualizar<T>(T obj, params String[] props)");
                    stream.WriteLine("\t\t{");
                    stream.WriteLine("\t\t\tList<PropertyInfo> lstPropetiesAt = new List<PropertyInfo>();");
                    stream.WriteLine("\t\t\tList<PropertyInfo> lstProperties = new List<PropertyInfo>(obj.GetType().GetProperties());");
                    stream.WriteLine("\t\t\tif (props.Length != 0)");
                    stream.WriteLine("\t\t\t{");
                    stream.WriteLine("\t\t\t\tforeach(String str in props)");
                    stream.WriteLine("\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\tPropertyInfo prop = lstProperties.Find(delegate(PropertyInfo prop1){ return prop1.Name == str; });");
                    stream.WriteLine("\t\t\t\t\tif(prop != null)");
                    stream.WriteLine("\t\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\t\tlstPropetiesAt.Add(prop);");
                    stream.WriteLine("\t\t\t\t\t}");
                    stream.WriteLine("\t\t\t\t\telse");
                    stream.WriteLine("\t\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\t\tthrow new Exception(String.Format(\"Propriedade {0} inexistente no objeto {1}\", str, obj.GetType().Name));");
                    stream.WriteLine("\t\t\t\t\t}");
                    stream.WriteLine("\t\t\t\t}");
                    stream.WriteLine("\t\t\t}");
                    stream.WriteLine("\t\t\telse");
                    stream.WriteLine("\t\t\t{");
                    stream.WriteLine("\t\t\t\tlstPropetiesAt = lstProperties;");
                    stream.WriteLine("\t\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\tforeach(PropertyInfo prop in lstProperties)");
                    stream.WriteLine("\t\t\t{");
                    stream.WriteLine("\t\t\t\tFieldAnnotation[] lstFieldAttribute = (FieldAnnotation[])prop.GetCustomAttributes(typeof(FieldAnnotation), true);");
                    stream.WriteLine("\t\t\t\tif(lstFieldAttribute.Length == 0)");
                    stream.WriteLine("\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\tif(lstPropetiesAt.Contains(prop))");
                    stream.WriteLine("\t\t\t\t\t\tlstPropetiesAt.Remove(prop);");
                    stream.WriteLine("\t\t\t\t}");
                    stream.WriteLine("\t\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\tList<PropertyInfo> lstChavePrimaria = new List<PropertyInfo>();");
                    stream.WriteLine("\t\t\tforeach(PropertyInfo prop in lstProperties)");
                    stream.WriteLine("\t\t\t{");
                    stream.WriteLine("\t\t\t\tFieldAnnotation[] lstFieldAttribute = (FieldAnnotation[]) prop.GetCustomAttributes(typeof(FieldAnnotation), true);");
                    stream.WriteLine("\t\t\t\tif(lstFieldAttribute.Length > 0)");
                    stream.WriteLine("\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\tFieldAnnotation fieldAttribute = lstFieldAttribute[0];");
                    stream.WriteLine("\t\t\t\t\tif(fieldAttribute.IsPrimaryKey)");
                    stream.WriteLine("\t\t\t\t\t\tlstChavePrimaria.Add(prop);");
                    stream.WriteLine("\t\t\t\t}");
                    stream.WriteLine("\t\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\tClassAnnotation[] lstClassAttribute = (ClassAnnotation[])obj.GetType().GetCustomAttributes(typeof(ClassAnnotation), true);");
                    stream.WriteLine("\t\t\tif (lstPropetiesAt.Count > 0 && lstClassAttribute.Length > 0 && lstChavePrimaria.Count > 0)");
                    stream.WriteLine("\t\t\t{");
                    stream.WriteLine("\t\t\t\tClassAnnotation classAttribute =lstClassAttribute[0];");
                    stream.WriteLine("\t\t\t\tStringBuilder query = new StringBuilder();");
                    stream.WriteLine("\t\t\t\tquery.Append(String.Format(\"UPDATE {0} SET \", classAttribute.TableName));");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\t\tforeach (PropertyInfo prop in lstPropetiesAt)");
                    stream.WriteLine("\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\tFieldAnnotation fieldAttribute = (FieldAnnotation)prop.GetCustomAttributes(typeof(FieldAnnotation), true)[0];");
                    stream.WriteLine("\t\t\t\t\tquery.Append(String.Format(\"{0} = {1}{2}{3}\", fieldAttribute.ColumName, \"" + DriverBanco.GetInicioParametroCommandText() + "\", String.Concat(\"" + prefixoParametro + "\" + fieldAttribute.EnumName), prop.Name != lstPropetiesAt[lstPropetiesAt.Count - 1].Name ? \", \" : \" \"));");
                    stream.WriteLine("\t\t\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\t\tquery.Append(\"where \");");
                    stream.WriteLine("\t\t\t\tforeach (PropertyInfo prop in lstChavePrimaria)");
                    stream.WriteLine("\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\tFieldAnnotation fieldAttribute = (FieldAnnotation)prop.GetCustomAttributes(typeof(FieldAnnotation), true)[0];");
                    stream.WriteLine("\t\t\t\t\tquery.Append(String.Format(\"{0} = {1}{2} {3}\", fieldAttribute.ColumName, \""+ DriverBanco.GetInicioParametroCommandText() +"\", String.Concat(\"" + prefixoParametro + "\" + fieldAttribute.EnumName), prop.Name != lstChavePrimaria[lstChavePrimaria.Count - 1].Name ? \"and \" : String.Empty));");
                    stream.WriteLine("\t\t\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine(String.Format("\t\t\t\t{0} conexao = null;", DriverBanco.ToTextConnection()));
                    stream.WriteLine("\t\t\t\ttry");
                    stream.WriteLine("\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\tconexao = this.GetConnection();");
                    stream.WriteLine(String.Format("\t\t\t\t\t{0} command = conexao.CreateCommand();", DriverBanco.ToTextCommand()));
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\t\t\tforeach (PropertyInfo prop in lstPropetiesAt)");
                    stream.WriteLine("\t\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\t\tFieldAnnotation fieldAttribute = (FieldAnnotation)prop.GetCustomAttributes(typeof(FieldAnnotation), true)[0];");
                    stream.WriteLine(String.Format("\t\t\t\t\t\t{0} param = command.CreateParameter();", DriverBanco.ToTextParameter()));
                    stream.WriteLine("\t\t\t\t\t\tparam.ParameterName = String.Format(\"{0}{1}\", \"" + prefixoParametro + "\", fieldAttribute.EnumName);");
                    stream.WriteLine("\t\t\t\t\t\tparam.Value = prop.GetValue(obj, null);");
                    stream.WriteLine("\t\t\t\t\t\tcommand.Parameters.Add(param);");
                    stream.WriteLine("\t\t\t\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\t\t\tforeach (PropertyInfo prop in lstChavePrimaria)");
                    stream.WriteLine("\t\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\t\tFieldAnnotation fieldAttribute = (FieldAnnotation)prop.GetCustomAttributes(typeof(FieldAnnotation), true)[0];");
                    stream.WriteLine(String.Format("\t\t\t\t\t\t{0} param = command.CreateParameter();", DriverBanco.ToTextParameter()));
                    stream.WriteLine("\t\t\t\t\t\tparam.ParameterName = String.Format(\"{0}{1}\", \"" + prefixoParametro + "\", fieldAttribute.EnumName);");
                    stream.WriteLine("\t\t\t\t\t\tparam.Value = prop.GetValue(obj, null);");
                    stream.WriteLine("\t\t\t\t\t\tcommand.Parameters.Add(param);");
                    stream.WriteLine("\t\t\t\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\t\t\tif(showSql)");
                    stream.WriteLine("\t\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\t\tDebug.WriteLine(String.Format(\"comando: {0}\", query.ToString()));");
                    stream.WriteLine("\t\t\t\t\t\tList<String> logParam = new List<String>();");
                    stream.WriteLine(String.Format("\t\t\t\t\t\tforeach ({0} paramLog in command.Parameters)", DriverBanco.ToTextParameter()));
                    stream.WriteLine("\t\t\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\t\t\tlogParam.Add(String.Format(\"{0} = {1} ({2})\", paramLog.ParameterName, paramLog.Value, paramLog." + DriverBanco.ToTextDbType() + ".ToString()));");
                    stream.WriteLine("\t\t\t\t\t\t}");
                    stream.WriteLine("\t\t\t\t\t\tDebug.WriteLine(String.Format(\"parâmetros: {0}\", String.Join(\", \", logParam.ToArray())));");
                    stream.WriteLine("\t\t\t\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\t\t\tcommand.CommandText = query.ToString();");
                    stream.WriteLine("\t\t\t\t\tcommand.CommandType = CommandType.Text;");
                    stream.WriteLine("\t\t\t\t\tconexao.Open();");
                    stream.WriteLine("\t\t\t\t\tcommand.ExecuteNonQuery();");
                    stream.WriteLine("\t\t\t\t}");
                    stream.WriteLine("\t\t\t\tcatch(Exception ex)");
                    stream.WriteLine("\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\tthrow ex;");
                    stream.WriteLine("\t\t\t\t}");
                    stream.WriteLine("\t\t\t\tfinally");
                    stream.WriteLine("\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\tif (conexao != null)");
                    stream.WriteLine("\t\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\t\tconexao.Close();");
                    stream.WriteLine("\t\t\t\t\t}");
                    stream.WriteLine("\t\t\t\t}");
                    stream.WriteLine("\t\t\t}");
                    stream.WriteLine("\t\t}");
                    stream.WriteLine("");

                    stream.WriteLine("\t\tpublic void Inserir<T>(T obj, params String[] props)");
                    stream.WriteLine("\t\t{");
                    stream.WriteLine("\t\t\tList<PropertyInfo> lstPropetiesIns = new List<PropertyInfo>();");
                    stream.WriteLine("\t\t\tList<PropertyInfo> lstProperties = new List<PropertyInfo>(obj.GetType().GetProperties());");
                    stream.WriteLine("\t\t\tif (props.Length != 0)");
                    stream.WriteLine("\t\t\t{");
                    stream.WriteLine("\t\t\t\tforeach(String str in props)");
                    stream.WriteLine("\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\tPropertyInfo prop = lstProperties.Find(delegate(PropertyInfo prop1){ return prop1.Name == str; });");
                    stream.WriteLine("\t\t\t\t\tif(prop != null)");
                    stream.WriteLine("\t\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\t\tlstPropetiesIns.Add(prop);");
                    stream.WriteLine("\t\t\t\t\t}");
                    stream.WriteLine("\t\t\t\t\telse");
                    stream.WriteLine("\t\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\t\tthrow new Exception(String.Format(\"Propriedade {0} inexistente no objeto {1}\", str, obj.GetType().Name));");
                    stream.WriteLine("\t\t\t\t\t}");
                    stream.WriteLine("\t\t\t\t}");
                    stream.WriteLine("\t\t\t}");
                    stream.WriteLine("\t\t\telse");
                    stream.WriteLine("\t\t\t{");
                    stream.WriteLine("\t\t\t\tlstPropetiesIns = lstProperties;");
                    stream.WriteLine("\t\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\tforeach(PropertyInfo prop in lstProperties)");
                    stream.WriteLine("\t\t\t{");
                    stream.WriteLine("\t\t\t\tFieldAnnotation[] lstFieldAttribute = (FieldAnnotation[])prop.GetCustomAttributes(typeof(FieldAnnotation), true);");
                    stream.WriteLine("\t\t\t\tif(lstFieldAttribute.Length == 0)");
                    stream.WriteLine("\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\tif(lstPropetiesIns.Contains(prop))");
                    stream.WriteLine("\t\t\t\t\t\tlstPropetiesIns.Remove(prop);");
                    stream.WriteLine("\t\t\t\t}");
                    stream.WriteLine("\t\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\tClassAnnotation[] lstClassAttribute = (ClassAnnotation[])obj.GetType().GetCustomAttributes(typeof(ClassAnnotation), true);");
                    stream.WriteLine("\t\t\tif (lstPropetiesIns.Count > 0 && lstClassAttribute.Length > 0)");
                    stream.WriteLine("\t\t\t{");
                    stream.WriteLine("\t\t\t\tClassAnnotation classAttribute = lstClassAttribute[0];");
                    stream.WriteLine("\t\t\t\tStringBuilder query = new StringBuilder();");
                    stream.WriteLine("\t\t\t\tquery.Append(String.Format(\"INSERT INTO {0} (\", classAttribute.TableName));");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\t\tforeach (PropertyInfo prop in lstPropetiesIns)");
                    stream.WriteLine("\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\tFieldAnnotation fieldAttribute = (FieldAnnotation)prop.GetCustomAttributes(typeof(FieldAnnotation), true)[0];");
                    stream.WriteLine("\t\t\t\t\tquery.Append(String.Format(\"{0}{1}\", fieldAttribute.ColumName, prop.Name != lstPropetiesIns[lstPropetiesIns.Count - 1].Name ? \", \" : String.Empty));");
                    stream.WriteLine("\t\t\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\t\tquery.Append(\") VALUES (\");");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\t\tforeach (PropertyInfo prop in lstPropetiesIns)");
                    stream.WriteLine("\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\tFieldAnnotation fieldAttribute = (FieldAnnotation)prop.GetCustomAttributes(typeof(FieldAnnotation), true)[0];");
                    stream.WriteLine("\t\t\t\t\tquery.Append(String.Format(\"{0}{1}{2}\", \""+ DriverBanco.GetInicioParametroCommandText() +"\", String.Concat(\""+ DriverBanco.GetPrefixoParametro() +"\" + fieldAttribute.EnumName), prop.Name != lstPropetiesIns[lstPropetiesIns.Count - 1].Name ? \", \" : \"\"));");
                    stream.WriteLine("\t\t\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\t\tquery.Append(\")\");");
                    stream.WriteLine("");
                    stream.WriteLine(String.Format("\t\t\t\t{0} conexao = null;", DriverBanco.ToTextConnection()));
                    stream.WriteLine("\t\t\t\ttry");
                    stream.WriteLine("\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\tconexao = this.GetConnection();");
                    stream.WriteLine(String.Format("\t\t\t\t\t{0} command = conexao.CreateCommand();", DriverBanco.ToTextCommand()));
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\t\t\tforeach (PropertyInfo prop in lstPropetiesIns)");
                    stream.WriteLine("\t\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\t\tFieldAnnotation fieldAttribute = (FieldAnnotation)prop.GetCustomAttributes(typeof(FieldAnnotation), true)[0];");
                    stream.WriteLine(String.Format("\t\t\t\t\t\t{0} param = command.CreateParameter();", DriverBanco.ToTextParameter()));
                    stream.WriteLine("\t\t\t\t\t\tparam.ParameterName = String.Format(\"{0}{1}\", \"" + prefixoParametro + "\", fieldAttribute.EnumName);");
                    stream.WriteLine("\t\t\t\t\t\tparam.Value = prop.GetValue(obj, null);");
                    stream.WriteLine("\t\t\t\t\t\tcommand.Parameters.Add(param);");
                    stream.WriteLine("\t\t\t\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\t\t\tif(showSql)");
                    stream.WriteLine("\t\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\t\tDebug.WriteLine(String.Format(\"comando: {0}\", query.ToString()));");
                    stream.WriteLine("\t\t\t\t\t\tList<String> logParam = new List<String>();");
                    stream.WriteLine(String.Format("\t\t\t\t\t\tforeach ({0} paramLog in command.Parameters)", DriverBanco.ToTextParameter()));
                    stream.WriteLine("\t\t\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\t\t\tlogParam.Add(String.Format(\"{0} = {1} ({2})\", paramLog.ParameterName, paramLog.Value, paramLog." + DriverBanco.ToTextDbType() + ".ToString()));");
                    stream.WriteLine("\t\t\t\t\t\t}");
                    stream.WriteLine("\t\t\t\t\t\tDebug.WriteLine(String.Format(\"parâmetros: {0}\", String.Join(\", \", logParam.ToArray())));");
                    stream.WriteLine("\t\t\t\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\t\t\tcommand.CommandText = query.ToString();");
                    stream.WriteLine("\t\t\t\t\tcommand.CommandType = CommandType.Text;");
                    stream.WriteLine("\t\t\t\t\tconexao.Open();");
                    stream.WriteLine("\t\t\t\t\tcommand.ExecuteNonQuery();");
                    stream.WriteLine("\t\t\t\t}");
                    stream.WriteLine("\t\t\t\tcatch(Exception ex)");
                    stream.WriteLine("\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\tthrow ex;");
                    stream.WriteLine("\t\t\t\t}");
                    stream.WriteLine("\t\t\t\tfinally");
                    stream.WriteLine("\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\tif (conexao != null)");
                    stream.WriteLine("\t\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\t\tconexao.Close();");
                    stream.WriteLine("\t\t\t\t\t}");
                    stream.WriteLine("\t\t\t\t}");
                    stream.WriteLine("\t\t\t}");
                    stream.WriteLine("\t\t}");
                    stream.WriteLine("");

                    stream.WriteLine("\t\tpublic void Excluir<T>(T obj, params String[] props)");
                    stream.WriteLine("\t\t{");
                    stream.WriteLine("\t\t\tList<PropertyInfo> lstPropetiesDel = new List<PropertyInfo>();");
                    stream.WriteLine("\t\t\tList<PropertyInfo> lstProperties = new List<PropertyInfo>(obj.GetType().GetProperties());");
                    stream.WriteLine("\t\t\tif (props.Length != 0)");
                    stream.WriteLine("\t\t\t{");
                    stream.WriteLine("\t\t\t\tforeach (String str in props)");
                    stream.WriteLine("\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\tPropertyInfo prop = lstProperties.Find(delegate(PropertyInfo prop1) { return prop1.Name == str; });");
                    stream.WriteLine("\t\t\t\t\tif (prop != null)");
                    stream.WriteLine("\t\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\t\tlstPropetiesDel.Add(prop);");
                    stream.WriteLine("\t\t\t\t\t}");
                    stream.WriteLine("\t\t\t\t\telse");
                    stream.WriteLine("\t\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\t\tthrow new Exception(String.Format(\"Propriedade {0} inexistente no objeto {1}\", str, obj.GetType().Name));");
                    stream.WriteLine("\t\t\t\t\t}");
                    stream.WriteLine("\t\t\t\t}");
                    stream.WriteLine("\t\t\t}");
                    stream.WriteLine("\t\t\telse");
                    stream.WriteLine("\t\t\t{");
                    stream.WriteLine("\t\t\t\tthrow new Exception(\"Informe pelo menos uma propriedade para poder excluir a informação\");");
                    stream.WriteLine("\t\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\tClassAnnotation[] lstClassAttribute = (ClassAnnotation[])obj.GetType().GetCustomAttributes(typeof(ClassAnnotation), true);");
                    stream.WriteLine("\t\t\tif (lstPropetiesDel.Count > 0 && lstClassAttribute.Length > 0)");
                    stream.WriteLine("\t\t\t{");
                    stream.WriteLine("\t\t\t\tClassAnnotation classAttribute = lstClassAttribute[0];");
                    stream.WriteLine("\t\t\t\tStringBuilder query = new StringBuilder();");
                    stream.WriteLine("\t\t\t\tquery.Append(String.Format(\"DELETE FROM {0} WHERE \", classAttribute.TableName));");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\t\tforeach (PropertyInfo prop in lstPropetiesDel)");
                    stream.WriteLine("\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\tFieldAnnotation fieldAttribute = (FieldAnnotation)prop.GetCustomAttributes(typeof(FieldAnnotation), true)[0];");
                    stream.WriteLine("\t\t\t\t\tquery.Append(String.Format(\"{0} = {1}{2}{3}\", fieldAttribute.ColumName, \"" + DriverBanco.GetInicioParametroCommandText() + "\", String.Concat(\""+ DriverBanco.GetPrefixoParametro() +"\" + fieldAttribute.EnumName), prop.Name != lstPropetiesDel[lstPropetiesDel.Count - 1].Name ? \" and \" : \"\"));");
                    stream.WriteLine("\t\t\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine(String.Format("\t\t\t\t{0} conexao = null;", DriverBanco.ToTextConnection()));
                    stream.WriteLine("\t\t\t\ttry");
                    stream.WriteLine("\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\tconexao = this.GetConnection();");
                    stream.WriteLine(String.Format("\t\t\t\t\t{0} command = conexao.CreateCommand();", DriverBanco.ToTextCommand()));
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\t\t\tforeach (PropertyInfo prop in lstPropetiesDel)");
                    stream.WriteLine("\t\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\t\tFieldAnnotation fieldAttribute = (FieldAnnotation)prop.GetCustomAttributes(typeof(FieldAnnotation), true)[0];");
                    stream.WriteLine(String.Format("\t\t\t\t\t\t{0} param = command.CreateParameter();", DriverBanco.ToTextParameter()));
                    stream.WriteLine("\t\t\t\t\t\tparam.ParameterName = String.Format(\"{0}{1}\", \"" + prefixoParametro + "\", fieldAttribute.EnumName);");
                    stream.WriteLine("\t\t\t\t\t\tparam.Value = prop.GetValue(obj, null);");
                    stream.WriteLine("\t\t\t\t\t\tcommand.Parameters.Add(param);");
                    stream.WriteLine("\t\t\t\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\t\t\tif(showSql)");
                    stream.WriteLine("\t\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\t\tDebug.WriteLine(String.Format(\"comando: {0}\", query.ToString()));");
                    stream.WriteLine("\t\t\t\t\t\tList<String> logParam = new List<String>();");
                    stream.WriteLine(String.Format("\t\t\t\t\t\tforeach ({0} paramLog in command.Parameters)", DriverBanco.ToTextParameter()));
                    stream.WriteLine("\t\t\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\t\t\tlogParam.Add(String.Format(\"{0} = {1} ({2})\", paramLog.ParameterName, paramLog.Value, paramLog." + DriverBanco.ToTextDbType() + ".ToString()));");
                    stream.WriteLine("\t\t\t\t\t\t}");
                    stream.WriteLine("\t\t\t\t\t\tDebug.WriteLine(String.Format(\"parâmetros: {0}\", String.Join(\", \", logParam.ToArray())));");
                    stream.WriteLine("\t\t\t\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\t\t\tcommand.CommandText = query.ToString();");
                    stream.WriteLine("\t\t\t\t\tcommand.CommandType = CommandType.Text;");
                    stream.WriteLine("\t\t\t\t\tconexao.Open();");
                    stream.WriteLine("\t\t\t\t\tcommand.ExecuteNonQuery();");
                    stream.WriteLine("\t\t\t\t}");
                    stream.WriteLine("\t\t\t\tcatch(Exception ex)");
                    stream.WriteLine("\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\tthrow ex;");
                    stream.WriteLine("\t\t\t\t}");
                    stream.WriteLine("\t\t\t\tfinally");
                    stream.WriteLine("\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\tif (conexao != null)");
                    stream.WriteLine("\t\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\t\tconexao.Close();");
                    stream.WriteLine("\t\t\t\t\t}");
                    stream.WriteLine("\t\t\t\t}");
                    stream.WriteLine("\t\t\t}");
                    stream.WriteLine("\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine("\t\tpublic DataTable Buscar<T>(params String[] props)");
                    stream.WriteLine("\t\t{");
                    stream.WriteLine("\t\t\tDataTable dtRetorno = new DataTable();");
                    stream.WriteLine("\t\t\tType type = typeof(T);");
                    stream.WriteLine("\t\t\tList<PropertyInfo> lstPropetiesSel = new List<PropertyInfo>();");
                    stream.WriteLine("\t\t\tList<PropertyInfo> lstProperties = new List<PropertyInfo>(type.GetProperties());");
                    stream.WriteLine("\t\t\tif (props.Length != 0)");
                    stream.WriteLine("\t\t\t{");
                    stream.WriteLine("\t\t\t\tforeach (String str in props)");
                    stream.WriteLine("\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\tPropertyInfo prop = lstProperties.Find(delegate(PropertyInfo prop1) { return prop1.Name == str; });");
                    stream.WriteLine("\t\t\t\t\tif (prop != null)");
                    stream.WriteLine("\t\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\t\tlstPropetiesSel.Add(prop);");
                    stream.WriteLine("\t\t\t\t\t}");
                    stream.WriteLine("\t\t\t\t\telse");
                    stream.WriteLine("\t\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\t\tthrow new Exception(String.Format(\"Propriedade {0} inexistente no objeto {1}\", str, type.Name));");
                    stream.WriteLine("\t\t\t\t\t}");
                    stream.WriteLine("\t\t\t\t}");
                    stream.WriteLine("\t\t\t}");
                    stream.WriteLine("\t\t\telse");
                    stream.WriteLine("\t\t\t{");
                    stream.WriteLine("\t\t\t\tlstPropetiesSel = lstProperties;");
                    stream.WriteLine("\t\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\tClassAnnotation[] lstClassAttribute = (ClassAnnotation[])type.GetCustomAttributes(typeof(ClassAnnotation), true);");
                    stream.WriteLine("\t\t\tif (lstPropetiesSel.Count > 0 && lstClassAttribute.Length > 0)");
                    stream.WriteLine("\t\t\t{");
                    stream.WriteLine("\t\t\t\tClassAnnotation classAttribute = lstClassAttribute[0];");
                    stream.WriteLine("\t\t\t\tStringBuilder query = new StringBuilder();");
                    stream.WriteLine("\t\t\t\tquery.Append(\"SELECT \");");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\t\tforeach (PropertyInfo prop in lstPropetiesSel)");
                    stream.WriteLine("\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\tFieldAnnotation fieldAttribute = (FieldAnnotation)prop.GetCustomAttributes(typeof(FieldAnnotation), true)[0];");
                    stream.WriteLine("\t\t\t\t\tquery.Append(String.Format(\"{0} AS {1}{2}\", fieldAttribute.ColumName, fieldAttribute.EnumName, prop.Name != lstPropetiesSel[lstPropetiesSel.Count - 1].Name ? \", \" : \" \"));");
                    stream.WriteLine("\t\t\t\t}");
                    stream.WriteLine("\t\t\t\tquery.Append(String.Format(\"FROM {0}\", classAttribute.TableName));");
                    stream.WriteLine("");
                    stream.WriteLine(String.Format("\t\t\t\t{0} conexao = null;", DriverBanco.ToTextConnection()));
                    stream.WriteLine("\t\t\t\ttry");
                    stream.WriteLine("\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\tconexao = this.GetConnection();");
                    stream.WriteLine(String.Format("\t\t\t\t\t{0} command = conexao.CreateCommand();", DriverBanco.ToTextCommand()));
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\t\t\tif (showSql)");
                    stream.WriteLine("\t\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\t\tDebug.WriteLine(String.Format(\"comando: {0}\", query.ToString()));");
                    stream.WriteLine("\t\t\t\t\t\tList<String> logParam = new List<String>();");
                    stream.WriteLine(String.Format("\t\t\t\t\t\tforeach ({0} paramLog in command.Parameters)", DriverBanco.ToTextParameter()));
                    stream.WriteLine("\t\t\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\t\t\tlogParam.Add(String.Format(\"{0} = {1} ({2})\", paramLog.ParameterName, paramLog.Value, paramLog." + DriverBanco.ToTextDbType() +".ToString()));");
                    stream.WriteLine("\t\t\t\t\t\t}");
                    stream.WriteLine("\t\t\t\t\t\tDebug.WriteLine(String.Format(\"parâmetros: {0}\", String.Join(\", \", logParam.ToArray())));");
                    stream.WriteLine("\t\t\t\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\t\t\tcommand.CommandText = query.ToString();");
                    stream.WriteLine("\t\t\t\t\tcommand.CommandType = CommandType.Text;");
                    stream.WriteLine("\t\t\t\t\tconexao.Open();");
                    stream.WriteLine("\t\t\t\t\tdtRetorno.Load(command.ExecuteReader());");
                    stream.WriteLine("\t\t\t\t}");
                    stream.WriteLine("\t\t\t\tcatch (Exception ex)");
                    stream.WriteLine("\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\tthrow ex;");
                    stream.WriteLine("\t\t\t\t}");
                    stream.WriteLine("\t\t\t\tfinally");
                    stream.WriteLine("\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\tif (conexao != null)");
                    stream.WriteLine("\t\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\t\tconexao.Close();");
                    stream.WriteLine("\t\t\t\t\t}");
                    stream.WriteLine("\t\t\t\t}");
                    stream.WriteLine("\t\t\t}");
                    stream.WriteLine("\t\t\treturn dtRetorno;");
                    stream.WriteLine("\t\t}");
                    stream.WriteLine("");

                    stream.WriteLine("\t\tpublic DataTable Buscar<T>(Condition condicao, params String[] props)");
                    stream.WriteLine("\t\t{");
                    stream.WriteLine("\t\t\tDataTable dtRetorno = new DataTable();");
                    stream.WriteLine("\t\t\tType type = typeof(T);");
                    stream.WriteLine("\t\t\tList<PropertyInfo> lstPropetiesSel = new List<PropertyInfo>();");
                    stream.WriteLine("\t\t\tList<PropertyInfo> lstProperties = new List<PropertyInfo>(type.GetProperties());");
                    stream.WriteLine("\t\t\tif (props.Length != 0)");
                    stream.WriteLine("\t\t\t{");
                    stream.WriteLine("\t\t\t\tforeach (String str in props)");
                    stream.WriteLine("\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\tPropertyInfo prop = lstProperties.Find(delegate(PropertyInfo prop1) { return prop1.Name == str; });");
                    stream.WriteLine("\t\t\t\t\tif (prop != null)");
                    stream.WriteLine("\t\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\t\tlstPropetiesSel.Add(prop);");
                    stream.WriteLine("\t\t\t\t\t}");
                    stream.WriteLine("\t\t\t\t\telse");
                    stream.WriteLine("\t\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\t\tthrow new Exception(String.Format(\"Propriedade {0} inexistente no objeto {1}\", str, type.Name));");
                    stream.WriteLine("\t\t\t\t\t}");
                    stream.WriteLine("\t\t\t\t}");
                    stream.WriteLine("\t\t\t}");
                    stream.WriteLine("\t\t\telse");
                    stream.WriteLine("\t\t\t{");
                    stream.WriteLine("\t\t\t\tlstPropetiesSel = lstProperties;");
                    stream.WriteLine("\t\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\t// Valida se existe a propriedade da condição dentro do objeto");
                    stream.WriteLine("\t\t\tforeach (KeyValuePair<String, KeyValuePair<object, ETipoWhere>> keyValue in condicao.GetCondicoes())");
                    stream.WriteLine("\t\t\t{");
                    stream.WriteLine("\t\t\t\tif(type.GetProperty(keyValue.Key) == null)");
                    stream.WriteLine("\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\tthrow(new Exception(String.Format(\"Condição com a propriedade {0} inexistente no objeto {1}\", keyValue.Key, type.Name)));");
                    stream.WriteLine("\t\t\t\t}");
                    stream.WriteLine("\t\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\tClassAnnotation[] lstClassAttribute = (ClassAnnotation[])type.GetCustomAttributes(typeof(ClassAnnotation), true);");
                    stream.WriteLine("\t\t\tif (lstPropetiesSel.Count > 0 && lstClassAttribute.Length > 0)");
                    stream.WriteLine("\t\t\t{");
                    stream.WriteLine("\t\t\t\tClassAnnotation classAttribute = lstClassAttribute[0];");
                    stream.WriteLine("\t\t\t\tStringBuilder query = new StringBuilder();");
                    stream.WriteLine("\t\t\t\tquery.Append(\"SELECT \");");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\t\tforeach (PropertyInfo prop in lstPropetiesSel)");
                    stream.WriteLine("\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\tFieldAnnotation fieldAttribute = (FieldAnnotation)prop.GetCustomAttributes(typeof(FieldAnnotation), true)[0];");
                    stream.WriteLine("\t\t\t\t\tquery.Append(String.Format(\"{0} AS {1}{2}\", fieldAttribute.ColumName, fieldAttribute.EnumName, prop.Name != lstPropetiesSel[lstPropetiesSel.Count - 1].Name ? \", \" : \" \"));");
                    stream.WriteLine("\t\t\t\t}");
                    stream.WriteLine("\t\t\t\tquery.Append(String.Format(\"FROM {0} WHERE \", classAttribute.TableName));");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\t\tforeach (KeyValuePair<String, KeyValuePair<object, ETipoWhere>> keyValue in condicao.GetCondicoes())");
                    stream.WriteLine("\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\tPropertyInfo prop = type.GetProperty(keyValue.Key);");
                    stream.WriteLine("\t\t\t\t\tFieldAnnotation fieldAttribute = (FieldAnnotation)prop.GetCustomAttributes(typeof(FieldAnnotation), true)[0];");
                    stream.WriteLine("\t\t\t\t\tquery.Append(String.Format(\"{0}{1} = {2}{3}{4} \", keyValue.Key == condicao.GetCondicoes()[0].Key ? String.Empty : Condition.ToTextTipoWhere(keyValue.Value.Value), fieldAttribute.ColumName, \""+ DriverBanco.GetInicioParametroCommandText() +"\", \""+DriverBanco.GetPrefixoParametro()+"\", fieldAttribute.EnumName));");
                    stream.WriteLine("\t\t\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine(String.Format("\t\t\t\t{0} conexao = null;", DriverBanco.ToTextConnection()));
                    stream.WriteLine("\t\t\t\ttry");
                    stream.WriteLine("\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\tconexao = this.GetConnection();");
                    stream.WriteLine("\t\t\t\t\tDbCommand command = conexao.CreateCommand();");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\t\t\tforeach (KeyValuePair<String, KeyValuePair<object, ETipoWhere>> keyValue in condicao.GetCondicoes())");
                    stream.WriteLine("\t\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\t\tPropertyInfo prop = type.GetProperty(keyValue.Key);");
                    stream.WriteLine("\t\t\t\t\t\tFieldAnnotation fieldAttribute = (FieldAnnotation)prop.GetCustomAttributes(typeof(FieldAnnotation), true)[0];");
                    stream.WriteLine(String.Format("\t\t\t\t\t\t{0} param = command.CreateParameter();", DriverBanco.ToTextParameter()));
                    stream.WriteLine("\t\t\t\t\t\tparam.ParameterName = String.Format(\"{0}{1}\", \""+ DriverBanco.GetInicioParametroCommandText() +"\", fieldAttribute.EnumName);");
                    stream.WriteLine("\t\t\t\t\t\tparam.Value = keyValue.Value.Key;");
                    stream.WriteLine("\t\t\t\t\t\tcommand.Parameters.Add(param);");
                    stream.WriteLine("\t\t\t\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\t\t\tif (showSql)");
                    stream.WriteLine("\t\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\t\tDebug.WriteLine(String.Format(\"comando: {0}\", query.ToString()));");
                    stream.WriteLine("\t\t\t\t\t\tList<String> logParam = new List<String>();");
                    stream.WriteLine(String.Format("\t\t\t\t\t\tforeach ({0} paramLog in command.Parameters)", DriverBanco.ToTextParameter() ));
                    stream.WriteLine("\t\t\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\t\t\tlogParam.Add(String.Format(\"{0} = {1} ({2})\", paramLog.ParameterName, paramLog.Value, paramLog.DbType.ToString()));");
                    stream.WriteLine("\t\t\t\t\t\t}");
                    stream.WriteLine("\t\t\t\t\t\tDebug.WriteLine(String.Format(\"parâmetros: {0}\", String.Join(\", \", logParam.ToArray())));");
                    stream.WriteLine("\t\t\t\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\t\t\tcommand.CommandText = query.ToString();");
                    stream.WriteLine("\t\t\t\t\tcommand.CommandType = CommandType.Text;");
                    stream.WriteLine("\t\t\t\t\tconexao.Open();");
                    stream.WriteLine("\t\t\t\t\tdtRetorno.Load(command.ExecuteReader());");
                    stream.WriteLine("\t\t\t\t}");
                    stream.WriteLine("\t\t\t\tcatch(Exception ex)");
                    stream.WriteLine("\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\tthrow ex;");
                    stream.WriteLine("\t\t\t\t}");
                    stream.WriteLine("\t\t\t\tfinally");
                    stream.WriteLine("\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\tif (conexao != null)");
                    stream.WriteLine("\t\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\t\tconexao.Close();");
                    stream.WriteLine("\t\t\t\t\t}");
                    stream.WriteLine("\t\t\t\t}");
                    stream.WriteLine("\t\t\t}");
                    stream.WriteLine("\t\t\treturn dtRetorno;");
                    stream.WriteLine("\t\t}");
                    
                    stream.WriteLine("");
                    stream.WriteLine("\t\tpublic DataTable BuscarPorId<T>(T obj, params String[] props)");
                    stream.WriteLine("\t\t{");
                    stream.WriteLine("\t\t\tDataTable dtRetorno = new DataTable();");
                    stream.WriteLine("\t\t\tList<PropertyInfo> lstPropetiesSel = new List<PropertyInfo>();");
                    stream.WriteLine("\t\t\tList<PropertyInfo> lstPropetiesChave = new List<PropertyInfo>();");
                    stream.WriteLine("\t\t\tList<PropertyInfo> lstProperties = new List<PropertyInfo>(obj.GetType().GetProperties());");
                    stream.WriteLine("\t\t\tif (props.Length != 0)");
                    stream.WriteLine("\t\t\t{");
                    stream.WriteLine("\t\t\t\tforeach (String str in props)");
                    stream.WriteLine("\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\tPropertyInfo prop = lstProperties.Find(delegate(PropertyInfo prop1) { return prop1.Name == str; });");
                    stream.WriteLine("\t\t\t\t\tif (prop != null)");
                    stream.WriteLine("\t\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\t\tlstPropetiesSel.Add(prop);");
                    stream.WriteLine("\t\t\t\t\t}");
                    stream.WriteLine("\t\t\t\t\telse");
                    stream.WriteLine("\t\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\t\tthrow new Exception(String.Format(\"Propriedade {0} inexistente no objeto {1}\", str, obj.GetType().Name));");
                    stream.WriteLine("\t\t\t\t\t}");
                    stream.WriteLine("\t\t\t\t}");
                    stream.WriteLine("\t\t\t}");
                    stream.WriteLine("\t\t\telse");
                    stream.WriteLine("\t\t\t{");
                    stream.WriteLine("\t\t\t\tlstPropetiesSel = lstProperties;");
                    stream.WriteLine("\t\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\tforeach (PropertyInfo prop in lstProperties)");
                    stream.WriteLine("\t\t\t{");
                    stream.WriteLine("\t\t\t\tFieldAnnotation[] fieldsAttribute = (FieldAnnotation[])prop.GetCustomAttributes(typeof(FieldAnnotation), true);");
                    stream.WriteLine("\t\t\t\tif (fieldsAttribute.Length > 0)");
                    stream.WriteLine("\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\tif (fieldsAttribute[0].IsPrimaryKey)");
                    stream.WriteLine("\t\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\t\tlstPropetiesChave.Add(prop);");
                    stream.WriteLine("\t\t\t\t\t}");
                    stream.WriteLine("\t\t\t\t}");
                    stream.WriteLine("\t\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\tif (lstPropetiesChave.Count == 0)");
                    stream.WriteLine("\t\t\t{");
                    stream.WriteLine("\t\t\t\tthrow new Exception(\"Objeto não possui chave primária para a busca\");");
                    stream.WriteLine("\t\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\tClassAnnotation[] lstClassAttribute = (ClassAnnotation[])obj.GetType().GetCustomAttributes(typeof(ClassAnnotation), true);");
                    stream.WriteLine("\t\t\tif (lstPropetiesSel.Count > 0 && lstClassAttribute.Length > 0)");
                    stream.WriteLine("\t\t\t{");
                    stream.WriteLine("\t\t\t\tClassAnnotation classAttribute = lstClassAttribute[0];");
                    stream.WriteLine("\t\t\t\tStringBuilder query = new StringBuilder();");
                    stream.WriteLine("\t\t\t\tquery.Append(\"SELECT \");");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\t\tforeach (PropertyInfo prop in lstPropetiesSel)");
                    stream.WriteLine("\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\tFieldAnnotation fieldAttribute = (FieldAnnotation)prop.GetCustomAttributes(typeof(FieldAnnotation), true)[0];");
                    stream.WriteLine("\t\t\t\t\tquery.Append(String.Format(\"{0} AS {1}{2}\", fieldAttribute.ColumName, fieldAttribute.EnumName, prop.Name != lstPropetiesSel[lstPropetiesSel.Count - 1].Name ? \", \" : \" \"));");
                    stream.WriteLine("\t\t\t\t}");
                    stream.WriteLine("\t\t\t\tquery.Append(String.Format(\"FROM {0} WHERE \", classAttribute.TableName));");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\t\tforeach (PropertyInfo prop in lstPropetiesChave)");
                    stream.WriteLine("\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\tFieldAnnotation fieldAttribute = (FieldAnnotation)prop.GetCustomAttributes(typeof(FieldAnnotation), true)[0];");
                    stream.WriteLine("\t\t\t\t\tquery.Append(String.Format(\"{0} = {1}{2}{3}{4} \", fieldAttribute.ColumName, \""+ DriverBanco.GetInicioParametroCommandText() +"\", \""+ DriverBanco.GetPrefixoParametro() +"\", fieldAttribute.EnumName, prop.Name != lstPropetiesChave[lstPropetiesChave.Count - 1].Name ? \" and \" : \"\"));");
                    stream.WriteLine("\t\t\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine(String.Format("\t\t\t\t{0} conexao = null;", DriverBanco.ToTextConnection()));
                    stream.WriteLine("\t\t\t\ttry");
                    stream.WriteLine("\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\tconexao = this.GetConnection();");
                    stream.WriteLine(String.Format("\t\t\t\t\t{0} command = conexao.CreateCommand();", DriverBanco.ToTextCommand()));
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\t\t\tforeach (PropertyInfo prop in lstPropetiesChave)");
                    stream.WriteLine("\t\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\t\tFieldAnnotation fieldAttribute = (FieldAnnotation)prop.GetCustomAttributes(typeof(FieldAnnotation), true)[0];");
                    stream.WriteLine(String.Format("\t\t\t\t\t\t{0} param = command.CreateParameter();", DriverBanco.ToTextParameter()));
                    stream.WriteLine("\t\t\t\t\t\tparam.ParameterName = String.Format(\"{0}{1}\", \""+ DriverBanco.GetPrefixoParametro() +"\", fieldAttribute.EnumName);");
                    stream.WriteLine("\t\t\t\t\t\tparam.Value = prop.GetValue(obj, null);");
                    stream.WriteLine("\t\t\t\t\t\tcommand.Parameters.Add(param);");
                    stream.WriteLine("\t\t\t\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\t\t\tif (showSql)");
                    stream.WriteLine("\t\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\t\tDebug.WriteLine(String.Format(\"comando: {0}\", query.ToString()));");
                    stream.WriteLine("\t\t\t\t\t\tList<String> logParam = new List<String>();");
                    stream.WriteLine(String.Format("\t\t\t\t\t\tforeach ({0} paramLog in command.Parameters)", DriverBanco.ToTextParameter()));
                    stream.WriteLine("\t\t\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\t\t\tlogParam.Add(String.Format(\"{0} = {1} ({2})\", paramLog.ParameterName, paramLog.Value, paramLog.DbType.ToString()));");
                    stream.WriteLine("\t\t\t\t\t\t}");
                    stream.WriteLine("\t\t\t\t\t\tDebug.WriteLine(String.Format(\"parâmetros: {0}\", String.Join(\", \", logParam.ToArray())));");
                    stream.WriteLine("\t\t\t\t\t}");
                    stream.WriteLine("");
                    stream.WriteLine("\t\t\t\t\tcommand.CommandText = query.ToString();");
                    stream.WriteLine("\t\t\t\t\tcommand.CommandType = CommandType.Text;");
                    stream.WriteLine("\t\t\t\t\tconexao.Open();");
                    stream.WriteLine("\t\t\t\t\tdtRetorno.Load(command.ExecuteReader());");
                    stream.WriteLine("\t\t\t\t}");
                    stream.WriteLine("\t\t\t\tcatch(Exception ex)");
                    stream.WriteLine("\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\tthrow ex;");
                    stream.WriteLine("\t\t\t\t}");
                    stream.WriteLine("\t\t\t\tfinally");
                    stream.WriteLine("\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\tif (conexao != null)");
                    stream.WriteLine("\t\t\t\t\t{");
                    stream.WriteLine("\t\t\t\t\t\tconexao.Close();");
                    stream.WriteLine("\t\t\t\t\t}");
                    stream.WriteLine("\t\t\t\t}");
                    stream.WriteLine("\t\t\t}");
                    stream.WriteLine("\t\t\treturn dtRetorno;");
                    stream.WriteLine("\t\t}");


                    stream.WriteLine("");
                    stream.WriteLine("\t\t#endregion");

                    stream.WriteLine("\t}");
                    stream.WriteLine("}");
                    stream.Close();

                    #endregion
                     * 
                     * */

                    #region     .....:::::     CRIA ARQUIVO ASSEMBLYINFO     :::::.....

                    String pathProperties = String.Concat(pathAcessoDados, @"\Properties");
                    Directory.CreateDirectory(pathProperties);
                    stream = new StreamWriter(String.Concat(pathProperties, @"\AssemblyInfo.cs"), true);

                    stream.WriteLine("using System.Reflection;");
                    stream.WriteLine("using System.Runtime.CompilerServices;");
                    stream.WriteLine("using System.Runtime.InteropServices;");
                    stream.WriteLine("");
                    stream.WriteLine("");
                    stream.WriteLine("");
                    stream.WriteLine(String.Format("[assembly: AssemblyTitle(\"{0}\")]", projeto.Nome));
                    stream.WriteLine("[assembly: AssemblyDescription(\"\")]");
                    stream.WriteLine("[assembly: AssemblyConfiguration(\"\")]");
                    stream.WriteLine("[assembly: AssemblyCompany(\"P.H.F. Dias Software\")]");
                    stream.WriteLine(String.Format("[assembly: AssemblyProduct(\"{0}\")]", projeto.Nome));
                    stream.WriteLine("[assembly: AssemblyCopyright(\"Copyright © P.H.F. Dias Software 2012\")]");
                    stream.WriteLine("[assembly: AssemblyTrademark(\"\")]");
                    stream.WriteLine("[assembly: AssemblyCulture(\"\")]");
                    stream.WriteLine("");
                    stream.WriteLine("[assembly: ComVisible(false)]");
                    stream.WriteLine("");
                    stream.WriteLine(String.Format("[assembly: Guid(\"{0}\")]", projeto.Guid.ToString()));
                    stream.WriteLine("");
                    stream.WriteLine("");
                    stream.WriteLine("[assembly: AssemblyVersion(\"1.0\")]");
                    stream.WriteLine("[assembly: AssemblyFileVersion(\"1.0\")]");

                    stream.Close();

                    #endregion

                    #region     .....:::::     CRIA ARQUIVO DE PROJETO     :::::.....

                    String nomeProjeto = String.Format("{0}.csproj", ETipoProjeto.AcessoDados.ToString());
                    String pathProjeto = String.Format(@"{0}\{1}", pathAcessoDados, nomeProjeto);

                    stream = new StreamWriter(pathProjeto, true);
                    stream.WriteLine("<Project DefaultTargets=\"Build\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">");

                    stream.WriteLine("\t<PropertyGroup>");
                    stream.WriteLine("\t\t<SchemaVersion>2.0</SchemaVersion>");
                    stream.WriteLine(String.Format("\t\t<ProjectGuid>{0}</ProjectGuid>", String.Concat("{", projeto.Guid.ToString(), "}")));
                    stream.WriteLine("\t\t<OutputType>Library</OutputType>");
                    stream.WriteLine("\t\t<AppDesignerFolder>Properties</AppDesignerFolder>");
                    stream.WriteLine(String.Format("\t\t<RootNamespace>{0}{1}</RootNamespace>", String.IsNullOrEmpty(Solucao.Nome) ? String.Empty : String.Concat(Solucao.Nome, "."), projeto.Namespace));
                    stream.WriteLine(String.Format("\t\t<AssemblyName>{0}{1}</AssemblyName>", String.IsNullOrEmpty(Solucao.Nome) ? String.Empty : String.Concat(Solucao.Nome, "."), projeto.Namespace));
                    stream.WriteLine("\t\t<SccProjectName></SccProjectName>");
                    stream.WriteLine("\t\t<SccLocalPath></SccLocalPath>");
                    stream.WriteLine("\t\t<SccAuxPath></SccAuxPath>");
                    stream.WriteLine("\t\t<SccProvider></SccProvider>");
                    stream.WriteLine("\t</PropertyGroup>");

                    stream.WriteLine("\t<PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' \">");
                    stream.WriteLine("\t\t<DebugSymbols>true</DebugSymbols>");
                    stream.WriteLine("\t\t<DebugType>full</DebugType>");
                    stream.WriteLine("\t\t<Optimize>false</Optimize>");
                    stream.WriteLine("\t\t<OutputPath>bin\\Debug\\</OutputPath>");
                    stream.WriteLine("\t\t<DefineConstants>DEBUG;TRACE</DefineConstants>");
                    stream.WriteLine("\t\t<ErrorReport>prompt</ErrorReport>");
                    stream.WriteLine("\t\t<WarningLevel>4</WarningLevel>");
                    stream.WriteLine("\t</PropertyGroup>");

                    stream.WriteLine("\t<PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' \">");
                    stream.WriteLine("\t\t<DebugType>pdbonly</DebugType>");
                    stream.WriteLine("\t\t<Optimize>true</Optimize>");
                    stream.WriteLine("\t\t<OutputPath>bin\\Release\\</OutputPath>");
                    stream.WriteLine("\t\t<DefineConstants>TRACE</DefineConstants>");
                    stream.WriteLine("\t\t<ErrorReport>prompt</ErrorReport>");
                    stream.WriteLine("\t\t<WarningLevel>4</WarningLevel>");
                    stream.WriteLine("\t</PropertyGroup>");

                    //stream.WriteLine("\t<ItemGroup>");
                    //foreach (ClasseVO classe1 in projeto.Classes)
                    //{
                    //    String procBuscar = String.Format("{0}BUSCAR_{1}", FrmGeradorProjetos.prefixoProcedure, classe1.Nome.ToUpper()).GetNomeProcedure(DriverBanco);
                    //    String procBuscarPorId = String.Format("{0}BUSCAR_POR_ID_{1}", FrmGeradorProjetos.prefixoProcedure, classe1.Nome.ToUpper()).GetNomeProcedure(DriverBanco);
                    //    String procExcluir = String.Format("{0}EXCLUIR_{1}", FrmGeradorProjetos.prefixoProcedure, classe1.Nome.ToUpper()).GetNomeProcedure(DriverBanco);
                    //    String procAtualizar = String.Format("{0}ATUALIZAR_{1}", FrmGeradorProjetos.prefixoProcedure, classe1.Nome.ToUpper()).GetNomeProcedure(DriverBanco);
                    //    String procInserir = String.Format("{0}INSERIR_{1}", FrmGeradorProjetos.prefixoProcedure, classe1.Nome.ToUpper()).GetNomeProcedure(DriverBanco);
                    //    stream.WriteLine(String.Format("\t\t<Content Include=\"Procedures\\{0}.sql\" />", procBuscar));
                    //    stream.WriteLine(String.Format("\t\t<Content Include=\"Procedures\\{0}.sql\" />", procBuscarPorId));
                    //    stream.WriteLine(String.Format("\t\t<Content Include=\"Procedures\\{0}.sql\" />", procInserir));
                    //    stream.WriteLine(String.Format("\t\t<Content Include=\"Procedures\\{0}.sql\" />", procAtualizar));
                    //    stream.WriteLine(String.Format("\t\t<Content Include=\"Procedures\\{0}.sql\" />", procExcluir));
                    //}
                    //stream.WriteLine("\t</ItemGroup>");

                    if (projeto.Classes.Count > 0)
                    {
                        projeto.Classes.Add(new ClasseVO() { Nome = "AssemblyInfo" });
                        //projeto.Classes.Add(new ClasseVO() { Nome = "Base" });
                        stream.WriteLine("\t<ItemGroup>");
                        
                        foreach (ClasseVO classe1 in projeto.Classes)
                            stream.WriteLine(String.Format("\t\t<Compile Include=\"{0}{1}.cs\" />", classe1.Nome == "AssemblyInfo" ? @"Properties\" : String.Empty, String.Concat(classe1.Nome, classe1.Nome == "AssemblyInfo" ? String.Empty : "DAO")));

                        stream.WriteLine("\t</ItemGroup>");
                    }

                    //classe = projeto.Classes.FindLast(delegate(ClasseVO classe1) { return classe1.Nome == "Base"; });
                    //if (classe != null)
                    //    projeto.Classes.Remove(classe);

                    stream.WriteLine("\t<ItemGroup>");
                    stream.WriteLine(String.Format("\t\t<ProjectReference Include=\"..\\{0}\\{0}.csproj\">", Solucao.Projetos[0].Nome));
                    stream.WriteLine(String.Concat("\t\t\t<Project>{", Solucao.Projetos[0].Guid.ToString(), "}</Project>"));
                    stream.WriteLine(String.Format("\t\t\t<Name>{0}</Name>", Solucao.Projetos[0].Nome));
                    stream.WriteLine("\t\t</ProjectReference>");
                    //stream.WriteLine(String.Format("\t\t<ProjectReference Include=\"..\\{0}\\{0}.csproj\">", Solucao.Projetos[0].Nome));
                    //stream.WriteLine(String.Concat("\t\t\t<Project>{", Solucao.Projetos[0].Guid.ToString(), "}</Project>"));
                    //stream.WriteLine(String.Format("\t\t\t<Name>{0}</Name>", Solucao.Projetos[0].Nome));
                    //stream.WriteLine("\t\t</ProjectReference>");
                    stream.WriteLine("\t</ItemGroup>");

                    stream.WriteLine("\t<ItemGroup>");
                    stream.WriteLine("\t\t<Reference Include=\"Db.Persistence.DataAccess, Version=1.0.0.0, Culture=neutral, processorArchitecture=MSIL\" />");
                    stream.WriteLine("\t\t<Reference Include=\"System\" />");
                    stream.WriteLine("\t\t<Reference Include=\"System.Data\" />");
                    stream.WriteLine("\t\t<Reference Include=\"System.Xml\" />");
                    stream.WriteLine("\t\t<Reference Include=\"System.Configuration\" />");
                    
                    EscreveImportReference(stream);
                    stream.WriteLine("\t</ItemGroup>");
                    EscreveItemGroupCsProj(stream);

                    stream.WriteLine("\t<Import Project=\"$(MSBuildBinPath)\\Microsoft.CSharp.targets\" />");
                    stream.WriteLine("</Project>");

                    stream.Close();
                    #endregion
                }

                #endregion

                Solucao.Projetos.Add(projeto);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (stream != null)
                    stream.Dispose();
            }
        }

        /// <summary>
        /// Escreve o início da procedure com o comando de criação de procedure
        /// </summary>
        private void EscreveCreateProcedure(StreamWriter stream)
        {
            switch (DriverBanco)
            {
                case EDriverBanco.Oracle:
                    stream.Write("CREATE OR REPLACE ");
                    break;
                case EDriverBanco.SqlServer:
                    stream.Write("CREATE ");
                    break;
            }
        }

        /// <summary>
        /// Verifica se deve ser criado o script com parentes para parametro output em procedure sem parâmetro.
        /// </summary>
        /// <returns></returns>
        private Boolean VerificaPossuiParametroOutPut()
        {
            Boolean retorno = false;
            switch (DriverBanco)
            {
                case EDriverBanco.Oracle:
                    retorno = true;
                    break;
            }
            return retorno;
        }

        /// <summary>
        /// Escreve o separado do parâmetro com o tipo de dado
        /// </summary>
        private void EscreveSeparadorParametroTipo(StreamWriter stream)
        {
            switch (DriverBanco)
            {
                case EDriverBanco.Oracle:
                    stream.Write("\t\tIN ");
                    break;
                case EDriverBanco.SqlServer:
                    stream.Write("\t\t");
                    break;
            }
        }

        /// <summary>
        /// Escreve o separado do parâmetro com o tipo de dado
        /// </summary>
        private void EscreveTipoParametro(StreamWriter stream, ClasseVO classe, PropriedadeVO prop, Boolean isUltimo, Boolean busca)
        {
            switch (DriverBanco)
            {
                case EDriverBanco.Oracle:
                    stream.WriteLine(String.Format("{0}.{1}%TYPE{2}", classe.Tabela.Nome, prop.Coluna.Nome, !isUltimo || busca ? "," : String.Empty));
                    break;
                case EDriverBanco.SqlServer:
                    String tipoColuna = prop.Coluna.TipoColuna.ToString();
                    
                    if (prop.Coluna.Tamanho.HasValue)
                    {
                        if(!prop.Coluna.Scala.HasValue)
                            tipoColuna = String.Concat(tipoColuna, String.Format("({0})", prop.Coluna.Tamanho));
                        else
                            tipoColuna = String.Concat(tipoColuna, String.Format("({0},{1})", prop.Coluna.Tamanho.GetValueOrDefault(), prop.Coluna.Scala.GetValueOrDefault()));
                    }
                    if (prop.Nullable)
                    {
                        tipoColuna = String.Concat(tipoColuna, String.Format(" = NULL{0}", !isUltimo ? "," : String.Empty));
                    }
                    else
                    {
                        tipoColuna = String.Concat(tipoColuna, String.Format("{0}", !isUltimo ? "," : String.Empty));
                    }
                    stream.WriteLine(tipoColuna);
                    break;
            }
        }

        /// <summary>
        /// Escreve inicialização da procedure
        /// </summary>
        /// <param name="?"></param>
        private void EscreveIniciarProcedure(StreamWriter stream)
        {
            switch (DriverBanco)
            {
                case EDriverBanco.Oracle:
                    stream.Write("IS");
                    break;
                case EDriverBanco.SqlServer:
                    stream.Write("AS");
                    break;
            }
        }

        /// <summary>
        /// Escreve término da procedure
        /// </summary>
        /// <param name="?"></param>
        private void EscreveTerminoProcedure(StreamWriter stream, String procedure)
        {
            switch (DriverBanco)
            {
                case EDriverBanco.Oracle:
                    stream.Write(String.Format("END {0};", procedure));
                    break;
                case EDriverBanco.SqlServer:
                    stream.WriteLine("END");
                    stream.WriteLine("GO");
                    break;
            }
        }

        /// <summary>
        /// Escreve parâmetro (tabela) de saída da procedure
        /// </summary>
        /// <param name="?"></param>
        private void EscreveSaidaTabelaProcedure(StreamWriter stream)
        {
            switch (DriverBanco)
            {
                case EDriverBanco.Oracle:
                    stream.WriteLine("\tio_cursor\t\t\tOUT SYS_REFCURSOR");
                    break;
                case EDriverBanco.SqlServer:
                    break;
            }
        }

        /// <summary>
        /// Escreve início de select
        /// </summary>
        /// <param name="?"></param>
        private void EscreveInicioSelectProcedure(StreamWriter stream)
        {
            switch (DriverBanco)
            {
                case EDriverBanco.Oracle:
                    stream.WriteLine("\tOPEN io_cursor FOR");
                    break;
                case EDriverBanco.SqlServer:
                    break;
            }
        }

        /// <summary>
        /// Escreve referencia de pasta e dll no arquivo .csproj
        /// </summary>
        /// <param name="stream"></param>
        private void EscreveItemGroupCsProj(StreamWriter stream)
        {
            stream.WriteLine("\t<ItemGroup>");
            
            switch (DriverBanco)
            {
                case EDriverBanco.Oracle:
                    stream.WriteLine("\t\t<Content Include=\"dlls\\Oracle.DataAccess.dll\" />");
                    break;
                case EDriverBanco.SqlServer:
                    break;
            }
            stream.WriteLine("\t\t<Content Include=\"dlls\\Db.Persistence.DataAccess.dll\" />");
            stream.WriteLine("\t\t<Content Include=\"dlls\\Db.Persistence.Utils.dll\" />");
            stream.WriteLine("\t\t<Content Include=\"dlls\\Web.Controls.Components.dll\" />");
            stream.WriteLine("\t</ItemGroup>");
        }

        /// <summary>
        /// Escreve referencia de pasta e dll no arquivo .csproj
        /// </summary>
        /// <param name="stream"></param>
        private void EscreveItemGroupPastasWebCsProj(StreamWriter stream)
        {
            stream.WriteLine("\t<ItemGroup>");

            stream.WriteLine("\t\t<Content Include=\"App_Themes\\Padrao\\geral.css\" />");
            stream.WriteLine("\t\t<Content Include=\"App_Themes\\Padrao\\jquery-ui.css\" />");
            stream.WriteLine("\t\t<Content Include=\"App_Themes\\Padrao\\images\\animated-overlay.gif\" />");
            stream.WriteLine("\t\t<Content Include=\"App_Themes\\Padrao\\images\\ui-bg_flat_0_aaaaaa_40x100.png\" />");
            stream.WriteLine("\t\t<Content Include=\"App_Themes\\Padrao\\images\\ui-bg_flat_75_ffffff_40x100.png\" />");
            stream.WriteLine("\t\t<Content Include=\"App_Themes\\Padrao\\images\\ui-bg_glass_55_fbf9ee_1x400.png\" />");
            stream.WriteLine("\t\t<Content Include=\"App_Themes\\Padrao\\images\\ui-bg_glass_65_ffffff_1x400.png\" />");
            stream.WriteLine("\t\t<Content Include=\"App_Themes\\Padrao\\images\\ui-bg_glass_75_dadada_1x400.png\" />");
            stream.WriteLine("\t\t<Content Include=\"App_Themes\\Padrao\\images\\ui-bg_glass_75_e6e6e6_1x400.png\" />");
            stream.WriteLine("\t\t<Content Include=\"App_Themes\\Padrao\\images\\ui-bg_glass_95_fef1ec_1x400.png\" />");
            stream.WriteLine("\t\t<Content Include=\"App_Themes\\Padrao\\images\\ui-bg_highlight-soft_75_cccccc_1x100.png\" />");
            stream.WriteLine("\t\t<Content Include=\"App_Themes\\Padrao\\images\\ui-icons_222222_256x240.png\" />");
            stream.WriteLine("\t\t<Content Include=\"App_Themes\\Padrao\\images\\ui-icons_2e83ff_256x240.png\" />");
            stream.WriteLine("\t\t<Content Include=\"App_Themes\\Padrao\\images\\ui-icons_454545_256x240.png\" />");
            stream.WriteLine("\t\t<Content Include=\"App_Themes\\Padrao\\images\\ui-icons_888888_256x240.png\" />");
            stream.WriteLine("\t\t<Content Include=\"App_Themes\\Padrao\\images\\ui-icons_cd0a0a_256x240.png\" />");

            stream.WriteLine("\t\t<Content Include=\"script\\jquery-1.9.1.js\" />");
            stream.WriteLine("\t\t<Content Include=\"script\\jquery-ui.js\" />");
            
            stream.WriteLine("\t</ItemGroup>");
        }

        /// <summary>
        /// Escreve import de referencia no projeto
        /// </summary>
        /// <param name="stream"></param>
        private void EscreveImportReference(StreamWriter stream)
        {
            switch (DriverBanco)
            {
                case EDriverBanco.Oracle:
                    stream.WriteLine("\t\t<Reference Include=\"System.Data.OracleClient\" />");
                    stream.WriteLine("\t\t<Reference Include=\"Oracle.DataAccess\">");
                    stream.WriteLine("\t\t\t<SpecificVersion>False</SpecificVersion>");
                    stream.WriteLine("\t\t\t<HintPath>dlls\\Oracle.DataAccess.dll</HintPath>");
                    stream.WriteLine("\t\t</Reference>");
                    break;
                case EDriverBanco.SqlServer:
                    break;
            }
        }

        /// <summary>
        /// Escreve o DataAdapter de acordo com o driver especificado
        /// </summary>
        /// <param name="stream"></param>
        private void EscreveDataAdapter(StreamWriter stream)
        {
            switch (DriverBanco)
            {
                case EDriverBanco.Oracle:
                    stream.WriteLine("\t\t\t\tOracleDataAdapter da = new OracleDataAdapter();");
                    break;
                case EDriverBanco.SqlServer:
                    break;
            }
        }

        /// <summary>
        /// Escreve os parâmetros de saída, dependendo do driver especificado
        /// </summary>
        /// <param name="stream"></param>
        private void EscreveOutPutParameter(StreamWriter stream)
        {
            switch (DriverBanco)
            {
                case EDriverBanco.Oracle:
                    stream.WriteLine("\t\t\t\tcommand.Parameters.Add(new OracleParameter(\"io_cursor\", OracleDbType.RefCursor, ParameterDirection.Output));");
                    stream.WriteLine("");
                    break;
                case EDriverBanco.SqlServer:
                    break;
            }
        }

        /// <summary>
        /// Escreve a execução do comando já carregando no DataTable, dependendo do driver especificado
        /// </summary>
        /// <param name="stream"></param>
        private void EscreveExecuteReader(StreamWriter stream)
        {
            switch (DriverBanco)
            {
                case EDriverBanco.Oracle:
                    stream.WriteLine("\t\t\t\tda.SelectCommand = command;");
                    stream.WriteLine("\t\t\t\tda.Fill(tabela);");
                    break;
                case EDriverBanco.SqlServer:
                    stream.WriteLine("\t\t\t\ttabela.Load(command.ExecuteReader());");
                    break;
            }
        }

        /// <summary>
        /// Cria as dlls necessárias para acesso a dados no banco, de acordo com o driver especificado
        /// </summary>
        private void CriaDllsProjeto()
        {
            String pathDlls = String.Concat(txtPath.Text, String.Format(@"\{0}\AcessoDados\dlls", Solucao.Nome));
            Directory.CreateDirectory(pathDlls);

            string exeFile = (new System.Uri(Assembly.GetEntryAssembly().CodeBase)).AbsolutePath;
            string exeDir = Directory.GetParent(Path.GetDirectoryName(exeFile)).Parent.FullName.Replace("%20", " ");
            
            switch (DriverBanco)
            {
                case EDriverBanco.Oracle:
                    File.Copy(String.Format(@"{0}\dlls\Oracle.DataAccess.dll", exeDir), String.Concat(pathDlls, @"\Oracle.DataAccess.dll"));
                    break;
                case EDriverBanco.SqlServer: 
                    break;
            }
            File.Copy(String.Format(@"{0}\dlls\Db.Persistence.DataAccess.dll", exeDir), String.Concat(pathDlls, @"\Db.Persistence.DataAccess.dll"));
            File.Copy(String.Format(@"{0}\dlls\Db.Persistence.Utils.dll", exeDir), String.Concat(pathDlls, @"\Db.Persistence.Utils.dll"));
            File.Copy(String.Format(@"{0}\dlls\Web.Controls.Components.dll", exeDir), String.Concat(pathDlls, @"\Web.Controls.Components.dll"));
        }

        /// <summary>
        /// Cria arquivos bases do projeto web
        /// </summary>
        private void CopiaPastasProjetoWeb()
        {
            String pathAppWeb = String.Concat(txtPath.Text, String.Format(@"\{0}\AplicacaoWeb", Solucao.Nome));
            String pathAppThemeWeb = String.Concat(txtPath.Text, String.Format(@"\{0}\AplicacaoWeb\App_Themes", Solucao.Nome));
            Directory.CreateDirectory(pathAppThemeWeb);
            pathAppThemeWeb = String.Concat(txtPath.Text, String.Format(@"\{0}\AplicacaoWeb\App_Themes\Padrao", Solucao.Nome));
            Directory.CreateDirectory(pathAppThemeWeb);
            Directory.CreateDirectory(String.Format(@"{0}\script", pathAppWeb));

            string exeFile = (new System.Uri(Assembly.GetEntryAssembly().CodeBase)).AbsolutePath;
            string exeDir = Directory.GetParent(Path.GetDirectoryName(exeFile)).Parent.FullName.Replace("%20", " ");

            //File.Copy(String.Format(@"{0}\css\geral.css", exeDir), String.Concat(pathAppThemeWeb, @"\geral.css"));
            //File.Copy(String.Format(@"{0}\css\jquery-ui.css", exeDir), String.Concat(pathAppThemeWeb, @"\jquery-ui.css"));
            CopyFolder(String.Format(@"{0}\css", exeDir), pathAppThemeWeb);
            CopyFolder(String.Format(@"{0}\script", exeDir), String.Format(@"{0}\script",pathAppWeb));
        }

        /// <summary>
        /// Copia pasta para um outro diretório
        /// </summary>
        /// <param name="sourceFolder"></param>
        /// <param name="destFolder"></param>
        private void CopyFolder(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);
            string[] files = Directory.GetFiles(sourceFolder);
            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                string dest = Path.Combine(destFolder, name);
                File.Copy(file, dest);
            }
            string[] folders = Directory.GetDirectories(sourceFolder);
            foreach (string folder in folders)
            {
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(destFolder, name);
                CopyFolder(folder, dest);
            }
        }


        /// <summary>
        /// Escreve a referencia a classe de banco, de acordo com o driver especificado
        /// </summary>
        /// <param name="stream"></param>
        private void EscreveReferenciaBanco(StreamWriter stream)
        {
            switch (DriverBanco)
            {
                case EDriverBanco.Oracle:
                    stream.WriteLine("using Oracle.DataAccess.Client;");
                    break;
                case EDriverBanco.SqlServer:
                    stream.WriteLine("using System.Data.SqlClient;");
                    break;
            }
        }

        /// <summary>
        /// Cria projeto Web para que seja executado
        /// </summary>
        private void GerarProjetoWeb()
        {
            StreamWriter stream = null;
            try
            {
                ProjetoVO projeto = new ProjetoVO() { Nome = ETipoProjeto.AplicacaoWeb.ToString(), Tipo = ETipoProjeto.AplicacaoWeb, Namespace = "AplicacaoWeb" };
                String pathAplicacaoWeb = String.Concat(txtPath.Text, String.Format(@"\{0}\AplicacaoWeb", Solucao.Nome));
                Directory.CreateDirectory(@pathAplicacaoWeb);

                CopiaPastasProjetoWeb();
                
                List<ClasseVO> lstClasses = Solucao.Projetos[0].Classes.Where(x => !new String[] { "AssemblyInfo", "Base" }.Any(x1 => x1 == x.Nome)).ToList();

                #region     .....:::::     CRIA ARQUIVO DA MASTER PAGE      :::::.....

                #region     .....:::::     CRIA ARQUIVO .ASPX     :::::.....

                String pathMasterPage = String.Format(@"{0}\Master.Master", pathAplicacaoWeb);
                stream = new StreamWriter(pathMasterPage, true, Encoding.UTF8);
                stream.WriteLine(String.Format("<%@ Master Language=\"C#\" AutoEventWireup=\"true\" CodeBehind=\"Master.master.cs\" Inherits=\"{0}.{1}.Master\" %>", Solucao.Nome, projeto.Nome));
                stream.WriteLine("");
                stream.WriteLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
                stream.WriteLine("<html xmlns=\"http://www.w3.org/1999/xhtml\" >");
                stream.WriteLine("\t<head runat=\"server\">");
                stream.WriteLine("\t\t<script type=\"text/javascript\">");
                stream.WriteLine("\t\t</script>");
                stream.WriteLine("");
                stream.WriteLine("\t\t<asp:ContentPlaceHolder ID=\"cphHead\" runat=\"server\"></asp:ContentPlaceHolder>");
                stream.WriteLine("\t</head>");
                stream.WriteLine("\t<body>");
                stream.WriteLine("\t\t<form id=\"form1\" runat=\"server\">");
                stream.WriteLine("\t\t\t<div>");
                stream.WriteLine("\t\t\t\t<asp:ContentPlaceHolder ID=\"cphBody\" runat=\"server\"></asp:ContentPlaceHolder>");
                stream.WriteLine("\t\t\t</div>");
                stream.WriteLine("\t\t</form>");
                stream.WriteLine("\t</body>");
                stream.WriteLine("</html>");
                stream.Close();

                #endregion

                #region     .....:::::     CRIA ARQUIVO .ASPX.CS     :::::.....

                String pathMasterPageCs = String.Format(@"{0}\Master.Master.cs", pathAplicacaoWeb);
                stream = new StreamWriter(pathMasterPageCs, true, Encoding.UTF8);
                stream.WriteLine("using System;");
                stream.WriteLine("using System.Data;");
                stream.WriteLine("using System.Configuration;");
                stream.WriteLine("using System.Collections;");
                stream.WriteLine("using System.Web;");
                stream.WriteLine("using System.Web.Security;");
                stream.WriteLine("using System.Web.UI;");
                stream.WriteLine("using System.Web.UI.WebControls;");
                stream.WriteLine("using System.Web.UI.WebControls.WebParts;");
                stream.WriteLine("using System.Web.UI.HtmlControls;");
                stream.WriteLine("");
                stream.WriteLine(String.Format("namespace {0}.{1}", Solucao.Nome, projeto.Nome));
                stream.WriteLine("{");
                stream.WriteLine("\tpublic partial class Master : System.Web.UI.MasterPage");
                stream.WriteLine("\t{");
                stream.WriteLine("\t\tprotected void Page_Load(object sender, EventArgs e)");
                stream.WriteLine("\t\t{");
                stream.WriteLine("\t\t}");
                stream.WriteLine("\t}");
                stream.WriteLine("}");
                stream.Close();

                #endregion

                #region     .....:::::     CRIA ARQUIVO .ASPX.DESIGNER.CS     :::::.....

                String pathMasterPageDesignerCs = String.Format(@"{0}\Master.Master.designer.cs", pathAplicacaoWeb);
                stream = new StreamWriter(pathMasterPageDesignerCs, true, Encoding.UTF8);
                stream.WriteLine(String.Format("namespace {0}.{1}", Solucao.Nome, projeto.Nome));
                stream.WriteLine("{");
                stream.WriteLine("\tpublic partial class Master");
                stream.WriteLine("\t{");
                stream.WriteLine("\t\tprotected global::System.Web.UI.WebControls.ContentPlaceHolder cphHead;");
                stream.WriteLine("");
                stream.WriteLine("\t\tprotected global::System.Web.UI.HtmlControls.HtmlForm form1;");
                stream.WriteLine("");
                stream.WriteLine("\t\tprotected global::System.Web.UI.WebControls.ContentPlaceHolder cphBody;");
                stream.WriteLine("");
                stream.WriteLine("\t}");
                stream.WriteLine("}");
                stream.Close();

                #endregion

                #endregion



                #region     .....:::::     CRIA INTERFACES DO PROJETO     :::::.....

                if (chkGerarInterfaces.Checked)
                {
                    foreach (ClasseVO classe in lstClasses)
                    {
                        projeto.Interfaces.Add(new InterfaceVO(classe) { Titulo = String.Format("Gerenciar {0}", classe.Tabela.Nome) });
                    }

                    FrmGerarInterfaces form1 = new FrmGerarInterfaces(projeto.Interfaces);
                    form1.ShowDialog();

                    foreach (InterfaceVO tela in projeto.Interfaces.Where(x=> x.Gerar))
                    {
                        ClasseVO classe = tela.Classe; 

                        #region     .....:::::     CRIA GERENCIAS ARQUIVO ASPX     :::::.....

                        String nomeGerenciaAspx = String.Format("FrmGerenciar{0}.aspx", classe.Nome);
                        String pathGerenciaAspx = String.Format(@"{0}\{1}", pathAplicacaoWeb, nomeGerenciaAspx);
                        stream = new StreamWriter(pathGerenciaAspx, true, Encoding.UTF8);
                        stream.WriteLine(String.Format("<%@ Page Language=\"C#\" MasterPageFile=\"~/Master.Master\" AutoEventWireup=\"true\" CodeBehind=\"{0}.cs\" Theme=\"Padrao\" Inherits=\"{1}.{2}.{3}\" %>", nomeGerenciaAspx, Solucao.Nome, projeto.Nome, String.Format("FrmGerenciar{0}", classe.Nome)));
                        stream.WriteLine("<%@ Register Assembly=\"Web.Controls.Components\" Namespace=\"Web.Controls.Components\" TagPrefix=\"WC\" %>");
                        stream.WriteLine("");
                        stream.WriteLine("<asp:Content ID=\"Content1\" ContentPlaceHolderID=\"cphHead\" runat=\"server\"></asp:Content>");
                        stream.WriteLine("<asp:Content ID=\"Content2\" ContentPlaceHolderID=\"cphBody\" runat=\"server\">");
                        stream.WriteLine("\t<asp:MultiView ID=\"mtvConteudo\" runat=\"server\" ActiveViewIndex=\"0\">");
                        stream.WriteLine("\t\t<asp:View ID=\"mkvBusca\" runat=\"server\">");
                        stream.WriteLine("\t\t\t<div class=\"form\">");
                        stream.WriteLine("\t\t\t\t<div class=\"titleForm\">");
                        stream.WriteLine(String.Format("\t\t\t\t\t{0}", tela.Titulo));
                        stream.WriteLine("\t\t\t\t</div>");
                        stream.WriteLine("");
                        foreach (CampoVO campo in tela.CamposPesquisa.Where(x => x.Gerar))
                        {
                            String validationGroup = "vgBusca";
                            String retorno = campo.ToWebComponent(validationGroup, "Busca");
                            if (!String.IsNullOrEmpty(retorno))
                            {
                                stream.WriteLine(String.Format("\t\t\t\t{0}",retorno));
                            }
                        }
                        stream.WriteLine("\t\t\t\t<div class=\"divBotoes\">");

                        stream.WriteLine("\t\t\t\t\t<WC:WButton runat=\"server\" ID=\"lnkPesquisar\" OnClick=\"lnkPesquisar_Click\" IconPrimary=\"ui-icon-search\" Text=\"Pesquisar\">");
                        stream.WriteLine("\t\t\t\t\t\t<Listeners>");
                        stream.WriteLine("\t\t\t\t\t\t\t<WC:ButtonListener Fn=\"function(){ return ValidateForm('vgBusca'); }\" Type=\"click\" />");
                        stream.WriteLine("\t\t\t\t\t\t</Listeners>");
                        stream.WriteLine("\t\t\t\t\t</WC:WButton>");
                        stream.WriteLine("\t\t\t\t\t<WC:WButton runat=\"server\" ID=\"lnkLimpar\" OnClick=\"lnkLimpar_Click\" Text=\"Limpar\" IconPrimary=\"ui-icon-trash\"></WC:WButton>");
                        stream.WriteLine("\t\t\t\t\t<WC:WButton runat=\"server\" ID=\"lnkNovo\" OnClick=\"lnkNovo_Click\" Text=\"Novo\" IconPrimary=\"ui-icon-circle-plus\"></WC:WButton>");
                        stream.WriteLine("\t\t\t\t</div>");
                        stream.WriteLine("\t\t\t\t<asp:ValidationSummary ID=\"vsBusca\" runat=\"server\" ValidationGroup=\"vgBusca\" EnableClientScript=\"true\" DisplayMode=\"BulletList\" ShowMessageBox=\"false\" ShowSummary=\"false\" />");
                        stream.WriteLine("\t\t\t</div>");
                        stream.WriteLine("");

                        stream.WriteLine(String.Format("\t\t\t<WC:WGrid ID=\"grv{0}\" runat=\"server\" AutoGenerateColumns=\"False\" OnPageIndexChange=\"grd{0}_OnPageIndexChange\" ShowCaption=\"false\" Title=\"{0}\" AllowPaging=\"true\">", classe.Nome));
                        stream.WriteLine("\t\t\t\t<Columns>");

                        foreach (CampoVO campo in tela.CamposGrid.Where(x=> x.Gerar))
                        {
                            stream.WriteLine(String.Format("\t\t\t\t\t{0}", campo.ToColumnGridComponent()));
                        }
                        stream.WriteLine("\t\t\t\t</Columns>");
                        stream.WriteLine("\t\t\t\t<FooterStyle BorderWidth=\"0\" />");
                        stream.WriteLine("\t\t\t</WC:WGrid>");
                        stream.WriteLine("");

                        stream.WriteLine("\t\t</asp:View>");
                        
                        stream.WriteLine("\t\t<asp:View ID=\"mkvManter\" runat=\"server\">");
                        stream.WriteLine("\t\t\t<div class=\"form\">");
                        stream.WriteLine("\t\t\t\t<div class=\"titleForm\">");
                        stream.WriteLine("\t\t\t\t\tCadastro/Alteração");
                        stream.WriteLine("\t\t\t\t</div>");
                        foreach (CampoVO campo in tela.CamposForm.Where(x=> x.Gerar))
                        {
                            String validationGroup = "vgSalvar";
                            stream.WriteLine(String.Format("\t\t\t\t{0}", campo.ToWebComponent(validationGroup, String.Empty)));
                        }
                        stream.WriteLine("\t\t\t\t<div class=\"divBotoes\">");
                        stream.WriteLine("\t\t\t\t\t<WC:WButton runat=\"server\" ID=\"lnkSalvar\" OnClick=\"lnkSalvar_Click\" Text=\"Salvar\" IconPrimary=\"ui-icon-disk\">");
                        stream.WriteLine("\t\t\t\t\t\t<Listeners>");
                        stream.WriteLine("\t\t\t\t\t\t\t<WC:ButtonListener Fn=\"function(){ return ValidateForm('vgSalvar'); }\" Type=\"click\" />");
                        stream.WriteLine("\t\t\t\t\t\t</Listeners>");
                        stream.WriteLine("\t\t\t\t\t</WC:WButton>");
                        stream.WriteLine("\t\t\t\t\t<WC:WButton runat=\"server\" ID=\"lnkCancelar\" OnClick=\"lnkCancelar_Click\" Text=\"Cancelar\" IconPrimary=\"ui-icon-cancel\"></WC:WButton>");
                        stream.WriteLine("\t\t\t\t</div>");
                        stream.WriteLine("\t\t\t\t<asp:ValidationSummary ID=\"vsSalvar\" runat=\"server\" ValidationGroup=\"vgSalvar\" EnableClientScript=\"true\" DisplayMode=\"BulletList\" ShowMessageBox=\"false\" ShowSummary=\"false\" />");
                        stream.WriteLine("\t\t\t</div>");
                        stream.WriteLine("\t\t</asp:View>");

                        stream.WriteLine("\t</asp:MultiView>");
                        stream.WriteLine("</asp:Content>");
                        
                        stream.Close();

                        #endregion

                        #region     .....:::::     CRIA GERENCIAS ARQUIVO ASPX.CS     :::::.....

                        String nomeGerenciaAspxCs = String.Format("FrmGerenciar{0}.aspx.cs", classe.Nome);
                        String pathGerenciaAspxCs = String.Format(@"{0}\{1}", pathAplicacaoWeb, nomeGerenciaAspxCs);
                        stream = new StreamWriter(pathGerenciaAspxCs, true);
                        stream.WriteLine("using System;");
                        stream.WriteLine("using System.Collections.Generic;");
                        stream.WriteLine("using System.Web.UI;");
                        stream.WriteLine("using System.Web.UI.WebControls;");
                        stream.WriteLine("using Web.Controls.Components;");
                        stream.WriteLine("using Db.Persistence.Utils;");
                        stream.WriteLine(String.Format("using {0}.{1};", Solucao.Nome, ETipoProjeto.Negocios.ToString()));
                        stream.WriteLine(String.Format("using {0}.{1};", Solucao.Nome,ETipoProjeto.Entidades.ToString()));
                        
                        stream.WriteLine("");
                        stream.WriteLine(String.Format("namespace {0}.{1}", Solucao.Nome, projeto.Nome));
                        stream.WriteLine("{");
                        stream.WriteLine(String.Format("\tpublic partial class FrmGerenciar{0} : BasePage", classe.Nome));
                        stream.WriteLine("\t{");
                        stream.WriteLine("");

                        #region     .....:::::     EVENTOS     :::::.....

                        stream.WriteLine("\t\t#region     .....:::::     EVENTOS     :::::.....");
                        
                        stream.WriteLine("");
                        stream.WriteLine("\t\tprotected void Page_Load(object sender, EventArgs e)");
                        stream.WriteLine("\t\t{");
                        stream.WriteLine("\t\t\tif(!IsPostBack)");
                        stream.WriteLine("\t\t\t{");
                        stream.WriteLine("\t\t\t\tPesquisar(1);");
                        
                        foreach (CampoVO campo in tela.CamposForm.Where(x => x.Propriedade.Coluna.ChaveEstrangeira != null))
                        {
                            stream.WriteLine(String.Format("\t\t\t\tCarregar{0}();", campo.Propriedade.Nome));
                        }
                        foreach (CampoVO campo in tela.CamposPesquisa.Where(x => x.Propriedade.Coluna.ChaveEstrangeira != null))
                        {
                            stream.WriteLine(String.Format("\t\t\t\tCarregar{0}Busca();", campo.Propriedade.Nome));
                        }
                        
                        stream.WriteLine("\t\t\t}");
                        stream.WriteLine("\t\t}");
                        
                        stream.WriteLine("");
                        stream.WriteLine("\t\t/// <summary>");
                        stream.WriteLine("\t\t/// evento disparado ao clicar no botão Limpar");
                        stream.WriteLine("\t\t/// </summary>");
                        stream.WriteLine("\t\t/// <param name=\"sender\"></param>");
                        stream.WriteLine("\t\t/// <param name=\"e\"></param>");
                        stream.WriteLine("\t\tprotected void lnkLimpar_Click(object sender, EventArgs e)");
                        stream.WriteLine("\t\t{");
                        stream.WriteLine("\t\t\tLimparCamposBusca();");
                        stream.WriteLine("\t\t}");

                        stream.WriteLine("");
                        stream.WriteLine("\t\t/// <summary>");
                        stream.WriteLine("\t\t/// evento disparado ao clicar no botão Pesquisar");
                        stream.WriteLine("\t\t/// </summary>");
                        stream.WriteLine("\t\t/// <param name=\"sender\"></param>");
                        stream.WriteLine("\t\t/// <param name=\"e\"></param>");
                        stream.WriteLine("\t\tprotected void lnkPesquisar_Click(object sender, EventArgs e)");
                        stream.WriteLine("\t\t{");
                        stream.WriteLine("\t\t\tPesquisar(1);");
                        stream.WriteLine("\t\t}");

                        stream.WriteLine("");
                        stream.WriteLine("\t\t/// <summary>");
                        stream.WriteLine("\t\t/// evento disparado ao clicar na paginação do grid");
                        stream.WriteLine("\t\t/// </summary>");
                        stream.WriteLine("\t\t/// <param name=\"sender\"></param>");
                        stream.WriteLine("\t\t/// <param name=\"e\"></param>");
                        stream.WriteLine(String.Format("\t\tprotected void grd{0}_OnPageIndexChange(Int32 PageIndex)", classe.Nome));
                        stream.WriteLine("\t\t{");
                        stream.WriteLine("\t\t\tPesquisar(PageIndex);");
                        stream.WriteLine("\t\t}");

                        stream.WriteLine("");
                        stream.WriteLine("\t\t/// <summary>");
                        stream.WriteLine("\t\t/// evento disparado ao clicar no botão editar do grid");
                        stream.WriteLine("\t\t/// </summary>");
                        stream.WriteLine("\t\t/// <param name=\"sender\"></param>");
                        stream.WriteLine("\t\t/// <param name=\"e\"></param>");
                        stream.WriteLine("\t\tprotected void lnkEditar_Click(object sender, EventArgs e)");
                        stream.WriteLine("\t\t{");
                        stream.WriteLine("\t\t\tLimparCamposForm();");
                        stream.WriteLine("\t\t\tPreencherCampos((sender as WButton).CommandArgument);");
                        stream.WriteLine("\t\t}");

                        stream.WriteLine("");
                        stream.WriteLine("\t\t/// <summary>");
                        stream.WriteLine("\t\t/// evento disparado ao clicar no botão novo");
                        stream.WriteLine("\t\t/// </summary>");
                        stream.WriteLine("\t\t/// <param name=\"sender\"></param>");
                        stream.WriteLine("\t\t/// <param name=\"e\"></param>");
                        stream.WriteLine("\t\tprotected void lnkNovo_Click(object sender, EventArgs e)");
                        stream.WriteLine("\t\t{");
                        stream.WriteLine("\t\t\tLimparCamposForm();");
                        stream.WriteLine("\t\t\tthis.mtvConteudo.ActiveViewIndex = 1;");
                        stream.WriteLine("\t\t}");

                        stream.WriteLine("");
                        stream.WriteLine("\t\t/// <summary>");
                        stream.WriteLine("\t\t/// evento disparado ao clicar no botão salvar");
                        stream.WriteLine("\t\t/// </summary>");
                        stream.WriteLine("\t\t/// <param name=\"sender\"></param>");
                        stream.WriteLine("\t\t/// <param name=\"e\"></param>");
                        stream.WriteLine("\t\tprotected void lnkSalvar_Click(object sender, EventArgs e)");
                        stream.WriteLine("\t\t{");
                        stream.WriteLine("\t\t\tSalvar();");
                        stream.WriteLine("\t\t}");

                        stream.WriteLine("");
                        stream.WriteLine("\t\t/// <summary>");
                        stream.WriteLine("\t\t/// evento que cancela uma edição ou cadastro");
                        stream.WriteLine("\t\t/// </summary>");
                        stream.WriteLine("\t\t/// <param name=\"sender\"></param>");
                        stream.WriteLine("\t\t/// <param name=\"e\"></param>");
                        stream.WriteLine("\t\tprotected void lnkCancelar_Click(object sender, EventArgs e)");
                        stream.WriteLine("\t\t{");
                        stream.WriteLine("\t\t\tthis.mtvConteudo.ActiveViewIndex = 0;");
                        stream.WriteLine("\t\t}");
                        
                        stream.WriteLine("");
                        stream.WriteLine("\t\t#endregion");

                        #endregion

                        #region     .....:::::     MÉTODOS     :::::.....

                        stream.WriteLine("");
                        stream.WriteLine("\t\t#region     .....:::::     MÉTODOS     :::::.....");

                        stream.WriteLine("");
                        stream.WriteLine("\t\t/// <summary>");
                        stream.WriteLine("\t\t/// metodo chamado para limpar os campos do form");
                        stream.WriteLine("\t\t/// </summary>");
                        stream.WriteLine("\t\tprivate void LimparCamposBusca()");
                        stream.WriteLine("\t\t{");
                        foreach (CampoVO campo in tela.CamposPesquisa.Where(x=> x.Gerar))
                        {
                            String nomePanel = "Busca";
                            stream.WriteLine(String.Format("\t\t\t{0}", campo.ToTextLimparCampo(nomePanel)));
                        }
                        stream.WriteLine("\t\t}");

                        stream.WriteLine("");
                        stream.WriteLine("\t\t/// <summary>");
                        stream.WriteLine("\t\t/// metodo chamado para pesquisar informações");
                        stream.WriteLine("\t\t/// </summary>");
                        stream.WriteLine("\t\tprivate void Pesquisar(Int32 index)");
                        stream.WriteLine("\t\t{");

                        stream.WriteLine(String.Format("\t\t\tPaginacao<{0}VO> paginacao = new {0}BO().BuscarComPaginacao(index, base.PageSize, new String[] {1});", classe.Nome, String.Concat("{", String.Join(",", tela.CamposGrid.Select(x => "\""+x.Propriedade.Nome + "\"").ToArray()), "}")));
                        stream.WriteLine(String.Format("\t\t\tgrv{0}.DataSource = paginacao.LstObjetos;", classe.Nome));
                        stream.WriteLine(String.Format("\t\t\tgrv{0}.DataBind();", classe.Nome));
                        stream.WriteLine(String.Format("\t\t\tgrv{0}.LoadPagination(paginacao.TotalRegistros / base.PageSize, index, paginacao.LstObjetos.Count);", classe.Nome));
                        stream.WriteLine("\t\t}");

                        stream.WriteLine("");
                        stream.WriteLine("\t\t/// <summary>");
                        stream.WriteLine("\t\t/// Carrega as informações nos campos quando esta sendo alterado");
                        stream.WriteLine("\t\t/// </summary>");
                        stream.WriteLine("\t\tprivate void PreencherCampos(String id)");
                        stream.WriteLine("\t\t{");
                        stream.WriteLine("\t\t\tthis.mtvConteudo.ActiveViewIndex = 1;");
                        stream.WriteLine("\t\t}");

                        stream.WriteLine("");
                        stream.WriteLine("\t\t/// <summary>");
                        stream.WriteLine("\t\t/// metodo chamado para salvar um centro d ecusto");
                        stream.WriteLine("\t\t/// </summary>");
                        stream.WriteLine("\t\tprivate void Salvar()");
                        stream.WriteLine("\t\t{");
                        stream.WriteLine("\t\t\tthis.mtvConteudo.ActiveViewIndex = 0;");
                        stream.WriteLine("\t\t}");

                        stream.WriteLine("");
                        stream.WriteLine("\t\t/// <summary>");
                        stream.WriteLine("\t\t/// limpa os campos do formulario");
                        stream.WriteLine("\t\t/// </summary>");
                        stream.WriteLine("\t\tprivate void LimparCamposForm()");
                        stream.WriteLine("\t\t{");

                        foreach (CampoVO campo in tela.CamposForm.Where(x => x.Gerar))
                        {
                            stream.WriteLine(String.Format("\t\t\t{0}", campo.ToTextLimparCampo("")));
                        }

                        stream.WriteLine("\t\t}");

                        stream.WriteLine("");
                        foreach (CampoVO campo in tela.CamposForm.Where(x => x.Propriedade.Coluna.ChaveEstrangeira != null))
                        {
                            if (String.IsNullOrEmpty(campo.DisplayField) && String.IsNullOrEmpty(campo.ValueField))
                                continue;

                            stream.WriteLine("\t\t/// <summary>");
                            stream.WriteLine("\t\t/// carrega {0}", campo.Propriedade.Nome);
                            stream.WriteLine("\t\t/// </summary>");
                            stream.WriteLine(String.Format("\t\tprivate void Carregar{0}()", campo.Propriedade.Nome));
                            stream.WriteLine("\t\t{");
                            
                            List<String> lstSelect = new List<String>();
                            if (!String.IsNullOrEmpty(campo.DisplayField))
                                lstSelect.Add(campo.DisplayField);
                            if (!String.IsNullOrEmpty(campo.ValueField))
                                lstSelect.Add(campo.ValueField);

                            stream.WriteLine(String.Format("\t\t\tList<{0}VO> lst = new {0}BO().Buscar({1});", tela.Classe.Nome, String.Join(",", lstSelect.ToArray())));
                            stream.WriteLine(String.Format("\t\t\t{0}{1}{2}.DataSource = lst;",campo.TipoComponent.ToTextPrefixoWebComponent(), campo.Propriedade.Nome, String.Empty));
                            stream.WriteLine(String.Format("\t\t\t{0}{1}{2}.DataBind();", campo.TipoComponent.ToTextPrefixoWebComponent(), campo.Propriedade.Nome, String.Empty));
                            stream.WriteLine("\t\t}");
                        }
                        foreach (CampoVO campo in tela.CamposPesquisa.Where(x => x.Propriedade.Coluna.ChaveEstrangeira != null))
                        {
                            stream.WriteLine("\t\t/// <summary>");
                            stream.WriteLine("\t\t/// carrega {0}", campo.Propriedade.Nome);
                            stream.WriteLine("\t\t/// </summary>");
                            stream.WriteLine(String.Format("\t\tprivate void Carregar{0}Busca()", campo.Propriedade.Nome));
                            stream.WriteLine("\t\t{");

                            List<String> lstSelect = new List<String>();
                            if (!String.IsNullOrEmpty(campo.DisplayField))
                                lstSelect.Add(campo.DisplayField);
                            if (!String.IsNullOrEmpty(campo.ValueField))
                                lstSelect.Add(campo.ValueField);

                            String nomePanel = "Busca";
                            stream.WriteLine(String.Format("\t\t\tList<{0}VO> lst = new {0}BO().Buscar({1});", tela.Classe.Nome, String.Join(",", lstSelect.ToArray())));
                            stream.WriteLine(String.Format("\t\t\t{0}{1}{2}.DataSource = lst;", campo.TipoComponent.ToTextPrefixoWebComponent(), campo.Propriedade.Nome, nomePanel));
                            stream.WriteLine(String.Format("\t\t\t{0}{1}{2}.DataBind();", campo.TipoComponent.ToTextPrefixoWebComponent(), campo.Propriedade.Nome, nomePanel));
                            stream.WriteLine("\t\t}");
                        }

                        stream.WriteLine("");
                        stream.WriteLine("\t\t#endregion");

                        #endregion

                        stream.WriteLine("");
                        stream.WriteLine("\t}");
                        stream.WriteLine("}");
                        stream.Close();

                        #endregion

                        #region     .....:::::     CRIA GERENCIAS ARQUIVO ASPX.DESIGNER.CS     :::::.....

                        String nomeGerenciaAspxDesignerCs = String.Format("FrmGerenciar{0}.aspx.designer.cs", classe.Nome);
                        String pathGerenciaAspxDesignerCs = String.Format(@"{0}\{1}", pathAplicacaoWeb, nomeGerenciaAspxDesignerCs);
                        stream = new StreamWriter(pathGerenciaAspxDesignerCs, true);
                        stream.WriteLine(String.Format("namespace {0}.{1}", Solucao.Nome, projeto.Nome));
                        stream.WriteLine("{");
                        stream.WriteLine("");
                        stream.WriteLine(String.Format("\tpublic partial class FrmGerenciar{0}", classe.Nome));
                        stream.WriteLine("\t{");
                        stream.WriteLine("");
                        stream.WriteLine("\t\tprotected global::System.Web.UI.WebControls.MultiView mtvConteudo;");
                        stream.WriteLine("");
                        stream.WriteLine("\t\tprotected global::System.Web.UI.WebControls.View mkvBusca;");
                        stream.WriteLine("");
                        stream.WriteLine("\t\tprotected global::Web.Controls.Components.WButton lnkPesquisar;");
                        stream.WriteLine("");
                        stream.WriteLine("\t\tprotected global::Web.Controls.Components.WButton lnkLimpar;");
                        stream.WriteLine("");
                        stream.WriteLine("\t\tprotected global::Web.Controls.Components.WButton lnkNovo;");
                        stream.WriteLine("");
                        stream.WriteLine(String.Format("\t\tprotected global::Web.Controls.Components.WGrid grv{0};", classe.Nome));
                        stream.WriteLine("");
                        stream.WriteLine("\t\tprotected global::System.Web.UI.WebControls.View mkvManter;");
                        stream.WriteLine("");
                        stream.WriteLine("\t\tprotected global::Web.Controls.Components.WButton lnkSalvar;");
                        stream.WriteLine("");
                        stream.WriteLine("\t\tprotected global::Web.Controls.Components.WButton lnkCancelar;");
                        stream.WriteLine("");
                        stream.WriteLine("\t\tprotected global::System.Web.UI.WebControls.ValidationSummary vsBusca;");
                        stream.WriteLine("");
                        stream.WriteLine("\t\tprotected global::System.Web.UI.WebControls.ValidationSummary vsSalvar;");

                        foreach (CampoVO campo in tela.CamposPesquisa.Where(x=> x.Gerar))
                        {
                            stream.WriteLine("");
                            stream.WriteLine(String.Format("\t\t{0}", campo.ToTextDesignerWebComponent("Busca")));
                        }

                        foreach (CampoVO campo in tela.CamposForm.Where(x => x.Gerar))
                        {
                            stream.WriteLine("");
                            stream.WriteLine(String.Format("\t\t{0}", campo.ToTextDesignerWebComponent("")));
                        }

                        stream.WriteLine("\t}");
                        stream.WriteLine("}");
                        stream.Close();


                        #endregion
                    }
                }

                #endregion

                #region     .....:::::     CRIA ARQUIVO APP.CONFIG     :::::.....

                String nomeAppConfig = "Web.config";
                String pathAppConfig = String.Format(@"{0}\{1}", pathAplicacaoWeb, nomeAppConfig);
                stream = new StreamWriter(pathAppConfig, true);
                stream.WriteLine("<?xml version=\"1.0\"?>");
                stream.WriteLine("<configuration>");
                stream.WriteLine("\t<connectionStrings>");
                stream.WriteLine(String.Format("\t\t<add name=\"ConnectionString\" connectionString=\"{0}\" />", this.GetConnectionString()));
                stream.WriteLine("\t</connectionStrings>");
                stream.WriteLine("\t<appSettings>");
                stream.WriteLine("\t\t<add key=\"ShowSql\" value=\"true\" />");
                stream.WriteLine("\t\t<add key=\"PageSize\" value=\"10\" />");
                stream.WriteLine("\t</appSettings>");
                stream.WriteLine("</configuration>");
                stream.Close();

                #endregion
                
                #region     .....:::::     CRIA ARQUIVO ASSEMBLYINFO     :::::.....

                String pathProperties = String.Concat(pathAplicacaoWeb, @"\Properties");
                Directory.CreateDirectory(pathProperties);
                stream = new StreamWriter(String.Concat(pathProperties, @"\AssemblyInfo.cs"), true);

                stream.WriteLine("using System.Reflection;");
                stream.WriteLine("using System.Runtime.CompilerServices;");
                stream.WriteLine("using System.Runtime.InteropServices;");
                stream.WriteLine("");
                stream.WriteLine("");
                stream.WriteLine("");
                stream.WriteLine(String.Format("[assembly: AssemblyTitle(\"{0}\")]", projeto.Nome));
                stream.WriteLine("[assembly: AssemblyDescription(\"\")]");
                stream.WriteLine("[assembly: AssemblyConfiguration(\"\")]");
                stream.WriteLine("[assembly: AssemblyCompany(\"P.H.F. Dias Software\")]");
                stream.WriteLine(String.Format("[assembly: AssemblyProduct(\"{0}\")]", projeto.Nome));
                stream.WriteLine("[assembly: AssemblyCopyright(\"Copyright © P.H.F. Dias Software 2012\")]");
                stream.WriteLine("[assembly: AssemblyTrademark(\"\")]");
                stream.WriteLine("[assembly: AssemblyCulture(\"\")]");
                stream.WriteLine("");
                stream.WriteLine("[assembly: ComVisible(false)]");
                stream.WriteLine("");
                stream.WriteLine(String.Format("[assembly: Guid(\"{0}\")]", projeto.Guid.ToString()));
                stream.WriteLine("");
                stream.WriteLine("");
                stream.WriteLine("[assembly: AssemblyVersion(\"1.0\")]");
                stream.WriteLine("[assembly: AssemblyFileVersion(\"1.0\")]");

                stream.Close();

                #endregion

                #region     .....:::::     CRIA ARQUIVO GLOBAL.ASAX     :::::.....

                String nomeGlobal = "Global.asax";
                String nomeGlobalCs = "Global.asax.cs";
                String pathGlobal = String.Format(@"{0}\{1}", pathAplicacaoWeb, nomeGlobal);
                stream = new StreamWriter(pathGlobal, true);
                stream.WriteLine(String.Format("<%@ Application Codebehind=\"{0}\" Inherits=\"{1}.{2}.Global\" Language=\"C#\" %>", nomeGlobalCs, Solucao.Nome, ETipoProjeto.AplicacaoWeb.ToString()));
                stream.Close();


                String pathGlobalCs = String.Format(@"{0}\{1}", pathAplicacaoWeb, nomeGlobalCs);
                stream = new StreamWriter(pathGlobalCs, true);
                stream.WriteLine("using System;");
                stream.WriteLine("using System.Data;");
                stream.WriteLine("using System.Configuration;");
                stream.WriteLine("using System.Collections;");
                stream.WriteLine("using System.Web;");
                stream.WriteLine("using System.Web.Security;");
                stream.WriteLine("using System.Web.SessionState;");
                stream.WriteLine("using Db.Persistence.Utils;");
                stream.WriteLine("using System.Reflection;");
                stream.WriteLine("using System.Collections.Generic;");
                stream.WriteLine("");
                stream.WriteLine(String.Format("namespace {0}.{1}", Solucao.Nome, ETipoProjeto.AplicacaoWeb.ToString() ));
                stream.WriteLine("{");
                stream.WriteLine("\tpublic class Global : System.Web.HttpApplication");
                stream.WriteLine("\t{");
                stream.WriteLine("\t\tprotected void Application_Start(object sender, EventArgs e)");
                stream.WriteLine("\t\t{");
                stream.WriteLine("\t\t\tforeach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())");
                stream.WriteLine("\t\t\t{");
                stream.WriteLine("\t\t\t\tif (ass.GetName().Name.IndexOf(\"Entidades\") >= 0)");
                stream.WriteLine("\t\t\t\t{");
                stream.WriteLine("\t\t\t\t\tUtils.entidadeAssembly = ass;");
                stream.WriteLine("\t\t\t\t\tUtils.namespaceEntidades = ass.GetName().Name;");
                stream.WriteLine("\t\t\t\t\tbreak;");
                stream.WriteLine("\t\t\t\t}");
                stream.WriteLine("\t\t\t}");
                stream.WriteLine("\t\t}");
                stream.WriteLine("");
                stream.WriteLine("\t\tprotected void Application_End(object sender, EventArgs e)");
                stream.WriteLine("\t\t{");
                stream.WriteLine("\t\t}");
                stream.WriteLine("\t}");
                stream.WriteLine("}");
                stream.Close();

                #endregion

                #region     .....:::::     CRIA ARQUIVO DE PROJETO     :::::.....

                String nomeProjeto = String.Format("{0}.csproj", ETipoProjeto.AplicacaoWeb.ToString());
                String pathProjeto = String.Format(@"{0}\{1}", pathAplicacaoWeb, nomeProjeto);

                stream = new StreamWriter(pathProjeto, true);
                stream.WriteLine("<Project DefaultTargets=\"Build\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">");

                stream.WriteLine("\t<PropertyGroup>");
                stream.WriteLine("\t\t<SchemaVersion>2.0</SchemaVersion>");
                stream.WriteLine(String.Format("\t\t<ProjectGuid>{0}</ProjectGuid>", String.Concat("{", projeto.Guid.ToString(), "}")));
                stream.WriteLine("\t\t<ProjectTypeGuids>{349c5851-65df-11da-9384-00065b846f21};{fae04ec0-301f-11d3-bf4b-00c04f79efbc}</ProjectTypeGuids>");
                stream.WriteLine("\t\t<OutputType>Library</OutputType>");
                stream.WriteLine("\t\t<AppDesignerFolder>Properties</AppDesignerFolder>");
                stream.WriteLine(String.Format("\t\t<RootNamespace>{0}{1}</RootNamespace>", String.IsNullOrEmpty(Solucao.Nome) ? String.Empty : String.Concat(Solucao.Nome, "."), projeto.Namespace));
                stream.WriteLine(String.Format("\t\t<AssemblyName>{0}{1}</AssemblyName>", String.IsNullOrEmpty(Solucao.Nome) ? String.Empty : String.Concat(Solucao.Nome, "."), projeto.Namespace));
                stream.WriteLine("\t\t<SccProjectName></SccProjectName>");
                stream.WriteLine("\t\t<SccLocalPath></SccLocalPath>");
                stream.WriteLine("\t\t<SccAuxPath></SccAuxPath>");
                stream.WriteLine("\t\t<SccProvider></SccProvider>");
                stream.WriteLine("\t</PropertyGroup>");

                stream.WriteLine("\t<PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' \">");
                stream.WriteLine("\t\t<DebugSymbols>true</DebugSymbols>");
                stream.WriteLine("\t\t<DebugType>full</DebugType>");
                stream.WriteLine("\t\t<Optimize>false</Optimize>");
                stream.WriteLine("\t\t<OutputPath>bin\\</OutputPath>");
                stream.WriteLine("\t\t<DefineConstants>DEBUG;TRACE</DefineConstants>");
                stream.WriteLine("\t\t<ErrorReport>prompt</ErrorReport>");
                stream.WriteLine("\t\t<WarningLevel>4</WarningLevel>");
                stream.WriteLine("\t</PropertyGroup>");

                stream.WriteLine("\t<PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' \">");
                stream.WriteLine("\t\t<DebugType>pdbonly</DebugType>");
                stream.WriteLine("\t\t<Optimize>true</Optimize>");
                stream.WriteLine("\t\t<OutputPath>bin\\</OutputPath>");
                stream.WriteLine("\t\t<DefineConstants>TRACE</DefineConstants>");
                stream.WriteLine("\t\t<ErrorReport>prompt</ErrorReport>");
                stream.WriteLine("\t\t<WarningLevel>4</WarningLevel>");
                stream.WriteLine("\t</PropertyGroup>");

                projeto.Classes.Add(new ClasseVO() { Nome = "Properties\\AssemblyInfo.cs" });
                stream.WriteLine("\t<ItemGroup>");

                foreach (ClasseVO classe in projeto.Classes)
                    stream.WriteLine(String.Format("\t\t<Compile Include=\"{0}\" />", classe.Nome));

                stream.WriteLine("\t</ItemGroup>");
                stream.WriteLine("\t<ItemGroup>");
                stream.WriteLine("\t\t<Content Include=\"Web.config\" />");
                stream.WriteLine("\t\t<Content Include=\"Global.asax\" />");
                stream.WriteLine("\t\t<Content Include=\"Master.Master\" />");

                foreach (ClasseVO classe in projeto.Interfaces.Where(x => x.Gerar).Select(x => x.Classe))
                {
                    stream.WriteLine(String.Format("\t\t<Content Include=\"FrmGerenciar{0}.aspx\" />", classe.Nome));
                }
                
                stream.WriteLine("\t</ItemGroup>");
                
                stream.WriteLine("\t<ItemGroup>");
                stream.WriteLine("\t\t<Compile Include=\"Global.asax.cs\">");
                stream.WriteLine("\t\t\t<DependentUpon>Global.asax</DependentUpon>");
                stream.WriteLine("\t\t</Compile>");
                
                stream.WriteLine("\t\t<Compile Include=\"Master.Master.cs\">");
                stream.WriteLine("\t\t\t<DependentUpon>Master.Master</DependentUpon>");
                stream.WriteLine("\t\t\t<SubType>ASPXCodeBehind</SubType>");
                stream.WriteLine("\t\t</Compile>");
                stream.WriteLine("\t\t<Compile Include=\"Master.Master.designer.cs\">");
                stream.WriteLine("\t\t\t<DependentUpon>Master.Master</DependentUpon>");
                stream.WriteLine("\t\t</Compile>");

                foreach (ClasseVO classe in projeto.Interfaces.Where(x => x.Gerar).Select(x=> x.Classe))
                {
                    stream.WriteLine(String.Format("\t\t<Compile Include=\"FrmGerenciar{0}.aspx.cs\">", classe.Nome));
                    stream.WriteLine(String.Format("\t\t\t<DependentUpon>FrmGerenciar{0}.aspx</DependentUpon>", classe.Nome));
                    stream.WriteLine("\t\t\t<SubType>ASPXCodeBehind</SubType>");
                    stream.WriteLine("\t\t</Compile>");

                    stream.WriteLine(String.Format("\t\t<Compile Include=\"FrmGerenciar{0}.aspx.designer.cs\">", classe.Nome));
                    stream.WriteLine(String.Format("\t\t\t<DependentUpon>FrmGerenciar{0}.aspx</DependentUpon>", classe.Nome));
                    stream.WriteLine("\t\t</Compile>");
                }
                
                stream.WriteLine("\t</ItemGroup>");
                
                stream.WriteLine("\t<ItemGroup>");
                stream.WriteLine(String.Format("\t\t<ProjectReference Include=\"..\\{0}\\{0}.csproj\">", Solucao.Projetos[0].Nome));
                stream.WriteLine(String.Concat("\t\t\t<Project>{", Solucao.Projetos[0].Guid.ToString(), "}</Project>"));
                stream.WriteLine(String.Format("\t\t\t<Name>{0}</Name>", Solucao.Projetos[0].Nome));
                stream.WriteLine("\t\t</ProjectReference>");
                stream.WriteLine(String.Format("\t\t<ProjectReference Include=\"..\\{0}\\{0}.csproj\">", Solucao.Projetos[2].Nome));
                stream.WriteLine(String.Concat("\t\t\t<Project>{", Solucao.Projetos[2].Guid.ToString(), "}</Project>"));
                stream.WriteLine(String.Format("\t\t\t<Name>{0}</Name>", Solucao.Projetos[2].Nome));
                stream.WriteLine("\t\t</ProjectReference>");
                stream.WriteLine("\t</ItemGroup>");

                stream.WriteLine("\t<ItemGroup>");
                stream.WriteLine("\t\t<Reference Include=\"Db.Persistence.Utils, Version=1.0.0.0, Culture=neutral, processorArchitecture=MSIL\">");
                stream.WriteLine("\t\t\t<SpecificVersion>False</SpecificVersion>");
                stream.WriteLine("\t\t\t<HintPath>..\\AcessoDados\\dlls\\Db.Persistence.Utils.dll</HintPath>");
                stream.WriteLine("\t\t</Reference>");
                stream.WriteLine("\t\t<Reference Include=\"Web.Controls.Components, Version=1.0.0.0, Culture=neutral, processorArchitecture=MSIL\">");
                stream.WriteLine("\t\t\t<SpecificVersion>False</SpecificVersion>");
                stream.WriteLine("\t\t\t<HintPath>..\\AcessoDados\\dlls\\Web.Controls.Components.dll</HintPath>");
                stream.WriteLine("\t\t</Reference>");
                stream.WriteLine("\t\t<Reference Include=\"System\" />");
                stream.WriteLine("\t\t<Reference Include=\"System.Data\" />");
                stream.WriteLine("\t\t<Reference Include=\"System.Drawing\" />");
                stream.WriteLine("\t\t<Reference Include=\"System.Web\" />");
                stream.WriteLine("\t\t<Reference Include=\"System.Xml\" />");
                stream.WriteLine("\t\t<Reference Include=\"System.Configuration\" />");
                stream.WriteLine("\t</ItemGroup>");

                EscreveItemGroupPastasWebCsProj(stream);

                stream.WriteLine("\t<Import Project=\"$(MSBuildBinPath)\\Microsoft.CSharp.targets\" />");
                stream.WriteLine("\t<Import Project=\"$(MSBuildExtensionsPath)\\Microsoft\\VisualStudio\\v8.0\\WebApplications\\Microsoft.WebApplication.targets\" />");
                stream.WriteLine("</Project>");

                stream.Close();
                #endregion
                
                Solucao.Projetos.Add(projeto);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (stream != null)
                    stream.Dispose();
            }
        }

        /// <summary>
        /// Gera arquivo de solução do projeto
        /// </summary>
        private void GerarSolucao()
        {
            StreamWriter stream = null;
            try
            {
                String pathSolucao = String.Concat(txtPath.Text, String.Format(@"\{0}", Solucao.Nome));
                Directory.CreateDirectory(pathSolucao);

                #region     .....:::::     CRIA ARQUIVO DA SOLUÇÃO     :::::.....

                String nomeSolucao = String.Format("{0}.sln", txtSolucao.Text);
                String pathProjeto = String.Format(@"{0}\{1}", pathSolucao, nomeSolucao);

                stream = new StreamWriter(pathProjeto, true);
                stream.WriteLine("Microsoft Visual Studio Solution File, Format Version 9.00");
                stream.WriteLine("# Visual Studio 2005");
                foreach(ProjetoVO projeto in Solucao.Projetos)
                {
                    stream.WriteLine(String.Format("Project(\"{0}\") = \"{1}\", \"{1}\\{2}.csproj\", \"{3}\"", String.Concat("{", Solucao.Guid.ToString(), "}"), projeto.Nome, projeto.Nome, String.Concat("{", projeto.Guid, "}") ));
                    stream.WriteLine("EndProject");
                }
                stream.WriteLine("Global");
                stream.WriteLine("\tGlobalSection(SolutionConfigurationPlatforms) = preSolution");
                stream.WriteLine("\t\tDebug|Any CPU = Debug|Any CPU");
                stream.WriteLine("\t\tRelease|Any CPU = Release|Any CPU");
                stream.WriteLine("\tEndGlobalSection");
                stream.WriteLine("\tGlobalSection(ProjectConfigurationPlatforms) = postSolution");
                foreach (ProjetoVO projeto in Solucao.Projetos)
                {
                    stream.WriteLine(String.Format("\t\t{0}.Debug|Any CPU.ActiveCfg = Debug|Any CPU", String.Concat("{", projeto.Guid.ToString() ,"}")));
                    stream.WriteLine(String.Format("\t\t{0}.Debug|Any CPU.Build.0 = Debug|Any CPU", String.Concat("{", projeto.Guid.ToString(), "}")));
                    stream.WriteLine(String.Format("\t\t{0}.Release|Any CPU.ActiveCfg = Release|Any CPU", String.Concat("{", projeto.Guid.ToString(), "}")));
                    stream.WriteLine(String.Format("\t\t{0}.Release|Any CPU.Build.0 = Release|Any CPU", String.Concat("{", projeto.Guid.ToString(), "}")));
                }
                stream.WriteLine("\tEndGlobalSection");
                stream.WriteLine("\tGlobalSection(SolutionProperties) = preSolution");
                stream.WriteLine("\t\tHideSolutionNode = FALSE");
                stream.WriteLine("\tEndGlobalSection");
                stream.WriteLine("EndGlobal");

                stream.Close();

                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (stream != null)
                    stream.Dispose();
            }
        }

        #endregion

        #region     .....:::::     EVENTOS     :::::.....

        /// <summary>
        /// Evento disparado ao clicar em conectar/desconectar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConnectar_Click(object sender, EventArgs e)
        {
            if (btnConnectar.Text == "Desconectar")
            {
                Connection = null;
                LimparCamposConnectar();
                LimparCamposGerarCodigo();
                EnableCamposConnectar(true);
                EnableCamposGerarCodigo(false);
                RemoveAllNodes();
            }
            else
            {
                if (DriverBanco == EDriverBanco.Nenhum)
                {
                    MessageBox.Show("Selecione o Driver", "Campos Obrigatórios");
                    cboDriver.Focus();
                }
                else if (String.IsNullOrEmpty(txtServidor.Text))
                {
                    MessageBox.Show("Preencha o Servidor", "Campos Obrigatórios");
                    txtServidor.Focus();
                }
                else if (!chkTipoAutenticacao.Checked && (String.IsNullOrEmpty(txtLogin.Text) || String.IsNullOrEmpty(txtSenha.Text)))
                {
                    MessageBox.Show("Preencha o Login e a Senha", "Campos Obrigatórios");
                    if (String.IsNullOrEmpty(txtLogin.Text))
                        txtLogin.Focus();
                    else
                        txtSenha.Focus();
                }
                else
                {
                    Boolean connectado = false;
                    try
                    {
                        TestarConexao();
                        connectado = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Erro");
                    }

                    if (connectado)
                    {
                        pnlGerar.Enabled = true;
                        lblStatus.Text = "Online";
                        lblStatus.ForeColor = Color.Green;
                        EnableCamposConnectar(false);
                        EnableCamposGerarCodigo(true);
                        LoadBancoDeDados();
                        if (DriverBanco == EDriverBanco.Oracle)
                            LoadTabelas();
                    }
                }
            }
        }

        /// <summary>
        /// Evento disparado ao check/uncheck tipo de autenticação
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkTipoAutenticacao_CheckedChanged(object sender, EventArgs e)
        {
            lblObrigatorioSenha.Visible = lblObrigatorioLogin.Visible = txtSenha.Enabled = txtLogin.Enabled = !chkTipoAutenticacao.Checked;
        }

        /// <summary>
        /// Evento disparado ao selecionar um item do combobox de banco de dados
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboBanco_SelectionChangeCommitted(object sender, EventArgs e)
        {
            LoadTabelas();
        }

        private void treeTabelasCampos_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Nodes.Count > 0)
                this.MarcarNosPosteriores(e.Node.Nodes, e.Node.Checked);
            else
                this.MarcarNodesAcima(e.Node, e.Node.Checked);

        }
        
        /// <summary>
        /// Evento disparado para gerar o código C#
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGerarCodigo_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtPath.Text))
            {
                MessageBox.Show("Selecione um diretório para gerar o código", "Campo Obrigatório");
            }
            else if (String.IsNullOrEmpty(txtSolucao.Text))
            {
                MessageBox.Show("Preencha o nome da solução do projeto", "Campo Obrigatório");
            }
            else
            {
                try
                {
                    Solucao = new SolucaoVO() { Nome = txtSolucao.Text };
                    
                    //GerarProjetoUtilitarios();
                    GerarProjetoEntidades();
                    GerarProjetoAcessoDados();
                    GerarProjetoNegocios();
                    GerarProjetoWeb();
                    GerarSolucao();

                    MessageBox.Show("Código gerado com sucesso.", "Sucesso");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Erro");
                }
            }
        }

        /// <summary>
        /// Evento chamado para selecionar um path
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelecionar_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowsDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtPath.Text = folderBrowsDialog.SelectedPath;
            }
        }

        /// <summary>
        /// Evento que somente deixa digitar números em um componente textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtTextBoxNumeros_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        #endregion

    }
        
    /// <summary>
    /// Enumerator de driver de banco
    /// </summary>
    public enum EDriverBanco
    {
        Nenhum = 0,
        Oracle = 1,
        SqlServer = 2
    }
}
