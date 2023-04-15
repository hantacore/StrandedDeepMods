using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StrandedDeepRpgMod
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ModDummyTransform
    {
        [JsonProperty(PropertyName = "localRotation")]
        public Quaternion localRotation { get; set; }

        [JsonProperty(PropertyName = "localPosition")]
        public Vector3 localPosition { get; set; }
    }
}
