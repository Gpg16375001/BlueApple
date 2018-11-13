using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SmileLab.Net.API
{
	public partial class ConsumerData
    {

        public ConsumerData(int id, int count)
        {
			ConsumerId = id;
			Count = count;
			ModificationDate = GameTime.SharedInstance.Now.ToString();
			CreationDate = GameTime.SharedInstance.Now.ToString();
        }
        public ConsumerData() { }
    }	
}
