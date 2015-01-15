using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Data;
using Oracle.DataAccess.Client;
using System.Data.OracleClient;
using Web.Controls.Components;

namespace GeradorCodigo.WinUI
{
    /// <summary>
    /// Classe que extende objetos para facilitar na programação
    /// </summary>
    public static class Extensions
    {

        #region metodos estaticos

        /// <summary>
        /// criação de método para o objeto String para retornar uma String sem mascara, utilizando extensions
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static String ToText(this ColunaVO coluna)
        {
            String retorno = String.Empty;
            switch (coluna.TipoColuna)
            {
                case ETipoColuna.BigInt:
                    retorno = String.Format("{0} {1} Nullable({2})", coluna.Nome, ETipoColuna.BigInt.ToString().ToUpper(), coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.Binary:
                    retorno = String.Format("{0} {1}{2} Nullable({3})", coluna.Nome, ETipoColuna.Binary.ToString().ToUpper(), coluna.Tamanho.HasValue ? String.Concat("(", coluna.Tamanho.GetValueOrDefault(), ")") : String.Empty, coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.Bit:
                    retorno = String.Format("{0} {1} Nullable({2})", coluna.Nome, ETipoColuna.Bit.ToString().ToUpper(), coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.Char:
                    retorno = String.Format("{0} {1}{2} Nullable({3})", coluna.Nome, ETipoColuna.Char.ToString().ToUpper(), coluna.Tamanho.HasValue ? String.Concat("(", coluna.Tamanho.GetValueOrDefault(), ")") : String.Empty, coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.Date:
                    retorno = String.Format("{0} {1} Nullable({2})", coluna.Nome, ETipoColuna.Date.ToString().ToUpper(), coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.DateTime:
                    retorno = String.Format("{0} {1} Nullable({2})", coluna.Nome, ETipoColuna.DateTime.ToString().ToUpper(), coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.DateTimeOffSet:
                    retorno = String.Format("{0} {1} Nullable({2})", coluna.Nome, ETipoColuna.DateTimeOffSet.ToString().ToUpper(), coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.Decimal:
                    retorno = String.Format("{0} {1}({2}{3}) Nullable({4})", coluna.Nome, ETipoColuna.Decimal.ToString().ToUpper(), coluna.Tamanho.HasValue ? coluna.Tamanho.GetValueOrDefault().ToString() : String.Empty, coluna.Scala.HasValue ? String.Concat(",", coluna.Scala.GetValueOrDefault()) : String.Empty, coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.Float:
                    retorno = String.Format("{0} {1} Nullable({2})", coluna.Nome, ETipoColuna.Float.ToString().ToUpper(), coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.Image:
                    retorno = String.Format("{0} {1} Nullable({2})", coluna.Nome, ETipoColuna.Image.ToString().ToUpper(), coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.Int:
                    retorno = String.Format("{0} {1} Nullable({2})", coluna.Nome, ETipoColuna.Int.ToString().ToUpper(), coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.Money:
                    retorno = String.Format("{0} {1} Nullable({2})", coluna.Nome, ETipoColuna.Money.ToString().ToUpper(), coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.NChar:
                    retorno = String.Format("{0} {1}{2} Nullable({3})", coluna.Nome, ETipoColuna.NChar.ToString().ToUpper(), coluna.Tamanho.HasValue ? String.Concat("(", coluna.Tamanho.GetValueOrDefault(), ")") : String.Empty, coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.NText:
                    retorno = String.Format("{0} {1} Nullable({2})", coluna.Nome, ETipoColuna.NText.ToString().ToUpper(), coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.Numeric:
                    retorno = String.Format("{0} {1}({2}{3}) Nullable({4})", coluna.Nome, ETipoColuna.Numeric.ToString().ToUpper(), coluna.Tamanho.HasValue ? coluna.Tamanho.GetValueOrDefault().ToString() : String.Empty, coluna.Scala.HasValue ? String.Concat(",", coluna.Scala.GetValueOrDefault()) : String.Empty, coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.NVarchar:
                    retorno = String.Format("{0} {1}{2} Nullable({3})", coluna.Nome, ETipoColuna.NVarchar.ToString().ToUpper(), coluna.Tamanho.HasValue ? String.Concat("(", coluna.Tamanho.GetValueOrDefault(), ")") : String.Empty, coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.Real:
                    retorno = String.Format("{0} {1} Nullable({2})", coluna.Nome, ETipoColuna.Real.ToString().ToUpper(), coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.SmallDateTime:
                    retorno = String.Format("{0} {1} Nullable({2})", coluna.Nome, ETipoColuna.SmallDateTime.ToString().ToUpper(), coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.SmallInt:
                    retorno = String.Format("{0} {1} Nullable({2})", coluna.Nome, ETipoColuna.SmallInt.ToString().ToUpper(), coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.SmallMoney:
                    retorno = String.Format("{0} {1} Nullable({2})", coluna.Nome, ETipoColuna.SmallMoney.ToString().ToUpper(), coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.SqlVariant:
                    retorno = String.Format("{0} {1} Nullable({2})", coluna.Nome, ETipoColuna.SqlVariant.ToString().ToUpper(), coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.Text:
                    retorno = String.Format("{0} {1} Nullable({2})", coluna.Nome, ETipoColuna.Text.ToString().ToUpper(), coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.Time:
                    retorno = String.Format("{0} {1} Nullable({2})", coluna.Nome, ETipoColuna.Time.ToString().ToUpper(), coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.TimeStamp:
                    retorno = String.Format("{0} {1} Nullable({2})", coluna.Nome, ETipoColuna.TimeStamp.ToString().ToUpper(), coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.TinyInt:
                    retorno = String.Format("{0} {1} Nullable({2})", coluna.Nome, ETipoColuna.TinyInt.ToString().ToUpper(), coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.UniqueIdentifier:
                    retorno = String.Format("{0} {1} Nullable({2})", coluna.Nome, ETipoColuna.UniqueIdentifier.ToString().ToUpper(), coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.VarBinary:
                    retorno = String.Format("{0} {1}{2} Nullable({3})", coluna.Nome, ETipoColuna.VarBinary.ToString().ToUpper(), coluna.Tamanho.HasValue ? String.Concat("(", coluna.Tamanho.GetValueOrDefault(), ")") : String.Empty, coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.Varchar:
                    retorno = String.Format("{0} {1}{2} Nullable({3})", coluna.Nome, ETipoColuna.Varchar.ToString().ToUpper(), coluna.Tamanho.HasValue ? String.Concat("(", coluna.Tamanho.GetValueOrDefault(), ")") : String.Empty, coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.Xml:
                    retorno = String.Format("{0} {1} Nullable({2})", coluna.Nome, ETipoColuna.Xml.ToString().ToUpper(), coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.BinaryDouble:
                    retorno = String.Format("{0} {1}({2}{3}) Nullable({4})", coluna.Nome, ETipoColuna.BinaryDouble.ToString().ToUpper(), coluna.Tamanho.HasValue ? coluna.Tamanho.GetValueOrDefault().ToString() : String.Empty, coluna.Scala.HasValue ? String.Concat(",", coluna.Scala.GetValueOrDefault()) : String.Empty, coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.BinaryFloat:
                    retorno = String.Format("{0} {1}({2}{3}) Nullable({4})", coluna.Nome, ETipoColuna.BinaryDouble.ToString().ToUpper(), coluna.Tamanho.HasValue ? coluna.Tamanho.GetValueOrDefault().ToString() : String.Empty, coluna.Scala.HasValue ? String.Concat(",", coluna.Scala.GetValueOrDefault()) : String.Empty, coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.Blob:
                    retorno = String.Format("{0} {1}{2} Nullable({3})", coluna.Nome, ETipoColuna.Blob.ToString().ToUpper(), coluna.Tamanho.HasValue ? String.Concat("(", coluna.Tamanho.GetValueOrDefault(), ")") : String.Empty, coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.Clob:
                    retorno = String.Format("{0} {1}{2} Nullable({3})", coluna.Nome, ETipoColuna.Varchar.ToString().ToUpper(), coluna.Tamanho.HasValue ? String.Concat("(", coluna.Tamanho.GetValueOrDefault(), ")") : String.Empty, coluna.IsNullable.ToString().ToLower());
                    break;
               case ETipoColuna.IntervalDayToSecond:
                    retorno = String.Format("{0} {1} Nullable({2})", coluna.Nome, ETipoColuna.IntervalDayToSecond.ToString().ToUpper(), coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.IntervalYearToMonth:
                    retorno = String.Format("{0} {1} Nullable({2})", coluna.Nome, ETipoColuna.IntervalYearToMonth.ToString().ToUpper(), coluna.IsNullable.ToString().ToLower());
                    break; 
                case ETipoColuna.Long:
                    retorno = String.Format("{0} {1}{2} Nullable({3})", coluna.Nome, ETipoColuna.Long.ToString().ToUpper(), coluna.Tamanho.HasValue ? String.Concat("(", coluna.Tamanho.GetValueOrDefault(), ")") : String.Empty, coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.LongRaw:
                    retorno = String.Format("{0} {1}{2} Nullable({3})", coluna.Nome, ETipoColuna.LongRaw.ToString().ToUpper(), coluna.Tamanho.HasValue ? String.Concat("(", coluna.Tamanho.GetValueOrDefault(), ")") : String.Empty, coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.NClob:
                    retorno = String.Format("{0} {1}{2} Nullable({3})", coluna.Nome, ETipoColuna.NClob.ToString().ToUpper(), coluna.Tamanho.HasValue ? String.Concat("(", coluna.Tamanho.GetValueOrDefault(), ")") : String.Empty, coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.Number:
                    retorno = String.Format("{0} {1}({2}{3}) Nullable({4})", coluna.Nome, ETipoColuna.Number.ToString().ToUpper(), coluna.Tamanho.HasValue ? coluna.Tamanho.GetValueOrDefault().ToString() : String.Empty, coluna.Scala.HasValue ? String.Concat(",", coluna.Scala.GetValueOrDefault()) : String.Empty, coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.NVarChar2:
                    retorno = String.Format("{0} {1}{2} Nullable({3})", coluna.Nome, ETipoColuna.Varchar.ToString().ToUpper(), coluna.Tamanho.HasValue ? String.Concat("(", coluna.Tamanho.GetValueOrDefault(), ")") : String.Empty, coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.Raw:
                    retorno = String.Format("{0} {1}{2} Nullable({3})", coluna.Nome, ETipoColuna.Raw.ToString().ToUpper(), coluna.Tamanho.HasValue ? String.Concat("(", coluna.Tamanho.GetValueOrDefault(), ")") : String.Empty, coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.TimeStampWithLocalTimeZone:
                    retorno = String.Format("{0} {1} Nullable({2})", coluna.Nome, ETipoColuna.TimeStampWithLocalTimeZone.ToString().ToUpper(), coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.TimeStampWithTimeZone:
                    retorno = String.Format("{0} {1} Nullable({2})", coluna.Nome, ETipoColuna.TimeStampWithTimeZone.ToString().ToUpper(), coluna.IsNullable.ToString().ToLower());
                    break;
                case ETipoColuna.VarChar2:
                    retorno = String.Format("{0} {1}{2} Nullable({3})", coluna.Nome, ETipoColuna.VarChar2.ToString().ToUpper(), coluna.Tamanho.HasValue ? String.Concat("(", coluna.Tamanho.GetValueOrDefault(), ")") : String.Empty, coluna.IsNullable.ToString().ToLower());
                    break;
            }
            return retorno;
        }

        /// <summary>
        /// criação de método para o objeto String para retornar uma String com a primeira letra minuscula
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static String ToAtributo(this String coluna)
        {
            if (coluna.Length > 0)
                return String.Concat(coluna.Substring(0, 1).ToLower(), coluna.Substring(1));
            return coluna;
        }

        /// <summary>
        /// Retorna a string com o component escrito
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static String ToWebComponent(this CampoVO campo, String validationGroup, String nomePanel)
        {
            String retorno = String.Empty;
            if (campo.TipoComponent == typeof(WTextBox))
            {
                retorno = String.Format("<WC:WTextBox ID=\"txt{0}{1}\" runat=\"server\" Required=\"{2}\" {3} TypeTextField=\"{4}\" Caption=\"{5}\" ValidationGroup=\"{6}\" />", campo.Propriedade.Nome, nomePanel, campo.Obrigatorio.ToString().ToLower(), campo.Propriedade.Coluna.Tamanho.HasValue ? String.Concat("MaxLength=\"", campo.Propriedade.Coluna.Tamanho, "\"") : String.Empty, campo.TipoCampo.ToString(), campo.Caption, validationGroup);
            }
            else if (campo.TipoComponent == typeof(WNumberBox))
            {
                retorno = String.Format("<WC:WNumberBox ID=\"txt{0}{1}\" runat=\"server\" Required=\"{2}\" {3} Caption=\"{4}\" ValidationGroup=\"{5}\" />", campo.Propriedade.Nome, nomePanel, campo.Obrigatorio.ToString().ToLower(), campo.Propriedade.Coluna.Tamanho.HasValue ? String.Concat("MaxLength=\"", campo.Propriedade.Coluna.Tamanho, "\"") : String.Empty, campo.Caption, validationGroup);
            }
            else if (campo.TipoComponent == typeof(WDropDownList))
            {
                String displayField = !String.IsNullOrEmpty(campo.DisplayField) ? String.Format(" DataTextField=\"{0}\"", campo.DisplayField) : String.Empty;
                String valueField = !String.IsNullOrEmpty(campo.ValueField) ? String.Format(" DataValueField=\"{0}\"", campo.ValueField) : String.Empty;
                retorno = String.Format("<WC:WDropDownList ID=\"ddl{0}{1}\" runat=\"server\" Required=\"{2}\" Caption=\"{3}\" ValidationGroup=\"{4}\"{5}{6}></WC:WDropDownList>", campo.Propriedade.Nome, nomePanel, campo.Obrigatorio.ToString().ToLower(), campo.Caption, validationGroup, displayField, valueField);
            }
            else if (campo.TipoComponent == typeof(WRadioButtonList))
            {
                String displayField = !String.IsNullOrEmpty(campo.DisplayField) ? String.Format(" DataTextField=\"{0}\"", campo.DisplayField) : String.Empty;
                String valueField = !String.IsNullOrEmpty(campo.ValueField) ? String.Format(" DataValueField=\"{0}\"", campo.ValueField) : String.Empty;

                retorno = String.Format("<WC:WRadioButtonList runat=\"server\" ID=\"rdb{0}{1}\" Caption=\"{2}\" Required=\"{3}\" ValidationGroup=\"{4}\"{5}{6}>", campo.Propriedade.Nome, nomePanel, campo.Caption, campo.Obrigatorio.ToString().ToLower(), validationGroup, displayField, valueField);

                if (String.IsNullOrEmpty(valueField) && String.IsNullOrEmpty(displayField))
                {
                    retorno += "<asp:ListItem Text=\"Item1\" Value=\"0\" />";
                    retorno += "<asp:ListItem Text=\"Item2\" Value=\"1\" />";
                    retorno += "<asp:ListItem Text=\"Item3\" Value=\"2\" />";
                }
                retorno += "</WC:WRadioButtonList>";
            }
            else if (campo.TipoComponent == typeof(WCheckBox))
            {
                retorno = String.Format("<WC:WCheckBox ID=\"chk{0}{1}\" runat=\"server\" Caption=\"{2}\" />", campo.Propriedade.Nome, nomePanel, campo.Caption);
            }
            else if (campo.TipoComponent == typeof(WLabel))
            {
                retorno = String.Format("<WC:WLabel runat=\"server\" ID=\"lbl{0}{1}\" ShowCaption=\"true\" Caption=\"{2}\" />", campo.Propriedade.Nome, nomePanel, campo.Caption);
            }
            return retorno;
        }

        /// <summary>
        /// Retorna o prefixo do nome do controle
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static String ToTextPrefixoWebComponent(this Type type)
        {
            String retorno = String.Empty;
            if (type == typeof(WTextBox))
            {
                retorno = "txt";
            }
            else if (type == typeof(WDropDownList))
            {
                retorno = "ddl";
            }
            else if (type == typeof(WRadioButtonList))
            {
                retorno = "rbl";
            }
            else if (type == typeof(WCheckBox))
            {
                retorno = "chk";
            }
            else if (type == typeof(WLabel))
            {
                retorno = "lbl";
            }
            return retorno;
        }

        /// <summary>
        /// Retorna uma coluna do gridview
        /// </summary>
        /// <param name="campo"></param>
        /// <returns></returns>
        public static String ToColumnGridComponent(this CampoVO campo)
        {
            String retorno = String.Format("<asp:BoundField DataField=\"{0}\" HeaderText=\"{0}\" SortExpression=\"{0}\" ", campo.Propriedade.Nome);
            if (!String.IsNullOrEmpty(campo.Mascara))
            {
                retorno += String.Format("DataFormatString=\"{0}\"", campo.Mascara);
            }
            retorno += "/>";
            return retorno;
        }

        /// <summary>
        /// Retorna o tipo de componente Web
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static String ToTextDesignerWebComponent(this CampoVO campo, String nomePanel)
        {
            String retorno = String.Empty;
            retorno = String.Format("protected global::Web.Controls.Components.{0} {1}{2}{3};", campo.TipoComponent.Name, campo.TipoComponent.ToTextPrefixoWebComponent(), campo.Propriedade.Nome, nomePanel);
            return retorno;
        }

        /// <summary>
        /// Retorna o código de limpeza de um campo específico
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static String ToTextLimparCampo(this CampoVO campo, String nomePanel)
        {
            String retorno = String.Empty;
            if(campo.TipoComponent == typeof(WTextBox))
            {
                retorno = String.Format("{0}{1}{2}.Text = String.Empty;", campo.TipoComponent.ToTextPrefixoWebComponent(), campo.Propriedade.Nome, nomePanel);
            }
            else if (campo.TipoComponent == typeof(WDropDownList) || campo.TipoComponent == typeof(WRadioButtonList))
            {
                retorno = String.Format("{0}{1}{2}.ClearSelection();", campo.TipoComponent.ToTextPrefixoWebComponent(), campo.Propriedade.Nome, nomePanel);
            }
            else if (campo.TipoComponent == typeof(WCheckBox))
            {
                retorno = String.Format("{0}{1}{2}.Checked = false;", campo.TipoComponent.ToTextPrefixoWebComponent(), campo.Propriedade.Nome, nomePanel);
            }
            return retorno;
        }

        /// <summary>
        /// Retorna o tipo de campo de acordo com o tipo da propriedade
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static WTextBox.ETypeMaskTextField GetTypeTextField(this CampoVO campo)
        {
            WTextBox.ETypeMaskTextField retorno = WTextBox.ETypeMaskTextField.Default;
            switch (campo.Propriedade.Coluna.TipoColuna)
            {
                case ETipoColuna.IntervalYearToMonth:
                case ETipoColuna.SmallInt:
                case ETipoColuna.Int:
                case ETipoColuna.BigInt:
                    retorno = WTextBox.ETypeMaskTextField.Numeric;
                    break;
                case ETipoColuna.VarChar2:
                case ETipoColuna.NVarChar2:
                case ETipoColuna.Long:
                case ETipoColuna.IntervalDayToSecond:
                case ETipoColuna.Varchar:
                case ETipoColuna.Text:
                case ETipoColuna.NVarchar:
                case ETipoColuna.NChar:
                case ETipoColuna.NText:
                case ETipoColuna.Char:
                    retorno = WTextBox.ETypeMaskTextField.Default;
                    break;
                case ETipoColuna.TimeStampWithTimeZone:
                case ETipoColuna.TimeStampWithLocalTimeZone:
                case ETipoColuna.SmallDateTime:
                case ETipoColuna.Date:
                case ETipoColuna.DateTime:
                case ETipoColuna.DateTimeOffSet:
                    retorno = WTextBox.ETypeMaskTextField.Date;
                    break;
                case ETipoColuna.SmallMoney:
                case ETipoColuna.Decimal:
                case ETipoColuna.Float:
                case ETipoColuna.Money:
                    retorno = WTextBox.ETypeMaskTextField.Money;
                    break;
                case ETipoColuna.Number:
                case ETipoColuna.Numeric:
                    if (!campo.Propriedade.Coluna.Scala.HasValue || campo.Propriedade.Coluna.Scala.GetValueOrDefault() == 0)
                    {
                        retorno = WTextBox.ETypeMaskTextField.Numeric;
                    }
                    else
                    {
                        retorno = WTextBox.ETypeMaskTextField.Money;
                    }
                    break;
            }
            return retorno;
        }

        /// <summary>
        /// Faz o parse do tipo de coluna para o tipo de variável
        /// </summary>
        /// <param name="tipoColuna"></param>
        public static ETipoVeriavel GetTipoVariavel(this ColunaVO coluna)
        {
            
            ETipoVeriavel tipoVariavel = ETipoVeriavel.Nenhum;

            if (coluna.IsChaveEstrangeira && coluna.ChaveEstrangeira != null)
                return ETipoVeriavel.Object;

            switch (coluna.TipoColuna)
            {
                case ETipoColuna.BigInt:
                    tipoVariavel = ETipoVeriavel.Int64;
                    break;
                case ETipoColuna.Binary:
                    tipoVariavel = ETipoVeriavel.ByteArray;
                    break;
                case ETipoColuna.Bit:
                    tipoVariavel = ETipoVeriavel.Boolean;
                    break;
                case ETipoColuna.Char:
                    tipoVariavel = ETipoVeriavel.String;
                    break;
                case ETipoColuna.Date:
                    tipoVariavel = ETipoVeriavel.DateTime;
                    break;
                case ETipoColuna.DateTime:
                    tipoVariavel = ETipoVeriavel.DateTime;
                    break;
                case ETipoColuna.DateTimeOffSet:
                    tipoVariavel = ETipoVeriavel.DateTimeOffSet;
                    break;
                case ETipoColuna.Decimal:
                    tipoVariavel = ETipoVeriavel.Decimal;
                    break;
                case ETipoColuna.Float:
                    tipoVariavel = ETipoVeriavel.Double;
                    break;
                case ETipoColuna.Image:
                    tipoVariavel = ETipoVeriavel.ByteArray;
                    break;
                case ETipoColuna.Int:
                    tipoVariavel = ETipoVeriavel.Int32;
                    break;
                case ETipoColuna.Money:
                    tipoVariavel = ETipoVeriavel.Decimal;
                    break;
                case ETipoColuna.NChar:
                    tipoVariavel = ETipoVeriavel.String;
                    break;
                case ETipoColuna.NText:
                    tipoVariavel = ETipoVeriavel.String;
                    break;
                case ETipoColuna.Numeric:
                    if (!coluna.Scala.HasValue || coluna.Scala.GetValueOrDefault() == 0)
                    {
                        if (coluna.Tamanho.GetValueOrDefault() <= 4)
                            tipoVariavel = ETipoVeriavel.Int16;
                        else if (coluna.Tamanho.GetValueOrDefault() <= 8)
                            tipoVariavel = ETipoVeriavel.Int32;
                        else
                            tipoVariavel = ETipoVeriavel.Int64;
                    }
                    else
                    {
                        tipoVariavel = ETipoVeriavel.Decimal;
                    }

                    break;
                case ETipoColuna.NVarchar:
                    tipoVariavel = ETipoVeriavel.String;
                    break;
                case ETipoColuna.Real:
                    tipoVariavel = ETipoVeriavel.Single;
                    break;
                case ETipoColuna.SmallDateTime:
                    tipoVariavel = ETipoVeriavel.DateTime;
                    break;
                case ETipoColuna.SmallInt:
                    tipoVariavel = ETipoVeriavel.Int16;
                    break;
                case ETipoColuna.SmallMoney:
                    tipoVariavel = ETipoVeriavel.Decimal;
                    break;
                case ETipoColuna.SqlVariant:
                    tipoVariavel = ETipoVeriavel.Object;
                    break;
                case ETipoColuna.Text:
                    tipoVariavel = ETipoVeriavel.String;
                    break;
                case ETipoColuna.Time:
                    tipoVariavel = ETipoVeriavel.TimeSpan;
                    break;
                case ETipoColuna.TimeStamp:
                    tipoVariavel = ETipoVeriavel.TimeSpan;
                    break;
                case ETipoColuna.TinyInt:
                    tipoVariavel = ETipoVeriavel.Byte;
                    break;
                case ETipoColuna.UniqueIdentifier:
                    tipoVariavel = ETipoVeriavel.Guid;
                    break;
                case ETipoColuna.VarBinary:
                    tipoVariavel = ETipoVeriavel.ByteArray;
                    break;
                case ETipoColuna.Varchar:
                    tipoVariavel = ETipoVeriavel.String;
                    break;
                case ETipoColuna.Xml:
                    tipoVariavel = ETipoVeriavel.Xml;
                    break;
                case ETipoColuna.BinaryFloat:
                    tipoVariavel = ETipoVeriavel.Decimal;
                    break;
                case ETipoColuna.BinaryDouble:
                    tipoVariavel = ETipoVeriavel.Decimal;
                    break;
                case ETipoColuna.Blob:
                    tipoVariavel = ETipoVeriavel.ByteArray;
                    break;
                case ETipoColuna.Clob:
                    tipoVariavel = ETipoVeriavel.String;
                    break;
                case ETipoColuna.IntervalDayToSecond:
                    tipoVariavel = ETipoVeriavel.TimeSpan;
                    break;
                case ETipoColuna.IntervalYearToMonth:
                    tipoVariavel = ETipoVeriavel.Int64;
                    break;
                case ETipoColuna.Long:
                    tipoVariavel = ETipoVeriavel.String;
                    break;
                case ETipoColuna.LongRaw:
                    tipoVariavel = ETipoVeriavel.ByteArray;
                    break;
                case ETipoColuna.NClob:
                    tipoVariavel = ETipoVeriavel.String;
                    break;
                case ETipoColuna.Number:
                    if(!coluna.Scala.HasValue || coluna.Scala.GetValueOrDefault() == 0)
                    {
                        if(coluna.Tamanho.GetValueOrDefault() <= 4)
                            tipoVariavel = ETipoVeriavel.Int16;
                        else if (coluna.Tamanho.GetValueOrDefault() <= 8)
                            tipoVariavel = ETipoVeriavel.Int32;
                        else
                            tipoVariavel = ETipoVeriavel.Int64;
                    }
                    else
                    {
                        tipoVariavel = ETipoVeriavel.Decimal;
                    }
                    break;
                case ETipoColuna.NVarChar2:
                    tipoVariavel = ETipoVeriavel.String;
                    break;
                case ETipoColuna.Raw:
                    tipoVariavel = ETipoVeriavel.ByteArray;
                    break;
                case ETipoColuna.TimeStampWithLocalTimeZone:
                    tipoVariavel = ETipoVeriavel.DateTime;
                    break;
                case ETipoColuna.TimeStampWithTimeZone:
                    tipoVariavel = ETipoVeriavel.DateTime;
                    break;
                case ETipoColuna.VarChar2:
                    tipoVariavel = ETipoVeriavel.String;
                    break;
            }
            return tipoVariavel;
        }

        /// <summary>
        /// Retorno a classe de tipo de parametro, de acordo com driver especificado
        /// </summary>
        /// <param name="driver"></param>
        /// <returns></returns>
        public static String ToTextDbType(this EDriverBanco driver)
        {
            String retorno = String.Empty;

            switch (driver)
            {
                case EDriverBanco.Oracle:
                    retorno = "OracleDbType";
                    break;
                case EDriverBanco.SqlServer:
                    retorno = "DbType";
                    break;
            }
            return retorno;
        }

        /// <summary>
        /// Retorno a classe de execução de comando, de acordo com driver especificado
        /// </summary>
        /// <param name="driver"></param>
        /// <returns></returns>
        public static String ToTextCommand(this EDriverBanco driver)
        {
            String retorno = String.Empty;

            switch (driver)
            {
                case EDriverBanco.Oracle:
                    retorno = "OracleCommand";
                    break;
                case EDriverBanco.SqlServer:
                    retorno = "DbCommand";
                    break;
            }
            return retorno;
        }

        /// <summary>
        /// Retorno a classe de manipulação de parametros, de acordo com driver especificado
        /// </summary>
        /// <param name="driver"></param>
        /// <returns></returns>
        public static String ToTextParameter(this EDriverBanco driver)
        {
            String retorno = String.Empty;

            switch (driver)
            {
                case EDriverBanco.Oracle:
                    retorno = "OracleParameter";
                    break;
                case EDriverBanco.SqlServer:
                    retorno = "DbParameter";
                    break;
            }
            return retorno;
        }

        /// <summary>
        /// Retorna o nome do enumerator do tipo de dado, de acordo com o driver especificado
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="driver"></param>
        /// <returns></returns>
        public static String ToTextTypeDriver(this PropriedadeVO prop, EDriverBanco driver)
        {
            String retorno = String.Empty;
            
            switch (driver)
            {
                case EDriverBanco.Oracle:
                    OracleDbType typeOracle = prop.GetDbTypeOracle();
                    retorno = typeOracle.ToString();
                    break;
                case EDriverBanco.SqlServer:
                    DbType typeSql = prop.GetDbTypeSqlServer();
                    retorno = typeSql.ToString();
                    break;
            }
            return retorno;
        }

        /// <summary>
        /// Faz o parse do tipo da propriedade para o tipo DbType SqlServer
        /// </summary>
        /// <param name="tipoColuna"></param>
        public static DbType GetDbTypeSqlServer(this PropriedadeVO prop)
        {
            DbType dbType = DbType.String;
            switch (prop.Coluna.TipoColuna)
            {
                case ETipoColuna.BigInt:
                    dbType = DbType.Int64;
                    break;
                case ETipoColuna.Binary:
                    dbType = DbType.Binary;
                    break;
                case ETipoColuna.Bit:
                    dbType = DbType.Boolean;
                    break;
                case ETipoColuna.Char:
                    dbType = DbType.String;
                    break;
                case ETipoColuna.Date:
                    dbType = DbType.Date;
                    break;
                case ETipoColuna.DateTime:
                    dbType = DbType.DateTime;
                    break;
                case ETipoColuna.DateTimeOffSet:
                    dbType = DbType.DateTimeOffset;
                    break;
                case ETipoColuna.Decimal:
                    dbType = DbType.Decimal;
                    break;
                case ETipoColuna.Float:
                    dbType = DbType.Decimal;
                    break;
                case ETipoColuna.Image:
                    dbType = DbType.Binary;
                    break;
                case ETipoColuna.Int:
                    dbType = DbType.Int32;
                    break;
                case ETipoColuna.Money:
                    dbType = DbType.Decimal;
                    break;
                case ETipoColuna.NChar:
                    dbType = DbType.String;
                    break;
                case ETipoColuna.NText:
                    dbType = DbType.String;
                    break;
                case ETipoColuna.Numeric:
                    dbType = DbType.Decimal;
                    break;
                case ETipoColuna.NVarchar:
                    dbType = DbType.String;
                    break;
                case ETipoColuna.Real:
                    dbType = DbType.Single;
                    break;
                case ETipoColuna.SmallDateTime:
                    dbType = DbType.DateTime;
                    break;
                case ETipoColuna.SmallInt:
                    dbType = DbType.Int16;
                    break;
                case ETipoColuna.SmallMoney:
                    dbType = DbType.Decimal;
                    break;
                case ETipoColuna.Text:
                    dbType = DbType.String;
                    break;
                case ETipoColuna.Time:
                    dbType = DbType.Time;
                    break;
                case ETipoColuna.TimeStamp:
                    dbType = DbType.Binary;
                    break;
                case ETipoColuna.TinyInt:
                    dbType = DbType.Byte;
                    break;
                case ETipoColuna.UniqueIdentifier:
                    dbType = DbType.Guid;
                    break;
                case ETipoColuna.VarBinary:
                    dbType = DbType.Binary;
                    break;
                case ETipoColuna.Varchar:
                    dbType = DbType.String;
                    break;
                case ETipoColuna.SqlVariant:
                    dbType = DbType.Object;
                    break;
                case ETipoColuna.Xml:
                    dbType = DbType.Xml;
                    break;
            }
            return dbType;
        }

        /// <summary>
        /// Faz o parse do tipo da propriedade para o tipo DbType Oracle
        /// </summary>
        /// <param name="tipoColuna"></param>
        public static OracleDbType GetDbTypeOracle(this PropriedadeVO prop)
        {
            OracleDbType dbType = OracleDbType.Varchar2;
            switch (prop.Coluna.TipoColuna)
            {
                case ETipoColuna.Blob:
                    dbType = OracleDbType.Blob;
                    break;
                case ETipoColuna.BinaryDouble:
                    dbType = OracleDbType.Double;
                    break;
                case ETipoColuna.BinaryFloat:
                    dbType = OracleDbType.Decimal;
                    break;
                case ETipoColuna.Char:
                    dbType = OracleDbType.Char;
                    break;
                case ETipoColuna.Clob:
                    dbType = OracleDbType.Clob;
                    break;
                case ETipoColuna.Date:
                    dbType = OracleDbType.Date;
                    break;
                case ETipoColuna.Decimal:
                    dbType = OracleDbType.Decimal;
                    break;
                case ETipoColuna.IntervalDayToSecond:
                    dbType = OracleDbType.IntervalDS;
                    break;
                case ETipoColuna.IntervalYearToMonth:
                    dbType = OracleDbType.IntervalYM;
                    break;
                case ETipoColuna.Long:
                    dbType = OracleDbType.Long;
                    break;
                case ETipoColuna.Number:
                    dbType = OracleDbType.Decimal;
                    break;
                case ETipoColuna.LongRaw:
                    dbType = OracleDbType.LongRaw;
                    break;
                case ETipoColuna.NChar:
                    dbType = OracleDbType.NChar;
                    break;
                case ETipoColuna.NClob:
                    dbType = OracleDbType.NClob;
                    break;
                case ETipoColuna.NVarChar2:
                    dbType = OracleDbType.NVarchar2;
                    break;
                case ETipoColuna.Raw:
                    dbType = OracleDbType.Raw;
                    break;
                case ETipoColuna.TimeStamp:
                    dbType = OracleDbType.TimeStamp;
                    break;
                case ETipoColuna.TimeStampWithLocalTimeZone:
                    dbType = OracleDbType.TimeStampLTZ;
                    break;
                case ETipoColuna.TimeStampWithTimeZone:
                    dbType = OracleDbType.TimeStampTZ;
                    break;
                case ETipoColuna.VarChar2:
                    dbType = OracleDbType.Varchar2;
                    break;
            }
            return dbType;
        }

        /// <summary>
        /// Retorna o tipo da variável em String, verificando se a mesma é Nullable
        /// </summary>
        /// <param name="tipoColuna"></param>
        public static String ToTextTipoVariavel(this PropriedadeVO prop)
        {
            return String.Format("{0}{1}", prop.Coluna.ChaveEstrangeira != null ? String.Format("{0}VO", prop.Coluna.ChaveEstrangeira.Classe.Nome) : prop.Tipo == ETipoVeriavel.ByteArray ? "byte[]" : prop.Tipo.ToString(), prop.Nullable && prop.Coluna.ChaveEstrangeira == null ? "?" : String.Empty);
        }

        /// <summary>
        /// Retorna o tipo de máscara do textfield de acordo com o tipo de dado
        /// </summary>
        /// <param name="prop"></param>
        public static WTextBox.ETypeMaskTextField ToTypeMaskTextField(this PropriedadeVO prop)
        {
            WTextBox.ETypeMaskTextField retorno = WTextBox.ETypeMaskTextField.Default;
            switch (prop.Tipo)
            {
                case ETipoVeriavel.DateTime:
                case ETipoVeriavel.DateTimeOffSet:
                    retorno = WTextBox.ETypeMaskTextField.Date;
                    break;
                case ETipoVeriavel.Decimal:
                case ETipoVeriavel.Double:
                    retorno = WTextBox.ETypeMaskTextField.Money;
                    break;
                case ETipoVeriavel.Int16:
                case ETipoVeriavel.Int32:
                case ETipoVeriavel.Int64:
                    retorno = WTextBox.ETypeMaskTextField.Numeric;
                    break;
            }
            return retorno;
        }

        /// <summary>
        /// Retorna se a variável pode ser nullable
        /// </summary>
        /// <param name="tipoColuna"></param>
        public static Boolean GetVariavelNullable(this ColunaVO coluna)
        {
            Boolean retorno = true;
            switch (coluna.TipoColuna)
            {
                case ETipoColuna.Binary:
                    retorno = false;
                    break;
                case ETipoColuna.Char:
                    retorno = false;
                    break;
                case ETipoColuna.Image:
                    retorno = false;
                    break;
                case ETipoColuna.NChar:
                    retorno = false;
                    break;
                case ETipoColuna.NText:
                    retorno = false;
                    break;
                case ETipoColuna.NVarchar:
                    retorno = false;
                    break;
                case ETipoColuna.SqlVariant:
                    retorno = false;
                    break;
                case ETipoColuna.Text:
                    retorno = false;
                    break;
                case ETipoColuna.VarBinary:
                    retorno = false;
                    break;
                case ETipoColuna.Varchar:
                    retorno = false;
                    break;
                case ETipoColuna.Xml:
                    retorno = false;
                    break;
                case ETipoColuna.Blob:
                    retorno = false;
                    break;
                case ETipoColuna.Clob:
                    retorno = false;
                    break;
                case ETipoColuna.Long:
                    retorno = false;
                    break;
                case ETipoColuna.LongRaw:
                    retorno = false;
                    break;
                case ETipoColuna.NClob:
                    retorno = false;
                    break;
                case ETipoColuna.NVarChar2:
                    retorno = false;
                    break;
                case ETipoColuna.Raw:
                    retorno = false;
                    break;
                case ETipoColuna.VarChar2:
                    retorno = false;
                    break;
            }
            return retorno;
        }

        /// <summary>
        /// Retorna o tipo de componente a ser renderizado
        /// </summary>
        /// <param name="tipoColuna"></param>
        public static Type ToTypeComponent(this String comp)
        {
            Type type = typeof(WTextBox);
            switch (comp)
            {
                case "TextBox":
                    type = typeof(WTextBox);
                    break;
                case "DropDownList":
                    type = typeof(WDropDownList);
                    break;
                case "RadioButtonList":
                    type = typeof(WRadioButtonList);
                    break;
                case "CheckBox":
                    type = typeof(WCheckBox);
                    break;
                case "Label":
                    type = typeof(WLabel);
                    break;
            }
            return type;
        }

        /// <summary>
        /// Retorna o a máscara utilizada on grid para formatar o campo
        /// </summary>
        /// <param name="comp"></param>
        public static String ToMaskGrid(this String comp)
        {
            String retorno = String.Empty;
            switch (comp)
            {
                case "31/01/2001":
                    retorno = "{0:dd/MM/yyyy}";
                    break;
                case "2001/01/31":
                    retorno = "{0:yyyy/MM/dd}";
                    break;
                case "100,00":
                    retorno = "{0:n2}";
                    break;
                case "100,0000":
                    retorno = "{0:n4}";
                    break;
            }
            return retorno;
        }

        /// <summary>
        /// Retorna o nome da classe de conexão em String
        /// </summary>
        /// <param name="tipoColuna"></param>
        public static String ToTextConnection(this EDriverBanco driver)
        {
            String retorno = String.Empty;
            switch (driver)
            {
                case EDriverBanco.SqlServer:
                    retorno = "SqlConnection";
                    break;
                case EDriverBanco.Oracle:
                    retorno = "OracleConnection";
                    break;
            }
            return retorno;
        }

        /// <summary>
        /// Retorna a classe base de acesso a dados em String
        /// </summary>
        /// <param name="tipoColuna"></param>
        public static String ToTextBaseDAO(this EDriverBanco driver)
        {
            String retorno = String.Empty;
            switch (driver)
            {
                case EDriverBanco.SqlServer:
                    retorno = "SqlBaseDAO";
                    break;
                case EDriverBanco.Oracle:
                    retorno = "OracleBaseDAO";
                    break;
            }
            return retorno;
        }

        /// <summary>
        /// Retorna o prefixo utilizado nas procedures
        /// </summary>
        /// <param name="tipoColuna"></param>
        public static String GetPrefixoParametro(this EDriverBanco driver)
        {
            String retorno = String.Empty;
            switch (driver)
            {
                case EDriverBanco.SqlServer:
                    retorno = "@";
                    break;
                case EDriverBanco.Oracle:
                    retorno = "P_";
                    break;
            }
            return retorno;
        }

        /// <summary>
        /// Retorna o prefixo utilizado nas procedures
        /// </summary>
        /// <param name="tipoColuna"></param>
        public static String GetInicioParametroCommandText(this EDriverBanco driver)
        {
            String retorno = String.Empty;
            switch (driver)
            {
                case EDriverBanco.SqlServer:
                    retorno = "";
                    break;
                case EDriverBanco.Oracle:
                    retorno = ":";
                    break;
            }
            return retorno;
        }

        /// <summary>
        /// Retorna o nome do parametro de entrada da procedure
        /// </summary>
        /// <param name="tipoColuna"></param>
        public static String GetNomeParametro(this PropriedadeVO prop, ClasseVO classe, String prefixoParam, EDriverBanco driver)
        {
            String retorno = String.Format("{0}{1}{2}", prefixoParam, classe.Nome, prop.Nome);

            switch (driver)
            {
                case EDriverBanco.Oracle:
                    if (retorno.Length > 30)
                    {
                        retorno = retorno.Substring(0, 30);    
                    }
                    break;
            }

            return retorno;
        }

        /// <summary>
        /// Retorna o nome da procedure
        /// </summary>
        /// <param name="tipoColuna"></param>
        public static String GetNomeProcedure(this String obj, EDriverBanco driver)
        {
            switch (driver)
            {
                case EDriverBanco.Oracle:
                    if (obj.Length > 30)
                    {
                        obj = obj.Substring(0, 30);
                    }
                    break;
            }

            return obj;
        }

        #endregion

    }

}
