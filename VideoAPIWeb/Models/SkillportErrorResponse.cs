using System.Collections.Generic;

namespace VideoAPIWeb.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class SkillportErrorResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SkillportErrorResponse"/> class.
        /// </summary>
        public SkillportErrorResponse()
        {
            this.messages = new List<string>();
        }

        /// <summary>
        /// Gets or sets the messages.
        /// </summary>
        /// <value>
        /// The messages.
        /// </value>
        public List<string> messages { get; set; }

    }
}