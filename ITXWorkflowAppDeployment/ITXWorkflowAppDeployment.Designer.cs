using System.Workflow.Activities;

namespace ITXWorkflowAppDeployment
{
    partial class ITXWorkflowAppDeployment
    {
        #region Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCode]
        private void InitializeComponent()
        {
            this.CanModifyActivities = true;
            System.Workflow.Runtime.CorrelationToken correlationtoken1 = new System.Workflow.Runtime.CorrelationToken();
            System.Workflow.ComponentModel.ActivityBind activitybind1 = new System.Workflow.ComponentModel.ActivityBind();
            this.Deployment = new System.Workflow.Activities.CodeActivity();
            this.onWorkflowActivated1 = new Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated();
            //
            // Deployment
            //
            this.Deployment.Name = "Deployment";
            this.Deployment.ExecuteCode += new System.EventHandler(this.Deployment_ExecuteCode);
            //
            // onWorkflowActivated1
            //
            correlationtoken1.Name = "ITXWorkflowAppDeployment";
            correlationtoken1.OwnerActivityName = "ITXWorkflowAppDeployment";
            this.onWorkflowActivated1.CorrelationToken = correlationtoken1;
            this.onWorkflowActivated1.EventName = "OnWorkflowActivated";
            this.onWorkflowActivated1.Name = "onWorkflowActivated1";
            activitybind1.Name = "ITXWorkflowAppDeployment";
            activitybind1.Path = "onWorkflowActivated1_WorkflowProperties1";
            this.onWorkflowActivated1.SetBinding(Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated.WorkflowPropertiesProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind1)));
            //
            // ITXWorkflowAppDeployment
            //
            this.Activities.Add(this.onWorkflowActivated1);
            this.Activities.Add(this.Deployment);
            this.Name = "ITXWorkflowAppDeployment";
            this.CanModifyActivities = false;
        }

        #endregion Designer generated code

        private CodeActivity Deployment;
        private Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated onWorkflowActivated1;
    }
}