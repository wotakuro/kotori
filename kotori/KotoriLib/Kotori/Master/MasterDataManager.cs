using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kotori.Master
{
    public class MasterDataManager
    {
        private static MasterDataManager instance = new MasterDataManager();

        public static MasterDataManager Instance
        {
            get {
                return instance;
            }
        }

        private MasterDataManager() { }

        public void Initialize(Type[] types)
        {
        }

        /// <summary>
        /// Get Module name from typeObject ( attribute ).
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private Kotori.Attribute.Sql.MasterData GetMasterAttribute(Type t)
        {
            var attributeList = t.GetCustomAttributes(typeof(Kotori.Attribute.Sql.MasterData), false);
            foreach (var attr in attributeList)
            {
                Kotori.Attribute.Sql.MasterData attrMaster = attr as Kotori.Attribute.Sql.MasterData;
                if (attrMaster != null)
                {
                    return attrMaster;
                }
            }
            return null;
        }


    }
}
