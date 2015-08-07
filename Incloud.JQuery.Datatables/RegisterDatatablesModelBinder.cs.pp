using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Incloud.JQuery.Datatables;

[assembly: PreApplicationStartMethod(typeof($rootnamespace$.RegisterDatatablesModelBinder), "Start")]

namespace $rootnamespace$ {
    public static class RegisterDatatablesModelBinder {
        public static void Start() {
            if (!ModelBinders.Binders.ContainsKey(typeof(DataTablesParam)))
                ModelBinders.Binders.Add(typeof(DataTablesParam), new Incloud.JQuery.Datatables.DataTablesModelBinder());
        }
    }
}
