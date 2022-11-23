using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SkyDew.Tiles
{
    public class LocationsContainer : MonoBehaviour
    {
        [SerializeField] private TileGenerator generatior;
        [SerializeField] private List<TilesData> locations = new List<TilesData>();

        [Button]
        public void SaveLocation(string location)
        {
            var data = generatior.GetData(location);
            var exLoc = locations.FirstOrDefault(x => x.locationName == location);
            if (exLoc != null)
            {
                locations.Remove(exLoc);
            }

            locations.Add(data);
        }

        [Button("Load by name")]
        public void Load(string locationName)
        {
            var location = locations.FirstOrDefault(x => x.locationName == locationName);
            generatior.SetData(location);
        }
        
        [Button("Load by index")]
        public void Load(int index)
        {
            generatior.SetData(locations[index]);
        }
    }
}