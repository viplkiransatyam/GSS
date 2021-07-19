using GSS.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSS.DataAccess.Layer
{
    public class CommonFunctions
    {
        /// <summary>
        /// Parse the consumption formula to formula collection
        /// </summary>
        /// <param name="ConsFormula"></param>
        /// <returns></returns>
        public static List<GasOilFormula> ParseGasOilFormula(string ConsFormula)
        {
            try
            {
                List<GasOilFormula> _GasOilFormulaColl = new List<GasOilFormula>();
                GasOilFormula _GasOilFormula;
                string[] SplitFomula = ConsFormula.Split('+');

                for (int i=0; i <= SplitFomula.GetUpperBound(0);i++)
                {
                    _GasOilFormula = new GasOilFormula();
                    string[] SplitFields = SplitFomula[i].Split('X');

                    string sTemp = SplitFields[0].Replace("(","");
                    sTemp = sTemp.Replace("(","");
                    _GasOilFormula.GasTypeID = Convert.ToInt16(sTemp);
                    
                    sTemp = SplitFields[1].Replace(")", "");
                    sTemp = sTemp.Replace("(", "");
                    _GasOilFormula.GasOilConsmptPercent = Convert.ToSingle(sTemp);
                    _GasOilFormulaColl.Add(_GasOilFormula);
                }
                return _GasOilFormulaColl;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// Parse the consumption formula to formula collection
        /// </summary>
        /// <param name="ConsFormula"></param>
        /// <returns></returns>
        public static string BuildGasOilFormula(List<GasOilFormula> objFormula)
        {
            try
            {
                string sFormula = "";
                foreach (GasOilFormula _GasOilFormula in objFormula)
                {
                    sFormula = sFormula + "(" + _GasOilFormula.GasTypeID + "X" + _GasOilFormula.GasOilConsmptPercent + ") +";
                }
                sFormula = sFormula.Substring(0, sFormula.Length - 1);

                return sFormula;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }
}
