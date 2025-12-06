using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FarmManager.Application.Common.Models.Cows
{
    public class FlaskWeightResponse
    {
        [JsonPropertyName("estimated_weight_kg")]
        public double EstimatedWeightKg { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }
    }
}
