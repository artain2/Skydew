using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using AppBootstrap.Runtime;
using UnityEngine;

namespace _SkyDew.Scripts.Common
{
    public interface IDataService
    {
        string GetDataKey();
        void SetData(object data);
        object GetData();
        object CreateData();
    }

    public interface IDataSaver
    {
        void Save();
    }

    [Injectable]
    public class DataService : IDataSaver
    {
        [Inject] private List<IDataService> _dataServices;

        public const string DataFilePath = "SpaceSheepData.data";

        private Dictionary<string, object> _dataNodes = new Dictionary<string, object>();

        [Init("Load")]
        private void Load()
        {
            DeserializeContainer();
            foreach (var service in _dataServices)
            {
                var key = service.GetDataKey();
                if (!_dataNodes.TryGetValue(key, out var data))
                {
                    var newData = service.CreateData();
                    _dataNodes.Add(key, newData);
                }

                service.SetData(_dataNodes[key]);
            }
        }

        public void Save()
        {
            foreach (var service in _dataServices)
            {
                var data = service.GetData();
                var key = service.GetDataKey();
                _dataNodes[key] = data;
            }

            SerializeContainer();
        }


        void SerializeContainer()
        {
            var fs = new FileStream(DataFilePath, FileMode.Create);
            var formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fs, _dataNodes);
            }
            catch (SerializationException e)
            {
                Debug.LogError("Failed to serialize. Reason: " + e.Message);
            }
            finally
            {
                fs.Close();
            }
        }

        void DeserializeContainer()
        {
            if (!File.Exists(DataFilePath))
            {
                _dataNodes = new Dictionary<string, object>();
                return;
            }
            
            var fs = new FileStream(DataFilePath, FileMode.Open);
            try
            {
                var formatter = new BinaryFormatter();
                _dataNodes = (Dictionary<string, object>) formatter.Deserialize(fs);
            }
            catch (SerializationException e)
            {
                Debug.Log(("Failed to deserialize. Reason: " + e.Message));
                _dataNodes = new Dictionary<string, object>();
            }
            finally
            {
                fs.Close();
            }
        }
    }
}