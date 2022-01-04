using System;
using System.Collections.Generic;
using System.Text;

namespace MecWise.Blazor.Common
{
    public class Config
    {

        //APIService Connecion
        public string apiUrl { get; set; }
        public string clientId { get; set; } = "mecwise.blazor";
        public string clientSecret { get; set; } = "st@rvisi0n";

        //Blazor site brand text
        public string appTitle { get; set; }

        //For Navigation to V5 Website
        public string v5ConnectRoot { get; set; }
        public string v5ConnectId { get; set; }
        public string v5ConnectKey { get; set; }


        //Home Page Url
        public string homePageUrl { get; set; } = "";

        //Redirect to this URL when session expire
        public string expireRedirect { get; set; } = "";

        //Remember login user for push notification
        public bool registerPushNoti { get; set; } = false;

        //Position for Toast Message [0=top,1=left,2=bottom,3=right]
        public ToastMessageLocation toastMessagePosition { get; set; } = ToastMessageLocation.bottom;

        //Redirect to this URL when session expire
        public int toastMessageDelay { get; set; } = 1;

        //Maximum upload file size in MB (Maximum 10)
        public int maxUploadSize { get; set; } = 2;

        //Display menu on left side
        public bool sideMenuLayout { get; set; } = false;




        //****************** Mobile Settings ****************************//

        //Display AccordionView instead of GridView in mobile screen size
        public bool enableAccordionView { get; set; } = true;

        //Display menu as right drawer in mobile screen size
        public bool mobileMenuDrawer { get; set; } = true;

        //Display bottom menu in mobile screen size
        public bool enableMobileMenu { get; set; } = true;

        //Mobile bottom menu setting
        public List<MobileNavButton> MobileNavButtons { get; set; } = new List<MobileNavButton>();

       
    }

    public class MobileNavButton
    {
        public string label { get; set; } = "";
        public string iconClass { get; set; } = "";
        public string href { get; set; } = ""; 
        public bool showUnread { get; set; } = false;
    }
}
