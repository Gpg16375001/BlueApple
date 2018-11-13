using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SmileLab.Net.API
{
    public partial class PvpTeamData
    {
        private Formation _Formation;
        public Formation Formation {
            get {
                if (_Formation == null) {
                    _Formation = MasterDataTable.formation [FormationId];
                }
                return _Formation;
            }
        }
    }
}