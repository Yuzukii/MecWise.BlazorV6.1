using DevExpress.Blazor;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MecWise.Blazor.Components
{
    public class FieldDxSchedulerResource : Field
    { 

        public bool popupVisible { get; set; } = false;

        public string validationMsg { get; set; } = "";

        public bool cancel { get; set; } = false;

        public string schedulerType { get; set; } = "";

        public static List<Resource> resourceList { get; set; } = new List<Resource>();

        public static List<Appointment> appointmentList { get; set; } = new List<Appointment>();

        public DxSchedulerDataStorage DataStorage { get; set; } = new DxSchedulerDataStorage();


        public FieldDxSchedulerResource() : base()
        {

        }
        public FieldDxSchedulerResource(string id, string description) : base(id, description)
        {
            this.schedulerType = description;
            this.popupVisible = false;
            this.validationMsg = "";
            this.cancel = false;
            this.schedulerType = "";
        }
        public FieldDxSchedulerResource(string id, string description, string styleClassWidth) : base(id, description, styleClassWidth)
        {

        }

        public static FieldDxSchedulerResource InitScheduler(string id, string description, List<Resource> resourceData, List<Appointment> appointmentData)
        {
            FieldDxSchedulerResource schedulerField = new FieldDxSchedulerResource(id, description);
            schedulerField.schedulerType = description;
            schedulerField.DataStorage = new DxSchedulerDataStorage()
            {
                AppointmentsSource = appointmentData,
                AppointmentMappings = new DxSchedulerAppointmentMappings()
                {
                    Id = "AppointmentId",
                    Type = "AppointmentType",
                    Start = "StartDate",
                    End = "EndDate",
                    Subject = "Caption",
                    AllDay = "AllDay",
                    Location = "Location",
                    Description = "Description",
                    LabelId = "Label",
                    StatusId = "Status",
                    RecurrenceInfo = "Recurrence",
                    ResourceId = "ResourceId",
                    CustomFieldMappings = new List<DxSchedulerCustomFieldMapping>
                    {
                        new DxSchedulerCustomFieldMapping {Name = "PackageCount", Mapping = "PackageCount" },
                        new DxSchedulerCustomFieldMapping {Name = "SrlNo", Mapping = "SrlNo"},
                        new DxSchedulerCustomFieldMapping {Name = "Capacity", Mapping = "Capacity"}
                    }
                },
                ResourcesSource = resourceData,
                ResourceMappings = new DxSchedulerResourceMappings()
                {
                    Id = "Id",
                    Caption = "Name",
                    BackgroundCssClass = "BackgroundCss",
                    TextCssClass = "TextCss"
                }
            };

            schedulerField.popupVisible = false;

            return schedulerField;
        }

        public static List<Resource> GetResources(JArray resourceData, bool ToFilter, string resourceIDs)
        {
            if (resourceData != null && !ToFilter)
            {

                ClearResources();

                foreach (var item in resourceData)
                {
                    resourceList.Add(new Resource()
                    {
                        Id = Convert.ToInt32(item["RES_ID"]),
                        Name = Convert.ToString(item["RES_NAME"]),
                        GroupId = 100,
                        BackgroundCss = "dx-green-color",
                        TextCss = "text-white"
                    });
                }
            }
            else if (resourceData != null && ToFilter)
            {
                ClearResources();

                string filterStr = "," + resourceIDs + ",";
                string containStr = "";

                foreach (var item in resourceData)
                {
                    containStr = "," + Convert.ToString(item["RES_ID"]) + ",";

                    if (filterStr.Contains(containStr))
                    {
                        resourceList.Add(new Resource()
                        {
                            Id = Convert.ToInt32(item["RES_ID"]),
                            Name = Convert.ToString(item["RES_NAME"]),
                            GroupId = 100,
                            BackgroundCss = "dx-green-color",
                            TextCss = "text-white"
                        });
                    }
                }
            }
            return resourceList;

        }

        public static List<Appointment> GetAppointments(JArray appointmentData)
        {
            if (appointmentData != null)
            {
                ClearAppointments();

                foreach (var item in appointmentData)
                {
                    appointmentList.Add(new Appointment()
                    {
                        AppointmentId = Convert.ToInt32(item["RUN_NO"]),
                        AppointmentType = Convert.ToInt32(item["APPT_TYPE"]),
                        StartDate = Convert.ToDateTime(item["APLN_DATETIME_FR"]),
                        EndDate = Convert.ToDateTime(item["APLN_DATETIME_TO"]),
                        Caption = Convert.ToString(item["BOOKING_TITLE"]),
                        AllDay = Convert.ToBoolean(item["ALL_DAY"]),
                        Description = Convert.ToString(item["BOOKING_DESC"]),
                        Label = Convert.ToInt32(item["LBL_COLOR"]),
                        Status = Convert.ToInt32(item["STATUS"]),
                        Recurrence = Convert.ToString(item["RECURRENCE"]),
                        ResourceId = Convert.ToInt32(item["RES_ID"]),
                        PackageCount = Convert.ToString(item["RSRV_CHAR_2"]),
                        SrlNo = Convert.ToString(item["SRL_NO"]),
                        Capacity = Convert.ToString(item["CAPACITY"])
                    });
                }
            }

            return appointmentList;
        }

        public static void ClearResources()
        {
            resourceList.Clear();
        }

        public static void ClearAppointments()
        {
            appointmentList.Clear();
        }

        public event EventHandler<SchedulerAppointmentOperationEventArgs> AppointmentInserting;
        internal void InvokeAppointmentInserting(object sender, SchedulerAppointmentOperationEventArgs e)
        {
            EventHandler<SchedulerAppointmentOperationEventArgs> handler = AppointmentInserting;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        public event EventHandler<DxSchedulerAppointmentItem> AppointmentInserted;
        internal void InvokeAppointmentInserted(object sender, DxSchedulerAppointmentItem e)
        {
            EventHandler<DxSchedulerAppointmentItem> handler = AppointmentInserted;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        public event EventHandler<SchedulerAppointmentOperationEventArgs> AppointmentUpdating;
        internal void InvokeAppointmentUpdating(object sender, SchedulerAppointmentOperationEventArgs e)
        {
            EventHandler<SchedulerAppointmentOperationEventArgs> handler = AppointmentUpdating;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        public event EventHandler<DxSchedulerAppointmentItem> AppointmentUpdated;
        internal void InvokeAppointmentUpdated(object sender, DxSchedulerAppointmentItem e)
        {
            EventHandler<DxSchedulerAppointmentItem> handler = AppointmentUpdated;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        public event EventHandler<SchedulerAppointmentOperationEventArgs> AppointmentRemoving;
        internal void InvokeAppointmentRemoving(object sender, SchedulerAppointmentOperationEventArgs e)
        {
            EventHandler<SchedulerAppointmentOperationEventArgs> handler = AppointmentRemoving;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        public event EventHandler<DxSchedulerAppointmentItem> AppointmentRemoved;
        internal void InvokeAppointmentRemoved(object sender, DxSchedulerAppointmentItem e)
        {
            EventHandler<DxSchedulerAppointmentItem> handler = AppointmentRemoved;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        public event EventHandler<SchedulerAppointmentFormEventArgs> AppointmentFormShowing;
        internal void InvokeAppointmentFormShowing(object sender, SchedulerAppointmentFormEventArgs e)
        {
            EventHandler<SchedulerAppointmentFormEventArgs> handler = AppointmentFormShowing;
            if (handler != null)
            {
                handler(sender, e);
            }
        }
    }

    public class Appointment
    {
        public Appointment() { }
        public int AppointmentId { get; set; }
        public int AppointmentType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Caption { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public int? Label { get; set; }
        public int Status { get; set; }
        public bool AllDay { get; set; }
        public string Recurrence { get; set; }
        public int? ResourceId { get; set; }
        public string PackageCount { get; set; }
        public string SrlNo { get; set; }
        public string Capacity { get; set; }
    }

    public class Resource
    {
        public int Id { get; set; }
        public int? GroupId { get; set; }
        public string Name { get; set; }
        public bool IsGroup { get; set; }
        public string TextCss { get; set; }
        public string BackgroundCss { get; set; }
        public override bool Equals(object obj)
        {
            Resource resource = obj as Resource;
            return resource != null && resource.Id == Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }
}
