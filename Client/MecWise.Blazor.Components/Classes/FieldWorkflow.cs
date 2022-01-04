using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MecWise.Blazor.Components
{

    public class FieldWorkflowActionEventArgs
    {
        public bool Cancel { get; set; } = false;

    }

    public class FieldWorkflow : Field
    {
        
        public delegate Task FieldWorkflowActionEventDelegate(object sender, FieldWorkflowActionEventArgs e);

        public event FieldWorkflowActionEventDelegate OnBeforeSubmit;
        internal async Task InvokeOnBeforeSubmitAsync(object sender, FieldWorkflowActionEventArgs e)
        {
            FieldWorkflowActionEventDelegate handler = OnBeforeSubmit;
            if (handler != null)
                await handler(sender, e);
        }



        public FieldWorkflow() : base()
        {

        }

        public FieldWorkflow(string id, string description) : base(id, description)
        {

        }

        public FieldWorkflow(string id, string description, string styleClassWidth) : base(id, description, styleClassWidth)
        {

        }

    }
}
