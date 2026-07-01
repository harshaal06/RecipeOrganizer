using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeOrganizer.Domain.Entity
{
        public class BaseResponse
        {
            /// <summary>
            /// ResponseCode variable used to define response code for response.
            /// </summary>
            public int ResponseCode { get; set; }

            /// <summary>
            /// ReposnseMessage variable used to define Message  for response.
            /// </summary>
            public string ResponseMessage { get; set; }

            /// <summary>
            /// Timestamp variable used to define Timestamp  for response.
            /// </summary>
            private long Timestamp { get; set; }

            /// <summary>
            /// Response record count.
            /// </summary>
            public int RecordCount { get; set; }
        }
}
