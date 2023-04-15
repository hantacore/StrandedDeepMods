using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StrandedDeepRpgMod
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ModMapObject
    {
        [JsonProperty(PropertyName = "prefab")]
        public string prefab { get; set; }

        [JsonProperty(PropertyName = "Transform")]
        public ModDummyTransform Transform { get; set; }

        [JsonProperty(PropertyName = "rpgModUniqueId")]
        public string rpgModUniqueId { get; set; }
    }
}
