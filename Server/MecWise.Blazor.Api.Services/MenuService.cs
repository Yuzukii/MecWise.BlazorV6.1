using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MecWise.Blazor.Entities;
using MecWise.Blazor.Api.DataAccess;
using MecWise.Blazor.Api.Repositories;
using MecWise.Blazor.Common;
using Newtonsoft.Json.Linq;
using Mecwise.Messenger.CommonClasses;

namespace MecWise.Blazor.Api.Services
{
    public class MenuService
    {
        DBHelper _db;
        EPF_BLZ_MNU_PAGE_Repository _menuPageRepo;
        EPF_BLZ_MNU_ITEM_Repository _menuItemRepo;
        EPF_BLZ_MNU_SUBITEM_Repository _menuSubItemRepo;
        EPF_BLZ_MNU_SUBITEM_DTL_Repository _menuSubItemDtlRepo;


        public MenuService() {
            _db = new DBHelper(ApiSetting.ConnectionString);
            _menuPageRepo = new EPF_BLZ_MNU_PAGE_Repository(_db);
            _menuItemRepo = new EPF_BLZ_MNU_ITEM_Repository(_db);
            _menuSubItemRepo = new EPF_BLZ_MNU_SUBITEM_Repository(_db);
            _menuSubItemDtlRepo = new EPF_BLZ_MNU_SUBITEM_DTL_Repository(_db);
        }

        public MenuService(SessionState session) {
            _db = new DBHelper(session.UserID, session.CompCode, session.LangId, ApiSetting.ConnectionString);
            _menuPageRepo = new EPF_BLZ_MNU_PAGE_Repository(_db);
            _menuItemRepo = new EPF_BLZ_MNU_ITEM_Repository(_db);
            _menuSubItemRepo = new EPF_BLZ_MNU_SUBITEM_Repository(_db);
            _menuSubItemDtlRepo = new EPF_BLZ_MNU_SUBITEM_DTL_Repository(_db);
        }

        public IEnumerable<EPF_BLZ_MNU_SUBITEM> GetMenuSubItems(string userId, int langId, bool includeDetails, bool isMobile) {
            List<EPF_BLZ_MNU_SUBITEM> menuSubItems = _menuSubItemRepo.GetMenuSubItems(userId, langId, isMobile).ToList();
            if (includeDetails) {
                foreach (EPF_BLZ_MNU_SUBITEM menuSubItem in menuSubItems) {
                    menuSubItem.MENU_SUBITEM_DTLS = _menuSubItemDtlRepo.GetMenuSubItemDtls(userId, langId,
                        menuSubItem.MENU_ID, menuSubItem.MENU_ITEM_ID, menuSubItem.MENU_SUB_ITEM_ID, isMobile).ToList();
                }
            }
            return menuSubItems;
        }

        public JArray GetQuickAccessMenu(string userId, int langId, bool isMobile) {
            return _menuSubItemDtlRepo.GetQuickAccessMenu(userId, langId, isMobile);
        }

        public IEnumerable<EPF_BLZ_MNU_PAGE> GetMenuPages(string userId, int langId, bool includeDetails, bool isMobile)
        {
            List<EPF_BLZ_MNU_PAGE> menuPages = _menuPageRepo.GetMenuPages(userId, langId, isMobile).ToList();
            if (includeDetails)
            {
                foreach (EPF_BLZ_MNU_PAGE menuPage in menuPages)
                {
                    menuPage.MENU_ITEMS = _menuItemRepo.GetMenuItems(userId, langId, menuPage.MENU_ID, isMobile).ToList();
                    foreach (EPF_BLZ_MNU_ITEM menuItem in menuPage.MENU_ITEMS)
                    {
                        menuItem.MENU_SUBITEMS = _menuSubItemRepo.GetMenuSubItems(userId, langId, menuItem.MENU_ID, menuItem.MENU_ITEM_ID, isMobile).ToList();
                        foreach (EPF_BLZ_MNU_SUBITEM menuSubItem in menuItem.MENU_SUBITEMS) {
                            menuSubItem.MENU_SUBITEM_DTLS = _menuSubItemDtlRepo.GetMenuSubItemDtls(userId, langId,
                                menuSubItem.MENU_ID, menuSubItem.MENU_ITEM_ID, menuSubItem.MENU_SUB_ITEM_ID, isMobile).ToList();
                        }
                    }
                }
            }
            return menuPages;
        }

        public IEnumerable<EPF_BLZ_MNU_PAGE> GetMenuPages(string userId, int langId, bool includeDetails, decimal menuId, bool isMobile)
        {
            List<EPF_BLZ_MNU_PAGE> menuPages = _menuPageRepo.GetMenuPages(userId, langId, menuId, isMobile).ToList();
            if (includeDetails) {
                foreach (EPF_BLZ_MNU_PAGE menuPage in menuPages)
                {
                    menuPage.MENU_ITEMS = _menuItemRepo.GetMenuItems(userId, langId, menuId, isMobile).ToList();
                    foreach (EPF_BLZ_MNU_ITEM menuItem in menuPage.MENU_ITEMS)
                    {
                        menuItem.MENU_SUBITEMS = _menuSubItemRepo.GetMenuSubItems(userId, langId, menuId, menuItem.MENU_ITEM_ID, isMobile).ToList();
                    }
                }
            }
            return menuPages;
        }


        public IEnumerable<EPF_BLZ_MNU_PAGE> GetMenuPage(string userId, int langId, decimal menuId, decimal menuItemId, decimal menuSubItemId, bool isMobile) 
        {
            List<EPF_BLZ_MNU_PAGE> menuPages = _menuPageRepo.GetMenuPages(userId, langId, menuId, isMobile).ToList();
            foreach (EPF_BLZ_MNU_PAGE menuPage in menuPages)
            {
                menuPage.MENU_ITEMS = _menuItemRepo.GetMenuItems(userId, langId, menuId, menuItemId, isMobile).ToList();
                foreach (EPF_BLZ_MNU_ITEM menuItem in menuPage.MENU_ITEMS)
                {
                    menuItem.MENU_SUBITEMS = _menuSubItemRepo.GetMenuSubItems(userId, langId, menuId, menuItemId, menuSubItemId, isMobile).ToList();
                    foreach (EPF_BLZ_MNU_SUBITEM menuSubItem in menuItem.MENU_SUBITEMS)
                    {
                        menuSubItem.MENU_SUBITEM_DTLS = _menuSubItemDtlRepo.GetMenuSubItemDtls(userId, langId, menuId, menuItemId, menuSubItemId, isMobile).ToList();
                    }
                }

            }
            return menuPages;
        }

        public int GetNumberOfUnReadNoti(string compCode, string empeId) {
            string groupIds = "1905000053,1905000054,1905000055";
            WebAPIUtility messengerClient = new WebAPIUtility(_db.EPSession);
            return messengerClient.GetNumberOfUnReadByEmpe(compCode, empeId, groupIds);
        }
    }
}
