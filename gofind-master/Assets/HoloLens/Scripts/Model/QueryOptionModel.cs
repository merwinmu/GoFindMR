using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.HoloLens.Scripts.Model
{
   

    public interface IAddedQueryOptionModel
    {
        // Dispatched when years changes
        event EventHandler<AddedQueryOption> OnDataReceived;
        void setData(string data);
    }
    public class QueryOptionModel : IAddedQueryOptionModel
    {
        public event EventHandler<AddedQueryOption> OnDataReceived= (sender, e) => { };
        private List<String>DataList = new List<string>();
        
        public void setData(string data)
        {
            DataList.Add(data);
        }
        
    }
}